using MySql.Data.MySqlClient;
using System;
using System.Security.Cryptography;
using System.Text;
using TypecursusApplicatie.Models;
using System.Collections.Generic;

namespace TypecursusApplicatie.Data_Access_Layer
{
    public class GebruikerDAL
    {
        private string connectionString;

        public GebruikerDAL()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Typecursusdatabase"].ConnectionString;
        }

        public List<LevelVoortgang> GetGebruikersVoortgangPerLevel(int gebruikersID)
        {
            List<LevelVoortgang> voortgangslijst = new List<LevelVoortgang>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"
                SELECT l.LevelID, l.LevelNaam, COUNT(m.ModuleID) AS TotaalModules, 
                       SUM(CASE WHEN gv.ModuleVoltooid THEN 1 ELSE 0 END) AS VoltooideModules
                FROM Levels l
                JOIN Modules m ON l.LevelID = m.LevelID
                LEFT JOIN GebruikersVoortgang gv ON m.ModuleID = gv.ModuleID AND gv.GebruikersID = @GebruikersID
                GROUP BY l.LevelID";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GebruikersID", gebruikersID);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int levelID = Convert.ToInt32(reader["LevelID"]);
                            string levelNaam = reader["LevelNaam"].ToString();
                            int totaalModules = Convert.ToInt32(reader["TotaalModules"]);
                            int voltooideModules = Convert.ToInt32(reader["VoltooideModules"]);

                            int voortgang = totaalModules > 0 ? (voltooideModules * 100 / totaalModules) : 0;

                            voortgangslijst.Add(new LevelVoortgang
                            {
                                LevelID = levelID,
                                LevelNaam = levelNaam,
                                Voortgang = voortgang
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het ophalen van de voortgang per level: " + ex.Message);
                }
            }
            return voortgangslijst;
        }

        public static string HashWachtwoord(string wachtwoord)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(wachtwoord));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }


    public void AddGebruiker(Gebruiker nieuweGebruiker)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Gebruikers (Voornaam, Achternaam, Emailadres, Wachtwoord) VALUES (@Voornaam, @Achternaam, @Emailadres, @Wachtwoord)", conn);

                cmd.Parameters.AddWithValue("@Voornaam", nieuweGebruiker.Voornaam);
                cmd.Parameters.AddWithValue("@Achternaam", nieuweGebruiker.Achternaam);
                cmd.Parameters.AddWithValue("@Emailadres", nieuweGebruiker.Emailadres);
                cmd.Parameters.AddWithValue("@Wachtwoord", HashWachtwoord(nieuweGebruiker.Wachtwoord));

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het registreren van de gebruiker: " + ex.Message);
                }
            }
        }

        public Gebruiker GetGebruikerByEmail(string email)
        {
            Gebruiker gebruiker = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Gebruikers WHERE Emailadres = @Emailadres", conn);
                cmd.Parameters.AddWithValue("@Emailadres", email);

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
                    Console.WriteLine("Fout bij het ophalen van de gebruiker: " + ex.Message);
                }
            }

            return gebruiker;
        }
    }
}
