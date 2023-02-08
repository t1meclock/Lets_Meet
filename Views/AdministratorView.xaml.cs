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
using Lets_Meet.ViewModels;

namespace Lets_Meet.Views
{
    public partial class AdministratorView: Window
    {
        NpgsqlConnection con = AppContext.GetConnection ();

        #region Списки для статистики
        List<string> hobbiesNames = new List<string> ();
        List<int> hobbiesIDs = new List<int> ();
        List<int> hobbiesStatistic = new List<int> ();
        List<int> hobbiesCount = new List<int> ();

        private string printInterests = String.Empty;
        #endregion

        public AdministratorView (int ID, int role)
        {
            AdministratorViewModel vm = new AdministratorViewModel (ID, role);
            this.DataContext = vm;

            InitializeComponent ();
            ReadAllHobbiesForStats ();

            if (vm.CloseAction == null)
                vm.CloseAction = new Action (this.Close);

            Closing += vm.OnWindowClosing;
        }

        private void ReadAllHobbiesForStats ()
        {
            con.Open ();

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

                con.Close ();

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
        }

        private void CreateDoc_Click (object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap (600, 300, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render (HobbyChartCC);

            string docPath = Path.Combine (@"C:\temp");
            if (!Directory.Exists (docPath))
                Directory.CreateDirectory (docPath);

            PngBitmapEncoder pngImage = new PngBitmapEncoder ();
            pngImage.Frames.Add (BitmapFrame.Create (renderTargetBitmap));

            string filePath = Path.Combine (docPath, string.Format ("diagram.png"));
            using (Stream stream = File.Create (filePath))
            {
                pngImage.Save (stream);
            }

            SaveDoc (printInterests, docPath);
        }

        private void SaveDoc (string printInterests, string docPath)
        {
            Document doc = new Document ();
            DocumentBuilder builder = new DocumentBuilder (doc);

            Font font = builder.Font;
            font.Size = 11;
            font.Bold = true;
            font.Color = System.Drawing.Color.Black;
            font.Name = "Arial";

            builder.Writeln (printInterests);
            builder.InsertImage (@"C:\temp\diagram.png");

            try
            {
                string filePathCrt = Path.Combine (docPath, string.Format ("StatOfInt.docx"));
                doc.Save (filePathCrt);

                MessageBox.Show ("Документ создан и сохранён по пути C:\\temp\\!");

                var filePathDel = Path.Combine (docPath, string.Format ("diagram.png"));
                if (File.Exists (filePathDel))
                    File.Delete (filePathDel);
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.ToString ());
            }
        }
    }
}