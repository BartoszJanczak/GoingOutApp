using Microsoft.Extensions.Logging;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GoingOutApp.ViewModel
{
    public class PointViewModel : INotifyPropertyChanged
    {
        private int _eventId;
        private Location _location;
        private Brush _pinColor;

        public PointViewModel(int Eventid, double x, double y, Brush Color)
        {
            EventId = Eventid;
            PinColor = Color;
            Location = new Location(x, y);
        }

        public Location Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public int EventId
        {
            get { return _eventId; }
            set
            {
                if (_eventId != value)
                {
                    _eventId = value;
                    OnPropertyChanged(nameof(EventId));
                }
            }
        }

        public Brush PinColor
        {
            get { return _pinColor; }
            set
            {
                if (_pinColor != value)
                {
                    _pinColor = value;
                    OnPropertyChanged(nameof(PinColor));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}