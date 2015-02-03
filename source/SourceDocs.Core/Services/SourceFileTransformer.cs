namespace SourceDocs.Core.Services
{
    public class SourceFileTransformer : IFileTransformer
    {
        public string Transform(string input)
        {
            return input; // simple for now
        }
    }
}