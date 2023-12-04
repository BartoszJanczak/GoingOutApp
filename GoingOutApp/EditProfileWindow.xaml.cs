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
    public partial class EditProfileWindow : Window
    {
        private User _userToEdit;
        public List<User>? DatabaseUsers { get; private set; }

        private DataContext _database { get; set; }

        private string selectedGender = "";

        public EditProfileWindow(User userToEdit)
        {
            InitializeComponent();
            _database = new DataContext();
            _userToEdit = userToEdit;
            InitializeFields();
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
            string name = txtName.Text;
            string surname = txtSurname.Text;
            int age = Convert.ToInt32(txtAge.Text);
            string gender = selectedGender;

            _userToEdit.Name = name;
            _userToEdit.Surname = surname;
            _userToEdit.Age = age;
            _userToEdit.Gender = gender;

            MessageBox.Show("Changes saved successfully.");
            Close();
        }

        private void InitializeFields()
        {
            txtName.Text = _userToEdit.Name;
            txtSurname.Text = _userToEdit.Surname;
            txtAge.Text = _userToEdit.Age.ToString();

            if (_userToEdit.Gender == "Male")
            {
                MaleButton_Click(null, null);
            }
            else if (_userToEdit.Gender == "Female")
            {
                FemaleButton_Click(null, null);
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
            if (!ifGenderIsSelected())
            {
                txtGenderValidation.Visibility = Visibility.Visible;
                validation = false;
            }
            else
            {
                txtGenderValidation.Visibility = Visibility.Collapsed;
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

        private bool ifGenderIsSelected()
        {
            if (string.IsNullOrWhiteSpace(selectedGender))
            {
                txtGenderValidation.Text = "Please select a gender.";
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

        #endregion FieldsVisibility
    }
}