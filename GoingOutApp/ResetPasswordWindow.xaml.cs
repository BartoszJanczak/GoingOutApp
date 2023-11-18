using System.Security.Cryptography;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using GoingOutApp.Services;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private DataContext _database { get; set; }
        private static RegisterWindow? _registerWindowInstance;
        public ResetPasswordWindow()
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

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;

            var result = EncodePassword(password, 20);
            _database.UpdatePassword(username, result.Item1, result.Item2);
        }
        private Tuple<string, string> EncodePassword(string password, int bytes)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, bytes))
            {
                byte[] salt = deriveBytes.Salt;
                byte[] key = deriveBytes.GetBytes(bytes);

                string encodedSalt = Convert.ToBase64String(salt);
                string encodedKey = Convert.ToBase64String(key);

                return new Tuple<string, string>(encodedSalt, encodedKey);
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

        private void textQuestion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cmbSecurityQuestion.IsDropDownOpen = true;
        }
        
        private void cmbSecurityQuestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSecurityQuestion.SelectedItem != null)
            {
                textQuestion.Visibility = Visibility.Collapsed;
            }
            else
            {
                textQuestion.Visibility = Visibility.Visible;
            }
        }

        private void textAnswer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtAnswer.Focus();
        }

        private void txtAnswer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAnswer.Text) && txtAnswer.Text.Length > 0) 
            {
                textAnswer.Visibility = Visibility.Collapsed;
            }
            else 
            { 
                textAnswer.Visibility = Visibility.Visible;
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

        private void textPassword2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword2.Focus();
        }

        private void txtPassword2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword2.Password) && txtPassword2.Password.Length > 0)
            {
                textPassword2.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword2.Visibility = Visibility.Visible;
            }
        }
    }
}
