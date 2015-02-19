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

            var index = new List<IndexItem>();

            if (TransformRecursive(options, index, new DirectoryInfo(options.WorkingDirectory)))
            {
                CopyAndSerializeIndex(options, index);
            }
        }

        private bool TransformRecursive(TransformOptions options, IList<IndexItem> items, DirectoryInfo parent)
        {
            Func<FileSystemInfo, IndexItem> createIndexItem = fileSystemInfo => new IndexItem
            {
                Name = fileSystemInfo.Name,
                Path = FileHelper.GetRelativePath(options.WorkingDirectory, fileSystemInfo.FullName).Replace('\\', '/'),
                Children = new List<IndexItem>()
            };

            var itemsAdded = false;

            foreach (var directory in parent.EnumerateDirectories())
            {
                var indexItem = createIndexItem(directory);

                // todo: use more complex match with * instead of StartsWith
                if (options.ExcludeDirectories.Any(indexItem.Path.StartsWith)) continue;

                if (TransformRecursive(options, indexItem.Children, directory))
                {
                    items.Add(indexItem);
                    itemsAdded = true;
                }
            }

            foreach (var fileInfo in parent.EnumerateFiles())
            {
                var indexItem = createIndexItem(fileInfo);

                string content;
                if (Transform(options, fileInfo, out content))
                {
                    items.Add(indexItem);
                    itemsAdded = true;

                    // todo: move to Transform
                    var destFileName = Path.Combine(options.TempDirectory, indexItem.Path);
                    FileHelper.EnsureDirectoryExists(Path.GetDirectoryName(destFileName));
                    File.WriteAllText(destFileName, content);
                }
            }

            return itemsAdded;
        }

        private static bool Transform(TransformOptions options, FileInfo fileInfo, out string output)
        {
            IFileTransformer transformer;
            if (options.FileTransformers.TryGetValue(fileInfo.Extension, out transformer))
            {
                output = transformer.Transform(fileInfo);
                return output != null;
            }

            output = null;
            return false;
        }

        private void CopyAndSerializeIndex(TransformOptions options, List<IndexItem> index)
        {
            FileHelper.EmptyDirectory(options.OutputDirectory);
            FileHelper.CopyDirectory(options.TempDirectory, options.OutputDirectory);
            FileHelper.EmptyDirectory(options.TempDirectory);

            File.WriteAllText(
                Path.Combine(options.OutputDirectory, "index.json"),
                _javaScriptSerializer.Serialize(index));
        }
    }
}
