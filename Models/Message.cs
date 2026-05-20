using System.ComponentModel.DataAnnotations;

namespace ikincelwebsitesi.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        // Foreign Keys
        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser? Sender { get; set; }

        public string ReceiverId { get; set; } = string.Empty;
        public ApplicationUser? Receiver { get; set; }

        public int ListingId { get; set; }
        public Listing? Listing { get; set; }
    }
}
