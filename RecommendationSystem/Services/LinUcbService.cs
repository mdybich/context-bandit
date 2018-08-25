using MathNet.Numerics.LinearAlgebra;
using RecommendationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommendationSystem.Services
{
    public class LinUcbService
    {
        private List<LinUcbResult> result = new List<LinUcbResult>();

        public void Learn(List<EncodedRating> encodedRatings)
        {
            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;

            foreach (var encodedRating in encodedRatings)
            {
                var A = M.DenseIdentity(8);
                var B = V.Dense(8, 0.0);

                foreach (var userAttribute in encodedRating.UserAtttributesWithRating)
                {
                    var teta = A.Inverse() * B;
                    // var first = teta.ToColumnMatrix().TransposeThisAndMultiply(userAttribute.UserAttribute);
                    // var second =
                    var test = userAttribute.UserAttribute.ToColumnMatrix();
                    var test2 = userAttribute.UserAttribute.ToRowMatrix();

                    var result = test * test2;

                    A = A + result;

                    var rating = userAttribute.Rating > 5 ? 1 : 0;
                    B = B + rating * userAttribute.UserAttribute;
                }

                result.Add(new LinUcbResult(A, B, encodedRating.MovieName));
            }
        }

        public string RecommendMovie(Vector<double> userAttribute)
        {
            var recommendation = new List<Recommendation>();

            foreach (var movie in result)
            {
                var teta = movie.A.Inverse() * movie.B;
                var first = teta.ToColumnMatrix().TransposeThisAndMultiply(userAttribute);
                var second = userAttribute.ToRowMatrix() * movie.A.Inverse() * userAttribute.ToColumnMatrix();

                var score = Math.Sqrt(first.ToArray()[0] + second.ToArray()[0,0]);
                recommendation.Add(new Recommendation { MovieName = movie.MovieName, Result = score });
            }
            return recommendation.OrderByDescending(r => r.Result).First().MovieName;
        }
    }
}
