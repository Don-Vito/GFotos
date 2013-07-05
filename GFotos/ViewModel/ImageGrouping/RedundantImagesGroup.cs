using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    class RedundantImagesGroup : ViewModelBase
    {
        private readonly int _id;
        private static int _currentId;
        public SafeObservableCollection<DirectoryInfo> Directories { get; private set; }        
        public SafeObservableCollection<RedundantImage> Images { get; private set; } 
        
        public ICommand CleanDirectoryCommand { get; private set; }
        public ICommand PreferDirectoryCommand { get; private set; }
        
        public string Title
        {
            get { return string.Format("Images group {0}", _id);  }
        }

        public static RedundantImagesGroup Create(IEnumerable<DirectoryInfo> directories,
                                                  IEnumerable<RedundantImage> images)
        {
            return new RedundantImagesGroup(directories, images, _currentId++);
        }

        private RedundantImagesGroup(IEnumerable<DirectoryInfo> directories, IEnumerable<RedundantImage> images, int id)
        {
            _id = id;
            Directories = SafeObservableCollection<DirectoryInfo>.Create();
            Directories.AddRange(directories);

            Images = SafeObservableCollection<RedundantImage>.Create();
            Images.AddRange(images);

            CleanDirectoryCommand = new RelayCommand(CleanDirectory, param => Directories.Count > 1);
            PreferDirectoryCommand = new RelayCommand(PreferDirectory, param => Directories.Count > 1);
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

        public void PreferDirectory(object obj)
        {
            var preferedDirectoryInfo = obj as DirectoryInfo;
            Debug.Assert(preferedDirectoryInfo != null, "directoryInfo != null");

            foreach (DirectoryInfo directory in Directories.ToList())
            {
                if (FileUtils.CompareDirectories(preferedDirectoryInfo, directory) != 0)
                {
                    CleanDirectory(directory);
                }
            }
                             
            CommandManager.InvalidateRequerySuggested();
        }
    }
}