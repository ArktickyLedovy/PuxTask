using Microsoft.Extensions.Logging;
using PuxTask.Abstract;
using PuxTask.Common.Entities;
using PuxTask.Common.Exceptions;
using System.Threading.Channels;
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
        /// <summary>
        /// Creates report for given path
        /// </summary>
        /// <param name="analysedFolderPath">path to directory given by user</param>
        /// <returns>Report</returns>
        /// <exception cref="InvalidPathException">Path is invalid</exception>
        public Report GetReport(string analysedFolderPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(analysedFolderPath))
                {
                    _logger.LogInformation($"Creating report for folder {analysedFolderPath}");
                    Report report = new(new List<FileReport>(), "Folder not analysed");
                    ICollection<FileInfo> filesFromRecord = new List<FileInfo>();
                    ICollection<FileInfo> analysedFiles = new List<FileInfo>();

                    analysedFiles = _fileService.GetFilesByPath(analysedFolderPath);
                    if (_recordService.TryGetLastRecordedFilesByAnalysedFolderPath(analysedFolderPath, out filesFromRecord))
                    {
                        _logger.LogInformation($"Record for this folder found");
                        report = CompareAndConvert(report, filesFromRecord, ref analysedFiles);
                        _recordService.SaveRecord(analysedFiles);
                        return report;
                    }
                    _logger.LogWarning($"Record for this folder not found");
                    report = Convert(report, ref analysedFiles);
                    _recordService.SaveRecord(analysedFiles, analysedFolderPath);
                    return report;
                }
                throw new InvalidPathException("Entered path was null or empty");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
        /// <summary>
        /// If record exists, compares files from record and from new analyse
        /// </summary>
        /// <param name="report">Instance of report</param>
        /// <param name="filesFromRecord">Files from record</param>
        /// <param name="analysedFiles">Newly analysed files</param>
        /// <returns>Filled report</returns>
        private Report CompareAndConvert(Report report, ICollection<FileInfo> filesFromRecord, ref ICollection<FileInfo> analysedFiles)
        {
            try
            {
                var recordedFilesCount = filesFromRecord.Count;
                var changes = false;
                _logger.LogInformation("Comparing recorded files and newly analysed files and creating report");
                foreach (var file in analysedFiles)
                {
                    var fileReport = new FileReport()
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
                            fileReport.State = Common.Enums.FileState.Unchanged;
                            _logger.LogInformation($"File wasn't modified. Current version: {file.Version}");
                        }
                        else
                        {
                            fileReport.State = Common.Enums.FileState.Modified;
                            file.Version++;
                            changes = true;
                            _logger.LogInformation($"File was modified. New version: {file.Version}");
                        }
                        filesFromRecord.Remove(recordFile);
                    }
                    else
                    {
                        _logger.LogInformation("This file is new or added after last analysis");
                        fileReport.State = Common.Enums.FileState.Added;
                        file.Version = 1;
                        changes = true;
                    }
                    fileReport.Version = file.Version;
                    _logger.LogInformation("Adding file informations to report");
                    report.FileReports.Add(fileReport);
                }
                foreach (var remainingRecordFile in filesFromRecord)
                {
                    _logger.LogInformation($"File {remainingRecordFile.Name} was deleted " +
                        $"on version {remainingRecordFile.Version}. Adding to report");
                    report.FileReports.Add(new()
                    {
                        FileName = remainingRecordFile.Name,
                        Version = remainingRecordFile.Version,
                        State = Common.Enums.FileState.Deleted
                    });
                    changes = true;
                }
                report.MessageForUser = $"Totally checked {recordedFilesCount} files from last record " +
                    $"and {analysedFiles.Count} newly analysed files. \n" +
                    $"There " + (changes ? "were some":"weren\'t any") + " changes registered and added to record";
                return report;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when comparing analysed files with recorded files or during report.FileReports creation", ex);
            }
        }
        /// <summary>
        /// If record doesn't exist, creates new report based on analyse
        /// </summary>
        /// <param name="report">Report instance</param>
        /// <param name="analysedFiles">Analysed files</param>
        /// <returns>Filled report</returns>
        private Report Convert(Report report, ref ICollection<FileInfo> analysedFiles)
        {
            try
            {
                foreach (var file in analysedFiles)
                {
                    _logger.LogInformation($"Adding file {file.Name} to report");
                    file.Version = 1;
                    report.FileReports.Add(new()
                    {
                        FileName = file.Name,
                        State = Common.Enums.FileState.Unchanged,
                        Version = file.Version
                    });
                }
                report.MessageForUser = $"Totally checked {analysedFiles.Count} newly analysed files and added to record.";
                return report;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong during report creation", ex);
            }
        }
    }
}
