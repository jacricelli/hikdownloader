namespace HikDownloader
{
    using CommandLine;
    using CommandLine.Text;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Programa.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        /// <param name="args">Argumentos.</param>
        public static void Main(string[] args)
        {
            ConsoleControlHandler.Enable();

            SentenceBuilder.Factory = () => new LocalizableSentenceBuilder();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        /// <summary>
        /// Ejecuta la descarga de grabaciones de acuerdo a las opciones especificadas.
        /// </summary>
        /// <param name="opts">Opciones.</param>
        private static void Run(Options opts)
        {
            PrintAppHeader();

            if (InitializeDependencies())
            {
                DownloadRecordings(opts.Canal, opts.Desde, opts.Hasta, opts.Combinar);
            }
        }

        /// <summary>
        /// Imprime la cabecera de la aplicación.
        /// </summary>
        private static void PrintAppHeader()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
            Console.WriteLine($"{versionInfo.ProductName} v{versionInfo.ProductVersion}\n");
        }

        /// <summary>
        /// Inicializa las dependencias de la aplicación.
        /// </summary>
        private static bool InitializeDependencies()
        {
            Config.Config config = null;
            try
            {
                config = JsonConvert.DeserializeObject<Config.Config>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la configuración: {ex.Message}");

                return false;
            }

            if (HCNetSDK.SDK.Initialize())
            {
                if (HCNetSDK.Log.SetLogToFile(config.HCNetSDK.Log))
                {
                    if (HCNetSDK.Session.Login(config.HCNetSDK.Session))
                    {
                        Search.DownloadDir = config.HikDownloader.Downloads.Dir;

                        Download.DownloadDir = config.HikDownloader.Downloads.Dir;
                        Download.SimultaneousTasks = config.HikDownloader.Downloads.SimultaneousTasks;

                        Combine.Config = config.FFmpeg;
                        Combine.DownloadDir = config.HikDownloader.Downloads.Dir;

                        return true;
                    }
                }
            }

            Console.WriteLine($"HCNetSDK: {HCNetSDK.Error.GetLastError()}");

            return false;
        }

        /// <summary>
        /// Descarga grabaciones.
        /// </summary>
        /// <param name="channels">Arreglo de canales.</param>
        /// <param name="from">Fecha a partir de la cual buscar grabaciones.</param>
        /// <param name="thru">Fecha hasta la cual buscar grabaciones.</param>
        /// <param name="combine">Combina todas las grabaciones de cada día en un único archivo.</param>
        private static void DownloadRecordings(IEnumerable<int> channels, DateTime from, DateTime thru, bool combine = true)
        {
            var chnls = channels.Distinct().ToArray();
            Array.Sort(chnls);

            var recordings = Search.Execute(chnls, from, thru);
            if (recordings.Count > 0)
            {
                Download.Execute(recordings);

                if (combine)
                {
                    Combine.Execute();
                }
            }

            Console.WriteLine("Se han completado todas las tareas.");
            Console.WriteLine();
        }
    }
}
