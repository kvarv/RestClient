using System.Web.Http;
using Owin;

namespace Rest.Tests.TestRestServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.MapHttpAttributeRoutes();
            app.UseWebApi(httpConfiguration);
        }
    }
}