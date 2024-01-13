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

        public event EventHandler<User>? UserBanned;

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

            foreach (var user in Users)
            {
                UpdateBanButtonContent(user);
            }
        }

        private void UsersDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is User user)
            {
                e.Row.Loaded += (s, args) => UpdateBanButtonContent(user);
            }
        }

        private void BanButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is User user)
            {
                bool wasBanned = user.IsBanned;

                _database.BanUser(user);
                UserBanned?.Invoke(this, user);
                RefreshDataGrid();

                if (wasBanned == user.IsBanned)
                {
                    string message = user.IsBanned ? "The user has been unbanned." : "The user has been banned.";
                    MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                UpdateBanButtonContent(user);
            }
        }

        private void UpdateBanButtonContent(User user)
        {
            // Pobierz przycisk z wiersza, który został właśnie zaktualizowany
            var button = GetButtonFromDataGridRow(user);

            // Zaktualizuj zawartość przycisku na podstawie stanu zbanowania użytkownika
            if (button != null)
            {
                button.Content = user.IsBanned ? "Unban" : "Ban";
            }
        }

        private Button? GetButtonFromDataGridRow(User user)
        {
            // Znajdź przycisk w wierszu odpowiadającym użytkownikowi
            var row = (DataGridRow)UsersDataGrid.ItemContainerGenerator.ContainerFromItem(user);
            if (row != null)
            {
                // Znajdź przycisk w struktórze wiersza
                var button = FindChild<Button>(row, "BanButton");
                return button;
            }

            return null;
        }

        private T? FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Rekurencyjne przeszukiwanie struktury w poszukiwaniu elementu o określonym nazwie
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild && child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    return typedChild;
                }

                var result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }

            return null;
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

public class UserBannedEventArgs : EventArgs
{
    public User BannedUser { get; }

    public UserBannedEventArgs(User bannedUser)
    {
        BannedUser = bannedUser;
    }
}