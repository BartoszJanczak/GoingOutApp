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
using System.Linq;

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

        private List<Event> events = new List<Event>();
        public ObservableCollection<PointViewModel> pushPins { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            OnShown();
            ListOfEvents.Items.Clear();

            events = _database.GetEvents();
            foreach (var ev in events)
            {
                ListOfEvents.Items.Add(ev);
            }
            pushPins = new ObservableCollection<PointViewModel>();
            var pins = Mapper.Map(_database.GetEventPushPins());

            foreach (var pin in pins)
            {
                pushPins.Add(pin);
            }
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
                _addWindowInstance.PinAdded += AddPin_EventAdded;
                _addWindowInstance.Closed += (s, e) => _addWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _addWindowInstance.Show();
            }
            else
            {
                _addWindowInstance.Focus();
            }
        }

        public void RefreshPins()
        {
            var pins = Mapper.Map(_database.GetEventPushPins());

            foreach (var pin in pins)
            {
                pushPins.Add(pin);
            }
        }

        private void AddPin_EventAdded(object? sender, EventArgs e)
        {
            RefreshPins();
        }

        private void AddEventWindow_EventAdded(object sender, EventArgs e)
        {
            OnShown();
        }

        // Stary kod na wszelki wypadek z metody ListOfEvents_MouseDoubleClick
        //private void ListOfEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (ListOfEvents.SelectedItem is Event selectedEvent)
        //    {
        //        EventDetailsWindow eventDetailsWindow = new EventDetailsWindow(selectedEvent.EventId);
        //        eventDetailsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        //        eventDetailsWindow.Left = this.Left + 15;
        //        eventDetailsWindow.Top = this.Top + 80;
        //        eventDetailsWindow.Show();

        //        var eventPin = pushPins.Where(p => p.EventId == selectedEvent.EventId).First();
        //        Map.ZoomLevel = 15;

        //        Map.Center = eventPin.Location;
        //    }
        //}

        private void ListOfEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListOfEvents.SelectedItem is Event selectedEvent)
            {
                EventDetailsWindow existingWindow = Application.Current.Windows.OfType<EventDetailsWindow>().FirstOrDefault();
                EventDetailsWindow eventDetailsWindow = new EventDetailsWindow(selectedEvent.EventId);

                if (existingWindow == null)
                {
                    eventDetailsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    eventDetailsWindow.Left = this.Left + 15;
                    eventDetailsWindow.Top = this.Top + 80;
                    eventDetailsWindow.Show();

                    var eventPin = pushPins.Where(p => p.EventId == selectedEvent.EventId).FirstOrDefault();

                    if (eventPin != null)
                    {
                        Map.ZoomLevel = 15;
                        Map.Center = eventPin.Location;
                    }
                }
                else
                {
                    existingWindow.Close();

                    eventDetailsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    eventDetailsWindow.Left = this.Left + 15;
                    eventDetailsWindow.Top = this.Top + 80;
                    eventDetailsWindow.Show();

                    var eventPin = pushPins.Where(p => p.EventId == selectedEvent.EventId).FirstOrDefault();

                    if (eventPin != null)
                    {
                        Map.ZoomLevel = 15;
                        Map.Center = eventPin.Location;
                    }
                }
            }
        }

        private void Pushpin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Pushpin pushpin)
            {
                var lokalizacja = pushpin.Location;
                var clickedEvent = _database.EventPushPins.Where(e => e.X == lokalizacja.Latitude && e.Y == lokalizacja.Longitude).FirstOrDefault().EventId;

                var esa = _database.GetEvent(clickedEvent);


                EventDetailsWindow eventDetailsWindow = new EventDetailsWindow(esa.EventId);

                eventDetailsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                eventDetailsWindow.Left = this.Left + 15;
                eventDetailsWindow.Top = this.Top + 80;
                eventDetailsWindow.Show();

                var eventPin = pushPins.Where(p => p.EventId == clickedEvent).First();

                Map.Center = eventPin.Location;
                Map.ZoomLevel = 15;
            }
        }

        private void Pushpin_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // Zmiana kształtu kursora na dłoń po najechaniu
                element.Cursor = Cursors.Hand;
            }
        }

        private void Pushpin_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // Przywrócenie domyślnego kształtu kursora po opuszczeniu
                element.Cursor = Cursors.Arrow;
            }
        }
    }
}