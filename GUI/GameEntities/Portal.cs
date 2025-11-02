// Thêm các thư viện cần thiết ở đầu tệp
using Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO; // <-- THÊM
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

        // --- SỬA: Thay thế 'anim' giả ---
        private AnimationActivity anim; // <-- SỬA
        private bool isReady = false;

        public Portal(float x, float y)
        {
            X = x; // Vị trí tâm
            Y = y;

            // --- SỬA: Tải hoạt ảnh thật cho Portal ---
            // Tải hoạt ảnh từ đường dẫn bạn yêu cầu (GUI/ImgSource/Portal/Active)
            anim = new AnimationActivity(5);
            string portalPath = Path.Combine("ImgSource", "Portal", "Active");
            anim.LoadImages(null, portalPath, null, null);

            isReady = true; // Đặt là sẵn sàng ngay
            // --- KẾT THÚC SỬA ---
        }

        public void Update()
        {
            // (Hiện không cần làm gì ở Update, GetNextFrame tự xử lý)
            isReady = true;
        }

        public bool CheckCollision(RectangleF playerHitbox)
        {
            return isReady && Hitbox.IntersectsWith(playerHitbox);
        }

        public void Draw(Graphics canvas, int scale)
        {
            if (!isReady) return;

            // --- SỬA: Vẽ hoạt ảnh thay vì hình elip ---
            Image frame = anim.GetNextFrame("down");
            if (frame != null)
            {
                // Vẽ cổng lớn hơn hitbox (ví dụ: 80x80 logic, scale sau)
                int drawWidth = 80;
                int drawHeight = 80;

                // Tính toán vị trí vẽ đã scale, căn giữa theo tâm X, Y
                float drawX = (X * scale) - (drawWidth * scale / 2f);
                float drawY = (Y * scale) - (drawHeight * scale / 2f);

                using (frame)
                {
                    canvas.DrawImage(frame, drawX, drawY, drawWidth * scale, drawHeight * scale);
                }
            }
            // --- KẾT THÚC SỬA ---
        }
    }
}