using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hermes.App.Helpers;
using System.Collections.ObjectModel;
using System.IO;

namespace Hermes.App.Mvvm.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? sourceDirectory;

        [ObservableProperty]
        private string? targetDirectory;

        [ObservableProperty]
        private ObservableCollection<string> targetContent = [];

        [ObservableProperty]
        private ObservableCollection<string> sourceContent = [];

        public MainViewModel()
        {
            MoveAsyncCommand = new AsyncRelayCommand(MoveAsync);
        }

        [RelayCommand]
        private void BrowseSourceDirectory()
        {
            SourceDirectory = BrowseDirectory();
        }

        [RelayCommand]
        private void BrowseTargetDirectory()
        {
            TargetDirectory = BrowseDirectory();
        }

        partial void OnSourceDirectoryChanging(string? value)
        {
            SourceContent.Clear();
            if (value != "" && Directory.Exists(value))
            {
                foreach (string file in Directory.GetFiles(value))
                {
                    SourceContent.Add(Path.GetFileName(file));
                }
                foreach (string dir in Directory.GetDirectories(value))
                {
                    SourceContent.Add(Path.GetFileName(dir));
                }
            }
        }

        partial void OnTargetDirectoryChanging(string? value)
        {
            TargetContent.Clear();
            if (value != "" && Directory.Exists(value))
            {
                foreach (string file in Directory.GetFiles(value))
                {
                    TargetContent.Add(Path.GetFileName(file));
                }
                foreach (string dir in Directory.GetDirectories(value))
                {
                    TargetContent.Add(Path.GetFileName(dir));
                }
            }
        }

        private string BrowseDirectory()
        {
            Microsoft.Win32.OpenFolderDialog dialog = new()
            {
                Multiselect = false,
                Title = "Select a folder"
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FolderName;
            }
            return "";
        }

        [RelayCommand]
        private void Move()
        {
            if (SourceDirectory != null && TargetDirectory != null && SourceDirectory != TargetDirectory)
            {
                string[] files = Directory.GetFiles(SourceDirectory);
                string[] folders = Directory.GetDirectories(SourceDirectory);
                foreach (string file in files)
                {
                    ExplorerHelper.Move(file, TargetDirectory);
                }

                foreach (string directory in folders)
                {
                    ExplorerHelper.Move(directory, TargetDirectory);
                }
            }
        }

        public IAsyncRelayCommand MoveAsyncCommand { get; }

        private async Task MoveAsync()
        {
            if (SourceDirectory != null && TargetDirectory != null && SourceDirectory != TargetDirectory)
            {
                string[] files = Directory.GetFiles(SourceDirectory);
                string[] folders = Directory.GetDirectories(SourceDirectory);
                foreach (string file in files)
                {
                    await ExplorerHelper.MoveAsync(file, TargetDirectory);
                }

                foreach (string directory in folders)
                {
                    await ExplorerHelper.MoveAsync(directory, TargetDirectory);
                }
            }
        }
    }
}