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

        public IEnumerable<LearnResult> LearnFromMovieLens(Dictionary<int, EncodedUser> encodedUsers, IEnumerable<MovieLensRating> movieLensRatings)
        {
            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;
            var learnResult = new List<LearnResult>();

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
                var B_Better = V.Dense(30, 0.0);

                foreach (var rating in ratingWithEncodedUser)
                {
                    var test = rating.EncodedUser.ToColumnMatrix();
                    var test2 = rating.EncodedUser.ToRowMatrix();

                    var result = test * test2;

                    A = A + result;

                    var ratio = rating.Rating > 3 ? 1 : 0;
                    var betterRadio = rating.Rating > 2 ? 1 : 0;
                    B = B + ratio * rating.EncodedUser;
                    B_Better = B_Better + betterRadio * rating.EncodedUser;
                }

                result.Add(new LinUcbResult(A, B, ratingWithEncodedUser.Key));
                learnResult.Add(new LearnResult
                {
                    MovieId = ratingWithEncodedUser.Key,
                    A = A.ToArray(),
                    B = B.ToArray(),
                    B_Tolerant = B_Better.ToArray()
                });
            }

            return learnResult;
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

        public Dictionary<int, List<PartResult>> GetParts(IEnumerable<LinUcbResult2> learnResult, IEnumerable<MovieLensUser> users, VectorBuilder<double> vectorBuilder)
        {
            var encodeService = new EncodeService();
            var dic = new Dictionary<int, List<PartResult>>();
            foreach (var user in users)
            {
                var userCode = encodeService.GetUserVector(user, vectorBuilder);
                var parts = new List<PartResult>();

                foreach (var movie in learnResult)
                {
                    var inversedA = movie.A.Inverse();
                    var teta = inversedA * movie.B;
                    var tetaTolerant = inversedA * movie.B_Tolerant;

                    var exploitation = teta.ToColumnMatrix().TransposeThisAndMultiply(userCode).ToArray()[0];
                    var expltationTolerant = tetaTolerant.ToColumnMatrix().TransposeThisAndMultiply(userCode).ToArray()[0];
                    var exploration = Math.Sqrt((userCode.ToRowMatrix() * movie.A.Inverse() * userCode.ToColumnMatrix()).ToArray()[0, 0]);

                    parts.Add(new PartResult
                    {
                        MovieId = movie.MovieId,
                        Exploration = exploration,
                        Exploitation = exploitation,
                        ExploitationTolerant = expltationTolerant
                    });
                }

                dic.Add(user.UserId, parts);
            }

            return dic;
        }

        public List<ConfusionMatrix> Test2(
            Dictionary<int, List<PartResult>> parts, 
            IEnumerable<MovieLensUser> users, 
            IEnumerable<MovieLensRating> movieLensRatings, 
            double ratio = 1
            )
        {
            var truePositive = 0;
            var trueNegative = 0;
            var falsePositive = 0;
            var falseNegative = 0;

            var truePositiveTolerant = 0;
            var trueNegativeTolerant = 0;
            var falsePositiveTolerant = 0;
            var falseNegativeTolerant = 0;

            foreach (var user in users)
            {
                var allRecommendations = new Dictionary<int, double>();
                var allRecommendationsTolerant = new Dictionary<int, double>();

                var movieParts = parts[user.UserId];
                foreach (var moviePart in movieParts)
                {
                    var exploration = ratio * moviePart.Exploration;
                    var score = moviePart.Exploitation + exploration;
                    var scoreTolerant = moviePart.ExploitationTolerant + exploration;

                    allRecommendations.Add(moviePart.MovieId, score);
                    allRecommendationsTolerant.Add(moviePart.MovieId, scoreTolerant);
                }

                var average = allRecommendations.Average(r => r.Value);
                var recommendations = allRecommendations.Where(r => r.Value > average).ToDictionary(r => r.Key, r => r.Value);

                var averateTolerant = allRecommendationsTolerant.Average(r => r.Value);
                var recommendationsTolerant = allRecommendationsTolerant.Where(r => r.Value > averateTolerant).ToDictionary(r => r.Key, r => r.Value);

                // Wszystkie oceny użytkownika
                var userRatings = movieLensRatings.Where(rating => rating.UserId == user.UserId);

                foreach (var userRating in userRatings)
                {
                    var isRecommended = recommendations.ContainsKey(userRating.MovieId);
                    var hasPositiveRating = userRating.Rating > 3;

                    if (isRecommended && hasPositiveRating)
                    {
                        truePositive++;
                    }

                    if (!isRecommended && !hasPositiveRating)
                    {
                        trueNegative++;
                    }

                    if (isRecommended && !hasPositiveRating)
                    {
                        falsePositive++;
                    }

                    if (!isRecommended && hasPositiveRating)
                    {
                        falseNegative++;
                    }

                    var isRecommendedWithTolerant = recommendationsTolerant.ContainsKey(userRating.MovieId);
                    var hasPositiveTolerantRating = userRating.Rating > 2;

                    if (isRecommendedWithTolerant && hasPositiveTolerantRating)
                    {
                        truePositiveTolerant++;
                    }

                    if (!isRecommendedWithTolerant && !hasPositiveTolerantRating)
                    {
                        trueNegativeTolerant++;
                    }

                    if (isRecommendedWithTolerant && !hasPositiveTolerantRating)
                    {
                        falsePositiveTolerant++;
                    }

                    if (!isRecommendedWithTolerant && hasPositiveTolerantRating)
                    {
                        falseNegativeTolerant++;
                    }
                }
            }

            var confusionMatrix = GetConfusionMatrix(
                truePositive,
                trueNegative,
                falsePositive,
                falseNegative,
                "Standard"
            );

            var confusionMatrixTolerant = GetConfusionMatrix(
                truePositiveTolerant,
                trueNegativeTolerant,
                falsePositiveTolerant,
                falseNegativeTolerant,
                "Tolerant"
            );

            return new List<ConfusionMatrix>
            {
                confusionMatrix,
                confusionMatrixTolerant
            };
        }

        private ConfusionMatrix GetConfusionMatrix(int truePositive, int trueNegative, int falsePositive, int falseNegative, string label)
        {
            var sensitivity = (double)truePositive / (truePositive + falseNegative);
            var specificity = (double)trueNegative / (trueNegative + falsePositive);
            var precision = (double)truePositive / (truePositive + falsePositive);
            var accuracy = (double)(truePositive + trueNegative) / (truePositive + trueNegative + falsePositive + falseNegative);

            return new ConfusionMatrix
            {
                Sensitivity = sensitivity,
                Specificity = specificity,
                Precision = precision,
                Accuracy = accuracy,
                Label = label
            };
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
