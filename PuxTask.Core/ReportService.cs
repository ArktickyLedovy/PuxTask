using PuxTask.Abstract;
using PuxTask.Common.Entities;
using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Core
{
    internal class ReportService : IReportService
    {
        private readonly IFileService _fileService;
        private readonly IRecordService _recordService;
        public ReportService(IFileService fileService, IRecordService recordService)
        {
            _fileService = fileService;
            _recordService = recordService;
        }
        public ICollection<FileReport> GetReports(string rootPath)
        {
            ICollection<FileReport> reports = new List<FileReport>();
            ICollection<FileInfo> filesFromRecord = new List<FileInfo>();
            ICollection<FileInfo> analysedFiles = new List<FileInfo>();

            analysedFiles = _fileService.GetFilesByPath(rootPath);
            if (_recordService.TryGetLastRecordedFilesByRootPath(rootPath, out filesFromRecord))
            {
                reports = CompareAndConvert(reports, filesFromRecord, ref analysedFiles);
                _recordService.SaveRecord(analysedFiles);
                return reports;
            }
            reports = Convert(reports, ref analysedFiles);
            _recordService.SaveRecord(analysedFiles, rootPath);
            return reports;
        }

        private ICollection<FileReport> CompareAndConvert(ICollection<FileReport> reports, ICollection<FileInfo> filesFromRecord, ref ICollection<FileInfo> analysedFiles)
        {
            Parallel.ForEach(analysedFiles, file =>
            {
                var report = new FileReport()
                {
                    FileName = file.Name,
                };
                var recordFile = filesFromRecord.Where(f => f.Name == file.Name).FirstOrDefault();
                if (recordFile != null)
                {
                    file.Version = recordFile.Version;
                    if (recordFile.FileHash == file.FileHash)
                    {
                        report.State = Common.Enums.FileState.Unchanged;
                    }
                    else
                    {
                        report.State = Common.Enums.FileState.Modified;
                        file.Version++;
                    }
                    filesFromRecord.Remove(recordFile);
                }
                else
                {
                    report.State = Common.Enums.FileState.Added;
                    file.Version = 1;
                }
                report.Version = file.Version;
                reports.Add(report);
            });
            foreach (var remainingRecordFiles in filesFromRecord)
            {
                reports.Add(new()
                {
                    FileName=remainingRecordFiles.Name,
                    State = Common.Enums.FileState.Deleted
                });
            }
            return reports;
        }
        private ICollection<FileReport> Convert(ICollection<FileReport> reports, ref ICollection<FileInfo> analysedFiles)
        {
            Parallel.ForEach(analysedFiles, file =>
            {
                file.Version = 1;
                reports.Add(new()
                {
                    FileName = file.Name,
                    State = Common.Enums.FileState.Unchanged,
                    Version = file.Version
                });
            });
            return reports;
        }
    }
}
