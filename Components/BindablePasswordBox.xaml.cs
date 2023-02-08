using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lets_Meet.Components
{
    public partial class BindablePasswordBox : UserControl
    {
        private bool _isPasswordChanging;

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register ("Password", typeof (string), typeof (BindablePasswordBox),
                new FrameworkPropertyMetadata (string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    PasswordPropertyChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        private static void PasswordPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BindablePasswordBox BindPasswordBox)
            {
                BindPasswordBox.UpdatePassword ();
            }
        }

        public string Password
        {
            get { return (string)GetValue (PasswordProperty); }
            set { SetValue (PasswordProperty, value); }
        }

        public BindablePasswordBox ()
        {
            InitializeComponent ();
        }

        private void PasswordBox_PasswordChanged (object sender, RoutedEventArgs e)
        {
            _isPasswordChanging = true;
            Password = BindPasswordBox.Password;
            _isPasswordChanging = false;
        }

        private void UpdatePassword ()
        {
            if (!_isPasswordChanging)
            {
                BindPasswordBox.Password = Password;
            }
        }
    }
}