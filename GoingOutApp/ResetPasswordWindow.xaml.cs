using System.Security.Cryptography;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using GoingOutApp.Services;
using System.Reflection;
using System.Text.RegularExpressions;
using GoingOutApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
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
        private DataContext _database { get; set; }
        private static RegisterWindow? _registerWindowInstance;
        public List<User>? DatabaseUsers { get; private set; }
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
            string securityAnswer = txtAnswer.Text;
            SecurityQuestion selectedQuestion = MapComboBoxSelectionToEnum();
            string securityQuestionAsString = selectedQuestion.ToString();

            if (AccountValidation(username, password, securityQuestionAsString, securityAnswer))
            {
                var result = EncodePassword(password, 20);
                _database.UpdatePassword(username, result.Item1, result.Item2, securityQuestionAsString, securityAnswer);
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
                    return SecurityQuestion.None; // Domyślna wartość, można dostosować do własnych potrzeb
            }
        }

        private void textUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUser.Focus();
        }

        private void RefreshData()
        {
            DatabaseUsers = _database.Users.ToList();
        }

        private bool AccountValidation(string username, string password, string securityQuestion, string securityAnswer)
        {
            bool validation = true;

            if (!ifAllFieldsAreCompleted(username, password, securityQuestion, securityAnswer))
            {
                MessageBox.Show("Proszę wypełnić wszystkie wymagane pola.");
                validation = false;
            }
            else
            {
                if (!ifQuestionAndAnswerAreValid(username, securityQuestion, securityAnswer))
                {
                    MessageBox.Show("Pytanie pomocnicze i/lub odpowiedź nie zgadzają się.");
                    validation = false;
                }
            }
            if (!ifAccountWithThatUserExists(username))
            {
                txtUsernameValidation.Text = "Nie istnieje użytkownik o podanej nazwie.";
                txtUsernameValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtUsernameValidation.Visibility = Visibility.Collapsed;
            }
            if (!ifQuestionSelected(securityQuestion))
            {
                txtQuestionValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtQuestionValidation.Visibility = Visibility.Collapsed;
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
            if (validation)
            {
                MessageBox.Show("Pomyślnie zresetowano hasło.");
                Close();
            }
            return validation;
        }
        #region ValidationMethods
        private bool ifAllFieldsAreCompleted(string username, string password, string securityQuestion, string securityAnswer)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                securityQuestion == SecurityQuestion.None.ToString()  ||
                string.IsNullOrWhiteSpace(securityAnswer))
            {
                return false;
            }
            return true;
        }

        private bool ifAccountWithThatUserExists(string username)
        {
            RefreshData();
            var ifUserExists = DatabaseUsers.Any(u => u.UserName == username);

            return ifUserExists;
        }
        private bool ifQuestionSelected(string securityQuestion)
        {
            if (securityQuestion == SecurityQuestion.None.ToString())
            {
                txtQuestionValidation.Text = "Należy wybrać pytanie pomocnicze.";
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

        private bool ifQuestionAndAnswerAreValid(string username, string securityQuestion, string securityAnswer)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    return user.SecurityQuestion == securityQuestion && user.SecurityAnswer == securityAnswer;
                }

                return false;
            }
        }
        #endregion
        #region FieldsVisibility
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
        #endregion
    }
}
