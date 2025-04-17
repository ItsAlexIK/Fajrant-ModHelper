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
        public string[] Aliases => new string[0];
        public string Description => "Kończy rozmowę i przywraca graczy do wcześniejszych ról.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not CommandSender || Player.Get(sender) is not Player moderator)
            {
                response = "Komenda dostępna tylko dla moderatorów!";
                return false;
            }

            Player targetPlayer = Plugin.Instance.MonitoredPlayers.FirstOrDefault(p => p.Role.Type == RoleTypeId.Tutorial);
            if (targetPlayer == null)
            {
                response = "Nie znaleziono gracza do przywrócenia!";
                return false;
            }

            RestorePlayerState(targetPlayer);
            RestorePlayerState(moderator);
            Plugin.Instance.MonitoredPlayers.Remove(targetPlayer);

            response = "Rozmowa zakończona. Gracze zostali przywróceni do pierwotnych ról.";
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
