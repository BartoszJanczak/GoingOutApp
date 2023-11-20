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

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class EventDetailsWindow : Window
    {
        private DataContext _database { get; set; }
        private int _eventId;
        public EventDetailsWindow(int eventId)
        {
            InitializeComponent();

            _eventId = eventId;
            RefreshDataContext();
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

                    if (isUserSignedUp)
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
                            MessageBox.Show("Już jesteś zapisany na to wydarzenie.");
                            TakePartButton.Visibility = Visibility.Collapsed;
                            CancelParticipationButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // Zapisz użytkownika na wydarzenie
                            dataContext.SignUpForEvent(dataContext, eventId, userId);

                            // Pokaż przycisk "Cancel Participation" i ukryj "Take Part"
                            TakePartButton.Visibility = Visibility.Collapsed;
                            CancelParticipationButton.Visibility = Visibility.Visible;

                            RefreshDataContext();
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
                MessageBox.Show("Błąd: Nie można pobrać danych wydarzenia.");
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
                        }
                        else
                        {
                            MessageBox.Show("Nie jesteś zapisany na to wydarzenie.");
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
                MessageBox.Show("Błąd: Nie można pobrać danych wydarzenia.");
            }
        }
    }
}
