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
using System.Text.RegularExpressions;
using System.Linq;
using Grpc.Core;

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
        public String recipeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Boolean getMostSearched { get; set; }

        [BindProperty(SupportsGet = true)]
        public Boolean getSchema { get; set; }


        public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> OnGet()
        {


            if (_cache.Get("searchRecipeResults") != null)
            {
                var cachedResult = _cache.Get<JObject>("searchRecipeResults");
                SearchRecipes.Result = cachedResult;

            }

            if (getMostSearched == true || getSchema ==true)
            {
                string fileName = "";
                if(getMostSearched == true)
                {
                    fileName = "Data_Feed.json";
                }
                else
                {
                    fileName = "dataFeedSchema.json";
                }
                Console.WriteLine("API hit");
                using (StreamReader r = new StreamReader(fileName))
                {

                    var json = r.ReadToEnd();

                    if (json != "")
                    {

                        r.Close();
                        return Content(json);

                    }
                    else
                    {
                        r.Close();
                        return new JsonResult("No visited records");
                    }

                    //result = jobj.ToString();
                    //Console.WriteLine(result);
                }



            }

            if (recipeId != null)
            {
                String recipeDetails = "";
               
                try
                {
                    HttpClient client = new HttpClient();
                    
                    HttpResponseMessage response = await client.GetAsync("https://forkify-api.herokuapp.com/api/get?rId=" + recipeId);

                    if (response.IsSuccessStatusCode)

                    {

                        recipeDetails = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(recipeDetails);
                        JSchema schema = JSchema.Parse(System.IO.File.ReadAllText("Recipeschema.json"));
                        IList<string> validationEvents = new List<string>();
                        if (jsonObject.IsValid(schema, out validationEvents))
                        {

                            Recipe.RecipeId = recipeId;
                            Recipe.Title = jsonObject["recipe"]["title"].ToString();
                            Recipe.Image = jsonObject["recipe"]["image_url"].ToString();
                            Recipe.Author = jsonObject["recipe"]["publisher"].ToString();
                            Recipe.Url = jsonObject["recipe"]["source_url"].ToString();
                            Recipe.Ingredients = jsonObject["recipe"]["ingredients"].ToArray();
                            var i = 0;
                            foreach (var ingredient in Recipe.Ingredients)
                            {
                               
                                var ingArr = ingredient.ToString().Split(" ");
                                if (!Regex.IsMatch(ingArr[0], @"^\d+"))
                                {
                                    var updatedIng = string.Join(" ", ingArr.Prepend("1"));

                                    var temp = Recipe.Ingredients.GetValue(i);
                                    var typ = temp.GetType();

                                    JObject obj = new JObject(new JProperty("updatedIng", updatedIng));

                                    Recipe.Ingredients.SetValue(obj["updatedIng"],i);
                                        
                                        
                                }
                                i++;
                            }
                            Recipe.CookingTime = Recipe.Ingredients.Length * 5;

                            string filepath = "Data_Feed.json";
                            Boolean writeFile = false;
                           
                            var mostSearched = new List<JObject>();
                            var mostSearchedJSON = "";
                            if (_cache.Get<List<JObject>>("mostSearched") == null)
                            {
                                
                                mostSearched.Add(jsonObject);
                                _cache.Set<List<JObject>>("mostSearched", mostSearched);

                            }
                            else
                            {

                                mostSearched = _cache.Get<List<JObject>>("mostSearched");

                                if (mostSearched.SingleOrDefault(x => x["recipe"]["recipe_id"].ToString().Equals(Recipe.RecipeId)) == null)
                                {
                                    mostSearched.Add(jsonObject);

                                }
                                

                                
                            }
                            
                            mostSearchedJSON = JsonConvert.SerializeObject(mostSearched);

                            using (StreamReader r = new StreamReader(filepath))
                            {
                                
                                var json = r.ReadToEnd();
                                
                                if (json != "") {
                                    var jobj = JArray.Parse(json);
                                
                                   
                                   
                                    
                                    foreach (var item in jobj)
                                        {
                                       
                                        var recId = item["recipe"]["recipe_id"];
                                        if (!Recipe.RecipeId.Equals(item["recipe"]["recipe_id"].ToString()))
                                        {
                                            JObject obj = new JObject();
                                           

                                            if (mostSearched.SingleOrDefault(x => x["recipe"]["recipe_id"].ToString().Equals(Recipe.RecipeId)) == null)
                                            {
                                                mostSearched.Add(JObject.Parse(item.ToString()));
                                                mostSearchedJSON = JsonConvert.SerializeObject(mostSearched);

                                            }
                                          
                                            writeFile = true;

                                        }
                                    }


                                }
                                else
                                {
                                    writeFile = true;
                                }
                             
                                //result = j obj.ToString();
                                //Console.WriteLine(result);
                            }

                            if (writeFile)
                            {
                               // System.IO.File.AppendAllText(filepath, ",");
                                System.IO.File.WriteAllText(filepath, mostSearchedJSON);

                            }




                        }



                        else
                        {
                            foreach (string evt in validationEvents)
                            {
                                Console.WriteLine(evt);

                            }
                            
                        }


                    }
                  

                }
                catch (Exception err)
                {
                    
                    Console.WriteLine(err);

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

        public ActionResult OnGetSearchResults()
        {


            var cachedResult = _cache.Get<JObject>("searchRecipeResults");
            return new JsonResult(JsonConvert.SerializeObject(cachedResult));


        }

        public  ActionResult OnPostSend()
        {
            var likedRecipes = new List<JObject>();
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
                        var temp = Like.Likes;
                        if (obj != null)
                        {
                      
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
                       
                        
                      
                       likedRecipes = _cache.Get<List<JObject>>("LikedRecipes");
                        
                        likedRecipesJSON = JsonConvert.SerializeObject(likedRecipes);
          
                    }
                    }
                }

            
            
            return new JsonResult(likedRecipesJSON);
        }


        public ActionResult OnPostDeleteLiked()
        {
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
          
                        Console.WriteLine(Like.Likes);
                        Like.Likes = _cache.Get<List<JObject>>("LikedRecipes");
                        var item = Like.Likes.SingleOrDefault(x => x["id"].ToString().Equals(obj["id"].ToString()));
                        Like.Likes.Remove(item);
                        Console.WriteLine(Like.Likes);
                        _cache.Set<List<JObject>>("LikedRecipes", Like.Likes);
                       objJSON = JsonConvert.SerializeObject(Like.Likes);


                    }
                }
            }



            return new JsonResult(objJSON);
        }


    
        public async Task<IActionResult>  OnPost()
        {   
            String responseContent="";
            var responseObject = new { };
             if (SearchRecipes.Query.Length != 0)
            {
                
                try
                {
                    HttpClient client = new HttpClient();

                    HttpResponseMessage response = await client.GetAsync("https://forkify-api.herokuapp.com/api/search?q="+ SearchRecipes.Query);

                    if (response.IsSuccessStatusCode)
                    {
                       responseContent = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(responseContent);
                        SearchRecipes.Result = jsonObject;

                        _cache.Set<JObject>("searchRecipeResults", SearchRecipes.Result);
                    }
                }
                catch(Exception err)
                {
                    return RedirectToPage("Error", err.Message);

                }

            }

            if (_cache.Get<List<JObject>>("LikedRecipes") != null)
            {
                ViewData["LikedRecipes"] = _cache.Get<List<JObject>>("LikedRecipes");

            }
            return Page();


        }
    }
}
