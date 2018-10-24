using MathNet.Numerics.LinearAlgebra;
using RecommendationSystem.Services;
using RecommendationSystem.Core.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using RecommendationSystem.Core.Models;

namespace RecommendationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var V = Vector<double>.Build;

            var fileService = new FileService();
            var dataService = new DataService();
            var encodeService = new EncodeService();
            var linUcbService = LinUcbService.Instance;

            var users = fileService.ReadUsers();
            var movies = fileService.ReadMovies();
            var ratings = fileService.ReadRatings();



            var divisions = new int[] { 70, 80, 90 };
            var ratios = new int[]
            {
                10, 20, 30, 40, 50, 60, 70, 80, 90, 100
            };

            foreach (var division in divisions)
            {
                var factor = (double)division / 100;
                var learningCount = Convert.ToInt32(users.Count * factor);
                var testingCount = users.Count - learningCount;

                var usersForLearn = users.Take(learningCount);
                var usersForTest = users.TakeLast(testingCount);
                var encodedUsersForLearn = encodeService.EncodeUser(usersForLearn);
                var encodedUsersForTest = encodeService.EncodeUser(usersForTest);

                // Prawdziwe wyniki!
                var parts = dataService.ReadSecond(division.ToString());
                var result = new Dictionary<int, List<ConfusionMatrix>>();

                foreach (var ratio in ratios)
                {
                    var partResult = linUcbService.Test2(parts, usersForTest, ratings, (double)ratio / 100);
                    result.Add(ratio, partResult);
                }

                dataService.WriteResult(result, division.ToString());

                // Build A B arrays (already done)
                //var result = linUcbService.LearnFromMovieLens(encodedUsersForLearn, ratings);
                //dataService.Write(result, division.ToString());

                // Build parts
                //var matrixes = dataService.Read(division.ToString());

                //var parts = linUcbService.GetParts(matrixes, usersForTest, V);
                //dataService.WriteSecond(parts, division.ToString());
            }

            Console.WriteLine("The end!");


            // linUcbService.Test(encodedUsersForTest, ratings);

            //var test = new Dictionary<int, EncodedUser>();
            //var enc = new EncodedUser
            //{
            //    UserId = 666,
            //    EncodedAttributes = V.DenseOfArray(new double[]
            //    {
            //        1.0, 0,
            //        0, 0, 0, 1.0, 0, 0, 0
            //    })
            //};
            //test.Add(666, enc);
            //var test2 = new List<MovieLensRating>();
            //test2.Add(new MovieLensRating(new string[] { "666", "123", "5"}));
            //linUcbService.LearnFromMovieLens(test, test2);
        }
    }
}
