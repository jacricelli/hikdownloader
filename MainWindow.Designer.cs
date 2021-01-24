namespace HikDownloader
{
    using HCNetSDK;

    partial class MainWindow
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            Session.Logout();

            SDK.Cleanup();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Download = new System.Windows.Forms.Button();
            this.Browse = new System.Windows.Forms.Button();
            this.DownloadDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Recordings = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Periods = new System.Windows.Forms.ComboBox();
            this.End = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.Start = new System.Windows.Forms.DateTimePicker();
            this.Events = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Channels = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Download);
            this.groupBox3.Controls.Add(this.Browse);
            this.groupBox3.Controls.Add(this.DownloadDir);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.Recordings);
            this.groupBox3.Location = new System.Drawing.Point(208, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(704, 544);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Grabaciones";
            // 
            // Download
            // 
            this.Download.Enabled = false;
            this.Download.Location = new System.Drawing.Point(574, 504);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(123, 32);
            this.Download.TabIndex = 8;
            this.Download.Text = "&Descargar";
            this.Download.UseVisualStyleBackColor = true;
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(389, 512);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(40, 24);
            this.Browse.TabIndex = 3;
            this.Browse.Text = "...";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // DownloadDir
            // 
            this.DownloadDir.Location = new System.Drawing.Point(8, 512);
            this.DownloadDir.Name = "DownloadDir";
            this.DownloadDir.ReadOnly = true;
            this.DownloadDir.Size = new System.Drawing.Size(376, 23);
            this.DownloadDir.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 492);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Guardar grabaciones en:";
            // 
            // Recordings
            // 
            this.Recordings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.Recordings.FullRowSelect = true;
            this.Recordings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Recordings.HideSelection = false;
            this.Recordings.Location = new System.Drawing.Point(8, 24);
            this.Recordings.MultiSelect = false;
            this.Recordings.Name = "Recordings";
            this.Recordings.Size = new System.Drawing.Size(688, 464);
            this.Recordings.TabIndex = 0;
            this.Recordings.UseCompatibleStateImageBehavior = false;
            this.Recordings.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Archivo";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Tamaño";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Comienzo";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Finalización";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Estado";
            this.columnHeader5.Width = 100;
            // 
            // Search
            // 
            this.Search.Enabled = false;
            this.Search.Location = new System.Drawing.Point(32, 500);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(128, 32);
            this.Search.TabIndex = 4;
            this.Search.Text = "&Buscar grabaciones";
            this.Search.UseVisualStyleBackColor = true;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 445);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Finalización";
            // 
            // Periods
            // 
            this.Periods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Periods.FormattingEnabled = true;
            this.Periods.Items.AddRange(new object[] {
            "Hoy",
            "Ayer",
            "Esta semana",
            "Semana anterior",
            "Últimas 2 semanas",
            "Este mes",
            "Mes anterior",
            "Rango personalizado"});
            this.Periods.Location = new System.Drawing.Point(8, 363);
            this.Periods.Name = "Periods";
            this.Periods.Size = new System.Drawing.Size(176, 23);
            this.Periods.TabIndex = 4;
            this.Periods.SelectedIndexChanged += new System.EventHandler(this.Periods_SelectedIndexChanged);
            // 
            // End
            // 
            this.End.Checked = false;
            this.End.CustomFormat = " dd-MM-yyyy HH:mm:ss";
            this.End.Enabled = false;
            this.End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.End.Location = new System.Drawing.Point(8, 464);
            this.End.Name = "End";
            this.End.Size = new System.Drawing.Size(176, 23);
            this.End.TabIndex = 6;
            this.End.Validating += new System.ComponentModel.CancelEventHandler(this.End_Validating);
            this.End.Validated += new System.EventHandler(this.End_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 392);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Comienzo";
            // 
            // Start
            // 
            this.Start.Checked = false;
            this.Start.CustomFormat = " dd-MM-yyyy HH:mm:ss";
            this.Start.Enabled = false;
            this.Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Start.Location = new System.Drawing.Point(8, 411);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(176, 23);
            this.Start.TabIndex = 4;
            this.Start.Validating += new System.ComponentModel.CancelEventHandler(this.Start_Validating);
            this.Start.Validated += new System.EventHandler(this.Start_Validated);
            // 
            // Events
            // 
            this.Events.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader8});
            this.Events.FullRowSelect = true;
            this.Events.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Events.HideSelection = false;
            this.Events.Location = new System.Drawing.Point(8, 560);
            this.Events.MultiSelect = false;
            this.Events.Name = "Events";
            this.Events.Size = new System.Drawing.Size(904, 120);
            this.Events.TabIndex = 8;
            this.Events.UseCompatibleStateImageBehavior = false;
            this.Events.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Código";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Mensaje";
            this.columnHeader8.Width = 800;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Search);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.Channels);
            this.groupBox4.Controls.Add(this.End);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.Start);
            this.groupBox4.Controls.Add(this.Periods);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(8, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(192, 544);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Canales";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 344);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Período";
            // 
            // Channels
            // 
            this.Channels.CheckBoxes = true;
            this.Channels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7});
            this.Channels.FullRowSelect = true;
            this.Channels.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.Channels.HideSelection = false;
            this.Channels.Location = new System.Drawing.Point(8, 24);
            this.Channels.Name = "Channels";
            this.Channels.Size = new System.Drawing.Size(176, 312);
            this.Channels.TabIndex = 11;
            this.Channels.UseCompatibleStateImageBehavior = false;
            this.Channels.View = System.Windows.Forms.View.Details;
            this.Channels.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.Channels_ItemChecked);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Width = 140;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 689);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.Events);
            this.Controls.Add(this.groupBox3);
            this.Enabled = false;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HikDownloader";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.TextBox DownloadDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView Recordings;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button Search;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Periods;
        private System.Windows.Forms.DateTimePicker End;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker Start;
        private new System.Windows.Forms.ListView Events;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView Channels;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}

