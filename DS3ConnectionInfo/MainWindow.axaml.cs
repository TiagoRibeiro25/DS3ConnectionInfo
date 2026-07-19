using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Steamworks;

namespace DS3ConnectionInfo
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameStartTimer, updateTimer;
        private ObservableCollection<string> playerNames = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            listPlayers.ItemsSource = playerNames;
            Title = "DS3 Connection Info";

            gameStartTimer = new DispatcherTimer();
            gameStartTimer.Interval = TimeSpan.FromSeconds(1);
            gameStartTimer.Tick += GameStartTimer_Tick;
            gameStartTimer.Start();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(1);
            updateTimer.Tick += UpdateTimer_Tick;

            Closed += (s, e) => Settings.Default.Save();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Player.UpdatePlayerList();
                var players = Player.ActivePlayers().ToList();

                Dispatcher.UIThread.Post(() =>
                {
                    playerNames.Clear();
                    textPlayerCount.Text = $"Players in session: {players.Count}";
                    foreach (var p in players)
                        playerNames.Add(p.SteamName);
                });
            }
            catch { }
        }

        private void GameStartTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (DS3Interop.TryAttach() && DS3Interop.FindWindow())
                {
                    DS3Interop.Process.EnableRaisingEvents = true;
                    DS3Interop.Process.Exited += (s, ev) =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            updateTimer.Stop();
                            SteamAPI.Shutdown();
                            Close();
                        });
                    };

                    Dispatcher.UIThread.Post(() =>
                    {
                        textGameState.Text = "DS3: RUNNING";
                        textGameState.Foreground = Brushes.LawnGreen;
                    });

                    File.WriteAllText("steam_appid.txt", "374320");
                    if (!SteamAPI.Init()) return;

                    updateTimer.Start();
                    gameStartTimer.Stop();
                }
            }
            catch { }
        }
    }
}
