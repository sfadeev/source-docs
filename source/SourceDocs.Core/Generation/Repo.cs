using System.Collections.Generic;

namespace SourceDocs.Core.Generation
{
    public class Repo
    {
        public Repo()
        {
            Nodes = new List<Node>();
        }

        public IList<Node> Nodes { get; set; }
    }
}