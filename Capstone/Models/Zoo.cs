using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Zoo
    {
        [Key]
        public int ZooId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ? Zipcode { get; set; }
        public string Country { get; set; }
        public string WebsiteURL { get; set; }
        public string ImagePath { get; set; }
        public virtual ICollection<TripItem> TripItems { get; set; }
        public virtual ICollection<WishlistItem> Wishlists { get; set; }
        public virtual ICollection<Animal> Animals { get; set; }
    }
}
