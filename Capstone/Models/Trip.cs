using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Trip
    {
        [Key]
        public int TripId { get; set; }
        [Required]
        public string Name { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<TripItem> TripItems { get; set; }
    }
}
