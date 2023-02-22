using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Abstract
{
    public interface IRecordService
    {
        void SaveRecord(ICollection<FileInfo> files);
        void SaveRecord(ICollection<FileInfo> files, string rootPath);
        bool TryGetLastRecordedFilesByRootPath(string rootPath, out ICollection<FileInfo>? filesFromRecord);
    }
}