using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using GoingOutApp.Models;
using GoingOutApp.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Media.Imaging;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class EventDetailsWindow : Window
    {
        private DataContext _database { get; set; } = new DataContext();
        private int _eventId;
        private List<string> participants = new List<string>();
        private static AddTaskwindow? _addWindowInstance;

        public event EventHandler DeletedEvent;

        public EventDetailsWindow(int eventId)
        {
            InitializeComponent();

            _eventId = eventId;
            RefreshDataContext();
            LoadPhoto();
            SetList();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RefreshDataContext()
        {
            using (DataContext dataContext = new DataContext())
            {
                Event selectedEvent = dataContext.GetEvent(_eventId);
                DataContext = selectedEvent;
                categorytxt.Text = selectedEvent.EventCategory.ToString();
                if (selectedEvent.NumberOfplaces == 0)
                {
                    ParticipantsTextBlock.Text = $"{selectedEvent.TakenPlaces}";
                }
                else
                {
                    ParticipantsTextBlock.Text = $"{selectedEvent.TakenPlaces}/{selectedEvent.NumberOfplaces} places taken";
                }
                if (UserService.LoggedInUser != null)
                {
                    int userId = UserService.LoggedInUser.UserId;
                    var numberOfLikes = dataContext.GetNumberOfLikes(_eventId);
                    if (dataContext.doesUserLikeEvent(userId, _eventId))
                    {
                        SetUnlikeBtn();
                    }
                    else
                    {
                        SetLikeBtn();
                    }
                }
            }
        }

        private void SetLikeBtn()
        {
            using (DataContext dataContext = new DataContext())
            {
                var numberOfLikes = dataContext.GetNumberOfLikes(_eventId);
                Uri uri = new Uri("/data/images/like.png", UriKind.Relative);
                BitmapImage bitmapImage = new BitmapImage(uri);
                System.Windows.Controls.Image image = new();
                image.Source = bitmapImage;
                image.Width = 20;
                image.Height = 20;
                likeButton.Foreground = new SolidColorBrush(Colors.Black);
                likeButton.Background = new SolidColorBrush(Colors.Green);
                likeButton.Cursor = Cursors.Hand;

                likeButton.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                                        {
                                            image,
                                            new TextBlock { Text = numberOfLikes.ToString(), Foreground = new SolidColorBrush(Colors.Black), Margin = new Thickness(5,0,0,0) }
                                        }
                };
            }
        }

        private void SetUnlikeBtn()
        {
            using (DataContext dataContext = new DataContext())
            {
                var numberOfLikes = dataContext.GetNumberOfLikes(_eventId);
                Uri uri = new Uri("/data/images/unlike.png", UriKind.Relative);
                BitmapImage bitmapImage = new BitmapImage(uri);
                System.Windows.Controls.Image image = new();
                image.Source = bitmapImage;
                image.Width = 20;
                image.Height = 20;
                likeButton.Foreground = new SolidColorBrush(Colors.Black);
                likeButton.Background = new SolidColorBrush(Colors.Red);
                likeButton.Cursor = Cursors.Hand;

                likeButton.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        image,
                        new TextBlock { Text = numberOfLikes.ToString(), Margin = new Thickness(5, 0, 0, 0) },
                    }
                };
            }
        }

        private void EventDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Event selectedEvent && UserService.LoggedInUser != null)
            {
                int eventId = selectedEvent.EventId;
                int userId = UserService.LoggedInUser.UserId;

                using (DataContext dataContext = new DataContext())
                {
                    var creatorId = dataContext.Events.FirstOrDefault(e => e.EventId == eventId)?.EventCreatorId;

                    if (creatorId == userId)
                    {
                        EditEvent.Visibility = Visibility.Visible;
                        DeleteEvent.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        EditEvent.Visibility = Visibility.Collapsed;
                        DeleteEvent.Visibility = Visibility.Collapsed;
                    }

                    if (dataContext.doesUserLikeEvent(userId, _eventId))
                    {
                        SetUnlikeBtn();
                    }
                    else
                    {
                        SetLikeBtn();
                    }
                    likeButton.IsEnabled = true;

                    bool isUserSignedUpToEvent = dataContext.EventParticipants.Any(ep => ep.EventId == eventId && ep.UserId == userId);

                    if (isUserSignedUpToEvent)
                    {
                        TakePartButton.Visibility = Visibility.Collapsed;
                        CancelParticipationButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TakePartButton.Visibility = Visibility.Visible;
                        CancelParticipationButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                likeButton.IsEnabled = false;
                CancelParticipationButton.Visibility = Visibility.Collapsed;
                EditEvent.Visibility = Visibility.Collapsed;
                DeleteEvent.Visibility = Visibility.Collapsed;
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Event selectedEvent)
            {
                if (UserService.LoggedInUser != null)
                {
                    int eventId = selectedEvent.EventId;
                    int userId = UserService.LoggedInUser.UserId;

                    using (DataContext dataContext = new DataContext())
                    {
                        bool isUserSignedUp = dataContext.EventParticipants.Any(ep => ep.EventId == eventId && ep.UserId == userId);

                        if (isUserSignedUp)
                        {
                            MessageBox.Show("You are already signed up for this event.");
                            TakePartButton.Visibility = Visibility.Collapsed;
                            CancelParticipationButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (selectedEvent.NumberOfplaces == 0 || selectedEvent.TakenPlaces < selectedEvent.NumberOfplaces)
                            {
                                dataContext.SignUpForEvent(dataContext, eventId, userId);

                                TakePartButton.Visibility = Visibility.Collapsed;
                                CancelParticipationButton.Visibility = Visibility.Visible;

                                RefreshDataContext();
                                SetList();
                            }
                            else
                            {
                                MessageBox.Show("The event is already full. Cannot sign up.");
                            }
                        }
                    }
                }
                else
                {
                    Close();
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Unable to download event data.");
            }
        }

        private void CancelParticipationButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Event selectedEvent)
            {
                if (UserService.LoggedInUser != null)
                {
                    int eventId = selectedEvent.EventId;
                    int userId = UserService.LoggedInUser.UserId;

                    using (DataContext dataContext = new DataContext())
                    {
                        bool isUserSignedUp = dataContext.EventParticipants.Any(ep => ep.EventId == eventId && ep.UserId == userId);

                        if (isUserSignedUp)
                        {
                            dataContext.CancelParticipation(dataContext, eventId, userId);

                            TakePartButton.Visibility = Visibility.Visible;
                            CancelParticipationButton.Visibility = Visibility.Collapsed;

                            RefreshDataContext();
                            SetList();
                        }
                        else
                        {
                            MessageBox.Show("You are not signed up for this event.");
                        }
                    }
                }
                else
                {
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Unable to download event data.");
            }
        }

        private void SetList()
        {
            listOfParticipants.Items.Clear();
            participants = _database.GetParticipants(_eventId);

            foreach (string ep in participants)
            {
                listOfParticipants.Items.Add(ep);
            }
        }

        private void LoadPhoto()
        {
            try
            {
                byte[] photoBytes = _database.GetPhoto(_eventId);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(photoBytes);
                bitmap.EndInit();

                photo.Source = bitmap;
            }
            catch (Exception ex)
            {
                photo.Source = null;
            }
        }

        private void ShowParticipants_Click(object sender, RoutedEventArgs e)
        {
            lblList.Visibility = Visibility.Visible;
            listOfParticipants.Visibility = Visibility.Visible;
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            using (DataContext dataContext = new DataContext())
            {
                Event selectedEvent = dataContext.GetEvent(_eventId);
                _addWindowInstance = new AddTaskwindow(selectedEvent);
                _addWindowInstance.Owner = this;
                _addWindowInstance.Show();
                _addWindowInstance.Closed += AddWindowInstance_Closed;
            }
        }

        private void AddWindowInstance_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Event selectedEvent && UserService.LoggedInUser != null)
            {
                int eventId = selectedEvent.EventId;
                int userId = UserService.LoggedInUser.UserId;

                using (DataContext dataContext = new DataContext())
                {
                    var creatorId = dataContext.Events.FirstOrDefault(ev => ev.EventId == eventId)?.EventCreatorId;

                    if (creatorId == userId)
                    {
                        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this event?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            dataContext.DeleteEvent(eventId);
                            dataContext.DeletePin(eventId);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are not authorized to delete this event.", "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void LikeButtonClick(object sender, RoutedEventArgs e)
        {
            if (UserService.LoggedInUser != null)
            {
                int userId = UserService.LoggedInUser.UserId;

                using (DataContext dataContext = new DataContext())
                {
                    if (dataContext.doesUserLikeEvent(userId, _eventId))
                    {
                        dataContext.DeleteLike(userId, _eventId);
                    }
                    else
                    {
                        dataContext.AddLike(userId, _eventId);
                    }
                }
                RefreshDataContext();
            }
        }
    }
}