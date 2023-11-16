using GoingOutApp.Services;
using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Linq;

namespace GoingOutApp
{
    /// <summary>
    /// Interaction logic for AddTaskwindow.xaml
    /// </summary>
    public partial class AddTaskwindow : Window
    {
        private MainWindow _mainWindowInstance = new MainWindow();
        private DataContext _database { get; set; }

        public event EventHandler EventAdded;

        public AddTaskwindow()
        {
            InitializeComponent();
            _database = new DataContext();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string eventName = AddEventName.Text;
            //string PhotoPath = photoPath,
            //string PhotoDescription = photoDescription,
            string eventDescription = AddEventDesc.Text;
            string eventLocation = AddEventLocation.Text;
            //string EventDateTime = eventDateTime,
            //string NumberOfplaces = numberOfplaces,
            //string OtherInfo = otherInfo

            _database.AddEvent(eventName, "photopath", "photodesc", eventDescription, eventLocation, DateTime.Now, 5, "otherinfo");
            EventAdded?.Invoke(this, EventArgs.Empty);
            Close();
           
        }
    }
}
