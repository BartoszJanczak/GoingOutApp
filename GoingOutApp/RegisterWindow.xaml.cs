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
    public partial class RegisterWindow : Window
    {
        public List<User> ?DatabaseUsers { get; private set; }

        private DataContext _database { get; set; }

        string selectedGender = "";
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
            string username = txtUser.Text;
            string password = txtPassword.Password;
            string name = txtName.Text;
            string surname = txtSurname.Text;
            int age = 0;
            string gender = selectedGender.ToString();

            try
            {
                age = Convert.ToInt32(txtAge.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Wprowadzono nieprawidłowy wiek. Proszę wpisać liczbę całkowitą.");
                return;
            }
            if (AccountValidation(username, password, name, surname, age, gender))
            {
                var result = EncodePassword(password, 20); 
                _database.CreateAccount(username, result.Item1, result.Item2, name, surname, age, gender);
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
            if (!ifAccountWithThatUserNameCanBeCreated(username))
            {
                MessageBox.Show("Konto o podanej nazwie użytkownika już istnieje.");
                return false;
            }
            if (!ifAllFieldsAreCompleted(username, password, name, surname, age))
            {
                MessageBox.Show("Proszę wypełnić wszystkie wymagane pola.");
                return false;
            }
            if (!ifGenderIsSelected())
            {
                MessageBox.Show("Proszę wybrać płeć.");
                return false;
            }
            if (!ifStrongPassword(password))
            {
                MessageBox.Show("Hasło musi się składać z minimum 8 znaków, w tym zawierać duże i małe litery, cyfry i znaki specjalne (np. !,@,#,$,%).");
                return false;
            }
            if (!ifNameValid(name))
            {
                MessageBox.Show("Imię musi zaczynać się z wielkiej litery i nie może zawierać cyfr.");
                return false;
            }
            if (!ifSurnameValid(surname))
            {
                MessageBox.Show("Nazwisko musi zaczynać się z wielkiej litery i nie może zawierać cyfr.");
                return false;
            }
            if(!ifAgeValid(age))
            {
                MessageBox.Show("Podaj poprawny wiek");
                return false;
            }
            MessageBox.Show("Pomyślnie zarejestrowano konto.");
            Close();
            return true;
        }

        #region ValidationMethods
        private bool ifAccountWithThatUserNameCanBeCreated(string login)
        {
            RefreshData();
            var ifUserExists = DatabaseUsers.Any(u => u.UserName == login);

            return !ifUserExists;
        }

        private bool ifAllFieldsAreCompleted(string username, string password, string name, string surname, int age)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(surname) ||
                age.ToString() == "")
            {
                return false;
            }
            return true;
        }

        private bool ifGenderIsSelected()
        {
            if (string.IsNullOrWhiteSpace(selectedGender))
            {
                return false;
            }
            return true;
        }

        private bool ifStrongPassword(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, "[!@#\\$%^&*()]"))
            {
                return false;
            }

            return true;
        }
        private bool ifNameValid(string name)
        {
            if (!Regex.IsMatch(name, "^[A-Z][a-zA-Z]*$"))
            {
                return false;
            }
            return true;
        }

        private bool ifSurnameValid(string surname)
        {
            if (!Regex.IsMatch(surname, "^[A-Z][a-zA-Z]*$"))
            {
                return false;
            }
            return true;
        }

        private bool ifAgeValid(int age)
        {
            if (age <= 0 || age > 150) 
            { 
                return false;
            }
            return true;
        }

        #endregion
        #region FieldsVisibility
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
        private void MaleButton_Click(object sender, RoutedEventArgs e)
        {
            MaleText.Foreground = new SolidColorBrush(Colors.Blue);
            FemaleText.Foreground = new SolidColorBrush(Color.FromRgb(172, 176, 175));
            selectedGender = "Male";
        }
        private void FemaleButton_Click(object sender, RoutedEventArgs e)
        {
            FemaleText.Foreground = new SolidColorBrush(Colors.Pink);
            MaleText.Foreground = new SolidColorBrush(Color.FromRgb(172, 176, 175));
            selectedGender = "Female";
        }
        #endregion
    }
}
