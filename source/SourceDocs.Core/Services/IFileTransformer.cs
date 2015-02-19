using System.IO;

namespace SourceDocs.Core.Services
{
    public interface IFileTransformer
    {
        string Transform(FileInfo fileInfo);
    }
}