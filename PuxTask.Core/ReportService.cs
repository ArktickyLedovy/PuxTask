using Microsoft.Extensions.Logging;
using PuxTask.Abstract;
using PuxTask.Common.Entities;
using System.Linq.Expressions;
using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Core
{
    internal class ReportService : IReportService
    {
        private readonly IFileService _fileService;
        private readonly IRecordService _recordService;
        private readonly ILogger<ReportService> _logger;
        public ReportService(IFileService fileService, IRecordService recordService, ILogger<ReportService> logger)
        {
            _logger = logger;
            _fileService = fileService;
            _recordService = recordService;
            _logger.LogInformation("Report service sucesfully instanciated");
        }
        public ICollection<FileReport> GetReports(string analysedFolderPath)
        {
            try
            {
                _logger.LogInformation($"Getting reports for folder {analysedFolderPath}");
                ICollection<FileReport> reports = new List<FileReport>();
                ICollection<FileInfo> filesFromRecord = new List<FileInfo>();
                ICollection<FileInfo> analysedFiles = new List<FileInfo>();

                analysedFiles = _fileService.GetFilesByPath(analysedFolderPath);
                if (_recordService.TryGetLastRecordedFilesByAnalysedFolderPath(analysedFolderPath, out filesFromRecord))
                {
                    _logger.LogInformation($"Record for this folder found");
                    reports = CompareAndConvert(reports, filesFromRecord, ref analysedFiles);
                    _recordService.SaveRecord(analysedFiles);
                    return reports;
                }
                _logger.LogWarning($"Record for this folder not found");
                reports = Convert(reports, ref analysedFiles);
                _recordService.SaveRecord(analysedFiles, analysedFolderPath);
                return reports;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        private ICollection<FileReport> CompareAndConvert(ICollection<FileReport> reports, ICollection<FileInfo> filesFromRecord, ref ICollection<FileInfo> analysedFiles)
        {
            try
            {
                _logger.LogInformation("Comparing recorded files and newly analysed files and creating report");
                foreach (var file in analysedFiles)
                {
                    var report = new FileReport()
                    {
                        FileName = file.Name,
                    };
                    _logger.LogInformation($"Checking file {file.Name}");
                    var recordFile = filesFromRecord.Where(f => f.Name == file.Name).FirstOrDefault();
                    if (recordFile != null)
                    {
                        file.Version = recordFile.Version;
                        if (recordFile.FileHash == file.FileHash)
                        {
                            report.State = Common.Enums.FileState.Unchanged;
                            _logger.LogInformation($"File wasn't modified. Current version: {file.Version}");
                        }
                        else
                        {
                            report.State = Common.Enums.FileState.Modified;
                            file.Version++;
                            _logger.LogInformation($"File was modified. New version: {file.Version}");
                        }
                        filesFromRecord.Remove(recordFile);
                    }
                    else
                    {
                        _logger.LogInformation("This file is new or added after last analysis");
                        report.State = Common.Enums.FileState.Added;
                        file.Version = 1;
                    }
                    report.Version = file.Version;
                    _logger.LogInformation("Adding file informations to reports");
                    reports.Add(report);
                }
                foreach (var remainingRecordFile in filesFromRecord)
                {
                    _logger.LogInformation($"File {remainingRecordFile.Name} was deleted " +
                        $"on version {remainingRecordFile.Version}. Adding to reports");
                    reports.Add(new()
                    {
                        FileName = remainingRecordFile.Name,
                        Version = remainingRecordFile.Version,
                        State = Common.Enums.FileState.Deleted
                    });
                }
                return reports;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when comparing analysed files with recorded files or during reports creation", ex);
            }
        }
        private ICollection<FileReport> Convert(ICollection<FileReport> reports, ref ICollection<FileInfo> analysedFiles)
        {
            try
            {
                foreach (var file in analysedFiles)
                {
                    _logger.LogInformation($"Adding file {file.Name} to report");
                    file.Version = 1;
                    reports.Add(new()
                    {
                        FileName = file.Name,
                        State = Common.Enums.FileState.Added,
                        Version = file.Version
                    });
                }
                return reports;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when comparing analysed files with recorded files or during reports creation", ex);
            }
        }
    }
}
