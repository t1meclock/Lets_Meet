using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Umbraco.Core.Composing;
using Aspose.Words;
using System.Reflection;
using Lets_Meet.Services;
using Lets_Meet.Models;
using System.Runtime.CompilerServices;
using Lets_Meet.Views;

namespace Lets_Meet.ViewModels
{
    public class AdministratorViewModel : INotifyPropertyChanged  //BaseViewModel, : IPageViewModel
    {
        NpgsqlConnection con = AppContext.GetConnection ();

        // Действие для закрытия окна
        public Action CloseAction { get; set; }

        #region Списки для статистики
        private List<string> _hobbiesNames = new List<string> ();
        private List<int> _hobbiesIDs = new List<int> ();
        private List<int> _hobbiesStatistic = new List<int> ();
        private List<int> _hobbiesCount = new List<int> ();

        private string printInterests = String.Empty;
        #endregion

        #region Массивы символов для пароля
        char [] symbols = new char [] { '!', '@', '#', '$', '%', '^', '&', '*' };
        char [] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray ();
        char [] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray ();
        char [] numbers = "0123456789".ToCharArray ();
        #endregion

        private int _ID, _role;

        #region Свойства

        #region Свойства окна
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

        private int _defaultSelectedIndex;
        public int DefaultSelectedIndex
        {
            get { return _defaultSelectedIndex; }
            set { _defaultSelectedIndex = 0; }
        }

        private int _selectedUserIndexDataGrid = -1;
        public int SelectedUserIndexDataGrid
        {
            get { return _selectedUserIndexDataGrid; }
            set
            {
                _selectedUserIndexDataGrid = value;
                OnPropertyChanged ("SelectedUserIndexDataGrid");
            }
        }

        private int _selectedHobbyIndexDataGrid = -1;
        public int SelectedHobbyIndexDataGrid
        {
            get { return _selectedHobbyIndexDataGrid; }
            set
            {
                _selectedHobbyIndexDataGrid = value;
                OnPropertyChanged ("SelectedHobbyIndexDataGrid");
            }
        }

        // Свойство для подключения User Control HobbyChart для графика Хобби
        private object _hobbyChart;
        public object HobbyChart
        {
            get { return _hobbyChart; }
            set
            {
                _hobbyChart = value;
            }
        }

        #region Свойства ошибок
        private string _newUserSurnameToolTip;
        public string NewUserSurnameToolTip
        {
            get { return _newUserSurnameToolTip; }
            set
            {
                _newUserSurnameToolTip = value;
                OnPropertyChanged ("NewUserSurnameToolTip");
            }
        }

        private Brush _newUserSurnameBackground;
        public Brush NewUserSurnameBackground
        {
            get { return _newUserSurnameBackground; }
            set
            {
                _newUserSurnameBackground = value;
                OnPropertyChanged ("NewUserSurnameBackground");
            }
        }

        private string _newUserNameToolTip;
        public string NewUserNameToolTip
        {
            get { return _newUserNameToolTip; }
            set
            {
                _newUserNameToolTip = value;
                OnPropertyChanged ("NewUserNameToolTip");
            }
        }

        private Brush _newUserNameBackground;
        public Brush NewUserNameBackground
        {
            get { return _newUserNameBackground; }
            set
            {
                _newUserNameBackground = value;
                OnPropertyChanged ("NewUserNameBackground");
            }
        }

        private string _newUserPatronymicToolTip;
        public string NewUserPatronymicToolTip
        {
            get { return _newUserPatronymicToolTip; }
            set
            {
                _newUserPatronymicToolTip = value;
                OnPropertyChanged ("NewUserPatronymicToolTip");
            }
        }

        private Brush _newUserPatronymicBackground;
        public Brush NewUserPatronymicBackground
        {
            get { return _newUserPatronymicBackground; }
            set
            {
                _newUserPatronymicBackground = value;
                OnPropertyChanged ("NewUserPatronymicBackground");
            }
        }

        private string _newUserLoginToolTip;
        public string NewUserLoginToolTip
        {
            get { return _newUserLoginToolTip; }
            set
            {
                _newUserLoginToolTip = value;
                OnPropertyChanged ("NewUserLoginToolTip");
            }
        }

        private Brush _newUserLoginBackground;
        public Brush NewUserLoginBackground
        {
            get { return _newUserLoginBackground; }
            set
            {
                _newUserLoginBackground = value;
                OnPropertyChanged ("NewUserLoginBackground");
            }
        }

        private string _newUserPasswordToolTip;
        public string NewUserPasswordToolTip
        {
            get { return _newUserPasswordToolTip; }
            set
            {
                _newUserPasswordToolTip = value;
                OnPropertyChanged ("NewUserPasswordToolTip");
            }
        }

        private Brush _newUserPasswordBackground;
        public Brush NewUserPasswordBackground
        {
            get { return _newUserPasswordBackground; }
            set
            {
                _newUserPasswordBackground = value;
                OnPropertyChanged ("NewUserPasswordBackground");
            }
        }

