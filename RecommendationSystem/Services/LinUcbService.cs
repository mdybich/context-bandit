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

                //result.Add(new LinUcbResult(A, B, encodedRating.MovieName));
            }
        }

        public void LearnFromMovieLens(Dictionary<int, EncodedUser> encodedUsers, IEnumerable<MovieLensRating> movieLensRatings)
        {
            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;

            var ratingsWithEncodedUser = movieLensRatings.Select(movieLensRating => new EncodedMovieLensRating()
            {
                MovieId = movieLensRating.MovieId,
                Rating = movieLensRating.Rating,
                EncodedUser = encodedUsers[movieLensRating.UserId].EncodedAttributes
            }).ToList().GroupBy(r => r.MovieId);

            foreach (var ratingWithEncodedUser in ratingsWithEncodedUser)
            {
                var A = M.DenseIdentity(30);
                var B = V.Dense(30, 0.0);

                foreach (var rating in ratingWithEncodedUser)
                {
                    var teta = A.Inverse() * B;
                    var test = rating.EncodedUser.ToColumnMatrix();
                    var test2 = rating.EncodedUser.ToRowMatrix();

                    var result = test * test2;

                    A = A + result;

                    var ratio = rating.Rating > 3 ? 1 : 0;
                    B = B + ratio * rating.EncodedUser;
                }

                result.Add(new LinUcbResult(A, B, ratingWithEncodedUser.Key));
            }
        }

        public string RecommendMovie(Vector<double> userCode, IEnumerable<MovieLensMovie> movieLensMovies)
        {
            var recommendation = new List<Recommendation>();

            foreach (var movie in result)
            {
                var teta = movie.A.Inverse() * movie.B;
                var first = teta.ToColumnMatrix().TransposeThisAndMultiply(userCode);
                var second = userCode.ToRowMatrix() * movie.A.Inverse() * userCode.ToColumnMatrix();

                var score = Math.Sqrt(first.ToArray()[0] + second.ToArray()[0,0]);
                recommendation.Add(new Recommendation { MovieId = movie.MovieId, Result = score });
            }
            var topMovieId = recommendation.OrderByDescending(r => r.Result).First().MovieId;
            return movieLensMovies.Where(movie => movie.MovieId == topMovieId).First().Name;
        }
    }
}
