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
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private static RegisterWindow? _registerWindowInstance;
        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_registerWindowInstance == null)
            {
                _registerWindowInstance = new RegisterWindow();
                _registerWindowInstance.Closed += (s, e) => _registerWindowInstance = null; // Reset _profileWindowInstance when the window is closed.
                _registerWindowInstance.Show();
            }
            else
            {
                _registerWindowInstance.Focus(); // Skoncentruj się na istniejącym oknie profilu.
            }
        }
    }
}
