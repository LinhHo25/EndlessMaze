using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BLL_HUY
{
    public class Player : Character
    {
        public Inventory Inventory { get; private set; }

        public Player(int health, int attack, Vector2 position)
            : base(health, attack, position)
        {
            Inventory = new Inventory();
        }

        protected override void Die()
        {
            Console.WriteLine("Người chơi đã hy sinh! Game Over.");
        }

        public bool CheckForItem(string itemId)
        {
            return Inventory.ContainsItem(itemId);
        }

        public void Move(Vector2 direction)
        {
            Position = Position + direction * 0.5f;
        }
    }
}
