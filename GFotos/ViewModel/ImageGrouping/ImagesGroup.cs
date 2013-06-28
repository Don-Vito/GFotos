using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    class ImagesGroup : ViewModelBase
    {
        private readonly IDictionary<DirectoryInfo, IEnumerable<FileInfo>> _groupingDictionary;
        public ICommand CleanDirectoryCommand;

        public IEnumerable<DirectoryInfo> Directories
        {
            get { return _groupingDictionary.Keys; }
        }

        public IEnumerable<Uri> ImageUris
        {
            get
            {
                if (_groupingDictionary.Any())
                {
                    IEnumerable<FileInfo> files = _groupingDictionary.Values.Take(1).ElementAt(0);
                    return files.Select(fileInfo => new Uri(fileInfo.FullName));    
                }
                
                return new List<Uri>();
            }
        }
       
        public ImagesGroup(IDictionary<DirectoryInfo, IEnumerable<FileInfo>> groupingDictionary)
        {
            _groupingDictionary = groupingDictionary;
            CleanDirectoryCommand = new RelayCommand(CleanDirectory);
        }

        private void CleanDirectory(object obj)
        {
            var directoryInfo = obj as DirectoryInfo;
            if (_groupingDictionary.ContainsKey(directoryInfo))
            {
                IEnumerable<FileInfo> filesToDelete = _groupingDictionary[directoryInfo];
                foreach (FileInfo fileInfo in filesToDelete)
                {
                      fileInfo.Delete();
                }

                _groupingDictionary.Remove(directoryInfo);
                RaisePropertyChanged("Directories");
                RaisePropertyChanged("ImageUris");
            }
        }
    }
}