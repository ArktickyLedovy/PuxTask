using PuxTask.Abstract;
using PuxTask.Common.Entities;
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
        //private readonly string archivesStorage;
        public RecordService()
        {
            //Get AppData path
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            recordStorage = Path.Combine(appdata, "Records");

            //Ensure records storage existence
            if (!Directory.Exists(recordStorage))
                Directory.CreateDirectory(recordStorage);

            /*archivesStorage = Path.Combine(appdata, "Archives");
            //Ensure archives storage existence
            if (!Directory.Exists(archivesStorage))
                Directory.CreateDirectory(archivesStorage);*/
        }
        #region Querry
        public bool TryGetLastRecordedFilesByRootPath(string rootPath, out ICollection<FileInfo>? filesFromRecord)
        {
            string recordPath = Path.Combine(recordStorage + "record_" + rootPath + ".json");
            if (File.Exists(recordPath))
            {
                using (FileStream openFileStream = File.OpenRead(recordPath))
                {
                    filesFromRecord = JsonSerializer.Deserialize<AnalysisRecord>(openFileStream).Files;
                }
                return true;
            }
            filesFromRecord = null;
            return false;
        }
        #endregion
        #region Command
        /// <summary>
        /// Creates new record, named with the given rootPath
        /// </summary>
        /// <param name="files">Analysed files</param>
        /// <param name="rootPath">Analysed path given by user</param>
        public void SaveRecord(ICollection<FileInfo> files, string rootPath)
        {
            string recordPath = Path.Combine(recordStorage + "record_" + rootPath + ".json");
            var analysisRecord = new AnalysisRecord()
            {
                Files = files,
                RootFolderPath = rootPath,
                Created = DateTime.Now,
                Updated = DateTime.Now,
            };
            using (FileStream createFileStream = File.Create(recordPath))
            {
                JsonSerializer.Serialize(createFileStream, analysisRecord);
            }
        }
        /// <summary>
        /// Updates opened record with given files
        /// </summary>
        /// <param name="files">Analysed files</param>
        public void SaveRecord(ICollection<FileInfo> files)
        {
            if (openedRecord is not null)
            {
                openedRecord.Files = files;
                openedRecord.Updated = DateTime.Now;
                string recordPath = Path.Combine(recordStorage + "record_" + openedRecord.RootFolderPath + ".json");
                using (FileStream writeFileStream = File.OpenWrite(recordPath))
                {
                    JsonSerializer.Serialize(writeFileStream, openedRecord);
                }
            }
        }
        #endregion
    }
}
