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

        public static string HashWachtwoord(string wachtwoord, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Combine the password and salt before hashing
                string saltedPassword = wachtwoord + salt;
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
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
            }

            return progressPercentage;
        }


        public void AddGebruiker(Gebruiker nieuweGebruiker)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                var salt = Guid.NewGuid().ToString();
                var hashedPassword = HashWachtwoord(nieuweGebruiker.Wachtwoord, salt);

                MySqlCommand cmd = new MySqlCommand("INSERT INTO Gebruikers (Voornaam, Achternaam, Emailadres, Wachtwoord, Salt) VALUES (@Voornaam, @Achternaam, @Emailadres, @Wachtwoord, @Salt)", conn);

                cmd.Parameters.AddWithValue("@Voornaam", nieuweGebruiker.Voornaam);
                cmd.Parameters.AddWithValue("@Achternaam", nieuweGebruiker.Achternaam);
                cmd.Parameters.AddWithValue("@Emailadres", nieuweGebruiker.Emailadres);
                cmd.Parameters.AddWithValue("@Wachtwoord", hashedPassword);
                cmd.Parameters.AddWithValue("@Salt", salt);

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
                            Salt = reader["Salt"].ToString(),
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

        public void UpdateUserProgress(int userId, int moduleId, bool isModuleCompleted)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Check if a record already exists
                string checkQuery = "SELECT COUNT(1) FROM GebruikersVoortgang WHERE GebruikersID = @UserId AND ModuleID = @ModuleId";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ModuleId", moduleId);

                int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                string query;
                
                if (recordExists > 0)
                {
                    // Update existing record
                    query = "UPDATE GebruikersVoortgang SET ModuleVoltooid = @IsModuleCompleted, VoltooiDatum = NOW() WHERE GebruikersID = @UserId AND ModuleID = @ModuleId";
                }
                else
                {
                    // Insert new record
                    query = "INSERT INTO GebruikersVoortgang (GebruikersID, ModuleID, ModuleVoltooid, VoltooiDatum) VALUES (@UserId, @ModuleId, @IsModuleCompleted, NOW())";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IsModuleCompleted", isModuleCompleted);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ModuleId", moduleId);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in updating/adding user progress: " + ex.Message);
                }
            }
        }

        public bool IsModuleCompleted(int userId, int moduleId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT ModuleVoltooid FROM GebruikersVoortgang WHERE GebruikersID = @UserId AND ModuleID = @ModuleId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ModuleId", moduleId);

                try
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToBoolean(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in IsModuleCompleted: " + ex.Message);
                }
            }
            return false; // Default to false if no record found or in case of an exception
        }

        public (List<double> Values, List<string> TimeLabels) GetWpmDataForUser(int userId)
        {
            List<double> values = new List<double>();
            List<string> timeLabels = new List<string>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT GebruikersWPM, PogingDatum FROM ModulePogingen WHERE GebruikersID = @UserID ORDER BY PogingDatum";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            values.Add(Convert.ToDouble(reader["GebruikersWPM"]));
                            timeLabels.Add(Convert.ToDateTime(reader["PogingDatum"]).ToShortDateString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetWpmDataForUser: " + ex.Message);
                }
            }

            return (values, timeLabels);
        }

        public Gebruiker GetGebruikerById(int userId)
        {
            Gebruiker gebruiker = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Gebruikers WHERE GebruikersID = @UserId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            gebruiker = new Gebruiker
                            {
                                GebruikersID = Convert.ToInt32(reader["GebruikersID"]),
                                Voornaam = reader["Voornaam"].ToString(),
                                Achternaam = reader["Achternaam"].ToString(),
                                Emailadres = reader["Emailadres"].ToString(),
                                // Other fields if necessary
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetGebruikerById: " + ex.Message);
                }
            }
            return gebruiker;
        }

        public IEnumerable<ModulePogingen> GetModulePogingenByUserId(int userId)
        {
            List<ModulePogingen> pogingen = new List<ModulePogingen>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM ModulePogingen WHERE GebruikersID = @UserId ORDER BY PogingDatum";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pogingen.Add(new ModulePogingen
                            {
                                PogingID = Convert.ToInt32(reader["PogingID"]),
                                GebruikersID = Convert.ToInt32(reader["GebruikersID"]),
                                ModuleID = Convert.ToInt32(reader["ModuleID"]),
                                GebruikersWPM = Convert.ToInt32(reader["GebruikersWPM"]),
                                GebruikersNauwkeurigheid = Convert.ToInt32(reader["GebruikersNauwkeurigheid"]),
                                PogingDatum = Convert.ToDateTime(reader["PogingDatum"])
                                // Other fields if necessary
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetModulePogingenByUserId: " + ex.Message);
                }
            }
            return pogingen;
        }

        public int GetCompletedLevelsCount(int userId)
        {
            int completedLevelsCount = 0;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"
                SELECT COUNT(DISTINCT LevelID) AS CompletedLevels
                FROM Modules
                INNER JOIN GebruikersVoortgang ON Modules.ModuleID = GebruikersVoortgang.ModuleID
                WHERE GebruikersVoortgang.GebruikersID = @UserId AND GebruikersVoortgang.ModuleVoltooid = TRUE";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    completedLevelsCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetCompletedLevelsCount: " + ex.Message);
                }
            }
            return completedLevelsCount;
        }

    }
}
