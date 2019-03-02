namespace Tax_Import_Data
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button5 = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.ImportWorker = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imgWait = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSleepTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAPIURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgWait)).BeginInit();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.Image = global::Tax_Import_Data.Properties.Resources.close;
            this.button5.Location = new System.Drawing.Point(370, 4);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(92, 75);
            this.button5.TabIndex = 17;
            this.toolTip1.SetToolTip(this.button5, "Exit");
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // btnImport
            // 
            this.btnImport.Image = global::Tax_Import_Data.Properties.Resources.history__1_;
            this.btnImport.Location = new System.Drawing.Point(279, 4);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(92, 75);
            this.btnImport.TabIndex = 16;
            this.toolTip1.SetToolTip(this.btnImport, "Manual Import");
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.button9_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::Tax_Import_Data.Properties.Resources.stop_playing1;
            this.btnStop.Location = new System.Drawing.Point(188, 4);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(92, 75);
            this.btnStop.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btnStop, "Stop Service");
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Image = global::Tax_Import_Data.Properties.Resources.play1;
            this.btnStart.Location = new System.Drawing.Point(97, 4);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(92, 75);
            this.btnStart.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btnStart, "Start Service");
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.button6_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Tax_Import_Data.Properties.Resources.save;
            this.btnSave.Location = new System.Drawing.Point(6, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 75);
            this.btnSave.TabIndex = 12;
            this.toolTip1.SetToolTip(this.btnSave, "Save Configuration");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.button5_Click);
            // 
            // ImportWorker
            // 
            this.ImportWorker.WorkerSupportsCancellation = true;
            this.ImportWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ImportWorker_DoWork);
            this.ImportWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ImportWorker_RunWorkerCompleted);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Tax Import Data is still running";
            this.notifyIcon1.BalloonTipTitle = "Tax Import Data";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Tax Import Data";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.BalloonTipClicked += new System.EventHandler(this.notifyIcon1_BalloonTipClicked);
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(115, 52);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.imgWait);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtSleepTime);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtAPIURL);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(11, 89);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(451, 201);
            this.panel1.TabIndex = 5;
            // 
            // imgWait
            // 
            this.imgWait.Image = global::Tax_Import_Data.Properties.Resources.i_animated_loading_32_2;
            this.imgWait.InitialImage = null;
            this.imgWait.Location = new System.Drawing.Point(358, 84);
            this.imgWait.Margin = new System.Windows.Forms.Padding(4);
            this.imgWait.Name = "imgWait";
            this.imgWait.Size = new System.Drawing.Size(47, 43);
            this.imgWait.TabIndex = 19;
            this.imgWait.TabStop = false;
            this.imgWait.Visible = false;
            this.imgWait.Click += new System.EventHandler(this.imgWait_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 178);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(513, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "Change connection,configuration requires restarting the service and  application";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(257, 89);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Hours";
            // 
            // txtSleepTime
            // 
            this.txtSleepTime.Location = new System.Drawing.Point(117, 84);
            this.txtSleepTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtSleepTime.Name = "txtSleepTime";
            this.txtSleepTime.Size = new System.Drawing.Size(128, 22);
            this.txtSleepTime.TabIndex = 6;
            this.txtSleepTime.TextChanged += new System.EventHandler(this.txtSleepTime_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 87);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Run Every";
            // 
            // txtAPIURL
            // 
            this.txtAPIURL.Location = new System.Drawing.Point(110, 41);
            this.txtAPIURL.Margin = new System.Windows.Forms.Padding(4);
            this.txtAPIURL.Name = "txtAPIURL";
            this.txtAPIURL.Size = new System.Drawing.Size(320, 22);
            this.txtAPIURL.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 44);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "TAX API URL";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Tax_Import_Data.Properties.Resources.orangeburst1;
            this.ClientSize = new System.Drawing.Size(470, 308);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TAX Import Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgWait)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAPIURL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSleepTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnImport;
        private System.ComponentModel.BackgroundWorker ImportWorker;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox imgWait;
    }
}

