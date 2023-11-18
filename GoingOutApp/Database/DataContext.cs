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

        public void CreateAccount(string username, string encodedPassword, string key, string name, string surname, int age, string gender, string securityQuestion, string securityAnswer)
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
                    SecurityQuestion = securityQuestion,
                    SecurityAnswer = securityAnswer,
                });
                context.SaveChanges();
            }
        }

        public void UpdatePassword(string username, string newPassword, string key, string securityQuestion, string serucrityAnswer)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    user.Password = newPassword;
                    user.Key = key;

                    context.SaveChanges();
                }
            }
        }

        public void AddEvent(string eventName, byte[] photoPath, string photoDescription, string eventDescription, string eventCity, string eventStreet, string eventBuildingNumber, DateTime eventDateTime, int numberOfplaces, string otherInfo)
        {
            using (DataContext context = new DataContext())
            {
                context.Events.Add(new Event
                {
                    EventName = eventName,
                    PhotoPath = photoPath,
                    PhotoDescription = photoDescription,
                    EventDescription = eventDescription,
                    Street = eventStreet,
                    City = eventCity,
                    NumberOfBuilding = eventBuildingNumber,
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