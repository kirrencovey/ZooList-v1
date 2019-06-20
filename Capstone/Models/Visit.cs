using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Visit
    {
        [Key]
        public int VisitId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string Comments { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public Zoo Zoo { get; set; }
        public int ZooId { get; set; }
    }
}
