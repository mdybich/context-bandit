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
        public int MovieId { get; set; }

        public LinUcbResult(Matrix<double> a, Vector<double> b, int movieId)
        {
            A = a;
            B = b;
            MovieId = movieId;
        }
    }
}
