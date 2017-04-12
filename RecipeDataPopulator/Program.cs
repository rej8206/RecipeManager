using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeDataPopulator
{
    class Program
    {
        static HttpClient _client = new HttpClient();
        static string _getPath = "recipes/{id}/information?includeNutrition=false";
        static string _apiKey = "SuPrDHDXwTmshHsn88i0dJQSzz6ep1aLKZVjsndTQvVuSOADks";
        static string _rawDataPath = "RawRecipes.txt";
        static string _sqlInsertRecipesPath = "..\\..\\..\\CSC455RecipeManager\\SQL\\Initialization\\InsertRecipes.sql";
        static string _sqlInsertRecipePartsPath = "..\\..\\..\\CSC455RecipeManager\\SQL\\Initialization\\InsertRecipeParts.sql";
        static string _sqlInsertMeasurementsPath = "..\\..\\...\\CSC455RecipeManager\\SQL\\Initialization\\InsertMeasurements.sql";
        static Regex stringCleanRegex = new Regex("[\"\\\\]");
        static int _maxId = 500000;
        static int _numOfRecipesToExtract = 100;

        static void Main(string[] args)
        {
            ExtractRecipes(_numOfRecipesToExtract);

            Console.WriteLine("\nDone");

            Console.ReadKey();
        }

        static void ExtractRecipes(int count)
        {
            InitClient();
            Random random = new Random();

            //Regex firstAttrRegex = new Regex("VALUES \\(\\s*(\\d+)");
            int recipeId = 0;
            List<int> usedIds = new List<int>();
            if (File.Exists(_sqlInsertRecipesPath))
            {
                using (StreamReader reader = new StreamReader(_sqlInsertRecipesPath))
                {
                    bool nextIsId = false;
                    Regex idRegex = new Regex("\\d+");
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (nextIsId)
                        {
                            recipeId = Int32.Parse(idRegex.Match(line).Value);
                            usedIds.Add(recipeId);
                            nextIsId = false;
                        }
                        else if (line.StartsWith("INSERT INTO "))
                        {
                            nextIsId = true;
                        }
                    }
                }
            }
            List<string> measureNames = new List<string>();
            if (File.Exists(_sqlInsertMeasurementsPath))
            {
                using (StreamReader reader = new StreamReader(_sqlInsertMeasurementsPath))
                {
                    bool nextIsName = false;
                    Regex nameRegex = new Regex("\\w+");
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (nextIsName)
                        {
                            string name = nameRegex.Match(line).Value;
                            measureNames.Add(name);
                            nextIsName = false;
                        }
                        else if (line.StartsWith("INSERT INTO "))
                        {
                            nextIsName = true;
                        }
                    }
                }
            }

            StreamWriter insertRecipesWriter = new StreamWriter(_sqlInsertRecipesPath, true);
            StreamWriter insertRecipePartsWriter = new StreamWriter(_sqlInsertRecipePartsPath, true);
            StreamWriter insertMeasurementsWriter = new StreamWriter(_sqlInsertMeasurementsPath, true);
            StreamWriter rawDataWriter = new StreamWriter(_rawDataPath, true);

            for (int i = 0; i < count; ++i)
            {
                int id = random.Next(_maxId);
                if (usedIds.Contains(id))
                    continue;

                var getRecipeTask = GetRecipeAsync(MakeGetRecipePath(id));
                getRecipeTask.Wait();
                Recipe recipe = getRecipeTask.Result;

                if (recipe == null || String.IsNullOrWhiteSpace(recipe.Title))
                {
                    Console.WriteLine("<DOES NOT EXIST>");
                    continue;
                }

                string insertRecipeStatement = MakeInsertStatement(
                    "Recipes",
                    recipe.Id,
                    recipe.Title,
                    PrepString(recipe.Instructions),
                    PrepString(recipe.Image),
                    recipe.Servings,
                    PrepString(recipe.SourceName),
                    recipe.ReadyInMinutes);
                insertRecipesWriter.WriteLine(insertRecipeStatement);

                int recipePartNumber = 0;
                foreach (ExtendedIngredient ingredient in recipe.ExtendedIngredients)
                {
                    if (!measureNames.Contains(ingredient.Unit))
                    {
                        string insertMeasurementStatement = MakeInsertStatement(
                            "Measurements",
                            PrepString(ingredient.Unit),
                            PrepString(ingredient.UnitShort));
                        insertMeasurementsWriter.WriteLine(insertMeasurementStatement);
                        measureNames.Add(ingredient.Unit);
                    }

                    string insertRecipePartStatement = MakeInsertStatement(
                        "RecipeParts",
                        recipe.Id,
                        recipePartNumber,
                        PrepString(ingredient.Name),
                        ingredient.Amount,
                        PrepString(ingredient.Unit),
                        PrepString(ingredient.OriginalString));
                    insertRecipePartsWriter.WriteLine(insertRecipePartStatement);

                    ++recipePartNumber;
                }
                usedIds.Add(recipe.Id);

                rawDataWriter.WriteLine();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Recipe));
                ser.WriteObject(rawDataWriter.BaseStream, recipe);

                Console.WriteLine(recipe.Title);

                insertRecipesWriter.Flush();
                insertRecipePartsWriter.Flush();
                insertMeasurementsWriter.Flush();
                rawDataWriter.Flush();
            }

            insertRecipesWriter.Close();
            insertRecipePartsWriter.Close();
            insertMeasurementsWriter.Close();
            rawDataWriter.Close();
        }

        static string MakeGetRecipePath(int id)
        {
            return _getPath.Replace("{id}", id.ToString());
        }

        static string MakeInsertStatement(string table, params object[] values)
        {
            StringBuilder statement = new StringBuilder("INSERT INTO " + table + " VALUES (");
            foreach (object value in values)
            {
                statement.Append("\n\t");
                if (value is string)
                {
                    if (String.Compare((string)value, 0, "NULL", 0, 5, true) == 0)
                        statement.AppendFormat("NULL");
                    else
                        statement.AppendFormat("\"{0}\"", value?.ToString() ?? "NULL");
                }
                else
                {
                    statement.Append(value?.ToString() ?? "NULL");
                }
                statement.Append(",");
            }
            statement.Remove(statement.Length - 1, 1);
            statement.Append(");");

            return statement.ToString();
        }

        static void InitClient()
        {
            _client.BaseAddress = new Uri("https://spoonacular-recipe-food-nutrition-v1.p.mashape.com/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-Mashape-Key", _apiKey);
        }

        static async Task<Recipe> GetRecipeAsync(string path)
        {
            Recipe recipe = null;
            HttpResponseMessage response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                Stream content = await response.Content.ReadAsStreamAsync();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Recipe));
                recipe = (Recipe)ser.ReadObject(content);
            }
            else
            {
                Console.WriteLine("<Unsuccessful response: {0}>", response.StatusCode);
            }
            return recipe;
        }

        static string PrepString(string s)
        {
            if (s == null)
                return "NULL";

            return stringCleanRegex.Replace(s, "\\$0"); // escape all double quotes and backslashes
        }
    }

    [DataContract(Name = "rootObject")]
    public class Recipe
    {
        [DataMember(Name = "vegetarian")]
        public bool Vegetarian { get; set; }
        [DataMember(Name = "vegan")]
        public bool Vegan { get; set; }
        [DataMember(Name = "glutenFree")]
        public bool GlutenFree { get; set; }
        [DataMember(Name = "dairyFree")]
        public bool DairyFree { get; set; }
        [DataMember(Name = "veryHealthy")]
        public bool VeryHealthy { get; set; }
        [DataMember(Name = "cheap")]
        public bool Cheap { get; set; }
        [DataMember(Name = "veryPopular")]
        public bool VeryPopular { get; set; }
        [DataMember(Name = "sustainable")]
        public bool Sustainable { get; set; }
        [DataMember(Name = "weightWatcherSmartPoints")]
        public int WeightWatcherSmartPoints { get; set; }
        [DataMember(Name = "gaps")]
        public string Gaps { get; set; }
        [DataMember(Name = "lowFodmap")]
        public bool LowFodmap { get; set; }
        [DataMember(Name = "ketogenic")]
        public bool Ketogenic { get; set; }
        [DataMember(Name = "whole30")]
        public bool Whole30 { get; set; }
        [DataMember(Name = "servings")]
        public int Servings { get; set; }
        [DataMember(Name = "sourceUrl")]
        public string SourceUrl { get; set; }
        [DataMember(Name = "spoonacularSourceUrl")]
        public string SpoonacularSourceUrl { get; set; }
        [DataMember(Name = "aggregateLikes")]
        public int AggregateLikes { get; set; }
        [DataMember(Name = "creditText")]
        public string CreditText { get; set; }
        [DataMember(Name = "sourceName")]
        public string SourceName { get; set; }
        [DataMember(Name = "extendedIngredients")]
        public ExtendedIngredient[] ExtendedIngredients { get; set; }
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "readyInMinutes")]
        public int ReadyInMinutes { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "imageType")]
        public string ImageType { get; set; }
        [DataMember(Name = "instructions")]
        public string Instructions { get; set; }
    }

    [DataContract(Name = "extendedIngredient")]
    public class ExtendedIngredient
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "aisle")]
        public string Aisle { get; set; }
        [DataMember(Name = "image")]
        public string Image { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "amount")]
        public float Amount { get; set; }
        [DataMember(Name = "unit")]
        public string Unit { get; set; }
        [DataMember(Name = "unitShort")]
        public string UnitShort { get; set; }
        [DataMember(Name = "unitLong")]
        public string UnitLong { get; set; }
        [DataMember(Name = "originalString")]
        public string OriginalString { get; set; }
        [DataMember(Name = "metaInformation")]
        public string[] MetaInformation { get; set; }
    }

}
