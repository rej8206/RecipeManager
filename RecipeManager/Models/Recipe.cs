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


        public int RecipeId { get; set; }
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
            recipeListCommand.CommandText = "SELECT * FROM RecipeLists JOIN Recipes on RecipeLists.RecipeId = Recipes.RecipeId";
           

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
                            RecipeId = Convert.ToInt32(Reader["RecipeId"]),
                            RecipeName = Convert.ToString(Reader["RecipeName"]),
                            Instructions = Convert.ToString(Reader["Instructions"]),
                            //Image = Convert.To
                            Servings = Convert.ToInt16(Reader["Servings"]),
                            SourceName = Convert.ToString(Reader["SourceName"]),
                            MinutesToMake = Convert.ToInt16(Reader["MinutesToMake"])
               
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

        public static Recipe SelectRecipe(int RecipeId)
        {
            Recipe output = new Recipe();
            MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);

           
            string CommandText = "SELECT * FROM Recipes WHERE RecipeId = @RecipeId;";
            MySqlCommand command = new MySqlCommand(CommandText);

            command.Parameters.AddWithValue("@RecipeId", RecipeId);



            try
            {

                connection.Open();
                MySqlDataReader Reader = command.ExecuteReader();
                if (Reader.Read())
                {
                    
                        var recipe = new Recipe()
                        {
                            RecipeId = Convert.ToInt32(Reader["RecipeId"]),
                            RecipeName = Convert.ToString(Reader["RecipeName"]),
                            Instructions = Convert.ToString(Reader["Instructions"]),
                            //Image = Convert.To
                            Servings = Convert.ToInt16(Reader["Servings"]),
                            SourceName = Convert.ToString(Reader["SourceName"]),
                            MinutesToMake = Convert.ToInt16(Reader["MinutesToMake"])

                        };
                        output=recipe;
          


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


