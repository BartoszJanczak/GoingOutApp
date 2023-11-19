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
        User LoggedInUser { get; set; }
        public UserProfileWindow(User user)
        {
            LoggedInUser = user;
            InitializeComponent();
            InitControls();
        }
        public void InitControls()
        {
            Name.Text ="Name: "+ LoggedInUser.Name;
            Surname.Text ="Surame: "+ LoggedInUser.Surname;
            Age.Text = "Age: " + Convert.ToString(LoggedInUser.Age);
            var gender = LoggedInUser.Gender == "Male"? "Male" : "Female";
            Gender.Text = "Gender: " + gender;
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
    }
}
