using System.Collections.Generic;

namespace SourceDocs.Core.Models
{
    public class Repos
    {
        public IList<Repo> Items { get; set; }
    }

    public class Repo
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public IList<Node> Nodes { get; set; }
    }
}