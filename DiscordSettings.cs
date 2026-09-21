using Process_Auto_Relaunch;
using RelaunchProcess.Properties;
using System;
using System.Windows.Forms;

namespace RelaunchProcess
{
    public partial class WebhookSettings : Form
    {
        private MainWindow parent;

        private const int dwhBotNameMaxLength = 32;

        // ссылка должна быть валидной, если она не пустая и имеет правильный формат URL
        private bool LinkIsValid(string url)
        {
            return Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                   !string.IsNullOrWhiteSpace(uri.Host);
        }

        // ссылка должна быть валидной, если она пустая или имеет правильный формат URL
        private bool LinkIsValidOrNull(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return true;
            }

            return Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                   !string.IsNullOrWhiteSpace(uri.Host);
        }

        public WebhookSettings(MainWindow _parent)
        {
            InitializeComponent();
            parent = _parent;
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            if ( (string.IsNullOrWhiteSpace(Settings.Default.dwhBotname) || Settings.Default.dwhBotname.Length > dwhBotNameMaxLength))
            {
                // значение по умолчанию
                Settings.Default.dwhBotname = "Auto Relauncher";
                Settings.Default.Save();
            }
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
            // запрещаем изменять настройки, если Discord включен
            // или разрешаем, если выключен
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
            if ( LinkIsValidOrNull(textDwhURL.Text) && LinkIsValidOrNull(textDwhAvatarUrl.Text) &&
                (!(string.IsNullOrWhiteSpace(textDwhBotName.Text) || textDwhBotName.Text.Length > dwhBotNameMaxLength)))
            {
                if (String.IsNullOrWhiteSpace(textDwhURL.Text))
                {
                    chbxDiscordEnabled.Checked = false;
                }
                if (!chbxDiscordEnabled.Checked)
                {
                    parent.dwhHook.CancelProcessing();
                }
                SaveSettings();
                DialogResult = DialogResult.OK;
                Close();
            }
            else 
            {
                if (!LinkIsValidOrNull(textDwhURL.Text))
                {
                    MessageBox.Show("Неверный формат URL вебхука.\rОчистите или исправьте.", "URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (!LinkIsValidOrNull(textDwhAvatarUrl.Text))
                {
                    MessageBox.Show("Неверный формат URL аватара бота.\rОчистите или исправьте.", "URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (string.IsNullOrWhiteSpace(textDwhBotName.Text) || textDwhBotName.Text.Length > dwhBotNameMaxLength)
                {
                    MessageBox.Show($"Имя бота не может быть пустым или длиннее {dwhBotNameMaxLength} символов.", "Запуск невозможен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearUrl(object sender, EventArgs e)
        {
            if ( (Button)sender == btnClearUrlField ) textDwhURL.Text = "";
            if ( (Button)sender == btnClearAvatarUrlField ) textDwhAvatarUrl.Text = "";
        }

        private void chbxDiscordEnabled_Click(object sender, EventArgs e)
        {
            if (chbxDiscordEnabled.Checked)
            {
                if ( !LinkIsValid(textDwhURL.Text) )
                {
                    chbxDiscordEnabled.Checked = false;
                    MessageBox.Show("Неверный формат URL вебхука.\rИ он не может быть пустым для запуска.\rИсправьте его.", "Запуск невозможен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if ( !LinkIsValidOrNull(textDwhAvatarUrl.Text) )
                {
                    chbxDiscordEnabled.Checked = false;
                    MessageBox.Show("Неверный формат URL аватара бота.\rИсправьте или оставьте пустым.", "Запуск невозможен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if ( string.IsNullOrWhiteSpace(textDwhBotName.Text) || textDwhBotName.Text.Length > dwhBotNameMaxLength )
                {
                    chbxDiscordEnabled.Checked = false;
                    MessageBox.Show($"Имя бота не может быть пустым или длиннее {dwhBotNameMaxLength} символов.", "Запуск невозможен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            UpdateUI();
        }
    }
}
