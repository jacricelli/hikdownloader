namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;
    using FluentDateTime;

    /// <summary>
    /// Opciones.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Intervalo.
        /// </summary>
        private string _intervalo;

        /// <summary>
        /// Desde.
        /// </summary>
        private DateTime _desde;

        /// <summary>
        /// Hasta.
        /// </summary>
        private DateTime _hasta;

        /// <summary>
        /// Canal.
        /// </summary>
        [Option('c', "canal", Separator = ',', Default = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, HelpText = "El número de canal donde buscar grabaciones.\nPuede especificarse más de un canal separando cada número con una coma.")]
        public IEnumerable<int> Canal { get; set; }

        /// <summary>
        /// Intervalo.
        /// </summary>
        [Option('i', "intervalo", SetName = "intervalo", Default = "hoy", HelpText = "El intervalo de tiempo para buscar grabaciones.\nValores permitidos: hoy, ayer, estaSemana, semanaPasada, ultimasDosSemanas, esteMes, mesPasado.")]
        public string Intervalo
        {
            get
            {
                return _intervalo;
            }

            set
            {
                var intervalos = new List<string> { "hoy", "ayer", "estaSemana", "semanaPasada", "ultimasDosSemanas", "esteMes", "mesPasado" };
                if (intervalos.IndexOf(value) < 0)
                {
                    throw new Exception($"El intervalo '{value}' no es válido");
                }
                else
                {
                    _intervalo = value;

                    if (_desde == default && _hasta == default)
                    {
                        switch (_intervalo)
                        {
                            case "hoy":
                                _desde = DateTime.Today.Midnight();
                                _hasta = DateTime.Today.EndOfDay();
                                break;

                            case "ayer":
                                _desde = DateTime.Today.PreviousDay().Midnight();
                                _hasta = DateTime.Today.PreviousDay().EndOfDay();
                                break;

                            case "estaSemana":
                                _desde = DateTime.Today.FirstDayOfWeek().AddDays(1);
                                _hasta = _desde.LastDayOfWeek().AddDays(1).EndOfDay();
                                break;

                            case "semanaPasada":
                                _desde = DateTime.Today.FirstDayOfWeek().AddDays(1).WeekEarlier();
                                _hasta = _desde.LastDayOfWeek().AddDays(1).EndOfDay();
                                break;

                            case "ultimasDosSemanas":
                                _desde = DateTime.Today.FirstDayOfWeek().AddDays(-13);
                                _hasta = DateTime.Today.WeekEarlier().LastDayOfWeek().AddDays(1).EndOfDay();
                                break;

                            case "esteMes":
                                _desde = DateTime.Today.BeginningOfMonth();
                                _hasta = DateTime.Today.EndOfMonth();
                                break;

                            case "mesPasado":
                                _desde = DateTime.Today.PreviousMonth().BeginningOfMonth();
                                _hasta = DateTime.Today.PreviousMonth().EndOfMonth();
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Desde.
        /// </summary>
        [Option('d', "desde", SetName = "rango", HelpText = "Fecha (en formato yyyy-MM-dd) a partir de la cual buscar grabaciones.\nNo disponbile si se especifica la opción 'i, intervalo'.")]
        public DateTime Desde
        {
            get
            {
                if (_desde == default && _hasta != default)
                {
                    _desde = _hasta.Midnight();
                }

                return _desde;
            }

            set
            {
                if (Hasta != default && value > Hasta)
                {
                    throw new Exception("La fecha debe ser menor a la especificada en la opción 'h, hasta'");
                }
                else
                {
                    _desde = value;
                }

            }
        }

        /// <summary>
        /// Hasta.
        /// </summary>
        [Option('h', "hasta", SetName = "rango", HelpText = "Fecha (en formato yyyy-MM-dd) hasta la cual buscar grabaciones.\nNo disponbile si se especifica la opción 'i, intervalo'.")]
        public DateTime Hasta
        {
            get
            {
                if (_hasta == default && _desde != default)
                {
                    _hasta = _desde.EndOfDay();
                }

                return _hasta;
            }

            set
            {
                if (Desde != default && value < Desde)
                {
                    throw new Exception("La fecha debe ser mayor a la especificada en la opción 'd, desde'");
                }
                else
                {
                    _hasta = value;
                }

            }
        }

        /// <summary>
        /// Combinar.
        /// </summary>
        [Option('m', "combinar", Default = false, HelpText = "Combina todas las grabaciones de cada día en un único archivo.")]
        public bool Combinar { get; set; }

        /// <summary>
        /// Descargar.
        /// </summary>
        [Option('b', "descargar", Default = false, HelpText = "Descarga las grabaciones encontradas por la búsqueda.")]
        public bool Descargar { get; set; }

        /// <summary>
        /// Ejemplos.
        /// </summary>
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>()
                {
                    new Example("Descargar grabaciones del canal 5 del día de ayer", new ExampleOptions {
                        Canal = "5",
                        Intervalo = "ayer"
                    }),
                    new Example("Descargar grabaciones de los canales 2 y 7 en una fecha específica", new ExampleOptions() {
                        Canal = "2,7",
                        Desde = "2021-01-01",
                        Hasta = "2021-01-07"
                    })
                };
            }
        }
    }

    /// <summary>
    /// Clase de opciones para los ejemplos de uso.
    /// </summary>
    public class ExampleOptions
    {
        /// <summary>
        /// Canal.
        /// </summary>
        [Option('c', "canal")]
        public string Canal { get; set; }

        /// <summary>
        /// Intervalo.
        /// </summary>
        [Option('i', "intervalo")]
        public string Intervalo { get; set; }

        /// <summary>
        /// Desde.
        /// </summary>
        [Option('d', "desde")]
        public string Desde { get; set; }

        /// <summary>
        /// Hasta.
        /// </summary>
        [Option('h', "hasta")]
        public string Hasta { get; set; }
    }
}