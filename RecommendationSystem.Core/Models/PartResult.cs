using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystem.Core.Models
{
    public class PartResult
    {
        public int MovieId { get; set; }
        public double Exploitation { get; set; }
        public double ExploitationTolerant { get; set; }
        public double Exploration { get; set; }
    }
}
