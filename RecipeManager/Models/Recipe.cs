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
        public Uri Image { get; set; }
        public int Servings { get; set; }
        public string SourceName { get; set; }
        public int MinutesToMake { get; set; }
    }

    public static class RecipeDb
    {
        public static List<Recipe> SelectUserRecipes()
        {


            List<Recipe> output = new List<Recipe>();
            MySqlConnection connection = MySqlProvider.Connection; //new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);

            MySqlCommand recipeListCommand = connection.CreateCommand();
            recipeListCommand.CommandText = "SELECT * FROM UserRecipeList JOIN Recipes on UserRecipeList.RecipeId = Recipes.RecipeId"; //"SELECT RecipeName FROM RecipeLists JOIN Recipes on RecipeLists.RecipeId = Recipes.RecipeId";


            try
            {

                //connection.Open();
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
                            Image = new Uri(Convert.ToString(Reader["Image"])),
                            Servings = Convert.ToInt16(Reader["Servings"]),
                            SourceName = Convert.ToString(Reader["SourceName"]),
                            MinutesToMake = Convert.ToInt16(Reader["MinutesToMake"])

                        };
                        output.Add(recipe);
                    } while (Reader.Read());


                }
            }
            catch (MySqlException ex)
            {
                output.Add(new Recipe() { RecipeName = ex.Message });
            }
            //finally
            //{
            //    connection.Close();
            //}
            return output;
        }



        public static Recipe SelectRecipe(int id)
        {

            Recipe output = new Recipe();
            MySqlConnection connection = MySqlProvider.Connection; //new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);

            MySqlCommand Command = connection.CreateCommand();
            
            Command.Parameters.AddWithValue("@param1", id);
            Command.CommandText = "SELECT * FROM UserRecipeList WHERE UserRecipes.RecipeId = @param1"; //"SELECT RecipeName FROM RecipeLists JOIN Recipes on RecipeLists.RecipeId = Recipes.RecipeId";


            try
            {

                //connection.Open();
                MySqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {

                    var recipe = new Recipe()
                    {
                        RecipeId = Convert.ToInt32(Reader["RecipeId"]),
                        RecipeName = Convert.ToString(Reader["RecipeName"]),
                        Instructions = Convert.ToString(Reader["Instructions"]),
                        Image = new Uri(Convert.ToString(Reader["Iamge"])),
                        Servings = Convert.ToInt16(Reader["Servings"]),
                        SourceName = Convert.ToString(Reader["SourceName"]),
                        MinutesToMake = Convert.ToInt16(Reader["MinutesToMake"])

                    };
                    output = recipe;



                }
            }
            catch (MySqlException ex)
            {
                // output.Add(new Recipe() { RecipeName = ex.Message });
            }
            //finally
            //{
            //    connection.Close();
            //}
            return output;

        }
    }

}
