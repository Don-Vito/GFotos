using System.Collections.Generic;
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
            ImagesGroups.AddRange(imagesGroups);

            if (ImagesGroups.Any())
            {
                SelectedImagesGroup = ImagesGroups.First();
            }

            GlobalPreferDirectoryCommand = new RelayCommand(GlobalPreferDirectory, param => ImagesGroups.Any());
        }

        private void GlobalPreferDirectory(object obj)
        {
            var directoryInfo = obj as DirectoryInfo;

            foreach (RedundantImagesGroup redundantImagesGroup in ImagesGroups)
            {
                redundantImagesGroup.PreferDirectory(directoryInfo);
            }
        }

    }
}