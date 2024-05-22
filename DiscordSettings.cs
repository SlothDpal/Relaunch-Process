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
            if (Uri.IsWellFormedUriString(textDwhURL.Text, UriKind.Absolute)||textDwhURL.Text=="")
            {
                if (textDwhURL.Text=="")
                {
                    chbxDiscordEnabled.Checked = false;
                }
                Settings.Default.Save();
                DialogResult = DialogResult.OK;
                Close();
            }
            else 
            {
                //TODO обработать ситуацию с неверным УРЛом
                textDwhURL.Text = "";
                MessageBox.Show("Неверный формат URL.", "URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClearUrlField_Click(object sender, EventArgs e)
        {
            textDwhURL.Text = "";
        }
    }
}
