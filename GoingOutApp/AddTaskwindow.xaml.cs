using GoingOutApp.Models;
using GoingOutApp.Services;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows.Input;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for AddTaskwindow.xaml
    /// </summary>
    public partial class AddTaskwindow : Window
    {
        private MainWindow _mainWindowInstance = new MainWindow();
        private DataContext _database { get; set; }

        public event EventHandler EventAdded;

        public event EventHandler PinAdded;

        public AddTaskwindow()
        {
            InitializeComponent();
            _database = new DataContext();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string eventName = AddEventName.Text;
            //string PhotoPath = photoPath,
            //string PhotoDescription = photoDescription,
            string eventDescription = AddEventDescription.Text;
            //string EventDateTime = eventDateTime,
            //string NumberOfplaces = numberOfplaces,
            //string OtherInfo = otherInfo
            string eventCity = AddEventCity.Text;
            string eventStreet = AddEventStreet.Text;
            string eventBuildingNumber = AddEventBuilding.Text;

            byte[] photoPath = new byte[3];
            photoPath[0] = byte.MinValue;
            photoPath[1] = 0;
            photoPath[2] = byte.MaxValue;

            _database.AddEvent(eventName, photoPath, "photodesc", eventDescription, eventCity, eventStreet, eventBuildingNumber, DateTime.Now, 5, "otherinfo");
            var location = $"{eventBuildingNumber}, {eventStreet} , {eventCity}";
            var lastEventsId = _database.Events.OrderByDescending(e => e.EventId).FirstOrDefault().EventId;

            var cords = await LocationService.GetLocationByCords(location);

            _database.CreatePushPin(lastEventsId, cords.Latitude, cords.Longitude);

            EventAdded?.Invoke(this, EventArgs.Empty);
            PinAdded?.Invoke(this, EventArgs.Empty);
            Close();
        }
        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddEventName.Focus();
        }

        private void AddEventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AddEventName.Text) && AddEventName.Text.Length > 0)
            {
                textName.Visibility = Visibility.Collapsed;
            }
            else
            {
                textName.Visibility = Visibility.Visible;
            }
        }

        private void textCity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddEventCity.Focus();
        }

        private void AddEventCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AddEventCity.Text) && AddEventCity.Text.Length > 0)
            {
                textCity.Visibility = Visibility.Collapsed;
            }
            else
            {
                textCity.Visibility = Visibility.Visible;
            }
        }

        private void textStreet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddEventStreet.Focus();
        }

        private void AddEventStreet_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AddEventStreet.Text) && AddEventStreet.Text.Length > 0)
            {
                textStreet.Visibility = Visibility.Collapsed;
            }
            else
            {
                textStreet.Visibility = Visibility.Visible;
            }
        }

        private void textBuilding_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddEventBuilding.Focus();
        }

        private void AddEventBuilding_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AddEventBuilding.Text) && AddEventBuilding.Text.Length > 0)
            {
                textBuilding.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBuilding.Visibility = Visibility.Visible;
            }
        }
        private void textDescription_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddEventDescription.Focus();
        }

        private void AddEventDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AddEventDescription.Text) && AddEventDescription.Text.Length > 0)
            {
                textDescription.Visibility = Visibility.Collapsed;
            }
            else
            {
                textDescription.Visibility = Visibility.Visible;
            }
        }
    }
}