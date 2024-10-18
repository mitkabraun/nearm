using System.Windows;

using nearm.ViewModels;
using nearm.Views;

namespace nearm;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        new MainWindow { DataContext = new ViewModel() }.Show();
    }
}
