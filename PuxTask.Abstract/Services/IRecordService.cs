using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Abstract
{
    public interface IRecordService
    {
        void SaveRecord(ICollection<FileInfo> files);
        void SaveRecord(ICollection<FileInfo> files, string analysedFolderPath);
        bool TryGetLastRecordedFilesByAnalysedFolderPath(string analysedFolderPath, out ICollection<FileInfo>? filesFromRecord);
    }
}