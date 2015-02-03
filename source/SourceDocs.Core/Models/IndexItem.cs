using System.Collections.Generic;

namespace SourceDocs.Core.Models
{
    public class IndexItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<IndexItem> Children { get; set; }
    }
}