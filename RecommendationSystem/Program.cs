using MathNet.Numerics.LinearAlgebra;
using RecommendationSystem.Models;
using RecommendationSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecommendationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var V = Vector<double>.Build;

            var fileService = new FileService();
            var encodeService = new EncodeService();
            var linUcbService = new LinUcbService();
            var uiService = new UIService();

            var users = fileService.ReadUsers();
            var movies = fileService.ReadMovies();
            var ratings = fileService.ReadRatings();

            var learningCount = Convert.ToInt32(users.Count * 0.7);
            var testingCount = users.Count - learningCount;

            var t = users.Take(learningCount);
            var encodedUsersForLearn = encodeService.EncodeUser(users.Take(learningCount));
            var encodedUsersForTest = encodeService.EncodeUser(users.TakeLast(testingCount));

            var test = new Dictionary<int, EncodedUser>();
            var enc = new EncodedUser
            {
                UserId = 666,
                EncodedAttributes = V.DenseOfArray(new double[]
                {
                    1.0, 0,
                    0, 0, 0, 1.0, 0, 0, 0
                })
            };
            test.Add(666, enc);
            var test2 = new List<MovieLensRating>();
            test2.Add(new MovieLensRating(new string[] { "666", "123", "5"}));
            linUcbService.LearnFromMovieLens(test, test2);

            // linUcbService.LearnFromMovieLens(encodedUsersForLearn, ratings);
            linUcbService.Test(encodedUsersForTest, ratings);

            // uiService.DisplayApplication();
            var vector = V.DenseOfArray(new double[] {
                0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                1.0, 0.0,
                0.0, 0.0, 0.0, 0.0,
                0.0, 0.0, 0.0, 0.0,
                0.0, 0.0, 0.0, 0.0,
                1.0, 0.0, 0.0, 0.0,
                0.0, 0.0, 0.0, 0.0, 0.0
            });

            var recomenndation = linUcbService.RecommendMovie(vector, movies);

            linUcbService.UpdateResult(vector, recomenndation.MovieId, 4.0);
        }
    }
}
