using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RecommendationSystem.Models;

namespace RecommendationSystem.Services
{
    public class FileService
    {
        public IEnumerable<MovieRating> ReadFile()
        {
            var movieRatings = new List<MovieRating>();

            using (var reader = new StreamReader(@"C:\data\Ratings.csv"))
            {
                var headerLine = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(';');
                    if (values.Length != 5)
                    {
                        throw new Exception();
                    }

                    var movieRating = new MovieRating(
                        values[0],
                        values[1],
                        values[2],
                        values[3],
                        values[4]
                        );

                    movieRatings.Add(movieRating);
                }
            }

            return movieRatings;
        }
    }
}
