namespace SourceDocs.Core.Services
{
    public class MarkdownFileTransformer : IFileTransformer
    {
        public string Transform(string input)
        {
            var md = new MarkdownDeep.Markdown
            {
                ExtraMode = true,
                SafeMode = false
            };

            return md.Transform(input);
        }
    }
}