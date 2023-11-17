using GoingOutApp.Services;
using GoingOutApp.ViewModel;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public ObservableCollection<PointViewModel> Points { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Points = new ObservableCollection<PointViewModel>()
            {
                new PointViewModel
                {
                    Location = new Location(50.668, 17.925), PinColor= Brushes.Red
                },
                new PointViewModel
                {
                    Location = new Location(54.668, 11.925), PinColor= Brushes.Violet
                },new PointViewModel
                {
                    Location = new Location(49.668, 17.925), PinColor= Brushes.Beige
                },new PointViewModel
                {
                    Location = new Location(50.668, 18.925), PinColor= Brushes.Blue
                }
            };
        }

        private void Window_mousedown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
            }
            else
            {
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                _addWindowInstance.Closed += (s, e) => _addWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _addWindowInstance.Show();
            }
            else
            {
                _addWindowInstance.Focus();
            }
        }
    }
}