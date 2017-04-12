using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text;


namespace RecipeManager.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PassHash { get; set; }
        public string Salt { get; set; }
        
        public static class UserDb
        {
            public static User SelectByUserId(int userid) {

                User output = null;
                MySqlConnection conn = new MySqlConnection(DatabaseConnectModel.DbConn);
                MySqlCommand command = new MySqlCommand("SelectByUserId", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", userid);
                try
                {
                    conn.Open();
                    MySqlDataReader DataReader = command.ExecuteReader();
                    if (DataReader.Read())
                    {
                        output = new User()
                        {
                            UserId = Convert.ToInt32("UserId"),
                            Username = DataReader["Username"].ToString(),
                            PassHash = DataReader["PassHash"].ToString()


                        };
                    }

                }
                catch
                {

                }
                finally
                {
                    conn.Close();
                }
                return output;
            }

        }  
    }
}