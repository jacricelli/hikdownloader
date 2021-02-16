namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Búsqueda.
    /// </summary>
    public static class Search
    {
        /// <summary>
        /// Representa una grabación.
        /// </summary>
        public struct Recording
        {
            /// <summary>
            /// Ruta al archivo original de la grabación.
            /// </summary>
            public string OriginalFullPath;

            /// <summary>
            /// Ruta al archivo de la grabación.
            /// </summary>
            public string FullPath;

            /// <summary>
            /// Nombre original del archivo de la grabación.
            /// </summary>
            public string OriginalFileName;
        }

        /// <summary>
        /// Ejecuta una búsqueda.
        /// </summary>
        /// <param name="channels">Arreglo de canales.</param>
        /// <param name="start">Fecha de comienzo.</param>
        /// <param name="end">Fecha de finalización.</param>
        public static List<Recording> Execute(int[] channels, DateTime start, DateTime end)
        {
            Console.WriteLine("Buscando...");
            Console.WriteLine("  > Configuración:");
            Console.WriteLine($"    Canales:   {string.Join(", ", channels)}");
            Console.WriteLine($"    Intervalo: {start:dd/MM/yyyy} a {end:dd/MM/yyyy}\n");
            Console.WriteLine("  > Resultados:");

            var recordings = new List<Recording>();
            var total = 0;
            var pending = 0;

            foreach (var channel in channels)
            {
                var count = 0;
                var missing = 0;

                foreach (var from in EachDay(start, end))
                {
                    var thru = from.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (thru > end)
                    {
                        thru = end;
                    }

                    Console.Write($"\r    Canal N° {channel:00}: {from:dd/MM/yyyy}");

                    var conditions = default(HCNetSDK.Search.NET_DVR_FILECOND);
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

                    var handle = HCNetSDK.Search.FindFile(HCNetSDK.Session.UserId, ref conditions);
                    if (handle > -1)
                    {
                        var searchResult = default(HCNetSDK.Search.NET_DVR_FINDDATA);
                        while (true)
                        {
                            var result = HCNetSDK.Search.FindNextFile(handle, ref searchResult);
                            if (result == HCNetSDK.Search.NET_DVR_ISFINDING)
                            {
                                continue;
                            }
                            else if (result == HCNetSDK.Search.NET_DVR_FILE_SUCCESS)
                            {
                                var item = GetRecording(searchResult);
                                if (!recordings.Contains(item))
                                {
                                    count++;

                                    if (!File.Exists(item.FullPath))
                                    {
                                        recordings.Add(item);

                                        missing++;
                                    }
                                }
                            }
                            else if (result == HCNetSDK.Search.NET_DVR_FILE_NOFIND || result == HCNetSDK.Search.NET_DVR_NOMOREFILE)
                            {
                                break;
                            }
                            else if (result == HCNetSDK.Search.NET_DVR_FIND_TIMEOUT)
                            {
                                Console.WriteLine($"\r    Canal N° {channel:00}: {from:dd/MM/yyyy} (Tiempo de espera agotado)");

                                break;
                            }
                            else if (result == HCNetSDK.Search.NET_DVR_FILE_EXCEPTION)
                            {
                                Console.WriteLine($"\r    Canal N° {channel:00}: {from:dd/MM/yyyy} (Error en la búsqueda)");

                                break;
                            }
                        }

                        HCNetSDK.Search.FindClose(handle);
                    }
                    else
                    {
                        Console.WriteLine($"\r    Canal N° {channel:00}: {from:dd/MM/yyyy} ({HCNetSDK.Error.GetLastError()})");
                    }
                }

                if (count > 0 && missing > 0)
                {
                    if (count == missing)
                    {
                        Console.Write($"\r    Canal N° {channel:00}: {missing:N0}                                        \n");
                    }
                    else
                    {
                        Console.Write($"\r    Canal N° {channel:00}: {missing:N0} de {count:N0}                                        \n");
                    }
                }
                else
                {
                    Console.Write($"\r    Canal N° {channel:00}: 0                                        \n");
                }

                total += count;
                pending += missing;
            }

            if (total > 0 && pending > 0)
            {
                if (total == pending)
                {
                    Console.WriteLine("\n" + $"    Restan por descargar {total:N0} grabaciones.\n");
                }
                else
                {
                    Console.WriteLine("\n" + $"    Restan por descargar {pending:N0} de {total:N0} grabaciones.\n");
                }
            }
            else
            {
                Console.WriteLine("\n" + $"    No quedan grabaciones por descargar.\n");
            }

            return recordings;
        }

        /// <summary>
        /// Obtiene una grabación.
        /// </summary>
        /// <param name="searchResult">Resultado de la búsqueda.</param>
        /// <returns>Grabación.</returns>
        private static Recording GetRecording(HCNetSDK.Search.NET_DVR_FINDDATA searchResult)
        {
            var path = string.Format("{0}\\{1}-{2:00}\\Canal {3:00}",
                Program.Config.HikDownloader.Downloads.Dir,
                searchResult.struStartTime.dwYear,
                searchResult.struStartTime.dwMonth,
                searchResult.sFileName.Split('_')[0].TrimStart(new char[] { 'c', 'h' }));

            return new Recording()
            {
                OriginalFileName = searchResult.sFileName,
                OriginalFullPath = $"{path}\\{searchResult.sFileName}",
                FullPath = string.Format("{0}\\{1}-{2:00}-{3:00}_{4:00}{5:00}{6:00}_{7:00}{8:00}{9:00}.avi",
                    path,
                    searchResult.struStartTime.dwYear,
                    searchResult.struStartTime.dwMonth,
                    searchResult.struStartTime.dwDay,
                    searchResult.struStartTime.dwHour,
                    searchResult.struStartTime.dwMinute,
                    searchResult.struStartTime.dwSecond,
                    searchResult.struStopTime.dwHour,
                    searchResult.struStopTime.dwMinute,
                    searchResult.struStopTime.dwSecond)
            };
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
    }
}
