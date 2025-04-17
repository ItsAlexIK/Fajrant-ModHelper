using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Linq;

namespace FajrantModHelper.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ModCommand : ICommand
    {
        public string Command => "mod";
        public string[] Aliases => new string[0];
        public string Description => "Zmienia moderatora i gracza na Tutoriala.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSender cmdSender || !cmdSender.CheckPermission("mod.helper"))
                return Fail("Nie masz uprawnień do użycia tej komendy!", out response);

            if (arguments.Count < 1 || !int.TryParse(arguments.At(0), out int targetId) || !Player.TryGet(targetId, out Player target))
                return Fail("Użycie: mod <id_gracza>", out response);

            if (Player.Get(sender) is not Player moderator)
                return Fail("Nie udało się znaleźć moderatora!", out response);

            var backup = Plugin.Instance.PlayerBackup;
            backup[moderator.UserId] = new PlayerBackupData(moderator);
            backup[target.UserId] = new PlayerBackupData(target);

            moderator.Role.Set(RoleTypeId.Tutorial);
            target.Role.Set(RoleTypeId.Tutorial);
            Plugin.Instance.MonitoredPlayers.Add(target);
            target.Broadcast(10, "<color=red>Moderator chce z Tobą porozmawiać. Opuszczenie serwera skutkuje automatycznym banem!</color>");

            return Success("Gracz został przeniesiony na Tutoriala!", out response);
        }

        private static bool Fail(string message, out string response)
        {
            response = message;
            return false;
        }

        private static bool Success(string message, out string response)
        {
            response = message;
            return true;
        }
    }
}
