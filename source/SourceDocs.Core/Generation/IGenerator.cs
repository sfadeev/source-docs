namespace SourceDocs.Core.Generation
{
    public interface IGenerator
    {
        bool ShouldGenerate();
    }

    public class GitGenerator : IGenerator
    {
        public string SourceUrl { get; set; }

        public string WorkingDir { get; set; }

        public bool ShouldGenerate()
        {
            return true;
        }
    }
}
