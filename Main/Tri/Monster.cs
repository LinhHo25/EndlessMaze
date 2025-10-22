using System.Drawing;
using System.Collections.Generic;
using System;

namespace Main.Tri
{
    // Định nghĩa các loại quái
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
        // SỬA LỖI CS0273: Thay 'private set' thành 'protected set'
        public float AttackDamage { get; protected set; }
        public float AttackSpeed { get; protected set; } // Tốc độ đánh
        public float MoveSpeed { get; protected set; }

        private PointF spawnPoint; // Vị trí gốc
        private float patrolRadius = 50f; // Phạm vi di chuyển
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
                    AttackSpeed = 1.5f; // Đánh nhanh hơn
                    MoveSpeed = 3.0f;
                    break;
                case MonsterType.Flame:
                    Health = 120;
                    AttackDamage = 20; // Dame to hơn
                    AttackSpeed = 1.0f;
                    MoveSpeed = 2.0f;
                    break;
                case MonsterType.Poison:
                    Health = 100;
                    AttackDamage = 5; // Gây độc (logic trong hàm Attack)
                    AttackSpeed = 1.2f;
                    MoveSpeed = 2.5f;
                    break;
            }
        }

        public override void Update()
        {
            // CẬP NHẬT LOGIC DI CHUYỂN
            // Đơn giản: Di chuyển ngẫu nhiên quanh điểm spawn

            float distanceToTarget = (float)Math.Sqrt(Math.Pow(Position.X - targetPosition.X, 2) + Math.Pow(Position.Y - targetPosition.Y, 2));

            // Nếu đã đến gần mục tiêu, chọn mục tiêu mới
            if (distanceToTarget < MoveSpeed)
            {
                GetNewPatrolPoint();
            }

            // Di chuyển về phía mục tiêu
            float angle = (float)Math.Atan2(targetPosition.Y - Position.Y, targetPosition.X - Position.X);
            float vx = (float)Math.Cos(angle) * MoveSpeed;
            float vy = (float)Math.Sin(angle) * MoveSpeed;

            // TODO: Thêm xử lý va chạm tường cho quái
            Position = new PointF(Position.X + vx, Position.Y + vy);
        }

        // Lấy 1 điểm ngẫu nhiên trong phạm vi
        private void GetNewPatrolPoint()
        {
            float angle = (float)(rand.NextDouble() * 2 * Math.PI);
            float radius = (float)(rand.NextDouble() * patrolRadius);
            float x = spawnPoint.X + (float)Math.Cos(angle) * radius;
            float y = spawnPoint.Y + (float)Math.Sin(angle) * radius;
            targetPosition = new PointF(x, y);
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
        }
    }

    // Lớp Boss (tương tự Monster nhưng mạnh hơn)
    public class Boss : Monster
    {
        public Boss(MonsterType type, PointF startPosition)
            : base(type, startPosition)
        {
            // Tăng chỉ số cho Boss
            this.Health *= 3;
            this.AttackDamage *= 2; // Giờ đã có thể truy cập

            // SỬA LỖI CS0200: Gán Width và Height thay vì BoundingBox
            this.Width = 48;
            this.Height = 48;
        }
    }
}

