using System;
using System.Diagnostics;
using System.Globalization;

namespace SourceDocs.Core
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
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "{0} ({1}), updated: {2}, generated: {3}", Name, RemoteName, Updated, Generated);
            }
        }
    }
}