using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class WishlistItem
    {
        [Key]
        public int WishlistItemId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public Zoo Zoo { get; set; }
        public int ZooId { get; set; }
    }
}
