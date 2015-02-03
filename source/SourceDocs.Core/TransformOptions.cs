using System.Collections.Generic;
using SourceDocs.Core.Services;

namespace SourceDocs.Core
{
    public class TransformOptions
    {
        public string[] ExcludeDirectories { get; set; }

        public IDictionary<string, IFileTransformer> FileTransformers { get; set; }
    }
}