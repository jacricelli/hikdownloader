namespace HikDownloader
{
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
        /// Número de archivo que se está procesndo.
        /// </summary>
        private static int fileCounter = 0;

        /// <summary>
        /// Combina todas las grabaciones de cada día en un único archivo.
        /// </summary>
        public static void Execute()
        {
            Console.WriteLine("Combinando...");
            Console.WriteLine("  > Configuración:");
            Console.WriteLine($"    Ruta de guardado:   {Program.Config.FFmpeg.Dir}");
            Console.WriteLine($"    Ruta de FFmpeg:     {Program.Config.FFmpeg.Bin}");
            Console.WriteLine($"    Tareas simultáneas: {Program.Config.FFmpeg.SimultaneousTasks}\n");
            Console.WriteLine("  > Progreso:");

            var files = GenerateFileLists();
            var error = false;

            if (files.Count > 0)
            {
                Console.WriteLine($"    Total: {files.Count}\n");

                Parallel.ForEach(
                    files,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Program.Config.FFmpeg.SimultaneousTasks
                    },
                    file =>
                    {
                        fileCounter++;

                        if (!CombineRecordings(file))
                        {
                            error = true;
                        }
                    }
                );

                Cleanup();

                if (!error)
                {
                    Console.WriteLine($"\r    Se han combinado los archivos.                    \n");
                }
                else
                {
                    Console.WriteLine($"\r    Se han combinado los archivos (con errores).                    \n");
                }
            }
            else
            {
                Console.WriteLine($"\n    No se han encontrado archivos.\n");
            }
        }

        /// <summary>
        /// Genera listas agrupadas por canal y fecha de las grabaciones.
        /// </summary>
        /// <returns>Una lista de archivos que contiene las grabaciones agrupadas por canal y fecha.</returns>
        private static List<string> GenerateFileLists()
        {
            var groups = Directory.EnumerateFiles(Program.Config.HikDownloader.Downloads.Dir, "*.avi", SearchOption.AllDirectories)
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

            return results.OrderBy(q => q).ToList();
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
            var path = Program.Config.FFmpeg.Dir;
            var outFile = string.Format("{0}\\{1}\\{2}\\{3}.avi", path, parts[1].Substring(0, parts[1].Length - 3), parts[0], parts[1]);
            var workingDirectory = Path.GetDirectoryName(outFile);

            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            if (File.Exists(outFile))
            {
                var i = 1;
                while (true)
                {
                    outFile = string.Format("{0}\\{1}\\{2}\\{3}_{4}.avi", path, parts[1].Substring(0, parts[1].Length - 3), parts[0], parts[1], i);
                    if (!File.Exists(outFile))
                    {
                        break;
                    }
                    i++;
                }
            }

            var log = new StringBuilder();

            using (var ffmpeg = new Process())
            {
                ffmpeg.StartInfo.FileName = Program.Config.FFmpeg.Bin;
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

                Console.Write($"\r    {fileCounter} {fileName}");
            }
            else
            {
                ok = false;

                var logPath = Util.GetPath("logs");
                var logFile = $"ffmpeg-{fileName}.log";

                File.WriteAllText(logPath + "\\" + logFile, results);
            }

            return ok;
        }

        /// <summary>
        /// Limpieza.
        /// </summary>
        private static void Cleanup()
        {
            DeleteEmptyDirs(Program.Config.HikDownloader.Downloads.Dir);

            if (!Directory.Exists(Program.Config.HikDownloader.Downloads.Dir))
            {
                Directory.CreateDirectory(Program.Config.HikDownloader.Downloads.Dir);
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
