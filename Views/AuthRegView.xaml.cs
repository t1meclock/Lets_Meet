using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Win32;
using Npgsql;
using System.IO;
using Path = System.IO.Path;
using Lets_Meet.ViewModels;

namespace Lets_Meet.Views
{
    public partial class AuthRegView: Window
    {
        public AuthRegView ()
        {
            AuthRegViewModel vm = new AuthRegViewModel ();
            this.DataContext = vm;

            InitializeComponent ();

            if (vm.CloseAction == null)
                vm.CloseAction = new Action (this.Close);

            Closing += vm.OnWindowClosing;
        }
    }
}
