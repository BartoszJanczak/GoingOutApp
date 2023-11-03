using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<User> DatabaseUsers { get; private set; }

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
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;
            var name = NameTextBox.Text;
            var surname = SurnameTextBox.Text;
            int age = Convert.ToInt32(AgeTextBox.Text);
            var gender = MaleRadioButton.IsChecked == true ? "Male" : "Female";

            if(AccountValidation(username, password, name, surname, age, gender))
            {
                _database.CreateAccount(username, password, name, surname, age, gender); 
            }
            else
            {
                // Wyświetlenie błędu

            }

            UsernameTextBox.Text = "dupa";
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
    }
}
