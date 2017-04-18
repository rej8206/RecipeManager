using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeManager.Models
{
    public class RecipeViewModel
    {
       
        public Recipe r {get; set;}
       // public int RecipeId { get; set;}
        public RecipeViewModel(int RecipeId)
        {
            r = RecipeDb.SelectRecipe(RecipeId);
        }
    }
}