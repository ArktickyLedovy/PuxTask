using PuxTask.Common.Entities;

namespace PuxTask.Abstract
{
    public interface IReportService
    {
        Report GetReports(string analysedFolderPath);
    }
}