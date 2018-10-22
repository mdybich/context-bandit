using RecommendationSystem.Core.Services;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RecommendationSystem.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var fileService = new FileService();
            var linUcbService = LinUcbService.Instance;
            var encodeService = new EncodeService();

            var users = fileService.ReadUsers();
            var ratings = fileService.ReadRatings();
            var encodedUsers = encodeService.EncodeUser(users);

            linUcbService.LearnFromMovieLens(encodedUsers, ratings);
        }
    }
}
