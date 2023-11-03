using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GoingOutApp.Models;
using GoingOutApp.Services;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public List<User> ?DatabaseUsers { get; private set; }

        private DataContext _database { get; set; }
        public RegisterWindow()
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
            int age = Convert.ToInt32(txtAge.Text);
            //var gender = MaleRadioButton.IsChecked == true ? "Male" : "Female";

            if(AccountValidation(username, password, name, surname, age, "M"))
            {
                var result = EncodePassword(password, 20); 
                _database.CreateAccount(username, result.Item1, result.Item2, name, surname, age, "M");
            }
            else
            {
                // Wyświetlenie błędu

            }

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

        private void AgeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, "[0-9]");
        }
       
        private void RefreshData()
        {
            DatabaseUsers = _database.Users.ToList();
        }
       
        private bool AccountValidation(string username, string password, string name, string surname, int age, string gender)
        {
            bool validation;
            // sprawdzenie czy wprowadzono poprawne dane
            validation = ifAccountWithThatUserNameCanBeCreated(username);
            //validation = Kolejna metoda do walidacji 
            //validation = Kolejna metoda do walidacji 
            //validation = Kolejna metoda do walidacji 
            return validation;
        }

        #region ValidationMethods
        private bool ifAccountWithThatUserNameCanBeCreated(string login)
        {
            RefreshData();
            var ifUserExists = DatabaseUsers.Any(u => u.UserName == login);

            return !ifUserExists;
        }

        // tutaj twórz metody do walidacji 
        #endregion
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
