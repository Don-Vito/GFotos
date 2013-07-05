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
        public ICommand CleanLocationCommand { get; private set; }
        public ICommand PreferLocationCommand { get; private set; }

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

            CleanLocationCommand = new RelayCommand(CleanLocation, param => FileInfos.Count > 1);
            PreferLocationCommand = new RelayCommand(PreferLocation, param => FileInfos.Count > 1);
        }

        public void RemoveFilesByDirectory(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo fileInfo in FileInfos.ToList())
            {
                if (FileUtils.CompareDirectories(fileInfo.Directory, directoryInfo) == 0)
                {
                    CleanLocation(fileInfo);
                }
            }
        }

        private void CleanLocation(object obj)
        {
            var fileInfo = obj as FileInfo;
            Debug.Assert(fileInfo != null, "directoryInfo != null");

            if (FileInfos.Count > 1)
            {
                FileInfos.Remove(fileInfo);
                CommandManager.InvalidateRequerySuggested();
            }            
        }

        public void PreferLocation(object obj)
        {
            var preferedFileInfo = obj as FileInfo;
            Debug.Assert(preferedFileInfo != null, "directoryInfo != null");

            foreach (FileInfo fileInfo in FileInfos.ToList())
            {
                if (!preferedFileInfo.FullName.ToLower().Equals(fileInfo.ToString().ToLower()))
                {
                    CleanLocation(fileInfo);
                }
            }

            CommandManager.InvalidateRequerySuggested();
        }

        public bool ContainedInDirectory(DirectoryInfo directory)
        {
            return FileInfos.Any(fileInfo => FileUtils.CompareDirectories(fileInfo.Directory, directory) == 0);
        }
    }
}