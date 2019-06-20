using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class GroupedZoos

    {
        public string State { get; set; }
        public int ZooCount { get; set; }
        public IEnumerable<Zoo> Zoos { get; set; }
    }
}
