using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Steamworks;

namespace DS3ConnectionInfo
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameStartTimer, updateTimer;
        private ObservableCollection<string> playerEntries = new ObservableCollection<string>();
        private Process gameProcess;
        private bool steamInitialized;

        public MainWindow()
        {
            InitializeComponent();

            listPlayers.ItemsSource = playerEntries;
            Title = "DS3 Connection Info";

            gameStartTimer = new DispatcherTimer();
            gameStartTimer.Interval = TimeSpan.FromSeconds(1);
            gameStartTimer.Tick += GameStartTimer_Tick;
            gameStartTimer.Start();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(1);
            updateTimer.Tick += UpdateTimer_Tick;

            Closed += (s, e) =>
            {
                if (steamInitialized)
                    SteamAPI.Shutdown();

                gameProcess?.Dispose();
            };
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Player.UpdatePlayerList();
                var players = Player.ActivePlayers().ToList();

                Dispatcher.UIThread.Post(() =>
                {
                    playerEntries.Clear();
                    textPlayerCount.Text = $"Players in session: {players.Count}";
                    foreach (var p in players)
                    {
                        string relay = p.IsRelay ? " [relay]" : "";
                        playerEntries.Add($"{p.SteamName} ({p.SteamId64}){relay}");
                    }
                });
            }
            catch { }
        }

        private void GameStartTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (gameProcess == null)
                {
                    gameProcess = FindGameProcess();
                    if (gameProcess == null)
                        return;

                    gameProcess.EnableRaisingEvents = true;
                    gameProcess.Exited += (s, ev) =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            updateTimer.Stop();
                            Close();
                        });
                    };

                    Dispatcher.UIThread.Post(() =>
                    {
                        textGameState.Text = "DS3: RUNNING";
                        textGameState.Foreground = Brushes.LawnGreen;
                    });
                }

                steamInitialized = SteamAPI.Init();
                if (!steamInitialized)
                    return;

                updateTimer.Start();
                gameStartTimer.Stop();
            }
            catch { }
        }

        private static Process FindGameProcess()
        {
            var processes = Process.GetProcessesByName("DarkSoulsIII");
            if (processes.Length == 0)
                return null;

            var game = processes[0];
            foreach (var process in processes.Skip(1))
                process.Dispose();

            return game;
        }
    }
}
