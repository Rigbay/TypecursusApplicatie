using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;
using TypecursusApplicatie.BusinessLogicLayer;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;
using LiveCharts.Wpf;

namespace TypecursusApplicatie
{
    public partial class ModuleDetailpagina : UserControl
    {
        private Module _currentModule;
        private MainWindow mainWindow;
        private DispatcherTimer typingTimer;
        private int currentWordIndex = 0;
        private int charsTypedCorrectly = 0;
        private int totalCharsTyped = 0;
        private Paragraph paragraph;
        private bool isTypingStarted = false;
        private List<string> wordsList;
        private GebruikerDAL GebruikerDAL;


        /* Initialisatie van de Module Detailpagina */

        // Constructor: Initalisatie van de Module Detailpagina
        public ModuleDetailpagina(MainWindow mainWindow, int moduleId)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.GebruikerDAL = new GebruikerDAL();
            typingTimer = new DispatcherTimer();
            typingTimer.Interval = TimeSpan.FromSeconds(1);
            typingTimer.Tick += TypingTimer_Tick;
            LoadModuleData(moduleId);
        }

        // Laadt de gegevens van de module
        private void LoadModuleData(int moduleId)
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            _currentModule = gebruikerDAL.GetModuleById(moduleId);

            // Initialisatie van de timerduur
            _currentModule.TimeLeft = 60;

            lblModuleName.Text = _currentModule.ModuleNaam; // Stelt de naam van de module in
            lblModuleRequirements.Text = $"Minimum Woorden Per Minuut: {_currentModule.MinWPM}\nMinimum Nauwkeurigheid: {_currentModule.MinNauwkeurigheid}%"; // Zet de minimum vereisten voor de module

