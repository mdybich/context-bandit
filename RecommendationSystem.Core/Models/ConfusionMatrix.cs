using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystem.Core.Models
{
    public class ConfusionMatrix
    {
        public double Sensitivity { get; set; }
        public double Specificity { get; set; }
        public double Precision { get; set; }
        public double Accuracy { get; set; }
        public string Label { get; set; }
    }
}
