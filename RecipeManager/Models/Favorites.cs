using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//corresponds with RecipeLists table
namespace RecipeManager.Models
{
    public class Favorites
    {
        public int UserId { get; set; }
        public int RecipeId { get; set; }
    }
}