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

namespace Lets_Meet.Views
{
    /// <summary>
    /// Логика взаимодействия для HobbyChart.xaml
    /// </summary>
    public partial class HobbyChart: UserControl
    {
        NpgsqlConnection con = AppContext.GetConnection ();

        #region Списки для статистики
        List<string> hobbiesNames = new List<string> ();
        List<int> hobbiesIDs = new List<int> ();
        List<int> hobbiesStatistic = new List<int> ();
        List<int> hobbiesCount = new List<int> ();

        private string printInterests = String.Empty;
        #endregion

        public HobbyChart ()
        {
            con.Open ();

            InitializeComponent ();
            ReadAllHobbiesForStats ();
        }

        #region Просмотр статистики
        #region [Реализация графика]
        class RecordCollection : ObservableCollection<Record>
        {
            public RecordCollection (List<Bar> barvalues)
            {
                Random rand = new Random ();
                BrushCollection brushcoll = new BrushCollection ();

                foreach (Bar barval in barvalues)
                {
                    int num = rand.Next (brushcoll.Count / 3);
                    Add (new Record (barval.Value, brushcoll [num], barval.BarName));
                }
            }
        }

        class BrushCollection : ObservableCollection<Brush>
        {
            public BrushCollection ()
            {
                Type _brush = typeof (Brushes);
                PropertyInfo [] props = _brush.GetProperties ();
                foreach (PropertyInfo prop in props)
                {
                    Brush _color = (Brush)prop.GetValue (null, null);
                    if (_color != Brushes.Black && _color != Brushes.Brown &&
                        _color != Brushes.Blue && _color != Brushes.Purple)
                        Add (_color);
                }
            }
        }

        class Bar
        {
            public string BarName { set; get; }

            public int Value { set; get; }
        }

        class Record : INotifyPropertyChanged
        {
            public Brush Color { set; get; }

            public string Name { set; get; }

            private int _data;
            public int Data
            {
                get { return _data; }
                set
                {
                    if (_data != value)
                        _data = value;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public Record (int value, Brush color, string name)
            {
                Data = value;
                Color = color;
                Name = name;
            }

            protected void PropertyOnChange (string propname)
            {
                if (PropertyChanged != null)
                    PropertyChanged (this, new PropertyChangedEventArgs (propname));
            }
        }
        #endregion

        private void ReadAllHobbiesForStats ()
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

                            hobbiesIDs.Add (hobbies.HobbyID);
                            hobbiesNames.Add (hobbies.HobbyName);
                        }
                    }
                }

                CountHobby ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void CountHobby ()
        {
            try
            {
                string query = "SELECT * FROM UsersHobbies";
                using (var cmd = new NpgsqlCommand (query, con))
                {
                    using (var rdr = cmd.ExecuteReader ())
                    {
                        while (rdr.Read ())
                        {
                            HobbiesStat hobbiesStat = new HobbiesStat ();

                            hobbiesStat.IdUserHobby = rdr.GetInt32 (0);
                            hobbiesStat.HobbyID = rdr.GetInt32 (1);
                            hobbiesStat.UserID = rdr.GetInt32 (2);

                            hobbiesStatistic.Add (hobbiesStat.HobbyID);
                        }
                    }
                }

                for (int i = 0; i < hobbiesIDs.Count; i++)
                {
                    int cnt = 0;

                    for (int j = 0; j < hobbiesStatistic.Count; j++)
                    {
                        if (hobbiesIDs [i] == hobbiesStatistic [j])
                        {
                            cnt++;
                        }
                    }

                    hobbiesCount.Add (cnt);
                }

                ShowHobbies ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }

        private void ShowHobbies ()
        {
            for (int i = 0; i < hobbiesNames.Count; i++)
            {
                printInterests += hobbiesNames [i] + ": " + hobbiesCount [i] + " пользователя;\n";
            }

            ReadStatistic ();
        }

        private void ReadStatistic ()
        {
            try
            {
                List<Bar> bar = new List<Bar> ();

                for (int i = 0; i < hobbiesNames.Count; i++)
                {
                    bar.Add (new Bar () { BarName = hobbiesNames [i], Value = hobbiesCount [i] });
                }

                this.DataContext = new RecordCollection (bar);
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
        #endregion
    }
}