        private string _newUserEmailToolTip;
        public string NewUserEmailToolTip
        {
            get { return _newUserEmailToolTip; }
            set
            {
                _newUserEmailToolTip = value;
                OnPropertyChanged ("NewUserEmailToolTip");
            }
        }

        private Brush _newUserEmailBackground;
        public Brush NewUserEmailBackground
        {
            get { return _newUserEmailBackground; }
            set
            {
                _newUserEmailBackground = value;
                OnPropertyChanged ("NewUserEmailBackground");
            }
        }

        private string _newHobbyToolTip;
        public string NewHobbyToolTip
        {
            get { return _newHobbyToolTip; }
            set
            {
                _newHobbyToolTip = value;
                OnPropertyChanged ("NewHobbyToolTip");
            }
        }

        private Brush _newHobbyBackground;
        public Brush NewHobbyBackground
        {
            get { return _newHobbyBackground; }
            set
            {
                _newHobbyBackground = value;
                OnPropertyChanged ("NewHobbyBackground");
            }
        }
        #endregion

        #endregion

        #region Свойства данных
        private ObservableCollection<User> _usersList = new ObservableCollection<User> ();
        public ObservableCollection<User> UsersList
        {
            get { return _usersList; }
            set
            {
                _usersList = value;
                OnPropertyChanged ("UsersList");
            }
        }

        private ObservableCollection<Hobby> _hobbiesList = new ObservableCollection<Hobby> ();
        public ObservableCollection<Hobby> HobbiesList
        {
            get { return _hobbiesList; }
            set 
            {
                _hobbiesList = value;
                OnPropertyChanged ("HobbiesList");
            }
        }

        private ObservableCollection<Hobby> _foundHobbies = new ObservableCollection<Hobby> ();
        public ObservableCollection<Hobby> FoundHobbies
        {
            get { return _foundHobbies; }
            set
            {
                _foundHobbies = value;
                OnPropertyChanged ("FoundHobbies");
            }
        }

        private string _newUserSurname;
        public string NewUserSurname
        {
            get { return _newUserSurname; }
            set
            {
                _newUserSurname = value;
                OnPropertyChanged ("NewUserSurname");
            }
        }

        private string _newUserName;
        public string NewUserName
        {
            get { return _newUserName; }
            set
            {
                _newUserName = value;
                OnPropertyChanged ("NewUserName");
            }
        }

        private string _newUserPatronymic;
        public string NewUserPatronymic
        {
            get { return _newUserPatronymic; }
            set
            {
                _newUserPatronymic = value;
                OnPropertyChanged ("NewUserPatronymic");
            }
        }

        private string _newUserAge;
        public string NewUserAge
        {
            get { return _newUserAge; }
            set
            {
                _newUserAge = value;
                OnPropertyChanged ("NewUserAge");
            }
        }

        private string _newUserLogin;
        public string NewUserLogin
        {
            get { return _newUserLogin; }
            set
            {
                _newUserLogin = value;
                OnPropertyChanged ("NewUserLogin");
            }
        }

        private string _newUserEmail;
        public string NewUserEmail
        {
            get { return _newUserEmail; }
            set
            {
                _newUserEmail = value;
                OnPropertyChanged ("NewUserEmail");
            }
        }

        private Hobby _newUserHobby;
        public Hobby NewUserHobby
        {
            get { return _newUserHobby; }
            set
            {
                _newUserHobby = value;
                OnPropertyChanged ("NewUserHobby");
            }
        }

        private string _newUserPassword;
        public string NewUserPassword
        {
            get { return _newUserPassword; }
            set
            {
                _newUserPassword = value;
                OnPropertyChanged ("NewUserPassword");
            }
        }

        private User _selectedUserDataGrid;
        public User SelectedUserDataGrid
        {
            get { return _selectedUserDataGrid; }
            set
            {
                _selectedUserDataGrid = value;
                OnPropertyChanged ("SelectedUserDataGrid");
            }
        }

        private Hobby _selectedHobbyDataGrid;
        public Hobby SelectedHobbyDataGrid
        {
            get { return _selectedHobbyDataGrid; }
            set
            {
                _selectedHobbyDataGrid = value;
                OnPropertyChanged ("SelectedHobbyDataGrid");
            }
        }

        private string _newHobby;
        public string NewHobby
        {
            get { return _newHobby; }
            set
            {
                _newHobby = value;
                OnPropertyChanged ("NewHobby");
            }
        }

        private bool _changeAge = false;
        public bool ChangeAge
        {
            get { return _changeAge; }
            set
            {
                _changeAge = value;
                OnPropertyChanged ("ChangeAge");
            }
        }

        private bool _changeHobby = false;
        public bool ChangeHobby
        {
            get { return _changeHobby; }
            set
            {
                _changeHobby = value;
                OnPropertyChanged ("ChangeHobby");
            }
        }
        #endregion

        #endregion

        #region Кнопки

