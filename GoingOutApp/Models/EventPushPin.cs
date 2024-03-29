﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoingOutApp.Models
{
    public class EventPushPin
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("EventId")]
        public int EventId { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
    }
}