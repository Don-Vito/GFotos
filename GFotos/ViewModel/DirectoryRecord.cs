using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Input;
using GFotos.Framework;

namespace GFotos.ViewModel
{
    class DirectoryRecord : ViewModelBase
    {
        public DirectoryInfo Info { get; private set; }
        public ICommand AlternateCheckedCommand { get; private set; }
        private IEnumerable<FileInfo> _files;
        private IList<DirectoryRecord> _directoryRecords;

        #region Selected
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected == value) return;

                _selected = value;
                RaisePropertyChanged("Selected");
            }
        }
        #endregion Selected

        #region Enabled
        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                
                _enabled = value;
                RaisePropertyChanged("Enabled");
            }
        }
        #endregion Enabled

        #region Chosen
        private bool _chosen;
        public bool Chosen
        {
            get { return _chosen; }
            set
            {
                if (_chosen == value) return;

                _chosen = value;
                RaisePropertyChanged("Chosen");
            }
        }
        #endregion Chosen

        #region Checked
        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value) return;
                
                _checked = value;
                RaisePropertyChanged("Checked");
                UpdateChildren();
            }
        }

        private void UpdateChildren()
        {
            if (_directoryRecords == null) return;
            
            foreach (DirectoryRecord directoryRecord in _directoryRecords)
            {
                directoryRecord.Enabled = Enabled && !Checked;
                directoryRecord.Checked = Checked;
                directoryRecord.Chosen = false;
            }
        }

        #endregion Checked

        #region Files
        public IEnumerable<FileInfo> Files
        {
            get { return _files ?? (_files = Info.GetFiles()); }
        }
        #endregion Files

        #region Directories
        public IEnumerable<DirectoryRecord> Directories
        {
            get { return _directoryRecords ?? (_directoryRecords = GetDirectories()); }
        }

        private IList<DirectoryRecord> GetDirectories()
        {
            try
            {
                return Info.GetDirectories("*", SearchOption.TopDirectoryOnly).Select(CreateDirectoryRecord).ToList();
            }
            catch (Exception exception)
            {
                if (exception is UnauthorizedAccessException || exception is IOException)
                {
                    return null;
                }
                throw;
            }
        }

        private DirectoryRecord CreateDirectoryRecord(DirectoryInfo directoryInfo)
        {
            var directoryRecord = new DirectoryRecord(directoryInfo);
            directoryRecord.PropertyChanged += RaiseChildPropertyChanged;
            return directoryRecord;
        }

        #endregion Directories

        public DirectoryRecord(DirectoryInfo info, bool enabled = true)
        {
            Info = info;
            Enabled = enabled;
            Checked = false;
            Chosen = false;
            Selected = false;

            AlternateCheckedCommand = new RelayCommand(param => { Checked = !_checked; }, param => Enabled);
        }
    }
}
