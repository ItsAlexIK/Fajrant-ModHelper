using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FajrantModHelper.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ModCommand : ICommand
    {
        public string Command => "mod";
        public string[] Aliases => new string[] { };
        public string Description => "Zmienia moderatora i gracza na Tutoriala.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSender commandSender || !commandSender.CheckPermission("mod.helper"))
            {
                response = "Nie masz uprawnień do użycia tej komendy!";
                return false;
            }

            if (arguments.Count < 1 || !int.TryParse(arguments.At(0), out int targetId) || !Player.TryGet(targetId, out Player target))
            {
                response = "Użycie: mod <id_gracza>";
                return false;
            }

            Player moderator = Player.List.FirstOrDefault(p => p.UserId == commandSender.SenderId);
            if (moderator == null)
            {
                response = "Nie udało się znaleźć moderatora!";
                return false;
            }

            Plugin.Instance.PlayerBackup[moderator.UserId] = new PlayerBackupData(moderator);
            Plugin.Instance.PlayerBackup[target.UserId] = new PlayerBackupData(target);

            moderator.Role.Set(RoleTypeId.Tutorial);
            target.Role.Set(RoleTypeId.Tutorial);

            Plugin.Instance.MonitoredPlayers.Add(target);
            target.Broadcast(10, "<color=red>Moderator chce z Tobą porozmawiać. Opuszczenie serwera skutkuje automatycznym banem!</color>");

            response = "Gracz został przeniesiony na Tutoriala!";
            return true;
        }
    }
}
