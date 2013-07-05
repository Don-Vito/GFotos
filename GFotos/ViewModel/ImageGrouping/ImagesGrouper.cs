using System.Collections.Generic;
using System.IO;
using System.Linq;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    internal class ImagesGrouper
    {
        public static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg" };

        public static IEnumerable<RedundantImagesGroup> GroupImages(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            IEnumerable<RedundantImage> redundantImages = FindRedundantImages(chosenDirectories);
            IEnumerable<KeyValuePair<IEnumerable<DirectoryInfo>, IEnumerable<RedundantImage>>> groupedDirectoriesDictionary = 
                GroupDirectoriesWithSameRedundantImages(redundantImages);
            
            return groupedDirectoriesDictionary.Select(
                keyValuePair => RedundantImagesGroup.Create(keyValuePair.Key, keyValuePair.Value));
        }

        private static IEnumerable<KeyValuePair<IEnumerable<DirectoryInfo>, IEnumerable<RedundantImage>>> 
            GroupDirectoriesWithSameRedundantImages(IEnumerable<RedundantImage> redundantImages)
        {
            var sameDirectoryGroupsDictionary = new Dictionary<HashSet<ComparableDirectoryInfo>, IList<RedundantImage>>(
                HashSet<ComparableDirectoryInfo>.CreateSetComparer());
            
            foreach (RedundantImage redundantImage in redundantImages)
            {
                IList<FileInfo> fileInfos = redundantImage.FileInfos.ToList();
                var directories = new HashSet<ComparableDirectoryInfo>(fileInfos.Select(fileInfo => new ComparableDirectoryInfo(fileInfo.Directory)));                

                if (!sameDirectoryGroupsDictionary.ContainsKey(directories))
                {
                    sameDirectoryGroupsDictionary[directories] = new List<RedundantImage>();
                }
                sameDirectoryGroupsDictionary[directories].Add(redundantImage);
            }

            return sameDirectoryGroupsDictionary.Select(
                keyValuePair => new KeyValuePair<IEnumerable<DirectoryInfo>, IEnumerable<RedundantImage>>(
                    keyValuePair.Key.Select(comparableDirectoryInfo => comparableDirectoryInfo.Info),
                    keyValuePair.Value));
        }

        private static IEnumerable<RedundantImage> FindRedundantImages(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            var sameFilesDictionary = new Dictionary<string, List<FileInfo>>();
            foreach (DirectoryRecord chosenDirectory in chosenDirectories)
            {
                ScanDirectory(chosenDirectory, sameFilesDictionary);
            }
            
            return sameFilesDictionary.Values.Where(fileInfos => fileInfos.Count > 1).
                Select(fileInfos => new RedundantImage(fileInfos));
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