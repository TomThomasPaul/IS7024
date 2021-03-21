using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomsSpaghetti.Models
{
    public class Recipe
    {
        public String RecipeId { get; set; }

        public String Title { get; set; }
        public String Author { get; set; } 
        public String Image { get; set; }
        public String Url { get; set; }
        public Array Ingredients { get; set; }
    }
}
