﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {  
        string mapPath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\data\images\mapa.png";

        private static ProfileWindow ?_profileWindowInstance;
        private static AddTaskwindow? _addWindowInstance;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Tworzenie nowego obiektu BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                // Ustawienie źródła obrazu na podstawie ścieżki
                bitmapImage.UriSource = new Uri(mapPath, UriKind.RelativeOrAbsolute);

                bitmapImage.EndInit();

                // Przypisanie obrazu do kontrolki Image
                MapBox.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd: " + ex.Message);
            }
        }

        private void Window_mousedown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_profileWindowInstance == null)
            {
                _profileWindowInstance = new ProfileWindow();
                _profileWindowInstance.Closed += (s, e) => _profileWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _profileWindowInstance.Show();
            }
            else
            {
                _profileWindowInstance.Focus();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_addWindowInstance == null)
            {
                _addWindowInstance = new AddTaskwindow();
                _addWindowInstance.Closed += (s, e) => _addWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _addWindowInstance.Show();
            }
            else
            {
                _addWindowInstance.Focus();
            }
        }
    }
}
