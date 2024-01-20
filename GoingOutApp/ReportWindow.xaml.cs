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
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private DataContext _database { get; set; }
        public ReportWindow()
        {
            InitializeComponent();
            _database = new DataContext();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDesc.Text))
            {
                _database.AddReport(txtDesc.Text);
                MessageBox.Show("Report sent");
                Close();    
            }
        }
    }
}
