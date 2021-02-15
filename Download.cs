namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Descarga.
    /// </summary>
    public static class Download
    {
        /// <summary>
        /// Archivo de registro.
        /// </summary>
        private static StreamWriter logStream;

        /// <summary>
        /// Ruta al directorio de descarga.
        /// </summary>
        public static string DownloadDir { get; set; }

        /// <summary>
        /// Número máximo de tareas simultáneas.
        /// </summary>
        public static int SimultaneousTasks { get; set; } = 3;

        /// <summary>
        /// Descarga grabaciones.
        /// </summary>
        /// <param name="recordings">Grabaciones.</param>
        public static void Execute(List<Search.Recording> recordings)
        {
            var logFile = Util.GetPath("logs", $"downloads-{DateTime.Now:yyyy-MM-dd}.txt");
            using (logStream = new StreamWriter(logFile, true))
            {
                Console.WriteLine("Descargando...");
                Console.WriteLine("  > Configuración:");
                Console.WriteLine($"    Ruta de descarga:      {DownloadDir}");
                Console.WriteLine($"    Descargas simultáneas: {SimultaneousTasks}\n");
                Console.WriteLine("  > Progreso:");

                var total = recordings.Count;
                var count = 0;
                var error = false;
                Console.Write($"    Se han descargando 0 de {total} grabaciones.");

                Parallel.ForEach(
                    recordings,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = SimultaneousTasks
                    },
                    recording =>
                    {
                        if (!DownloadRecording(recording))
                        {
                            error = true;
                        }

                        count++;
                        if (!error)
                        {
                            Console.Write($"\r    Se han descargando {count:N0} de {total:N0} grabaciones.");
                        }
                        else
                        {
                            Console.Write($"\r    Se han descargando {count:N0} de {total:N0} grabaciones (con errores).");
                        }
                    }
                );

                if (!error)
                {
                    Console.WriteLine($"\r    Se han descargado todas las grabaciones.                                        ");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"\r    Se han descargado todas las grabaciones (con errores).                                        ");
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Descarga una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        private static bool DownloadRecording(Search.Recording recording)
        {
            var directory = Path.GetDirectoryName(recording.OriginalFullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var ok = true;
            var handle = HCNetSDK.Download.GetFileByName(HCNetSDK.Session.UserId, recording.OriginalFileName, recording.OriginalFullPath);
            if (handle > -1)
            {
                uint iOutValue = 0;
                if (HCNetSDK.Download.PlayBackControl(handle, 1, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    while (true)
                    {
                        Thread.Sleep(750);

                        var progress = HCNetSDK.Download.GetDownloadPos(handle);
                        if (progress < 0 || progress == 200)
                        {
                            HCNetSDK.Download.StopGetFile(handle);

                            ok = false;
                            LogError(recording, $"Progreso: {progress}");

                            break;
                        }
                        else if (progress == 100)
                        {
                            HCNetSDK.Download.StopGetFile(handle);

                            File.Move(recording.OriginalFullPath, recording.FullPath);

                            break;
                        }
                    }
                }
                else
                {
                    ok = false;
                    LogError(recording, HCNetSDK.Error.GetLastError());
                }
            }
            else
            {
                ok = false;
                LogError(recording, HCNetSDK.Error.GetLastError());
            }

            return ok;
        }

        /// <summary>
        /// Registra un mensaje de error.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="message">Mensaje.</param>
        private static void LogError(Search.Recording recording, string message)
        {
            logStream.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - {recording.OriginalFileName} - {Path.GetFileName(recording.FullPath)}]: {message}");
            logStream.Flush();
        }
    }
}
