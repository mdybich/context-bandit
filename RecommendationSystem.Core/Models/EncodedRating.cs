using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using System.Text;

namespace RecommendationSystem.Models
{
    public class EncodedRating
    {
        public string MovieName { get; set; }
        public List<UserAtttributeWithRating> UserAtttributesWithRating { get; set; }

        public EncodedRating(string movieName, UserAtttributeWithRating userAtttributeWithRating)
        {
            MovieName = movieName;
            UserAtttributesWithRating = new List<UserAtttributeWithRating>() { userAtttributeWithRating };
        }
    }

    public class UserAtttributeWithRating
    {
        public Vector<double> UserAttribute { get; set; }
        public int Rating { get; set; }

        public UserAtttributeWithRating(Vector<double> userAttribute, int rating)
        {
            UserAttribute = userAttribute;
            Rating = rating;
        }
    }
}
