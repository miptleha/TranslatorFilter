using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Translator;

namespace Classic
{
    public class Global : HttpApplication
    {
        public Global()
        {
            this.PostRequestHandlerExecute += Global_PostRequestHandlerExecute;
        }

        private void Global_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            Response.Filter = new TranslatorFilterStream(Response.Filter);
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}