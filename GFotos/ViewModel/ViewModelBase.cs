using System.ComponentModel;

namespace GFotos.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void RaiseChildPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, eventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
