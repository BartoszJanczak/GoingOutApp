﻿using System;
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
using System.IO;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public enum SecurityQuestion
        {
            FavoriteColor,
            FirstPet,
            BirthCity,
            FavoriteBook,
            DreamJob,
            None
        }

        public List<User>? DatabaseUsers { get; private set; }

        private DataContext _database { get; set; }

        private byte[] photoBytes;

        private string selectedGender = "";

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SignUpButton_Click(sender, e);
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;
            string name = txtName.Text;
            string surname = txtSurname.Text;
            int age = Convert.ToInt32(txtAge.Text);
            string gender = selectedGender.ToString();
            string securityAnswer = txtAnswer.Text;
            SecurityQuestion selectedQuestion = MapComboBoxSelectionToEnum();
            string securityQuestionAsString = selectedQuestion.ToString();

            if (photoBytes == null)
            {
                photoBytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "/../../.." + "/data/images/userProfile.png");
            }

            if (AccountValidation(username, password, name, surname, age, gender))
            {
                var result = EncodePassword(password, 20);
                _database.CreateAccount(username, result.Item1, result.Item2, name, surname, age, gender, securityQuestionAsString, securityAnswer, photoBytes);
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

        private SecurityQuestion MapComboBoxSelectionToEnum()
        {
            switch (cmbSecurityQuestion.SelectedIndex)
            {
                case 0:
                    return SecurityQuestion.FavoriteColor;

                case 1:
                    return SecurityQuestion.FirstPet;

                case 2:
                    return SecurityQuestion.BirthCity;

                case 3:
                    return SecurityQuestion.FavoriteBook;

                case 4:
                    return SecurityQuestion.DreamJob;

                default:
                    return SecurityQuestion.None;
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
                MessageBox.Show("Please fill in all required fields.");
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
                validation = false;
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
                txtSurnameValidation.Visibility = Visibility.Collapsed;
            }
            if (!ifAgeValid(age))
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
                MessageBox.Show("Account successfully registered.");
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
            txtUsernameValidation.Text = "An account with this username already exists.";

            return !ifUserExists;
        }

        private bool ifGenderIsSelected()
        {
            if (string.IsNullOrWhiteSpace(selectedGender))
            {
                txtGenderValidation.Text = "Please select a gender.";
                return false;
            }
            return true;
        }

        private bool ifStrongPassword(string password)
        {
            if (password.Length < 8)
            {
                txtPasswordValidation.Text = "Password must be at least 8 characters long.";
                return false;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                txtPasswordValidation.Text = "Password must contain uppercase letters.";
                return false;
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                txtPasswordValidation.Text = "Password must contain lowercase letters.";
                return false;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                txtPasswordValidation.Text = "Password must contain digits.";
                return false;
            }

            if (!Regex.IsMatch(password, "[!@#\\$%^&*()]"))
            {
                txtPasswordValidation.Text = "Password must contain special characters (e.g. !,@,#,$,%).";
                return false;
            }

            return true;
        }

        private bool ifPasswordEqual(string password)
        {
            string enteredPassword = txtPassword2.Password;

            if (enteredPassword != password)
            {
                txtPassword2Validation.Text = "Passwords must match.";
                return false;
            }

            return true;
        }

        private bool ifNameValid(string name)
        {
            if (!Regex.IsMatch(name, "^[A-Z][a-zA-Z]*$"))
            {
                txtNameValidation.Text = "Name must start with uppercase letter and cannot contain digits.";
                return false;
            }
            return true;
        }

        private bool ifSurnameValid(string surname)
        {
            if (!Regex.IsMatch(surname, "^[A-Z][a-zA-Z]*$"))
            {
                txtSurnameValidation.Text = "Surname must start with uppercase letter and cannot contain digits.";
                return false;
            }
            return true;
        }

        private bool ifAgeValid(int age)
        {
            if (age <= 0 || age > 150)
            {
                txtAgeValidation.Text = "Please provide a valid age.";
                return false;
            }
            return true;
        }

        #endregion ValidationMethods

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

        #endregion FieldsVisibility
    }
}