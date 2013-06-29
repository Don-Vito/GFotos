using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    class RedundantImagesGroup : ViewModelBase
    {
        public SafeObservableCollection<DirectoryInfo> Directories { get; private set; }        
        public SafeObservableCollection<RedundantImage> Images { get; private set; } 
        public ICommand CleanDirectoryCommand { get; private set; }
        
        public RedundantImagesGroup(IEnumerable<DirectoryInfo> directories, IEnumerable<RedundantImage> images)
        {            
            Directories = new SafeObservableCollection<DirectoryInfo>();
            Directories.AddRange(directories);

            Images = new SafeObservableCollection<RedundantImage>();
            Images.AddRange(images);
            
            CleanDirectoryCommand = new RelayCommand(CleanDirectory, param=>Directories.Count > 1);
        }

        private void CleanDirectory(object obj)
        {
            var directoryInfo = obj as DirectoryInfo;
            Debug.Assert(directoryInfo != null, "directoryInfo != null");

            foreach (RedundantImage redundantImage in Images)
            {
                redundantImage.RemoveFilesByDirectory(directoryInfo);
            }
            
            Directories.Remove(directoryInfo);            
            CommandManager.InvalidateRequerySuggested();
        }
    }
}