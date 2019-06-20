using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class TripItem
    {
        [Key]
        public int TripItemId { get; set; }
        public Zoo Zoo { get; set; }
        public int ZooId { get; set; }
        public Trip Trip { get; set; }
        public int TripId { get; set; }
    }
}
