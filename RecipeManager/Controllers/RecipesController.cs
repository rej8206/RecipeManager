using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RecipeManager.Controllers
{
    public class RecipesController : Controller
    {
        // GET: Recipes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Recipe (int RecipeId)
        {
            Recipe model = RecipeDb.SelectRecipe(RecipeId);

            return View(model);
        }

        public ActionResult Search()
        {
            return View();
        }
        public ActionResult ShoppingList()
        {
            return View();
        }
        public ActionResult UserRecipes()
        {
            RecipeListViewModel model;
            model = new RecipeListViewModel();
            return View(model);
        }
    }
}