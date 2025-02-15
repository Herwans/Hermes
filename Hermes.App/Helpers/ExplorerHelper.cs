using System.IO;

namespace Hermes.App.Helpers
{
    public static class ExplorerHelper
    {
        public static void Move(string source, string target)
        {
            if (File.Exists(source))
            {
                MoveFile(source, target);
            }
            else if (Directory.Exists(source))
            {
                MoveDirectory(source, target);
            }
        }

        private static void MoveDirectory(string source, string target)
        {
            string directoryname = Path.GetFileName(source);
            string newLocation = Path.Combine(target, directoryname);
            Directory.CreateDirectory(newLocation);

            if (!IsEmpty(source))
            {
                string[] files = Directory.GetFiles(source);
                string[] dirs = Directory.GetDirectories(source);
                foreach (string file in files)
                {
                    Move(file, newLocation);
                }
                foreach (string dir in dirs)
                {
                    Move(dir, newLocation);
                }
            }
            if (IsEmpty(source))
            {
                Directory.Delete(source, false);
            }
        }

        private static void MoveFile(string source, string target)
        {
            Directory.CreateDirectory(target);
            string filename = Path.GetFileName(source);
            string newLocation = Path.Combine(target, filename);
            if (!File.Exists(newLocation))
            {
                File.Copy(source, newLocation);
                long originalSize = new FileInfo(source).Length;
                long copySize = new FileInfo(newLocation).Length;
                if (originalSize == copySize)
                {
                    File.Delete(source);
                }
            }
        }

        private static bool IsEmpty(string directory)
        {
            return Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0;
        }
    }
}