using System.Collections.Generic;

namespace SourceDocs.Core.Models
{
    public class Repo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public IList<Node> Nodes { get; set; }
    }
}