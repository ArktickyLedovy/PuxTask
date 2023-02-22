using PuxTask.Common.Entities;

namespace PuxTask.Abstract
{
    public interface IReportService
    {
        ICollection<FileReport> GetReports(string analysedFolderPath);
    }
}