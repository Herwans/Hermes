using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        [RelayCommand]
        private void BrowseSourceDirectory()
        {
            SourceDirectory = BrowseDirectory();
            SourceContent.Clear();

            if (SourceDirectory != "")
            {
                foreach (string file in Directory.GetFiles(SourceDirectory))
                {
                    SourceContent.Add(Path.GetFileName(file));
                }
                foreach (string dir in Directory.GetDirectories(SourceDirectory))
                {
                    SourceContent.Add(Path.GetFileName(dir));
                }
            }
        }

        [RelayCommand]
        private void BrowseTargetDirectory()
        {
            TargetDirectory = BrowseDirectory();
            TargetContent.Clear();

            if (TargetDirectory != "")
            {
                foreach (string file in Directory.GetFiles(TargetDirectory))
                {
                    TargetContent.Add(Path.GetFileName(file));
                }
                foreach (string dir in Directory.GetDirectories(TargetDirectory))
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
            if (Directory.Exists(SourceDirectory)
                && Directory.Exists(TargetDirectory)
                && SourceDirectory != TargetDirectory)
            {
                string[] files = Directory.GetFiles(SourceDirectory);
                string[] folders = Directory.GetDirectories(SourceDirectory);
                foreach (string file in files)
                {
                    string newLocation = Path.Combine(TargetDirectory, Path.GetFileName(file));
                    if (!File.Exists(newLocation))
                    {
                        File.Move(file, newLocation);
                    }
                }

                foreach (string directory in folders)
                {
                    string newLocation = Path.Combine(TargetDirectory, Path.GetFileName(directory));
                    if (!Directory.Exists(newLocation))
                    {
                        Directory.Move(directory, newLocation);
                    }
                }
            }
        }
    }
}