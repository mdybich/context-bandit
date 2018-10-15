using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecommendationSystem.Api.Models
{
    public class PostRecommendation
    {
        public string Gender { get; set; }
        public int AgeId { get; set; }
        public int OccuptionId { get; set; }
    }
}