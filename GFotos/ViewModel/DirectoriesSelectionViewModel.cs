using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel
{
    internal class DirectoriesSelectionViewModel : ViewModelBase
    {
        public SafeObservableCollection<DirectoryRecord> RootDirectories { get; private set; }
        public SafeObservableCollection<DirectoryRecord> ChosenDirectories { get; private set; }

        public ICommand ChooseDirectoryCommand { get; private set; }
        public ICommand UnchooseDirectoryCommand { get; private set; }
        public ICommand ClearSelectionCommand { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public DirectoriesSelectionViewModel()
        {
            var rootDirectories = Directory.GetLogicalDrives().Select(
                folder => CreateDirectoryRecord(new DirectoryInfo(folder)));

            IsEnabled = true;
            RootDirectories = new SafeObservableCollection<DirectoryRecord>();
            RootDirectories.AddRange(rootDirectories);
            if (RootDirectories.Any())
            {
                RootDirectories.First().Selected = true;
            }

            ChooseDirectoryCommand = new RelayCommand(param => { ((DirectoryRecord)param).Chosen = true; });
            UnchooseDirectoryCommand = new RelayCommand(param => { ((DirectoryRecord)param).Chosen = false; });
            ChosenDirectories = new SafeObservableCollection<DirectoryRecord>();

            ClearSelectionCommand = new RelayCommand(param => ClearSelectedCommands(), param => ChosenDirectories.Any());
        }

        private DirectoryRecord CreateDirectoryRecord(DirectoryInfo directoryInfo)
        {
            var directoryRecord = new DirectoryRecord(directoryInfo);
            directoryRecord.PropertyChanged += HandleDirectoryPropertyChanged;
            return directoryRecord;
        }

        private void HandleDirectoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Chosen")
            {
                var directoryRecord = sender as DirectoryRecord;
                Debug.Assert(directoryRecord != null, "directoryRecord != null");
                if (directoryRecord.Chosen)
                {
                    ChosenDirectories.Add(directoryRecord);
                }
                else
                {
                    ChosenDirectories.Remove(directoryRecord);
                }
            }
        }

        private void ClearSelectedCommands()
        {
            foreach (DirectoryRecord chosenDirectory in ChosenDirectories.ToList())
            {
                chosenDirectory.Checked = false;
                chosenDirectory.Chosen = false;
            }
        }
    }
}