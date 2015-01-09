using System.Collections.Generic;

namespace SourceDocs.Core
{
    public class IndexItem
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<IndexItem> Children { get; set; }
    }
}