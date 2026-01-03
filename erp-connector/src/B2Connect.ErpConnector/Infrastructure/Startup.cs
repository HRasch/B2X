using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace B2Connect.ErpConnector.Infrastructure
{
    /// <summary>
    /// OWIN Startup class for configuring the Web API.
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Configure JSON serialization
            var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.Formatting = Formatting.Indented;
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Remove XML formatter (JSON only)
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Convention-based routing
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Enable CORS (for development)
            // config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            app.UseWebApi(config);
        }
    }
}
