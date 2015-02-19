using System.IO;

namespace SourceDocs.Core.Services
{
    public class MarkdownFileTransformer : IFileTransformer
    {
        public string Transform(FileInfo fileInfo)
        {
            var input = File.ReadAllText(fileInfo.FullName);

            var md = new MarkdownDeep.Markdown
            {
                ExtraMode = true,
                SafeMode = false
            };

            return md.Transform(input);
        }
    }
}