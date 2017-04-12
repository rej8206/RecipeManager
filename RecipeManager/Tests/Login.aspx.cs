using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace CSC455RecipeManager
{
    public partial class Login : System.Web.UI.Page
    {
        private static readonly Regex UsernameRegex = new Regex("^\\w+$");
        private static readonly Regex PasswordSanitationRegex = new Regex("(['\"])");
        private static readonly int MaxPasswordLength = 100;
        private static string ValidResult = "True";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SignInButton_Clicked(object sender, EventArgs e)
        {
            if (!UsernameRegex.IsMatch(UsernameBox.Text))
            {
                ShowInvalidResult();
                return;
            }
            if (PasswordBox.Text.Length > MaxPasswordLength)
            {
                ShowInvalidResult();
                return;
            }

            RecipeListBox.Items.Clear();

            try
            {
                MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);
                connection.Open();

                MySqlCommand verifyCommand = connection.CreateCommand();
                String sanitizedPassword = PasswordSanitationRegex.Replace(PasswordBox.Text, "\\$1");
                verifyCommand.CommandText = "SELECT ValidateUser('" + UsernameBox.Text + "', '" + sanitizedPassword + "') AS Result;";

                MySqlDataReader verifyReader = verifyCommand.ExecuteReader();
                verifyReader.Read();
                bool isValid = verifyReader["Result"].ToString() == ValidResult;
                verifyReader.Close();
                if (isValid)
                {
                    ResultLabel.Text = "Successful login";

                    RecipeListBox.Visible = true;

                    MySqlCommand recipeListCommand = connection.CreateCommand();
                    recipeListCommand.CommandText = "SELECT RecipeName FROM UserRecipeList";
                    MySqlDataReader recipeListReader = recipeListCommand.ExecuteReader();
                    while (recipeListReader.Read())
                    {
                        RecipeListBox.Items.Add(recipeListReader["RecipeName"].ToString());
                    }
                    recipeListReader.Close();
                }
                else
                {
                    ShowInvalidResult();

                    RecipeListBox.Visible = false;
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                ResultLabel.Text = "An error occured: " + ex.Message;
            }
        }

        protected void CancelSignInButton_Clicked(object sender, EventArgs e)
        {
            Server.Transfer("Default.aspx");
        }

        private void ShowInvalidResult()
        {
            ResultLabel.Text = "Invalid Uername or Password";
        }
    }
}