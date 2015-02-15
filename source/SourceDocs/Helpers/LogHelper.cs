using System;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using log4net;

namespace SourceDocs.Helpers
{
    public static class LogHelper
    {
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Initialize()
        {
            GlobalContext.Properties["UserName"] = new HttpContextProperty(x => x.User.Identity.Name);
            GlobalContext.Properties["UserHostAddress"] = new HttpContextProperty(x => x.Request.UserHostAddress);
            GlobalContext.Properties["UserHostName"] = new HttpContextProperty(x => x.Request.UserHostName);
            GlobalContext.Properties["UserAgent"] = new HttpContextProperty(x => x.Request.UserAgent);
            GlobalContext.Properties["RawUrl"] = new HttpContextProperty(x => x.Request.RawUrl);
        }

        public static void LogCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (Log.IsErrorEnabled)
            {
                Log.Error("Current domain unhandled exception, terminating: " + e.IsTerminating,
                    e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject.ToString()));
            }
        }
        
        public static void LogHttpRuntimeShutdownReason()
        {
            if (Log.IsErrorEnabled)
            {
                try
                {
                    Log.ErrorFormat("HostingEnvironment.ShutdownReason = " + HostingEnvironment.ShutdownReason);

                    // http://weblogs.asp.net/scottgu/archive/2005/12/14/433194.aspx

                    var runtime = (HttpRuntime) typeof (HttpRuntime).InvokeMember("_theRuntime",
                        BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
                    if (runtime == null) return;

                    var shutDownMessage = (string) runtime.GetType().InvokeMember("_shutDownMessage",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

                    var shutDownStack = (string) runtime.GetType().InvokeMember("_shutDownStack",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null);

                    Log.ErrorFormat("HttpRuntime shutdown reason: {0} \n{1}", shutDownMessage, shutDownStack);

                }
                catch (Exception ex)
                {
                    Log.Error("Failed to log HttpRuntime shutdown reason", ex);
                }
            }
        }

        private static bool IsHttpRequest
        {
            get { return HttpContext.Current != null && HttpContext.Current.Handler != null; }
        }

        private class HttpContextProperty
        {
            private readonly Func<HttpContext, string> _expression;

            public HttpContextProperty(Func<HttpContext, string> expression)
            {
                _expression = expression;
            }

            [DebuggerStepThrough]
            public override string ToString()
            {
                try
                {
                    return IsHttpRequest ? _expression(HttpContext.Current) ?? string.Empty : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}