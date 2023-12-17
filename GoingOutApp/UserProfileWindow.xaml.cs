using GoingOutApp.Models;
using GoingOutApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.IO;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for UserProfileWindow.xaml
    /// </summary>
    ///

    public partial class UserProfileWindow : Window
    {
        private DataContext _database { get; set; }
        private User LoggedInUser { get; set; }
        private byte[] photoBytes;

        public UserProfileWindow(User user)
        {
            LoggedInUser = user;
            _database = new DataContext();
            InitializeComponent();
            InitControls();
            ShowParticipatedEvents();
        }

        public void InitControls()
        {
            Name.Text = "Name: " + LoggedInUser.Name;
            Surname.Text = "Surname: " + LoggedInUser.Surname;
            Age.Text = "Age: " + Convert.ToString(LoggedInUser.Age);
            var gender = LoggedInUser.Gender == "Male" ? "Male" : "Female";
            Gender.Text = "Gender: " + gender;
            imgProfile.Source = ByteArrayToBitmapImage(LoggedInUser.PhotoPath);
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void ShowParticipatedEvents()
        {
            List<Event> participatedEvents = _database.GetParticipatedEvents(LoggedInUser.UserId);
            ParticipatedEventsItemsControl.ItemsSource = participatedEvents;
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            UserService.Logout();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EditUserData_Click(object sender, RoutedEventArgs e)
        {
            EditProfileWindow editUserDataWindow = new EditProfileWindow(LoggedInUser);
            editUserDataWindow.Owner = this;
            editUserDataWindow.ShowDialog();

            InitControls();
        }

        private void UpdateUserProfilePhoto(byte[] newPhotoBytes)
        {
            try
            {
                LoggedInUser.PhotoPath = newPhotoBytes;
                _database.UpdateUserPhotoPath(LoggedInUser.UserId, newPhotoBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while updating user photo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    photoBytes = File.ReadAllBytes(filePath);
                    BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                    imgProfile.Source = bitmap;
                    UpdateUserProfilePhoto(photoBytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while opening: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelParticipationButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParticipatedEventsItemsControl.SelectedItems.Count > 0)
            {
                if (UserService.LoggedInUser != null)
                {
                    int userId = UserService.LoggedInUser.UserId;

                    using (DataContext dataContext = new DataContext())
                    {
                        foreach (var selectedItem in ParticipatedEventsItemsControl.SelectedItems)
                        {
                            if (selectedItem is Event selectedEvent)
                            {
                                int eventId = selectedEvent.EventId;

                                bool isUserSignedUp = dataContext.EventParticipants.Any(ep => ep.EventId == eventId && ep.UserId == userId);

                                if (isUserSignedUp)
                                {
                                    dataContext.CancelParticipation(dataContext, eventId, userId);
                                }
                                else
                                {
                                    MessageBox.Show($"You are not signed up for the event '{selectedEvent.EventName}'.");
                                }
                            }
                        }

                        ShowParticipatedEvents();
                    }
                }
                else
                {
                    // Obsługa przypadku, gdy użytkownik nie jest zalogowany
                }
            }
            else
            {
                MessageBox.Show("Please select events to cancel participation.");
            }
        }
    }
}