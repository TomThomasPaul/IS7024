using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomsSpaghetti.Models
{
    public class Recipe
    {
        public string RecipeId { get; set; }

        public string Title { get; set; }
        public string Author { get; set; } 
        public string Image { get; set; }
        public string Url { get; set; }
        public Array Ingredients { get; set; }
    }
}
