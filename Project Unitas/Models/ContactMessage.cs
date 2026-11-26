using System.ComponentModel.DataAnnotations;

namespace Project_Unitas.Models
{
    /// <summary>A message sent with the contact form (table "ContactMessages").</summary>
    public class ContactMessage
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string? Company { get; set; }

        [MaxLength(254)]
        public string Email { get; set; } = "";

        [MaxLength(50)]
        public string Subject { get; set; } = "";

        [MaxLength(2000)]
        public string Message { get; set; } = "";

        /// <summary>When the message was sent (UTC).</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>Set by customer service once the message has been answered.</summary>
        public bool Handled { get; set; }
    }
}
