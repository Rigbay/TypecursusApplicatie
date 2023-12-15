using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TypecursusApplicatie.Models;

namespace TypecursusApplicatie.Data_Access_Layer
{
    public class GebruikerDAL
    {
        private string connectionString;

        public GebruikerDAL()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Typecursusdatabase"].ConnectionString;
        }

        public Gebruiker GetGebruiker(int id)
        {
            Gebruiker gebruiker = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM Gebruikers WHERE GebruikersID = {id}", conn);

                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        gebruiker = new Gebruiker
                        {
                            GebruikersID = Convert.ToInt32(reader["GebruikersID"]),
                            Voornaam = reader["Voornaam"].ToString(),
                            Achternaam = reader["Achternaam"].ToString(),
                            Emailadres = reader["Emailadres"].ToString(),
                            Wachtwoord = reader["Wachtwoord"].ToString()
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Error handling
                    Console.WriteLine("Fout bij het ophalen van de gebruiker: " + ex.Message);
                }
            }

            return gebruiker;
        }

        // Andere methoden...
    }
}
