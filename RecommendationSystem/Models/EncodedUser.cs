using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Models
{
    public class EncodedUser
    {
        public int UserId { get; set; }
        public Vector<double> EncodedAttributes { get; set; }
    }
}
