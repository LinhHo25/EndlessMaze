using System;
using System.Collections.Generic;
using System.Drawing;

namespace Main.Tri
{
    // Enum định nghĩa các loại vật phẩm
    public enum ItemType
    {
        HealthPotion,       // Thuốc hồi máu
        AttackPotion,       // Thuốc tăng tấn công
        DefensePotion,      // Thuốc tăng phòng ngự
        PoisonResistPotion, // Thuốc kháng độc
        CoolingWater        // Nước giải nhiệt (cho map Flame)
    }

    // Enum định nghĩa các loại hiệu ứng
    public enum BuffType
    {
        Attack,
        Defense,
        PoisonResist,
        Cooling
    }

    // Lớp chứa thông tin về một hiệu ứng (buff) đang kích hoạt
    public class Buff
    {
        public BuffType Type { get; private set; }
        public int DurationFrames { get; set; } // Thời gian hiệu lực (tính bằng số frame)
        public float Potency { get; private set; } // Độ mạnh (ví dụ: 1.5f cho 50% attack)

        public Buff(BuffType type, int durationInSeconds, float potency = 0)
        {
            Type = type;
            DurationFrames = durationInSeconds * 60; // Giả sử game chạy 60 FPS
            Potency = potency;
        }

        // Đếm ngược thời gian
        public bool Tick()
        {
            DurationFrames--;
            return DurationFrames <= 0; // Trả về true nếu hết hạn
        }
    }


    // Lớp định nghĩa vật phẩm có thể nhặt (nằm trên map)
    public class CollectibleItem : GameObject
    {
        public ItemType Item { get; private set; }
        private Brush itemBrush;

        public CollectibleItem(ItemType itemType, PointF position)
            : base(position.X, position.Y, 20, 20, GameObjectType.Item) // Kích thước 20x20
        {
            this.Item = itemType;

            // Chọn màu cho vật phẩm
            switch (itemType)
            {
                case ItemType.HealthPotion: itemBrush = Brushes.Red; break;
                case ItemType.AttackPotion: itemBrush = Brushes.Orange; break;
                case ItemType.DefensePotion: itemBrush = Brushes.Gray; break;
                case ItemType.PoisonResistPotion: itemBrush = Brushes.GreenYellow; break;
                case ItemType.CoolingWater: itemBrush = Brushes.Cyan; break;
                default: itemBrush = Brushes.Magenta; break;
            }
        }

        public override void Draw(Graphics g)
        {
            // Vẽ vật phẩm (hình thoi)
            PointF p1 = new PointF(X + Width / 2, Y);
            PointF p2 = new PointF(X + Width, Y + Height / 2);
            PointF p3 = new PointF(X + Width / 2, Y + Height);
            PointF p4 = new PointF(X, Y + Height / 2);
            g.FillPolygon(itemBrush, new[] { p1, p2, p3, p4 });
            g.DrawPolygon(Pens.Black, new[] { p1, p2, p3, p4 });
        }
    }
}
