using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystem.Core.Models
{
    public class DataComputation
    {
        public string Id { get; set; }
        public double[] A { get; set; }
        public double[] B { get; set; }
        public double[] B_Better { get; set; }
        public Dictionary<int, UserValue> Users { get; set; }
    }
}
