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
            logAlways = 1,          // писать везде
            logUpdateStatus = 2,    // писать в строке состояния
            logHistory = 4,         // писать в окне истории перезапусков
            logDiscord = 8          // писать в Дискорд
        }
        private delegate void UpdateLogDelegate(string text, NotifyLevel level = NotifyLevel.logUpdateStatus);
        private readonly UpdateLogDelegate updateLogDelegate;
        private DiscordWebhook dwhHook;
        private DiscordMessage dwhMessage;
        private Process WatchedProcess;
        private double cpuLastTime = 0;
        private Stopwatch cpuMeasureTimer;

        public Form1()
        {
            InitializeComponent();
            this.updateLogDelegate = this.UpdateStatus;
            this.updateLogDelegate += this.SendDiscordMessage;
            this.updateLogDelegate += this.HistoryLog;
            myBackgroundWorker.WorkerSupportsCancellation = true;
            dwhHook = new DiscordWebhook();
            cpuMeasureTimer = new Stopwatch();
        }

        /// <summary>
        /// Событие запуска формы
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
        /// Восстановление настроек
        /// </summary>
        private void LoadOldState()
        {
            if (Settings.Default.saveOldState)
            {
                radioButtonEnableWathing.Checked = Settings.Default.enableWatching;
            }
            
        }

        /// <summary>
        /// Метод для события отключения
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
                UpdateStatus("Отменяем...",NotifyLevel.logUpdateStatus);
            }
        }

        /// <summary>
        /// Метод для события включения
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
                MessageBox.Show("Имя процесса не может быть пустым!" +
                    "\nУкажите имя процесса", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (String.IsNullOrEmpty(Settings.Default.startProgramPath))
            {
                error = true;
                MessageBox.Show("Программа для запуска не указана." +
                    "\nУкажите программу для запуска", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Status($"Запускаем наблюдение...", NotifyLevel.logDiscord);
            }
        }

        /// <summary>
        /// Обновление статуса в программе
        /// </summary>
        /// <param name="text">Текст для отображения/отправки </param>
        /// <param name="level">Флаги для назначения отправки</param>
        public void UpdateStatus( string text, NotifyLevel level )
        {
            if (!level.HasFlag(NotifyLevel.logAlways) && !level.HasFlag(NotifyLevel.logUpdateStatus)) return;
            labelStatus.Text = text;
        }

        /// <summary>
        /// Добавление строки в Истории Запусков
        /// </summary>
        /// <param name="text">Текст для отображения/отправки </param>
        /// <param name="level">Флаги для назначения отправки</param>
        private void HistoryLog( string text, NotifyLevel level )
        {
            if (!level.HasFlag(NotifyLevel.logAlways) && !level.HasFlag(NotifyLevel.logHistory)) return;
            richTextBoxHistory.Text += DateTime.Now.ToString() + ": " + text + "\n";
        }

        /// <summary>
        /// Отправка статуса в Discord
        /// </summary>
        /// <param name="text">Текст для отображения/отправки </param>
        /// <param name="level">Флаги для назначения отправки</param>
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
                    Status($"Ошибка отправки в дискорд.",NotifyLevel.logHistory);
                    Debug.WriteLine($"Discord messaging error: {ex.Message}");
                    //Settings.Default.dwhEnabled = false;
                    //Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Обновление статуса в программе
        /// </summary>
        /// <param name="text">Текст для отображения/отправки </param>
        /// <param name="level">Флаги для назначения отправки</param>
        public void Status(string text, NotifyLevel level = NotifyLevel.logUpdateStatus)
        {
            Invoke(updateLogDelegate, text, level);
        }

        private void CheckProgramState()
        {
            bool watching = radioButtonEnableWathing.Checked;
            Debug.WriteLine($"Наблюдение: {watching}");

            groupBoxProcessName.Enabled = !watching;
            groupBoxProgramStart.Enabled = !watching;
            groupBoxActions.Enabled = !watching;
            btnShowDiscordSettings.Enabled = !watching; //отключаем кнопку настроек дискорда

            Settings.Default.enableWatching = watching;

            
        }

        /// <summary>
        /// Выбор файла для запуска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSetProgramStart_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Исполняемые файлы (*.exe)|*.exe";
            openFile.Title = "Укажите программу запуска";

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
        /// Событие перед закрытием формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
            Status("Наблюдение отменено - приложение закрыто.", NotifyLevel.logAlways);
        }

        private bool ProcessByNameIsRuning(string name)
        {
            var sessionid = Process.GetCurrentProcess().SessionId;
            var processes = Process.GetProcessesByName(name);
            foreach (var process in processes)
            {
                Debug.WriteLine($"Found proces: {process.ProcessName}. Session Id: {process.SessionId}. Current Session Id: {sessionid}");
                if (process.SessionId == sessionid)
                {
                    WatchedProcess = process;
                    return true;
                }
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

            WatchedProcess=Process.Start(path, args);
            cpuLastTime = 0;
            cpuMeasureTimer.Start();
            Status("Процесс был запущен.", NotifyLevel.logAlways);
        }

        private void BackgroundWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int i = (int)numericUpDown1.Value;

            while (!worker.CancellationPending)
            {
                if (ProcessByNameIsRuning(textBoxProcessName.Text))
                {
                    double cpuTotalTime, cpuPercent;
                    string ProcessAnswer;

                    cpuMeasureTimer.Stop();
                    try
                    {
                        cpuTotalTime = WatchedProcess.TotalProcessorTime.TotalMilliseconds - cpuLastTime;
                        cpuLastTime = WatchedProcess.TotalProcessorTime.TotalMilliseconds;
                        cpuPercent = cpuTotalTime * 100 / (Environment.ProcessorCount * cpuMeasureTimer.ElapsedMilliseconds);
                        ProcessAnswer = (WatchedProcess.Responding) ? "Активен" : "Неактивен";
                    }
                    catch
                    {
                        cpuTotalTime = 0;
                        cpuPercent = 0;
                        ProcessAnswer = "Неактивен";
                    }
                    cpuMeasureTimer.Reset();
                    cpuMeasureTimer.Start();

                    Status($"Процесс уже запущен.",NotifyLevel.logUpdateStatus);
                    processInformationLabel.Text = $"Интерфейс: {ProcessAnswer}. ЦПУ: {cpuPercent:f2}% {cpuTotalTime:f2}мсек";
                    if (i < (int)numericUpDown1.Value) SendDiscordMessage($"Процесс {textBoxProcessName.Text} запущен.",NotifyLevel.logDiscord);
                    i = (int)numericUpDown1.Value;
                }
                else
                {
                    processInformationLabel.Text = "";
                    if (radioButtonRestartTimer.Checked)
                    {
                        if (i==(int)numericUpDown1.Value) Status($"Процесс {textBoxProcessName.Text} не найден. Запуск через {i} сек",NotifyLevel.logDiscord);
                        i--;
                        Status($"Процесс {textBoxProcessName.Text} не найден. Запуск через {i}", NotifyLevel.logUpdateStatus);
                    }

                    if (i <= 0 || radioButtonRestartNow.Checked)
                    {
                        i = (int)numericUpDown1.Value;
                        Status($"Запускаем {textBoxProcessName.Text}", NotifyLevel.logUpdateStatus|NotifyLevel.logDiscord);
                        ProcessStart(Settings.Default.startProgramPath, textBoxArguments.Text);
                    }
                }

                Thread.Sleep(1000);
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                processInformationLabel.Text = "";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Status("Наблюдение отменено.",NotifyLevel.logUpdateStatus|NotifyLevel.logDiscord);
            }
            else if (e.Error != null)
            {
                string error_message = "Error: " + e.Error.Message + "\n" + e.Error.StackTrace;
                Status("Произошла ошибка! Наблюдение остановлено.\n" + error_message, NotifyLevel.logUpdateStatus | NotifyLevel.logDiscord);
                MessageBox.Show(error_message, "Ошибка наблюдения", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                radioButtonDisableWathing.Checked = true;
            }
            else
            {
                Status("Наблюдение остановлено.", NotifyLevel.logUpdateStatus|NotifyLevel.logDiscord);
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
