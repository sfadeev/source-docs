namespace SourceDocs.Core
{
    public class SourceFileTransformer : IFileTransformer
    {
        public string Transform(string input)
        {
            return input; // simple for now
        }
    }
}