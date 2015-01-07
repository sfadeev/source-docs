using Nancy;

namespace SourceDocs
{
    public class ApiModule : NancyModule
    {
        public ApiModule() : base("/api")
        {
            Get["/repos"] = parameters => new
            {
                items = new[]
                {
                    new { id = "jQuery" },
                    new { id = "LibGit2Sharp" },
                    new { id = "log4net" },
                    new { id = "Nancy" },
                    new { id = "Nancy.Hosting.Aspnet" },
                    new { id = "Nancy.Viewengines.Razor" },
                    new { id = "Twitter.Bootstrap.Less" }
                }
            };
        }
    }
}