using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GoingOutApp.Services;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private DataContext _database { get; set; }
        private static RegisterWindow? _registerWindowInstance;
        private bool isPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
            _database = new DataContext();
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
            var username = txtUser.Text;
            var password = txtPassword.Password;
            var correctData = UserService.ValidateSignIn(username, password);
            if (correctData)
            {
                var result = MessageBox.Show("Successfully logged in.");
                if (result == MessageBoxResult.OK)
                {
                    this.Close();
                    UserProfileWindow user = new UserProfileWindow(UserService.LoggedInUser);
                    user.Show();
                }
            }
            else
            {
                MessageBox.Show("Incorrect data was entered.");
            }
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

        private void ResetLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ResetPasswordWindow resetWindow = new ResetPasswordWindow();
            resetWindow.ShowDialog();
        }

        private void ResetLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void ResetLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void EyeImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                eyeImage.Source = new BitmapImage(new Uri("/data/images/hide.png", UriKind.Relative));
                passwordTxtBox.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                passwordTxtBox.Visibility = Visibility.Visible;
            }
            else
            {
                eyeImage.Source = new BitmapImage(new Uri("/data/images/show.png", UriKind.Relative));
                txtPassword.Password = passwordTxtBox.Text;
                passwordTxtBox.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
            }
        }

        private void PasswordTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                txtPassword.Password = passwordTxtBox.Text;
            }
        }
    }
}