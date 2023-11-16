using GoingOutApp.Models;
using GoingOutApp.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {  
        string mapPath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\data\images\mapa.png";

        private static LoginWindow ?_profileWindowInstance;
        private static AddTaskwindow? _addWindowInstance;
        private static UserProfileWindow? _userProfileWindowInstance;
        private DataContext _database { get; set; } = new DataContext();

        List<Event> events = new List<Event>(); 

        public MainWindow()
        {
            InitializeComponent();
            OnShown();

            try
            {
                // Tworzenie nowego obiektu BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                // Ustawienie źródła obrazu na podstawie ścieżki
                bitmapImage.UriSource = new Uri(mapPath, UriKind.RelativeOrAbsolute);

                bitmapImage.EndInit();

                // Przypisanie obrazu do kontrolki Image
                MapBox.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd: " + ex.Message);
            }
        }

        public void OnShown()
        {
            events = _database.GetEvents();

            // Zakładając, że masz kontrolkę ListView o nazwie listViewEvents w MainWindow
            ListOfEvents.Items.Clear(); // Wyczyść istniejące elementy

            foreach (Event ev in events)
            {
                // Dodaj wydarzenie do ListView
                ListOfEvents.Items.Add(new
                {
                    EventName = ev.EventName,
                    //EventLocation = ev.EventLocation,
                    EventDateTime = ev.EventDateTime.ToString(),
                    // Reszta propercji
                });
            }
        }

        private void Window_mousedown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

            // ZMAYKANIE WSZYSTKICH OKIEN TRZEBA ZROBIC BO JAK OTWORZYSZ JAKIES INNE I POTEM WSZYSTKO ZAMKNIESZ TO SIE PROGRAM NIE KONCZY IDK
            this.Close();
            //_profileWindowInstance.Close();
            if(_addWindowInstance != null)
                _addWindowInstance.Close();

            if (_profileWindowInstance != null)
                _profileWindowInstance.Close();
            //_userProfileWindowInstance.Close();

        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_profileWindowInstance == null)
            {
                if(UserService.LoggedInUser == null)
                {
                    _profileWindowInstance = new LoginWindow();
                    _profileWindowInstance.Closed += (s, e) => _profileWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                    _profileWindowInstance.Show();
                }
                else
                {
                    _userProfileWindowInstance = new UserProfileWindow(UserService.LoggedInUser);
                    _userProfileWindowInstance.Closed += (s, e) => _profileWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                    _userProfileWindowInstance.Show();
                }
            }
            else
            {
                _profileWindowInstance.Focus();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_addWindowInstance == null)
            {
                _addWindowInstance = new AddTaskwindow();
                _addWindowInstance.Owner = this;
                _addWindowInstance.EventAdded += AddEventWindow_EventAdded; 
                _addWindowInstance.Closed += (s, e) => _addWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _addWindowInstance.Show();
            }
            else
            {
                _addWindowInstance.Focus();
            }
        }
        private void AddEventWindow_EventAdded(object sender, EventArgs e)
        {
            OnShown();
        }
    }
}
