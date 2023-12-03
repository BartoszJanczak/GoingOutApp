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

                if (selectedEvent.NumberOfplaces == 0)
                {
                    ParticipantsTextBlock.Text = $"{selectedEvent.TakenPlaces}";
                }
                else
                {
                    ParticipantsTextBlock.Text = $"{selectedEvent.TakenPlaces}/{selectedEvent.NumberOfplaces} places taken";
                }
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
                    bool isUserSignedUp = dataContext.EventParticipants.Any(ep => ep.EventId == eventId && ep.UserId == userId);

                    var creatorId = dataContext.Events.FirstOrDefault(e => e.EventId == eventId).EventCreatorId;
                    
                  

                    if (isUserSignedUp)
                    {
                        if (userId == creatorId)
                        {
                            EditEvent.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            EditEvent.Visibility = Visibility.Collapsed;
                        }

                        TakePartButton.Visibility = Visibility.Collapsed;
                        CancelParticipationButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TakePartButton.Visibility = Visibility.Visible;
                        CancelParticipationButton.Visibility = Visibility.Collapsed;
                        EditEvent.Visibility = Visibility.Collapsed;

                    }
                }
            }
            else
            {
                CancelParticipationButton.Visibility = Visibility.Collapsed;
                EditEvent.Visibility = Visibility.Collapsed;
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
                _addWindowInstance = new AddTaskwindow(selectedEvent) ;
                _addWindowInstance.Owner = this;
                _addWindowInstance.Show();
                _addWindowInstance.Closed += AddWindowInstance_Closed;
            }
               
        }
        private void AddWindowInstance_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}