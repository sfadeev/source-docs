using System;
using System.Collections.Generic;

namespace SourceDocs.Core
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Node> UpdateNodes(IEnumerable<Node> nodes);

        bool UpdateNode(Node node);
    }
}