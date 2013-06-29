using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    internal class ImagesGrouper
    {
        public static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg" };

        public static IEnumerable<ImagesGroup> GroupImages(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            IEnumerable<KeyValuePair<string, List<FileInfo>>> sameFilesDictionary = FindSameFiles(chosenDirectories);
            IEnumerable<KeyValuePair<HashSet<ComparableDirectoryInfo>, List<FileInfo>>> groupedDirectoriesDictionary = 
                GroupDirectoriesWithSameFiles(sameFilesDictionary);
            
            return groupedDirectoriesDictionary.Select(keyValuePair => CreateGroup(keyValuePair.Key, keyValuePair.Value));
        }

        private static IEnumerable<KeyValuePair<HashSet<ComparableDirectoryInfo>, List<FileInfo>>> GroupDirectoriesWithSameFiles(
            IEnumerable<KeyValuePair<string, List<FileInfo>>> sameFilesDictionary)
        {
            var sameDirectoryGroupsDictionary = new Dictionary<HashSet<ComparableDirectoryInfo>, List<FileInfo>>(
                HashSet<ComparableDirectoryInfo>.CreateSetComparer());
            
            foreach (KeyValuePair<string, List<FileInfo>> keyValuePair in sameFilesDictionary)
            {
                IList<FileInfo> fileInfos = keyValuePair.Value;
                var directories = new HashSet<ComparableDirectoryInfo>(fileInfos.Select(fileInfo => new ComparableDirectoryInfo(fileInfo.Directory)));                

                if (!sameDirectoryGroupsDictionary.ContainsKey(directories))
                {
                    sameDirectoryGroupsDictionary[directories] = new List<FileInfo>();
                }
                sameDirectoryGroupsDictionary[directories].AddRange(fileInfos);
            }

            return sameDirectoryGroupsDictionary;
        }

        private static ImagesGroup CreateGroup(IEnumerable<ComparableDirectoryInfo> directories, IList<FileInfo> files)
        {
            IDictionary<DirectoryInfo, IEnumerable<FileInfo>> filesByDirectoriesDictionary = directories.ToDictionary(
                directory => directory.Info,
                directory => files.Where(file => IsParentDirectory(file, directory)));

            return new ImagesGroup(filesByDirectoriesDictionary);
        }

        private static bool IsParentDirectory(FileInfo file, ComparableDirectoryInfo directory)
        {
            Debug.Assert(file.Directory != null, "file.Directory != null");
            return directory.Equals(new ComparableDirectoryInfo(file.Directory));
        }

        private static IEnumerable<KeyValuePair<string, List<FileInfo>>> FindSameFiles(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            var sameFilesDictionary = new Dictionary<string, List<FileInfo>>();
            foreach (DirectoryRecord chosenDirectory in chosenDirectories)
            {
                ScanDirectory(chosenDirectory, sameFilesDictionary);
            }
            return sameFilesDictionary.Where(keyValuePair => keyValuePair.Value.Count > 1);
        }

        private static void ScanDirectory(DirectoryRecord chosenDirectory, Dictionary<string, List<FileInfo>> sameFilesDictionary)
        {
            var filteredFiles = chosenDirectory.Files.Where(fileInfo => AllowedExtensions.Contains(fileInfo.Extension.ToLower()));
            foreach (FileInfo fileInfo in filteredFiles)
            {
                try
                {
                    string hash = FileUtils.ComputeHash(fileInfo);

                    if (!sameFilesDictionary.ContainsKey(hash))
                    {
                        sameFilesDictionary[hash] = new List<FileInfo>();
                    }

                    sameFilesDictionary[hash].Add(fileInfo);
                }
                catch (IOException)
                {
                }
            }

            foreach (DirectoryRecord directoryRecord in chosenDirectory.Directories)
            {
                ScanDirectory(directoryRecord, sameFilesDictionary);
            }
        }
    }
}