        #region Кнопки управления пользователями
        private RelayCommand _addUser_Click;
        public RelayCommand AddUser_Click
        {
            get
            {
                return _addUser_Click ?? (_addUser_Click = new RelayCommand (obj =>
                 {
                     AddUser ();
                 }));
            }
        }

        private RelayCommand _changeUser_Click;
        public RelayCommand ChangeUser_Click
        {
            get
            {
                return _changeUser_Click ?? (_changeUser_Click = new RelayCommand (obj =>
                {
                    ChangeUser ();
                }));
            }
        }

        private RelayCommand _deleteUser_Click;
        public RelayCommand DeleteUser_Click
        {
            get
            {
                return _deleteUser_Click ?? (_deleteUser_Click = new RelayCommand (obj =>
                {
                    DeleteOrRestoreUser ();
                }));
            }
        }
        #endregion

        #region Кнопки управления хобби
        private RelayCommand _addHobby_Click;
        public RelayCommand AddHobby_Click
        {
            get
            {
                return _addHobby_Click ?? (_addHobby_Click = new RelayCommand (obj =>
                {
                    AddHobby ();
                }));
            }
        }

        private RelayCommand _deleteHobby_Click;
        public RelayCommand DeleteHobby_Click
        {
            get
            {
                return _deleteHobby_Click ?? (_deleteHobby_Click = new RelayCommand (obj =>
                {
                    DeleteHobby ();
                }));
            }
        }
        #endregion

        private RelayCommand _logOut_Click;
        public RelayCommand LogOut_Click
        {
            get
            {
                return _logOut_Click ?? (_logOut_Click = new RelayCommand (obj =>
                {
                    LogOut ();
                }));
            }
        }
        #endregion

        // Ивент для отслеживания изменения свойств
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged ([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged (this, new PropertyChangedEventArgs (prop));
        }

        // Конструктор
        public AdministratorViewModel (int ID, int role)
        {
            this._ID = ID;
            this._role = role;
            this._hobbyChart = new HobbyChart ();
            HobbyChart = _hobbyChart;

            con.Open ();

            InitializeView ();

            ReadAllUsers ();
            FillHobbyComBox ();
            ReadAllHobbies ();
        }

        // Функция для инициализации свойств окна
        private void InitializeView ()
        {
            TitleText = "Let's Meet | Администратор";
        }

        // --------------------------------------------------------------------------

        #region Управление пользователями
        private void ReadAllUsers ()
        {
            UsersList.Clear ();

            try
            {
                // Чтение списка юзеров из БД и заполнение DataGrid
                string query = "SELECT * FROM Users WHERE Role_ID != '" + this._role + "'";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            User user = new User ();

                            user.UserID = rdr.GetInt32 (0);
                            user.Surname = rdr.GetString (1);
                            user.Name = rdr.GetString (2);
                            user.Patronymic = rdr.GetString (3);
                            user.Age = rdr.GetString (4);
                            user.Email = rdr.GetString (5);
                            user.Login = rdr.GetString (6);
                            user.Password = rdr.GetString (7);
                            user.HobbyID = rdr.GetInt32 (8);
                            user.IsDeleted = rdr.GetBoolean (9);
                            user.RoleID = rdr.GetInt32 (10);
                            user.HobbyName = String.Empty;

                            UsersList.Add (user);
                        }
                    }
                }

