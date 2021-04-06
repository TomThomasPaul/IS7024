using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomsSpaghetti.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace MomsSpaghetti.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache _cache;
     
        private readonly ILogger<IndexModel> _logger;

        [BindProperty(SupportsGet =true)]
        public UserSearch SearchRecipes { get; set; }

        [BindProperty(SupportsGet =true)]
        public Recipe Recipe { get; set; }

        [BindProperty(SupportsGet = true)]
        public Like Like { get; set; }

        [BindProperty(SupportsGet = true)]
        public string RecipeId { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }


        public async Task<IActionResult> OnGet()
        {
            //Runs when the page is loaded for the first time when the searchRecepients doesn't have a value
            if (_cache.Get("searchRecipeResults") == null)
            {
                return Page();
            }


            if (_cache.Get("searchRecipeResults") != null)
            {

                //ViewData["recipe"] = SearchRecipes.Result["recipes"][0]["title"];
                //var temp = TempData["searchResultsString"];
                var cachedResult = _cache.Get<JObject>("searchRecipeResults");
                SearchRecipes.Result = cachedResult;

            }
            //Loads the Recipe that selected 
            if (RecipeId != null)
            {
                string RecipeDetails = "";
                // var recipeObject = new { };
                try
                {
                    HttpClient client = new HttpClient();

                    // client.DefaultRequestHeaders.Add("q", UserInput);

                    HttpResponseMessage response = await client.GetAsync("https://forkify-api.herokuapp.com/api/get?rId=" + RecipeId);


                    var temp = response;

                    if (response.IsSuccessStatusCode)

                    {

                        RecipeDetails = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(RecipeDetails);
                        JSchema schema = JSchema.Parse(System.IO.File.ReadAllText("Recipeschema.json"));
                        IList<string> validationEvents = new List<string>();
                        if (jsonObject.IsValid(schema, out validationEvents))
                        {

                            Recipe.RecipeId = RecipeId;
                            Recipe.Title = jsonObject["recipe"]["title"].ToString();
                            Recipe.Image = jsonObject["recipe"]["image_url"].ToString();
                            Recipe.Author = jsonObject["recipe"]["publisher"].ToString();
                            Recipe.Url = jsonObject["recipe"]["source_url"].ToString();
                            Recipe.Ingredients = jsonObject["recipe"]["ingredients"].ToArray();


                            //UserSearch SearchRecipes = new UserSearch();
                            // recipeObject = jsonObject;
                            // ViewData["result"] = SearchRecipes.Result["recipes"][0]["title"];

                            /*  var props = SearchRecipes.Result.Properties();
                             var  propsArray = props.ToArray();
                              var first = propsArray[1].Value[0]["title"];  //Upto VALUE[0] gives the first recipe object


                              */

                        }


                        else
                        {
                            foreach (string evt in validationEvents)
                            {
                                Console.WriteLine(evt);

                            }
                            // ViewData["result"] = new List<result>();
                        }


                    }
                  

                }
                catch
                {
                   
                    return Page();
                }

            }

            if (_cache.Get<List<JObject>>("LikedRecipes") != null)
            {
                ViewData["LikedRecipes"] = _cache.Get<List<JObject>>("LikedRecipes");

            }

            return Page();
        }

        public ActionResult OnGetLiked()
        {
            var response = "false";
            var recipeId =Request.Query["recipeId"];
            var cachedResult = _cache.Get<List<JObject>>("LikedRecipes");
            if (cachedResult != null) {
                var item = cachedResult.SingleOrDefault(x => x["id"].ToString().Equals(recipeId));
                if (item != null)
                {
                    var itemJSON = JsonConvert.SerializeObject(item);
                    return new JsonResult(itemJSON);
                }




            }



            return new JsonResult(response);
        }

        public  ActionResult OnPostSend()
        {


            // JObject returnTemp = null;
            var LikedRecipes = new List<JObject>();
            string requestBody;
            string likedRecipesJSON="";
                MemoryStream stream = new MemoryStream();
                Request.Body.CopyTo(stream);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                     requestBody = reader.ReadToEnd();
                    if (requestBody.Length > 0)
                    {
                        var obj = JObject.Parse(requestBody);
                        if (obj != null)
                        {
                        // var temp = Like.Likes;
                        // var  myList = new List<Object>();
                        var cachedResult = _cache.Get<List<JObject>>("LikedRecipes");
                        if (Like.Likes == null && cachedResult == null) {
                            Like.Likes = new List<JObject>();
                            Like.Likes.Add(obj);
                            _cache.Set<List<JObject>>("LikedRecipes", Like.Likes);
                        }
                        else {
                            Like.Likes = _cache.Get<List<JObject>>("LikedRecipes");
                            Like.Likes.Add(obj);
                            _cache.Set<List<JObject>>("LikedRecipes", Like.Likes);
                        }
                       
                        
                       // ViewData["LikedRecipes"] = _cache.Get<List<JObject>>("LikedRecipes");
                       LikedRecipes = _cache.Get<List<JObject>>("LikedRecipes");
                        
                        likedRecipesJSON = JsonConvert.SerializeObject(LikedRecipes);
                        // returnTemp = temp;
                        //foreach (var recipe in temp)
                        //{
                        //    var image = recipe["image"];

                        //        }
                        //Like.Likes.SetValue(obj, Like.Likes.Length);
                        // Like.Likes.SetValue(obj, 0);
                    }
                    }
                }

            
            
            return new JsonResult(likedRecipesJSON);
        }

        //Removes a recipe from Favorites
        public ActionResult OnPostDeleteLiked()
        {


            // JObject returnTemp = null;
            //var likedRecipes = new List<JObject>();
            string requestBody;
            string objJSON = "";
            MemoryStream stream = new MemoryStream();
            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    var obj = JObject.Parse(requestBody);
                    if (obj != null)
                    {
                        // var temp = Like.Likes;
                        // var  myList = new List<Object>();
                        // var cachedResult = _cache.Get<List<JObject>>("LikedRecipes");
                        Console.WriteLine(Like.Likes);
                        Like.Likes = _cache.Get<List<JObject>>("LikedRecipes");
                        
                        Like.Likes.Remove(Like.Likes.SingleOrDefault(x => x["id"].ToString().Equals(obj["id"].ToString())));
                        Console.WriteLine(Like.Likes);
                        _cache.Set<List<JObject>>("LikedRecipes", Like.Likes);
                       objJSON = JsonConvert.SerializeObject(Like.Likes);


                    }
                }
            }



            return new JsonResult(objJSON);
        }


        //User searches for a recipe
        public async Task<IActionResult>  OnPost()
        {   
            string ResponseContent="";
            var ResponseObject = new { };
            if (SearchRecipes.Query.Length != 0)
            {
                
                try
                {
                    HttpClient client = new HttpClient();

                    // client.DefaultRequestHeaders.Add("q", UserInput);

                    HttpResponseMessage response = await client.GetAsync("https://forkify-api.herokuapp.com/api/search?q="+ SearchRecipes.Query);

                    if (response.IsSuccessStatusCode)
                    {
                       ResponseContent = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(ResponseContent);
                        //TempData["searchResultsString"] = responseContent;
                        //UserSearch SearchRecipes = new UserSearch();
                        SearchRecipes.Result = jsonObject;
                        // ViewData["result"] = SearchRecipes.Result["recipes"][0]["title"];
                       /* var cacheEntry = _cache.CreateEntry("searchRecipeResults");
                        cacheEntry.SetValue(SearchRecipes.Result);
                        cacheEntry.Dispose(); //adds item to cache
                       */

                        _cache.Set<JObject>("searchRecipeResults", SearchRecipes.Result);
                        /*  var props = SearchRecipes.Result.Properties();
                         var  propsArray = props.ToArray();
                          var first = propsArray[1].Value[0]["title"];  //Upto VALUE[0] gives the first recipe object


                          */



                    }
                }
                catch(Exception err)
                {
                    return RedirectToPage("Error", err.Message);

                }

            }



            // return RedirectToPage("/ResultsPage", new { result = SearchRecipes.Result }) ;
            //return Content(responseContent);

          if (_cache.Get<List<JObject>>("LikedRecipes") != null)
            {
                ViewData["LikedRecipes"] = _cache.Get<List<JObject>>("LikedRecipes");

            }
            return Page();


        }
    }
}
