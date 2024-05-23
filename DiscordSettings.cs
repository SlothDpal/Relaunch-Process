using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RelaunchProcess.Properties;

namespace RelaunchProcess
{
    public partial class DiscordSettings : Form
    {
        public DiscordSettings()
        {
            InitializeComponent();
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
                Settings.Default.Save();
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
    }
}
