using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using TypecursusApplicatie.Models;
using System.Collections.Generic;
using TypecursusApplicatie.BusinessLogicLayer;

namespace TypecursusApplicatie.Data_Access_Layer
{
    public class GebruikerDAL
    {
        private string connectionString;

        public GebruikerDAL()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Typecursusdatabase"].ConnectionString;
        }

        public List<Level> GetAllLevels()
        {
            List<Level> levels = new List<Level>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT LevelID, LevelNaam FROM Levels";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            levels.Add(new Level
                            {
                                LevelID = Convert.ToInt32(reader["LevelID"]),
                                LevelNaam = reader["LevelNaam"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het ophalen van alle levels: " + ex.Message);
                }
            }
            return levels;
        }
        public List<Module> GetAllModules()
        {
            List<Module> modules = new List<Module>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT ModuleID, LevelID, ModuleNaam, ModuleBeschrijving, ModuleContent, MinWPM, MinNauwkeurigheid FROM Modules";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            modules.Add(new Module
                            {
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                LevelID = Convert.ToInt32(reader["LevelID"]),
                                ModuleNaam = reader["ModuleNaam"].ToString(),
                                ModuleBeschrijving = reader["ModuleBeschrijving"].ToString(),
                                ModuleContent = reader["ModuleContent"].ToString(),
                                MinWPM = Convert.ToInt32(reader["MinWPM"]),
                                MinNauwkeurigheid = Convert.ToInt32(reader["MinNauwkeurigheid"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het ophalen van alle modules: " + ex.Message);
                }
            }
            return modules;
        }


        public void AddModulePoging(ModulePogingen poging)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO ModulePogingen (GebruikersID, ModuleID, GebruikersWPM, GebruikersNauwkeurigheid, PogingDatum) VALUES (@GebruikersID, @ModuleID, @GebruikersWPM, @GebruikersNauwkeurigheid, @PogingDatum)";
                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);

                cmd.Parameters.AddWithValue("@GebruikersID", poging.GebruikersID);
                cmd.Parameters.AddWithValue("@ModuleID", poging.ModuleID);
                cmd.Parameters.AddWithValue("@GebruikersWPM", poging.GebruikersWPM);
                cmd.Parameters.AddWithValue("@GebruikersNauwkeurigheid", poging.GebruikersNauwkeurigheid);
                cmd.Parameters.AddWithValue("@PogingDatum", poging.PogingDatum);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het toevoegen van modulepoging: " + ex.Message);
                }
            }
        }


        public List<Module> GetModulesForLevel(int levelId)
        {
            List<Module> modules = new List<Module>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT ModuleID, LevelID, ModuleNaam, ModuleBeschrijving, ModuleContent, MinWPM, MinNauwkeurigheid FROM Modules WHERE LevelID = @LevelID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LevelID", levelId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            modules.Add(new Module
                            {
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                LevelID = Convert.ToInt32(reader["LevelID"]),
                                ModuleNaam = reader["ModuleNaam"].ToString(),
                                ModuleBeschrijving = reader["ModuleBeschrijving"].ToString(),
                                ModuleContent = reader["ModuleContent"].ToString(),
                                MinWPM = Convert.ToInt32(reader["MinWPM"]),
                                MinNauwkeurigheid = Convert.ToInt32(reader["MinNauwkeurigheid"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het ophalen van modules voor level: " + ex.Message);
                }
            }
            return modules;
        }

        public Module GetModuleById(int moduleId)
        {
            Module module = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Modules WHERE ModuleID = @ModuleID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ModuleID", moduleId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            module = new Module
                            {
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                LevelID = Convert.ToInt32(reader["LevelID"]),
                                ModuleNaam = reader["ModuleNaam"].ToString(),
                                ModuleBeschrijving = reader["ModuleBeschrijving"].ToString(),
                                ModuleContent = reader["ModuleContent"].ToString(),
                                MinWPM = Convert.ToInt32(reader["MinWPM"]),
                                MinNauwkeurigheid = Convert.ToInt32(reader["MinNauwkeurigheid"])
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het ophalen van de module: " + ex.Message);
                }
            }

            return module;
        }



        public static string HashWachtwoord(string wachtwoord)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(wachtwoord));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }


        public int GetProgressForLevel(int userId, int levelId)
        {
            int progressPercentage = 0;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Replace this query with one that suits your database schema
                    var query = @"SELECT COUNT(*) AS CompletedModules,
                                (SELECT COUNT(*) FROM Modules WHERE LevelID = @LevelID) AS TotalModules
                                FROM GebruikersVoortgang
                                INNER JOIN Modules ON GebruikersVoortgang.ModuleID = Modules.ModuleID
                                WHERE GebruikersVoortgang.GebruikersID = @UserID AND Modules.LevelID = @LevelID AND GebruikersVoortgang.ModuleVoltooid = TRUE;
";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@LevelID", levelId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int completedModules = reader.GetInt32("CompletedModules");
                                int totalModules = reader.GetInt32("TotalModules");

                                if (totalModules > 0)
                                {
                                    progressPercentage = (completedModules * 100) / totalModules;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetProgressForLevel: " + ex.Message);
                // Handle exception as needed
            }

            return progressPercentage;
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
