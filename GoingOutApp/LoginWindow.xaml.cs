using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoingOutApp.Services;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static RegisterWindow? _registerWindowInstance;
        public LoginWindow()
        {
            InitializeComponent();
        }

        public class LoginUserData
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            //string password = PasswordBox.Password;
            txtUser.Text = "Dupa";
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (_registerWindowInstance == null)
            {
                _registerWindowInstance = new RegisterWindow();
                _registerWindowInstance.Closed += (s, e) => _registerWindowInstance = null; // Reset _loginWindowInstance when the window is closed.
                _registerWindowInstance.Show();
            }
            else
            {
                _registerWindowInstance.Focus(); // Skoncentruj się na istniejącym oknie profilu.
            }
        }
        
        private void textUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUser.Focus();
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUser.Text) && txtUser.Text.Length > 0) 
            {
                textUser.Visibility = Visibility.Collapsed;
            }
            else 
            { 
                textUser.Visibility = Visibility.Visible;
            }
        }
        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Password) && txtPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }
    }
}
