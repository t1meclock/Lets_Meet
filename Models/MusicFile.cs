using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows;

namespace Lets_Meet.Models
{
    // Перечисление жанров
    public enum Genres: byte
    {
        Blues,
        ClassicRock,
        Country,
        Dance,
        Disco,
        Funk,
        Grunge,
        HipHop,
        Jazz,
        Metal,
        NewAge,
        Oldies,
        Other,
        Pop,
        RnB,
        Rap,
        Reggae,
        Rock,
        Techno,
        Industrial,
        Alternative,
        Ska,
        DeathMetal,
        Pranks,
        Soundtrack,
        EuroTechno,
        Ambient,
        TripHop,
        Vocal,
        JazzFunk,
        Fusion,
        Trance,
        Classical,
        Instrumental,
        Acid,
        House,
        Game,
        SoundClip,
        Gospel,
        Noise,
        AlternRock,
        Bass,
        Soul,
        Punk,
        Space,
        Mediative,
        InstrumentalPop,
        InstrumentalRock,
        Ethnic,
        Gothic,
        Darkwave,
        TechnoIndustrial,
        Electronic,
        PopFolk,
        Eurodance,
        Dream,
        SouthernRock,
        Comedy,
        Cult,
        Gangsta,
        Top40,
        ChristianRap,
        PopFunk,
        Jungle,
        NativeAmerican,
        Cabaret,
        NewWave,
        Psychadelic,
        Rave,
        Showtunes,
        Trailer,
        LoFi,
        Tribal,
        AcidPunk,
        AcidJazz,
        Polka,
        Retro,
        Musical,
        RocknRoll,
        HardRock,
        None = 255
    };

    [Serializable]

    public class MusicFile
    {
        private Genres genre;

        private bool hasID3Tag;

        private string name, path, id3Year, mptime, id3Album, id3Artist, id3Comment, id3Title;

        private byte id3Genre, id3TrackNumber;
        private double duration;

        public MusicFile ()
        {
            this.genre = Genres.None;

            this.hasID3Tag = false;

            this.id3Album = String.Empty;
            this.name = String.Empty;
            this.path = String.Empty;
            this.id3Comment = String.Empty;
            this.id3Title = String.Empty;
            this.id3Year = String.Empty;
            this.id3Artist = String.Empty;
            this.mptime = String.Empty;

            this.id3Genre = 0;
            this.id3TrackNumber = 0;
            this.duration = 0;
        }

        //Свойства для сбора метаданных трека
        #region Свойства
        public Genres Genre
        {
            get { return this.genre; }
            set { this.genre = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public double Duration
        {
            get { return this.duration; }
            set { this.duration = value; }
        }

        public string MPtime
        {
            get { return this.mptime; }
            set { this.mptime = value; }
        }

        public bool HasID3Tag
        {
            get { return this.hasID3Tag; }
            set { this.hasID3Tag = value; }
        }

        public string ID3Album
        {
            get { return this.id3Album; }
            set { this.id3Album = value; }
        }

        public string ID3Commnet
        {
            get { return this.id3Comment; }
            set { this.id3Comment = value; }
        }

        public string ID3Title
        {
            get { return this.id3Title; }
            set { this.id3Title = value; }
        }

        public string ID3Artist
        {
            get { return this.id3Artist; }
            set { this.id3Artist = value; }
        }

        public string ID3Year
        {
            get { return this.id3Year; }
            set { this.id3Year = value; }
        }

        public byte ID3Genre
        {
            get { return this.id3Genre; }
            set { this.id3Genre = value; }
        }

        public byte ID3TrackNumber
        {
            get { return this.id3TrackNumber; }
            set { this.id3TrackNumber = value; }
        }
        #endregion

        public static void SerializeObject (ObservableCollection<MusicFile> mp3col)
        {
            BinaryFormatter binary = new BinaryFormatter ();

            using (FileStream fstream = new FileStream ("listmp3.dat", FileMode.OpenOrCreate, FileAccess.Write))
            {
                binary.Serialize (fstream, mp3col);
            }
        }

        public static ObservableCollection<MusicFile> DeserializeObject ()
        {
            BinaryFormatter binary = new BinaryFormatter ();

            ObservableCollection<MusicFile> mp3col = new ObservableCollection<MusicFile> ();

            using (FileStream fstream = new FileStream ("listmp3.dat", FileMode.Open, FileAccess.Read))
            {
                mp3col = (ObservableCollection<MusicFile>)binary.Deserialize (fstream);
            }

            return mp3col;
        }
    }
}
