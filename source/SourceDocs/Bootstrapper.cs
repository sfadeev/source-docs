using System.Reflection;
using log4net;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Json;
using Nancy.TinyIoc;
using SourceDocs.Core.Services;
using SourceDocs.Services;

namespace SourceDocs
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            if (Log.IsDebugEnabled) Log.Debug("Configuring conventions.");

            base.ConfigureConventions(conventions);

            JsonSettings.MaxJsonLength = int.MaxValue;

            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("assets"));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register<IContextProvider, NancyContextProvider>();
            container.Register<IJavaScriptSerializer, DefaultJavaScriptSerializer>();

            container.Register<IEventAggregator, EventAggregator>().AsSingleton();
            container.Register<IRepositoryCatalog, DefaultRepositoryCatalog>().AsSingleton();
            container.Register<ITaskManager, DefaultTaskManager>().AsSingleton();
        }
    }

    public class StartableRunner : IApplicationStartup
    {
        private readonly TinyIoCContainer _container;

        public StartableRunner(TinyIoCContainer container)
        {
            _container = container;
        }

        public void Initialize(IPipelines pipelines)
        {
            var startables = _container.ResolveAll<IStartable>();

            foreach (var startable in startables)
            {
                startable.Start();
            }
        }
    }
}