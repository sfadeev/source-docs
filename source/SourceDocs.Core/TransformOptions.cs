using System.Collections.Generic;
using SourceDocs.Core.Services;

namespace SourceDocs.Core
{
    // todo: split to environment and settings
    public class TransformOptions
    {
        public string TempDirectory { get; set; }

        public string WorkingDirectory { get; set; }

        public string OutputDirectory { get; set; }

        public string[] ExcludeDirectories { get; set; }

        public IDictionary<string, IFileTransformer> FileTransformers { get; set; }
    }
}