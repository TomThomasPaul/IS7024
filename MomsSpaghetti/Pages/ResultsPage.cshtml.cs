using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace MomsSpaghetti.Pages
{
    public class ResultsPageModel : PageModel
    { 
        [BindProperty(SupportsGet =true)]
        public JObject Result { get; set; }
        public void OnGet(JObject result)
        {
            Result = result;
        }
    }
}
