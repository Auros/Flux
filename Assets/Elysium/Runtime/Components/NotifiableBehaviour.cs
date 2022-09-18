﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Elysium.Components
{
    /// <summary>
    /// A MonoBehaviour which implements INotifyPropertyChanged
    /// </summary>
    [PublicAPI]
    public class NotifiableBehaviour : MonoBehaviour, INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;
        
        protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
            => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            
            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}