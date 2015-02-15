using System;
using System.Reflection;
using log4net;
using Microsoft.AspNet.SignalR;
using SourceDocs.Core.Services;

namespace SourceDocs.Services
{
    public class NotificationHub : Hub
    {
    }

    public class SignalRNotificationService : INotificationService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Notify(string message, NotificationType type)
        {
            if (Log.IsDebugEnabled) Log.Debug("Checking for updates in repositories.");

            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

            context.Clients.All.notify(new
            {
                text = message,
                type = type.ToString().ToLowerInvariant(),
                when = DateTime.Now
            });

            // context.Clients.Group("groupname").methodInJavascript("hello world");
        }
    }
}