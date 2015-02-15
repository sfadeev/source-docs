using System;
using Owin;
using SourceDocs.Helpers;

namespace SourceDocs
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AppDomain.CurrentDomain.UnhandledException += LogHelper.LogCurrentDomainUnhandledException;

            app.MapSignalR();
        }
    }
}