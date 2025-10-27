using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BLL_HUY
{
    internal struct LootEntry
    {
        public string ItemId;
        public int Weight;
    }

    public static class ItemFactory
    {
        public static IItem CreateItem(string itemId)
        {
            switch (itemId)
            {
                case "KEY_MASTER":
                    return new KeyItem("KEY_MASTER", "Chìa khóa Master");

                case "ARMOR_RARE":
                    var stats = new Dictionary<string, float> { { "Defense", 15 }, { "Health", 5 } };
                    return new EquipmentItem("ARMOR_RARE", "Giáp Da Hiếm", ItemSlot.Armor, stats);

                default:
                    return null;
            }
        }
    }

    public static class LootingSystem
    {
        private static Random rng = new Random();

        private static readonly Dictionary<LootTableID, List<LootEntry>> LootTables = new Dictionary<LootTableID, List<LootEntry>>()
        {
            {
                LootTableID.StandardEnemy, new List<LootEntry>
                {
                    new LootEntry { ItemId = "KEY_MASTER", Weight = 10 },
                    new LootEntry { ItemId = "ARMOR_RARE", Weight = 5 },
                }
            },
            {
                LootTableID.EliteEnemy, new List<LootEntry>
                {
                    new LootEntry { ItemId = "KEY_MASTER", Weight = 30 },
                    new LootEntry { ItemId = "ARMOR_RARE", Weight = 20 },
                }
            }
        };

        public static IItem GenerateLoot(LootTableID tableId)
        {
            if (!LootTables.ContainsKey(tableId))
            {
                Console.WriteLine($"Lỗi: Không tìm thấy LootTableID {tableId}.");
                return null;
            }

            var entries = LootTables[tableId];
            int totalWeight = entries.Sum(e => e.Weight);
            int randomNumber = rng.Next(1, totalWeight + 1);

            int currentWeight = 0;
            foreach (var entry in entries)
            {
                currentWeight += entry.Weight;
                if (randomNumber <= currentWeight)
                {
                    Console.WriteLine($"-> Rớt ra item: {entry.ItemId}");
                    return ItemFactory.CreateItem(entry.ItemId);
                }
            }
            return null;
        }
    }
}
