using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nearm.ViewModels.Base;

internal class BaseNotifyPropertyChanged : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string callerMemberName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(callerMemberName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string callerMemberName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
    }
}
