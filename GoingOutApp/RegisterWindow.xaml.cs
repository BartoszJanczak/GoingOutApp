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
            int age = Convert.ToInt32(txtAge.Text);
            string gender = selectedGender.ToString();

            if (AccountValidation(username, password, name, surname, age, gender))
            {
                var result = EncodePassword(password, 20); 
                _database.CreateAccount(username, result.Item1, result.Item2, name, surname, age, gender);
            }
            else
            {
                

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
            bool validation = true;

            if (!ifAllFieldsAreCompleted(username, password, name, surname, age))
            {
                MessageBox.Show("Proszę wypełnić wszystkie wymagane pola.");
                validation = false;
            }
            if (!ifAccountWithThatUserNameCanBeCreated(username))
            {
                txtUsernameValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtUsernameValidation.Visibility = Visibility.Collapsed;
            }
            if (!ifGenderIsSelected())
            {
                txtGenderValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtGenderValidation.Visibility = Visibility.Collapsed;
            }
            if (!ifStrongPassword(password))
            {
                txtPasswordValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtPasswordValidation.Visibility = Visibility.Collapsed;
            }
            if (!ifPasswordEqual(password))
            {
                txtPassword2Validation.Visibility = Visibility.Visible;
            }
            else
            {
                txtPassword2Validation.Visibility = Visibility.Collapsed;
            }
            if (!ifNameValid(name))
            {
                txtNameValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtNameValidation.Visibility = Visibility.Collapsed;
            }
            if (!ifSurnameValid(surname))
            {
                txtSurnameValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtSurnameValidation.Visibility= Visibility.Collapsed;
            }
            if(!ifAgeValid(age))
            {
                txtAgeValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtAgeValidation.Visibility = Visibility.Collapsed;
            }
            if (validation)
            {
            MessageBox.Show("Pomyślnie zarejestrowano konto.");
            Close();
            }
            return validation;
        }

        #region ValidationMethods
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

        private bool ifAccountWithThatUserNameCanBeCreated(string login)
        {
            RefreshData();
            var ifUserExists = DatabaseUsers.Any(u => u.UserName == login);
            txtUsernameValidation.Text = "Konto o podanej nazwie użytkownika już istnieje.";

            return !ifUserExists;
        }

        private bool ifGenderIsSelected()
        {
            if (string.IsNullOrWhiteSpace(selectedGender))
            {
                txtGenderValidation.Text = "Proszę wybrać płeć.";
                return false;
            }
            return true;
        }

        private bool ifStrongPassword(string password)
        {
            if (password.Length < 8)
            {
                txtPasswordValidation.Text = "Hasło musi się składać z minimum 8 znaków.";
                return false;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                txtPasswordValidation.Text = "Hasło musi zawierać wielkie litery.";
                return false;
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                txtPasswordValidation.Text = "Hasło musi zawierać małe litery";
                return false;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                txtPasswordValidation.Text = "Hasło musi zawierać cyfry.";
                return false;
            }

            if (!Regex.IsMatch(password, "[!@#\\$%^&*()]"))
            {
                txtPasswordValidation.Text = "Hasło musi zawierać znaki specjalne (np. !,@,#,$,%).";
                return false;
            }

            return true;
        }

        private bool ifPasswordEqual(string password)
        {
            string enteredPassword = txtPassword2.Password;

            if (enteredPassword != password)
            {
                txtPassword2Validation.Text = "Hasła muszą być takie same.";
                return false;
            }

            return true;
        }
        private bool ifNameValid(string name)
        {
            if (!Regex.IsMatch(name, "^[A-Z][a-zA-Z]*$"))
            {
                txtNameValidation.Text = "Imię musi zaczynać się z wielkiej litery i nie może zawierać cyfr.";
                return false;
            }
            return true;
        }

        private bool ifSurnameValid(string surname)
        {
            if (!Regex.IsMatch(surname, "^[A-Z][a-zA-Z]*$"))
            {
                txtSurnameValidation.Text = "Nazwisko musi zaczynać się z wielkiej litery i nie może zawierać cyfr.";
                return false;
            }
            return true;
        }

        private bool ifAgeValid(int age)
        {
            if (age <= 0 || age > 150) 
            {
                txtAgeValidation.Text = "Podaj poprawny wiek.";
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
