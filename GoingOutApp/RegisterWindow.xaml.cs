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
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string age = AgeTextBox.Text;
            string gender = MaleRadioButton.IsChecked == true ? "Male" : "Female";
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
    }
}
