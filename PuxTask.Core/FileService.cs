using Microsoft.Extensions.Logging;
using PuxTask.Abstract;
using PuxTask.Common.Exceptions;
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
        /// <summary>
        /// Gets files from given directory
        /// </summary>
        /// <param name="analysedFolderPath">Path from input</param>
        /// <returns>File infos</returns>
        public ICollection<FileInfo> GetFilesByPath(string analysedFolderPath)
        {
            try
            {
                if (Directory.Exists(analysedFolderPath))
                {
                    //Change relative path to absolute
                    if (!Path.IsPathRooted(analysedFolderPath))
                    {
                        _logger.LogInformation($"Path from input is relative: {analysedFolderPath}. Changing to absolute");
                        analysedFolderPath = Path.GetFullPath(analysedFolderPath);
                    }
                    _logger.LogInformation("Getting files in directory " + analysedFolderPath);
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
                throw new InvalidPathException($"Directory on location {analysedFolderPath} does not exist. Path is invalid");
            }
            catch (InvalidPathException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when getting files in "+analysedFolderPath, ex);
            }
        }
    }
}
