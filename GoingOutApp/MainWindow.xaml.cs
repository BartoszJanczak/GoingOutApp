using GoingOutApp.Models;
using GoingOutApp.Services;
using GoingOutApp.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static LoginWindow? _profileWindowInstance;
        private static AddTaskwindow? _addWindowInstance;
        private static UserProfileWindow? _userProfileWindowInstance;
        private DataContext _database { get; set; } = new DataContext();

        public ObservableCollection<PointViewModel> Points { get; set; }

        private List<Event> events = new List<Event>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            OnShown();
            ListOfEvents.Items.Clear();

            List<Event> events = _database.GetEvents();
            foreach (var ev in events)
            {
                ListOfEvents.Items.Add(ev);
            }


            Points = new ObservableCollection<PointViewModel>();
        }

        public void OnShown()
        {
            events = _database.GetEvents();

            ListOfEvents.Items.Clear();
            ListOfEvents.DisplayMemberPath = "EventName";

            foreach (Event ev in events)
            {
                ListOfEvents.Items.Add(ev);
            }
        }

        private async void Window_mousedown(object sender, MouseButtonEventArgs e)
        {
            var location = "7d, 98-400 Górka Wieruszowska, Polska";
            if (e.ChangedButton == MouseButton.Right)
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"http://dev.virtualearth.net/REST/v1/Locations?query={location}&key=tdR8B4UFCok6HiAPmoQ3~K8lYPO2jpRrn2Eo7sfgHRQ~ArKu6p1ZhDGu_ekMQ6eam5QBW67AVHme_OOL_4LkpzH0P8ScgJT2w-UtzHnjRbr4";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        JObject jObject = JObject.Parse(json);

                        var geocodePoints = jObject.SelectTokens("$.resourceSets[0].resources[0].geocodePoints[*].coordinates");

                        List<System.Windows.Point> points = new List<System.Windows.Point>();

                        foreach (var coordinates in geocodePoints)
                        {
                            double latitude = Convert.ToDouble(coordinates[0]);
                            double longitude = Convert.ToDouble(coordinates[1]);

                            points.Add(new System.Windows.Point(latitude, longitude));
                        }
                        Points.Add(Mapper.Map(points[0]));
                    }
                    else
                    {
                        Console.WriteLine($"Błąd: {response.StatusCode}");
                    }
                }
            }
            else
            {
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // ZMAYKANIE WSZYSTKICH OKIEN TRZEBA ZROBIC BO JAK OTWORZYSZ JAKIES INNE I POTEM WSZYSTKO ZAMKNIESZ TO SIE PROGRAM NIE KONCZY IDK
            this.Close();
            //_profileWindowInstance.Close();
            if (_addWindowInstance != null)
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
                if (UserService.LoggedInUser == null)
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

        private void ListOfEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListOfEvents.SelectedItem is Event selectedEvent)
            {
                EventDetailsWindow eventDetailsWindow = new EventDetailsWindow(selectedEvent);
                eventDetailsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                eventDetailsWindow.Left = this.Left + 15;
                eventDetailsWindow.Top = this.Top + 80;
                eventDetailsWindow.Show();
            }
        }
    }
}