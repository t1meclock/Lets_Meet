using System;
using System.Collections.Generic;
using System.Data;
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
using Npgsql;
using System.IO;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Lets_Meet.ViewModels;
using Lets_Meet.Models;
using WMPLib;

namespace Lets_Meet.Views
{
    public partial class MainView: Window
    {
        MainViewModel vm;

        public MainView (int ID, int role)
        {
            vm = new MainViewModel (ID, role);
            this.DataContext = vm;

            InitializeComponent ();

            if (vm.CloseAction == null)
                vm.CloseAction = new Action (this.Close);

            Closing += vm.OnWindowClosing;
        }

        private void TrackStatusSlider_PreviewMouseLeftButtonUp (object sender, MouseButtonEventArgs e)
        {
            var item = e.GetPosition (TrackStatusSlider);
            double result = vm.TrackStatusSliderMaximum * item.X / 340;
            vm.TrackStatusSliderValue = result;
            vm.mediaPlayer.controls.currentPosition = vm.TrackStatusSliderValue;
        }

        private void ChangeVolumeSlider (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vm.mediaPlayer.settings.volume = vm.VolumeSliderValue;
        }
    }
}
