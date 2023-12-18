using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GoingOutApp.Models;
using GoingOutApp.Services;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        private DataContext _database { get; set; }
        private static RegisterWindow? _registerWindowInstance;
        private bool isPasswordVisible = false;
        public List<User> Users { get; set; }

        public AdminPanelWindow()
        {
            _database = new DataContext();
            InitializeComponent();
            ShowUsers();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowUsers()
        {
            Users = _database.GetUsers();
            UsersDataGrid.ItemsSource = Users;
        }

        private void BanButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is User user)
            {
                bool wasBanned = user.IsBanned;

                BanUser(user);
                RefreshDataGrid();

                if (wasBanned == user.IsBanned)
                {
                    string message = user.IsBanned ? "The user has been unbanned." : "The user has been banned.";
                    MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BanUser(User user)
        {
            using (DataContext context = new DataContext())
            {
                var userToUpdate = context.Users.FirstOrDefault(u => u.UserId == user.UserId);

                if (userToUpdate != null)
                {
                    userToUpdate.IsBanned = !userToUpdate.IsBanned;
                    context.SaveChanges();
                }
            }
        }

        private void RefreshDataGrid()
        {
            Users = _database.GetUsers();
            UsersDataGrid.ItemsSource = Users;
        }
    }

    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] byteArray)
            {
                BitmapImage image = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(byteArray))
                {
                    stream.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                return image;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}