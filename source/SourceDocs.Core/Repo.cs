using System.Collections.Generic;

namespace SourceDocs.Core
{
    public class Repo
    {
        public string Id { get; set; }

        public IList<Node> Nodes { get; set; }
    }
}