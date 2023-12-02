using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoingOutApp.Models
{
    public class Event
    {
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