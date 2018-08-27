using RecommendationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

using UserAttributeCode = System.Tuple<string, int[]>;
using OccupationCode = System.Tuple<int, int[]>;

namespace RecommendationSystem.Services
{
    public class EncodeService
    {
        private List<UserAttributeCode> ageCodes;
        private List<UserAttributeCode> newAgeCodes;
        private List<OccupationCode> occupationCodes;
        private List<UserAttributeCode> genderCodes;
        private List<UserAttributeCode> educationCodes;

        public EncodeService()
        {
            GenerateAgeCodes();
            GenerateNewAgeCodes();
            GenerateOccupationCodes();
            GenerateGenderCodes();
            GenerateEducationCodes();
        }

        public List<EncodedRating> Encode(IEnumerable<MovieRating> movieRatings)
        {
            var V = Vector<double>.Build;
            var encodedRatings = new List<EncodedRating>();

            foreach (var movieRating in movieRatings)
            {
                var ageCode = ageCodes.Find(code => code.Item1 == movieRating.Age).Item2;
                var sexCode = genderCodes.Find(code => code.Item1 == movieRating.Sex).Item2;
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

        public Dictionary<int, EncodedUser> EncodeUser(IEnumerable<MovieLensUser> movieLensUsers)
        {
            var V = Vector<double>.Build;

            var encodedUsers = new List<EncodedUser>();

            foreach (var movieLensUser in movieLensUsers)
            {
                var ageCode = newAgeCodes.Find(code => code.Item1 == movieLensUser.AgeGroup).Item2;
                var genderCode = genderCodes.Find(code => code.Item1 == movieLensUser.Gender).Item2;
                var occupationCode = occupationCodes.Find(code => code.Item1 == movieLensUser.OccupationId).Item2;

                var userCode = ageCode
                    .Concat(genderCode)
                    .Concat(occupationCode)
                    .Select(Convert.ToDouble)
                    .ToArray();

                var userVector = V.DenseOfArray(userCode);
                encodedUsers.Add(new EncodedUser { UserId = movieLensUser.UserId, EncodedAttributes = userVector });
            }

            return encodedUsers.ToDictionary(e => e.UserId);
        }

        private void GenerateAgeCodes()
        {
            ageCodes = new List<UserAttributeCode>
            {
                new UserAttributeCode("Young",  GenerateEncodedVector(0, 3)),
                new UserAttributeCode("Middle", GenerateEncodedVector(1, 3)),
                new UserAttributeCode("Old",    GenerateEncodedVector(2, 3)),
            };
        }

        private void GenerateNewAgeCodes()
        {
            newAgeCodes = new List<UserAttributeCode>
            {
                new UserAttributeCode("1",  GenerateEncodedVector(0, 7)),
                new UserAttributeCode("18", GenerateEncodedVector(1, 7)),
                new UserAttributeCode("25", GenerateEncodedVector(2, 7)),
                new UserAttributeCode("35", GenerateEncodedVector(3, 7)),
                new UserAttributeCode("45", GenerateEncodedVector(4, 7)),
                new UserAttributeCode("50", GenerateEncodedVector(5, 7)),
                new UserAttributeCode("56", GenerateEncodedVector(6, 7)),
            };
        }

        private void GenerateOccupationCodes()
        {
            occupationCodes = new List<OccupationCode>
            {
                new OccupationCode(0, GenerateEncodedVector(0, 21)),
                new OccupationCode(1, GenerateEncodedVector(1, 21)),
                new OccupationCode(2, GenerateEncodedVector(2, 21)),
                new OccupationCode(3, GenerateEncodedVector(3, 21)),
                new OccupationCode(4, GenerateEncodedVector(4, 21)),
                new OccupationCode(5, GenerateEncodedVector(5, 21)),
                new OccupationCode(6, GenerateEncodedVector(6, 21)),
                new OccupationCode(7, GenerateEncodedVector(7, 21)),
                new OccupationCode(8, GenerateEncodedVector(8, 21)),
                new OccupationCode(9, GenerateEncodedVector(9, 21)),
                new OccupationCode(10, GenerateEncodedVector(10, 21)),
                new OccupationCode(11, GenerateEncodedVector(11, 21)),
                new OccupationCode(12, GenerateEncodedVector(12, 21)),
                new OccupationCode(13, GenerateEncodedVector(13, 21)),
                new OccupationCode(14, GenerateEncodedVector(14, 21)),
                new OccupationCode(15, GenerateEncodedVector(15, 21)),
                new OccupationCode(16, GenerateEncodedVector(16, 21)),
                new OccupationCode(17, GenerateEncodedVector(17, 21)),
                new OccupationCode(18, GenerateEncodedVector(18, 21)),
                new OccupationCode(19, GenerateEncodedVector(19, 21)),
                new OccupationCode(20, GenerateEncodedVector(20, 21))
            };
        }

        private void GenerateGenderCodes()
        {
            genderCodes = new List<UserAttributeCode>
            {
                new UserAttributeCode("M", GenerateEncodedVector(0, 2)),
                new UserAttributeCode("F", GenerateEncodedVector(1, 2))
            };
        }

        private void GenerateEducationCodes()
        {
            educationCodes = new List<UserAttributeCode>
            {
                new UserAttributeCode("Primary", GenerateEncodedVector(0, 3)),
                new UserAttributeCode("Secondary", GenerateEncodedVector(1, 3)),
                new UserAttributeCode("Higher", GenerateEncodedVector(2, 3))
            };
        }

        private int[] GenerateEncodedVector(int codePosition, int vectorLength)
        {
            var vector = Enumerable.Repeat(0, vectorLength).ToArray();
            vector[codePosition] = 1;
            return vector;
        }
    }
}
