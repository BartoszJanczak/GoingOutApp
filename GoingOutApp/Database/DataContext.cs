using GoingOutApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using static GoingOutApp.AddTaskwindow;

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
        public DbSet<Like> Likes { get; set; }
        public DbSet<EventComment> EventComments { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }

        public DbSet<EventPushPin> EventPushPins { get; set; }
        public DbSet<EventHistory> EventHistory { get; set; }

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

        public void CreateAccount(string username, string encodedPassword, string key, string name, string surname, int age, string gender, string securityQuestion, string securityAnswer, byte[] photoPath)
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
                    PhotoPath = photoPath,
                    IsBanned = false,
                });
                context.SaveChanges();
            }
        }

        public void UpdateEvent(Event eventToUpdate)
        {
            using (DataContext context = new DataContext())
            {
                var ev = context.Events.FirstOrDefault(e => e.EventId == eventToUpdate.EventId);
                if (ev != null)
                {
                    ev.NumberOfplaces = eventToUpdate.NumberOfplaces;
                    ev.EventName = eventToUpdate.EventName;
                    ev.EventCategory = eventToUpdate.EventCategory;
                    ev.EventDateTime = eventToUpdate.EventDateTime;
                    ev.EventDescription = eventToUpdate.EventDescription;
                    ev.PhotoDescription = eventToUpdate.PhotoDescription;
                    ev.PhotoPath = eventToUpdate.PhotoPath;

                    context.SaveChanges();
                }
            }
        }

        public void UpdateUserPhotoPath(int userId, byte[] newPhotoPath)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    user.PhotoPath = newPhotoPath;
                    context.SaveChanges();
                }
            }
        }

        public void DeletePin(int eventId)
        {
            using (DataContext context = new DataContext())
            {
                var deletedEventPin = context.EventPushPins.FirstOrDefault(e => e.EventId == eventId);
                if (deletedEventPin != null)
                {
                    // Remove event
                    context.EventPushPins.Remove(deletedEventPin);
                    context.SaveChanges();
                }
            }
        }

        public void DeleteEvent(int eventId)
        {
            using (DataContext context = new DataContext())
            {
                var selectedEvent = context.Events.FirstOrDefault(e => e.EventId == eventId);
                if (selectedEvent != null)
                {
                    // Remove event
                    context.Events.Remove(selectedEvent);
                    context.SaveChanges();
                }
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

        public void AddEvent(int eventCreatorId, string eventName, byte[] photoPath, string photoDescription, string eventDescription, string eventCity, string eventStreet, string eventBuildingNumber, string eventDateTime, string eventHour, int numberOfplaces, string otherInfo, string eventCategory)
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
                    EventHour = eventHour,
                    NumberOfplaces = numberOfplaces,
                    OtherInfo = otherInfo,
                    TakenPlaces = 0,
                    EventCategory = eventCategory,
                    EventCreatorId = eventCreatorId
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

        public List<string> GetParticipants(int eventId)
        {
            using (DataContext context = new DataContext())
            {
                // Pobierz nazwy użytkowników z modelu User, którzy są uczestnikami wydarzenia o danym identyfikatorze (eventId)
                var participants = context.EventParticipants
                    .Where(ep => ep.EventId == eventId)
                    .Join(context.Users, ep => ep.UserId, u => u.UserId, (ep, u) => u.UserName)
                    .ToList();

                return participants;
            }
        }

        public List<User> GetUsers()
        {
            using (DataContext context = new DataContext())
            {
                List<User> users = context.Users.Where(u => u.UserName != "admin").ToList();
                return users;
            }
        }

        public byte[] GetPhoto(int eventId)
        {
            using (DataContext context = new DataContext())
            {
                var photoBytes = context.Events.Where(ep => ep.EventId == eventId).Select(x => x.PhotoPath).FirstOrDefault();
                return photoBytes;
            }
        }

        public void BanUser(User user)
        {
            var existingUser = Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser != null)
            {
                existingUser.IsBanned = true;
                SaveChanges();
            }
        }

        public int GetNumberOfLikes(int eventId)
        {
            using (DataContext context = new DataContext())
            {
                int numberOfLikes = context.Likes.Where(l => l.EventId == eventId).Count();
                return numberOfLikes;
            }
        }

        public bool doesUserLikeEvent(int userId, int eventId)
        {
            using (DataContext context = new DataContext())
            {
                bool exists = context.Likes.Where(l => l.EventId == eventId && userId == l.UserId).Any();
                return exists;
            }
        }

        public void AddLike(int userId, int eventId)
        {
            using (DataContext context = new DataContext())
            {
                context.Likes.Add(new Like
                {
                    EventId = eventId,
                    UserId = userId
                });
                context.SaveChanges();
            }
        }

        public void DeleteLike(int userId, int eventId)
        {
            using (DataContext context = new DataContext())
            {
                var like = context.Likes.Where(u => u.UserId == userId && u.EventId == eventId).First();
                if(like != null)
                {
                context.Likes.Remove(like);

                }
                context.SaveChanges();
            }
        }
    }
}