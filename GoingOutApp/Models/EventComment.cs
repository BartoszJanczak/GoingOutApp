using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoingOutApp.Models
{
    public class EventComment
    {
        [Key]
        public int EventCommentId { get; set; }
        [ForeignKey("EventId")]
        public int EventId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentCreationDate { get; set; }
    }
}