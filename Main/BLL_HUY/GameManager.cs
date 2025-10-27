using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main.BLL_HUY;

namespace Main.BLL_HUY
{
    public static class GameManager
    {
        public static Player CurrentPlayer { get; private set; }
        public static List<Enemy> ActiveEnemies { get; private set; }
        public static List<IInteractable> ActiveObjects { get; private set; }

        private const float DeltaTime = 0.033f;

        public static void InitializeGame()
        {
            CurrentPlayer = new Player(100, 10, new Vector2(50, 50));

            ActiveEnemies = new List<Enemy>
            {
                new Enemy(50, 5, new Vector2(300, 300)),
                new Enemy(70, 8, new Vector2(150, 400))
            };

            ActiveObjects = new List<IInteractable>
            {
                new Door("KEY_MASTER", new Vector2(500, 200))
            };

            Console.WriteLine("Game Logic đã được khởi tạo.");
        }

        public static void Update()
        {
            foreach (var enemy in ActiveEnemies.ToList())
            {
                if (enemy.IsAlive)
                {
                    enemy.UpdateAI(CurrentPlayer);
                }
            }

            ActiveEnemies.RemoveAll(e => !e.IsAlive);

        }

        public static void HandleMovement(Vector2 direction)
        {
            if (CurrentPlayer != null && CurrentPlayer.IsAlive)
            {
                CurrentPlayer.Move(direction);
            }
        }

        public static void HandleInteraction()
        {
            if (CurrentPlayer == null) return;

            foreach (var obj in ActiveObjects)
            {
                if (Vector2.Distance(CurrentPlayer.Position, obj.Position) < 50)
                {
                    obj.Interact(CurrentPlayer);
                    return;
                }
            }
        }
    }
}
