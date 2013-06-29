using System;
using System.IO;
using System.Security.Cryptography;

namespace GFotos.Framework
{
    internal class FileUtils
    {
        public static string ComputeHash(FileSystemInfo fileSystemInfo)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileSystemInfo.FullName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public static int CompareDirectories(DirectoryInfo directoryInfo, DirectoryInfo otherDirectoryInfo)
        {
            return String.Compare(
                directoryInfo.FullName.ToLower(), 
                otherDirectoryInfo.FullName.ToLower(), 
                StringComparison.Ordinal);
        }
    }
}