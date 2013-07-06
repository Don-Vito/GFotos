using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel.ImageGrouping
{
    public class RedundantImagesGroup : ViewModelBase
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

        public int TotalFiles { get { return Images.Sum(image => image.FileInfos.Count); } }

        public long TotalSize { get { return Images.Sum(image => image.FileInfos.Count * image.FileSize); } }

        public string Summary
        {
            get
            {
                return string.Format("Files count: {0}  Occupied space: {1} KB", TotalFiles, TotalSize / 1024);
            }
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

            var redundantImages = images as IList<RedundantImage> ?? images.ToList();
            foreach (RedundantImage redundantImage in redundantImages)
            {
                redundantImage.FileInfos.CollectionChanged += HandleImagesChanged;
                Images.Add(redundantImage);
            }

            CleanDirectoryCommand = new RelayCommand(CleanDirectory, param => Directories.Count > 1);
            PreferDirectoryCommand = new RelayCommand(PreferDirectory, param => Directories.Count > 1);
        }

        private void HandleImagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove)
            {
                return;
            }

            foreach (var oldItem in e.OldItems)
            {
                var removedFile = oldItem as FileInfo;
                HandleFileRemoval(removedFile);
            }

            RaisePropertyChanged("TotalSize");
            RaisePropertyChanged("TotalFiles");
            RaisePropertyChanged("Summary");
        }

        private void HandleFileRemoval(FileInfo removedFile)
        {
            if (!Images.All(image => image.ContainedInDirectory(removedFile.Directory)))
            {
                Directories.Remove(directory => FileUtils.CompareDirectories(directory, removedFile.Directory) == 0);                
                CommandManager.InvalidateRequerySuggested();                
            }
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