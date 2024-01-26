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
    // De Gebruiker Data Access Layer klasse bevat alle methodes voor database interacties gerelateerd aan gebruikers
    public class GebruikerDAL
    {
        // Verbindingsstring voor de database
        private string connectionString;

        // Constructor van GebruikerDAL, stelt de verbindingsstring in
        public GebruikerDAL()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Typecursusdatabase"].ConnectionString;
        }

        // Methode om gebruiker op te halen op basis van emailadres
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

        // Methode om gebruiker op te halen op basis van gebruikersID 
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

        // Methode om gebruiker toe te voegen aan de database
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

        // Methode om wachtwoord te hashen
        public static string HashWachtwoord(string wachtwoord, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = wachtwoord + salt;
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Methode om alle Levels op te halen
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

        // Methode om alle modules op te halen
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

        // Methode om modulepoging toe te voegen aan de database
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

        // Methode om alle modules voor een level op te halen
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

        // Methode om module op te halen op basis van moduleID
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

        // Methode om gebruikers progressie op te halen
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

        // Methode om gebruiker progressie bij te werken
        public void UpdateUserProgress(int userId, int moduleId, bool isModuleCompleted)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Controleert of er al een record bestaat voor de gebruiker en module
                string checkQuery = "SELECT COUNT(1) FROM GebruikersVoortgang WHERE GebruikersID = @UserId AND ModuleID = @ModuleId";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ModuleId", moduleId);

                int recordExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                string query;
                
                if (recordExists > 0)
                {
                    // Werkt het bestaande record bij
                    query = "UPDATE GebruikersVoortgang SET ModuleVoltooid = @IsModuleCompleted, VoltooiDatum = NOW() WHERE GebruikersID = @UserId AND ModuleID = @ModuleId";
                }
                else
                {
                    // voegt een nieuw record toe
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

        // Methode om te controleren of een module voltooid is
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
            return false;
        }

        // Methode om de wpm van een gebruiker op te halen
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

        // Een IEnumerable om alle modulepogingen van een gebruiker op te halen dit is zo ingesteld omdat er meerdere pogingen kunnen zijn
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

        // Methode om de telling van voltooide modules op te halen
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

        // Methode om alle voltooide modules op te halen voor een gebruiker
        public int GetCompletedModulesByUserId(int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM GebruikersVoortgang WHERE GebruikersID = @UserId AND ModuleVoltooid = TRUE";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Methode om de badges telling op te halen voor een gebruiker
        public int GetEarnedBadgesCountByUserId(int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM GebruikersBadges WHERE GebruikersID = @UserId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Methode om alle badges op te halen uit de database
        public List<Badge> GetAllBadges()
        {
            List<Badge> badges = new List<Badge>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT BadgeID, BadgeNaam, Criteria, BadgeAfbeelding, BadgeBeschrijving FROM Badges";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            badges.Add(new Badge
                            {
                                BadgeID = Convert.ToInt32(reader["BadgeID"]),
                                BadgeNaam = reader["BadgeNaam"].ToString(),
                                Criteria = reader["Criteria"].ToString(),
                                BadgeAfbeelding = (byte[])reader["BadgeAfbeelding"],
                                BadgeBeschrijving = reader["BadgeBeschrijving"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetAllBadges: " + ex.Message);
                }
            }
            return badges;
        }

        // Methode om alle badges op te halen voor een gebruiker
        public List<Badge> GetBadgesByUserId(int userId)
        {
            List<Badge> badges = new List<Badge>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT Badges.*, GebruikersBadges.BadgeBehaalDatum FROM Badges INNER JOIN GebruikersBadges ON Badges.BadgeID = GebruikersBadges.BadgeID WHERE GebruikersBadges.GebruikersID = @UserId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            badges.Add(new Badge
                            {
                                BadgeID = Convert.ToInt32(reader["BadgeID"]),
                                BadgeNaam = reader["BadgeNaam"].ToString(),
                                Criteria = reader["Criteria"].ToString(),
                                BadgeAfbeelding = (byte[])reader["BadgeAfbeelding"],
                                BadgeBeschrijving = reader["BadgeBeschrijving"].ToString(),
                                IsUnlocked = true,
                                BadgeBehaalDatum = reader.IsDBNull(reader.GetOrdinal("BadgeBehaalDatum")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("BadgeBehaalDatum"))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetBadgesByUserId: " + ex.Message);
                }
            }
            return badges;
        }

        // Methode om een badge toe te voegen aan een gebruiker
        public void AddBadgeToUser(int userId, int badgeId)
        {             using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO GebruikersBadges (GebruikersID, BadgeID, BadgeBehaalDatum) VALUES (@UserId, @BadgeId, NOW())";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@BadgeId", badgeId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in AddBadgeToUser: " + ex.Message);
                }
            }
        }

        // Methode om alle badges op te halen voor een gebruiker
        public List<Badge> GetBadgesForUser(int userId)
        {
            // Implementation for getting badges for a user
            List<Badge> badges = new List<Badge>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Badges";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            badges.Add(new Badge
                            {
                                BadgeID = Convert.ToInt32(reader["BadgeID"]),
                                BadgeNaam = reader["BadgeNaam"].ToString(),
                                Criteria = reader["Criteria"].ToString(),
                                BadgeAfbeelding = (byte[])reader["BadgeAfbeelding"],
                                BadgeBeschrijving = reader["BadgeBeschrijving"].ToString(),
                                IsUnlocked = false
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetBadgesForUser: " + ex.Message);
                }
            }
            return badges;
        }

        // Methode om de laatste prestatie van een gebruiker op te halen
        public UserPerformance GetLatestPerformanceForUser(int userId)
        {
            UserPerformance performance = new UserPerformance();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // Query om de WPM en Accuracy op te halen
                string wpmAccuracyQuery = @"
            SELECT AVG(GebruikersWPM) AS AvgWPM, AVG(GebruikersNauwkeurigheid) AS AvgAccuracy 
            FROM ModulePogingen 
            WHERE GebruikersID = @UserId";
                MySqlCommand wpmAccuracyCmd = new MySqlCommand(wpmAccuracyQuery, conn);
                wpmAccuracyCmd.Parameters.AddWithValue("@UserId", userId);

                // Query om alle voltooide levels op te halen
                string levelsCompletedQuery = @"
            SELECT COUNT(DISTINCT LevelID) AS LevelsCompleted
            FROM Modules 
            INNER JOIN GebruikersVoortgang ON Modules.ModuleID = GebruikersVoortgang.ModuleID
            WHERE GebruikersVoortgang.GebruikersID = @UserId AND GebruikersVoortgang.ModuleVoltooid = TRUE";
                MySqlCommand levelsCompletedCmd = new MySqlCommand(levelsCompletedQuery, conn);
                levelsCompletedCmd.Parameters.AddWithValue("@UserId", userId);

                // Query om te controleren of alle badges zijn behaald
                string allBadgesQuery = @"
            SELECT (SELECT COUNT(*) FROM Badges) AS TotalBadges, 
                   (SELECT COUNT(DISTINCT BadgeID) FROM GebruikersBadges WHERE GebruikersID = @UserId) AS EarnedBadges";
                MySqlCommand allBadgesCmd = new MySqlCommand(allBadgesQuery, conn);
                allBadgesCmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();

                    // Voert de WPM en Accuracy query uit
                    using (var reader = wpmAccuracyCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            performance.WPM = reader.IsDBNull(reader.GetOrdinal("AvgWPM")) ? 0 : reader.GetDouble("AvgWPM");
                            performance.Accuracy = reader.IsDBNull(reader.GetOrdinal("AvgAccuracy")) ? 0 : reader.GetDouble("AvgAccuracy");
                        }
                    }

                    // Voert de Levels Completed query uit
                    levelsCompletedCmd.ExecuteNonQuery();
                    performance.LevelsCompleted = Convert.ToInt32(levelsCompletedCmd.ExecuteScalar());

                    // Voert de All Badges query uit
                    using (var reader = allBadgesCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int totalBadges = reader.GetInt32("TotalBadges");
                            int earnedBadges = reader.GetInt32("EarnedBadges");
                            performance.AllBadgesEarned = (earnedBadges == totalBadges);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in GetLatestPerformanceForUser: " + ex.Message);
                }
            }

            return performance;
        }

        // Methode om te controleren of een badge is toegekend aan een gebruiker
        public bool IsBadgeAwardedToUser(int userId, int badgeId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM GebruikersBadges WHERE GebruikersID = @UserId AND BadgeID = @BadgeId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@BadgeId", badgeId);

                try
                {
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in IsBadgeAwardedToUser: " + ex.Message);
                    return false;
                }
            }
        }
    }

    // Een klasse om de prestaties van een gebruiker bij te houden
    public class UserPerformance
    {
        public int PogingID { get; set; }
        public int GebruikersID { get; set; }
        public int ModuleID { get; set; }
        public int GebruikersWPM { get; set; }
        public int GebruikersNauwkeurigheid { get; set; }
        public DateTime PogingDatum { get; set; }
        public double Accurracy { get; set; }
        public double WPM { get; set; }
        public int UserId { get; set; }
        public double Accuracy { get; set; }
        public int LevelsCompleted { get; set; }
        public bool AllBadgesEarned { get; set; } 
    }
}
