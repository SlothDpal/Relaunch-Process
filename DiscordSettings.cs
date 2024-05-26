using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RelaunchProcess.Properties;

namespace RelaunchProcess
{
    public partial class WebhookSettings : Form
    {
        public WebhookSettings()
        {
            InitializeComponent();
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            textDwhBotName.Text = Settings.Default.dwhBotname;
            textDwhAvatarUrl.Text = Settings.Default.dwhAvatarURL;
            textDwhURL.Text = Settings.Default.dwhURL;
            chbxDiscordEnabled.Checked = Settings.Default.dwhEnabled;
        }

        private void SaveSettings()
        {
            Settings.Default.dwhBotname = textDwhBotName.Text;
            Settings.Default.dwhAvatarURL = textDwhAvatarUrl.Text;
            Settings.Default.dwhURL = textDwhURL.Text;
            Settings.Default.dwhEnabled = chbxDiscordEnabled.Checked;
            Settings.Default.Save();
        }

        public void UpdateUI()
        {
            groupBoxSettingsDiscord.Enabled = !chbxDiscordEnabled.Checked;
        }

        private void WebhookSettings_FormLoad(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if ( (String.IsNullOrEmpty(textDwhURL.Text) ||
                Uri.IsWellFormedUriString(textDwhURL.Text, UriKind.Absolute)) &&
                (String.IsNullOrEmpty(textDwhAvatarUrl.Text) || 
                Uri.IsWellFormedUriString(textDwhAvatarUrl.Text, UriKind.Absolute)) )
            {
                if (String.IsNullOrEmpty(textDwhURL.Text))
                {
                    chbxDiscordEnabled.Checked = false;
                }
                SaveSettings();
                DialogResult = DialogResult.OK;
                Close();
            }
            else 
            {
                MessageBox.Show("Неверный формат URL.\rОчистите или исправьте.", "URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearUrl(object sender, EventArgs e)
        {
            if ( (Button)sender == btnClearUrlField ) textDwhURL.Text = "";
            if ( (Button)sender == btnClearAvatarUrlField ) textDwhAvatarUrl.Text = "";
        }

        private void chbxDiscordEnabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }
}
