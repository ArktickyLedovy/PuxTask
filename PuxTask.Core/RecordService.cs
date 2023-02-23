using Microsoft.Extensions.Logging;
using PuxTask.Abstract;
using PuxTask.Common.Entities;
using PuxTask.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FileInfo = PuxTask.Common.Entities.FileInfo;

namespace PuxTask.Core
{
    internal class RecordService : IRecordService
    {
        private AnalysisRecord openedRecord;
        private readonly string recordStorage;
        private readonly ILogger<RecordService> _logger;
        //private readonly string archivesStorage;
        public RecordService(ILogger<RecordService> logger)
        {
            _logger = logger;

            //Get AppData path
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            recordStorage = Path.Combine(appdata, "Records");

            //Ensure records storage existence
            if (!Directory.Exists(recordStorage))
                Directory.CreateDirectory(recordStorage);

            _logger.LogInformation("Record service sucesfully instanciated");
        }
        #region Querry
        public bool TryGetLastRecordedFilesByAnalysedFolderPath(string analysedFolderPath, out ICollection<FileInfo>? filesFromRecord)
        {
            try
            {
                _logger.LogInformation($"Looking for analysis record of directory {analysedFolderPath}");
                analysedFolderPath = analysedFolderPath.Replace("\\", "_").Replace(":", "_");
                string recordPath = Path.Combine(recordStorage, ("record_" + analysedFolderPath + ".json"));
                if (File.Exists(recordPath))
                {
                    _logger.LogInformation($"Record found. Location: {recordPath}");
                    using (FileStream openFileStream = File.OpenRead(recordPath))
                    {
                        var record = JsonSerializer.Deserialize<AnalysisRecord>(openFileStream);
                        if (openedRecord is null || !openedRecord.Equals(record))
                        {
                            openedRecord = record;
                            _logger.LogInformation("Caching new record");
                        }
                        filesFromRecord = record.Files;
                    }
                    return true;
                }
                _logger.LogInformation($"Record not found. Application will create new record after the analysis is finished");
                filesFromRecord = null;
                return false;
            }
            catch (JsonException ex)
            {
                _logger.LogError("Unable to deserialize last record. Application will ignore it and act like there's none", ex);
                filesFromRecord = null;
                return false;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when trying to get last record for "+analysedFolderPath, ex);
            }
        }
        #endregion
        #region Command
        /// <summary>
        /// Creates new record, named with the given analysedFolderPath
        /// </summary>
        /// <param name="files">Analysed files</param>
        /// <param name="analysedFolderPath">Analysed path given by user</param>
        public void SaveRecord(ICollection<FileInfo> files, string analysedFolderPath)
        {
            try
            {
                _logger.LogInformation("Creating new analysis record for directory " + analysedFolderPath);
                analysedFolderPath = analysedFolderPath.Replace("\\", "_").Replace(":", "_");
                string recordPath = Path.Combine(recordStorage, ("record_" + analysedFolderPath + ".json"));
                var analysisRecord = new AnalysisRecord()
                {
                    Files = files,
                    AnalysedFolderPath = analysedFolderPath,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                };
                _logger.LogInformation("Record created. Location: " + recordPath);
                using (FileStream createFileStream = File.Create(recordPath))
                {
                    JsonSerializer.Serialize(createFileStream, analysisRecord);
                    _logger.LogInformation("Record filled with data and saved sucesfully");
                }
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when creating new record", ex);
            }
        }
        /// <summary>
        /// Updates opened record with given files
        /// </summary>
        /// <param name="files">Analysed files</param>
        public void SaveRecord(ICollection<FileInfo> files)
        {
            try
            {
                if (openedRecord is not null)
                {
                    var analysedFolderPath = openedRecord.AnalysedFolderPath.Replace("\\", "_").Replace(":", "_");
                    openedRecord.Files = files;
                    openedRecord.Updated = DateTime.Now;
                    string recordPath = Path.Combine(recordStorage, ("record_" + analysedFolderPath + ".json"));
                    _logger.LogInformation("Updatind analysis record. Location: " + recordPath);
                    using (FileStream writeFileStream = File.OpenWrite(recordPath))
                    {
                        writeFileStream.SetLength(0);
                        JsonSerializer.Serialize(writeFileStream, openedRecord);
                    }
                }
                else
                {
                    throw new NullCachedRecordException("Record, which was supposed to be cached, was null");
                }
            }
            catch (NullCachedRecordException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new("Something went wrong when updating record", ex);
            }
        }
        #endregion
    }
}
