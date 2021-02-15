namespace HikDownloader
{
    using Pastel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Combinar.
    /// </summary>
    public static class Combine
    {
        /// <summary>
        /// Configuración de FFmpeg.
        /// </summary>
        public static Config.FFmpegConfig Config { get; set; }

        /// <summary>
        /// Ruta al directorio de descarga.
        /// </summary>
        public static string DownloadDir { get; set; }

        /// <summary>
        /// Combina todas las grabaciones de cada día en un único archivo.
        /// </summary>
        public static void Execute()
        {
            Console.WriteLine("Combinando...");
            Console.WriteLine("  > Configuración:");
            Console.WriteLine($"    Ruta de guardado:   {Config.Dir}");
            Console.WriteLine($"    Ruta de FFmpeg:     {Config.Bin}");
            Console.WriteLine($"    Tareas simultáneas: {Config.SimultaneousTasks}\n");
            Console.WriteLine("  > Progreso:");

            var files = GenerateFileLists();

            if (files.Count > 0)
            {
                Parallel.ForEach(
                    files,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Config.SimultaneousTasks
                    },
                    file =>
                    {
                        CombineRecordings(file);
                    }
                );

                Cleanup();

                Console.WriteLine();
                Console.WriteLine($"    Se han combinado los archivos.".Pastel("00FFFF"));
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"    No se han encontrado archivos.".Pastel("00FFFF"));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Genera listas agrupadas por canal y fecha de las grabaciones.
        /// </summary>
        /// <returns>Una lista de archivos que contiene las grabaciones agrupadas por canal y fecha.</returns>
        private static List<string> GenerateFileLists()
        {
            var groups = Directory.EnumerateFiles(DownloadDir, "*.avi", SearchOption.AllDirectories)
                .Select(x => $"file '{x}'")
                .GroupBy(i =>
                {
                    var parts = i.Split('\\');
                    var channel = parts[parts.Length - 2];
                    var date = parts[parts.Length - 1].Split('_')[0];

                    return $"{channel}_{date}";
                });

            var results = new List<string>();
            var path = Util.GetPath("temp");

            foreach (var group in groups)
            {
                var fileName = group.Key + ".txt";
                File.WriteAllLines(path + "\\" + fileName, group);

                results.Add(path + "\\" + fileName);
            }

            return results;
        }

        /// <summary>
        /// Combina grabaciones.
        /// </summary>
        /// <param name="file">Ruta al archivo que contiene la lista de grabaciones.</param>
        private static bool CombineRecordings(string file)
        {
            var ok = true;
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('_');
            var path = Config.Dir;
            var outFile = string.Format("{0}\\{1}\\{2}\\{3}.avi", path, parts[1].Substring(0, parts[1].Length - 3), parts[0], parts[1]);
            var workingDirectory = Path.GetDirectoryName(outFile);

            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            var log = new StringBuilder();

            using (var ffmpeg = new Process())
            {
                ffmpeg.StartInfo.FileName = Config.Bin;
                ffmpeg.StartInfo.Arguments = string.Format("-y -loglevel error -f concat -safe 0 -i \"{0}\" -c copy \"{1}\"", file, outFile);
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardOutput = true;
                ffmpeg.StartInfo.RedirectStandardError = true;
                ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.WorkingDirectory = workingDirectory;

                ffmpeg.EnableRaisingEvents = true;
                ffmpeg.OutputDataReceived += (s, e) => log.AppendLine(e.Data);
                ffmpeg.ErrorDataReceived += (s, e) => log.AppendLine(e.Data);
                ffmpeg.Start();
                ffmpeg.BeginOutputReadLine();
                ffmpeg.BeginErrorReadLine();
                ffmpeg.WaitForExit();
            }

            var results = log.ToString().Trim();
            if (results.Length == 0)
            {
                File.ReadAllLines(file)
                    .Select(i => i.Replace("file '", string.Empty).TrimEnd('\''))
                    .ToList()
                    .ForEach(delegate (string f)
                    {
                        File.Delete(f);
                    });

                File.Delete(file);

                Console.WriteLine($"    Se ha combinado {fileName}.".Pastel("00FFFF"));
            }
            else
            {
                ok = false;

                var logPath = Util.GetPath("logs");
                var logFile = $"ffmpeg-{fileName}.log";

                File.WriteAllText(logPath + "\\" + logFile, results);

                Console.WriteLine($"    Error al combinar {fileName}.".Pastel("FF0000"));
            }

            return ok;
        }

        /// <summary>
        /// Limpieza.
        /// </summary>
        private static void Cleanup()
        {
            DeleteEmptyDirs(DownloadDir);

            if (!Directory.Exists(DownloadDir))
            {
                Directory.CreateDirectory(DownloadDir);
            }
        }

        /// <summary>
        /// Elimina directorios vacíos de forma recursiva.
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteEmptyDirs(string path)
        {
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                DeleteEmptyDirs(dir);
            }

            var entries = Directory.EnumerateFileSystemEntries(path);

            if (!entries.Any())
            {
                try
                {
                    Directory.Delete(path);
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
            }
        }
    }
}
