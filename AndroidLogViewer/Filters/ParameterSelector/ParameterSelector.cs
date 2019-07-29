using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AndroidLogViewer.Annotations;

namespace AndroidLogViewer.Filters.ParameterSelector
{
    public abstract class ParameterSelector<TParameter> : IParameterSelector<TParameter>
    {
        private bool isParameterValid;
        private TParameter parameter;

        protected class OnParameterValueChangingArgs : EventArgs
        {
            public TParameter OldValue { get; }

            public TParameter NewValue { get; }

            public bool Cancel { get; set; } = false;

            public OnParameterValueChangingArgs(TParameter oldValue, TParameter newValue)
            {
                OldValue = oldValue;
                NewValue = newValue;
            }
        }

        protected EventHandler<OnParameterValueChangingArgs> OnParameterValueChanging;
        protected EventHandler<EventArgs> OnParameterValueChanged;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public TParameter Parameter
        {
            get => parameter;
            protected set
            {
                if (OnParameterValueChanging != null)
                {
                    var args = new OnParameterValueChangingArgs(parameter, value);
                    OnParameterValueChanging(this, args);
                    if (args.Cancel)
                    {
                        OnPropertyChanged();
                        return;
                    }
                }

                if (Equals(value, parameter)) return;
                parameter = value;
                OnPropertyChanged();
                
                OnParameterValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool IsParameterValid
        {
            get => isParameterValid;
            protected set
            {
                if (value == isParameterValid) return;
                isParameterValid = value;
                OnPropertyChanged();
            }
        }
    }
}