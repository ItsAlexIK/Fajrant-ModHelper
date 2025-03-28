using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace FajrantModHelper
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "FajrantModHelper";
        public override string Author => "ItsAlex";
        public override Version Version => new(1, 0, 6);
        public static Plugin Instance { get; private set; }
        private TeamKillHandler teamKillHandler;
        public HashSet<string> BannedPlayers { get; } = new();
        public HashSet<Player> MonitoredPlayers { get; } = new();
        public Dictionary<string, PlayerBackupData> PlayerBackup { get; } = new();

        public override void OnEnabled()
        {
            Instance = this;
            Log.Info("ModHelper enabled!");
            teamKillHandler = new TeamKillHandler();
            teamKillHandler.Register();
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Player.Banned += OnPlayerBanned;
        }

        public override void OnDisabled()
        {
            Instance = null;
            Log.Info("ModHelper disabled!");
            teamKillHandler.Unregister();
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Player.Banned -= OnPlayerBanned;
            MonitoredPlayers.Clear();
            BannedPlayers.Clear();
            PlayerBackup.Clear();
        }

        private void OnPlayerBanned(BannedEventArgs ev)
        {
            if (ev.Target != null)
                BannedPlayers.Add(ev.Target.UserId);
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            if (!MonitoredPlayers.Remove(ev.Player) || ev.Player.Role.Type != RoleTypeId.Tutorial || BannedPlayers.Contains(ev.Player.UserId))
                return;

            BannedPlayers.Add(ev.Player.UserId);
            BanManager.OfflineBanPlayer(BanHandler.BanType.UserId, ev.Player.UserId, "Ucieczka - Odwołać możesz się na Discordzie", TimeSpan.FromDays(30), ev.Player.Nickname);
            BanManager.OfflineBanPlayer(BanHandler.BanType.IP, ev.Player.IPAddress, "Ucieczka - Odwołać możesz się na Discordzie", TimeSpan.FromDays(30), ev.Player.Nickname);
            PlayerBackup.Remove(ev.Player.UserId);
            Map.Broadcast(10, $"{ev.Player.Nickname} został zbanowany!");
        }
    }
}