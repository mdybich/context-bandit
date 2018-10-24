using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using RecommendationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationSystem.Core.Services
{
    public class DataService
    {
        public void Write(IEnumerable<LearnResult> learnResult, string label)
        {
            JsonSerializer serializer = new JsonSerializer();

            var path = string.Format(@"C:\Users\mdybich.LGBSPL\Documents\context-bandit\data_{0}.json", label);
            using (var sw = new StreamWriter(path))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, learnResult);
            }
        }

        public IEnumerable<LinUcbResult2> Read(string label)
        {
            var V = Vector<double>.Build;
            var M = Matrix<double>.Build;

            var path = string.Format(@"C:\Users\mdybich.LGBSPL\Documents\context-bandit\data_{0}.json", label);
            List<LearnResult> result;

            using (var file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (List<LearnResult>)serializer.Deserialize(file, typeof(List<LearnResult>));
            }

            return result.Select(item => new LinUcbResult2
            {
                MovieId = item.MovieId,
                A = M.DenseOfArray(item.A),
                B = V.DenseOfArray(item.B),
                B_Tolerant = V.DenseOfArray(item.B_Tolerant)
            });
        }

        public void WriteSecond(Dictionary<int, List<PartResult>> result, string label)
        {
            JsonSerializer serializer = new JsonSerializer();
            var path = string.Format(@"C:\Users\mdybich.LGBSPL\Documents\context-bandit\data_second_{0}.json", label);

            using (var sw = new StreamWriter(path))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, result);
            }
        }

        public Dictionary<int, List<PartResult>> ReadSecond(string label)
        {
            var path = string.Format(@"C:\Users\mdybich.LGBSPL\Documents\context-bandit\data_second_{0}.json", label);
            Dictionary<int, List<PartResult>> result;

            using (var file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (Dictionary<int, List<PartResult>>)serializer.Deserialize(file, typeof(Dictionary<int, List<PartResult>>));
            }

            return result;
        }

        public void WriteResult(Dictionary<int, List<ConfusionMatrix>> result, string label)
        {
            JsonSerializer serializer = new JsonSerializer();
            var path = string.Format(@"C:\Users\mdybich.LGBSPL\Documents\context-bandit\result_{0}.json", label);

            using (var sw = new StreamWriter(path))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, result);
            }
        }
    }
}
