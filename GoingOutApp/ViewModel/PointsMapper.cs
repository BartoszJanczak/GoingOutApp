﻿using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GoingOutApp.ViewModel
{
    internal static class Mapper
    {
        public static System.Windows.Point Map(PointViewModel pointViewModel)
        {
            return new System.Windows.Point(pointViewModel.Location.Altitude, pointViewModel.Location.Longitude);
        }

        public static PointViewModel Map(System.Windows.Point pointViewModel)
        {
            return new PointViewModel(pointViewModel.X, pointViewModel.Y, Brushes.AliceBlue);
        }

        public static ObservableCollection<PointViewModel> Map(List<System.Windows.Point> windowsPoints)
        {
            ObservableCollection<PointViewModel> pointViewModels = new ObservableCollection<PointViewModel>();

            foreach (var windowsPoint in windowsPoints)
            {
                PointViewModel pointViewModel = new PointViewModel(windowsPoint.X, windowsPoint.Y, Brushes.AliceBlue);
                pointViewModels.Add(pointViewModel);
            }

            return pointViewModels;
        }
    }
}