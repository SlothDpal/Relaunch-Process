namespace RelaunchProcess
{
    partial class DiscordSettings
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chbxDiscordEnabled = new System.Windows.Forms.CheckBox();
            this.textDwhURL = new System.Windows.Forms.TextBox();
            this.lblDwhURL = new System.Windows.Forms.Label();
            this.btnClearUrlField = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(193, 89);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Сохранить";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(274, 89);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отменить";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // chbxDiscordEnabled
            // 
            this.chbxDiscordEnabled.AutoSize = true;
            this.chbxDiscordEnabled.Checked = global::RelaunchProcess.Properties.Settings.Default.dwhEnabled;
            this.chbxDiscordEnabled.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::RelaunchProcess.Properties.Settings.Default, "dwhEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chbxDiscordEnabled.Location = new System.Drawing.Point(12, 63);
            this.chbxDiscordEnabled.Name = "chbxDiscordEnabled";
            this.chbxDiscordEnabled.Size = new System.Drawing.Size(232, 17);
            this.chbxDiscordEnabled.TabIndex = 2;
            this.chbxDiscordEnabled.Text = "Включить отправку сообщений в Discord";
            this.chbxDiscordEnabled.UseVisualStyleBackColor = true;
            // 
            // textDwhURL
            // 
            this.textDwhURL.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::RelaunchProcess.Properties.Settings.Default, "dwhURL", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textDwhURL.Location = new System.Drawing.Point(12, 37);
            this.textDwhURL.Name = "textDwhURL";
            this.textDwhURL.Size = new System.Drawing.Size(337, 20);
            this.textDwhURL.TabIndex = 3;
            this.textDwhURL.Text = global::RelaunchProcess.Properties.Settings.Default.dwhURL;
            // 
            // lblDwhURL
            // 
            this.lblDwhURL.AutoSize = true;
            this.lblDwhURL.Location = new System.Drawing.Point(9, 9);
            this.lblDwhURL.Name = "lblDwhURL";
            this.lblDwhURL.Size = new System.Drawing.Size(147, 13);
            this.lblDwhURL.TabIndex = 4;
            this.lblDwhURL.Text = "URL-адрес Discord веб-хука";
            // 
            // btnClearUrlField
            // 
            this.btnClearUrlField.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClearUrlField.Location = new System.Drawing.Point(351, 38);
            this.btnClearUrlField.Name = "btnClearUrlField";
            this.btnClearUrlField.Size = new System.Drawing.Size(19, 19);
            this.btnClearUrlField.TabIndex = 5;
            this.btnClearUrlField.Text = "X";
            this.btnClearUrlField.UseVisualStyleBackColor = true;
            this.btnClearUrlField.Click += new System.EventHandler(this.BtnClearUrlField_Click);
            // 
            // DiscordSettings
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(375, 128);
            this.ControlBox = false;
            this.Controls.Add(this.btnClearUrlField);
            this.Controls.Add(this.lblDwhURL);
            this.Controls.Add(this.textDwhURL);
            this.Controls.Add(this.chbxDiscordEnabled);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiscordSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Настройки Discord webhook";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbxDiscordEnabled;
        private System.Windows.Forms.TextBox textDwhURL;
        private System.Windows.Forms.Label lblDwhURL;
        private System.Windows.Forms.Button btnClearUrlField;
    }
}