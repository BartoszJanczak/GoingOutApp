using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoingOutApp.Models
{
    public class EventParticipant
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("EventId")]
        public int EventId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public string ParticipantStatus { get; set; }
    }
}