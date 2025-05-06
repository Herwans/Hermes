using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Hermes.App.ViewModels.Components
{
    public partial class DirectoryItemViewModel: ObservableObject
    {
        public string SourcePath { get; }
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        public bool isDirectory;

        [ObservableProperty]
        private bool isChecked = false;

        public DirectoryItemViewModel(string path)
        {
            SourcePath = path;
            Name = Path.GetFileName(path);
            IsDirectory = Directory.Exists(path);
        }

    }
}
