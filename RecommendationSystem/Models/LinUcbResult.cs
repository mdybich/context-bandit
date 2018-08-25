using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Models
{
    public class LinUcbResult
    {
        public Matrix<double> A { get; set; }
        public Vector<double> B { get; set; }
        public string MovieName { get; set; }

        public LinUcbResult(Matrix<double> a, Vector<double> b, string movieName)
        {
            A = a;
            B = b;
            MovieName = movieName;
        }
    }
}
