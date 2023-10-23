using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace GoingOutApp.Services
{
    public class DatabaseService : IDataBaseService
    {
        private string connectionString;

        public DatabaseService(string dbFilePath)
        {
            dbFilePath = $"Data Source ={dbFilePath};Version=3;";
        }

        // Database structure creation
        public void InitializeDatabase()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS Users (" +
                    "UserId INTEGER PRIMARY KEY, " +
                    "Username TEXT, " +
                    "Password TEXT, " +
                    "Name TEXT, " +
                    "Surname TEXT, " +
                    "Age TEXT, " +
                    "Gender TEXT, " +
                    "Country TEXT)",
                    connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS Events (" +
                    "EventId INTEGER PRIMARY KEY, " +
                    "EventCreatorId INTEGER, " +
                    "EventName TEXT, " +
                    "EventDescription TEXT, " +
                    "EventDateTime DATETIME, " +
                    "EventLocation TEXT, " +
                    "NumberOfPlaces INTEGER," +
                    "OtherInfo TEXT," +
                    "FOREIGN KEY (EventCreatorId) REFERENCES Users(UserId))",
                    connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS EventParticipants (" +
                    "EventParticipantId INTEGER PRIMARY KEY, " +
                    "EventId INTEGER, " +
                    "UserId INTEGER, " +
                    "ParticipantStatus TEXT, " +
                    "FOREIGN KEY (EventId) REFERENCES Events(EventId), " +
                    "FOREIGN KEY (UserId) REFERENCES Users(UserId))",
                    connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS EventComments (" +
                    "EventCommentId INTEGER PRIMARY KEY, " +
                    "EventId INTEGER, " +
                    "UserId INTEGER, " +
                    "CommentText TEXT, " +
                    "CommentDateTime DATETIME, " +
                    "FOREIGN KEY (EventId) REFERENCES Events(EventId), " +
                    "FOREIGN KEY (UserId) REFERENCES Users(UserId))",
                    connection))
                {
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = new SqliteCommand(
                    "CREATE TABLE IF NOT EXISTS EventPhotos (" +
                    "EventPhotoId INTEGER PRIMARY KEY, " +
                    "EventId INTEGER, " +
                    "PhotoPath TEXT, " +
                    "PhotoDescription TEXT, " +
                    "PhotoUploadDateTime DATETIME, " +
                    "FOREIGN KEY (EventId) REFERENCES Events(EventId))",
                    connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

    }


}