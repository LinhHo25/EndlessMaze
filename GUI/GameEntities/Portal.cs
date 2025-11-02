using Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.GameEntities
{
    public class Portal
    {
        public float X, Y; // Vị trí logic 1:1 (tâm)
        public int Width = 50;
        public int Height = 50;
        public RectangleF Hitbox => new RectangleF(X - Width / 2, Y - Height / 2, Width, Height);

        private AnimationActivity anim;
        private bool isReady = false; // Cờ báo hiệu đã sẵn sàng để qua màn

        public Portal(float x, float y)
        {
            X = x; // Vị trí tâm
            Y = y;

            // TODO: Tải hoạt ảnh cho Portal (hiện tại dùng ảnh giả)
            // anim = new AnimationActivity(5);
            // anim.LoadImages(null, "path/to/portal/images", null, null);
        }

        public void Update()
        {
            // (Nếu có hoạt ảnh, cập nhật ở đây)
            // ví dụ: if (anim.IsFinished) isReady = true;

            // Giả sử sau 1 giây là sẵn sàng
            isReady = true;
        }

        public bool CheckCollision(RectangleF playerHitbox)
        {
            return isReady && Hitbox.IntersectsWith(playerHitbox);
        }

        public void Draw(Graphics canvas, int scale)
        {
            if (!isReady) return; // Chưa vẽ nếu chưa sẵn sàng

            // Vẽ một hình elip màu tím làm cổng
            float drawX = (X - Width / 2) * scale;
            float drawY = (Y - Height / 2) * scale;
            canvas.FillEllipse(Brushes.Purple, drawX, drawY, Width * scale, Height * scale);
        }
    }
}
