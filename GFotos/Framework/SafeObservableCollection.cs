using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace GFotos.Framework
{
    public class SafeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _currentDispatcher;

        public static SafeObservableCollection<T> Create()
        {
            SafeObservableCollection<T> result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                result = new SafeObservableCollection<T>();                
            });

            return result;
        }

        public SafeObservableCollection()
        {            
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }        
 
        private void DoDispatchedAction(Action action)
        {
            if (_currentDispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                _currentDispatcher.Invoke(DispatcherPriority.DataBind, action);
            }
        }
 
        protected override void ClearItems()
        {
            DoDispatchedAction(() => base.ClearItems());
        }
  
        protected override void InsertItem(int index, T item)
        {
            DoDispatchedAction(() => base.InsertItem(index, item));
        }
  
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            DoDispatchedAction(() => base.MoveItem(oldIndex, newIndex));
        }
       
        /// <summary> 
        /// Removes the item at the specified index 
        /// </summary> 
        ///<param name="index">The index of the item which should be removed</param> 
        protected override void RemoveItem(int index)
        {
            DoDispatchedAction(() => base.RemoveItem(index));
        }
    
        protected override void SetItem(int index, T item)
        {
            DoDispatchedAction(() => base.SetItem(index, item));
        }
 
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            DoDispatchedAction(() => base.OnCollectionChanged(e));
        }
 
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            DoDispatchedAction(() => base.OnPropertyChanged(e));
        } 
      
        public void AddRange(IEnumerable<T> items)
        {
            DoDispatchedAction(() => AddRangeInThread(items));            
        }

        private void AddRangeInThread(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Items.Add(item);                
            }
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
