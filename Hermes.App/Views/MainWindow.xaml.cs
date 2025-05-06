using Hermes.App.ViewModels;
using System.Windows;

namespace Hermes.App.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainViewModel vm = new();
        DataContext = vm;
    }
}