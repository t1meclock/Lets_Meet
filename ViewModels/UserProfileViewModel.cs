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
using Lets_Meet.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Net;
using Lets_Meet.Models;

namespace Lets_Meet.ViewModels
{
    public class UserProfileViewModel : INotifyPropertyChanged
    {
        NpgsqlConnection con = AppContext.GetConnection ();

        public Action CloseAction { get; set; }

        private int _ID;

        #region Свойства
        private string _titleText;
        public string TitleText
        {
            get { return _titleText; }
            set
            {
                _titleText = value;
                OnPropertyChanged ("TitleText");
            }
        }

        private string _userProfileHeader;
        public string UserProfileHeader
        {
            get { return _userProfileHeader; }
            set
            {
                _userProfileHeader = value;
                OnPropertyChanged ("UserProfileHeader");
            }
        }

        private BitmapImage _setAvatarPath;
        public BitmapImage AvatarSourse
        {
            get { return _setAvatarPath; }
            set { _setAvatarPath = value; }
        }

        private string _showUserSurname;
        public string ShowUserSurname
        {
            get { return _showUserSurname; }
            set
            {
                _showUserSurname += value;
                OnPropertyChanged ("ShowUserSurname");
            }
        }

        private string _showUserName;
        public string ShowUserName
        {
            get { return _showUserName; }
            set
            {
                _showUserName += value;
                OnPropertyChanged ("ShowUserName");
            }
        }

        private string _showUserPatronymic;
        public string ShowUserPatronymic
        {
            get { return _showUserPatronymic; }
            set
            {
                _showUserPatronymic += value;
                OnPropertyChanged ("ShowUserPatronymic");
            }
        }

        private string _showUserAge;
        public string ShowUserAge
        {
            get { return _showUserAge; }
            set
            {
                _showUserAge += value;
                OnPropertyChanged ("ShowUserAge");
            }
        }

        private string _showUserHobby;
        public string ShowUserHobby
        {
            get { return _showUserHobby; }
            set
            {
                _showUserHobby += value;
                OnPropertyChanged ("ShowUserHobby");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged ([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged (this, new PropertyChangedEventArgs (prop));
        }

        public UserProfileViewModel (int ID)
        {
            this._ID = ID;

            con.Open ();

            InitializeView ();

            ShowData ();
        }

        private void InitializeView ()
        {
            TitleText = "Let's Meet | Профиль пользователя";
        }

        private void ShowData ()
        {
            string avatarImage = Path.Combine (Environment.CurrentDirectory, @"avatars\");
            string userSurname = String.Empty;
            string userName = String.Empty;
            string userPatronymic = String.Empty;
            string userAge = String.Empty;
            string userHobbyName = String.Empty;
            int userHobbyID = -1;

            try
            {
                string queryUsr = "SELECT * FROM Users WHERE ID_User = '" + this._ID + "'";
                using (var cmdUsr = new NpgsqlCommand (queryUsr, con))
                {
                    using (var rdrUsr = cmdUsr.ExecuteReader ())
                    {
                        if (rdrUsr.Read ())
                        {
                            userSurname = rdrUsr.GetString (1);
                            userName = rdrUsr.GetString (2);
                            userPatronymic = rdrUsr.GetString (3);
                            userAge = rdrUsr.GetString (4);
                            userHobbyID = rdrUsr.GetInt32 (8);
                            avatarImage += rdrUsr.GetString (11);
                        }
                    }
                }

                string sqlHby = "SELECT * FROM Hobbies WHERE ID_Hobby = '" + userHobbyID + "'";
                using (var cmdHby = new NpgsqlCommand (sqlHby, con))
                {
                    using (var rdrHby = cmdHby.ExecuteReader ())
                    {
                        if (rdrHby.Read ())
                        {
                            userHobbyName = rdrHby.GetString (1);
                        }
                    }
                }

                UserProfileHeader = ShowUserSurname + " " + ShowUserName;

                ShowUserSurname = "Фамилия: " + userSurname.Trim ();
                ShowUserName = "Имя: " + userName.Trim ();
                ShowUserPatronymic = "Отчество: " + userPatronymic.Trim ();
                ShowUserAge = "Возраст: " + userAge.Trim ();
                ShowUserHobby = "Хобби: " + userHobbyName.Trim ();

                BitmapImage userAvatarImage = new BitmapImage (new Uri (avatarImage));

                AvatarSourse = userAvatarImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
    }
}
