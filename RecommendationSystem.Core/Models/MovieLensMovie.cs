using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Core.Models
{
    public class MovieLensMovie
    {
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }

        public MovieLensMovie(string[] movieAttributes)
        {
            MovieId = int.Parse(movieAttributes[0]);
            SetMovieDetails(movieAttributes[1]);
            Genre = movieAttributes[2];
            
        }

        private void SetMovieDetails(string movieDetails)
        {
            var fileNameEndIndex = movieDetails.LastIndexOf('(');
            var yearEndIndex = movieDetails.LastIndexOf(')');
            Name = movieDetails.Substring(0, fileNameEndIndex).Trim();
            var year = movieDetails.Substring(fileNameEndIndex + 1, yearEndIndex - fileNameEndIndex - 1);
            Year = int.Parse(year);
        }
    }
}
