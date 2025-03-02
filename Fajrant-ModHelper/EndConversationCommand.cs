using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using System.Linq;

namespace FajrantModHelper.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EndConversationCommand : ICommand
    {
        public string Command => "koniec";
        public string[] Aliases => new string[] { };
        public string Description => "Kończy rozmowę i przywraca graczy do ich wcześniejszej roli.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSender commandSender)
            {
                response = "Komenda dostępna tylko dla moderatorów!";
                return false;
            }

            Player moderator = Player.Get(sender);
            if (moderator == null)
            {
                response = "Nie udało się znaleźć moderatora!";
                return false;
            }

            Player targetPlayer = Plugin.Instance.MonitoredPlayers.FirstOrDefault(p => p.Role.Type == RoleTypeId.Tutorial);
            Player.TryGet(targetPlayer.Id, out targetPlayer);
            if (targetPlayer == null)
            {
                response = "Nie znaleziono gracza do przywrócenia!";
                return false;
            }

            RestorePlayerState(targetPlayer);
            RestorePlayerState(moderator);

            Plugin.Instance.MonitoredPlayers.Remove(targetPlayer);
            response = "Rozmowa zakończona. Obaj gracze zostali przywróceni do pierwotnych ról, ekwipunku i pozycji.";
            return true;
        }

        private void RestorePlayerState(Player player)
        {
            if (Plugin.Instance.PlayerBackup.TryGetValue(player.UserId, out var backup))
            {
                backup.Restore(player);
                Plugin.Instance.PlayerBackup.Remove(player.UserId);
            }
        }
    }
}
