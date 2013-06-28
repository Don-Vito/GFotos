using System.Collections.Generic;
using System.IO;

namespace GFotos.ViewModel.ImageGrouping
{
    internal class ImagesGrouper
    {        
        public static IEnumerable<ImagesGroup> GroupImages(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            IDictionary<int, IList<FileInfo>> sameFilesDictionary = GroupFiles(chosenDirectories);
            return GroupDirectories(sameFilesDictionary);            
        }

        private static IEnumerable<ImagesGroup> GroupDirectories(IDictionary<int, IList<FileInfo>> sameFilesDictionary)
        {            
            throw new System.NotImplementedException();
        }

        private static IDictionary<int, IList<FileInfo>> GroupFiles(IEnumerable<DirectoryRecord> chosenDirectories)
        {
            var sameFilesDictionary = new Dictionary<int, IList<FileInfo>>();
            foreach (DirectoryRecord chosenDirectory in chosenDirectories)
            {
                ScanDirectory(chosenDirectory, sameFilesDictionary);
            }
            return sameFilesDictionary;
        }

        private static void ScanDirectory(DirectoryRecord chosenDirectory, Dictionary<int, IList<FileInfo>> sameFilesDictionary)
        {
            foreach (FileInfo fileInfo in chosenDirectory.Files)
            {
                try
                {
                    int crc = ComputeCrc(fileInfo);

                    if (!sameFilesDictionary.ContainsKey(crc))
                    {
                        sameFilesDictionary[crc] = new List<FileInfo>();
                    }

                    sameFilesDictionary[crc].Add(fileInfo);
                }
                catch (IOException)
                {
                }
            }

            foreach (DirectoryRecord directoryInfo in chosenDirectory.Directories)
            {
                ScanDirectory(directoryInfo, sameFilesDictionary);
            }
        }

        private static int ComputeCrc(FileInfo fileInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}