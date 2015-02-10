using System;
using System.Collections.Generic;
using SourceDocs.Core.Models;

namespace SourceDocs.Core
{
    public interface IRepository : IDisposable
    {
        string Url { get; }

        IEnumerable<Node> UpdateNodes(IEnumerable<Node> nodes = null);

        bool UpdateNode(Node node);
    }
}