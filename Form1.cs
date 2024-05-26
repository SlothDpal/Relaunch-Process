using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using RelaunchProcess.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Discord;
using Discord.Webhook;
using System.Data;
using static System.Net.Mime.MediaTypeNames;
using RelaunchProcess;


namespace Process_Auto_Relaunch
{
    public partial class Form1 : Form
    {
        [Flags]
        public enum NotifyLevel
        {
            logNone = 0,
            logAlways = 1,          // ������ �����
            logUpdateStatus = 2,    // ������ � ������ ���������
            logHistory = 4,         // ������ � ���� ������� ������������
            logDiscord = 8          // ������ � �������
        }
        private delegate void UpdateLogDelegate(string text, NotifyLevel level = NotifyLevel.logUpdateStatus);
        private readonly UpdateLogDelegate updateLogDelegate;
        private DiscordWebhook dwhHook;
        private DiscordMessage dwhMessage;

        public Form1()
        {
            InitializeComponent();
            this.updateLogDelegate = this.UpdateStatus;
            this.updateLogDelegate += this.SendDiscordMessage;
            this.updateLogDelegate += this.HistoryLog;
            myBackgroundWorker.WorkerSupportsCancellation = true;
            dwhHook = new DiscordWebhook();
            /*if ( Uri.IsWellFormedUriString(Settings.Default.dwhURL,UriKind.Absolute) && Settings.Default.dwhEnabled && Settings.Default.dwhURL!="") 
            {
                dwhHook.Url = Settings.Default.dwhURL;
            }
            else if (Settings.Default.dwhEnabled) { 
                Debug.WriteLine($"������ � URL ���-���� ({Settings.Default.dwhURL}). ����� � Discord ��������.");
                HistoryLog($"������ � URL ���-���� ({Settings.Default.dwhURL}). ����� � Discord ��������.");
                Settings.Default.dwhEnabled = false;
                Settings.Default.Save();
            }*/

        }

        /// <summary>
        /// ������� ������� �����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadOldState();

            //MessageBox.Show(Environment.UserDomainName);

            CheckProgramState();
        }

        /// <summary>
        /// �������������� ��������
        /// </summary>
        private void LoadOldState()
        {
            if (Settings.Default.saveOldState)
            {
                radioButtonEnableWathing.Checked = Settings.Default.enableWatching;
            }
            
        }

        /// <summary>
        /// ����� ��� ������� ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonDisableWathing_CheckedChanged(object sender, EventArgs e)
        {
            CheckProgramState();

            if (!radioButtonDisableWathing.Checked)
            {
                return;
            }

            if (myBackgroundWorker.WorkerSupportsCancellation && myBackgroundWorker.IsBusy)
            {
                myBackgroundWorker.CancelAsync();
                UpdateStatus("��������...",NotifyLevel.logUpdateStatus);
            }
        }

        /// <summary>
        /// ����� ��� ������� ���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonEnableWathing_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonEnableWathing.Checked)
            {
                return;
            }
            bool error = false;

