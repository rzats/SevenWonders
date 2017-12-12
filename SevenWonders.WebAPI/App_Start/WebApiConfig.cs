using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SevenWonders.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}",
           new { id = RouteParameter.Optional });

            config
               .EnableSwagger(c => c.SingleApiVersion("v1", "SevenWonders"))
               .EnableSwaggerUi();
        }
    }
}
