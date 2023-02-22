using PuxTask.Common.Enums;
using System.Security.Cryptography;
using System.Text;

namespace PuxTask.Common.Entities
{
    public class FileInfo
    {
        //Empty constructor for json deserialization
        public FileInfo(){}
        public FileInfo(FileStream stream, BufferedStream bufferedStream, string analysedFolderPath)
        {
            using (var sha1 = new SHA1Managed())
            {
                FileHash = Convert.ToBase64String(sha1.ComputeHash(stream));
            }
            Path = stream.Name;
            Name = stream.Name.Replace(analysedFolderPath, "");
        }
        public string Path { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Version { get; set; } = default!;
        public string FileHash { get; set; } = default!;
    }
}
