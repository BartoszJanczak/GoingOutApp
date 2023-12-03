using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoingOutApp.Models
{
    public class Event
    {
        public Event()
        {
                
        }
        public Event(int eventCreatorId, string eventName, byte[] photoPath, string photoDescription, string eventDescription, string eventCity, string eventStreet, string eventBuildingNumber, string eventDate, int numberOfPlaces, string otherInfo, string eventCategory)
        {
            this.EventCreatorId = eventCreatorId;
            this.EventName = eventName;
            this.PhotoPath = photoPath;
            this.PhotoDescription = photoDescription;
            this.EventDescription = eventDescription;
            this.City = eventCity;
            this.Street = eventStreet;
            this.NumberOfBuilding = eventBuildingNumber;
            this.EventDateTime = eventDate;
            this.NumberOfplaces = numberOfPlaces;
            this.OtherInfo = otherInfo;
            this.EventCategory = eventCategory;
        }

        [Key]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public int EventCreatorId { get; set; }

        public string EventName { get; set; }
        public byte[] PhotoPath { get; set; }
        public string PhotoDescription { get; set; }
        public string EventDescription { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string NumberOfBuilding { get; set; }
        public string EventDateTime { get; set; }
        public int NumberOfplaces { get; set; }
        public string OtherInfo { get; set; }
        public int TakenPlaces { get; set; }
        public string EventCategory { get; set; }
    }
}