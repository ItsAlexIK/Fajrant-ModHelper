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

            var targetPlayer = Plugin.Instance.MonitoredPlayers.FirstOrDefault(p => p.Role.Type == RoleTypeId.Tutorial);

            bool restoredAnyone = false;

            if (targetPlayer != null && Plugin.Instance.PlayerBackup.ContainsKey(targetPlayer.UserId))
            {
                RestorePlayerState(targetPlayer);
                Plugin.Instance.MonitoredPlayers.Remove(targetPlayer);
                restoredAnyone = true;
            }

            if (Plugin.Instance.PlayerBackup.ContainsKey(moderator.UserId))
            {
                RestorePlayerState(moderator);
                restoredAnyone = true;
            }

            if (!restoredAnyone)
            {
                response = "Nie znaleziono żadnych danych do przywrócenia.";
                return false;
            }

            response = "Zakończono rozmowę. Przywrócono dostępne stany graczy.";
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
