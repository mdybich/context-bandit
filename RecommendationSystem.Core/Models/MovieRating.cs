using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Core.Models
{
    public class MovieRating
    {
        public string MovieName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Education { get; set; }
        public int Rating { get; set; }

        public MovieRating
            (
            string movieName, 
            string age, 
            string sex, 
            string education, 
            string rating
            )
        {
            MovieName = movieName;
            Age = age;
            Sex = sex;
            Education = education;
            Rating = int.Parse(rating);
        }
    }
}
