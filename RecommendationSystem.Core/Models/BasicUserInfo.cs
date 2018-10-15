using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystem.Core.Models
{
    public interface IBasicUserInfo
    {
        string Gender { get; set; }
        int OccupationId { get; set; }
        string AgeGroup { get; set; }
    }

    public class BasicUserInfo : IBasicUserInfo
    {
        public string Gender { get; set; }
        public int OccupationId { get; set; }
        public string AgeGroup { get; set; }
    }
}
