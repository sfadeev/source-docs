using System;

namespace SourceDocs.Core.Generation
{
    /// <summary>
    /// Info about documentation node (repository branch)
    /// </summary>
    public class Node
    {
        public string Name { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Generated { get; set; }
    }
}