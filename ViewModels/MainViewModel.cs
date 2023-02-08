using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Npgsql;
using System.IO;
using Path = System.IO.Path;
using Lets_Meet.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Lets_Meet.Models;
using WMPLib;
using System.Windows.Threading;

namespace Lets_Meet.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        NpgsqlConnection con = AppContext.GetConnection ();

        ViewManager viewManager = new ViewManager ();

        public Action CloseAction { get; set; }

        private int _ID = -1, _role = -1;

        // -----------------------------------------------------
        // Переменные личного кабинета
        private int _IDhobby;
        private string _surname, _name, _patronymic, _age, _login, _password, _email, _hobbyName;
        private bool _avatarSelected = false, _setDefault = false;
        // -----------------------------------------------------

        // -----------------------------------------------------
        // Переменные плеера
        public WindowsMediaPlayer mediaPlayer = new WindowsMediaPlayer ();

        DispatcherTimer _timerOfDuration = new DispatcherTimer ();

        private string _fileNameAndPath = string.Empty, _fileName = string.Empty;
        private int _minutes = 0, _seconds = 0;
        private double _positionInPlaylist = 0, _trackDuration = 0;
        private bool _flag = false, _stopped = false, _paused = false;
        // -----------------------------------------------------

        #region Массивы символов для проверки пароля
        char [] symbols = new char [] { '!', '@', '#', '$', '%', '^', '&', '*' };
        char [] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray ();
        char [] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray ();
        char [] numbers = "0123456789".ToCharArray ();
        #endregion

        #region Свойства

        #region Свойства View
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

        private int _viewHeight;
        public int ViewHeight
        {
            get { return _viewHeight; }
            set
            {
                _viewHeight = value;
                OnPropertyChanged ("ViewHeight");
            }
        }

        private int _viewWIdth;
        public int ViewWidth
        {
            get { return _viewWIdth; }
            set
            {
                _viewWIdth = value;
                OnPropertyChanged ("ViewWidth");
            }
        }
        #endregion

        #region Свойства вкладки чатов

        #region Свойства данных чатов
        private ObservableCollection<Chat> _foundChatsList = new ObservableCollection<Chat> ();
        public ObservableCollection<Chat> FoundChatsList
        {
            get { return _foundChatsList; }
            set
            {
                _foundChatsList = value;
                OnPropertyChanged ("FoundChatsList");
            }
        }

        private ObservableCollection<Message> _foundMessagesList = new ObservableCollection<Message> ();
        public ObservableCollection<Message> FoundMessagesList
        {
            get { return _foundMessagesList; }
            set
            {
                _foundMessagesList = value;
                OnPropertyChanged ("FoundMessagesList");
            }
        }

        private string _textOfMessage;
        public string TextOfMessage
        {
            get { return _textOfMessage; }
            set
            {
                _textOfMessage = value;
                OnPropertyChanged ("TextOfMessage");
            }
        }
        #endregion

        #region Свойства управления вкладки чатов
        private int _selectedChatIndex = -1;
        public int SelectedChatIndex
        {
            get { return _selectedChatIndex; }
            set
            {
                _selectedChatIndex = value;
                OnPropertyChanged ("SelectedChatIndex");
            }
        }

        private Chat _selectedChatID;
        public Chat SelectedChatID
        {
            get { return _selectedChatID; }
            set
            {
                _selectedChatID = value;
                OnPropertyChanged ("SelectedChatID");
            }
        }
        #endregion

        #endregion

        #region Свойства вкладки пользователей

        #region Свойства данных пользователей
        private ObservableCollection<User> _foundUsersList = new ObservableCollection<User> ();
        public ObservableCollection<User> FoundUsersList
        {
            get { return _foundUsersList; }
            set
            {
                _foundUsersList = value;
                OnPropertyChanged ("FoundUsersList");
            }
        }

        private ObservableCollection<Hobby> _hobbiesList = new ObservableCollection<Hobby> ();
        public ObservableCollection<Hobby> HobbiesList
        {
            get { return _hobbiesList; }
            set { _hobbiesList = value; }
        }
        #endregion

        #region Свойства управления вкладкой пользователей
        private string _searchBySurname;
        public string SearchBySurname
        {
            get { return _searchBySurname; }
            set
            {
                _searchBySurname = value;
                OnPropertyChanged ("SearchBySurname");
            }
        }

        private string _searchByAge;
        public string SearchByAge
        {
            get { return _searchByAge; }
            set
            {
                _searchByAge = value;
                OnPropertyChanged ("SearchByAge");
            }
        }

        private int _searchByAgeSelectedIndex = 0;
        public int SearchByAgeSelectedIndex
        {
            get { return _searchByAgeSelectedIndex; }
            set
            {
                _searchByAgeSelectedIndex = value;
                OnPropertyChanged ("SearchByAgeSelectedIndex");
            }
        }

        private Hobby _searchByHobby;
        public Hobby SelectedHobbyValue
        {
            get { return _searchByHobby; }
            set
            {
                _searchByHobby = value;
                OnPropertyChanged ("SelectedHobbyValue");
            }
        }

        private int _searchByHobbySelectedIndex = -1;
        public int SelectedHobbyIndex
        {
            get { return _searchByHobbySelectedIndex; }
            set
            {
                _searchByHobbySelectedIndex = value;
                OnPropertyChanged ("SelectedHobbyIndex");
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

        private int _selectedIndexUserDataGrid = -1;
        public int SelectedIndexUserDataGrid
        {
            get { return _selectedIndexUserDataGrid; }
            set
            {
                _selectedIndexUserDataGrid = value;
                OnPropertyChanged ("SelectedIndexUserDataGrid");
            }
        }
        #endregion

        #endregion

        #region Свойства вкладки личного кабинета

        #region Свойства отображения/скрытия полей
        private Visibility _showDataField;
        public Visibility ShowDataField
        {
            get { return _showDataField; }
            set
            {
                _showDataField = value;
                OnPropertyChanged ("ShowDataField");
            }
        }

        private Visibility _changeDataField;
        public Visibility ChangeDataField
        {
            get { return _changeDataField; }
            set
            {
                _changeDataField = value;
                OnPropertyChanged ("ChangeDataField");
            }
        }

        private Visibility _changeMyDataButtonVis;
        public Visibility ChangeMyDataButtonVis
        {
            get { return _changeMyDataButtonVis; }
            set
            {
                _changeMyDataButtonVis = value;
                OnPropertyChanged ("ChangeMyDataButtonVis");
            }
        }

        private Visibility _undoChangesButtonVis;
        public Visibility UndoChangesButtonVis
        {
            get { return _undoChangesButtonVis; }
            set
            {
                _undoChangesButtonVis = value;
                OnPropertyChanged ("UndoChangesButtonVis");
            }
        }

        private bool _changeMyDataButtonIsEnbl;
        public bool ChangeMyDataButtonIsEnbl
        {
            get { return _changeMyDataButtonIsEnbl; }
            set
            {
                _changeMyDataButtonIsEnbl = value;
                OnPropertyChanged ("ChangeMyDataButtonIsEnbl");
            }
        }

        private bool _saveChangesButtonIsEnbl;
        public bool SaveChangesButtonIsEnbl
        {
            get { return _saveChangesButtonIsEnbl; }
            set
            {
                _saveChangesButtonIsEnbl = value;
                OnPropertyChanged ("SaveChangesButtonIsEnbl");
            }
        }
        #endregion

        #region Свойства данных личного кабинета
        private string _myProfileHeaderText;
        public string MyProfileHeaderText
        {
            get { return _myProfileHeaderText; }
            set
            {
                _myProfileHeaderText = value;
                OnPropertyChanged ("MyProfileHeaderText");
            }
        }

        private BitmapImage _setAvatarPath;
        public BitmapImage AvatarSourse
        {
            get { return _setAvatarPath; }
            set
            {
                _setAvatarPath = value;
                OnPropertyChanged ("AvatarSourse");
            }
        }

        private string _currentAvatarImage;

        // Свойства для вывода данных
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

        private string _showUserSurname;
        public string ShowUserSurname
        {
            get { return _showUserSurname; }
            set
            {
                _showUserSurname = value;
                OnPropertyChanged ("ShowUserSurname");
            }
        }

        private string _showUserName;
        public string ShowUserName
        {
            get { return _showUserName; }
            set
            {
                _showUserName = value;
                OnPropertyChanged ("ShowUserName");
            }
        }

        private string _showUserPatronymic;
        public string ShowUserPatronymic
        {
            get { return _showUserPatronymic; }
            set
            {
                _showUserPatronymic = value;
                OnPropertyChanged ("ShowUserPatronymic");
            }
        }

        private string _showUserAge;
        public string ShowUserAge
        {
            get { return _showUserAge; }
            set
            {
                _showUserAge = value;
                OnPropertyChanged ("ShowUserAge");
            }
        }

        private string _showUserLogin;
        public string ShowUserLogin
        {
            get { return _showUserLogin; }
            set
            {
                _showUserLogin = value;
                OnPropertyChanged ("ShowUserLogin");
            }
        }

        private string _showUserEmail;
        public string ShowUserEmail
        {
            get { return _showUserEmail; }
            set
            {
                _showUserEmail = value;
                OnPropertyChanged ("ShowUserEmail");
            }
        }

        private string _showUserHobby;
        public string ShowUserHobby
        {
            get { return _showUserHobby; }
            set
            {
                _showUserHobby = value;
                OnPropertyChanged ("ShowUserHobby");
            }
        }

        // Свойства для изменения данных
        private string _typeNewEmail;
        public string TypeNewEmail
        {
            get { return _typeNewEmail; }
            set
            {
                _typeNewEmail = value;
                OnPropertyChanged ("TypeNewEmail");
            }
        }

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged ("NewPassword");
            }
        }

        private string _newPasswordRepeat;
        public string NewPasswordRepeat
        {
            get { return _newPasswordRepeat; }
            set
            {
                _newPasswordRepeat = value;
                OnPropertyChanged ("NewPasswordRepeat");
            }
        }

        private Hobby _newHobby;
        public Hobby NewHobby
        {
            get { return _newHobby; }
            set
            {
                _newHobby = value;
                OnPropertyChanged ("NewHobby");
            }
        }

        private int _changeHobbySelectedIndex = -1;
        public int SelectedChangeHobbyIndex
        {
            get { return _changeHobbySelectedIndex; }
            set
            {
                _changeHobbySelectedIndex = value;
                OnPropertyChanged ("SelectedChangeHobbyIndex");
            }
        }

        private bool _hobbyIsChanging = false;
        public bool HobbyIsChanging
        {
            get { return _hobbyIsChanging; }
            set
            {
                _hobbyIsChanging = value;
                OnPropertyChanged ("HobbyIsChanging");
            }
        }
        #endregion

        #region Свойства ошибок полей изменения данных
        private string _changeEmailToolTip;
        public string ChangeEmailToolTip
        {
            get { return _changeEmailToolTip; }
            set
            {
                _changeEmailToolTip = value;
                OnPropertyChanged ("ChangeEmailToolTip");
            }
        }

        private Brush _changeEmailBackground;
        public Brush ChangeEmailBackground
        {
            get { return _changeEmailBackground; }
            set
            {
                _changeEmailBackground = value;
                OnPropertyChanged ("ChangeEmailBackground");
            }
        }

        private string _changePasswordToolTip;
        public string ChangePasswordToolTip
        {
            get { return _changePasswordToolTip; }
            set
            {
                _changePasswordToolTip = value;
                OnPropertyChanged ("ChangePasswordToolTip");
            }
        }

        private Brush _changePasswordBackground;
        public Brush ChangePasswordBackground
        {
            get { return _changePasswordBackground; }
            set
            {
                _changePasswordBackground = value;
                OnPropertyChanged ("ChangePasswordBackground");
            }
        }
        #endregion

        #endregion

        #region Свойства плеера

        #region Свойства инициализации окна
        private BitmapImage _openFilesBtnImg;
        public BitmapImage OpenFilesBtnImg
        {
            get { return _openFilesBtnImg; }
            set
            {
                _openFilesBtnImg = value;
            }
        }

        private BitmapImage _deleteFilesBtnImg;
        public BitmapImage DeleteFilesBtnImg
        {
            get { return _deleteFilesBtnImg; }
            set
            {
                _deleteFilesBtnImg = value;
            }
        }

        private BitmapImage _playMusicBtnImg;
        public BitmapImage PlayMusicBtnImg
        {
            get { return _playMusicBtnImg; }
            set
            {
                _playMusicBtnImg = value;
            }
        }

        private BitmapImage _pauseMusicBtnImg;
        public BitmapImage PauseMusicBtnImg
        {
            get { return _pauseMusicBtnImg; }
            set
            {
                _pauseMusicBtnImg = value;
            }
        }

        private BitmapImage _stopMusicBtnImg;
        public BitmapImage StopMusicBtnImg
        {
            get { return _stopMusicBtnImg; }
            set
            {
                _stopMusicBtnImg = value;
            }
        }

        private BitmapImage _playBackBtnImg;
        public BitmapImage PlayBackBtnImg
        {
            get { return _playBackBtnImg; }
            set
            {
                _playBackBtnImg = value;
            }
        }

        private BitmapImage _playForwardBtnImg;
        public BitmapImage PlayForwardBtnImg
        {
            get { return _playForwardBtnImg; }
            set
            {
                _playForwardBtnImg = value;
            }
        }
        #endregion

        #region Свойства данных плеера
        private ObservableCollection<MusicFile> _infoBoxMP3 = new ObservableCollection<MusicFile> ();

        private ObservableCollection<MusicFile> _fileInfoMP3 = new ObservableCollection<MusicFile> ();
        public ObservableCollection<MusicFile> FileInfoMP3
        {
            get { return _fileInfoMP3; }
            set
            {
                _fileInfoMP3 = value;
                OnPropertyChanged ("FileInfoMP3");
            }
        }

        private ObservableCollection<MusicFile> _playlistMP3 = new ObservableCollection<MusicFile> ();
        public ObservableCollection<MusicFile> PlaylistMP3
        {
            get { return _playlistMP3; }
            set
            {
                _playlistMP3 = value;
                OnPropertyChanged ("PlaylistMP3");
            }
        }
        #endregion

        #region Свойства управления плеером
        private MusicFile _selectedMusicFile;
        public MusicFile SelectedMusicFile
        {
            get { return _selectedMusicFile; }
            set
            {
                _selectedMusicFile = value;
                OnPropertyChanged ("SelectedMusicFile");
            }
        }

        private int _volumeSliderMinimum;
        public int VolumeSliderMinimum
        {
            get { return _volumeSliderMinimum; }
            set
            {
                _volumeSliderMinimum = value;
                OnPropertyChanged ("VolumeSliderMinimum");
            }
        }

        private int _volumeSliderMaximum;
        public int VolumeSliderMaximum
        {
            get { return _volumeSliderMaximum; }
            set
            {
                _volumeSliderMaximum = value;
                OnPropertyChanged ("VolumeSliderMaximum");
            }
        }

        private int _volumeSliderValue;
        public int VolumeSliderValue
        {
            get { return _volumeSliderValue; }
            set
            {
                _volumeSliderValue = value;
                OnPropertyChanged ("VolumeSliderValue");
            }
        }

        private double _trackStatusSliderMinimum;
        public double TrackStatusSliderMinimum
        {
            get { return _trackStatusSliderMinimum; }
            set
            {
                _trackStatusSliderMinimum = value;
                OnPropertyChanged ("TrackStatusSliderMinimum");
            }
        }

        private double _trackStatusSliderMaximum;
        public double TrackStatusSliderMaximum
        {
            get { return _trackStatusSliderMaximum; }
            set
            {
                _trackStatusSliderMaximum = value;
                OnPropertyChanged ("TrackStatusSliderMaximum");
            }
        }

        private double _trackStatusSliderValue;
        public double TrackStatusSliderValue
        {
            get { return _trackStatusSliderValue; }
            set
            {
                _trackStatusSliderValue = value;
                OnPropertyChanged ("TrackStatusSliderValue");
            }
        }

        private int _playlistSelectedIndex = -1;
        public int PlaylistSelectedIndex
        {
            get { return _playlistSelectedIndex; }
            set
            {
                _playlistSelectedIndex = value;
                OnPropertyChanged ("PlaylistSelectedIndex");
            }
        }

        private string _trackStatusLabelValue;
        public string TrackStatusLabelValue
        {
            get { return _trackStatusLabelValue; }
            set
            {
                _trackStatusLabelValue = value;
                OnPropertyChanged ("TrackStatusLabelValue");
            }
        }
        #endregion

        #endregion

        #endregion

        #region Кнопки

        #region Кнопки чатов
        private RelayCommand _refreshChat_Click;
        public RelayCommand RefreshChat_Click
        {
            get
            {
                return _refreshChat_Click ?? (_refreshChat_Click = new RelayCommand (obg =>
                 {
                     RefreshChat ();
                 }));
            }
        }

        private RelayCommand _openChat_Click;
        public RelayCommand OpenChat_Click
        {
            get
            {
                return _openChat_Click ?? (_openChat_Click = new RelayCommand (obj =>
                   {
                       ReadMessages ();
                   }));
            }
        }

        private RelayCommand _deleteChat_Click;
        public RelayCommand DeleteChat_Click
        {
            get
            {
                return _deleteChat_Click ?? (_deleteChat_Click = new RelayCommand (obj =>
                   {
                       DeleteChat ();
                   }));
            }
        }

        private RelayCommand _sendMessage_Click;
        public RelayCommand SendMessage_Click
        {
            get
            {
                return _sendMessage_Click ?? (_sendMessage_Click = new RelayCommand (obj =>
                 {
                     SendMessage ();
                 }));
            }
        }
        #endregion

        #region Кнопки пользователей
        private RelayCommand _searchUsers_Click;
        public RelayCommand SearchUsers_Click
        {
            get
            {
                return _searchUsers_Click ?? (_searchUsers_Click = new RelayCommand (obj =>
                 {
                     if ((SearchBySurname == null || SearchBySurname.Length == 0) && SearchByAgeSelectedIndex == 0 && SelectedHobbyIndex == -1)
                     {
                         ResetSearch ();
                     }
                     else
                     {
                         SearchUsers ();
                     }
                 }));
            }
        }

        private RelayCommand _resetSearch_Click;
        public RelayCommand ResetSearch_Click
        {
            get
            {
                return _resetSearch_Click ?? (_resetSearch_Click = new RelayCommand (obj =>
                 {
                     ResetSearch ();
                 }));
            }
        }

        private RelayCommand _showUserProfile_Click;
        public RelayCommand ShowUserProfile_Click
        {
            get
            {
                return _showUserProfile_Click ?? (_showUserProfile_Click = new RelayCommand (obj =>
                 {
                     if (SelectedIndexUserDataGrid == -1)
                     {
                         MessageBox.Show ("Выберите пользователя.");
                     }
                     else
                     {
                         int userID = SelectedUserDataGrid.UserID;

                         viewManager.ShowView (3, userID, 1);
                     }
                 }));
            }
        }

        private RelayCommand _startChat_Click;
        public RelayCommand StartChat_Click
        {
            get
            {
                return _startChat_Click ?? (_startChat_Click = new RelayCommand (obj =>
                {
                    StartChat ();
                }));
            }
        }
        #endregion

        #region Кнопки личного кабинета
        private RelayCommand _changeMyData_Click;
        public RelayCommand ChangeMyData_Click
        {
            get
            {
                return _changeMyData_Click ?? (_changeMyData_Click = new RelayCommand (obj =>
                {
                    ProfileFields (true);
                }));
            }
        }

        private RelayCommand _saveChanges_Click;
        public RelayCommand SaveChanges_Click
        {
            get
            {
                return _saveChanges_Click ?? (_saveChanges_Click = new RelayCommand (obj =>
                 {
                     SaveChanges ();
                 }));
            }
        }

        private RelayCommand _undoChanges_Click;
        public RelayCommand UndoChanges_Click
        {
            get
            {
                return _undoChanges_Click ?? (_undoChanges_Click = new RelayCommand (obj =>
                {
                    UndoAvatarChanges ();
                    ProfileFields (false);
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

        private RelayCommand _undoAvatarChanges_Click;
        public RelayCommand UndoAvatarChanges_Click
        {
            get
            {
                return _undoAvatarChanges_Click ?? (_undoAvatarChanges_Click = new RelayCommand (obj =>
                {
                    UndoAvatarChanges ();
                }));
            }
        }
        #endregion

        #region Кнопки плеера
        private RelayCommand _browseFiles_Click;
        public RelayCommand BrowseFiles_Click
        {
            get
            {
                return _browseFiles_Click ?? (_browseFiles_Click = new RelayCommand (obj =>
                {
                    BrowseFiles ();
                }));
            }
        }

        private RelayCommand _deleteTrack_Click;
        public RelayCommand DeleteTrack_Click
        {
            get
            {
                return _deleteChat_Click ?? (_deleteChat_Click = new RelayCommand (obj =>
                {
                    DeleteTrack ();
                }));
            }
        }

        private RelayCommand _playMusic_Click;
        public RelayCommand PlayMusic_Click
        {
            get
            {
                return _playMusic_Click ?? (_playMusic_Click = new RelayCommand (obj =>
                {
                    PlayMusic ();
                }));
            }
        }

        private RelayCommand _pauseMusic_Click;
        public RelayCommand PauseMusic_Click
        {
            get
            {
                return _pauseMusic_Click ?? (_pauseMusic_Click = new RelayCommand (obj =>
                {
                    PauseMusic ();
                }));
            }
        }

        private RelayCommand _stopMusic_Click;
        public RelayCommand StopMusic_Click
        {
            get
            {
                return _stopMusic_Click ?? (_stopMusic_Click = new RelayCommand (obj =>
                {
                    StopMusic ();
                }));
            }
        }

        private RelayCommand _playBack_Click;
        public RelayCommand PlayBack_Click
        {
            get
            {
                return _playBack_Click ?? (_playBack_Click = new RelayCommand (obj =>
                {
                    PlayBack ();
                }));
            }
        }

        private RelayCommand _playForward_Click;
        public RelayCommand PlayForward_Click
        {
            get
            {
                return _playForward_Click ?? (_playForward_Click = new RelayCommand (obj =>
                {
                    PlayForward ();
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged ([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged (this, new PropertyChangedEventArgs (prop));
        }

        public MainViewModel (int ID, int role)
        {
            this._ID = ID;
            this._role = role;

            con.Open ();

            InitializeView ();
            InitPlayer ();

            ReadChatList ();

            ReadAllUsers ();
            ResetSearch ();

            ReadProfile ();
            ReadAllHobbies ();
        }

        private void InitializeView ()
        {
            AvatarSourse = new BitmapImage (new Uri ("pack://siteoforigin:,,,/avatars/defaultAvatar.png"));

            TitleText = "Let's Meet | Общение";
            ViewHeight = 650;
            ViewWidth = 1000;

            ProfileFields (false);
        }

        #region Вкладка чата
        private void ReadChatList ()
        {
            FoundChatsList.Clear ();

            try
            {
                string query = "SELECT * FROM Chats WHERE ID_Sender = '" + this._ID + "' OR ID_Reciever = '" + this._ID + "' AND Is_Deleted = False";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            Chat chats = new Chat ();

                            chats.ChatID = rdr.GetInt32 (0);
                            chats.IDSender = rdr.GetInt32 (1);
                            chats.IDReciever = rdr.GetInt32 (2);
                            chats.IsDeleted = rdr.GetBoolean (3);
                            chats.ChatName = String.Empty;

                            FoundChatsList.Add (chats);
                        }
                    }
                }

                GenerateChatName ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void GenerateChatName ()
        {
            string interlocutorName = String.Empty;

            int senderID, recieverID;

            try
            {
                foreach (Chat chat in FoundChatsList)
                {
                    senderID = chat.IDSender;
                    recieverID = chat.IDReciever;

                    if (this._ID == senderID)
                    {
                        string query = "SELECT * FROM Users WHERE ID_User = '" + recieverID + "'";
                        using (var cmd = new NpgsqlCommand (query, con))
                        {
                            using (var rdr = cmd.ExecuteReader ())
                            {
                                if (rdr.Read ())
                                {
                                    interlocutorName = rdr.GetString (1);
                                    interlocutorName += " " + rdr.GetString (2);
                                }
                            }
                        }

                        chat.ChatName = "Чат с " + interlocutorName;
                    }
                    else if (this._ID == recieverID)
                    {
                        string query = "SELECT * FROM Users WHERE ID_User = '" + senderID + "'";
                        using (var cmd = new NpgsqlCommand (query, con))
                        {
                            using (var rdr = cmd.ExecuteReader ())
                            {
                                if (rdr.Read ())
                                {
                                    interlocutorName = rdr.GetString (1);
                                    interlocutorName += " " + rdr.GetString (2);
                                }
                            }
                        }

                        chat.ChatName = "Чат с " + interlocutorName;
                    }
                    else
                    {
                        MessageBox.Show ("Неизвестный пользователь.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void ReadMessages ()
        {
            FoundMessagesList.Clear ();

            try
            {
                int chatID = SelectedChatID.ChatID;

                string query = "SELECT * FROM Messages WHERE ID_Chat = '" + chatID + "'";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            Message messages = new Message ();

                            messages.MessageID = rdr.GetInt32 (0);
                            messages.IdSender = rdr.GetInt32 (1);
                            messages.IdReciever = rdr.GetInt32 (2);
                            messages.TextMessage = rdr.GetString (3);
                            messages.IdChat = rdr.GetInt32 (4);

                            FoundMessagesList.Add (messages);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void RefreshChat ()
        {
            if (SelectedChatIndex == -1)
            {
                MessageBox.Show ("Выберите чат.");
            }
            else
            {
                ReadMessages ();
            }
        }

        private void DeleteChat ()
        {
            if (SelectedChatIndex == -1)
            {
                MessageBox.Show ("Выберите чат для удаления.");
            }
            else
            {
                try
                {
                    int chatID = SelectedChatID.ChatID;

                    string query = "UPDATE Chats SET Is_Deleted = True WHERE ID_Chat = '" + chatID + "'";
                    using (var cmd = new NpgsqlCommand (query, con))
                    {
                        cmd.ExecuteNonQuery ();
                    }

                    MessageBox.Show ("Чат удалён.");

                    ReadChatList ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }

        private void SendMessage ()
        {
            string senderName = GetSenderName ();

            if (SelectedChatIndex == -1)
            {
                MessageBox.Show ("Выберите чат.");
            }
            else if (TextOfMessage == null || TextOfMessage.Length == 0)
            {
                MessageBox.Show ("Введите сообщение.");
            }
            else
            {
                try
                {
                    int chatID = SelectedChatID.ChatID;
                    int senderID = SelectedChatID.IDSender;
                    int recieverID = SelectedChatID.IDReciever;

                    string textMessage = DateTime.Now.ToShortTimeString () + " " + senderName.Trim () + ": " + TextOfMessage.Trim ();

                    string query = "INSERT INTO Messages (ID_Sender, ID_Reciever, Text_Message, ID_Chat) VALUES (@IDSender, @IDReciever, @TextMessage, @IDChat)";
                    using (var cmd = new NpgsqlCommand (query, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add (new NpgsqlParameter ("@IDSender", senderID));
                        cmd.Parameters.Add (new NpgsqlParameter ("@IDReciever", recieverID));
                        cmd.Parameters.Add (new NpgsqlParameter ("@TextMessage", textMessage));
                        cmd.Parameters.Add (new NpgsqlParameter ("@IDChat", chatID));

                        cmd.ExecuteNonQuery ();
                    }

                    TextOfMessage = null;

                    ReadMessages ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }

        private string GetSenderName ()
        {
            try
            {
                string query = "SELECT * FROM Users WHERE ID_User = '" + this._ID + "'";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        if (rdr.Read ())
                        {
                            return rdr.GetString (2);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());

                return null;
            }
        }
        #endregion

        #region Вкладка пользователей
        private void ReadAllUsers ()
        {
            FoundUsersList.Clear ();

            try
            {
                string query = "SELECT * FROM Users WHERE ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
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
                            user.HobbyID = rdr.GetInt32 (8);
                            user.HobbyName = String.Empty;

                            FoundUsersList.Add (user);
                        }
                    }
                }

                GetUsersHobbiesNames ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void GetUsersHobbiesNames ()
        {
            foreach (User user in FoundUsersList)
            {
                int hobbyID = user.HobbyID;

                try
                {
                    string queryGetHobbyName = "SELECT * FROM Hobbies WHERE ID_Hobby = '" + hobbyID + "'";
                    using (var cmdGetHobbyName = new NpgsqlCommand (queryGetHobbyName, con))
                    {
                        using (var rdrGetHobbyName = cmdGetHobbyName.ExecuteReader ())
                        {
                            if (rdrGetHobbyName.Read ())
                            {
                                user.HobbyName = rdrGetHobbyName.GetString (1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }

        private void SearchUsers ()
        {
            FoundUsersList.Clear ();

            // Поиск по фамилии
            // -------------------------------------
            if ((SearchBySurname != null && SearchBySurname.Length != 0) && SearchByAgeSelectedIndex == 0 && SelectedHobbyIndex == -1)
            {
                try
                {
                    string querySur = "SELECT * FROM Users WHERE Surname_User LIKE '" + SearchBySurname + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdSur = new NpgsqlCommand (querySur, con))
                    {
                        using (var rdrSur = cmdSur.ExecuteReader ())
                        {
                            while (rdrSur.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrSur.GetInt32 (0);
                                user.Surname = rdrSur.GetString (1);
                                user.Name = rdrSur.GetString (2);
                                user.Patronymic = rdrSur.GetString (3);
                                user.Age = rdrSur.GetString (4);
                                user.Email = rdrSur.GetString (5);
                                user.HobbyID = rdrSur.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
            // -------------------------------------
            // Поиск по хобби
            // -------------------------------------
            else if ((SearchBySurname == null || SearchBySurname.Length == 0) && SearchByAgeSelectedIndex == 0 && SelectedHobbyIndex != -1)
            {
                int hobbyID = SelectedHobbyValue.HobbyID;

                try
                {
                    string queryHob = "SELECT * FROM Users WHERE Hobby_ID = '" + hobbyID + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdHob = new NpgsqlCommand (queryHob, con))
                    {
                        using (var rdrHob = cmdHob.ExecuteReader ())
                        {
                            while (rdrHob.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrHob.GetInt32 (0);
                                user.Surname = rdrHob.GetString (1);
                                user.Name = rdrHob.GetString (2);
                                user.Patronymic = rdrHob.GetString (3);
                                user.Age = rdrHob.GetString (4);
                                user.Email = rdrHob.GetString (5);
                                user.HobbyID = rdrHob.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
            // -------------------------------------
            // Поиск по возрасту
            // -------------------------------------
            else if ((SearchBySurname == null || SearchBySurname.Length == 0) && SearchByAgeSelectedIndex > 0 && SelectedHobbyIndex == -1)
            {
                try
                {
                    string queryAge = "SELECT * FROM Users WHERE Age_User = '" + SearchByAge + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdAge = new NpgsqlCommand (queryAge, con))
                    {
                        using (var rdrAge = cmdAge.ExecuteReader ())
                        {
                            while (rdrAge.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrAge.GetInt32 (0);
                                user.Surname = rdrAge.GetString (1);
                                user.Name = rdrAge.GetString (2);
                                user.Patronymic = rdrAge.GetString (3);
                                user.Age = rdrAge.GetString (4);
                                user.Email = rdrAge.GetString (5);
                                user.HobbyID = rdrAge.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
            // -------------------------------------
            // Поиск по фамилии и хобби
            // -------------------------------------
            else if ((SearchBySurname != null && SearchBySurname.Length != 0) && SearchByAgeSelectedIndex == 0 && SelectedHobbyIndex != -1)
            {
                int hobbyID = SelectedHobbyValue.HobbyID;

                try
                {
                    string querySurAndHo = "SELECT * FROM Users WHERE Surname_User LIKE '" + SearchBySurname + "' AND Hobby_ID = '" + hobbyID + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdSurAndHo = new NpgsqlCommand (querySurAndHo, con))
                    {
                        using (var rdrSurAndHo = cmdSurAndHo.ExecuteReader ())
                        {
                            while (rdrSurAndHo.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrSurAndHo.GetInt32 (0);
                                user.Surname = rdrSurAndHo.GetString (1);
                                user.Name = rdrSurAndHo.GetString (2);
                                user.Patronymic = rdrSurAndHo.GetString (3);
                                user.Age = rdrSurAndHo.GetString (4);
                                user.Email = rdrSurAndHo.GetString (5);
                                user.HobbyID = rdrSurAndHo.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
            // -------------------------------------
            // Поиск по фамилии и возрасту
            // -------------------------------------
            else if ((SearchBySurname != null && SearchBySurname.Length != 0) && SearchByAgeSelectedIndex > 0 && SelectedHobbyIndex == -1)
            {
                try
                {
                    string querySurAndAge = "SELECT * FROM Users WHERE Surname_User LIKE '" + SearchBySurname + "' AND Age_User = '" + SearchByAge + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdSurAndAge = new NpgsqlCommand (querySurAndAge, con))
                    {
                        using (var rdrSurAndAge = cmdSurAndAge.ExecuteReader ())
                        {
                            while (rdrSurAndAge.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrSurAndAge.GetInt32 (0);
                                user.Surname = rdrSurAndAge.GetString (1);
                                user.Name = rdrSurAndAge.GetString (2);
                                user.Patronymic = rdrSurAndAge.GetString (3);
                                user.Age = rdrSurAndAge.GetString (4);
                                user.Email = rdrSurAndAge.GetString (5);
                                user.HobbyID = rdrSurAndAge.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
            // -------------------------------------
            // Поиск по возрасту и хобби
            // -------------------------------------
            else if ((SearchBySurname == null || SearchBySurname.Length == 0) && SearchByAgeSelectedIndex > 0 && SelectedHobbyIndex != -1)
            {
                int hobbyID = SelectedHobbyValue.HobbyID;

                try
                {
                    string queryHoAndAge = "SELECT * FROM Users WHERE Hobby_ID = '" + hobbyID + "' AND Age_User = '" + SearchByAge + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdHoAndAge = new NpgsqlCommand (queryHoAndAge, con))
                    {
                        using (var rdrHoAndAge = cmdHoAndAge.ExecuteReader ())
                        {
                            while (rdrHoAndAge.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrHoAndAge.GetInt32 (0);
                                user.Surname = rdrHoAndAge.GetString (1);
                                user.Name = rdrHoAndAge.GetString (2);
                                user.Patronymic = rdrHoAndAge.GetString (3);
                                user.Age = rdrHoAndAge.GetString (4);
                                user.Email = rdrHoAndAge.GetString (5);
                                user.HobbyID = rdrHoAndAge.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
            // -------------------------------------
            // Поиск по всем параметрам
            // -------------------------------------
            else
            {
                int hobbyID = SelectedHobbyValue.HobbyID;
                try
                {
                    string queryTotal = "SELECT * FROM Users WHERE Surname_User LIKE '" + SearchBySurname + "' AND Age_User = '" + SearchByAge + "' AND Hobby_ID = '" + hobbyID + "' AND ID_User != '" + this._ID + "' AND Role_ID != '" + 0 + "' AND Is_Deleted = False";
                    using (var cmdTotal = new NpgsqlCommand (queryTotal, con))
                    {
                        using (var rdrTotal = cmdTotal.ExecuteReader ())
                        {
                            while (rdrTotal.Read ())
                            {
                                User user = new User ();

                                user.UserID = rdrTotal.GetInt32 (0);
                                user.Surname = rdrTotal.GetString (1);
                                user.Name = rdrTotal.GetString (2);
                                user.Patronymic = rdrTotal.GetString (3);
                                user.Age = rdrTotal.GetString (4);
                                user.Email = rdrTotal.GetString (5);
                                user.HobbyID = rdrTotal.GetInt32 (8);
                                user.HobbyName = String.Empty;

                                FoundUsersList.Add (user);
                            }
                        }
                    }

                    GetUsersHobbiesNames ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }

        private void ResetSearch ()
        {
            SearchBySurname = null;
            SearchByAgeSelectedIndex = 0;
            SelectedHobbyIndex = -1;

            ReadAllUsers ();
        }

        private void StartChat ()
        {
            if (SelectedIndexUserDataGrid == -1)
            {
                MessageBox.Show ("Выберите пользователя.");
            }
            else
            {
                try
                {
                    int recieverID = SelectedUserDataGrid.UserID;

                    string query = "INSERT INTO Chats (ID_Sender, ID_Reciever) VALUES (@IDSender, @IDReciever)";
                    using (var cmd = new NpgsqlCommand (query, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add (new NpgsqlParameter ("@IDSender", this._ID));
                        cmd.Parameters.Add (new NpgsqlParameter ("@IDReciever", recieverID));

                        cmd.ExecuteNonQuery ();
                    }

                    MessageBox.Show ("Чат создан.");

                    ReadChatList ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.ToString ());
                }
            }
        }
        #endregion

        #region Вкладка личного кабинета
        private void ReadProfile ()
        {
            _currentAvatarImage = Path.Combine (Environment.CurrentDirectory, @"avatars\");

            string queryMyProfile = "SELECT * FROM Users WHERE ID_User = '" + this._ID + "'";
            using (var cmdPrfl = new NpgsqlCommand (queryMyProfile, con))
            {
                using (var rdrPrfl = cmdPrfl.ExecuteReader ())
                {
                    if (rdrPrfl.Read ())
                    {
                        _surname = rdrPrfl.GetString (1);
                        _name = rdrPrfl.GetString (2);
                        _patronymic = rdrPrfl.GetString (3);
                        _age = rdrPrfl.GetString (4);
                        _email = rdrPrfl.GetString (5);
                        _login = rdrPrfl.GetString (6);
                        _password = rdrPrfl.GetString (7);
                        _IDhobby = rdrPrfl.GetInt32 (8);
                        _currentAvatarImage += rdrPrfl.GetString (11);
                    }
                }
            }

            string queryHby = "SELECT * FROM Hobbies WHERE ID_Hobby = '" + _IDhobby + "'";
            using (var cmdHby = new NpgsqlCommand (queryHby, con))
            {
                using (var rdrHby = cmdHby.ExecuteReader ())
                {
                    if (rdrHby.Read ())
                    {
                        Hobby currentHobby = new Hobby ();

                        currentHobby.HobbyID = rdrHby.GetInt32 (0);
                        currentHobby.HobbyName = _hobbyName = rdrHby.GetString (1);

                        NewHobby = currentHobby;
                    }
                }
            }

            ShowUserSurname = "Ваша фамилия: " + _surname.Trim ();
            ShowUserName = "Ваше имя: " + _name.Trim ();
            ShowUserPatronymic = "Ваше отчество: " + _patronymic.Trim ();
            ShowUserAge = "Ваш возраст: " + _age.ToString ().Trim ();
            ShowUserLogin = "Ваш Логин: " + _login.Trim ();
            ShowUserEmail = "Ваша эл. почта: " + _email.Trim ();
            ShowUserHobby = "Ваше хобби: " + _hobbyName.Trim ();

            AvatarSourse = new BitmapImage (new Uri (_currentAvatarImage));
        }

        private void ReadAllHobbies ()
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

        private void ChooseNewAvatar ()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog ();

            openFileDialog.Filter = "Pictures (*.png; *.img; *.jpg; *.jpeg)|*.png; *.img; *.jpg; *.jpeg";
            openFileDialog.InitialDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog () == true)
            {
                PathToAvatar = openFileDialog.FileName;

                AvatarSourse = new BitmapImage (new Uri (PathToAvatar));

                _avatarSelected = true;
                _setDefault = false;
            }
        }

        private void UndoAvatarChanges ()
        {
            PathToAvatar = null;

            BitmapImage userAvatarImage = new BitmapImage (new Uri (_currentAvatarImage));

            AvatarSourse = userAvatarImage;

            _avatarSelected = false;
            _setDefault = false;
        }

        private void SetDefaultAvatar ()
        {
            PathToAvatar = String.Empty;
            AvatarSourse = new BitmapImage (new Uri ("pack://siteoforigin:,,,/avatars/defaultAvatar.png"));

            _avatarSelected = true;
            _setDefault = true;
        }

        private string UploadAvatar (string pathToAvatar)
        {
            string fileName = Path.GetFileName (pathToAvatar);
            string pathOfCurrentAvatars = Path.Combine (Environment.CurrentDirectory, @"avatars\");

            // Копирование файла в корневую папку программы
            File.Copy (Path.Combine (Path.GetDirectoryName (pathToAvatar), fileName), Path.Combine (pathOfCurrentAvatars, fileName), true);

            return fileName;
        }

        private void ProfileFields (bool isChanging)
        {
            if (!isChanging)
            {
                MyProfileHeaderText = "Моя страница - Просмотр";

                ShowDataField = Visibility.Visible;
                ChangeDataField = Visibility.Collapsed;

                ChangeMyDataButtonIsEnbl = true;
                SaveChangesButtonIsEnbl = false;
                ChangeMyDataButtonVis = Visibility.Visible;
                UndoChangesButtonVis = Visibility.Collapsed;

                TypeNewEmail = null;
                NewPassword = null;
                NewPasswordRepeat = null;
                SelectedHobbyIndex = 0;
                HobbyIsChanging = false;
            }
            else
            {
                MyProfileHeaderText = "Моя страница - Изменение";

                ShowDataField = Visibility.Collapsed;
                ChangeDataField = Visibility.Visible;

                ChangeMyDataButtonIsEnbl = false;
                SaveChangesButtonIsEnbl = true;
                ChangeMyDataButtonVis = Visibility.Collapsed;
                UndoChangesButtonVis = Visibility.Visible;
            }
        }

        private void SaveChanges ()
        {
            CryptPass cryptPass = new CryptPass ();

            bool emailChanged, passwordChanged, hobbyChanged, dataChanged = false;
            int newHobbyID = NewHobby.HobbyID;

            // Смена имейла
            if (TypeNewEmail != null && TypeNewEmail.Length > 0)
            {
                if ((TypeNewEmail.Length < 5 || TypeNewEmail.Length > 30) || (!TypeNewEmail.Contains ('@') || !TypeNewEmail.Contains ('.')) || TypeNewEmail.Contains (' '))
                {
                    ChangeEmailToolTip = "Некорректный E-Mail. Необходимо значение от 5 до 30 символов, а также E-Mail должен содержать символы '@' и '.' и не должен содержать пробелы.";
                    ChangeEmailBackground = Brushes.Red;

                    return;
                }

                if (TypeNewEmail.Trim () == _email)
                {
                    ChangeEmailToolTip = "Новый E-Mail совпадает со старым.";
                    ChangeEmailBackground = Brushes.Red;

                    return;
                }

                emailChanged = true;

                ChangeEmailToolTip = null;
                ChangeEmailBackground = Brushes.Transparent;
            }
            else
            {
                ChangeEmailToolTip = null;
                ChangeEmailBackground = Brushes.Transparent;

                emailChanged = false;
            }

            // Смена пароля
            if ((NewPassword != null && NewPassword.Length > 0) && (NewPasswordRepeat != null && NewPasswordRepeat.Length > 0))
            {
                // Подсчёт выполненных условий для пароля
                int correctPassword = 0;
                bool conditionMet = false;

                // Общая проверка введён ли пароль
                if ((NewPassword.Length < 5 || NewPassword.Length > 30) || (NewPasswordRepeat.Length < 5 || NewPasswordRepeat.Length > 30) || NewPassword.Contains (' ') || NewPasswordRepeat.Contains (' '))
                {
                    ChangePasswordToolTip = "Некорректный пароль. Необходимо значение от 5 до 30 символов, а так же не должен содержать пробелы.";
                    ChangePasswordBackground = Brushes.Red;

                    return;
                }

                // Совпадают ли введённые пароли
                if (NewPassword.Trim () != NewPasswordRepeat.Trim ())
                {
                    ChangePasswordToolTip = "Проверьте правильность введённого пароля.";
                    ChangePasswordBackground = Brushes.Red;

                    return;
                }

                // Шифрование нового пароля для проверки с записью в БД
                string newPassword = cryptPass.Crypt (NewPassword.Trim ());

                // Совпадает ли новый пароль со старым
                if (newPassword == _password)
                {
                    ChangePasswordToolTip = "Пароль совпадает со старым.";
                    ChangePasswordBackground = Brushes.Red;

                    return;
                }

                // Проверка на наличие спецсимволов в пароле
                foreach (char c in NewPasswordRepeat)
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
                foreach (char c in NewPasswordRepeat)
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
                foreach (char c in NewPasswordRepeat)
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
                foreach (char c in NewPasswordRepeat)
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
                    ChangePasswordToolTip = "Пароль должен содержать латинские буквы алфавита верхнего и нижнего регистров, минимум" +
                        "один спецсимвол '!', '@', '#', '$', '%', '^', '&', '*' и минимум одну цифру (0-9).";
                    ChangePasswordBackground = Brushes.Red;

                    passwordChanged = false;

                    return;
                }
                else
                {
                    ChangePasswordToolTip = null;
                    ChangePasswordBackground = Brushes.Transparent;

                    passwordChanged = true;
                }
            }
            else
            {
                ChangePasswordToolTip = null;
                ChangePasswordBackground = Brushes.Transparent;

                passwordChanged = false;
            }

            // Смена хобби
            if (newHobbyID != _IDhobby && newHobbyID != -1)
            {
                hobbyChanged = true;
            }
            else
            {
                hobbyChanged = false;
            }

            // Применение изменений
            try
            {
                // Смена аватара
                if (_avatarSelected)
                {
                    string avatarName = String.Empty;

                    if (_setDefault)
                    {
                        avatarName = "defaultAvatar.png";
                    }
                    else
                    {
                        avatarName = UploadAvatar (PathToAvatar);
                    }

                    string queryUpdateAvatar = "UPDATE Users SET Avatar = '" + avatarName + "' WHERE ID_User = '" + this._ID + "'";
                    using (var cmdUpdateAvatar = new NpgsqlCommand (queryUpdateAvatar, con))
                    {
                        cmdUpdateAvatar.ExecuteNonQuery ();
                    }

                    dataChanged = true;
                }

                if (emailChanged)
                {
                    string queryUpdateEmail = "UPDATE Users SET Email_User = '" + TypeNewEmail + "' WHERE ID_User = '" + this._ID + "'";
                    using (var cmdUpdateEmail = new NpgsqlCommand (queryUpdateEmail, con))
                    {
                        cmdUpdateEmail.ExecuteNonQuery ();
                    }

                    dataChanged = true;
                }

                if (passwordChanged)
                {
                    string queryUpdatePassword = "UPDATE Users SET Password_User = '" + cryptPass.Crypt (NewPassword.Trim ()) + "' WHERE ID_User = '" + this._ID + "'";
                    using (var cmdUpdatePassword = new NpgsqlCommand (queryUpdatePassword, con))
                    {
                        cmdUpdatePassword.ExecuteNonQuery ();
                    }

                    dataChanged = true;
                }

                if (hobbyChanged)
                {
                    string queryUpdateHobby = "UPDATE Users SET Hobby_ID = '" + newHobbyID + "' WHERE ID_User = '" + this._ID + "'";
                    using (var cmdUpdateHobby = new NpgsqlCommand (queryUpdateHobby, con))
                    {
                        cmdUpdateHobby.ExecuteNonQuery ();
                    }

                    dataChanged = true;
                }

                if (dataChanged)
                {
                    MessageBox.Show ("Данные успешно изменены!");

                    ReadProfile ();

                    TypeNewEmail = null;
                    ChangeEmailToolTip = null;
                    ChangeEmailBackground = Brushes.Transparent;

                    NewPassword = null;
                    NewPasswordRepeat = null;
                    ChangePasswordToolTip = null;
                    ChangePasswordBackground = Brushes.Transparent;

                    ProfileFields (false);
                }
                else
                {
                    MessageBox.Show ("Введите данные для изменения!");

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
        #endregion

        #region Вкладка плеера
        // Инициализация вкладки плеера (установка картинок на кнопки, инициализация элементов управления
        private void InitPlayer ()
        {
            OpenFilesBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/open.png"));
            DeleteFilesBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/delete.png"));
            PlayBackBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/play_bck.png"));
            PlayMusicBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/play.png"));
            PauseMusicBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/pause.png"));
            StopMusicBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/stop.png"));
            PlayForwardBtnImg = new BitmapImage (new Uri ("pack://siteoforigin:,,,/img/UI/play_fwd.png"));

            mediaPlayer.PlayStateChange += OnStateChanged;
            _timerOfDuration.Interval = new TimeSpan (10000000);
            _timerOfDuration.Tick += OnTick;
            VolumeSliderMaximum = 100;
            VolumeSliderMinimum = 0;
            VolumeSliderValue = 50;
            mediaPlayer.settings.volume = VolumeSliderValue;

            TrackStatusLabelValue = "00:00";
        }

        // Определение продолжительности трека
        private void OnStateChanged (Int32 state)
        {
            if (state == 1)
            {
                if (_stopped)
                {
                    _timerOfDuration.Stop ();

                    TrackStatusSliderValue = 0;
                }
                else if (state == 1 && !_stopped)
                {
                    if (_flag)
                    {
                        _timerOfDuration.Stop ();

                        var currentindex = PlaylistSelectedIndex + 1;

                        if (currentindex >= PlaylistMP3.Count)
                        {
                            currentindex = 0;
                        }

                        PlaylistSelectedIndex = currentindex;
                        mediaPlayer.URL = (PlaylistMP3 [currentindex] as MusicFile).Path;
                        AddToListMP3 ();
                        _trackDuration = Math.Round ((PlaylistMP3 [currentindex] as MusicFile).Duration);
                        TrackStatusSliderMaximum = _trackDuration;
                        TrackStatusSliderMinimum = 0;
                        TrackStatusSliderValue = 0;

                        _timerOfDuration.Interval = new TimeSpan (10000000);

                        _timerOfDuration.Start ();
                        _flag = false;
                    }
                }
            }
            else if (state == 2)
            {
                _timerOfDuration.Stop ();
            }
        }

        // Текущая позиция в минутах и секундах
        private void OnTick (object sender, EventArgs e)
        {
            TrackStatusSliderValue++;
            double dif = TrackStatusSliderMaximum - TrackStatusSliderValue;
            int m = GetMinutes (dif);
            int s = GetSeconds (dif, m);
            
            if (!_stopped)
                TrackStatusLabelValue = String.Format ("{0}:{1}", Convert.ToString (m), Convert.ToString (s));

            if (!_flag)
            {
                mediaPlayer.controls.play ();
            }
        }

        // Получение ID3Tag'а из аудиодорожки
        /*
         * 
         * Стоит отметить, что не во всех треках есть ID3Tag.
         * Так что в некоторых случаях он может не отображаться в информации о треке,
         * либо отображаемая информация может быть неполная.
         * 
         */
        private void WriteId3Tag (MusicFile file)
        {
            byte [] arrbid3 = new byte [128];

            for (int i = 0; i < arrbid3.Length; i++)
            {
                arrbid3 [i] = 0;
            }

            var buffer = Encoding.GetEncoding (1251).GetBytes ("TAG");
            Array.Copy (buffer, 0, arrbid3, 0, buffer.Length);
            buffer = Encoding.GetEncoding (1251).GetBytes (file.ID3Title);
            Array.Copy (buffer, 0, arrbid3, 3, buffer.Length);
            buffer = Encoding.GetEncoding (1251).GetBytes (file.ID3Artist);
            Array.Copy (buffer, 0, arrbid3, 33, buffer.Length);
            buffer = Encoding.GetEncoding (1251).GetBytes (file.ID3Album);
            Array.Copy (buffer, 0, arrbid3, 63, buffer.Length);
            buffer = Encoding.GetEncoding (1251).GetBytes (file.ID3Year);
            Array.Copy (buffer, 0, arrbid3, 93, buffer.Length);
            buffer = Encoding.GetEncoding (1251).GetBytes (file.ID3Commnet);
            Array.Copy (buffer, 0, arrbid3, 97, buffer.Length);

            arrbid3 [126] = file.ID3TrackNumber;
            arrbid3 [127] = file.ID3Genre;

            using (FileStream fstream = new FileStream (file.Path, FileMode.Open, FileAccess.Write))
            {
                fstream.Seek (-128, SeekOrigin.End);
                fstream.Write (arrbid3, 0, 128);
            }

            file.HasID3Tag = true;
        }

        private void AddToListMP3 ()
        {
            if (_fileInfoMP3.Count != 0)
            {
                _fileInfoMP3.Clear ();
            }

            _fileInfoMP3.Add (_infoBoxMP3 [PlaylistSelectedIndex]);
        }

        // Создание объекта трека
        private MusicFile TrackCreate (string filename, string name)
        {
            MusicFile file = new MusicFile ();

            ReadMP3File (ref file, filename);

            file.Path = filename;
            file.Name = name;
            file.MPtime = mediaPlayer.newMedia (this._fileNameAndPath).durationString;
            file.Duration = mediaPlayer.newMedia (this._fileNameAndPath).duration;

            GetMinutes (file.Duration);

            return file;
        }

        // Чтение данных трека
        private void ReadMP3File (ref MusicFile file, string filename)
        {
            byte [] buffer = new byte [128];

            using (FileStream fstream = new FileStream (filename, FileMode.Open, FileAccess.Read))
            {
                fstream.Seek (-128, SeekOrigin.End);
                fstream.Read (buffer, 0, 128);
            }

            Encoding.RegisterProvider (CodePagesEncodingProvider.Instance);
            string id3Tag = Encoding.GetEncoding (1251).GetString (buffer);

            if (id3Tag.Substring (0, 3) == "TAG")
            {
                file.ID3Album = id3Tag.Substring (63, 30).Trim ();
                file.ID3Artist = id3Tag.Substring (33, 30).Trim ();
                file.ID3Commnet = id3Tag.Substring (97, 28).Trim ();
                file.ID3Title = id3Tag.Substring (3, 30).Trim ();
                file.ID3Year = id3Tag.Substring (93, 4).Trim ();

                if (id3Tag [125] == 0)
                {
                    file.ID3TrackNumber = buffer [126];
                }
                else
                {
                    file.ID3TrackNumber = 0;
                }

                file.ID3Genre = buffer [127];
                file.Genre = (Genres)file.ID3Genre;
                file.HasID3Tag = true;
            }
        }

        // Получение минут
        private int GetMinutes (double time)
        {
            return _minutes = (int)(time / 60);
        }

        // Получение секунд
        private int GetSeconds (double time, int minute)
        {
            return _seconds = (int)(time - minute * 60);
        }

        // -----------------------Логика кнопок плеера------------------------
        // -------------------------------------------------------------------

        // Загрузка файла в плейлист
        private void BrowseFiles ()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog ();

            openFileDialog.Filter = "File (*.mp3)|*.mp3";
            openFileDialog.InitialDirectory = Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);

            if (openFileDialog.ShowDialog () == true)
            {
                _flag = true;

                this._fileNameAndPath = openFileDialog.FileName;
                this._fileName = Path.GetFileNameWithoutExtension (this._fileNameAndPath);

                for (int i = 0; i < _playlistMP3.Count; ++i)
                {
                    if (_playlistMP3 [i].Path == this._fileNameAndPath)
                    {
                        _flag = false;
                    }
                }

                if (_flag)
                {
                    _playlistMP3.Add (TrackCreate (this._fileNameAndPath, this._fileName));
                    _infoBoxMP3.Add (TrackCreate (this._fileNameAndPath, this._fileName));
                }
            }
        }

        // Удаление трека
        private void DeleteTrack ()
        {
            if (PlaylistSelectedIndex != -1)
            {
                for (int i = 0; i < PlaylistSelectedIndex; i++)
                {
                    var selectedItem = (MusicFile)PlaylistMP3 [i];

                    int index = GetTrackIndex (selectedItem);

                    _playlistMP3.RemoveAt (index);
                }

                MessageBox.Show ("Трек удалён.");
            }
            else
            {
                MessageBox.Show ("Выберите трек для удаления.");
            }
        }

        private int GetTrackIndex (MusicFile track)
        {
            for (int i = 0; i < _playlistMP3.Count; ++i)
            {
                if (track.Name == _playlistMP3 [i].Name)
                {
                    return i;
                }
            }

            return -1;
        }

        // Воспроизвести трек
        private void PlayMusic ()
        {
            if (PlaylistSelectedIndex != -1)
            {
                if (!_paused)
                {
                    mediaPlayer.URL = (PlaylistMP3 [PlaylistSelectedIndex] as MusicFile).Path;
                    _trackDuration = Math.Round ((PlaylistMP3 [PlaylistSelectedIndex] as MusicFile).Duration);
                    AddToListMP3 ();
                    TrackStatusSliderMaximum = _trackDuration;
                    TrackStatusSliderMinimum = 0;
                    TrackStatusSliderValue = 0;
                    mediaPlayer.controls.play ();

                    _timerOfDuration.Start ();
                }
                else
                {
                    mediaPlayer.controls.currentPosition = _positionInPlaylist;

                    if (mediaPlayer.controls.currentPosition != 0)
                    {
                        mediaPlayer.controls.play ();
                        _timerOfDuration.Start ();
                        _paused = !_paused;
                    }
                }

                if (_stopped)
                {
                    _stopped = false;
                }
            }
            else
            {
                MessageBox.Show ("Выберите трек для проигрывания!", "Выбор трека", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Приостановить проигрывание
        private void PauseMusic ()
        {
            if (!_paused)
            {
                mediaPlayer.controls.pause ();
                _positionInPlaylist = mediaPlayer.controls.currentPosition;
                if (_positionInPlaylist != 0)
                {
                    _paused = !_paused;
                }
            }
        }

        // Остановить проигрывание
        private void StopMusic ()
        {
            _paused = false;
            _stopped = true;
            _fileInfoMP3.Clear ();
            mediaPlayer.controls.stop ();

            TrackStatusLabelValue = "00:00";
        }

        // Предыдущий трек
        private void PlayBack ()
        {
            if (PlaylistMP3.Count != 0)
            {
                _stopped = true;
                mediaPlayer.controls.stop ();
                _stopped = false;

                var previndex = PlaylistSelectedIndex - 1;

                if (previndex < 0)
                {
                    previndex = PlaylistMP3.Count - 1;
                }

                PlaylistSelectedIndex = previndex;
                mediaPlayer.URL = (PlaylistMP3 [previndex] as MusicFile).Path;
                AddToListMP3 ();
                _trackDuration = (PlaylistMP3 [previndex] as MusicFile).Duration;
                TrackStatusSliderMaximum = _trackDuration;
                TrackStatusSliderMinimum = 0;
                TrackStatusSliderValue = 0;
                mediaPlayer.controls.play ();
                _timerOfDuration.Start ();
            }
            else
            {
                return;
            }
        }

        // Следующий трек
        private void PlayForward ()
        {
            if (PlaylistMP3.Count != 0)
            {
                _stopped = true;
                mediaPlayer.controls.stop ();
                _stopped = false;

                var nextrack = PlaylistSelectedIndex + 1;

                if (nextrack >= PlaylistMP3.Count)
                {
                    nextrack = 0;
                }

                PlaylistSelectedIndex = nextrack;
                mediaPlayer.URL = (PlaylistMP3 [nextrack] as MusicFile).Path;
                AddToListMP3 ();
                _trackDuration = (PlaylistMP3 [nextrack] as MusicFile).Duration;
                TrackStatusSliderMaximum = _trackDuration;
                TrackStatusSliderMinimum = 0;
                TrackStatusSliderValue = 0;
                mediaPlayer.controls.play ();
                _timerOfDuration.Start ();
            }
            else
            {
                return;
            }
        }
        #endregion

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