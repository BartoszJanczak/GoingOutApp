using GoingOutApp.Models;
using GoingOutApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for UserProfileWindow.xaml
    /// </summary>
    ///

    public partial class UserProfileWindow : Window
    {
        private DataContext _database { get; set; }
        private User LoggedInUser { get; set; }

        public UserProfileWindow(User user)
        {
            LoggedInUser = user;
            _database = new DataContext();
            InitializeComponent();
            InitControls();
            ShowParticipatedEvents();
        }

        public void InitControls()
        {
            Name.Text = "Name: " + LoggedInUser.Name;
            Surname.Text = "Surname: " + LoggedInUser.Surname;
            Age.Text = "Age: " + Convert.ToString(LoggedInUser.Age);
            var gender = LoggedInUser.Gender == "Male" ? "Male" : "Female";
            Gender.Text = "Gender: " + gender;
        }

        private void ShowParticipatedEvents()
        {
            List<Event> participatedEvents = _database.GetParticipatedEvents(LoggedInUser.UserId);
            ParticipatedEventsItemsControl.ItemsSource = participatedEvents;
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            UserService.Logout();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EditUserData_Click(object sender, RoutedEventArgs e)
        {
            // Otwórz okno edycji danych użytkownika
            EditProfileWindow editUserDataWindow = new EditProfileWindow(LoggedInUser);
            editUserDataWindow.Owner = this;
            editUserDataWindow.ShowDialog();

            // Zaktualizuj dane na widoku po zamknięciu okna edycji danych użytkownika
            InitControls();
        }
    }
}