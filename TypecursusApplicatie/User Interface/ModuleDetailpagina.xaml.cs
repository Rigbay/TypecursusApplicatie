using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using TypecursusApplicatie.Data_Access_Layer;
using TypecursusApplicatie.Models;
using TypecursusApplicatie.BusinessLogicLayer;
using System.Windows.Input;

namespace TypecursusApplicatie
{
    public partial class ModuleDetailpagina : UserControl
    {
        private Module _currentModule;
        private MainWindow mainWindow;
        private int timeLeft;
        private DispatcherTimer typingTimer;
        private int currentWordIndex = 0;
        private int charsTypedCorrectly = 0;
        private int totalCharsTyped = 0;
        private string[] displayedWords;

        public ModuleDetailpagina(MainWindow mainWindow, int moduleId)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            LoadModuleData(moduleId);
        }

        private void LoadModuleData(int moduleId)
        {
            GebruikerDAL gebruikerDAL = new GebruikerDAL();
            _currentModule = gebruikerDAL.GetModuleById(moduleId);
            InitializeTypingTest();
        }

        private void InitializeTypingTest()
        {
            rtxtTypetestText.Document.Blocks.Clear();
            rtxtTypetestText.AppendText(_currentModule.ModuleContent);

            string text = new TextRange(rtxtTypetestText.Document.ContentStart, rtxtTypetestText.Document.ContentEnd).Text;
            displayedWords = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            lblModuleName.Text = _currentModule.ModuleNaam;
            lblModuleRequirements.Text = $"Min WPM: {_currentModule.MinWPM}, Min Nauwkeurigheid: {_currentModule.MinNauwkeurigheid}%";

            timeLeft = 60; // Of een andere waarde afhankelijk van de module
            typingTimer = new DispatcherTimer();
            typingTimer.Interval = TimeSpan.FromSeconds(1);
            typingTimer.Tick += TypingTimer_Tick;
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                lblTimer.Text = $"Tijd: {timeLeft}";
            }
            else
            {
                typingTimer.Stop();
                MessageBox.Show("Tijd is op!");
                SaveUserProgress(); // Opslaan van gebruikersvoortgang als de tijd om is
            }
        }

        private void TxtTypingArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!typingTimer.IsEnabled && !string.IsNullOrWhiteSpace(txtTypingArea.Text))
            {
                typingTimer.Start();
            }
            UpdateTypingStatistics();
        }

        private void UpdateTypingStatistics()
        {
            string typedWord = txtTypingArea.Text.TrimEnd();

            if (currentWordIndex >= displayedWords.Length) return;

            string currentWord = displayedWords[currentWordIndex];
            totalCharsTyped += typedWord.Length;

            if (typedWord.EndsWith(" ") || typedWord.EndsWith("\n"))
            {
                typedWord = typedWord.Trim();

                if (typedWord == currentWord)
                {
                    charsTypedCorrectly += typedWord.Length;
                    currentWordIndex++;
                }
                txtTypingArea.Clear();

                // Update labels
                CalculateWPM();
                CalculateAccuracy();
            }
        }

        private double CalculateWPM()
        {
            double wpm = ((double)charsTypedCorrectly / 5) / ((60 - timeLeft) / 60.0);
            lblWPM.Text = $"WPM: {Math.Round(wpm, 2)}";
            return wpm; // Retourneer de berekende WPM
        }

        private double CalculateAccuracy()
        {
            double accuracy = ((double)charsTypedCorrectly / totalCharsTyped) * 100;
            lblAccuracy.Text = $"Nauwkeurigheid: {Math.Round(accuracy, 2)}%";
            return accuracy; // Retourneer de berekende nauwkeurigheid
        }

        private void SaveUserProgress()
        {
            int userId = UserSession.CurrentUserID; // Verondersteld dat UserSession een geldige referentie is
            double wpm = CalculateWPM();
            double accuracy = CalculateAccuracy();

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
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetTypingTest();
        }

        private void ResetTypingTest()
        {
            typingTimer.Stop();
            timeLeft = 60;
            txtTypingArea.Clear();
            InitializeTypingTest(); // Reset de tekst en laadt deze opnieuw
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            SaveUserProgress();
            mainWindow.LoadModuleOverzichtspagina(_currentModule.LevelID);
        }

        private void Homepagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is MainWindow main)
            {
                mainWindow = main;
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadLoginControl();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LogoutUser();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadRegisterControl();

        }

        private void LevelsPagina_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadLevelsControl();
        }

        private void SidebarToggle_Click(object sender, RoutedEventArgs e)
        {
            Sidebar.Width = Sidebar.Width == 0 ? 250 : 0;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.LoadHomeControl();
        }

        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            mainWindow.LoadHomeControl();
        }
    }
}
