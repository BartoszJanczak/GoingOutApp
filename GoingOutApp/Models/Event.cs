﻿using System;
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
        public string PhotoPath { get; set; }
        public string PhotoDescription { get; set; }
        public string EventDescription { get; set; }
        public string EventLocation { get; set; }
        public DateTime EventDateTime { get; set; }
        public int NumberOfplaces { get; set; }
        public string OtherInfo { get; set; }
    }
}