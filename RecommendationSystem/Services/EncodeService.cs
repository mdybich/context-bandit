using RecommendationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

using UserAttributeCode = System.Tuple<string, int[]>;

namespace RecommendationSystem.Services
{
    public class EncodeService
    {
        private readonly List<UserAttributeCode> ageCodes = new List<UserAttributeCode>
        {
            new UserAttributeCode("Young", new int[] { 1, 0, 0 }),
            new UserAttributeCode("Middle", new int[] { 0, 1, 0 }),
            new UserAttributeCode("Old", new int[] { 0, 0, 1 })
        };

        private readonly List<UserAttributeCode> sexCodes = new List<UserAttributeCode>
        {
            new UserAttributeCode("M", new int[] { 1, 0}),
            new UserAttributeCode("F", new int[] { 0, 1})
        };

        private readonly List<UserAttributeCode> educationCodes = new List<UserAttributeCode>
        {
            new UserAttributeCode("Primary", new int[] { 1, 0, 0 }),
            new UserAttributeCode("Secondary", new int[] { 0, 1, 0 }),
            new UserAttributeCode("Higher", new int[] { 0, 0, 1 })
        };

        public List<EncodedRating> Encode(IEnumerable<MovieRating> movieRatings)
        {
            var V = Vector<double>.Build;
            var encodedRatings = new List<EncodedRating>();

            foreach (var movieRating in movieRatings)
            {
                var ageCode = ageCodes.Find(code => code.Item1 == movieRating.Age).Item2;
                var sexCode = sexCodes.Find(code => code.Item1 == movieRating.Sex).Item2;
                var educationCode = educationCodes.Find(code => code.Item1 == movieRating.Education).Item2;
                var userCode = ageCode
                    .Concat(sexCode)
                    .Concat(educationCode)
                    .Select(Convert.ToDouble)
                    .ToArray();

                var vector = V.DenseOfArray(userCode);
                var userAtttributeWithRating = new UserAtttributeWithRating(vector, movieRating.Rating);

                var existingEncodedRating = encodedRatings.Find(r => r.MovieName == movieRating.MovieName);

                if (existingEncodedRating != null)
                {
                    existingEncodedRating.UserAtttributesWithRating.Add(userAtttributeWithRating);
                }
                else
                {
                    encodedRatings.Add(new EncodedRating(movieRating.MovieName, userAtttributeWithRating));
                }
            }

            return encodedRatings;
        }
    }
}
