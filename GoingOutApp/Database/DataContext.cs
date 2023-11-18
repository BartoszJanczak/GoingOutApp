using GoingOutApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoingOutApp.Services
{
    public class DataContext : DbContext
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

        public void AddEvent(string eventName, byte[] photoPath, string photoDescription, string eventDescription, string eventCity, string eventStreet, string evenCountry, DateTime eventDateTime, int numberOfplaces, string otherInfo)
        {
            using (DataContext context = new DataContext())
            {
                context.Events.Add(new Event
                {
                    EventName = eventName,
                    Photo = photoPath,
                    PhotoDescription = photoDescription,
                    EventDescription = eventDescription,
                    Street = eventStreet,
                    City = eventCity,
                    EventDateTime = eventDateTime,
                    NumberOfplaces = numberOfplaces,
                    OtherInfo = otherInfo
                });
                context.SaveChanges();
            }
        }

        public List<Event> GetEvents()
        {
            using (DataContext context = new DataContext())
            {
                // Pobierz wszystkie wydarzenia z bazy danych
                List<Event> events = context.Events.ToList();

                return events;
            }
        }
    }
}