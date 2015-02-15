using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SourceDocs.Core.Helpers
{
    public static class FileHelper
    {
        public static string GetWorkingDir(string workingRoot, string repoUrl, params string[] repoPaths)
        {
            Func<string, string> fixDirName = dirName =>
                Path.GetInvalidFileNameChars().Aggregate(dirName, (current, c) => current.Replace(c, '_'));

            var workingDir = Path.Combine(new[] { workingRoot, fixDirName(repoUrl) }.Concat(repoPaths.Select(fixDirName)).ToArray());

            EnsureDirectoryExists(workingDir);

            return workingDir;
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void EmptyDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public static void CopyDirectory(string from, string to)
        {
            var directoryInfo = new DirectoryInfo(@from);

            // todo: enumerate only files or merge with Transform method
            foreach (var fileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                var relativePath = GetRelativePath(directoryInfo.FullName, fileSystemInfo.FullName);

                var destFileName = Path.Combine(to, relativePath);

                if ((fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.CreateDirectory(destFileName);
                }
                else
                {
                    File.Copy(fileSystemInfo.FullName, destFileName);
                }
            }
        }

        public static string GetRelativePath(string folderPath, string filePath)
        {
            var fileUri = new Uri(filePath);

            if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                folderPath += Path.DirectorySeparatorChar;
            }

            var folderUri = new Uri(folderPath);

            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
