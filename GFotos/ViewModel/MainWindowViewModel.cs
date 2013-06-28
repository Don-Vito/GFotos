using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public string Title { get; private set; }
        public SafeObservableCollection<DirectoryRecord> RootDirectories { get; private set; }
        public SafeObservableCollection<DirectoryRecord> ChosenDirectories { get; private set; }

        public ICommand ChooseDirectoryCommand { get; private set; }
        public ICommand UnchooseDirectoryCommand { get; private set; }
        public ICommand CleanupCommand { get; private set; }

        public MainWindowViewModel()
        {
            Title = "GFotos";
            
            var rootDirectories = Directory.GetLogicalDrives().Select(
                folder => CreateDirectoryRecord(new DirectoryInfo(folder)));

            RootDirectories = new SafeObservableCollection<DirectoryRecord>();
            RootDirectories.AddRange(rootDirectories);

            ChooseDirectoryCommand = new RelayCommand(param => { ((DirectoryRecord) param).Chosen = true; });
            UnchooseDirectoryCommand = new RelayCommand(param => { ((DirectoryRecord)param).Chosen = false; });
            ChosenDirectories = new SafeObservableCollection<DirectoryRecord>();

            CleanupCommand = new RelayCommand(param => Cleanup(), param => ChosenDirectories.Any());
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
            throw new System.NotImplementedException();
        }
    }
}