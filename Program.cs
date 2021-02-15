namespace HikDownloader
{
    using CommandLine;
    using CommandLine.Text;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Pastel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Programa.
    /// </summary>
    public static class Program
    {
        #region Cierre de la aplicación
        /// <summary>
        /// Indica que se han llevado a cabo las tareas de limpieza.
        /// </summary>
        private static bool cleanedUp = false;

        /// <summary>
        /// Agrega o quita una función HandlerRoutine definida por la aplicación de la lista de funciones de controlador para el proceso de llamada.
        /// </summary>
        /// <param name="Handler">Puntero a la función HandlerRoutine definida por la aplicación que se va a agregar o quitar. Este parámetro puede ser NULL.</param>
        /// <param name="Add">Si este parámetro es TRUE, se agrega el controlador; si es FALSE, se quita el controlador.</param>
        /// <returns>
        /// Si la función se realiza correctamente, el valor devuelto es distinto de cero.
        /// Si la función no se realiza correctamente, el valor devuelto es cero. Para obtener información de error extendida, llame a GetLastError.
        /// </returns>
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        /// <summary>
        /// Un tipo de delegado que se utilizará como rutina del controlador.
        /// </summary>
        /// <param name="CtrlType">Mensaje de control.</param>
        /// <returns>Si la función maneja la señal de control, debería devolver TRUE. Si devuelve FALSE, se utiliza la siguiente función de controlador en la lista de controladores para este proceso.</returns>
        private delegate bool HandlerRoutine(CtrlTypes CtrlType);

        /// <summary>
        /// Mensajes de control enviados a la rutina del controlador.
        /// </summary>
        private enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        /// <summary>
        /// Rutina del controlador.
        /// </summary>
        /// <param name="CtrlType">Mensaje de control.</param>
        /// <returns>Si la función maneja la señal de control, debería devolver TRUE. Si devuelve FALSE, se utiliza la siguiente función de controlador en la lista de controladores para este proceso.</returns>
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            if (ctrlType == CtrlTypes.CTRL_C_EVENT || ctrlType == CtrlTypes.CTRL_BREAK_EVENT)
            {
                Console.Write("\n¿Confirma la cancelación de la tarea y cierre de la aplicación? (s/n) ");
                var key = Console.ReadKey();
                if (key.KeyChar.ToString().Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    Cleanup();

                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine();
                }

                return true;
            }
            else if (ctrlType == CtrlTypes.CTRL_CLOSE_EVENT)
            {
                Cleanup();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Responde a la salida del proceso.
        /// </summary>
        /// <param name="sender">Origen del evento.</param>
        /// <param name="e">Datos del evento.</param>
        private static void OnProcessExit(object sender, EventArgs e)
        {
            Cleanup();
        }

        /// <summary>
        /// Ejecuta las tareas de limpieza previas al cierre de la aplicación.
        /// </summary>
        private static void Cleanup()
        {
            if (!cleanedUp)
            {
                cleanedUp = true;

                HCNetSDK.Logout();

                HCNetSDK.Cleanup();
            }
        }
        #endregion

        /// <summary>
        /// Configuración.
        /// </summary>
        public static Config Config { get; private set; }

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        /// <param name="args">Argumentos.</param>
        public static void Main(string[] args)
        {
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
            var location = Assembly.GetExecutingAssembly().Location;
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
            Console.WriteLine($"{versionInfo.ProductName} v{versionInfo.ProductVersion}\n");

            try
            {
                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al configurar la consola: {ex.Message}".Pastel("FFFFFF").PastelBg("FF0000"));

                return;
            }

            try
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la configuración: {ex.Message}".Pastel("FFFFFF").PastelBg("FF0000"));

                return;
            }

            if (HCNetSDK.Initialize())
            {
                if (HCNetSDK.SetLogToFile(Config.HCNetSDK.Log))
                {
                    if (HCNetSDK.Login(Config.HCNetSDK.Device))
                    {
                        var channels = opts.Canal.Distinct().ToArray();
                        Array.Sort(channels);

                        var results = Search(channels, opts.Desde, opts.Hasta);
                        var total = results.Count;
                        var error = false;
                        if (total >  0)
                        {
                            Console.WriteLine("Descargando...");
                            Console.WriteLine("  > Configuración:");
                            Console.WriteLine($"    Ruta:     {Config.HikDownloader.Downloads.Dir}");
                            Console.WriteLine($"    Paralelo: {Config.HikDownloader.Downloads.Parallel}\n");

                            var count = 0;
                            Console.Write($"    Se han descargando 0 de {total} grabaciones.".Pastel("00FFFF"));

                            Parallel.ForEach(
                                results,
                                new ParallelOptions
                                {
                                    MaxDegreeOfParallelism = Config.HikDownloader.Downloads.Parallel
                                },
                                recording =>
                                {
                                    if (!Download(recording))
                                    {
                                        error = true;
                                    }

                                    count++;
                                    if (!error)
                                    {
                                        Console.Write($"\r    Se han descargando {count:N0} de {total:N0} grabaciones.".Pastel("00FFFF"));
                                    }
                                    else
                                    {
                                        Console.Write($"\r    Se han descargando {count:N0} de {total:N0} grabaciones (con errores).".Pastel("00FFFF"));
                                    }
                                }
                            );

                            Console.WriteLine($"\r    Se han descargado todas las grabaciones.".Pastel("00FFFF"));
                        }

                        return;
                    }
                }
            }

            Console.WriteLine($"HCNetSDK: {HCNetSDK.GetLastError()}".Pastel("FFFFFF").PastelBg("FF0000"));
        }

        #region Búsqueda
        /// <summary>
        /// Realiza una búsqueda.
        /// </summary>
        /// <param name="channels">Uno o más canales.</param>
        /// <param name="start">Fecha de comienzo.</param>
        /// <param name="end">Fecha de finalización.</param>
        /// <returns>Resultados de la búsqueda.</returns>
        private static List<HCNetSDK.NET_DVR_FINDDATA> Search(int[] channels, DateTime start, DateTime end)
        {
            Console.WriteLine("Buscando...");
            Console.WriteLine("  > Configuración:");
            Console.WriteLine($"    Canales:   {string.Join(", ", channels)}");
            Console.WriteLine($"    Intervalo: {start:dd/MM/yyyy} a {end:dd/MM/yyyy}\n");
            Console.WriteLine("  > Resultados:");

            var total = 0;
            var totalPending = 0;
            var results = new List<HCNetSDK.NET_DVR_FINDDATA>();
            foreach (var channel in channels)
            {
                var count = 0;
                var countPending = 0;
                foreach (var from in EachDay(start, end))
                {
                    var thru = from.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (thru > end)
                    {
                        thru = end;
                    }

                    var conditions = default(HCNetSDK.NET_DVR_FILECOND);
                    conditions.lChannel = channel;
                    conditions.dwFileType = 0xff;
                    conditions.dwIsLocked = 0xff;

                    conditions.struStartTime.dwYear = (uint)from.Year;
                    conditions.struStartTime.dwMonth = (uint)from.Month;
                    conditions.struStartTime.dwDay = (uint)from.Day;
                    conditions.struStartTime.dwHour = 0;
                    conditions.struStartTime.dwMinute = 0;
                    conditions.struStartTime.dwSecond = 0;

                    conditions.struStopTime.dwYear = (uint)thru.Year;
                    conditions.struStopTime.dwMonth = (uint)thru.Month;
                    conditions.struStopTime.dwDay = (uint)thru.Day;
                    conditions.struStopTime.dwHour = 23;
                    conditions.struStopTime.dwMinute = 59;
                    conditions.struStopTime.dwSecond = 59;

                    var handle = HCNetSDK.FindFile(HCNetSDK.UserId, ref conditions);
                    if (handle > -1)
                    {
                        var recording = default(HCNetSDK.NET_DVR_FINDDATA);
                        while (true)
                        {
                            var result = HCNetSDK.FindNextFile(handle, ref recording);
                            if (result == HCNetSDK.NET_DVR_ISFINDING)
                            {
                                continue;
                            }
                            else if (result == HCNetSDK.NET_DVR_FILE_SUCCESS)
                            {
                                if (!results.Contains(recording))
                                {
                                    if (!File.Exists(GetRecordingPath(recording)))
                                    {
                                        results.Add(recording);

                                        countPending++;
                                    }

                                    count++;
                                }
                            }
                            else if (result == HCNetSDK.NET_DVR_FILE_NOFIND || result == HCNetSDK.NET_DVR_NOMOREFILE)
                            {
                                break;
                            }
                            else if (result == HCNetSDK.NET_DVR_FIND_TIMEOUT)
                            {
                                Console.WriteLine($"    Canal N° {channel:00}: Tiempo de espera agotado.".Pastel("FFA500"));

                                break;
                            }
                            else if (result == HCNetSDK.NET_DVR_FILE_EXCEPTION)
                            {
                                Console.WriteLine($"    Canal N° {channel:00}: Error en la búsqueda.".Pastel("FF0000"));

                                break;
                            }
                        }

                        HCNetSDK.FindClose(handle);
                    }
                    else
                    {
                        Console.WriteLine($"    Canal N° {channel:00}: {HCNetSDK.GetLastError()}".Pastel("FF0000"));
                    }
                }

                if (count > 0)
                {
                    if (count == countPending)
                    {
                        Console.WriteLine($"    Canal N° {channel:00}: {countPending:N0} grabaciones.");
                    }
                    else
                    {
                        Console.WriteLine($"    Canal N° {channel:00}: {countPending:N0} de {count:N0} grabaciones.");
                    }
                }
                else
                {
                    Console.WriteLine($"    Canal N° {channel:00}: 0 grabaciones.");
                }

                total += count;
                totalPending += countPending;
            }

            if (total > 0)
            {
                if (total == totalPending)
                {
                    Console.WriteLine("\n" + $"    Restan por descargar {total:N0} grabaciones.".Pastel("00FFFF") + "\n");
                }
                else
                {
                    Console.WriteLine("\n" + $"    Restan por descargar {totalPending:N0} de {total:N0} grabaciones.".Pastel("00FFFF") + "\n");
                }
            }
            else
            {
                Console.WriteLine("\n" + $"    No quedan grabaciones por descargar.".Pastel("00FFFF") + "\n");
            }

            return results;
        }

        /// <summary>
        /// Obtiene cada día entre dos fechas.
        /// </summary>
        /// <param name="from">Desde.</param>
        /// <param name="thru">Hasta</param>
        /// <returns>Cada día entre <paramref name="from"/> y <paramref name="thru"/> inclusive.</returns>
        private static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
        #endregion

        #region Descarga
        /// <summary>
        /// Descarga una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        private static bool Download(HCNetSDK.NET_DVR_FINDDATA recording)
        {
            var file = GetRecordingPath(recording, false);
            var directory = Path.GetDirectoryName(file);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var error = false;
            var handle = HCNetSDK.GetFileByName(HCNetSDK.UserId, recording.sFileName, file);
            if (handle > -1)
            {
                uint iOutValue = 0;
                if (HCNetSDK.PlayBackControl(handle, 1, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    while (true)
                    {
                        Thread.Sleep(750);

                        var progress = HCNetSDK.GetDownloadPos(handle);
                        if (progress < 0)
                        {
                            error = true;

                            HCNetSDK.StopGetFile(handle);

                            break;
                        }
                        else if (progress >= 0 && progress < 100)
                        {
                            // NOP
                        }
                        else if (progress == 100)
                        {
                            HCNetSDK.StopGetFile(handle);

                            File.Move(file, GetRecordingPath(recording));

                            break;
                        }
                        else if (progress == 200)
                        {
                            error = true;

                            HCNetSDK.StopGetFile(handle);

                            break;
                        }
                    }
                }
                else
                {
                    error = true;
                }
            }
            else
            {
                error = true;
            }

            return error == false;
        }

        /// <summary>
        /// Obtiene el nombre de archivo de una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="extension">Anexar extensión al archivo.</param>
        /// <returns>Nombre del archivo.</returns>
        private static string GetRecordingFileName(HCNetSDK.NET_DVR_FINDDATA recording, bool extension = true)
        {
            return string.Format(
                "{0}-{1:00}\\Channel {2:00}\\{0}-{1:00}-{3:00}_{4:00}{5:00}{6:00}_{7:00}{8:00}{9:00}{10}",
                recording.struStartTime.dwYear,
                recording.struStartTime.dwMonth,
                recording.sFileName.Split('_')[0].TrimStart(new char[] { 'c', 'h' }),
                recording.struStartTime.dwDay,
                recording.struStartTime.dwHour,
                recording.struStartTime.dwMinute,
                recording.struStartTime.dwSecond,
                recording.struStopTime.dwHour,
                recording.struStopTime.dwMinute,
                recording.struStopTime.dwSecond,
                extension ? ".avi" : string.Empty);
        }

        /// <summary>
        /// Obtiene la ruta absoluta del archivo de una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="extension">Anexar extensión al archivo.</param>
        /// <returns>Ruta al archivo.</returns>
        private static string GetRecordingPath(HCNetSDK.NET_DVR_FINDDATA recording, bool extension = true)
        {
            return $"{Config.HikDownloader.Downloads.Dir}\\{GetRecordingFileName(recording, extension)}";
        }
        #endregion
    }
}
