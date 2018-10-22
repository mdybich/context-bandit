using RecommendationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationSystem.Core.Models
{
    public class MovieLensUser : IBasicUserInfo
    {
        public int UserId { get; set; }
        public string Gender { get; set; }
        public int OccupationId { get; set; }
        public string AgeGroup { get; set; }
        public string ZipCode { get; set; }

        public MovieLensUser(string[] userAttributes)
        {
            UserId = int.Parse(userAttributes[0]);
            Gender = userAttributes[1];
            AgeGroup = userAttributes[2];
            OccupationId = int.Parse(userAttributes[3]);
            ZipCode = userAttributes[4];
        }
    }
}
