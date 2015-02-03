using System.IO;
using Newtonsoft.Json;
using SourceDocs.Core.Models;
using SourceDocs.Core.Services;

namespace SourceDocs.Core
{
    public interface IRepositoryCatalog
    {
        Repos GetRepos();
    }

    public class DefaultRepositoryCatalog : IRepositoryCatalog
    {
        private readonly IContextProvider _contextProvider;

        public DefaultRepositoryCatalog(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public Repos GetRepos()
        {
            var reposConfigPath = _contextProvider.MapPath("repos.json");
            var reposStream = File.ReadAllText(reposConfigPath);
            var result = JsonConvert.DeserializeObject<Repos>(reposStream);

            foreach (var item in result.Items)
            {
                item.Nodes = new[]
                {
                    new Node { Name = "2.1.3" },
                    new Node { Name = "2.1.1" },
                    new Node { Name = "2.1.0" },
                    new Node { Name = "2.0.3" },
                    new Node { Name = "2.0.2" },
                    new Node { Name = "2.0.1" },
                    new Node { Name = "2.0.0" },
                    new Node { Name = "1.11.2" },
                    new Node { Name = "1.11.1" },
                    new Node { Name = "1.11.0" }
                };
            }

            return result;
        }
    }
}
