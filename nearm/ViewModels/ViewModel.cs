using nearm.ViewModels.Base;

namespace nearm.ViewModels;

internal class ViewModel : BaseNotifyPropertyChanged
{
    private string title = "nearm";
    public string Title { get => title; set => Set(ref title, value); }
}
