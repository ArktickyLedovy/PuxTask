using PuxTask.Abstract;
using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Core
{
    internal class FileService : IFileService
    {
        public ICollection<FileInfo> GetFilesByPath(string rootPath)
        {
            var files = new List<FileInfo>();
            string[] allPaths = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in allPaths)
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    files.Add(new(stream, rootPath));
                }
            }
            return files;
        }
    }
}
