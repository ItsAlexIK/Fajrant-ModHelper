using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Extensions;

namespace FajrantModHelper
{
    public class PlayerBackupData
    {
        public RoleTypeId Role { get; }
        public List<ItemType> Inventory { get; }
        public Dictionary<ItemType, ushort> Ammo { get; }
        public Vector3 Position { get; }
        public float Health { get; }
        public float ArtificialHealth { get; }
        public float HumeShield { get; }
        public Dictionary<EffectType, byte> Effects { get; }

        public PlayerBackupData(Player player)
        {
            Role = player.Role.Type;
            Inventory = player.Items.Select(item => item.Type).ToList();
            Ammo = player.Ammo.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Position = player.Position;
            Health = player.Health;
            ArtificialHealth = player.ArtificialHealth;
            HumeShield = player.HumeShield;
            Effects = player.ActiveEffects.ToDictionary(effect => effect.GetEffectType(), effect => effect.Intensity);
        }

        public void Restore(Player player)
        {
            player.Role.Set(Role);
            player.ClearInventory();
            Inventory.ForEach(item => player.AddItem(item));
            player.Position = Position;

            foreach (var ammo in Ammo)
            {
                player.SetAmmo(ammo.Key.GetAmmoType(), ammo.Value);
            }

            player.Health = Health;
            player.ArtificialHealth = ArtificialHealth;
            player.HumeShield = HumeShield;

            foreach (EffectType effect in System.Enum.GetValues(typeof(EffectType)))
            {
                player.DisableEffect(effect);
            }

            foreach (var effect in Effects)
            {
                player.EnableEffect(effect.Key, effect.Value, true);
            }
        }
    }
}