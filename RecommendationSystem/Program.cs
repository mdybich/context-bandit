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

            var t = fileService.ReadFile();
            var c = encodeService.Encode(t);

            linUcbService.Learn(c);

            var V = Vector<double>.Build;
            Console.WriteLine("Age (Young, Middle, Old):");
            var line = Console.ReadLine();
            var vector = V.DenseOfArray(new double[] { 0.0, 0.0, 1.0, 1.0, 0.0, 0.0, 1.0, 0.0 });

            var recomenndation = linUcbService.RecommendMovie(vector);
        }
    }
}
