using System;
using System.Diagnostics;

namespace SourceDocs.Core.Models
{
    /// <summary>
    /// Info about documentation node (repository branch)
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Node
    {
        public string Name { get; set; }

        public string RemoteName { get; set; }

        public DateTimeOffset Updated { get; set; }

        public DateTimeOffset? Generated { get; set; }

        private string DebuggerDisplay
        {
            get { return string.Format("{0} ({1}), updated: {2}", Name, RemoteName, Updated); }
        }
    }
}