            if (String.IsNullOrEmpty(textBoxProcessName.Text))
            {
                error = true;
                MessageBox.Show("��� �������� �� ����� ���� ������!" +
                    "\n������� ��� ��������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (String.IsNullOrEmpty(Settings.Default.startProgramPath))
            {
                error = true;
                MessageBox.Show("��������� ��� ������� �� �������." +
                    "\n������� ��������� ��� �������", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (error)
            {
                radioButtonEnableWathing.Checked = false;
                radioButtonDisableWathing.Checked = true;
                return;
            }

            if (!myBackgroundWorker.IsBusy)
            {
                myBackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// ���������� ������� � ���������
        /// </summary>
        /// <param name="text">����� ��� �����������/�������� </param>
        /// <param name="level">����� ��� ���������� ��������</param>
        public void UpdateStatus( string text, NotifyLevel level )
        {
            if (!level.HasFlag(NotifyLevel.logAlways) && !level.HasFlag(NotifyLevel.logUpdateStatus)) return;
            labelStatus.Text = text;
        }

        /// <summary>
        /// ���������� ������ � ������� ��������
        /// </summary>
        /// <param name="text">����� ��� �����������/�������� </param>
        /// <param name="level">����� ��� ���������� ��������</param>
        private void HistoryLog( string text, NotifyLevel level )
        {
            if (!level.HasFlag(NotifyLevel.logAlways) && !level.HasFlag(NotifyLevel.logHistory)) return;
            richTextBoxHistory.Text += DateTime.Now.ToString() + ": " + text + "\n";
        }

        /// <summary>
        /// �������� ������� � Discord
        /// </summary>
        /// <param name="text">����� ��� �����������/�������� </param>
        /// <param name="level">����� ��� ���������� ��������</param>
        public void SendDiscordMessage( string text, NotifyLevel level )
        {
            if (!level.HasFlag(NotifyLevel.logAlways) && !level.HasFlag(NotifyLevel.logDiscord)) return;
            if (Settings.Default.dwhEnabled)
            {
                dwhHook.Url = Settings.Default.dwhURL;
                dwhMessage.Username = Settings.Default.dwhBotname;
                dwhMessage.AvatarUrl = Settings.Default.dwhAvatarURL;
                dwhMessage.Content = ":arrows_counterclockwise: " + text;
                try
                {
                    dwhHook.Send(dwhMessage);
                }
                catch (Exception ex)
                {
                    Status($"������ �������� � �������.",NotifyLevel.logHistory);
                    Debug.WriteLine($"Discord messaging error: {ex.Message}");
                    //Settings.Default.dwhEnabled = false;
                    //Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// ���������� ������� � ���������
        /// </summary>
        /// <param name="text">����� ��� �����������/�������� </param>
        /// <param name="level">����� ��� ���������� ��������</param>
        public void Status(string text, NotifyLevel level = NotifyLevel.logUpdateStatus)
        {
            Invoke(updateLogDelegate, text, level);
        }

        private void CheckProgramState()
        {
            bool watching = radioButtonEnableWathing.Checked;
            Debug.WriteLine($"����������: {watching}");

            groupBoxProcessName.Enabled = !watching;
            groupBoxProgramStart.Enabled = !watching;
            groupBoxActions.Enabled = !watching;
            btnShowDiscordSettings.Enabled = !watching; //��������� ������ �������� ��������

            Settings.Default.enableWatching = watching;

            
        }

        /// <summary>
        /// ����� ����� ��� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSetProgramStart_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "����������� ����� (*.exe)|*.exe";
            openFile.Title = "������� ��������� �������";

            if (openFile.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            int lastSlash = openFile.FileName.LastIndexOf("\\");
            textBoxProcessName.Text = openFile.FileName.Substring(lastSlash+1);
            textBoxProcessName.Text = textBoxProcessName.Text.Remove(textBoxProcessName.Text.Length-4);
            Settings.Default.startProgramPath = openFile.FileName;
            Settings.Default.Save();
            openFile.Dispose();
        }

        /// <summary>
        /// ������� ����� ��������� �����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }

        private bool ProcessByNameIsRuning(string name)
        {
            var sessionid = Process.GetCurrentProcess().SessionId;
            var processes = Process.GetProcessesByName(name);
            foreach (var process in processes)
            {
                Debug.WriteLine($"Found proces: {process.ProcessName}. Session Id: {process.SessionId}. Current Session Id: {sessionid}");
                if (process.SessionId == sessionid)
                    return true;
            }

            Debug.WriteLine($"Process {name} for current session id {sessionid} not found");
            return false;
        }

        private void ProcessStart(string path, string args)
        {
            if (checkBoxCheckProcess.Checked)
            {
                if (ProcessByNameIsRuning(path))
                {
                    return;
                }
            }

            Status("������� ��� �������.", NotifyLevel.logAlways);
            Process.Start(path, args);
        }

        private void BackgroundWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int i = (int)numericUpDown1.Value;

            while (!worker.CancellationPending)
            {
                if (ProcessByNameIsRuning(textBoxProcessName.Text))
                {
                    Status($"������� ��� �������",NotifyLevel.logUpdateStatus);
                    if (i < (int)numericUpDown1.Value) SendDiscordMessage($"������� {textBoxProcessName.Text} �������.",NotifyLevel.logDiscord);
                    i = (int)numericUpDown1.Value;
                }
                else
                {
                    if (radioButtonRestartTimer.Checked)
                    {
                        if (i==(int)numericUpDown1.Value) Status($"������� {textBoxProcessName.Text} �� ������. ������ ����� {i} ���",NotifyLevel.logDiscord);
                        i--;
                        Status($"������� {textBoxProcessName.Text} �� ������. ������ ����� {i}", NotifyLevel.logUpdateStatus);
                    }

                    if (i <= 0 || radioButtonRestartNow.Checked)
                    {
                        i = (int)numericUpDown1.Value;
                        Status($"��������� {textBoxProcessName.Text}", NotifyLevel.logUpdateStatus|NotifyLevel.logDiscord);
                        ProcessStart(Settings.Default.startProgramPath, textBoxArguments.Text);
                    }
                }

                Thread.Sleep(1000);
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Status("���������� ��������.",NotifyLevel.logUpdateStatus|NotifyLevel.logDiscord);
            }
            else if (e.Error != null)
            {
                Status("��������� ������! ���������� �����������.", NotifyLevel.logUpdateStatus | NotifyLevel.logDiscord);
                MessageBox.Show("Error: " + e.Error.Message, "������", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                radioButtonDisableWathing.Checked = true;
            }
            else
            {
                Status("���������� �����������.", NotifyLevel.logUpdateStatus|NotifyLevel.logDiscord);
            }
        }

        private void btnShowDiscordSettings_Click(object sender, EventArgs e)
        {
            WebhookSettings discordSettings;
            discordSettings = new WebhookSettings();
            discordSettings.ShowDialog(this);
            discordSettings.Dispose();
        }
    }
}
