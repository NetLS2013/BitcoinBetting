﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Core.ViewModels.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, Action action = null, IEnumerable<string> additionalprops = null, [CallerMemberName] string propertyName = null)
        {

            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            //Check for related fields
            if (additionalprops != null)
            {
                foreach (var s in additionalprops)
                    OnPropertyChanged(s);
            };
            //Fire any post set action that was supplied
            if (action != null) action();
            return true;
        }

        //Standard IsBusy property
        private bool _isbusy;
        public bool IsBusy
        {
            get { return this._isbusy; }
            set { SetField(ref this._isbusy, value); }
        }

    }

    public class ReadOnlyWrapperViewModel<T> : BaseViewModel
    {
        private T _item;
        public ReadOnlyWrapperViewModel(T item)
        {
            Item = item;
        }

        public T Item { get { return this._item; } private set { SetField(ref this._item, value); } }
    }
    public class ReadOnlySelectable<T> : BaseViewModel
    {

        public ReadOnlySelectable(T item)
        {
            Item = item;
        }
        public T Item { get; private set; }

        private bool _selected;

        public bool Selected
        {
            get { return this._selected; }
            set { SetField(ref this._selected, value); }
        }

    }
}

