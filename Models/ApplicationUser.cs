using Microsoft.AspNetCore.Identity;

namespace ikincelwebsitesi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        
        // Navigation Properties
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
