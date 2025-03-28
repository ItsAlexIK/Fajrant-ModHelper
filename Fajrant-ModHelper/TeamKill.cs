using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using PlayerRoles;

namespace FajrantModHelper
{
    public class TeamKillHandler
    {
        public void Register()
        {
            Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
        }

        private void OnPlayerDying(DyingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null || ev.Attacker == ev.Player)
                return;

            if (ev.Attacker.Role.Type == RoleTypeId.ClassD && ev.Player.Role.Type == RoleTypeId.ClassD)
                return;

            if (ev.Attacker.Role.Side == ev.Player.Role.Side)
            {
                string message = $"[TEAMKILL] {ev.Attacker.Nickname} ({ev.Attacker.Role.Type}) zabił {ev.Player.Nickname} ({ev.Player.Role.Type})";

                ServerConsole.AddLog(message, ConsoleColor.Red);

                foreach (var admin in Player.List)
                {
                    if (admin.CheckPermission("admin.chat"))
                    {
                        admin.ShowHint($"<color=red>{message}</color>", 15);
                    }
                }
            }
        }
    }
}