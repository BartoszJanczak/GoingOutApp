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

        private string defaultNumberOfPlacesValue = "0";

        public enum EventCategory
        {
            Social,
            Show,
            Sport
        }

        public AddTaskwindow()
        {
            InitializeComponent();
            _database = new DataContext();
            AddEventNumberOfPlaces.Text = defaultNumberOfPlacesValue;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddEventName.Text.Length > 0 && AddEventCity.Text.Length > 0 && AddEventStreet.Text.Length > 0 && AddEventBuilding.Text.Length > 0 && AddEventNumberOfPlaces.Text.Length > 0 && AddEventDate.Text.Length > 0 && AddEventDescription.Text.Length > 0)
            {
                if (int.TryParse(AddEventBuilding.Text, out _) && int.TryParse(AddEventNumberOfPlaces.Text, out _))
                {
                    string eventName = AddEventName.Text;
                    //string PhotoPath = photoPath,
                    string eventDescription = AddEventDescription.Text;
                    string eventDate = string.Empty;
                    if (!string.IsNullOrEmpty(AddEventDate.Text))
                    {
                        eventDate = AddEventDate.Text;
                    }
                    else
                    {
                        eventDate = DateTime.Now.ToString();
                    }
                    int numberOfPlaces = int.Parse(AddEventNumberOfPlaces.Text);
                    //string OtherInfo = otherInfo
                    string eventCity = AddEventCity.Text;
                    string eventStreet = AddEventStreet.Text;
                    string eventBuildingNumber = AddEventBuilding.Text;

                    byte[] photoPath = new byte[3];
                    photoPath[0] = byte.MinValue;
                    photoPath[1] = 0;
                    photoPath[2] = byte.MaxValue;

                    EventCategory eventCategoryEnum = (EventCategory)cmbCategory.SelectedIndex;
                    string eventCategory = eventCategoryEnum.ToString();

                    _database.AddEvent(eventName, photoPath, "photodesc", eventDescription, eventCity, eventStreet, eventBuildingNumber, eventDate, numberOfPlaces, "otherinfo", eventCategory);
                    var location = $"{eventBuildingNumber}, {eventStreet} , {eventCity}";
                    var lastEventsId = _database.Events.OrderByDescending(e => e.EventId).FirstOrDefault().EventId;

                    var cords = await LocationService.GetLocationByCords(location);

                    _database.CreatePushPin(lastEventsId, cords.Latitude, cords.Longitude);

                    EventAdded?.Invoke(this, EventArgs.Empty);
                    PinAdded?.Invoke(this, EventArgs.Empty);
                    Close();
                }
                else
                {
                    txtNumberInfo.Text = "Number of places should be a number";
                    txtPlacesInfo.Text = "Number of building should be a number";
                }
            }
            else
            {
                MessageBox.Show("Please fill all");
            }
        }

        private void text_MouseDown(Control control)
        {
            control.Focus();
        }

        private void AddEvent_TextChanged(string AddEvent, TextBlock textBlock)
        {
            if (!string.IsNullOrEmpty(AddEvent) && AddEvent.Length > 0)
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }

        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            text_MouseDown(AddEventName);
        }

        private void AddEventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEvent_TextChanged(AddEventName.Text, textName);
        }

        private void textCity_MouseDown(object sender, MouseButtonEventArgs e)
        {
            text_MouseDown(AddEventCity);
        }

        private void AddEventCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEvent_TextChanged(AddEventCity.Text, textCity);
        }

        private void textStreet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            text_MouseDown(AddEventStreet);
        }

        private void AddEventStreet_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEvent_TextChanged(AddEventStreet.Text, textStreet);
        }

        private void textBuilding_MouseDown(object sender, MouseButtonEventArgs e)
        {
            text_MouseDown(AddEventBuilding);
        }

        private void AddEventBuilding_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEvent_TextChanged(AddEventBuilding.Text, textBuilding);
        }

        private void textDescription_MouseDown(object sender, MouseButtonEventArgs e)
        {
            text_MouseDown(AddEventDescription);
        }

        private void AddEventDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEvent_TextChanged(AddEventDescription.Text, textDescription);
        }

        private void textNumberOfPlaces_MouseDown(object sender, MouseButtonEventArgs e)
        {
            text_MouseDown(AddEventNumberOfPlaces);
        }

        private void AddEventNumberOfPlaces_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddEvent_TextChanged(AddEventNumberOfPlaces.Text, textNumberOfPlaces);
        }

        private void LimitPlacesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            AddEventNumberOfPlaces.IsEnabled = true;
        }

        private void LimitPlacesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            AddEventNumberOfPlaces.IsEnabled = false;
            AddEventNumberOfPlaces.Text = defaultNumberOfPlacesValue;
        }

        static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}