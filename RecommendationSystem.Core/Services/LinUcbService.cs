using MathNet.Numerics.LinearAlgebra;
using RecommendationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommendationSystem.Core.Services
{
    public class LinUcbService
    {
        private List<LinUcbResult> result = new List<LinUcbResult>();
        private static LinUcbService instance;

        public static LinUcbService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LinUcbService();
                }

                return instance;
            }
        }

        private LinUcbService() {}

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

            var ratingsWithEncodedUser = movieLensRatings
                .Where(movieLensRating => encodedUsers.Any(user => user.Key == movieLensRating.UserId))
                .Select(movieLensRating => new EncodedMovieLensRating()
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

        public IEnumerable<MovieLensMovie> RecommendMovies(Vector<double> userCode, IEnumerable<MovieLensMovie> movieLensMovies)
        {
            var recommendation = new List<Recommendation>();

            foreach (var movie in result)
            {
                var teta = movie.A.Inverse() * movie.B;
                var first = teta.ToColumnMatrix().TransposeThisAndMultiply(userCode);
                var second = userCode.ToRowMatrix() * movie.A.Inverse() * userCode.ToColumnMatrix();

                var score = first.ToArray()[0] + 0.1 * Math.Sqrt(second.ToArray()[0,0]);
                recommendation.Add(new Recommendation { MovieId = movie.MovieId, Result = score });
            }

            var topMovieIds = recommendation.OrderByDescending(r => r.Result).Take(20).Select(r => r.MovieId);
            var moviesToRecommend = movieLensMovies.Where(movie => topMovieIds.Contains(movie.MovieId));

            return moviesToRecommend;
        }

        public IEnumerable<Recommendation> RecommendMovies(Vector<double> userCode)
        {
            var recommendation = new List<Recommendation>();

            foreach (var movie in result)
            {
                var teta = movie.A.Inverse() * movie.B;
                var first = teta.ToColumnMatrix().TransposeThisAndMultiply(userCode);
                var second = userCode.ToRowMatrix() * movie.A.Inverse() * userCode.ToColumnMatrix();

                var score = first.ToArray()[0] + Math.Sqrt(second.ToArray()[0, 0]);
                recommendation.Add(new Recommendation { MovieId = movie.MovieId, Result = score });
            }
            return recommendation.OrderByDescending(r => r.Result).Where(r => r.Result > 0.8);
        }

        public void Test(Dictionary<int, EncodedUser> encodedUsers, IEnumerable<MovieLensRating> movieLensRatings)
        {
            var list = new List<double>();
            foreach (var user in encodedUsers)
            {
                var recommendations = RecommendMovies(user.Value.EncodedAttributes);

                var realRating = movieLensRatings.Where(rating =>
                    rating.UserId == user.Key &&
                    recommendations.Any(r => r.MovieId == rating.MovieId)
                );
                var realRatingCount = realRating.Count();

                var goodRecommendation = realRating.Where(r => r.Rating > 3).Count();
                if (realRatingCount > 0)
                {
                    var v = (double)goodRecommendation / realRatingCount;
                    list.Add(v);
                }
                
            }

            var t = list.Sum() / list.Count();
        }

        public void UpdateResult(Vector<double> userCode, int movieId, double rating)
        {
            var t = result.Find(r => r.MovieId == movieId);
            var test = userCode.ToColumnMatrix();
            var test2 = userCode.ToRowMatrix();

            var result1 = test * test2;

            t.A = t.A + result1;

            var ratio = rating > 3 ? 1 : 0;
            t.B = t.B + ratio * userCode;
        }
    }
}
