using Nancy;
using Nancy.Conventions;
using Nancy.Json;
using Nancy.TinyIoc;
using SourceDocs.Core;
using SourceDocs.Core.Services;
using SourceDocs.Services;

namespace SourceDocs
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            JsonSettings.MaxJsonLength = int.MaxValue;

            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("assets"));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IContextProvider, NancyContextProvider>();
            container.Register<IRepositoryCatalog, DefaultRepositoryCatalog>();
        }
    }
}