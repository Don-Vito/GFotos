using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;
using GFotos.ViewModel.ImageGrouping;

namespace GFotos.ViewModel
{
    internal class SearchResultsViewModel : ViewModelBase
    {
        public SafeObservableCollection<RedundantImagesGroup> ImagesGroups { get; private set; }
        public ICommand GlobalPreferDirectoryCommand { get; private set; }

        private RedundantImagesGroup _selectedImagesGroup;
        public RedundantImagesGroup SelectedImagesGroup
        {
            get { return _selectedImagesGroup; }
            set
            {
                if (_selectedImagesGroup != value)
                {
                    _selectedImagesGroup = value;
                    RaisePropertyChanged("SelectedImagesGroup");
                }
            }
        }

        public SearchResultsViewModel(IEnumerable<RedundantImagesGroup> imagesGroups)
        {
            ImagesGroups = new SafeObservableCollection<RedundantImagesGroup>();

            var redundantImagesGroups = imagesGroups as IList<RedundantImagesGroup> ?? imagesGroups.ToList();
            foreach (RedundantImagesGroup redundantImagesGroup in redundantImagesGroups)
            {
                redundantImagesGroup.PropertyChanged += HandleGroupChanged;
                ImagesGroups.Add(redundantImagesGroup);
            }

            if (ImagesGroups.Any())
            {
                SelectedImagesGroup = ImagesGroups.First();
            }

            GlobalPreferDirectoryCommand = new RelayCommand(GlobalPreferDirectory, param => ImagesGroups.Any());
        }

        private void HandleGroupChanged(object sender, PropertyChangedEventArgs e)
        {
            var redundantImagesGroup = sender as RedundantImagesGroup;
            if (redundantImagesGroup == null)
            {
                return;
            }

            if (e.PropertyName == "IsClean")
            {
                ImagesGroups.Remove(redundantImagesGroup);
            }
        }

        private void GlobalPreferDirectory(object obj)
        {
            var directoryInfo = obj as DirectoryInfo;

            // We need to enumerate over a copy of the ImagesGroup,
            // since some group might be cleaned by Global Prefer and as a result removed from the list
            foreach (RedundantImagesGroup redundantImagesGroup in ImagesGroups.ToList())
            {
                redundantImagesGroup.PreferDirectory(directoryInfo);
            }
        }
    }
}