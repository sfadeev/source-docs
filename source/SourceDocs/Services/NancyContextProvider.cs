using System.IO;
using Nancy;
using SourceDocs.Core.Services;

namespace SourceDocs.Services
{
    public class NancyContextProvider : IContextProvider
    {
        private readonly string _workingDir;

        public NancyContextProvider(IRootPathProvider rootPathProvider)
        {
            _workingDir = Path.Combine(rootPathProvider.GetRootPath(), ".repos/");
        }

        public string MapPath(string relativePath)
        {
            return Path.Combine(_workingDir, relativePath);
        }
    }
}