            InitializeTypingTest();
        }

        // Initialiseert de typetest
        private void InitializeTypingTest()
        {
            wordsList = _currentModule.ModuleContent.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(word => !string.IsNullOrWhiteSpace(word))
                        .ToList();
            paragraph = new Paragraph
            {
                TextAlignment = TextAlignment.Center
            };
            rtxtTypetestText.Document.Blocks.Clear();
            rtxtTypetestText.Document.Blocks.Add(paragraph);

            foreach (string word in wordsList)
            {
                Run wordRun = new Run(word);
                paragraph.Inlines.Add(wordRun);
                paragraph.Inlines.Add(new Run(" ")); 
            }

            currentWordIndex = 0;
            HighlightCurrentWord();
        }





        /* Typetest functionaliteit */

        // Markeert het huidige woord tijdens het typen
        private void HighlightCurrentWord()
        {
            int wordIndex = 0;

            foreach (Inline inline in paragraph.Inlines)
            {
                if (inline is Run run)
                {
                    run.Background = wordIndex == currentWordIndex && !string.IsNullOrWhiteSpace(run.Text)
                                     ? Brushes.LightGray : Brushes.Transparent;

                    if (!string.IsNullOrWhiteSpace(run.Text))
                    {
                        wordIndex++;
                    }
                }
            }
        }


        // Controleert en markeert het getypte woord
        private void CheckAndHighlightWord()
        {
            string typedText = txtTypingArea.Text.Trim();
            string currentWord = currentWordIndex < wordsList.Count ? wordsList[currentWordIndex] : "";

            totalCharsTyped += typedText.Length;

            Run run = FindRunByIndex(currentWordIndex);

            if (typedText.Equals(currentWord, StringComparison.OrdinalIgnoreCase))
            {
                charsTypedCorrectly += typedText.Length;
                if (run != null) run.Foreground = Brushes.Green;
                currentWordIndex++;
            }
            else
            {
                if (run != null) run.Foreground = Brushes.Red;
            }

            HighlightCurrentWord();
            CalculateWPM();
            CalculateAccuracy();

            txtTypingArea.Clear();
            ScrollToCurrentWord();
        }

        private void ScrollToCurrentWord()
        {
            if (currentWordIndex >= wordsList.Count) return;

            Run run = FindRunByIndex(currentWordIndex);
            if (run != null)
            {
                Rect runPosition = run.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                double verticalOffset = rtxtTypetestText.VerticalOffset;
                double viewportHeight = rtxtTypetestText.ViewportHeight;

                if (runPosition.Bottom > verticalOffset + viewportHeight)
                {
                    rtxtTypetestText.ScrollToVerticalOffset(verticalOffset + runPosition.Bottom - verticalOffset - viewportHeight);
                }
                else if (runPosition.Top < verticalOffset)
                {
                    rtxtTypetestText.ScrollToVerticalOffset(runPosition.Top);
                }
            }
        }







        // Helper methode om te controleren of een Run zichtbaar is in de RichTextBox
        private bool IsRunVisible(Rect runPosition)
        {
            var visibleRange = new Rect(0, rtxtTypetestText.VerticalOffset,
                                        rtxtTypetestText.ViewportWidth, rtxtTypetestText.ViewportHeight);
            return visibleRange.Contains(runPosition.TopLeft) || visibleRange.Contains(runPosition.BottomRight);
        }




        // Update de statistieken van het typen voor de huidige poging
        private void UpdateTypingStatistics()
        {
            string typedText = txtTypingArea.Text;
            string currentWord = GetCurrentWord();

            if (typedText.EndsWith(" "))
            {
                totalCharsTyped += typedText.Trim().Length;

                if (typedText.Trim().Equals(currentWord))
                {
                    charsTypedCorrectly += typedText.Trim().Length;
                    MarkCurrentWordCorrect();
                }
                else
                {
                    MarkCurrentWordIncorrect();
                }

                currentWordIndex++;
                txtTypingArea.Clear();
                HighlightCurrentWord();
                CalculateWPM();
                CalculateAccuracy();
            }
        }

        private void RtxtTypetestText_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true; 
        }

        // Geeft het huidige woord terug dat getypt moet worden
        private string GetCurrentWord()
        {
            int nonSpaceRunIndex = 0;
            foreach (Run run in paragraph.Inlines.OfType<Run>())
            {
                if (nonSpaceRunIndex == currentWordIndex)
                {
                    return run.Text.TrimEnd();
                }
                nonSpaceRunIndex++;
            }
            return string.Empty;
        }

        // Vindt de Run op basis van de index van het woord
        private Run FindRunByIndex(int index)
        {
            int wordIndex = 0;
            foreach (Inline inline in paragraph.Inlines)
            {
                if (inline is Run run && run.Text.Trim() != "")
                {
                    if (wordIndex == index)
                    {
                        return run;
                    }
                    wordIndex++;
                }
            }
            return null;
        }




        /* Typtest metrische berekeningen */

        // Berekent de WPM voor de huidige poging
        private double CalculateWPM()
        {
            double timeElapsedInMinutes = (60 - _currentModule.TimeLeft) / 60.0;
            if (timeElapsedInMinutes == 0) return 0;
            double wpm = charsTypedCorrectly / 5d / timeElapsedInMinutes;
            lblWPM.Text = $"WPM: {Math.Round(wpm, 2)}";
            return wpm;
        }

        // Berekent de nauwkeurigheid voor de huidige poging in procenten
        private double CalculateAccuracy()
        {
            double accuracy = (double)charsTypedCorrectly / totalCharsTyped * 100;
            lblAccuracy.Text = $"Nauwkeurigheid: {Math.Round(accuracy, 2)}%";
            return accuracy;
        }




        /* Gebruikersvoortgang management */

        // Slaat de voortgang van de gebruiker op
        private bool SaveUserProgress()
        {
            int userId = UserSession.CurrentUserID;
            double wpm = CalculateWPM();
            double accuracy = CalculateAccuracy();

            // Rond de WPM en nauwkeurigheid af
            int roundedWPM = (int)Math.Round(wpm);
            int roundedAccuracy = (int)Math.Round(accuracy);

            ModulePogingen poging = new ModulePogingen
            {
                GebruikersID = userId,
                ModuleID = _currentModule.ModuleID,
                GebruikersWPM = (int)wpm,
                GebruikersNauwkeurigheid = (int)accuracy,
                PogingDatum = DateTime.Now
            };

            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            gebruikerDAL.AddModulePoging(poging);

            bool isModuleCompleted = wpm >= _currentModule.MinWPM && accuracy >= _currentModule.MinNauwkeurigheid;
            if (isModuleCompleted)
            {
                gebruikerDAL.UpdateUserProgress(userId, _currentModule.ModuleID, true);
            }

            CheckAndAwardBadges();
            return isModuleCompleted;
        }

        // Checkt of de gebruiker een badge heeft behaald
        private void CheckAndAwardBadges()
        {
            int userId = UserSession.CurrentUserID;
            UserPerformance userPerformance = GebruikerDAL.GetLatestPerformanceForUser(userId);
            var allAvailableBadges = GebruikerDAL.GetAllBadges(); 

            foreach (var badge in allAvailableBadges)
            {
                // Checkt of de gebruiker de badge al heeft behaald
                if (!GebruikerDAL.IsBadgeAwardedToUser(userId, badge.BadgeID))
                {
                    if (CheckBadgeCriteria(userPerformance, badge.Criteria, _currentModule))
                    {
                        GebruikerDAL.AddBadgeToUser(userId, badge.BadgeID);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Gefeliciteerd! Je hebt de volgende badge verdiend!: {badge.BadgeNaam}");
                        });
                    }
                }
            }
        }


        //Contoleert of de gebruiker de criteria voor de badge heeft behaald
        private bool CheckBadgeCriteria(UserPerformance performance, string criteria, Module currentModule)
        {
            if (performance == null || string.IsNullOrEmpty(criteria))
            {
                return false;
            }

            if (criteria.EndsWith(" WPM"))
            {
                if (int.TryParse(criteria.Replace(" WPM", ""), out int targetWPM))
                {
                    return performance.WPM >= targetWPM || (currentModule != null && currentModule.MinWPM >= targetWPM);
                }
            }
            else if (criteria.StartsWith("Level "))
            {
                if (int.TryParse(criteria.Substring(6), out int targetLevel))
                {
                    return performance.LevelsCompleted >= targetLevel;
                }
            }
            else if (criteria == "Alle Badges Behaald")
            {
                return performance.AllBadgesEarned;
            }

            return false;
        }



        // Reset de typetest en de timer
        private void ResetTypingTest()
        {
            typingTimer.Stop();
            _currentModule.TimeLeft = 60;
            txtTypingArea.Clear();
            txtTypingArea.IsEnabled = true;
            isTypingStarted = false;
            currentWordIndex = 0;
            charsTypedCorrectly = 0;
            totalCharsTyped = 0;
            InitializeTypingTest();
            lblTimer.Text = "Tijd: 60";
            lblWPM.Text = "WPM: 0";
            lblAccuracy.Text = "Accuracy: 0%";
            rtxtTypetestText.CaretPosition = rtxtTypetestText.Document.ContentStart;
            rtxtTypetestText.ScrollToHome();
        }





        /* UI Interactie event handlers */

        // Timer voor de typetest
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (_currentModule.TimeLeft > 0)
            {
                _currentModule.TimeLeft--;
                lblTimer.Text = $"Tijd: {_currentModule.TimeLeft}";
            }
            else
            {
                typingTimer.Stop();
                bool isCompleted = SaveUserProgress();
                txtTypingArea.IsEnabled = false;
                if (isCompleted)
                {
                    MessageBox.Show("Je hebt de module behaald!.");
                }
                else
                {
                    MessageBox.Show("De tijd is op");
                }
            }
        }

        // Behandelt tekstwijzigingen in het typgebied
        private void TxtTypingArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isTypingStarted && !string.IsNullOrWhiteSpace(txtTypingArea.Text))
            {
                typingTimer.Start();
                isTypingStarted = true;
            }
        }

        // Spacebar key event handler
        private void TxtTypingArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                CheckAndHighlightWord();
                e.Handled = true;
            }
        }

        // Reset de typetest en de timer wanneer de gebruiker op de resetknop klikt
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetTypingTest();
        }

        // Navigeert terug naar de module overzichtspagina
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Stopt de timer als deze actief is
            if (typingTimer.IsEnabled)
            {
                typingTimer.Stop();

                // Slaat de voortgang van de gebruiker op als de tijd op is
                if (_currentModule.TimeLeft == 0)
                {
                    SaveUserProgress();
                }

                ResetTypingTest();
            }

            // Navigeert terug naar de module overzichtspagina
            if (mainWindow != null)
            {
                mainWindow.LoadModuleOverzichtspagina(_currentModule.LevelID);
            }
        }

        // Event handler voor het laden van de homepagina
        private void Homepagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainWindow main)
            {
                mainWindow = main;
            }
        }

        // Event handler voor de statistiekenpagina knop
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadAccountControl();
        }

        // Event handler voor de login knop
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadLoginControl();
        }

        // Event handler voor de logout knop
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LogoutUser();
        }

        // Event handler voor de registratie knop
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadRegisterControl();

        }

        // Event handler voor de levels pagina knop
        private void LevelsPagina_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadLevelsControl();
        }

        // Event handler voor de zijbalk knop
        private void SidebarToggle_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Width = Sidebar.Width == 0 ? 250 : 0;
        }

        // Event handler voor de homepagina knop
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadHomeControl();
        }

        // Event handler voor de logo knop
        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            mainWindow.LoadHomeControl();
        }

        // Event handler voor de level selectie combobox
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedLevel = comboBox.SelectedItem as Level;
            if (selectedLevel != null && selectedLevel.IsUnlocked)
            {
                mainWindow.LoadModuleOverzichtspagina(selectedLevel.LevelID);
            }
            else
            {

                comboBox.SelectedIndex = -1;
            }
        }




        /* UI Feedback methodes */

        // Markeert het huidige woord groen als het correct getypt is
        private void MarkCurrentWordCorrect()
        {
            Run run = FindRunByIndex(currentWordIndex);
            if (run != null)
            {
                run.Foreground = Brushes.Green;
            }
        }

        // Markeert het huidige woord rood als het incorrect getypt is
        private void MarkCurrentWordIncorrect()
        {
            Run run = FindRunByIndex(currentWordIndex);
            if (run != null)
            {
                run.Foreground = Brushes.Red;
            }
        }

    }
}
