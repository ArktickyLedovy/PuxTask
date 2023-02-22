namespace PuxTask.Common.Entities
{
    public class AnalysisRecord
    {
        //public Guid ArchiveID { get; set; } = default!;
        public string RootFolderPath { get; set; } = default!;
        public DateTime Created { get; set; } = default!;
        public DateTime Updated { get; set; } = default!;
        public ICollection<FileInfo> Files { get; set; } = default!;
    }
}
