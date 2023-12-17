using GoingOutApp.Models;
using GoingOutApp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {
        private EventDetailsWindow _eventDetailsWindowInstance;
        private DataContext _database { get; set; } = new DataContext();

        private List<Event> events = new List<Event>();
        public static User LoggedInUser { get; private set; }


        public CalendarWindow()
        {
            InitializeComponent();
            OnShown();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void RefreshEvents()
        {
            var events = _database.GetEvents();

            ListOfEvents.Items.Clear();
            foreach (var ev in events)
            {
                ListOfEvents.Items.Add(ev);
            }
        }

        private void ListOfEvents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListOfEvents.SelectedItem is Event selectedEvent)
            {
                EventDetailsWindow existingWindow = Application.Current.Windows.OfType<EventDetailsWindow>().FirstOrDefault();
                EventDetailsWindow eventDetailsWindow = new EventDetailsWindow(selectedEvent.EventId);

                if (existingWindow != null)
                {
                    existingWindow.Close();
                }

                eventDetailsWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                eventDetailsWindow.Left = this.Left + 15;
                eventDetailsWindow.Top = this.Top + 80;
                eventDetailsWindow.Show();
                _eventDetailsWindowInstance = eventDetailsWindow;
                //eventDetailsWindow.Closed += EventDetailsWindow_Closed;

            }
        }

        //private void EventDetailsWindow_Closed(object? sender, EventArgs e)
        //{
        //    RefreshEvents();
        //}

        public void OnShown()
        {
            LoadData();

            ListOfEvents.Items.Clear();
            ListOfEvents.DisplayMemberPath = "EventName";

            foreach (Event ev in events)
            {
                ListOfEvents.Items.Add(ev);
            }
        }

        private void LoadData()
        {
            events = _database.GetEvents();
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = calendar.SelectedDate.Value;
                ShowEventsForSelectedDate(selectedDate);
            }
        }

        private void ShowEventsForSelectedDate(DateTime selectedDate)
        {
            var userId = UserService.LoggedInUser.UserId;

            var eventsForDate = events
                .Where(ev => DateTime.ParseExact(ev.EventDateTime, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date == selectedDate.Date &&
                             (_database.EventParticipants.Any(ep => ep.EventId == ev.EventId && ep.UserId == userId) || ev.EventCreatorId == userId))
                .ToList();

            ListOfEvents.Items.Clear();
            foreach (var ev in eventsForDate)
            {
                ListOfEvents.Items.Add(ev);
            }
        }


    }
}
