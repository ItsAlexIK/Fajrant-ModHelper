using Exiled.API.Features;
using InventorySystem.Items;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerStatsSystem;
using Exiled.API.Extensions;
using Exiled.API.Enums;
using PlayerStatsSystem; // Ensure this is included for effect handling

namespace FajrantModHelper
{
    public class PlayerBackupData
    {
        public RoleTypeId Role { get; }
        public List<ItemType> Inventory { get; }
        public Dictionary<Exiled.API.Enums.AmmoType, ushort> Ammo { get; }
        public Vector3 Position { get; }
        public float Health { get; }
        public float ArtificialHealth { get; }
        public float HumeShield { get; }
        public Dictionary<Exiled.API.Enums.EffectType, byte> Effects { get; }

        public PlayerBackupData(Player player)
        {
            Role = player.Role.Type;
            Inventory = player.Items.Select(item => item.Type).ToList();
            Ammo = player.Ammo.ToDictionary(kvp => kvp.Key.GetAmmoType(), kvp => kvp.Value);
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
            foreach (var item in Inventory)
            {
                player.AddItem(item);
            }
            player.Position = Position;
            player.Ammo.Clear();
            foreach (var ammo in Ammo)
            {
                if (ammo.Key.GetItemType() != ItemType.None)
                {
                    player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                }
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
