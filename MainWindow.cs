namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using FluentDateTime;
    using HCNetSDK;

    /// <summary>
    /// Ventana principal.
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Tipos de períodos.
        /// </summary>
        private enum PeriodsTypes : int
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
        /// Estado de la descarga.
        /// </summary>
        private enum DownloadStatus : int
        {
            /// <summary>
            /// Completada.
            /// </summary>
            completed = 0,

            /// <summary>
            /// Incompleta.
            /// </summary>
            incomplete = 1,

            /// <summary>
            /// Pendiente.
            /// </summary>
            pending = 2
        }

        /// <summary>
        /// Contador de grabaciones por canal.
        /// </summary>
        private int recordingsPerChannel;

        /// <summary>
        /// Grabaciones pendiente de descarga.
        /// </summary>
        private List<int> pendingRecordings;

        /// <summary>
        /// Indica si se están descargando archivos.
        /// </summary>
        private bool isDownloading = false;

        /// <summary>
        /// Índice de la grabación que se está descargando.
        /// </summary>
        private int currentRecordingIndex = -1;

        /// <summary>
        /// Descarga actual.
        /// </summary>
        private int currentDownload = -1;

        /// <summary>
        /// Manejador de la descarga.
        /// </summary>
        private int downloadHandle = -1;

        /// <summary>
        /// Ruta absoluta del archivo de la descarga actual.
        /// </summary>
        private string currentFile;

        /// <summary>
        /// Ruta absoluta del archivo temporal de la descarga actual.
        /// </summary>
        private string currentTempFile;

        /// <summary>
        /// Momento en el que se inicia la descarga.
        /// </summary>
        private DateTime downloadStartTime;

        /// <summary>
        /// Tamaño descargado.
        /// </summary>
        private ulong downloadedSize = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

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
                    Events.Items.Add(new ListViewItem(new string[] { message, code.ToString() }));
                }));
            }
            else
            {
                Events.Items.Add(new ListViewItem(new string[] { message, code.ToString() }));
            }
        }

        /// <summary>
        /// Responde a la carga del formulario.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Downloads == string.Empty)
            {
                Properties.Settings.Default.Downloads = Util.GetDirectory("downloads");
                Properties.Settings.Default.Save();
            }

            for (var i = 1; i <= 16; i++)
            {
                Channels.Items.Add(new ListViewItem
                {
                    Text = string.Format("{0:00} - {1}", i, Properties.Resources.ResourceManager.GetString("Channel" + i)),
                    Tag = new Channel(i),
                });
            }

            Periods.SelectedIndex = (int)PeriodsTypes.lastWeek;

            DownloadDir.Text = Properties.Settings.Default.Downloads;

            Searcher.OnStart += Search_OnStart;
            Searcher.OnFinish += Search_OnFinish;
            Searcher.OnBegin += Search_OnBegin;
            Searcher.OnEnd += Search_OnEnd;
            Searcher.OnResult += Search_OnResult;
            Searcher.OnError += Search_OnError;
            Searcher.OnCancel += Search_OnCancel;
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

                    if (SDK.EnableLogging(Util.GetDirectory("logs")))
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
        /// Guarda el valor validado en las propiedades del control.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Start_Validated(object sender, EventArgs e)
        {
            Start.Tag = Start.Value;
        }

        /// <summary>
        /// Valida que la fecha de comienzo es menor o igual que la fecha de finalización.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Start_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Start.Value >= End.Value)
            {
                MessageBox.Show("La fecha de comienzo debe ser menor a la fecha de finalización.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                e.Cancel = true;
                Start.Value = (DateTime)Start.Tag;
            }
        }

        /// <summary>
        /// Guarda el valor validado en las propiedades del control.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void End_Validated(object sender, EventArgs e)
        {
            End.Tag = End.Value;
        }

        /// <summary>
        /// Valida que la fecha de finalización es mayor o igual que la fecha de comienzo.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void End_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (End.Value <= Start.Value)
            {
                MessageBox.Show("La fecha de finalización debe ser mayor a la fecha de comienzo.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                e.Cancel = true;
                End.Value = (DateTime)End.Tag;
            }
        }

        /// <summary>
        /// Responde al cambio de período.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Periods_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = (PeriodsTypes)Periods.SelectedIndex;
            switch (index)
            {
                case PeriodsTypes.today:
                    Start.Value = DateTime.Today.Midnight();
                    End.Value = DateTime.Today.EndOfDay();
                    break;

                case PeriodsTypes.yesterday:
                    Start.Value = DateTime.Today.PreviousDay().Midnight();
                    End.Value = DateTime.Today.PreviousDay().EndOfDay();
                    break;

                case PeriodsTypes.thisWeek:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1);
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case PeriodsTypes.lastWeek:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1).WeekEarlier();
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case PeriodsTypes.lastTwoWeeks:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(-13);
                    End.Value = DateTime.Today.WeekEarlier().LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case PeriodsTypes.thisMonth:
                    Start.Value = DateTime.Today.BeginningOfMonth();
                    End.Value = DateTime.Today.EndOfMonth();
                    break;

                case PeriodsTypes.lastMonth:
                    Start.Value = DateTime.Today.PreviousMonth().BeginningOfMonth();
                    End.Value = DateTime.Today.PreviousMonth().EndOfMonth();
                    break;
            }

            Start.Enabled = index == PeriodsTypes.customRange;
            End.Enabled = index == PeriodsTypes.customRange;

            Start.Tag = Start.Value;
            End.Tag = End.Value;
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
                }
            }
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
            if (Searcher.IsRunning)
            {
                Searcher.Cancel();

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
                Periods.Enabled = false;
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
                Periods.Enabled = true;
                groupBox3.Enabled = true;
                Start.Enabled = (PeriodsTypes)Periods.SelectedIndex == PeriodsTypes.customRange;
                End.Enabled = Start.Enabled;
                Recordings.EndUpdate();
                groupBox3.Text = string.Format("Grabaciones ({0})", Recordings.Items.Count);
                Download.Enabled = Recordings.Items.Count > 0;

                currentDownload = -1;
                currentFile = null;
                currentTempFile = null;
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

            var status = string.Empty;
            switch (CheckDownloadStatus(evt.Recording))
            {
                case DownloadStatus.completed:
                    status = "Completada";
                    break;

                case DownloadStatus.incomplete:
                    status = "Incompleta";
                    break;

                case DownloadStatus.pending:
                    status = "Pendiente";
                    break;
            }

            var item = new ListViewItem(new string[]
            {
                (Recordings.Items.Count + 1).ToString(),
                evt.Recording.Channel.Number.ToString(),
                evt.Recording.Video.FileName,
                evt.Recording.Video.FileSizeWithPrefix,
                evt.Recording.Video.Start.ToString("dd/MM/yyyy hh:mm:ss"),
                evt.Recording.Video.End.ToString("dd/MM/yyyy hh:mm:ss"),
                status,
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

        /// <summary>
        /// Descarga grabaciones.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Download_Click(object sender, EventArgs e)
        {
            if (!isDownloading)
            {
                if (Recordings.Items.Count > 0)
                {
                    pendingRecordings = new List<int>();
                    downloadedSize = 0;

                    for (var i = 0; i < Recordings.Items.Count; i++)
                    {
                        var recording = (Recording)Recordings.Items[i].Tag;
                        if (CheckDownloadStatus(recording) != DownloadStatus.completed)
                        {
                            pendingRecordings.Add(i);
                        }
                    }

                    DownloadRecording();
                }
            }
            else
            {
                Downloader.StopDownload(downloadHandle);

                Recordings.Items[currentRecordingIndex].SubItems[6].Text = "Cancelada";

                DownloadManager.Enabled = false;
                downloadHandle = -1;
                currentDownload--;
                isDownloading = false;

                Download.Text = "&Descargar";
                groupBox4.Enabled = true;
                Browse.Enabled = true;
            }
        }

        /// <summary>
        /// Método auxiliar para descargar grabaciones.
        /// </summary>
        private void DownloadRecording()
        {
            if (currentDownload < 0)
            {
                currentDownload = 0;
            }
            else
            {
                currentDownload++;
            }

            if (pendingRecordings.Count > 0 && currentDownload < pendingRecordings.Count)
            {
                currentRecordingIndex = pendingRecordings[currentDownload];
                var recording = (Recording)Recordings.Items[currentRecordingIndex].Tag;
                currentTempFile = GetRecordingPath(recording, "tmp");
                currentFile = GetRecordingPath(recording, "avi");

                var directory = Path.GetDirectoryName(currentTempFile);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                downloadHandle = Downloader.PrepareDownload(Session.User.Identifier, recording.Video.FileName, currentTempFile);
                if (downloadHandle > -1)
                {
                    if (Downloader.StartDownload(downloadHandle))
                    {
                        if (!isDownloading)
                        {
                            downloadStartTime = DateTime.Now;
                            isDownloading = true;

                            Download.Text = "&Cancelar";
                            groupBox4.Enabled = false;
                            Browse.Enabled = false;
                        }

                        Recordings.Items[currentRecordingIndex].Selected = true;
                        Recordings.Items[currentRecordingIndex].EnsureVisible();

                        DownloadManager.Enabled = true;
                    }
                    else
                    {
                        Recordings.Items[currentRecordingIndex].SubItems[6].Text = "Error";

                        currentDownload--;

                        LogEvent("Se ha producido un error al comenzar la descarga.", SDK.GetLastError());
                    }
                }
                else
                {
                    Recordings.Items[currentRecordingIndex].SubItems[6].Text = "Error";

                    currentDownload--;

                    LogEvent("Se ha producido un error al preparar la descarga.", SDK.GetLastError());
                }
            }
            else
            {
                if (isDownloading)
                {
                    isDownloading = false;

                    Download.Text = "&Descargar";
                    groupBox4.Enabled = true;
                    Browse.Enabled = true;
                }

                DownloadManager.Enabled = false;

                currentDownload = -1;
                currentRecordingIndex = -1;
                downloadHandle = -1;
            }
        }



        /// <summary>
        /// Gestiona la descarga de grabaciones.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void DownloadManager_Tick(object sender, EventArgs e)
        {
            var timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(downloadStartTime).Ticks * (pendingRecordings.Count - (currentDownload + 1)) / (currentDownload + 1));
            System.Diagnostics.Debug.WriteLine(timeRemaining.ToDisplayString());

            var s = (DateTime.Now - downloadStartTime).TotalSeconds;
            var r = Convert.ToUInt64(downloadedSize / s);

            System.Diagnostics.Debug.WriteLine(Util.ToReadableSize(r));

            var progress = Downloader.GetDownloadProgress(downloadHandle);
            if (progress < 0)
            {
                Downloader.StopDownload(downloadHandle);

                Recordings.Items[currentRecordingIndex].SubItems[6].Text = "Error";

                DownloadManager.Enabled = false;
                downloadHandle = -1;
                currentDownload--;
                isDownloading = false;

                Download.Text = "&Descargar";
                groupBox4.Enabled = true;
                Browse.Enabled = true;

                LogEvent("Se ha producido un error al obtener el progreso de la descarga.", SDK.GetLastError());
            }
            else if (progress >= 0 && progress < 100)
            {
                Recordings.Items[currentRecordingIndex].SubItems[6].Text = string.Format("Descargando {0}%", progress);
            }
            else if (progress == 100)
            {
                Downloader.StopDownload(downloadHandle);

                DownloadManager.Enabled = false;
                downloadHandle = -1;

                File.Move(currentTempFile, currentFile);

                Recordings.Items[currentRecordingIndex].SubItems[6].Text = "Completada";

                downloadedSize += ((Recording)Recordings.Items[currentRecordingIndex].Tag).Video.FileSize;

                DownloadRecording();
            }
            else if (progress == 200)
            {
                Downloader.StopDownload(downloadHandle);

                Recordings.Items[currentRecordingIndex].SubItems[6].Text = "Error";

                DownloadManager.Enabled = false;
                downloadHandle = -1;
                currentDownload--;
                isDownloading = false;

                Download.Text = "&Descargar";
                groupBox4.Enabled = true;
                Browse.Enabled = true;

                LogEvent("Se ha producido un error al consultar el progreso de la descarga.", SDK.GetLastError());
            }
        }

        /// <summary>
        /// Obtiene la ruta absoluta para una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="extension">Extensión del archivo.</param>
        /// <returns>Ruta absoluta.</returns>
        private static string GetRecordingPath(Recording recording, string extension)
        {
            return string.Format(
                "{0}\\{1}-{2:00}\\Channel {3:00}\\{4}_{5}_{6}.{7}",
                Properties.Settings.Default.Downloads,
                recording.Video.Start.Year,
                recording.Video.Start.Month,
                recording.Channel.Number,
                recording.Video.Start.ToString("yyyy-MM-dd"),
                recording.Video.Start.ToString("hhmmss"),
                recording.Video.End.ToString("hhmmss"),
                extension);
        }

        /// <summary>
        /// Comprueba el estado de una descarga.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <returns>Estado de la descarga.</returns>
        private static DownloadStatus CheckDownloadStatus(Recording recording)
        {
            if (File.Exists(GetRecordingPath(recording, "avi")))
            {
                return DownloadStatus.completed;
            }
            else if (File.Exists(GetRecordingPath(recording, "tmp")))
            {
                return DownloadStatus.incomplete;
            }
            else
            {
                return DownloadStatus.pending;
            }
        }
    }
}
