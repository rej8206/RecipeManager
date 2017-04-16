using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RecipeManager.Models
{
    public class Recipe
    {
        public int RecpieId { get; set; }
        public string RecipeName { get; set; }
        public string Instructions { get; set; } //may change later
        public string Image { get; set; }
        public int Servings { get; set; }
        public string SourceName { get; set; }
        public int MinutesToMake { get; set; }
    }

    public static class RecipeDb
    {
        public static List<Recipe> SelectUserRecipes()
        {


            List<Recipe> output = new List<Recipe>();
            MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);
            
            MySqlCommand recipeListCommand = connection.CreateCommand();
            recipeListCommand.CommandText = "SELECT RecipeName FROM RecipeLists JOIN Recipes on RecipeLists.RecipeId = Recipes.RecipeId";
           

            try
            {

                connection.Open();
                MySqlDataReader Reader = recipeListCommand.ExecuteReader();
                if (Reader.Read())
                {
                    do
                    {
                        var recipe = new Recipe()
                        {
                            //RecpieId = Convert.ToInt16(Reader["RecipeId"]),
                            RecipeName = Convert.ToString(Reader["RecipeName"]),
                            //Instructions = Convert.ToString(Reader["Instructions"]),
                            //Image = Convert.To
                            //Servings = Convert.ToInt16(Reader["Servings"]),
                            //SourceName = Convert.ToString(Reader["SourceName"]),
                            //MinutesToMake = Convert.ToInt16(Reader["MinutesToMake"])
               
                        };
                        output.Add(recipe);
                    } while (Reader.Read());


                }
            }
            catch (MySqlException ex)
            { }
            finally

            {
                connection.Close();
            }
            return output;   
        }


    }

}
