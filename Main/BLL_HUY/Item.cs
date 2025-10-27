using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BLL_HUY
{
    public enum ItemRarity { Common, Rare, Epic }
    public enum ItemSlot { None, Weapon, Armor, Key, Consumable }
    public enum LootTableID { StandardEnemy, EliteEnemy, Chest }

    public interface IItem
    {
        string ItemId { get; }
        string Name { get; }
        ItemSlot Slot { get; }
        void Use(Player player);
    }

    public class EquipmentItem : IItem
    {
        public string ItemId { get; private set; }
        public string Name { get; private set; }
        public ItemSlot Slot { get; private set; }
        public Dictionary<string, float> StatBonus { get; private set; }

        public EquipmentItem(string id, string name, ItemSlot slot, Dictionary<string, float> bonus)
        {
            ItemId = id; Name = name; Slot = slot; StatBonus = bonus;
        }
        public void Use(Player player) { player.Inventory.Equip(this); }
    }

    public class KeyItem : IItem
    {
        public string ItemId { get; private set; }
        public string Name { get; private set; }
        public ItemSlot Slot { get; } = ItemSlot.Key;

        public KeyItem(string id, string name) { ItemId = id; Name = name; }
        public void Use(Player player) { }
    }
}
