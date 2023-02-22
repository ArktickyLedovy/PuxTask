using Microsoft.Extensions.Logging;
using PuxTask.Abstract;
using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Core
{
    internal class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
            _logger.LogInformation("File service sucesfully instanciated");
        }

        public ICollection<FileInfo> GetFilesByPath(string analysedFolderPath)
        {
            try
            {
                _logger.LogInformation("Getting files in directory" + analysedFolderPath);
                var files = new List<FileInfo>();
                string[] allPaths = Directory.GetFiles(analysedFolderPath, "*.*", SearchOption.AllDirectories);
                foreach (var filePath in allPaths)
                {
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        using (BufferedStream bufferedStream = new BufferedStream(stream))
                        {
                            files.Add(new(stream, bufferedStream, analysedFolderPath));
                        }
                    }
                }
                return files;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when getting files in "+analysedFolderPath, ex);
            }
        }
    }
}
