using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystem.Core.Models
{
    public class LearnResult
    {
        public int MovieId { get; set; }
        public double[,] A { get; set; }
        public double[] B { get; set; }
        public double[] B_Tolerant { get; set; }
    }
}
