using System.Windows.Forms;

namespace Process_Auto_Relaunch
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBoxActions = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonRestartTimer = new System.Windows.Forms.RadioButton();
            this.buttonSetProgramStart = new System.Windows.Forms.Button();
            this.groupBoxProgramStart = new System.Windows.Forms.GroupBox();
            this.labelArguments = new System.Windows.Forms.Label();
            this.groupBoxProcessName = new System.Windows.Forms.GroupBox();
            this.groupBoxEnabled = new System.Windows.Forms.GroupBox();
            this.radioButtonEnableWathing = new System.Windows.Forms.RadioButton();
            this.radioButtonDisableWathing = new System.Windows.Forms.RadioButton();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.myBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.checkBoxSaveState = new System.Windows.Forms.CheckBox();
            this.textBoxProcessName = new System.Windows.Forms.TextBox();
            this.textBoxArguments = new System.Windows.Forms.TextBox();
            this.labelProgramStartPath = new System.Windows.Forms.Label();
            this.checkBoxCheckProcess = new System.Windows.Forms.CheckBox();
            this.radioButtonRestartNow = new System.Windows.Forms.RadioButton();
            this.groupBoxActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBoxProgramStart.SuspendLayout();
            this.groupBoxProcessName.SuspendLayout();
            this.groupBoxEnabled.SuspendLayout();
            this.groupBoxStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxActions
            // 
            this.groupBoxActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxActions.Controls.Add(this.numericUpDown1);
            this.groupBoxActions.Controls.Add(this.label1);
            this.groupBoxActions.Controls.Add(this.checkBoxCheckProcess);
            this.groupBoxActions.Controls.Add(this.radioButtonRestartTimer);
            this.groupBoxActions.Controls.Add(this.radioButtonRestartNow);
            this.groupBoxActions.Location = new System.Drawing.Point(10, 64);
            this.groupBoxActions.Name = "groupBoxActions";
            this.groupBoxActions.Size = new System.Drawing.Size(287, 111);
            this.groupBoxActions.TabIndex = 0;
            this.groupBoxActions.TabStop = false;
            this.groupBoxActions.Text = "Действие после отсутствия процесса";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(149, 19);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1800,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(54, 20);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(209, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "секунд";
            // 
            // radioButtonRestartTimer
            // 
            this.radioButtonRestartTimer.AutoSize = true;
            this.radioButtonRestartTimer.Checked = true;
            this.radioButtonRestartTimer.Location = new System.Drawing.Point(9, 19);
            this.radioButtonRestartTimer.Name = "radioButtonRestartTimer";
            this.radioButtonRestartTimer.Size = new System.Drawing.Size(134, 17);
            this.radioButtonRestartTimer.TabIndex = 1;
            this.radioButtonRestartTimer.TabStop = true;
            this.radioButtonRestartTimer.Text = "Перезапустить через";
            this.radioButtonRestartTimer.UseVisualStyleBackColor = true;
            // 
            // buttonSetProgramStart
            // 
            this.buttonSetProgramStart.Location = new System.Drawing.Point(4, 19);
            this.buttonSetProgramStart.Name = "buttonSetProgramStart";
            this.buttonSetProgramStart.Size = new System.Drawing.Size(67, 28);
            this.buttonSetProgramStart.TabIndex = 1;
            this.buttonSetProgramStart.Text = "Обзор";
            this.buttonSetProgramStart.UseVisualStyleBackColor = true;
            this.buttonSetProgramStart.Click += new System.EventHandler(this.buttonSetProgramStart_Click);
            // 
            // groupBoxProgramStart
            // 
            this.groupBoxProgramStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProgramStart.Controls.Add(this.textBoxArguments);
            this.groupBoxProgramStart.Controls.Add(this.labelArguments);
            this.groupBoxProgramStart.Controls.Add(this.labelProgramStartPath);
            this.groupBoxProgramStart.Controls.Add(this.buttonSetProgramStart);
            this.groupBoxProgramStart.Location = new System.Drawing.Point(10, 180);
            this.groupBoxProgramStart.Name = "groupBoxProgramStart";
            this.groupBoxProgramStart.Size = new System.Drawing.Size(287, 113);
            this.groupBoxProgramStart.TabIndex = 6;
            this.groupBoxProgramStart.TabStop = false;
            this.groupBoxProgramStart.Text = "Запуск программы";
            // 
            // labelArguments
            // 
            this.labelArguments.AutoSize = true;
            this.labelArguments.Location = new System.Drawing.Point(4, 72);
            this.labelArguments.Name = "labelArguments";
            this.labelArguments.Size = new System.Drawing.Size(153, 13);
            this.labelArguments.TabIndex = 3;
            this.labelArguments.Text = "Дополнительные аргументы";
            // 
            // groupBoxProcessName
            // 
            this.groupBoxProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProcessName.Controls.Add(this.textBoxProcessName);
            this.groupBoxProcessName.Location = new System.Drawing.Point(10, 10);
            this.groupBoxProcessName.Name = "groupBoxProcessName";
            this.groupBoxProcessName.Size = new System.Drawing.Size(287, 50);
            this.groupBoxProcessName.TabIndex = 8;
            this.groupBoxProcessName.TabStop = false;
            this.groupBoxProcessName.Text = "Название наблюдаемого процесса";
            // 
            // groupBoxEnabled
            // 
            this.groupBoxEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEnabled.Controls.Add(this.checkBoxSaveState);
            this.groupBoxEnabled.Controls.Add(this.radioButtonEnableWathing);
            this.groupBoxEnabled.Controls.Add(this.radioButtonDisableWathing);
            this.groupBoxEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxEnabled.Location = new System.Drawing.Point(10, 298);
            this.groupBoxEnabled.Name = "groupBoxEnabled";
            this.groupBoxEnabled.Size = new System.Drawing.Size(287, 72);
            this.groupBoxEnabled.TabIndex = 9;
            this.groupBoxEnabled.TabStop = false;
            this.groupBoxEnabled.Text = "Состояние";
            // 
            // radioButtonEnableWathing
            // 
            this.radioButtonEnableWathing.AutoSize = true;
            this.radioButtonEnableWathing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioButtonEnableWathing.Location = new System.Drawing.Point(106, 19);
            this.radioButtonEnableWathing.Name = "radioButtonEnableWathing";
            this.radioButtonEnableWathing.Size = new System.Drawing.Size(75, 17);
            this.radioButtonEnableWathing.TabIndex = 1;
            this.radioButtonEnableWathing.Text = "Включено";
            this.radioButtonEnableWathing.UseVisualStyleBackColor = true;
            this.radioButtonEnableWathing.CheckedChanged += new System.EventHandler(this.radioButtonEnableWathing_CheckedChanged);
            // 
            // radioButtonDisableWathing
            // 
            this.radioButtonDisableWathing.AutoSize = true;
            this.radioButtonDisableWathing.Checked = true;
            this.radioButtonDisableWathing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioButtonDisableWathing.Location = new System.Drawing.Point(8, 19);
            this.radioButtonDisableWathing.Name = "radioButtonDisableWathing";
            this.radioButtonDisableWathing.Size = new System.Drawing.Size(83, 17);
            this.radioButtonDisableWathing.TabIndex = 0;
            this.radioButtonDisableWathing.TabStop = true;
            this.radioButtonDisableWathing.Text = "Выключено";
            this.radioButtonDisableWathing.UseVisualStyleBackColor = true;
            this.radioButtonDisableWathing.CheckedChanged += new System.EventHandler(this.radioButtonDisableWathing_CheckedChanged);
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxStatus.Controls.Add(this.labelStatus);
            this.groupBoxStatus.Location = new System.Drawing.Point(10, 375);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(287, 52);
            this.groupBoxStatus.TabIndex = 10;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Текущий статус";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(15, 25);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(74, 13);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Не запущено";
            // 
            // myBackgroundWorker
            // 
            this.myBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerDoWork);
            this.myBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // checkBoxSaveState
            // 
            this.checkBoxSaveState.AutoSize = true;
            this.checkBoxSaveState.Checked = global::RelaunchProcess.Properties.Settings.Default.saveOldState;
            this.checkBoxSaveState.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSaveState.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::RelaunchProcess.Properties.Settings.Default, "saveOldState", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxSaveState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBoxSaveState.Location = new System.Drawing.Point(15, 41);
            this.checkBoxSaveState.Name = "checkBoxSaveState";
            this.checkBoxSaveState.Size = new System.Drawing.Size(201, 17);
            this.checkBoxSaveState.TabIndex = 2;
            this.checkBoxSaveState.Text = "Запоминать последнее состояние";
            this.checkBoxSaveState.UseVisualStyleBackColor = true;
            // 
            // textBoxProcessName
            // 
            this.textBoxProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProcessName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::RelaunchProcess.Properties.Settings.Default, "processName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxProcessName.Location = new System.Drawing.Point(5, 20);
            this.textBoxProcessName.Name = "textBoxProcessName";
            this.textBoxProcessName.Size = new System.Drawing.Size(277, 20);
            this.textBoxProcessName.TabIndex = 3;
            this.textBoxProcessName.Text = global::RelaunchProcess.Properties.Settings.Default.processName;
            // 
            // textBoxArguments
            // 
            this.textBoxArguments.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::RelaunchProcess.Properties.Settings.Default, "arguments", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxArguments.Location = new System.Drawing.Point(5, 88);
            this.textBoxArguments.Name = "textBoxArguments";
            this.textBoxArguments.Size = new System.Drawing.Size(275, 20);
            this.textBoxArguments.TabIndex = 4;
            this.textBoxArguments.Text = global::RelaunchProcess.Properties.Settings.Default.arguments;
            // 
            // labelProgramStartPath
            // 
            this.labelProgramStartPath.AutoSize = true;
            this.labelProgramStartPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::RelaunchProcess.Properties.Settings.Default, "startProgramPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelProgramStartPath.Location = new System.Drawing.Point(5, 50);
            this.labelProgramStartPath.Name = "labelProgramStartPath";
            this.labelProgramStartPath.Size = new System.Drawing.Size(0, 13);
            this.labelProgramStartPath.TabIndex = 2;
            this.labelProgramStartPath.Text = global::RelaunchProcess.Properties.Settings.Default.startProgramPath;
            // 
            // checkBoxCheckProcess
            // 
            this.checkBoxCheckProcess.AutoSize = true;
            this.checkBoxCheckProcess.Checked = global::RelaunchProcess.Properties.Settings.Default.checkBeforeStart;
            this.checkBoxCheckProcess.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCheckProcess.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::RelaunchProcess.Properties.Settings.Default, "checkBeforeStart", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxCheckProcess.Location = new System.Drawing.Point(12, 80);
            this.checkBoxCheckProcess.Name = "checkBoxCheckProcess";
            this.checkBoxCheckProcess.Size = new System.Drawing.Size(153, 17);
            this.checkBoxCheckProcess.TabIndex = 2;
            this.checkBoxCheckProcess.Text = "Не создавать дубликаты";
            this.checkBoxCheckProcess.UseVisualStyleBackColor = true;
            // 
            // radioButtonRestartNow
            // 
            this.radioButtonRestartNow.AutoSize = true;
            this.radioButtonRestartNow.Checked = global::RelaunchProcess.Properties.Settings.Default.restartNow;
            this.radioButtonRestartNow.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::RelaunchProcess.Properties.Settings.Default, "restartNow", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioButtonRestartNow.Location = new System.Drawing.Point(8, 41);
            this.radioButtonRestartNow.Name = "radioButtonRestartNow";
            this.radioButtonRestartNow.Size = new System.Drawing.Size(135, 17);
            this.radioButtonRestartNow.TabIndex = 0;
            this.radioButtonRestartNow.Text = "Перезапускать сразу";
            this.radioButtonRestartNow.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 445);
            this.Controls.Add(this.groupBoxStatus);
            this.Controls.Add(this.groupBoxEnabled);
            this.Controls.Add(this.groupBoxProcessName);
            this.Controls.Add(this.groupBoxProgramStart);
            this.Controls.Add(this.groupBoxActions);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Relaunch Process";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxActions.ResumeLayout(false);
            this.groupBoxActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBoxProgramStart.ResumeLayout(false);
            this.groupBoxProgramStart.PerformLayout();
            this.groupBoxProcessName.ResumeLayout(false);
            this.groupBoxProcessName.PerformLayout();
            this.groupBoxEnabled.ResumeLayout(false);
            this.groupBoxEnabled.PerformLayout();
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBoxActions;
        private Label label1;
        private CheckBox checkBoxCheckProcess;
        private RadioButton radioButtonRestartTimer;
        private RadioButton radioButtonRestartNow;
        private Button buttonSetProgramStart;
        private TextBox textBoxProcessName;
        private GroupBox groupBoxProgramStart;
        private Label labelProgramStartPath;
        private RadioButton radioButtonEnableWathing;
        private RadioButton radioButtonDisableWathing;
        private CheckBox checkBoxSaveState;
        private GroupBox groupBoxProcessName;
        private GroupBox groupBoxEnabled;
        private GroupBox groupBoxStatus;
        private TextBox textBoxArguments;
        private Label labelArguments;
        public Label labelStatus;
        private System.ComponentModel.BackgroundWorker myBackgroundWorker;
        private NumericUpDown numericUpDown1;
    }
}
