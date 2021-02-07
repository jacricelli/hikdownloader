﻿namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using FluentDateTime;
    using HCNetSDK;

    /// <summary>
    /// Ventana principal.
    /// </summary>
    public partial class MainWindow : Form
    {
        #region Formulario
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Responde a la carga del formulario.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            SetupChannels();

            SetupSearch();

            SetupDownload();
        }

        /// <summary>
        /// Inicializa el entorno de HCNetSDK.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private async void MainWindow_Shown(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (SDK.Initialize())
                {
                    LogEvent("Se ha inicializado el entorno de programación.");

                    var logPath = string.Format("logs\\HCNetSDK\\{0}_{1}", DateTime.Now.ToString("yyyy-MM-dd"), System.Diagnostics.Process.GetCurrentProcess().Id);
                    if (SDK.EnableLogging(Util.GetDirectory(logPath)))
                    {
                        LogEvent("Se ha habilitado el registro de mensajes de la SDK.");

                        Session.Address = Properties.Settings.Default.Address;
                        Session.Port = Properties.Settings.Default.Port;

                        if (Session.Login(Properties.Settings.Default.UserName, Properties.Settings.Default.Password))
                        {
                            LogEvent("Se ha iniciado sesión.", SDK.GetLastError());

                            Invoke(new MethodInvoker(delegate
                            {
                                Enabled = true;
                            }));

                            return;
                        }
                        else
                        {
                            LogEvent("Error al iniciar sesión.", SDK.GetLastError());
                        }
                    }
                    else
                    {
                        LogEvent("Error al habilitar el registro de mensajes.", SDK.GetLastError());
                    }
                }
                else
                {
                    LogEvent("Error al inicializar el entorno de programación.", SDK.GetLastError());
                }
            });
        }

        /// <summary>
        /// Cierre del formulario.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Searcher.IsRunning)
            {
                e.Cancel = true;
                MessageBox.Show("Cancele la búsqueda o espere a que finalice para cerrar la aplicación.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Descarga
        /// <summary>
        /// Descargas.
        /// </summary>
        private Dictionary<Recording, ListViewItem> downloads;

        /// <summary>
        /// Prepara la descarga.
        /// </summary>
        private void SetupDownload()
        {
            if (Properties.Settings.Default.Downloads == string.Empty)
            {
                Properties.Settings.Default.Downloads = Util.GetDirectory("downloads");
                Properties.Settings.Default.Save();
            }

            DownloadDir.Text = Properties.Settings.Default.Downloads;

            Downloader.DownloadDir = Properties.Settings.Default.Downloads;
            Downloader.ParallelDownloads = Properties.Settings.Default.ParallelDownloads;

            Downloader.OnStart += Download_OnStart;
            Downloader.OnFinish += Download_OnFinish;
            Downloader.OnBegin += Download_OnBegin;
            Downloader.OnEnd += Download_OnEnd;
            Downloader.OnDownloading += Download_OnDownloading;
            Downloader.OnError += Download_OnError;
            Downloader.OnCancel += Download_OnCancel;
        }

        /// <summary>
        /// Cambia el directorio de descarga.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                if (DownloadDir.Text != string.Empty)
                {
                    dialog.SelectedPath = DownloadDir.Text;
                }

                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    DownloadDir.Text = dialog.SelectedPath;

                    Properties.Settings.Default.Downloads = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    Downloader.DownloadDir = Properties.Settings.Default.Downloads;
                }
            }
        }

        /// <summary>
        /// Descarga grabaciones.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private async void Download_Click(object sender, EventArgs e)
        {
            if (Recordings.Items.Count == 0)
            {
                return;
            }

            await Task.Run(() =>
            {
                Invoke(new MethodInvoker(delegate
                {
                    if (Downloader.IsRunning)
                    {
                        Downloader.Cancel();

                        return;
                    }

                    downloads = new Dictionary<Recording, ListViewItem>();

                    var recordings = new List<Recording>();
                    foreach (var item in Recordings.Items)
                    {
                        var recording = (ListViewItem)item;
                        var t = (Recording)(recording).Tag;

                        downloads.Add(t, recording);
                        recordings.Add(t);
                    }

                    Downloader.Download(recordings);
                }));
            });
        }

        /// <summary>
        /// Comienzo del proceso de descarga.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnStart(object sender, EventArgs e)
        {
            LogEvent("Comenzando a descargar grabaciones.");

            Invoke(new MethodInvoker(delegate
            {
                Download.Text = "&Cancelar";
                groupBox4.Enabled = false;
                Browse.Enabled = false;
            }));
        }

        /// <summary>
        /// Fin del proceso de descarga.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnFinish(object sender, EventArgs e)
        {
            LogEvent("Se ha finalizado la descarga de grabaciones.");

            Invoke(new MethodInvoker(delegate
            {
                Download.Text = "&Descargar";
                groupBox4.Enabled = true;
                Browse.Enabled = true;
                Download.Enabled = Recordings.Items.Count > 0;
            }));
        }

        /// <summary>
        /// Comienzo de la descarga de una grabación.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnBegin(object sender, EventArgs e)
        {
            var evt = (DownloadEvent)e;
            var item = downloads[evt.Recording];

            if (Recordings.InvokeRequired)
            {
                Recordings.Invoke(new MethodInvoker(delegate
                {
                    item.SubItems[6].Text = "Descargando 0%";
                    item.EnsureVisible();
                }));
            }
            else
            {
                item.SubItems[6].Text = "Descargando 0%";
                item.EnsureVisible();
            }
        }

        /// <summary>
        /// Fin de la descarga de una grabación.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnEnd(object sender, EventArgs e)
        {
            var evt = (DownloadEvent)e;
            var item = downloads[evt.Recording];

            if (Recordings.InvokeRequired)
            {
                Recordings.Invoke(new MethodInvoker(delegate
                {
                    item.Remove();
                }));
            }
            else
            {
                item.Remove();
            }

            downloads[evt.Recording].Remove();
        }

        /// <summary>
        /// Descarga en progreso.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnDownloading(object sender, EventArgs e)
        {
            var evt = (DownloadProgress)e;
            var item = downloads[evt.Recording];

            if (Recordings.InvokeRequired)
            {
                Recordings.Invoke(new MethodInvoker(delegate
                {
                    item.SubItems[6].Text = string.Format("Descargando {0}%", evt.Progress);
                }));
            }
            else
            {
                item.SubItems[6].Text = string.Format("Descargando {0}%", evt.Progress);
            }
        }

        /// <summary>
        /// Error en la descarga.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnError(object sender, EventArgs e)
        {
            var evt = (DownloadError)e;
            var item = downloads[evt.Recording];

            LogEvent(
                string.Format("Se produjo un error al descargar la grabación {0} ({1}).", evt.Recording.Video.FileName, Downloader.GetRecordingFileName(evt.Recording)),
                evt.Code);

            if (Recordings.InvokeRequired)
            {
                Recordings.Invoke(new MethodInvoker(delegate
                {
                    item.SubItems[6].Text = "Error";
                }));
            }
            else
            {
                item.SubItems[6].Text = "Error";
            }
        }

        /// <summary>
        /// Cancelación del proceso de descarga.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_OnCancel(object sender, EventArgs e)
        {
            var evt = (DownloadEvent)e;
            var item = downloads[evt.Recording];

            if (Recordings.InvokeRequired)
            {
                Recordings.Invoke(new MethodInvoker(delegate
                {
                    item.SubItems[6].Text = "Cancelada";
                }));
            }
            else
            {
                item.SubItems[6].Text = "Cancelada";
            }
        }
        #endregion

        #region Canales
        /// <summary>
        /// Prepara el listado de canales.
        /// </summary>
        private void SetupChannels()
        {
            for (var i = 1; i <= 16; i++)
            {
                Channels.Items.Add(new ListViewItem
                {
                    Text = string.Format("{0:00} - {1}", i, Properties.Resources.ResourceManager.GetString("Channel" + i)),
                    Tag = new Channel(i),
                });
            }
        }
        #endregion

        #region Eventos
        /// <summary>
        /// Agrega un evento al registro de eventos.
        /// </summary>
        /// <param name="code">Código.</param>
        /// <param name="message">Mensaje.</param>
        private void LogEvent(string message, uint code = 0)
        {
            if (Events.InvokeRequired)
            {
                Events.Invoke(new MethodInvoker(delegate
                {
                    Events.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString("dd/MM/yyyy HH:mm"), message, code.ToString() }));
                    Events.Items[Events.Items.Count - 1].EnsureVisible();
                }));
            }
            else
            {
                Events.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString("dd/MM/yyyy HH:mm"), message, code.ToString() }));
                Events.Items[Events.Items.Count - 1].EnsureVisible();
            }
        }
        #endregion

        #region Búsqueda
        /// <summary>
        /// Intervalos.
        /// </summary>
        private enum Interval : int
        {
            today = 0,
            yesterday = 1,
            thisWeek = 2,
            lastWeek = 3,
            lastTwoWeeks = 4,
            thisMonth = 5,
            lastMonth = 6,
            customRange = 7,
        }

        /// <summary>
        /// Contador de grabaciones por canal.
        /// </summary>
        private int recordingsPerChannel;

        /// <summary>
        /// Prepara la búsqueda.
        /// </summary>
        private void SetupSearch()
        {
            Intervals.SelectedIndex = (int)Interval.lastWeek;

            Searcher.OnStart += Search_OnStart;
            Searcher.OnFinish += Search_OnFinish;
            Searcher.OnBegin += Search_OnBegin;
            Searcher.OnEnd += Search_OnEnd;
            Searcher.OnResult += Search_OnResult;
            Searcher.OnError += Search_OnError;
            Searcher.OnCancel += Search_OnCancel;
        }

        /// <summary>
        /// Responde al cambio de intervalo.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Intervals_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = (Interval)Intervals.SelectedIndex;
            switch (index)
            {
                case Interval.today:
                    Start.Value = DateTime.Today.Midnight();
                    End.Value = DateTime.Today.EndOfDay();
                    break;

                case Interval.yesterday:
                    Start.Value = DateTime.Today.PreviousDay().Midnight();
                    End.Value = DateTime.Today.PreviousDay().EndOfDay();
                    break;

                case Interval.thisWeek:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1);
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case Interval.lastWeek:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1).WeekEarlier();
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case Interval.lastTwoWeeks:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(-13);
                    End.Value = DateTime.Today.WeekEarlier().LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case Interval.thisMonth:
                    Start.Value = DateTime.Today.BeginningOfMonth();
                    End.Value = DateTime.Today.EndOfMonth();
                    break;

                case Interval.lastMonth:
                    Start.Value = DateTime.Today.PreviousMonth().BeginningOfMonth();
                    End.Value = DateTime.Today.PreviousMonth().EndOfMonth();
                    break;
            }

            Start.Enabled = index == Interval.customRange;
            End.Enabled = index == Interval.customRange;

            Start.Tag = Start.Value;
            End.Tag = End.Value;
        }

        /// <summary>
        /// Cambia el estado del botón de búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Channels_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Search.Enabled = Channels.CheckedIndices.Count > 0;
        }

        /// <summary>
        /// Ejecuta o cancela una búsqueda de grabaciones.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private async void Search_Click(object sender, EventArgs e)
        {
            if (End.Value < Start.Value)
            {
                MessageBox.Show("La fecha de comienzo debe ser menor a la fecha de finalización.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            if (Searcher.IsRunning)
            {
                Searcher.Cancel = true;

                return;
            }

            var channels = new List<Channel>();
            foreach (var item in Channels.CheckedItems)
            {
                channels.Add((Channel)((ListViewItem)item).Tag);
            }

            await Task.Run(() =>
            {
                Searcher.Search(channels.ToArray(), Start.Value, End.Value);
            });
        }

        /// <summary>
        /// Inicio de la búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnStart(object sender, EventArgs e)
        {
            LogEvent("Se ha iniciado la búsqueda.");

            Invoke(new MethodInvoker(delegate
            {
                Search.Text = "&Cancelar búsqueda";
                Channels.Enabled = false;
                Intervals.Enabled = false;
                Start.Enabled = false;
                End.Enabled = false;
                groupBox3.Enabled = false;
                Recordings.BeginUpdate();
                Recordings.Items.Clear();
            }));
        }

        /// <summary>
        /// Fin de la búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnFinish(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                Search.Text = "&Buscar grabaciones";

                Channels.Enabled = true;
                Intervals.Enabled = true;
                groupBox3.Enabled = true;
                Start.Enabled = (Interval)Intervals.SelectedIndex == Interval.customRange;
                End.Enabled = Start.Enabled;
                Recordings.EndUpdate();
                Download.Enabled = Recordings.Items.Count > 0;
            }));

            LogEvent("Se ha finalizado la búsqueda.");
        }

        /// <summary>
        /// Comienzo de la búsqueda en un canal.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnBegin(object sender, EventArgs e)
        {
            var evt = (SearchEvent)e;

            recordingsPerChannel = 0;

            LogEvent(string.Format("Comenzando búsqueda en el canal {0}.", evt.Channel.Number));
        }

        /// <summary>
        /// Fin de la búsqueda en un canal.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnEnd(object sender, EventArgs e)
        {
            var evt = (SearchEvent)e;

            LogEvent(string.Format("Se han encontrado {0} grabaciones.", recordingsPerChannel));
            LogEvent(string.Format("Finalizada búsqueda en el canal {0}.", evt.Channel.Number));
        }

        /// <summary>
        /// Procesa un resultado de la búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnResult(object sender, EventArgs e)
        {
            var evt = (SearchResult)e;

            if (Downloader.RecordingFileExists(evt.Recording))
            {
                return;
            }

            var item = new ListViewItem(new string[]
            {
                (Recordings.Items.Count + 1).ToString(),
                evt.Recording.Channel.Number.ToString(),
                evt.Recording.Video.FileName,
                evt.Recording.Video.FileSizeWithPrefix,
                evt.Recording.Video.Start.ToString("dd/MM/yyyy HH:mm:ss"),
                evt.Recording.Video.End.ToString("dd/MM/yyyy HH:mm:ss"),
                "Pendiente",
            })
            {
                Tag = evt.Recording
            };

            Recordings.Invoke(new MethodInvoker(delegate
            {
                Recordings.Items.Add(item);
            }));

            recordingsPerChannel++;
        }

        /// <summary>
        /// Responde a un error en la búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnError(object sender, EventArgs e)
        {
            LogEvent("Se ha producido un error.", ((SearchError)e).Code);
        }

        /// <summary>
        /// Responde a la cancelación de la búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        public void Search_OnCancel(object sender, EventArgs e)
        {
            LogEvent("Se ha cancelado la búsqueda.");
        }
        #endregion
    }
}
