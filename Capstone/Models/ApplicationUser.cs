using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Capstone.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
        public virtual ICollection<WishlistItem> Wishlist { get; set; }
        public virtual ICollection<Animal> Animals { get; set; }
    }
}