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
        public String recipeId { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> OnGet()
        { 
          
            
            if(_cache.Get("searchRecipeResults") != null)
            {

                //ViewData["recipe"] = SearchRecipes.Result["recipes"][0]["title"];
                //var temp = TempData["searchResultsString"];
               var cachedResult= _cache.Get<JObject>("searchRecipeResults");
                SearchRecipes.Result = cachedResult;

            }

            if (recipeId != null)
            {
                String recipeDetails = "";
               // var recipeObject = new { };
                try
                { 
                    HttpClient client = new HttpClient();

                    // client.DefaultRequestHeaders.Add("q", UserInput);

                    HttpResponseMessage response = await client.GetAsync("https://forkify-api.herokuapp.com/api/get?rId=" + recipeId);

                    var temp = response;

                    if (response.IsSuccessStatusCode)
                    {
                        recipeDetails = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(recipeDetails);
                        Recipe.RecipeId = recipeId;
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

                }
                catch (Exception err)
                {
                    //return RedirectToPage("Error", err.Message);

                }
            }

            return Page();
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

                    // client.DefaultRequestHeaders.Add("q", UserInput);

                    HttpResponseMessage response = await client.GetAsync("https://forkify-api.herokuapp.com/api/search?q="+ SearchRecipes.Query);

                    if (response.IsSuccessStatusCode)
                    {
                       responseContent = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(responseContent);
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
            return Page();


        }
    }
}
