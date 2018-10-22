using MathNet.Numerics.LinearAlgebra;
using RecommendationSystem.Core.Models;
using RecommendationSystem.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RecommendationSystem.Api.Controllers
{
    public class RecommendationsController : ApiController
    {
        public IHttpActionResult Post([FromBody]BasicUserInfo userInfo)
        {
            var vectorBuilder = Vector<double>.Build;
            var encodeService = new EncodeService();
            var fileService = new FileService();
            var linUcbService = LinUcbService.Instance;

            var movies = fileService.ReadMovies();
            var userVector = encodeService.GetUserVector(userInfo, vectorBuilder);

            var recommendations = linUcbService.RecommendMovies(userVector, movies);
            Console.WriteLine(recommendations);

            return Ok(recommendations);
        }
    }
}
