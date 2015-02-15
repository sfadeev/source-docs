using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SourceDocs.Core.Helpers;
using SourceDocs.Core.Models;

namespace SourceDocs.Core.Services
{
    public interface IRepositoryTransformer
    {
        void Transform(TransformOptions options);
    }

    public class RepositoryTransformer : IRepositoryTransformer
    {
        private readonly IJavaScriptSerializer _javaScriptSerializer;
        private readonly INotificationService _notificationService;

        public RepositoryTransformer(IJavaScriptSerializer javaScriptSerializer, INotificationService notificationService)
        {
            _javaScriptSerializer = javaScriptSerializer;
            _notificationService = notificationService;
        }

        public void Transform(TransformOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");

            FileHelper.EnsureDirectoryExists(options.TempDirectory);
            FileHelper.EmptyDirectory(options.TempDirectory);

            var index = new List<IndexItem>();

            var directoryInfo = new DirectoryInfo(options.WorkingDirectory);

            foreach (var fileInfo in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = FileHelper.GetRelativePath(directoryInfo.FullName, fileInfo.FullName);

                // todo: use more complex match with * instead of StartsWith
                if (options.ExcludeDirectories.Any(relativePath.StartsWith)) continue;

                IFileTransformer transformer;
                if (options.FileTransformers.TryGetValue(fileInfo.Extension, out transformer))
                {
                    index.Add(new IndexItem { Name = Path.GetFileName(relativePath), Path = relativePath.Replace('\\', '/') });

                    var destFileName = Path.Combine(options.TempDirectory, relativePath);
                    var destDirectoryName = Path.GetDirectoryName(destFileName);

                    FileHelper.EnsureDirectoryExists(destDirectoryName);

                    var input = File.ReadAllText(fileInfo.FullName);
                    var output = transformer.Transform(input);

                    File.WriteAllText(destFileName, output);
                }
            }

            FileHelper.EmptyDirectory(options.OutputDirectory);
            FileHelper.CopyDirectory(options.TempDirectory, options.OutputDirectory);
            FileHelper.EmptyDirectory(options.TempDirectory);

            var indexPath = Path.Combine(options.OutputDirectory, "index.json");
            File.WriteAllText(indexPath, _javaScriptSerializer.Serialize(index));
        }
    }
}
