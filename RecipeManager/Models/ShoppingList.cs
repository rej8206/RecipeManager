using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeManager.Models
{
    public class ShoppingList
    {
        public int UserId { get; set; }
        public string IngName { get; set; }
        public int PantryAmount { get; set; }
        public string MeasureName { get; set; }
    }
}