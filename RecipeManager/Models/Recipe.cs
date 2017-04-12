using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeManager.Models
{
    public class Recipe
    {
        public int RecipieId { get; set; }
        public string RecipeName { get; set; }
        public string Instructions { get; set; } //may change later
        public string Image { get; set; }
        public int Servings { get; set; }
        public string SourceName { get; set; }
        public int MinutesToMake { get; set; }
    }

    public static class RecipeDb
    {
    }

}
