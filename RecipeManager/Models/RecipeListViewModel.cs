using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeManager.Models
{
    public class RecipeListViewModel
    {
        public List<Recipe> Recipes { get; set; }
        public RecipeListViewModel()
        {
            Recipes = RecipeDb.SelectUserRecipes();
        }
    }
}