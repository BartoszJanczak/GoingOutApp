using GoingOutApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;

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

        public DbSet<EventPushPin> EventPushPins { get; set; }

        public void CreatePushPin(int eventId, double locationX, double locationY)
        {
            using (DataContext context = new DataContext())
            {
                context.EventPushPins.Add(new EventPushPin
                {
                    EventId = eventId,
                    X = locationX,
                    Y = locationY
                });
                context.SaveChanges();
            }
        }

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

        public void AddEvent(string eventName, byte[] photoPath, string photoDescription, string eventDescription, string eventCity, string eventStreet, string eventBuildingNumber, string eventDateTime, int numberOfplaces, string otherInfo)
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
                    OtherInfo = otherInfo,
                    TakenPlaces = 0
                });
                context.SaveChanges();
            }
        }

        public void SignUpForEvent(DataContext context, int eventId, int userId)
        {
            var existingParticipant = context.EventParticipants
                .FirstOrDefault(ep => ep.EventId == eventId && ep.UserId == userId);

            if (existingParticipant == null)
            {
                context.EventParticipants.Add(new EventParticipant
                {
                    EventId = eventId,
                    UserId = userId,
                    ParticipantStatus = "1"
                });

                UpdateTakenPlaces(eventId);
                context.SaveChanges();
            }
        }

        public void CancelParticipation(DataContext context, int eventId, int userId)
        {
            var existingParticipant = context.EventParticipants.FirstOrDefault(ep => ep.EventId == eventId && ep.UserId == userId);

            if (existingParticipant != null)
            {
                context.EventParticipants.Remove(existingParticipant);

                UpdateTakenPlaces(eventId);
                context.SaveChanges();
            }
        }

        public List<Event> GetParticipatedEvents(int userId)
        {
            using (DataContext dataContext = new DataContext())
            {
                var eventIds = dataContext.EventParticipants
                    .Where(ep => ep.UserId == userId)
                    .Select(ep => ep.EventId)
                    .ToList();

                var participatedEvents = dataContext.Events
                    .Where(e => eventIds.Contains(e.EventId))
                    .ToList();

                return participatedEvents;
            }
        }

        public void UpdateTakenPlaces(int eventId)
        {
            using (DataContext context = new DataContext())
            {
                var selectedEvent = context.Events.FirstOrDefault(e => e.EventId == eventId);
                if (selectedEvent != null)
                {
                    selectedEvent.TakenPlaces = context.EventParticipants.Count(ep => ep.EventId == eventId);
                    context.SaveChanges();
                }
            }
        }

        //public Event GetEventById(int id)
        //{
        //    using (DataContext context = new DataContext())
        //    {
        //        // Pobierz wszystkie wydarzenia z bazy danych

        //        private event = context.Events.Where(e => e.EventId = id);

        //        return events;
        //    }
        //}

        public Event GetEvent(int id)
        {
            using (DataContext context = new DataContext())
            {
                UpdateTakenPlaces(id);

                Event returnEvent = context.Events.ToList().Where(e => e.EventId == id).First();

                return returnEvent;
            }
        }

        public static User GetLoggedInUser(string username, string password)
        {
            using (DataContext context = new DataContext())
            {
                // Sprawdź, czy użytkownik istnieje w bazie danych
                User loggedInUser = context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);

                return loggedInUser;
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

        public List<EventPushPin> GetEventPushPins()
        {
            using (DataContext context = new DataContext())
            {
                // Pobierz wszystkie wydarzenia z bazy danych
                List<EventPushPin> events = context.EventPushPins.ToList();

                return events;
            }
        }
    }
}