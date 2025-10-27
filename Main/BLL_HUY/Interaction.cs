using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BLL_HUY
{
    public interface IInteractable
    {
        Vector2 Position { get; }
        void Interact(Player player);
    }

    public class Door : IInteractable
    {
        public Vector2 Position { get; set; }
        public bool IsLocked { get; private set; } = true;
        public string RequiredKeyID { get; }

        public Door(string keyId, Vector2 position)
        {
            RequiredKeyID = keyId;
            Position = position;
        }

        public void Interact(Player player)
        {
            if (IsLocked)
            {
                if (player.CheckForItem(RequiredKeyID))
                {
                    IsLocked = false;
                    Console.WriteLine("Cửa mở khóa!");
                }
                else
                {
                    Console.WriteLine("Cửa bị khóa. Cần chìa khóa.");
                }
            }
        }
    }
}
