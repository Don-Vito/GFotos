using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;
using GFotos.ViewModel.ImageGrouping;

namespace GFotos.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly BackgroundWorker _groupingBackgroundWorker;
        public string Title { get; private set; }
        public SafeObservableCollection<DirectoryRecord> RootDirectories { get; private set; }
        public SafeObservableCollection<DirectoryRecord> ChosenDirectories { get; private set; }
        public SafeObservableCollection<RedundantImagesGroup> ImagesGroups { get; private set; }

        public ICommand ChooseDirectoryCommand { get; private set; }
        public ICommand UnchooseDirectoryCommand { get; private set; }
        public ICommand CleanupCommand { get; private set; }
        public ICommand CancelCleanupCommand { get; private set; }
        public ICommand ClearSelectionCommand { get; private set; }
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

        private bool _directoriesSelectionEnabled;
        public bool DirectoriesSelectionEnabled
        {
            get { return _directoriesSelectionEnabled; } 
            set
            {
                if (_directoriesSelectionEnabled != value)
                {
                    _directoriesSelectionEnabled = value;
                    RaisePropertyChanged("DirectoriesSelectionEnabled");
                }
            }
        }       

        public MainWindowViewModel()
        {
            Title = "GFotos";
            DirectoriesSelectionEnabled = true;
            SelectedImagesGroup = null;
            ImagesGroups = new SafeObservableCollection<RedundantImagesGroup>();

            _groupingBackgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _groupingBackgroundWorker.DoWork += RunGroupingHandler;
            _groupingBackgroundWorker.RunWorkerCompleted += GroupingCompletedHandler;

            var rootDirectories = Directory.GetLogicalDrives().Select(
                folder => CreateDirectoryRecord(new DirectoryInfo(folder)));

            RootDirectories = new SafeObservableCollection<DirectoryRecord>();
            RootDirectories.AddRange(rootDirectories);

            ChooseDirectoryCommand = new RelayCommand(param => { ((DirectoryRecord) param).Chosen = true; });
            UnchooseDirectoryCommand = new RelayCommand(param => { ((DirectoryRecord)param).Chosen = false; });
            ChosenDirectories = new SafeObservableCollection<DirectoryRecord>();

            CleanupCommand = new RelayCommand(param => Cleanup(), param => ChosenDirectories.Any() && DirectoriesSelectionEnabled);
            CancelCleanupCommand = new RelayCommand(param => CancelCleanup(), param => CanCancelCleanup());

            ClearSelectionCommand = new RelayCommand(param => ClearSelectedCommands(), param => ChosenDirectories.Any());

            GlobalPreferDirectoryCommand = new RelayCommand(GlobalPreferDirectory, param => ImagesGroups.Any());
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

        private void Cleanup()
        {
            DirectoriesSelectionEnabled = false;
            ImagesGroups.Clear();
            _groupingBackgroundWorker.RunWorkerAsync();
        }

        private void GroupingCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            DirectoriesSelectionEnabled = true;

            if (ImagesGroups.Any())
            {
                SelectedImagesGroup = ImagesGroups.First();
            }
            
            // The cancel command is not disabled without this line
            // Since it is a rare transition we are ok to initiate commands predicates reevaluation
            CommandManager.InvalidateRequerySuggested();
        }

        private void RunGroupingHandler(object sender, DoWorkEventArgs e)
        {
            IEnumerable<RedundantImagesGroup> imagesGroups = ImagesGrouper.GroupImages(ChosenDirectories);
            ImagesGroups.AddRange(imagesGroups);            
        }

        private void CancelCleanup()
        {
            _groupingBackgroundWorker.CancelAsync();
        }

        private bool CanCancelCleanup()
        {
            return _groupingBackgroundWorker.IsBusy;
        }

        private void ClearSelectedCommands()
        {
            
            foreach (DirectoryRecord chosenDirectory in ChosenDirectories.ToList())
            {
                chosenDirectory.Selected = false;
                chosenDirectory.Chosen = false;
            }
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