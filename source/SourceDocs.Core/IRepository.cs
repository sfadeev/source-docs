using System.Collections.Generic;
using SourceDocs.Core.Models;

namespace SourceDocs.Core
{
    public interface IRepository
    {
        // todo: remove
        string Url { get; }

        IList<Node> UpdateNodes(IEnumerable<Node> nodes = null);

        bool UpdateNode(Node node);
    }
}