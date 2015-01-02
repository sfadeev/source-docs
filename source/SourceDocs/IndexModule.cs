using Nancy;

namespace SourceDocs
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                return View["index"];
            };

            Get["/files"] = parameters =>
            {
                return new[]
                {
                    new { name = "Contacts 1", url = "contacts", navigationTrigger = "contacts:list" },
                    new { name = "Contacts 2", url = "contacts", navigationTrigger = "contacts:list" },
                    new { name = "Contacts 3", url = "contacts", navigationTrigger = "contacts:list" },
                    new { name = "About", url = "about", navigationTrigger = "about:show" }
                };

                // return Response.AsFile(".repos/index.json");
            };

            Get["/Dashboard{index}"] = x =>
            {
                return View["Dashboard" + x.index];
            };
        }
    }
}