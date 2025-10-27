using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.BLL_HUY
{
    public class Enemy : Character
    {
        private const float MovementSpeed = 0.05f;

        public Enemy(int health, int attack, Vector2 position)
            : base(health, attack, position) { }

        protected override void Die()
        {
            Console.WriteLine($"Quái vật bị tiêu diệt.");
            LootingSystem.GenerateLoot(LootTableID.StandardEnemy);
        }

        public void UpdateAI(Player player)
        {
            if (!IsAlive) return;

            float distanceToPlayer = Vector2.Distance(Position, player.Position);

            Vector2 direction = Vector2.Normalize(player.Position - Position);
            Position = Position + direction * MovementSpeed;

            if (distanceToPlayer <= 1.0f)
            {
                Attack(player);
            }
        }
    }
}
