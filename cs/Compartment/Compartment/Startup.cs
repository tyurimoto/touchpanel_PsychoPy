using System.Web.Http;
using Owin;

namespace Compartment
{
    /// <summary>
    /// OWIN Startup class for Web API configuration
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configure Web API using OWIN
        /// </summary>
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Web API routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // JSON formatter settings
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling =
                Newtonsoft.Json.NullValueHandling.Ignore;

            // Enable CORS (Cross-Origin Resource Sharing) for PsychoPy access
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            // Use Web API with OWIN
            app.UseWebApi(config);
        }
    }
}
