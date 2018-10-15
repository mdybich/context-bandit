using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Models
{
    public class EncodedMovieLensRating
    {
        public int MovieId { get; set; }
        public int Rating { get; set; }
        public Vector<double> EncodedUser { get; set; }
    }
}
