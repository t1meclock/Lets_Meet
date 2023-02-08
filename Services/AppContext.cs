using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Npgsql;
using System.Windows;
using Lets_Meet.Models;

namespace Lets_Meet
{
    //
    //Подключение к базе данных
    //
    class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<HobbiesStat> HobbiesStat { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public static NpgsqlConnection GetConnection ()
        {
            return new NpgsqlConnection (@"Server = localhost; Port = 5432; User Id = postgres; Password = 1234; Database = LetsMeet1;");
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql (@"Server = localhost; Port = 5432; User Id = postgres; Password = 1234; Database = LetsMeet1;");
        }
    }
}
