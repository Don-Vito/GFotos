using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    internal class RedundantImage : ViewModelBase
    {
        public SafeObservableCollection<FileInfo> FileInfos { get; private set; }
        public ICommand RemoveFileCommand { get; private set; }

        public Uri ImageUri
        {
            get
            {                
                return new Uri(FileInfos.First().FullName);
            }
        }

        public RedundantImage(IEnumerable<FileInfo> fileInfos)
        {
            FileInfos = SafeObservableCollection<FileInfo>.Create();
            FileInfos.AddRange(fileInfos);
            
            RemoveFileCommand = new RelayCommand(RemoveFile, param => FileInfos.Count > 1);
        }

        private void RemoveFile(object obj)
        {
            var fileInfo = obj as FileInfo;
            Debug.Assert(fileInfo != null, "fileInfo != null");
            
            //fileInfo.Delete();
            FileInfos.Remove(fileInfo);
        }

        public void RemoveFilesByDirectory(DirectoryInfo directoryInfo)
        {
            FileInfos.Remove(fileInfo => FileUtils.CompareDirectories(fileInfo.Directory, directoryInfo) == 0);
        }
    }
}