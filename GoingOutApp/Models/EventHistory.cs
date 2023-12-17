using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoingOutApp.Models
{
    public class EventHistory
    {
        [Key]
        public int EventHistoryId { get; set; }

        [ForeignKey("EventId")]
        public int EventId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string EventName { get; set; }
        public string EventDateTime { get; set; }
    }
}