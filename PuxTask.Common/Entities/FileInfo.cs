using PuxTask.Common.Enums;
using System.Security.Cryptography;

namespace PuxTask.Common.Entities
{
    public class FileInfo
    {
        public FileInfo(FileStream stream, string rootPath)
        {
            HashAlgorithm sha1 = HashAlgorithm.Create();
            Path = stream.Name;
            Name = stream.Name.Replace(rootPath, "");
            FileHash = sha1.ComputeHash(stream);
        }
        public string Path { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Version { get; set; } = default!;
        public byte[] FileHash { get; set; } = default!;
    }
}
