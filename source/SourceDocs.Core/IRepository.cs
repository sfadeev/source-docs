using System;
using System.Collections.Generic;
using SourceDocs.Core.Models;

namespace SourceDocs.Core
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Node> UpdateNodes(IEnumerable<Node> nodes);

        bool UpdateNode(Node node);
    }
}