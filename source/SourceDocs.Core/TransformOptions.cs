using System.Collections.Generic;

namespace SourceDocs.Core
{
    public class TransformOptions
    {
        public string[] ExcludeDirectories { get; set; }

        public IDictionary<string, IFileTransformer> FileTransformers { get; set; }
    }
}