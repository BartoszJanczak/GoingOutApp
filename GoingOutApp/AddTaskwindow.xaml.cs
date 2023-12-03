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
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Markup;

namespace GoingOutApp
{
    public enum TrybOkna
    {
        New,
        Edit
    }
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

        private byte[] photoBytes;

        private TrybOkna tryb;

        private GoingOutApp.Models.Event eventToEdit;

        User user { get; set; }

        public enum EventCategory
        {
            Social,
            Show,
            Sport
        }


        public AddTaskwindow()
        {
            tryb = TrybOkna.New;
            InitializeComponent();
            _database = new DataContext();
            AddEventNumberOfPlaces.Text = defaultNumberOfPlacesValue;
            TitleText.Text = "Add new event";
            CancelButton.Visibility = Visibility.Hidden;
        }

        public AddTaskwindow(GoingOutApp.Models.Event eventToEdit)
        {
            tryb = TrybOkna.Edit;
            this.eventToEdit = eventToEdit;
            InitializeComponent();
            _database = new DataContext();
            TitleText.Text = "Edit your event";
            SubmitButton.Content = "Save";
            CancelButton.Visibility = Visibility.Visible;

            AddEventName.Text = eventToEdit.EventName;
            AddEventCity.Text = eventToEdit.City;
            AddEventCity.IsEnabled = false;
            AddEventStreet.Text = eventToEdit.Street;
            AddEventStreet.IsEnabled = false;
            AddEventBuilding.Text = eventToEdit.NumberOfBuilding;
            AddEventBuilding.IsEnabled = false;
            cmbCategory.Text = eventToEdit.EventCategory; 
            AddEventDate.Text = eventToEdit.EventDateTime;
            AddEventDescription.Text = eventToEdit.EventDescription;
            if(eventToEdit.NumberOfplaces != 0)
            {
                LimitPlacesCheckbox.IsChecked = true;
                AddEventNumberOfPlaces.Text = Convert.ToString(eventToEdit.NumberOfplaces);
            }
            try
            {
                photoBytes = _database.GetPhoto(eventToEdit.EventId);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(photoBytes);
                bitmap.EndInit();

                imagePreview.Source = bitmap;
            }
            catch (Exception ex)
            {
                imagePreview.Source = null;
            }
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


                    if (tryb == TrybOkna.New)
                    {
                        var obiekt = new GoingOutApp.Models.Event();
                    }

                    user = UserService.LoggedInUser;
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
                    int eventCreatorId = user.UserId;

                    //byte[] photoPath = new byte[3];
                    //photoPath[0] = byte.MinValue;
                    //photoPath[1] = 0;
                    //photoPath[2] = byte.MaxValue;

                    EventCategory eventCategoryEnum = (EventCategory)cmbCategory.SelectedIndex;
                    string eventCategory = eventCategoryEnum.ToString();

                    if (photoBytes == null)
                    {
                        photoBytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/../../.." + "/data/images/concert.jpg");
                    }
                    _database.AddEvent(eventCreatorId, eventName, photoBytes, "photodesc", eventDescription, eventCity, eventStreet, eventBuildingNumber, eventDate, numberOfPlaces, "otherinfo", eventCategory);
                    var location = $"{eventBuildingNumber}, {eventStreet} , {eventCity}";
                    var lastEventsId = _database.Events.OrderByDescending(e => e.EventId).FirstOrDefault().EventId;
                    if (tryb == TrybOkna.New)
                    {
                        _database.AddEvent(eventCreatorId, eventName, photoBytes, "photodesc", eventDescription, eventCity, eventStreet, eventBuildingNumber, eventDate, numberOfPlaces, "otherinfo", eventCategory);
                        var location = $"{eventBuildingNumber}, {eventStreet} , {eventCity}";
                        var lastEventsId = _database.Events.OrderByDescending(e => e.EventId).FirstOrDefault().EventId;

                        var cords = await LocationService.GetLocationByCords(location);

                        _database.CreatePushPin(lastEventsId, cords.Latitude, cords.Longitude);

                        EventAdded?.Invoke(this, EventArgs.Empty);
                        PinAdded?.Invoke(this, EventArgs.Empty);

                    }
                    else if(tryb == TrybOkna.Edit)
                    {

                        var objectToUpdate = new GoingOutApp.Models.Event(eventCreatorId, eventName, photoBytes, "photodesc", eventDescription, eventCity, eventStreet, eventBuildingNumber, eventDate, numberOfPlaces, "otherinfo", eventCategory);
                        objectToUpdate.EventId = eventToEdit.EventId;
                        _database.UpdateEvent(objectToUpdate);
                        EventAdded?.Invoke(this, EventArgs.Empty);


                    }

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

        #region metody do klikania
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

#endregion

        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter

            if (openFileDialog.ShowDialog() == true) 
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    photoBytes = File.ReadAllBytes(filePath);
                    BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                    imagePreview.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while opening: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}