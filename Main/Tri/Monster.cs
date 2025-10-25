using System.Drawing;
using System.Collections.Generic;
using System;

namespace Main.Tri
{
    // Định nghĩa các loại quái (Giữ nguyên)
    public enum MonsterType
    {
        Water,
        Flame,
        Poison
    }

    // Lớp cơ sở cho Quái
    public class Monster : GameObject
    {
        public MonsterType MonsterClass { get; private set; }
        public float Health { get; set; }
        public float AttackDamage { get; protected set; }
        public float AttackSpeed { get; protected set; } // Tốc độ đánh
        public float MoveSpeed { get; protected set; }

        private PointF spawnPoint; // Vị trí gốc
        private float patrolRadius = 150f; // Tầm di chuyển (tăng lên)
        private float detectionRadius = 250f; // Tầm phát hiện người chơi
        private PointF targetPosition; // Vị trí đang di chuyển tới
        private Random rand = new Random();

        public Monster(MonsterType type, PointF startPosition)
            : base(startPosition.X, startPosition.Y, 28, 28, GameObjectType.Monster)
        {
            this.MonsterClass = type;
            this.spawnPoint = startPosition;
            this.targetPosition = startPosition;

            // Gán thuộc tính dựa trên loại quái
            switch (type)
            {
                case MonsterType.Water:
                    Health = 80;
                    AttackDamage = 10;
                    AttackSpeed = 1.5f;
                    MoveSpeed = 2.0f; // Quái di chuyển chậm hơn Player
                    break;
                case MonsterType.Flame:
                    Health = 120;
                    AttackDamage = 20;
                    AttackSpeed = 1.0f;
                    MoveSpeed = 1.5f;
                    break;
                case MonsterType.Poison:
                    Health = 100;
                    AttackDamage = 5;
                    AttackSpeed = 1.2f;
                    MoveSpeed = 1.8f;
                    break;
            }
        }

        /// <summary>
        /// Hàm Update mới: Chỉ cập nhật logic, KHÔNG di chuyển
        /// </summary>
        public override void Update()
        {
            // (Hiện tại hàm này trống, bạn có thể thêm logic animation cho quái ở đây sau)
        }

        /// <summary>
        /// Hàm tính toán AI và trả về vector di chuyển mong muốn
        /// </summary>
        public PointF CalculateMovementVector(PointF playerPosition)
        {
            float distanceToPlayer = GetDistance(this.Position, playerPosition);

            // 1. Nếu thấy người chơi -> đuổi theo
            if (distanceToPlayer < detectionRadius)
            {
                targetPosition = playerPosition;
            }
            // 2. Nếu không thấy người chơi, đi tuần tra
            else
            {
                float distanceToTarget = GetDistance(this.Position, targetPosition);
                // Nếu đã đến gần mục tiêu tuần tra, chọn mục tiêu mới
                if (distanceToTarget < MoveSpeed * 2)
                {
                    GetNewPatrolPoint();
                }
            }

            // Tính toán vector di chuyển về phía mục tiêu
            float angle = (float)Math.Atan2(targetPosition.Y - Position.Y, targetPosition.X - Position.X);
            float vx = (float)Math.Cos(angle);
            float vy = (float)Math.Sin(angle);

            // Trả về vector đã chuẩn hóa (chưa nhân tốc độ)
            // (Việc nhân tốc độ và va chạm sẽ do file Map xử lý)
            return new PointF(vx * MoveSpeed, vy * MoveSpeed);
        }

        // Lấy 1 điểm ngẫu nhiên trong phạm vi tuần tra
        private void GetNewPatrolPoint()
        {
            float angle = (float)(rand.NextDouble() * 2 * Math.PI);
            float radius = (float)(rand.NextDouble() * patrolRadius);
            float x = spawnPoint.X + (float)Math.Cos(angle) * radius;
            float y = spawnPoint.Y + (float)Math.Sin(angle) * radius;
            targetPosition = new PointF(x, y);
        }

        // Hàm tiện ích tính khoảng cách
        private float GetDistance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public void Attack(Player player)
        {
            if (MonsterClass == MonsterType.Poison)
            {
                player.IsPoisoned = true;
            }
            player.CurrentHealth -= this.AttackDamage;
        }

        public override void Draw(Graphics g)
        {
            Color color = Color.Gray;
            switch (MonsterClass)
            {
                case MonsterType.Water: color = Color.Blue; break;
                case MonsterType.Flame: color = Color.Red; break;
                case MonsterType.Poison: color = Color.Purple; break;
            }
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, BoundingBox);
            }
            // (Bạn có thể thêm thanh máu cho quái ở đây)
        }
    }

    // Lớp Boss (tương tự Monster nhưng mạnh hơn)
    public class Boss : Monster
    {
        public Boss(MonsterType type, PointF startPosition)
            : base(type, startPosition)
        {
            this.Health *= 3;
            this.AttackDamage *= 2;
            this.Width = 48;
            this.Height = 48;
            this.MoveSpeed *= 0.8f; // Boss chậm hơn quái thường
        }
    }
}

