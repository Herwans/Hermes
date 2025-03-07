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

        public async static Task MoveAsync(string source, string target, CancellationToken cancellationToken = default)
        {
            if (File.Exists(source))
            {
                await MoveFileAsync(source, target, cancellationToken);
            }
            else if (Directory.Exists(source))
            {
                await MoveDirectoryAsync(source, target, cancellationToken);
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

        private async static Task MoveDirectoryAsync(string source, string target, CancellationToken cancellationToken = default)
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
                    await MoveAsync(file, newLocation, cancellationToken);
                }
                foreach (string dir in dirs)
                {
                    await MoveAsync(dir, newLocation, cancellationToken);
                }
            }
            if (IsEmpty(source))
            {
                Directory.Delete(source, false);
            }
        }

        private async static Task MoveFileAsync(string source, string target, CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(target);
            string filename = Path.GetFileName(source);
            string newLocation = Path.Combine(target, filename);
            if (!File.Exists(newLocation))
            {
                await CopyFileAsync(source, target, cancellationToken);
                long originalSize = new FileInfo(source).Length;
                long copySize = new FileInfo(newLocation).Length;
                if (originalSize == copySize)
                {
                    File.Delete(source);
                }
            }
        }

        public async static Task CopyFileAsync(string source, string target, CancellationToken cancellationToken = default)
        {
            string filename = Path.GetFileName(source);
            string newLocation = Path.Combine(target, filename);
            var openForReading = new FileStreamOptions { Mode = FileMode.Open };
            using FileStream sourceStream = new FileStream(source, openForReading);

            var createForWriting = new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                Options = FileOptions.WriteThrough,
                BufferSize = 8 * 1024 * 1024,
                PreallocationSize = sourceStream.Length
            };
            using FileStream destination = new FileStream(newLocation, createForWriting);
            await sourceStream.CopyToAsync(destination, 8 * 1024 * 1024, cancellationToken);
        }

        public static async Task CopyFileParallelAsync(string source, string target, CancellationToken cancellationToken = default)
        {
            const int bufferSize = 64 * 1024 * 1024;
            const int maxConcurrentReads = 4;
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentReads);

            string filename = Path.GetFileName(source);
            string newLocation = Path.Combine(target, filename);

            using FileStream sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan);
            using FileStream destinationStream = new FileStream(newLocation, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.WriteThrough);

            long fileSize = sourceStream.Length;
            long totalBytesCopied = 0;

            List<Task> tasks = new List<Task>();

            for (long offset = 0; offset < fileSize; offset += bufferSize)
            {
                await semaphore.WaitAsync(cancellationToken);

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        byte[] buffer = new byte[bufferSize];
                        int bytesRead;

                        lock (sourceStream)
                        {
                            sourceStream.Seek(offset, SeekOrigin.Begin);
                            bytesRead = sourceStream.Read(buffer, 0, buffer.Length);
                        }

                        if (bytesRead > 0)
                        {
                            lock (destinationStream)
                            {
                                destinationStream.Seek(offset, SeekOrigin.Begin);
                                destinationStream.Write(buffer, 0, bytesRead);
                            }

                            Interlocked.Add(ref totalBytesCopied, bytesRead);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        private static bool IsEmpty(string directory)
        {
            return Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0;
        }
    }
}