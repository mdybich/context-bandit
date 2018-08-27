using MathNet.Numerics.LinearAlgebra;
using RecommendationSystem.Services;
using System;

namespace RecommendationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileService = new FileService();
            var encodeService = new EncodeService();
            var linUcbService = new LinUcbService();
            var uiService = new UIService();

            var t = fileService.ReadFile();
            var c = encodeService.Encode(t);

            linUcbService.Learn(c);

            uiService.DisplayApplication();
            var V = Vector<double>.Build;
            var vector = V.DenseOfArray(new double[] { 0.0, 0.0, 1.0, 1.0, 0.0, 0.0, 1.0, 0.0 });

            var recomenndation = linUcbService.RecommendMovie(vector);
        }
    }
}
