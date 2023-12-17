using GoingOutApp.Services;
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

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for YesNoWindow.xaml
    /// </summary>
    public partial class YesNoWindow : Window
    {
        private static LoginWindow? _profileWindowInstance;
        private static RegisterWindow? _registerWindowInstance;
        private static AddTaskwindow? _addWindowInstance;
        private static UserProfileWindow? _userProfileWindowInstance;
        private static EventDetailsWindow? _eventDetailsWindowInstance;
        private static ResetPasswordWindow? _resetPasswordWindowInstance;
        private static AboutUs? _aboutUsInstance;
        private static System.Windows.Point Cords;
        public YesNoWindow(double x,double y)
        {
            InitializeComponent();
            Cords.X = x;
            Cords.Y = y;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UserService.LoggedInUser != null)
            {
                if (_addWindowInstance == null)
                {
                    _addWindowInstance = new AddTaskwindow(Cords.X,Cords.Y);
                    _addWindowInstance.Owner = this;
                    _addWindowInstance.EventAdded += AddEventWindow_EventAdded;
                    _addWindowInstance.PinAdded += AddPin_EventAdded;
                    _addWindowInstance.Closed += (s, e) => _addWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                    _addWindowInstance.Closed += (s, e) => this.Close(); // Reset _profileWindowInstance when the window is closed.
                    _addWindowInstance.Show();
                }
                else
                {
                    _addWindowInstance.Focus();
                }
            }
            else
            {
                _profileWindowInstance = new LoginWindow();
                _profileWindowInstance.Closed += (s, e) => _profileWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _profileWindowInstance.Show();
            }
        }

        private void AddEventWindow_EventAdded(object sender, EventArgs e)
        {
            
        }     
        private void AddPin_EventAdded(object sender, EventArgs e)
        {
           
        }
    }
}
