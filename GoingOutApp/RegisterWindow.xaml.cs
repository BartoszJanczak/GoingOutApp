using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public class RegisterUserData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Age { get; set; }
            public string Gender { get; set; }
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
            //UserData userData = new UserData()
            //{
            //    Username = UsernameTextBox.Text,
            //    Password = PasswordTextBox.Text,
            //    Name = NameTextBox.Text,
            //    Surname = SurnameTextBox.Text,
            //    Age = AgeTextBox.Text,
            //    Gender = MaleRadioButton.IsChecked == true ? "Male" : "Female"
            //};
            string username = txtUser.Text;
            string password = txtPassword.Password;
            string name = txtName.Text;
            string surname = txtSurname.Text;
            string age = txtAge.Text;
        }
        private void AgeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, "[0-9]");
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
        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Focus();
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text.Length > 0)
            {
                textName.Visibility = Visibility.Collapsed;
            }
            else
            {
                textName.Visibility = Visibility.Visible;
            }
        }

        private void textSurname_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSurname.Focus();
        }

        private void txtSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSurname.Text) && txtSurname.Text.Length > 0)
            {
                textSurname.Visibility = Visibility.Collapsed;
            }
            else
            {
                textSurname.Visibility = Visibility.Visible;
            }
        }

        private void textAge_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtAge.Focus();
        }

        private void txtAge_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAge.Text) && txtAge.Text.Length > 0)
            {
                textAge.Visibility = Visibility.Collapsed;
            }
            else
            {
                textAge.Visibility = Visibility.Visible;
            }
        }
    }
}
