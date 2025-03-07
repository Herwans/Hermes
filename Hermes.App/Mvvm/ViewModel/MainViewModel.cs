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
        [NotifyCanExecuteChangedFor(nameof(MoveCommand))]
        private string? sourceDirectory;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveCommand))]
        private string? targetDirectory;

        [ObservableProperty]
        private ObservableCollection<string> targetContent = [];

        [ObservableProperty]
        private ObservableCollection<string> sourceContent = [];

        [ObservableProperty]
        private int currentValue = 0;

        [ObservableProperty]
        private int maximumValue = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveCommand))]
        [NotifyCanExecuteChangedFor(nameof(BrowseSourceDirectoryCommand))]
        [NotifyCanExecuteChangedFor(nameof(BrowseTargetDirectoryCommand))]
        private bool isBusy = false;

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private void BrowseSourceDirectory()
        {
            SourceDirectory = BrowseDirectory();
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private void BrowseTargetDirectory()
        {
            TargetDirectory = BrowseDirectory();
        }

        partial void OnSourceDirectoryChanging(string? value)
        {
            if (value != null)
                SourceContentUpdate(value);
        }

        private void SourceContentUpdate(string value)
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
            if (value != null)
                TargetContentUpdate(value);
        }

        private void TargetContentUpdate(string value)
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

        [RelayCommand(CanExecute = nameof(CanMove))]
        private async Task MoveAsync()
        {
            IsBusy = true;
            if (SourceDirectory != null && TargetDirectory != null && SourceDirectory != TargetDirectory)
            {
                string[] files = Directory.GetFiles(SourceDirectory);
                string[] folders = Directory.GetDirectories(SourceDirectory);
                MaximumValue = files.Length + folders.Length;
                CurrentValue = 0;
                foreach (string file in files)
                {
                    await ExplorerHelper.MoveAsync(file, TargetDirectory);
                    TargetContentUpdate(TargetDirectory);
                    SourceContentUpdate(SourceDirectory);
                    CurrentValue++;
                }

                foreach (string directory in folders)
                {
                    await ExplorerHelper.MoveAsync(directory, TargetDirectory);
                    TargetContentUpdate(TargetDirectory);
                    SourceContentUpdate(SourceDirectory);
                    CurrentValue++;
                }
                TargetContentUpdate(TargetDirectory);
                SourceContentUpdate(SourceDirectory);
            }
            IsBusy = false;
        }

        private bool IsNotBusy()
        {
            return !IsBusy;
        }

        private bool CanMove()
        {
            return IsNotBusy()
                && Directory.Exists(SourceDirectory)
                && TargetDirectory != "";
        }
    }
}