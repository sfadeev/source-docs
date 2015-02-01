using System.IO;
using Newtonsoft.Json;

namespace SourceDocs.Core
{
    public interface IRepositoryCatalog
    {
        Repos GetRepos();
    }

    public class DefaultRepositoryCatalog : IRepositoryCatalog
    {
        private readonly string _workingRoot;

        public DefaultRepositoryCatalog(string workingRoot)
        {
            _workingRoot = workingRoot;
        }

        public Repos GetRepos()
        {
            var reposConfig = Path.Combine(_workingRoot, "repos.json");
            var reposStream = File.ReadAllText(reposConfig);
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
