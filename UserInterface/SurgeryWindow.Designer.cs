namespace UserInterface
{
    partial class SurgeryWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tab = new TabControl();
            tabRecord = new TabPage();
            numRoom = new NumericUpDown();
            imgCapture = new PictureBox();
            btnStop = new Button();
            btnStart = new Button();
            lblRoom = new Label();
            tabRecordings = new TabPage();
            listView1 = new ListView();
            txtDuration = new Label();
            txtStart = new Label();
            txtCaptures = new Label();
            txtRoom = new Label();
            lblStart = new Label();
            lblDuration = new Label();
            lblCaptures = new Label();
            lblRoom2 = new Label();
            dtpDay = new DateTimePicker();
            lblDay = new Label();
            tabOptions = new TabPage();
            lblDemo = new Label();
            btnSave = new Button();
            selNeuralNet = new ComboBox();
            lblNeuralNet = new Label();
            numInterval = new NumericUpDown();
            lblInterval = new Label();
            ckbDemo = new CheckBox();
            tab.SuspendLayout();
            tabRecord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imgCapture).BeginInit();
            tabRecordings.SuspendLayout();
            tabOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            SuspendLayout();
            // 
            // tab
            // 
            tab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tab.Controls.Add(tabRecord);
            tab.Controls.Add(tabRecordings);
            tab.Controls.Add(tabOptions);
            tab.Location = new Point(12, 12);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(758, 429);
            tab.TabIndex = 0;
            // 
            // tabRecord
            // 
            tabRecord.Controls.Add(numRoom);
            tabRecord.Controls.Add(imgCapture);
            tabRecord.Controls.Add(btnStop);
            tabRecord.Controls.Add(btnStart);
            tabRecord.Controls.Add(lblRoom);
            tabRecord.Location = new Point(4, 29);
            tabRecord.Name = "tabRecord";
            tabRecord.Padding = new Padding(3);
            tabRecord.Size = new Size(750, 396);
            tabRecord.TabIndex = 0;
            tabRecord.Text = "Gravar";
            tabRecord.UseVisualStyleBackColor = true;
            // 
            // numRoom
            // 
            numRoom.Location = new Point(6, 26);
            numRoom.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom.Name = "numRoom";
            numRoom.Size = new Size(125, 27);
            numRoom.TabIndex = 5;
            numRoom.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // imgCapture
            // 
            imgCapture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            imgCapture.BackColor = Color.DarkGray;
            imgCapture.Location = new Point(137, 6);
            imgCapture.Name = "imgCapture";
            imgCapture.Size = new Size(607, 384);
            imgCapture.SizeMode = PictureBoxSizeMode.Zoom;
            imgCapture.TabIndex = 4;
            imgCapture.TabStop = false;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.Enabled = false;
            btnStop.Location = new Point(6, 361);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(125, 29);
            btnStop.TabIndex = 3;
            btnStop.Text = "Parar";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.Location = new Point(6, 326);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(125, 29);
            btnStart.TabIndex = 2;
            btnStart.Text = "Iniciar";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblRoom
            // 
            lblRoom.AutoSize = true;
            lblRoom.Location = new Point(3, 3);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(37, 20);
            lblRoom.TabIndex = 0;
            lblRoom.Text = "Sala";
            // 
            // tabRecordings
            // 
            tabRecordings.Controls.Add(listView1);
            tabRecordings.Controls.Add(txtDuration);
            tabRecordings.Controls.Add(txtStart);
            tabRecordings.Controls.Add(txtCaptures);
            tabRecordings.Controls.Add(txtRoom);
            tabRecordings.Controls.Add(lblStart);
            tabRecordings.Controls.Add(lblDuration);
            tabRecordings.Controls.Add(lblCaptures);
            tabRecordings.Controls.Add(lblRoom2);
            tabRecordings.Controls.Add(dtpDay);
            tabRecordings.Controls.Add(lblDay);
            tabRecordings.Location = new Point(4, 29);
            tabRecordings.Name = "tabRecordings";
            tabRecordings.Padding = new Padding(3);
            tabRecordings.Size = new Size(750, 396);
            tabRecordings.TabIndex = 1;
            tabRecordings.Text = "Gravações";
            tabRecordings.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            listView1.Location = new Point(137, 3);
            listView1.Name = "listView1";
            listView1.Size = new Size(610, 387);
            listView1.TabIndex = 10;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // txtDuration
            // 
            txtDuration.AutoSize = true;
            txtDuration.Font = new Font("Segoe UI", 8F);
            txtDuration.Location = new Point(3, 373);
            txtDuration.Name = "txtDuration";
            txtDuration.Size = new Size(15, 19);
            txtDuration.TabIndex = 9;
            txtDuration.Text = "-";
            // 
            // txtStart
            // 
            txtStart.AutoSize = true;
            txtStart.Font = new Font("Segoe UI", 8F);
            txtStart.Location = new Point(3, 328);
            txtStart.Name = "txtStart";
            txtStart.Size = new Size(15, 19);
            txtStart.TabIndex = 8;
            txtStart.Text = "-";
            // 
            // txtCaptures
            // 
            txtCaptures.AutoSize = true;
            txtCaptures.Font = new Font("Segoe UI", 8F);
            txtCaptures.Location = new Point(3, 283);
            txtCaptures.Name = "txtCaptures";
            txtCaptures.Size = new Size(15, 19);
            txtCaptures.TabIndex = 7;
            txtCaptures.Text = "-";
            // 
            // txtRoom
            // 
            txtRoom.AutoSize = true;
            txtRoom.Font = new Font("Segoe UI", 8F);
            txtRoom.Location = new Point(3, 238);
            txtRoom.Name = "txtRoom";
            txtRoom.Size = new Size(15, 19);
            txtRoom.TabIndex = 6;
            txtRoom.Text = "-";
            // 
            // lblStart
            // 
            lblStart.AutoSize = true;
            lblStart.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblStart.Location = new Point(3, 309);
            lblStart.Name = "lblStart";
            lblStart.Size = new Size(45, 19);
            lblStart.TabIndex = 5;
            lblStart.Text = "Início";
            // 
            // lblDuration
            // 
            lblDuration.AutoSize = true;
            lblDuration.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblDuration.Location = new Point(3, 354);
            lblDuration.Name = "lblDuration";
            lblDuration.Size = new Size(65, 19);
            lblDuration.TabIndex = 4;
            lblDuration.Text = "Duração";
            // 
            // lblCaptures
            // 
            lblCaptures.AutoSize = true;
            lblCaptures.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblCaptures.Location = new Point(3, 264);
            lblCaptures.Name = "lblCaptures";
            lblCaptures.Size = new Size(68, 19);
            lblCaptures.TabIndex = 3;
            lblCaptures.Text = "Capturas";
            // 
            // lblRoom2
            // 
            lblRoom2.AutoSize = true;
            lblRoom2.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblRoom2.Location = new Point(3, 219);
            lblRoom2.Name = "lblRoom2";
            lblRoom2.Size = new Size(37, 19);
            lblRoom2.TabIndex = 2;
            lblRoom2.Text = "Sala";
            // 
            // dtpDay
            // 
            dtpDay.CustomFormat = "dd/MM/yyyy";
            dtpDay.Format = DateTimePickerFormat.Custom;
            dtpDay.Location = new Point(6, 26);
            dtpDay.Name = "dtpDay";
            dtpDay.Size = new Size(125, 27);
            dtpDay.TabIndex = 1;
            dtpDay.Value = new DateTime(2024, 9, 30, 20, 16, 0, 0);
            // 
            // lblDay
            // 
            lblDay.AutoSize = true;
            lblDay.Location = new Point(3, 3);
            lblDay.Name = "lblDay";
            lblDay.Size = new Size(32, 20);
            lblDay.TabIndex = 0;
            lblDay.Text = "Dia";
            // 
            // tabOptions
            // 
            tabOptions.Controls.Add(ckbDemo);
            tabOptions.Controls.Add(lblDemo);
            tabOptions.Controls.Add(btnSave);
            tabOptions.Controls.Add(selNeuralNet);
            tabOptions.Controls.Add(lblNeuralNet);
            tabOptions.Controls.Add(numInterval);
            tabOptions.Controls.Add(lblInterval);
            tabOptions.Location = new Point(4, 29);
            tabOptions.Name = "tabOptions";
            tabOptions.Padding = new Padding(3);
            tabOptions.Size = new Size(750, 396);
            tabOptions.TabIndex = 2;
            tabOptions.Text = "Opções";
            tabOptions.UseVisualStyleBackColor = true;
            // 
            // lblDemo
            // 
            lblDemo.AutoSize = true;
            lblDemo.Location = new Point(137, 145);
            lblDemo.Name = "lblDemo";
            lblDemo.Size = new Size(50, 20);
            lblDemo.TabIndex = 5;
            lblDemo.Text = "Demo";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(6, 361);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(125, 29);
            btnSave.TabIndex = 4;
            btnSave.Text = "Salvar";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // selNeuralNet
            // 
            selNeuralNet.FormattingEnabled = true;
            selNeuralNet.Items.AddRange(new object[] { "Yolo", "Custom Vision" });
            selNeuralNet.Location = new Point(137, 97);
            selNeuralNet.Name = "selNeuralNet";
            selNeuralNet.Size = new Size(125, 28);
            selNeuralNet.TabIndex = 3;
            // 
            // lblNeuralNet
            // 
            lblNeuralNet.AutoSize = true;
            lblNeuralNet.Location = new Point(137, 74);
            lblNeuralNet.Name = "lblNeuralNet";
            lblNeuralNet.Size = new Size(88, 20);
            lblNeuralNet.TabIndex = 2;
            lblNeuralNet.Text = "Rede neural";
            // 
            // numInterval
            // 
            numInterval.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numInterval.Location = new Point(137, 26);
            numInterval.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            numInterval.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new Size(125, 27);
            numInterval.TabIndex = 1;
            numInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.Location = new Point(137, 3);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(67, 20);
            lblInterval.TabIndex = 0;
            lblInterval.Text = "Intervalo";
            // 
            // ckbDemo
            // 
            ckbDemo.AutoSize = true;
            ckbDemo.Location = new Point(141, 170);
            ckbDemo.Name = "ckbDemo";
            ckbDemo.Size = new Size(18, 17);
            ckbDemo.TabIndex = 6;
            ckbDemo.UseVisualStyleBackColor = true;
            // 
            // SurgeryWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 453);
            Controls.Add(tab);
            MinimumSize = new Size(800, 500);
            Name = "SurgeryWindow";
            Text = "Form1";
            tab.ResumeLayout(false);
            tabRecord.ResumeLayout(false);
            tabRecord.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)imgCapture).EndInit();
            tabRecordings.ResumeLayout(false);
            tabRecordings.PerformLayout();
            tabOptions.ResumeLayout(false);
            tabOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tab;
        private TabPage tabRecord;
        private TabPage tabRecordings;
        private TabPage tabOptions;
        private Label lblRoom;
        private Button btnStop;
        private Button btnStart;
        private PictureBox imgCapture;
        private NumericUpDown numRoom;
        private DateTimePicker dtpDay;
        private Label lblDay;
        private Label lblRoom2;
        private Label lblCaptures;
        private Label lblDuration;
        private Label txtDuration;
        private Label txtStart;
        private Label txtCaptures;
        private Label txtRoom;
        private Label lblStart;
        private ListView listView1;
        private Label lblInterval;
        private Label lblNeuralNet;
        private NumericUpDown numInterval;
        private Button btnSave;
        private ComboBox selNeuralNet;
        private Label lblDemo;
        private CheckBox ckbDemo;
    }
}
