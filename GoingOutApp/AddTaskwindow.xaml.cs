using GoingOutApp.Models;
using GoingOutApp.Services;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

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
            string eventDescription = AddEventDesc.Text;
            //string EventDateTime = eventDateTime,
            //string NumberOfplaces = numberOfplaces,
            //string OtherInfo = otherInfo
            string eventCity = txtCity.Text;
            string eventStreet = txtStreet.Text;
            string eventBuildingNumber = txtNumberOfBuilding.Text;

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
    }
}