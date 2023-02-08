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
    public class AuthRegViewModel: INotifyPropertyChanged
    {
        readonly NpgsqlConnection con = AppContext.GetConnection ();
        readonly ViewManager viewManager = new ViewManager ();

        // Действие для закрытия окна
        public Action CloseAction { get; set; }
        
        private int _ID, _role;
        private bool _avatarSelected = false;

        #region Массивы символов для проверки пароля
        readonly char [] symbols = "!@#$%^&*".ToCharArray ();
        readonly char [] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray ();
        readonly char [] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray ();
        readonly char [] numbers = "0123456789".ToCharArray ();
        #endregion

        #region Свойства

        #region Свойства данных
        private string _pathToAvatar;
        public string PathToAvatar
        {
            get { return _pathToAvatar; }
            set
            {
                _pathToAvatar = value;
                OnPropertyChanged ("PathToAvatar");
            }
        }

        private BitmapImage _avatarSource;
        public BitmapImage AvatarSourse
        {
            get { return _avatarSource; }
            set 
            { 
                _avatarSource = value;
                OnPropertyChanged ("AvatarSourse");
            }
        }

        private ObservableCollection<Hobby> _hobbiesList = new ObservableCollection<Hobby> ();
        public ObservableCollection<Hobby> HobbiesList
        {
            get { return _hobbiesList; }
            set { _hobbiesList = value; }
        }

        private string _authLogin;
        public string AuthLogin
        {
            get { return _authLogin; }
            set
            {
                _authLogin = value;
                OnPropertyChanged ("AuthLogin");
            }
        }

        private string _regSurname;
        public string RegSurname
        {
            get { return _regSurname; }
            set
            {
                _regSurname = value;
                OnPropertyChanged ("RegSurname");
            }
        }

        private string _regName;
        public string RegName
        {
            get { return _regName; }
            set
            {
                _regName = value;
                OnPropertyChanged ("RegName");
            }
        }

        private string _regPatronymic;
        public string RegPatronymic
        {
            get { return _regPatronymic; }
            set
            {
                _regPatronymic = value;
                OnPropertyChanged ("RegPatronymic");
            }
        }

        private string _regAge;
        public string RegAge
        {
            get { return _regAge; }
            set
            {
                _regAge = value;
                OnPropertyChanged ("RegAge");
            }
        }

        private string _regLogin;
        public string RegLogin
        {
            get { return _regLogin; }
            set
            {
                _regLogin = value;
                OnPropertyChanged ("RegLogin");
            }
        }

        private string _regEmail;
        public string RegEmail
        {
            get { return _regEmail; }
            set
            {
                _regEmail = value;
                OnPropertyChanged ("RegEmail");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set 
            {
                _password = value;
                OnPropertyChanged ("Password");
            }
        }

        private string _passwordRepeat;
        public string PasswordRepeat
        {
            get { return _passwordRepeat; }
            set 
            {
                _passwordRepeat = value;
                OnPropertyChanged ("PasswordRepeat");
            }
        }

        private Hobby _regHobby;
        public Hobby RegHobby
        {
            get { return _regHobby; }
            set
            {
                _regHobby = value;
                OnPropertyChanged ("RegHobby");
            }
        }
        #endregion

        #region Свойства для элементов окна

        #region Свойства для размеров окна
        private int _authRegWindowHeight;
        public int AuthRegWindowHeight
        {
            get { return _authRegWindowHeight; }
            set
            {
                _authRegWindowHeight = value;
                OnPropertyChanged ("AuthRegWindowHeight");
            }
        }

        private int _authRegWindowWidth;
        public int AuthRegWindowWidth
        {
            get { return _authRegWindowWidth; }
            set {
                _authRegWindowWidth = value;
                OnPropertyChanged ("AuthRegWindowWidth");
            }
        }

        private int _authRegBoardHeight;
        public int AuthRegBoardHeight
        {
            get { return _authRegBoardHeight; }
            set
            {
                _authRegBoardHeight = value;
                OnPropertyChanged ("AuthRegBoardHeight");
            }
        }

        private int _authRegBoardWidth;
        public int AuthRegBoardWidth
        {
            get { return _authRegBoardWidth; }
            set
            {
                _authRegBoardWidth = value;
                OnPropertyChanged ("AuthRegBoardWidth");
            }
        }

        private int _authRegRowHeight;
        public int AuthRegRowHeight
        {
            get { return _authRegRowHeight; }
            set
            {
                _authRegRowHeight = value;
                OnPropertyChanged ("AuthRegRowHeight");

            }
        }

        private int _authRegSPWidth;
        public int AuthRegSPWidth
        {
            get { return _authRegSPWidth; }
            set
            {
                _authRegSPWidth = value;
                OnPropertyChanged ("AuthRegSPWidth");
            }
        }
        #endregion

        #region Свойства для изменения содержимого окна
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

        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                OnPropertyChanged ("HeaderText");
            }
        }

        private bool _swToRegIsEnbl;
        public bool SwToRegIsEnbl
        {
            get { return _swToRegIsEnbl; }
            set
            {
                _swToRegIsEnbl = value;
                OnPropertyChanged ("SwToRegIsEnbl");
            }
        }

        private bool _swToAuthIsEnbl;
        public bool SwToAuthIsEnbl
        {
            get { return _swToAuthIsEnbl; }
            set
            {
                _swToAuthIsEnbl = value;
                OnPropertyChanged ("SwToAuthIsEnbl");
            }
        }

        private Visibility _authBodyVisibility;
        public Visibility AuthBodyVisibility
        {
            get { return _authBodyVisibility; }
            set
            {
                _authBodyVisibility = value;
                OnPropertyChanged ("AuthBodyVisibility");
            }
        }

        private Visibility _regBodyVisibility;
        public Visibility RegBodyVisibility
        {
            get { return _regBodyVisibility; }
            set
            {
                _regBodyVisibility = value;
                OnPropertyChanged ("RegBodyVisibility");
            }
        }

        private int _defaultSelectedIndex;
        public int DefaultSelectedIndex
        {
            get { return _defaultSelectedIndex; }
            set { _defaultSelectedIndex = 0; }
        }
        #endregion

        #region Свойства ошибок
        private string _authSignInToolTip;
        public string AuthSignInToolTip
        {
            get { return _authSignInToolTip; }
            set
            {
                _authSignInToolTip = value;
                OnPropertyChanged ("AuthSignInToolTip");
            }
        }

        private Brush _authSignInBackground;
        public Brush AuthSignInBackground
        {
            get { return _authSignInBackground; }
            set
            {
                _authSignInBackground = value;
                OnPropertyChanged ("AuthSignInBackground");
            }
        }

        private string _regSurnameToolTip;
        public string RegSurnameToolTip
        {
            get { return _regSurnameToolTip; }
            set
            {
                _regSurnameToolTip = value;
                OnPropertyChanged ("RegSurnameToolTip");
            }
        }

        private Brush _regSurnameBackground;
        public Brush RegSurnameBackground
        {
            get { return _regSurnameBackground; }
            set
            {
                _regSurnameBackground = value;
                OnPropertyChanged ("RegSurnameBackground");
            }
        }

        private string _regNameToolTip;
        public string RegNameToolTip
        {
            get { return _regNameToolTip; }
            set
            {
                _regNameToolTip = value;
                OnPropertyChanged ("RegNameToolTip");
            }
        }

        private Brush _regNameBackground;
        public Brush RegNameBackground
        {
            get { return _regNameBackground; }
            set
            {
                _regNameBackground = value;
                OnPropertyChanged ("RegNameBackground");
            }
        }

        private string _regPatronymicToolTip;
        public string RegPatronymicToolTip
        {
            get { return _regPatronymicToolTip; }
            set
            {
                _regPatronymicToolTip = value;
                OnPropertyChanged ("RegPatronymicToolTip");
            }
        }

        private Brush _regPatronymicBackground;
        public Brush RegPatronymicBackground
        {
            get { return _regPatronymicBackground; }
            set
            {
                _regPatronymicBackground = value;
                OnPropertyChanged ("RegPatronymicBackground");
            }
        }

        private string _regLoginToolTip;
        public string RegLoginToolTip
        {
            get { return _regLoginToolTip; }
            set
            {
                _regLoginToolTip = value;
                OnPropertyChanged ("RegLoginToolTip");
            }
        }

        private Brush _regLoginBackground;
        public Brush RegLoginBackground
        {
            get { return _regLoginBackground; }
            set
            {
                _regLoginBackground = value;
                OnPropertyChanged ("RegLoginBackground");
            }
        }

        private string _regPasswordToolTip;
        public string RegPasswordToolTip
        {
            get { return _regPasswordToolTip; }
            set
            {
                _regPasswordToolTip = value;
                OnPropertyChanged ("RegPasswordToolTip");
            }
        }

        private Brush _regPasswordBackground;
        public Brush RegPasswordBackground
        {
            get { return _regPasswordBackground; }
            set
            {
                _regPasswordBackground = value;
                OnPropertyChanged ("RegPasswordBackground");
            }
        }

        private string _regEmailToolTip;
        public string RegEmailToolTip
        {
            get { return _regEmailToolTip; }
            set
            {
                _regEmailToolTip = value;
                OnPropertyChanged ("RegEmailToolTip");
            }
        }

        private Brush _regEmailBackground;
        public Brush RegEmailBackground
        {
            get { return _regEmailBackground; }
            set
            {
                _regEmailBackground = value;
                OnPropertyChanged ("RegEmailBackground");
            }
        }
        #endregion

        #endregion

        #endregion

        #region Кнопки
        private RelayCommand _switchToReg_Click;
        public RelayCommand SwitchToReg_Click
        {
            get
            {
                return _switchToReg_Click ?? (_switchToReg_Click = new RelayCommand (obj =>
                {
                    SwitchViews (true);
                }));
            }
        }

        private RelayCommand _switchToAuth_Click;
        public RelayCommand SwitchToAuth_Click
        {
            get
            {
                return _switchToAuth_Click ?? (_switchToAuth_Click = new RelayCommand (obj =>
                {
                    SwitchViews (false);
                }));
            }
        }

        private RelayCommand _signIn_Click;
        public RelayCommand SignIn_Click
        {
            get
            {
                return _signIn_Click ?? (_signIn_Click = new RelayCommand (obj =>
                {
                    // В obj передаётся пароль при авторизации, что позволяет не хранить пароль для авторизации в виде строки и
                    // повышает безопасность программы
                    SigningIn (obj);
                }));
            }
        }

        private RelayCommand _chooseNewAvatar_Click;
        public RelayCommand ChooseNewAvatar_Click
        {
            get
            {
                return _chooseNewAvatar_Click ?? (_chooseNewAvatar_Click = new RelayCommand (obj =>
                {
                    ChooseNewAvatar ();
                }));
            }
        }

        private RelayCommand _setDefaultAvatar_Click;
        public RelayCommand SetDefaultAvatar_Click
        {
            get
            {
                return _setDefaultAvatar_Click ?? (_setDefaultAvatar_Click = new RelayCommand (obj =>
                {
                    SetDefaultAvatar ();
                }));
            }
        }

        private RelayCommand _registration_Click;
        public RelayCommand Registration_Click
        {
            get
            {
                return _registration_Click ?? (_registration_Click = new RelayCommand (obj =>
                {
                    Registration ();
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
        public AuthRegViewModel ()
        {
            con.Open ();

            InitializeView ();
            FillHobbiesCB ();
        }

        // Функция для инициализации свойств окна
        private void InitializeView ()
        {
            AvatarSourse = new BitmapImage (new Uri ("pack://siteoforigin:,,,/avatars/defaultAvatar.png"));

            SwitchViews (false);
        }

        // --------------------------------------------------------------------------

        #region Реализация авторизации
        private void SigningIn (object param)
        {
            CryptPass cryptPass = new CryptPass ();

            var passwordBox = param as PasswordBox;
            if (passwordBox == null)
                return;

            string password = cryptPass.Crypt (passwordBox.Password);

            string checkLogin = String.Empty, checkPassword = String.Empty;

            if (AuthLogin == null || AuthLogin.Length == 0 || password.Length == 0)
            {
                AuthSignInToolTip = "Неверный логин или пароль.";
                AuthSignInBackground = Brushes.Red;

                return;
            }
            else
            {
                AuthSignInToolTip = null;
                AuthSignInBackground = Brushes.Transparent;

                try
                {
                    string querySignIn = "SELECT * FROM Users WHERE Login_User = '" + AuthLogin + "' AND Password_User = '" + password + "' AND Is_Deleted = False";
                    using (var cmd = new NpgsqlCommand(querySignIn, con))
                    {
                        cmd.Parameters.AddWithValue ("Login_User", AuthLogin);
                        cmd.Parameters.AddWithValue ("Password_User", password);

                        using (var rdr = cmd.ExecuteReader ())
                        {
                            if (rdr.Read ())
                            {
                                _ID = rdr.GetInt32 (0);
                                checkLogin = rdr.GetString (6);
                                checkPassword = rdr.GetString (7);
                                _role = rdr.GetInt32 (10);
                            }
                        }
                    }

                    if (AuthLogin == checkLogin && password == checkPassword)
                    {
                        if (_role == 0)
                        {
                            IViewService viewService = viewManager;
                            viewService.ShowView (1, _ID, _role);

                            con.Close ();

                            CloseAction ();
                        }
                        else if (_role == 1)
                        {
                            IViewService viewService = viewManager;
                            viewService.ShowView (2, _ID, _role);

                            con.Close ();

                            CloseAction ();
                        }
                        else
                        {
                            AuthSignInToolTip = "Неверный логин или пароль.";
                            AuthSignInBackground = Brushes.Red;

                            passwordBox.Clear ();

                            return;
                        }
                    }
                    else
                    {
                        AuthSignInToolTip = "Неверный логин или пароль.";
                        AuthSignInBackground = Brushes.Red;

                        passwordBox.Clear ();

                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }
        #endregion

        #region Реализация регистрации
        // Заполнене комбобокса Хобби
        private void FillHobbiesCB ()
        {
            try
            {
                string query = "SELECT * FROM Hobbies";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            Hobby hobby = new Hobby
                            {
                                HobbyID = rdr.GetInt32 (0),
                                HobbyName = rdr.GetString (1)
                            };

                            HobbiesList.Add (hobby);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        // Логика кнопки "Выбор аватара"
        private void ChooseNewAvatar ()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Pictures (*.png; *.img; *.jpg; *.jpeg)|*.png; *.img; *.jpg; *.jpeg",
                InitialDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog () == true)
            {
                PathToAvatar = openFileDialog.FileName;

                AvatarSourse = new BitmapImage (new Uri (PathToAvatar));

                _avatarSelected = true;
            }
        }

        // Логика кнопки "Сбросить аватар"
        private void SetDefaultAvatar ()
        {
            PathToAvatar = String.Empty;
            AvatarSourse = new BitmapImage (new Uri ("pack://siteoforigin:,,,/avatars/defaultAvatar.png"));

            _avatarSelected = false;
        }

        // Загрузка аватара в базу данных и папку ...\avatars\ в директории проекта
        private string UploadAvatar (string pathToAvatar)
        {
            string fileName = Path.GetFileName (pathToAvatar);
            string pathOfCurrentAvatars = Path.Combine (Environment.CurrentDirectory, @"avatars\");

            // Копирование файла в директорию проекта
            File.Copy (Path.Combine (Path.GetDirectoryName (pathToAvatar), fileName), Path.Combine (pathOfCurrentAvatars, fileName), true);

            return fileName;
        }

        // Проверка всех введённых данных при нажатии кнопки "Зарегистрироваться" и дальнейшая регистрация в случае верных данных
        private void Registration ()
        {
            // Подсчёт выполненных условий для регистрации
            int isCorrectData = 0;

            // Подсчёт выполненных условий для пароля
            int correctPassword = 0;
            bool conditionMet;

            #region Проверка фамилии
            if (RegSurname == null || RegSurname.Length < 2 || RegSurname.Length > 20)
            {
                RegSurnameToolTip = "Некорректная фамилия. Необходимо значение от 2 до 20 символов.";
                RegSurnameBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                RegSurnameToolTip = null;
                RegSurnameBackground = Brushes.Transparent;

                isCorrectData += 1;
            }
            #endregion

            #region Проверка имени
            if (RegName == null || RegName.Length < 1 || RegName.Length > 15)
            {
                RegNameToolTip = "Некорректное имя. Необходимо значение от 1 до 15 символов.";
                RegNameBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                RegNameToolTip = null;
                RegNameBackground = Brushes.Transparent;

                isCorrectData += 1;
            }
            #endregion

            #region Проверка отчества
            if (RegPatronymic != null && RegPatronymic.Length > 0)
            {
                if (RegPatronymic.Length < 3 || RegPatronymic.Length > 20 || RegPatronymic.Contains (' '))
                {
                    RegPatronymicToolTip = "Некорректное отчество. Необходимо значение от 3 до 20 символов, а так же значение не должно содержать пробелы.";
                    RegPatronymicBackground = Brushes.Red;

                    isCorrectData -= 1;
                }
                else
                {
                    RegPatronymicToolTip = null;
                    RegPatronymicBackground = Brushes.Transparent;

                    isCorrectData += 1;
                }
            }
            else
            {
                RegPatronymicToolTip = null;
                RegPatronymicBackground = Brushes.Transparent;

                RegPatronymic = "-";

                isCorrectData += 1;
            }
            #endregion

            #region Проверка логина
            if (RegLogin == null || RegLogin.Length < 3 || RegLogin.Length > 20 || RegLogin.Contains(' '))
            {
                RegLoginToolTip = "Некорректный Логин. Необходимо значение от 3 до 20 символов.";
                RegLoginBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                char [] lettersForLogin = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz".ToCharArray ();
                bool isLetter = false;

                foreach (char c in RegLogin)
                {
                    foreach (char l in lettersForLogin)
                    {
                        if (c == l)
                        {
                            isLetter = true;
                            
                            break;
                        }
                        else
                        {
                            isLetter = false;
                        }
                    }

                    if (!isLetter)
                        break;
                }

                if (!isLetter)
                {
                    RegLoginToolTip = "Некорректный Логин. Логин должен состоять из букв латинского алфавита верхнего или нижнего регистра.";
                    RegLoginBackground = Brushes.Red;

                    isCorrectData -= 1;
                }
                else
                {
                    RegLoginToolTip = null;
                    RegLoginBackground = Brushes.Transparent;

                    isCorrectData += 1;
                }
            }
            #endregion

            #region Проверка имейла
            if (RegEmail == null || (RegEmail.Length < 5 || RegEmail.Length > 30) || (!RegEmail.Contains ('@') || !RegEmail.Contains ('.') || RegEmail.Contains (' ')))
            {
                RegEmailToolTip = "Некорректный E-Mail. Необходимо значение от 5 до 30 символов, а также E-Mail должен содержать символы '@' и '.' и не должен содержать пробелы.";
                RegEmailBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                RegEmailToolTip = null;
                RegEmailBackground = Brushes.Transparent;

                isCorrectData += 1;
            }
            #endregion

            #region Проверка пароля
            if (Password == null || Password.Length < 5 || Password.Length > 30 || PasswordRepeat == null || PasswordRepeat.Length < 5 || PasswordRepeat.Length > 30 || Password.Contains (' ') || PasswordRepeat.Contains (' '))
            {
                RegPasswordToolTip = "Некорректный пароль. Необходимо значение от 5 до 30 символов, а так же не должен содержать пробелы.";
                RegPasswordBackground = Brushes.Red;

                isCorrectData -= 1;
            }
            else
            {
                if (Password != PasswordRepeat)
                {
                    RegPasswordToolTip = "Проверьте правильность введённого пароля.";
                    RegPasswordBackground = Brushes.Red;

                    isCorrectData -= 1;
                }
                else
                {
                    // Проверка на наличие спецсимволов в пароле
                    foreach (char c in PasswordRepeat)
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
                    foreach (char c in PasswordRepeat)
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
                    foreach (char c in PasswordRepeat)
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
                    foreach (char c in PasswordRepeat)
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
                        RegPasswordToolTip = "Пароль должен содержать латинские буквы алфавита верхнего и нижнего регистров, минимум" +
                            "один спецсимвол '!', '@', '#', '$', '%', '^', '&', '*' и минимум одну цифру (0-9).";
                        RegPasswordBackground = Brushes.Red;

                        isCorrectData -= 1;
                    }
                    else
                    {
                        RegPasswordToolTip = null;
                        RegPasswordBackground = Brushes.Transparent;

                        isCorrectData += 1;
                    }
                }

                // Проверка общего кол-ва введённых корректных значений
                if (isCorrectData == 6)
                {
                    // Вызов функции для записи пользователя в БД
                    Reg ();
                }
                else
                {
                    return;
                }
            }
            #endregion
        }

        // Функция для записи пользователя в БД
        private void Reg ()
        {
            CryptPass cryptPass = new CryptPass ();

            int hobbyID = RegHobby.HobbyID;
            string avatarName = String.Empty;

            // Запись пути аватарки, если _avatarSelected == true (если аватарка выбрана)
            if (_avatarSelected)
            {
                avatarName = UploadAvatar (PathToAvatar);
            }

            try
            {
                // Регистрация с общими данными (без аватарки)

                // Шифрование пароля
                string cryptedPassword = cryptPass.Crypt (PasswordRepeat);

                string queryUser = "INSERT INTO Users (Surname_User, Name_User, Patronymic_User, Age_User, Email_User, Login_User, Password_User, Hobby_ID, Role_ID, Avatar) VALUES (@Surname, @Name, @Patronymic, @AgeUser, @Email, @LoginUser, @PasswordUser, @HobbyID, @RoleID, @AvatarImg) RETURNING ID_User";
                using (var cmdUser = new NpgsqlCommand (queryUser, con))
                {
                    cmdUser.CommandType = CommandType.Text;

                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Surname", RegSurname));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Name", RegName));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Patronymic", RegPatronymic));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@AgeUser", RegAge));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@Email", RegEmail));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@LoginUser", RegLogin));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@PasswordUser", cryptedPassword));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@HobbyID", hobbyID));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@RoleID", 1));
                    cmdUser.Parameters.Add (new NpgsqlParameter ("@AvatarImg", "defaultAvatar.png"));

                    // Получение ID нового пользователя для добавления его записи в статистику Хобби и загрузки аватарки в случае выбора
                    this._ID = (int)cmdUser.ExecuteScalar ();
                }

                // Добавление аватарки в БД, если _avatarSelected == true (если аватарка выбрана)
                if (_avatarSelected)
                {
                    string querySetAvatar = "UPDATE Users SET Avatar = '" + avatarName + "' WHERE ID_User = '" + this._ID + "'";
                    using (var cmdSetAvatar = new NpgsqlCommand (querySetAvatar, con))
                    {
                        cmdSetAvatar.ExecuteNonQuery ();
                    }
                }

                // Добавление нового пользователя в статистику Хобби
                string queryHobby = "INSERT INTO UsersHobbies (Hobby_ID, User_ID) VALUES (@HobbyID, @UserID)";
                using (var cmdHobby = new NpgsqlCommand (queryHobby, con))
                {
                    cmdHobby.CommandType = CommandType.Text;
                    cmdHobby.Parameters.Add (new NpgsqlParameter ("@HobbyID", hobbyID));
                    cmdHobby.Parameters.Add (new NpgsqlParameter ("@UserID", this._ID));

                    cmdHobby.ExecuteScalar ();
                }

                MessageBox.Show ("Регистрация успешна.");

                // Переключение свойств окна на авторизацию
                SwitchViews (false);
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
        #endregion

        // Изменение свойств окна
        private void SwitchViews (bool isAuthActive)
        {
            if (isAuthActive)
            {
                HeaderText = "Регистрация";
                TitleText = "Let's Meet | Регистрация";

                SwToRegIsEnbl = false;
                SwToAuthIsEnbl = true;

                AuthBodyVisibility = Visibility.Collapsed;
                RegBodyVisibility = Visibility.Visible;

                AuthRegWindowHeight = 800;
                AuthRegWindowWidth = 800;

                AuthRegBoardHeight = 700;
                AuthRegBoardWidth = 700;

                AuthRegRowHeight = 550;
                AuthRegSPWidth = 650;
            }
            else
            {
                HeaderText = "Авторизация";
                TitleText = "Let's Meet | Авторизация";

                SwToRegIsEnbl = true;
                SwToAuthIsEnbl = false;

                AuthBodyVisibility = Visibility.Visible;
                RegBodyVisibility = Visibility.Collapsed;

                AuthRegWindowHeight = 450;
                AuthRegWindowWidth = 650;

                AuthRegBoardHeight = 350;
                AuthRegBoardWidth = 550;

                AuthRegRowHeight = 200;
                AuthRegSPWidth = 500;
            }
        }

        // Функция на случай принудительного закрытия окна
        public void OnWindowClosing (object sender, CancelEventArgs e)
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close ();
            }
        }
    }
}