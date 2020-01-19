using System;
using System.Collections.Generic;
using System.Web.Services;
using MySql.Data.MySqlClient;
using FSWeb.classes.db;
using FSWeb.classes.tools;

namespace FSWeb.models.login
{
    public partial class Login : System.Web.UI.Page
    {
        [WebMethod]
        public static bool checkLogin(ajaxQuery Data)
        {
            db myDB = new db(); 
            MySqlConnection connection = new MySqlConnection(myDB.ConnectString);
            string query = "Select Id from Users where Username = @Username and AuthCode = @AuthCode";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", Data.Username);
            cmd.Parameters.AddWithValue("@AuthCode", Data.AuthCode);
            connection.Open();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool DoesExist = false;
            while (dataReader.Read())
            {
                DoesExist = true;
            }

            dataReader.Close();
            connection.Close();
            return DoesExist;
        }

        [WebMethod]
        public static string register(ajaxQuery Data)
        {
            
            if (checkLogin(Data)) { return "1~Username already Exists!"; }
            db myDB = new db();
            string thisDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            hashAndSalt Password = new hashAndSalt();
            string Salt = Password.CreateSalt(32);
            string Hash = Password.GenerateHash(Data.Password);
            string FinalHash = Password.GenerateFinalHash(Hash, Salt);
            MySqlConnection connection = new MySqlConnection(myDB.ConnectString);
            string query = "insert into Users (Username, Password, SignupDate, Salt) Values(@Username, '" + FinalHash + "', '" + thisDate + "','" + Salt + "')";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", Data.Username);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            return "2~Successfully Registered! Please login now.";
        }

        [WebMethod]
        public static string login(ajaxQuery Data)
        {
            db myDB = new db();
            string thisDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySqlConnection connection = new MySqlConnection(myDB.ConnectString);
            string query = "Select Password, Salt, Id from Users where Username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", Data.Username);
            connection.Open();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            string Hash = "";
            string Salt = "";
            int Id = 0;
            hashAndSalt Password = new hashAndSalt();
            while (dataReader.Read())
            {
                Hash = dataReader[0].ToString();
                Salt = dataReader[1].ToString();
                Id = Convert.ToInt32(dataReader[2]);
            }
            dataReader.Close();
            if (Hash == "")
            {
                connection.Close();
                return "1~No user exists by that Username";
            }
            else
            {
                string FinalHash = Password.GenerateFinalHash(Data.Password, Salt);
                if(!Password.AreEqual(Data.Password,FinalHash, Salt))
                {
                    connection.Close();
                    return "1~Bad Password or Username";
                }
                else
                {
                    string NewPass = Password.CreateSalt(32);
                    query = "update Users set LastLogin = '" + thisDate + "', AuthCode = '" + NewPass + "' where Id = " + Id;
                    connection.Close();
                    return "2~" + NewPass;
                }
            }
        }
    }
}