                // Присваивание наименования Хобби по ID каждому юзеру
                foreach (User userFromDG in UsersList)
                {
                    string sqlHby = "SELECT * FROM Hobbies WHERE ID_Hobby = '" + userFromDG.HobbyID + "'";
                    using (var cmdHby = new NpgsqlCommand (sqlHby, con))
                    {
                        using (var rdrHby = cmdHby.ExecuteReader ())
                        {
                            if (rdrHby.Read ())
                            {
                                userFromDG.HobbyName = rdrHby.GetString (1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void FillHobbyComBox ()
        {
            HobbiesList.Clear ();

            try
            {
                string query = "SELECT * FROM Hobbies";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            Hobby hobbies = new Hobby ();

                            hobbies.HobbyID = rdr.GetInt32 (0);
                            hobbies.HobbyName = rdr.GetString (1);

                            HobbiesList.Add (hobbies);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void DeleteOrRestoreUser ()
        {
            if (SelectedUserIndexDataGrid == -1)
            {
                MessageBox.Show ("Выберите пользователя для удаления.");
            }
            else
            {
                try
                {
                    int userID = SelectedUserDataGrid.UserID;
                    bool userIsDeleted = false;

                    // Получение статуса пользователя (удалён или нет)
                    string queryStatusUser = "SELECT * FROM Users WHERE ID_User = '" + userID + "'";
                    using (var cmdStatusUser = new NpgsqlCommand (queryStatusUser, con))
                    {
                        using (var rdrStatusUser = cmdStatusUser.ExecuteReader ())
                        {
                            if (rdrStatusUser.Read ())
                            {
                                userIsDeleted = rdrStatusUser.GetBoolean (9);
                            }
                        }
                    }

                    // Если пользователь не удалён - удаляем
                    if (userIsDeleted == false)
                    {
                        // Удаление записи пользователя из Хобби
                        // ------------------------------
                        string queryDel = "DELETE FROM UsersHobbies WHERE User_ID = '" + userID + "'";
                        using (var cmdDel = new NpgsqlCommand (queryDel, con))
                        {
                            cmdDel.ExecuteNonQuery ();
                        }
                        // ------------------------------

                        // Удаление пользователя посредством изменения свойства Is_Deleted
                        // ------------------------------
                        string queryUpd = "UPDATE Users SET Is_Deleted = True WHERE ID_User ='" + userID + "'";
                        using (var cmdUpd = new NpgsqlCommand (queryUpd, con))
                        {
                            cmdUpd.ExecuteNonQuery ();
                        }
                        // ------------------------------

                        MessageBox.Show ("Пользователь успешно удалён.");

                        ReadAllUsers ();
                    }
                    // Если пользователь удалён - восстанавливаем
                    else
                    {
                        int hobbyID = -1;

                        // Получение ID хобби восстанавливаемого пользователя
                        string queryGetHobbyID = "SELECT * FROM Users WHERE ID_User = '" + userID + "'";
                        using (var cmdGetHobbyID = new NpgsqlCommand (queryGetHobbyID, con))
                        {
                            using (var rdrGetHobbyID = cmdGetHobbyID.ExecuteReader ())
                            {
                                if (rdrGetHobbyID.Read ())
                                {
                                    hobbyID = rdrGetHobbyID.GetInt32 (8);
                                }
                            }
                        }

                        // Добавление записи пользователя в Хобби
                        // ------------------------------
                        string queryHobby = "INSERT INTO UsersHobbies (Hobby_ID, User_ID) VALUES (@HobbyID, @UserID)";
                        using (var cmdHobby = new NpgsqlCommand (queryHobby, con))
                        {
                            cmdHobby.CommandType = CommandType.Text;
                            cmdHobby.Parameters.Add (new NpgsqlParameter ("@HobbyID", hobbyID));
                            cmdHobby.Parameters.Add (new NpgsqlParameter ("@UserID", userID));

                            cmdHobby.ExecuteScalar ();
                        }
                        // ------------------------------

                        // Восстановление пользователя посредством изменения свойства Is_Deleted
                        // ------------------------------
                        string queryUpd = "UPDATE Users SET Is_Deleted = False WHERE ID_User ='" + userID + "'";
                        using (var cmdUpd = new NpgsqlCommand (queryUpd, con))
                        {
                            cmdUpd.ExecuteNonQuery ();
                        }
                        // ------------------------------

                        MessageBox.Show ("Пользователь успешно восстановлен.");

                        ReadAllUsers ();
                    }                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }

        private void ChangeUser ()
        {
            if (SelectedUserIndexDataGrid == -1)
            {
                MessageBox.Show ("Выберите пользователя для изменения.");
            }
            else
            {
                try
                {
                    CryptPass cryptPass = new CryptPass ();

                    bool isChanged = false;
                    bool correctPassword = false;
                    bool hobbyChanged = false;

                    int userID = SelectedUserDataGrid.UserID;

                    string surname = String.Empty;
                    string name = String.Empty;
                    string patronymic = String.Empty;
                    string age = String.Empty;
                    string email = String.Empty;
                    string login = String.Empty;
                    string password = String.Empty;
                    int IDhobby = -1;

                    string queryReUs = "SELECT * FROM Users WHERE ID_User = '" + userID + "'";
                    using (var cmdReUs = new NpgsqlCommand (queryReUs, con))
                    {
                        using (var rdrReUs = cmdReUs.ExecuteReader ())
                        {
                            if (rdrReUs.Read ())
                            {
                                surname = rdrReUs.GetString (1);
                                name = rdrReUs.GetString (2);
                                patronymic = rdrReUs.GetString (3);
                                age = rdrReUs.GetString (4);
                                email = rdrReUs.GetString (5);
                                login = rdrReUs.GetString (6);
                                password = rdrReUs.GetString (7);
                                IDhobby = rdrReUs.GetInt32 (8);
                            }
                        }
                    }

                    // Смена фамилии
                    if (NewUserSurname != null && NewUserSurname.Length > 0)
                    {
                        if (NewUserSurname.Length < 2 || NewUserSurname.Length > 20)
                        {
                            NewUserSurnameToolTip = "Некорректная фамилия. Необходимо значение от 2 до 20 символов.";
                            NewUserSurnameBackground = Brushes.Red;
                        }
                        else
                        {
                            if (NewUserSurname.Contains (' '))
                            {
                                NewUserSurnameToolTip = "Значение не должно содержать пробелы.";
                                NewUserSurnameBackground = Brushes.Red;
                            }
                            else
                            {
                                NewUserSurnameToolTip = null;
                                NewUserSurnameBackground = Brushes.Transparent;

                                surname = NewUserSurname.Trim ();

                                isChanged = true;
                            }
                        }
                    }
                    else
                    {
                        NewUserSurnameToolTip = null;
                        NewUserSurnameBackground = Brushes.Transparent;
                    }

                    // Смена имени
                    if (NewUserName != null && NewUserName.Length > 0)
                    {
                        if (NewUserName.Length < 1 || NewUserName.Length > 15)
                        {
                            NewUserNameToolTip = "Некорректное имя. Необходимо значение от 1 до 15 символов.";
                            NewUserNameBackground = Brushes.Red;
                        }
                        else
                        {
                            if (NewUserName.Contains (' '))
                            {
                                NewUserNameToolTip = "Значение не должно содержать пробелы.";
                                NewUserNameBackground = Brushes.Red;
                            }
                            else
                            {
                                NewUserNameToolTip = null;
                                NewUserNameBackground = Brushes.Transparent;

                                name = NewUserName.Trim ();

                                isChanged = true;
                            }
                        }
                    }
                    else
                    {
                        NewUserNameToolTip = null;
                        NewUserNameBackground = Brushes.Transparent;
                    }

                    // Смена отчества
                    if (NewUserPatronymic != null && NewUserPatronymic.Length > 0)
                    {
                        if (NewUserPatronymic.Length < 3 || NewUserPatronymic.Length > 20)
                        {
                            NewUserPatronymicToolTip = "Некорректное отчество. Необходимо значение от 3 до 20 символов.";
                            NewUserPatronymicBackground = Brushes.Red;
                        }
                        else
                        {
                            if (NewUserPatronymic.Contains (' '))
                            {
                                NewUserPatronymicToolTip = "Значение не должно содержать пробелы.";
                                NewUserPatronymicBackground = Brushes.Red;
                            }
                            else
                            {
                                NewUserPatronymicToolTip = null;
                                NewUserPatronymicBackground = Brushes.Transparent;

                                patronymic = NewUserPatronymic.Trim ();

                                isChanged = true;
                            }
                        }
                    }
                    else
                    {
                        NewUserPatronymicToolTip = null;
                        NewUserPatronymicBackground = Brushes.Transparent;
                    }

                    // Смена имейла
                    if (NewUserEmail != null && NewUserEmail.Length > 0)
                    {
                        if ((NewUserEmail.Length < 5 || NewUserEmail.Length > 30) || (!NewUserEmail.Contains ('@') || !NewUserEmail.Contains ('.')))
                        {
                            NewUserEmailToolTip = "Некорректный E-Mail. Необходимо значение от 5 до 30 символов, а также должно содержать символы '@' и '.'.";
                            NewUserEmailBackground = Brushes.Red;
                        }
                        else
                        {
                            if (NewUserEmail.Contains (' '))
                            {
                                NewUserEmailToolTip = "Значение не должно содержать пробелы.";
                                NewUserEmailBackground = Brushes.Red;
                            }
                            else
                            {
                                NewUserEmailToolTip = null;
                                NewUserEmailBackground = Brushes.Transparent;

                                email = NewUserEmail.Trim ();

                                isChanged = true;
                            }
                        }
                    }
                    else
                    {
                        NewUserEmailToolTip = null;
                        NewUserEmailBackground = Brushes.Transparent;
                    }

                    // Смена логина
                    if (NewUserLogin != null && NewUserLogin.Length > 0)
                    {
                        if (NewUserLogin.Length < 3 || NewUserLogin.Length > 20)
                        {
                            NewUserLoginToolTip = "Некорректный Login. Необходимо значение от 3 до 20 символов.";
                            NewUserLoginBackground = Brushes.Red;
                        }
                        else
                        {
                            if (NewUserLogin.Contains (' '))
                            {
                                NewUserLoginToolTip = "Значение не должно содержать пробелы.";
                                NewUserLoginBackground = Brushes.Red;
                            }
                            else
                            {
                                NewUserLoginToolTip = null;
                                NewUserLoginBackground = Brushes.Transparent;

                                login = NewUserLogin.Trim ();

                                isChanged = true;
                            }
                        }
                    }
                    else
                    {
                        NewUserLoginToolTip = null;
                        NewUserLoginBackground = Brushes.Transparent;
                    }

                    // Смена пароля
                    if (NewUserPassword != null && NewUserPassword.Length > 0)
                    {
                        if (NewUserPassword.Length < 5 || NewUserPassword.Length > 30)
                        {
                            NewUserPasswordToolTip = "Некорректный пароль. Необходимо значение от 5 до 30 символов.";
                            NewUserPasswordBackground = Brushes.Red;
                        }
                        else
                        {
                            if (NewUserPassword.Contains (' '))
                            {
                                NewUserPasswordToolTip = "Проверьте правильность введённого пароля.";
                                NewUserPasswordBackground = Brushes.Red;
                            }
                            else
                            {
                                foreach (char c in NewUserPassword)
                                {
                                    foreach (char s in symbols)
                                    {
                                        if (c == s)
                                        {
                                            correctPassword = true;

                                            break;
                                        }
                                        else
                                            correctPassword = false;
                                    }

                                    if (correctPassword)
                                        break;
                                }

                                foreach (char c in NewUserPassword)
                                {
                                    foreach (char uC in upperCase)
                                    {
                                        if (c == uC)
                                        {
                                            correctPassword = true;

                                            break;
                                        }
                                        else
                                            correctPassword = false;
                                    }

                                    if (correctPassword)
                                        break;
                                }

                                foreach (char c in NewUserPassword)
                                {
                                    foreach (char lC in lowerCase)
                                    {
                                        if (c == lC)
                                        {
                                            correctPassword = true;

                                            break;
                                        }
                                        else
                                            correctPassword = false;
                                    }

                                    if (correctPassword)
                                        break;
                                }

                                foreach (char c in NewUserPassword)
                                {
                                    foreach (char n in numbers)
                                    {
                                        if (c == n)
                                        {
                                            correctPassword = true;

                                            break;
                                        }
                                        else
                                            correctPassword = false;
                                    }

                                    if (correctPassword)
                                        break;
                                }

                                if (!correctPassword)
                                {
                                    NewUserPasswordToolTip = "Пароль должен содержать латинские буквы алфавита верхнего и нижнего регистров, хотя бы" +
                                        "один спецсимвол '!', '@', '#', '$', '%', '^', '&', '*' и хотя бы одну цифру (0-9).";
                                    NewUserPasswordBackground = Brushes.Red;
                                }
                                else
                                {
                                    string newPassword = cryptPass.Crypt (NewUserPassword.Trim ());

                                    if (newPassword == password)
                                    {
                                        NewUserPasswordToolTip = "Пароль совпадает со старым.";
                                        NewUserPasswordBackground = Brushes.Red;
                                    }
                                    else
                                    {
                                        NewUserPasswordToolTip = null;
                                        NewUserPasswordBackground = Brushes.Transparent;

                                        password = cryptPass.Crypt (NewUserPassword.Trim ());
                                        
                                        isChanged = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        NewUserPasswordToolTip = null;
                        NewUserPasswordBackground = Brushes.Transparent;
                    }

                    // Смена возраста
                    if (age != NewUserAge && ChangeAge == true)
                    {
                        age = NewUserAge.Trim ();

                        isChanged = true;
                    }

                    // Смена хобби
                    if (NewUserHobby.HobbyID != IDhobby && ChangeHobby == true)
                    {
                        IDhobby = NewUserHobby.HobbyID;

                        isChanged = true;

                        hobbyChanged = true;
                    }

                    // Применение изменений
                    if (isChanged)
                    {
                        string query = "UPDATE Users SET Surname_User = '" + surname + "', Name_User = '" + name + "', Patronymic_User = '" + patronymic + "', Age_User = '" + age + "', Email_User = '" + email + "', Login_User = '" + login + "', Password_User = '" + password + "', Hobby_ID = '" + IDhobby + "' WHERE ID_User = '" + userID + "'";
                        using (var cmd = new NpgsqlCommand (query, con))
                        {
                            cmd.ExecuteNonQuery ();
                        }

                        MessageBox.Show ("Данные пользователя успешно изменены.");

                        ReadAllUsers ();

                        NewUserSurname = null;
                        NewUserName = null;
                        NewUserPatronymic = null;
                        NewUserEmail = null;
                        NewUserLogin = null;
                        NewUserPassword = null;
                        DefaultSelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show ("Введите данные для изменения.");
                    }

                    if (hobbyChanged)
                    {
                        string queryUpdHobby = "UPDATE UsersHobbies SET Hobby_ID = '" + IDhobby + "' WHERE User_ID = '" + userID + "'";
                        using (var cmdUpdHobby = new NpgsqlCommand (queryUpdHobby, con))
                        {
                            cmdUpdHobby.ExecuteNonQuery ();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }

        private void AddUser ()
        {
            // Подсчёт выполненных условий для регистрации
            int isCorrectData = 0;

            // Подсчёт выполненных условий для пароля
            int correctPassword = 0;
            bool conditionMet = false;

            if (NewUserSurname == null || NewUserSurname.Length < 2 || NewUserSurname.Length > 20)
            {
                NewUserSurnameToolTip = "Некорректная фамилия. Необходимо значение от 2 до 20 символов.";
                NewUserSurnameBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                NewUserSurnameToolTip = null;
                NewUserSurnameBackground = Brushes.Transparent;

                isCorrectData += 1;
            }

            if (NewUserName == null || NewUserName.Length < 1 || NewUserName.Length > 15)
            {
                NewUserNameToolTip = "Некорректное имя. Необходимо значение от 1 до 15 символов.";
                NewUserNameBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                NewUserNameToolTip = null;
                NewUserNameBackground = Brushes.Transparent;

                isCorrectData += 1;
            }

            if (NewUserPatronymic != null && NewUserPatronymic.Length > 0)
            {
                if (NewUserPatronymic.Length < 3 || NewUserPatronymic.Length > 20 || NewUserPatronymic.Contains (' '))
                {
                    NewUserPatronymicToolTip = "Некорректное отчество. Необходимо значение от 3 до 20 символов, а так же значение не должно содержать пробелы.";
                    NewUserPatronymicBackground = Brushes.Red;

                    isCorrectData -= 1;
                }
                else
                {
                    NewUserPatronymicToolTip = null;
                    NewUserPatronymicBackground = Brushes.Transparent;

                    isCorrectData += 1;
                }
            }
            else
            {
                NewUserPatronymicToolTip = null;
                NewUserPatronymicBackground = Brushes.Transparent;

                NewUserPatronymic = "-";

                isCorrectData += 1;
            }

            if (NewUserLogin == null || NewUserLogin.Length < 3 || NewUserLogin.Length > 20)
            {
                NewUserLoginToolTip = "Некорректный Логин. Необходимо значение от 3 до 20 символов.";
                NewUserLoginBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                NewUserLoginToolTip = null;
                NewUserLoginBackground = Brushes.Transparent;

                isCorrectData += 1;
            }

            if (NewUserEmail == null || (NewUserEmail.Length < 5 || NewUserEmail.Length > 30) || (!NewUserEmail.Contains ('@') || !NewUserEmail.Contains ('.') || NewUserEmail.Contains(' ')))
            {
                NewUserEmailToolTip = "Некорректный E-Mail. Необходимо значение от 5 до 30 символов, а также E-Mail должен содержать символы '@' и '.'.";
                NewUserEmailBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                NewUserEmailToolTip = null;
                NewUserEmailBackground = Brushes.Transparent;

                isCorrectData += 1;
            }

            if (NewUserPassword == null || NewUserPassword.Length < 5 || NewUserPassword.Length > 30)
            {
                NewUserPasswordToolTip = "Некорректный пароль. Необходимо значение от 5 до 30 символов.";
                NewUserPasswordBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                if (NewUserPassword.Contains (' '))
                {
                    NewUserPasswordToolTip = "Проверьте правильность введённого пароля.";
                    NewUserPasswordBackground = Brushes.Red;

                    isCorrectData -= 1;
                }
                else
                {
                    // Проверка на наличие спецсимволов в пароле
                    foreach (char c in NewUserPassword)
                    {
                        conditionMet = false;

                        foreach (char s in symbols)
                        {
                            if (c == s)
                            {
                                correctPassword += 1;
                                conditionMet = true;

                                break;
                            }
                        }

                        if (conditionMet)
                            break;
                    }

                    // Проверка на наличие буквы верхнего регистра в пароле
                    foreach (char c in NewUserPassword)
                    {
                        conditionMet = false;

                        foreach (char uC in upperCase)
                        {
                            if (c == uC)
                            {
                                correctPassword += 1;
                                conditionMet = true;

                                break;
                            }
                        }

                        if (conditionMet)
                            break;
                    }

                    // Проверка на наличие буквы нижнего регистра в пароле
                    foreach (char c in NewUserPassword)
                    {
                        conditionMet = false;

                        foreach (char lC in lowerCase)
                        {
                            if (c == lC)
                            {
                                correctPassword += 1;
                                conditionMet = true;

                                break;
                            }
                        }

                        if (conditionMet)
                            break;
                    }

                    // Проверка на наличие цифры в пароле
                    foreach (char c in NewUserPassword)
                    {
                        conditionMet = false;

                        foreach (char n in numbers)
                        {
                            if (c == n)
                            {
                                correctPassword += 1;
                                conditionMet = true;

                                break;
                            }
                        }

                        if (conditionMet)
                            break;
                    }

                    if (correctPassword != 4)
                    {
                        NewUserPasswordToolTip = "Пароль должен содержать латинские буквы алфавита верхнего и нижнего регистров, минимум" +
                            "один спецсимвол '!', '@', '#', '$', '%', '^', '&', '*' и минимум одну цифру (0-9).";
                        NewUserPasswordBackground = Brushes.Red;

                        isCorrectData -= 1;
                    }
                    else
                    {
                        NewUserPasswordToolTip = null;
                        NewUserPasswordBackground = Brushes.Transparent;

                        isCorrectData += 1;
                    }
                }

                // Проверка общего кол-ва введённых корректных значений
                if (isCorrectData == 6)
                {
                    // Вызов функции для записи пользователя в БД
                    Add ();
                }
                else
                {
                    return;
                }
            }
        }

        private void Add ()
        {
            CryptPass cryptPass = new CryptPass ();

            int hobbyID = NewUserHobby.HobbyID;

            try
            {
                // Регистрация с общими данными (без аватарки)
                string cryptedPassword = cryptPass.Crypt (NewUserPassword);

                string queryUser = "INSERT INTO Users (Surname_User, Name_User, Patronymic_User, Age_User, Email_User, Login_User, Password_User, Hobby_ID, Role_ID, Avatar) VALUES (@Surname, @Name, @Patronymic, @AgeUser, @Email, @LoginUser, @PasswordUser, @HobbyID, @RoleID, @AvatarImg) RETURNING ID_User";
                using (var cmdUser = new NpgsqlCommand (queryUser, con))
                {
                    cmdUser.CommandType = CommandType.Text;

                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Surname", NewUserSurname));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Name", NewUserName));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Patronymic", NewUserPatronymic));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@AgeUser", NewUserAge));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Email", NewUserEmail));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@LoginUser", NewUserLogin));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@PasswordUser", cryptedPassword));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@HobbyID", hobbyID));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@RoleID", 1));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@AvatarImg", "defaultAvatar.png"));

                    this._ID = (int)cmdUser.ExecuteScalar ();
                }

                string queryHobby = "INSERT INTO UsersHobbies (Hobby_ID, User_ID) VALUES (@HobbyID, @UserID)";
                using (var cmdHobby = new NpgsqlCommand (queryHobby, con))
                {
                    cmdHobby.CommandType = CommandType.Text;
                    cmdHobby.Parameters.Add (new NpgsqlParameter ("@HobbyID", hobbyID));
                    cmdHobby.Parameters.Add (new NpgsqlParameter ("@UserID", this._ID));

                    cmdHobby.ExecuteScalar ();
                }

                MessageBox.Show ("Пользователь добавлен.");

                NewUserSurname = null;
                NewUserName = null;
                NewUserPatronymic = null;
                NewUserLogin = null;
                NewUserEmail = null;
                NewUserPassword = null;
                DefaultSelectedIndex = 0;

                ReadAllUsers ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
        #endregion

        #region Управление хобби
        private void ReadAllHobbies ()
        {
            FoundHobbies.Clear ();

            try
            {
                string query = "SELECT * FROM Hobbies WHERE ID_Hobby != '" + 0 + "'";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            Hobby hobbies = new Hobby ();

                            hobbies.HobbyID = rdr.GetInt32 (0);
                            hobbies.HobbyName = rdr.GetString (1);

                            FoundHobbies.Add (hobbies);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void AddHobby ()
        {
            if (NewHobby != null && NewHobby.Length > 0)
            {
                if (NewHobby.Length < 3 || NewHobby.Length > 20 || NewHobby.Contains (' '))
                {
                    NewHobbyToolTip = "Некорректное название.";
                    NewHobbyBackground = Brushes.Red;
                }
                else
                {
                    foreach (Hobby hobby in HobbiesList)
                    {
                        if (NewHobby == hobby.HobbyName)
                        {
                            NewHobbyToolTip = "Такое хобби уже существует.";
                            NewHobbyBackground = Brushes.Red;

                            return;
                        }
                    }

                    try
                    {
                        string query = "INSERT INTO Hobbies (Name_Hobby) VALUES (@NameHobby)";
                        using (var cmd = new NpgsqlCommand (query, con))
                        {
                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.Add (new NpgsqlParameter ("@NameHobby", NewHobby));

                            cmd.ExecuteNonQuery ();
                        }

                        MessageBox.Show ("Хобби добавлено.");

                        ReadAllHobbies ();
                        FillHobbyComBox ();

                        NewHobbyToolTip = null;
                        NewHobbyBackground = Brushes.Transparent;

                        NewHobby = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show (ex.ToString ());
                    }
                }
            }
            else
            {
                NewHobbyToolTip = "Заполните данные.";
                NewHobbyBackground = Brushes.Red;
            }
        }

        private void DeleteHobby ()
        {
            try
            {
                int hobbyID = SelectedHobbyDataGrid.HobbyID;

                string queryUpd = "UPDATE Users SET Hobby_ID = 0 WHERE Hobby_ID = '" + hobbyID + "'";
                using (var cmdUpd = new NpgsqlCommand (queryUpd, con))
                {
                    cmdUpd.ExecuteNonQuery ();
                }

                string queryDelFromStat = "DELETE FROM UsersHobbies WHERE Hobby_ID = '" + hobbyID + "'";
                using (var cmdDelFromStat = new NpgsqlCommand (queryDelFromStat, con))
                {
                    cmdDelFromStat.ExecuteNonQuery ();
                }

                string queryDel = "DELETE FROM Hobbies WHERE ID_Hobby = '" + hobbyID + "'";
                using (var cmdDel = new NpgsqlCommand (queryDel, con))
                {
                    cmdDel.ExecuteNonQuery ();
                }

                MessageBox.Show ("Хобби удалено.");

                ReadAllHobbies ();
                FillHobbyComBox ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
        #endregion

        #region Просмотр статистики

        /*
         * 
         * 
         * Просмотр статистики реализован в User Control HobbyChart.xaml и AdministratorView.xaml.cs
         * 
         * 
         */
       
        #endregion

        // Выход из учетной записи
        private void LogOut ()
        {
            con.Close ();

            ViewManager viewManager = new ViewManager ();
            viewManager.ShowView (0, -1, -1);

            CloseAction ();
        }

        // Ивент на случай закрытия окна
        public void OnWindowClosing (object sender, CancelEventArgs e)
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close ();
            }
        }
    }
}
