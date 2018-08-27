using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Models
{
    public class MovieLensRating
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Rating { get; set; }

        public MovieLensRating(string[] ratingAttributes)
        {
            UserId = int.Parse(ratingAttributes[0]);
            MovieId = int.Parse(ratingAttributes[1]);
            Rating = int.Parse(ratingAttributes[2]);
        }
    }
}
