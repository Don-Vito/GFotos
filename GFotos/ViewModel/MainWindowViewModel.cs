using System.Collections.Generic;
using System.ComponentModel;
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

        public SearchResultsViewModel SearchResults { get; private set; }
        public DirectoriesSelectionViewModel DirectoriesSelection { get; private set; }

        public ICommand CleanupCommand { get; private set; }
        public ICommand CancelCleanupCommand { get; private set; }

        private bool _isSearching;
        public bool IsSearching
        {
            get { return _isSearching; } 
            set
            {
                if (_isSearching != value)
                {
                    _isSearching = value;
                    DirectoriesSelection.IsEnabled = !_isSearching;
                    RaisePropertyChanged("IsSearching");
                }
            }
        }       

        public MainWindowViewModel()
        {
            Title = "GFotos";
            IsSearching = false;

            CleanupCommand = new RelayCommand(param => Cleanup(), param => DirectoriesSelection.ChosenDirectories.Any() && !IsSearching);
            CancelCleanupCommand = new RelayCommand(param => CancelCleanup(), param => CanCancelCleanup());
            DirectoriesSelection = new DirectoriesSelectionViewModel(CleanupCommand) {IsSelected = true};

            _groupingBackgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _groupingBackgroundWorker.DoWork += RunGroupingHandler;
            _groupingBackgroundWorker.RunWorkerCompleted += GroupingCompletedHandler;
        }      

        private void Cleanup()
        {
            IsSearching = true;
            SearchResults = null;
            RaisePropertyChanged("SearchResults");
            _groupingBackgroundWorker.RunWorkerAsync();
        }

        private void GroupingCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            IsSearching = false;

            if (e.Cancelled)
            {
                return;
            }

            var imagesGroups = e.Result as IEnumerable<RedundantImagesGroup>;
            SearchResults = new SearchResultsViewModel(imagesGroups);
            RaisePropertyChanged("SearchResults");         
            
            // The cancel command is not disabled without this line
            // Since it is a rare transition we are ok to initiate commands predicates reevaluation
            CommandManager.InvalidateRequerySuggested();
        }

        private void RunGroupingHandler(object sender, DoWorkEventArgs e)
        {
            e.Result = ImagesGrouper.GroupImages(DirectoriesSelection.ChosenDirectories);
        }

        private void CancelCleanup()
        {
            _groupingBackgroundWorker.CancelAsync();
        }

        private bool CanCancelCleanup()
        {
            return _groupingBackgroundWorker.IsBusy;
        }        
    }
}