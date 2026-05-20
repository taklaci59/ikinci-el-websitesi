using System.ComponentModel.DataAnnotations;

namespace ikincelwebsitesi.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
