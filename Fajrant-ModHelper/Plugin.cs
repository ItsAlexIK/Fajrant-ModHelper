using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FajrantModHelper
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "FajrantModHelper";
        public override string Author => "ItsAlex";
        public override Version Version => new(1, 0, 6);
        public static Plugin Instance { get; private set; }

        public HashSet<string> BannedPlayers { get; } = new();
        public HashSet<Player> MonitoredPlayers { get; } = new();
        public HashSet<string> RecentlyBannedPlayers { get; } = new();
        public Dictionary<string, PlayerBackupData> PlayerBackup { get; } = new();

        public override void OnEnabled()
        {
            Instance = this;
            Log.Info("ModHelper enabled!");
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Player.Banned += OnPlayerBanned;
        }

        public override void OnDisabled()
        {
            Instance = null;
            Log.Info("ModHelper disabled!");
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Player.Banned -= OnPlayerBanned;
            MonitoredPlayers.Clear();
            BannedPlayers.Clear();
            RecentlyBannedPlayers.Clear();
            PlayerBackup.Clear();
        }

        private void OnPlayerBanned(BannedEventArgs ev)
        {
            if (ev.Target != null)
            {
                RecentlyBannedPlayers.Add(ev.Target.UserId);
            }
            else
            {
                Log.Warn("OnPlayerBanned: ev.Target is null, skipping...");
            }
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            if (RecentlyBannedPlayers.Remove(ev.Player.UserId)) return;

            if (ev.Player.Role.Type == RoleTypeId.Tutorial && MonitoredPlayers.Contains(ev.Player))
            {
                if (BannedPlayers.Add(ev.Player.UserId))
                {
                    BanPlayer(ev.Player);
                    Log.Info($"Gracz {ev.Player.Nickname} ({ev.Player.UserId}) został automatycznie zbanowany na 30 dni za opuszczenie serwera.");
                }
                MonitoredPlayers.Remove(ev.Player);
                PlayerBackup.Remove(ev.Player.UserId);
            }
        }

        private void BanPlayer(Player player)
        {
            BanManager.OfflineBanPlayer(BanHandler.BanType.UserId, player.UserId, "Ucieczka - Odwołać możesz się na Discordzie", TimeSpan.FromDays(30), player.Nickname);
            BanManager.OfflineBanPlayer(BanHandler.BanType.IP, player.IPAddress, "Ucieczka - Odwołać możesz się na Discordzie", TimeSpan.FromDays(30), player.Nickname);
        }
    }
}
