using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace DBConnectionLayerFrontEnd.Commands
{
    public abstract class ViewModelBase:INotifyPropertyChanged,IDisposable
    {
        public virtual string DisplayName
        { get; protected set; }

        public virtual string CurrentStatus
        {
            get;
            protected set;
        }

        public virtual double Progress
        {
            get;
            set;
        }

        public virtual Visibility ProgressBarVisibility
        {
            get;
            set;
        }

        #region INotify property change Interface Implementation
        public event PropertyChangedEventHandler PropertyChaning;
        protected virtual void OnPropertyChaning(string propertyName)
        {
            if (this.PropertyChaning != null)
                this.PropertyChaning(this, new PropertyChangedEventArgs(propertyName));

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion

        #region IDispose Interface Implementation
        public void Dispose()
        {
            this.OnDispose();
        }

        protected virtual void OnDispose()
        {

        }

        #endregion
    }
}
