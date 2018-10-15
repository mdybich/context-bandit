using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RecommendationSystem.Models;

namespace RecommendationSystem.Services
{
    public class FileService
    {
        private readonly string separator = "::";

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

        public List<MovieLensUser> ReadUsers()
        {
            var users = new List<MovieLensUser>();

            using (var reader = new StreamReader(@"C:\data\users.dat"))
            {
                while (!reader.EndOfStream)
                {
                    var userAttributes = reader.ReadLine().Split(new string[] { separator }, StringSplitOptions.None);
                    users.Add(new MovieLensUser(userAttributes));
                }
            }

            return users;
        }

        public IEnumerable<MovieLensMovie> ReadMovies()
        {
            var movies = new List<MovieLensMovie>();

            using (var reader = new StreamReader(@"C:\data\movies.dat"))
            {
                while (!reader.EndOfStream)
                {
                    var userAttributes = reader.ReadLine().Split(new string[] { separator }, StringSplitOptions.None);
                    movies.Add(new MovieLensMovie(userAttributes));
                }
            }

            return movies;
        }

        public IEnumerable<MovieLensRating> ReadRatings()
        {
            var ratings = new List<MovieLensRating>();

            using (var reader = new StreamReader(@"C:\data\ratings.dat"))
            {
                while (!reader.EndOfStream)
                {
                    var userAttributes = reader.ReadLine().Split(new string[] { separator }, StringSplitOptions.None);
                    ratings.Add(new MovieLensRating(userAttributes));
                }
            }

            return ratings;
        }
    }
}
