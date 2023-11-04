using GoingOutApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GoingOutApp.Services
{
    public class DataContext: DbContext
	{
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = GoingOut.db");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventComment> EventComments { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }


        public void CreateAccount(string username, string encodedPassword, string key, string name, string surname, int age, string gender)
        {
            using (DataContext context = new DataContext())
            {
                context.Users.Add(new User
                {
                    UserName = username,
                    Password = encodedPassword,
                    Key = key,
                    Name = name,
                    Surname = surname,
                    Age = age,
                    Gender = gender,

                });
                context.SaveChanges();
            }
        }

       
       
    }
}
