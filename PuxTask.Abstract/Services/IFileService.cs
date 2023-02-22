using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Abstract
{
    public interface IFileService
    {
        ICollection<FileInfo> GetFilesByPath(string analysedFolderPath);
    }
}