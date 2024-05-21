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

        private void DiscordSettings_Load(object sender, EventArgs e)
        {
            chbxDiscordEnabled.Checked=Settings.Default.dwhEnabled;
            textDwhURL.Text=Settings.Default.dwhURL;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Uri.IsWellFormedUriString(textDwhURL.Text, UriKind.Absolute))
            {
                Settings.Default.dwhEnabled = chbxDiscordEnabled.Checked;
                Settings.Default.dwhURL = textDwhURL.Text;
                Settings.Default.Save();
                DialogResult = DialogResult.OK;
                Close();
            }
            else 
            {
                //TODO обработать ситуацию с неверным УРЛом
            }


        }
    }
}
