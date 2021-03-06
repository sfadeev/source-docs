using System.Collections.Generic;

namespace SourceDocs.Core.Services
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