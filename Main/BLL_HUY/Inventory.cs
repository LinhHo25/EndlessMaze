using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BLL_HUY
{
    public class Inventory
    {
        private List<IItem> items = new List<IItem>();
        private Dictionary<ItemSlot, EquipmentItem> equippedItems = new Dictionary<ItemSlot, EquipmentItem>();

        public void AddItem(IItem item)
        {
            items.Add(item);
            Console.WriteLine($"-> Nhặt: {item.Name}.");
        }

        public bool ContainsItem(string itemId)
        {
            return items.Any(i => i.ItemId == itemId);
        }

        public void Equip(EquipmentItem item)
        {
            equippedItems[item.Slot] = item;
            Console.WriteLine($"-> Trang bị {item.Name} thành công.");
        }
    }
}
