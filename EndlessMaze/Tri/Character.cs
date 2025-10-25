using System.Drawing;

namespace EndlessMaze.Tri
{
    // Lớp cơ sở cho Người chơi và Quái vật
    public abstract class Character
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 30; // Kích thước mặc định
        public int Height { get; set; } = 30; // Kích thước mặc định
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Speed { get; set; }

        /// <summary>
        /// Trả về hitbox hình chữ nhật của nhân vật.
        /// </summary>
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        /// <summary>
        /// Phương thức trừu tượng để vẽ nhân vật.
        /// </summary>
        public abstract void Draw(Graphics g);
    }

    // Lớp Người chơi
    public class Player : Character
    {
        public int Stamina { get; set; }
        public int MaxStamina { get; set; }

        public Player()
        {
            Width = 30;
            Height = 30;
        }

        public override void Draw(Graphics g)
        {
            // Vẽ Người chơi (màu xanh dương)
            g.FillEllipse(Brushes.DodgerBlue, GetBounds());
            // Vẽ đường viền
            g.DrawEllipse(new Pen(Color.White, 2), GetBounds());

            // Vẽ Thanh máu (Tạm thời)
            float hpRatio = (float)Health / MaxHealth;
            int barHeight = 5;
            Rectangle healthRect = new Rectangle(X, Y - barHeight - 2, Width, barHeight);
            g.FillRectangle(Brushes.Red, healthRect);
            g.FillRectangle(Brushes.LimeGreen, X, Y - barHeight - 2, (int)(Width * hpRatio), barHeight);
        }

        public void DrawStamina(Graphics g)
        {
            // Vẽ Thanh thể lực ngay dưới chân
            float staminaRatio = (float)Stamina / MaxStamina;
            int barHeight = 3;
            int barWidth = Width + 10;
            int barX = X - 5;
            int barY = Y + Height + 5;

            // Nền đen
            g.FillRectangle(Brushes.Black, barX, barY, barWidth, barHeight);
            // Thể lực màu vàng
            g.FillRectangle(Brushes.Yellow, barX, barY, (int)(barWidth * staminaRatio), barHeight);
        }
    }

    // Lớp Quái vật
    public class Monster : Character
    {
        public Monster()
        {
            Width = 30;
            Height = 30;
        }

        public override void Draw(Graphics g)
        {
            // Vẽ Quái vật (màu đỏ)
            g.FillEllipse(Brushes.IndianRed, GetBounds());
            // Vẽ mắt hoặc họa tiết đơn giản
            g.FillEllipse(Brushes.Black, X + 10, Y + 10, 5, 5);
            g.FillEllipse(Brushes.Black, X + 15, Y + 10, 5, 5);

            // Vẽ Thanh máu
            float hpRatio = (float)Health / MaxHealth;
            int barHeight = 4;
            Rectangle healthRect = new Rectangle(X, Y - barHeight - 2, Width, barHeight);
            g.FillRectangle(Brushes.Red, healthRect);
            g.FillRectangle(Brushes.Orange, X, Y - barHeight - 2, (int)(Width * hpRatio), barHeight);
        }
    }
}
