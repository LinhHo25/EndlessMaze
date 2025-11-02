using Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.GameEntities
{
    // --- SỬA: Thêm IDisposable ---
    public class SpellEffect : IDisposable
    {
        // Vị trí logic 1:1
        public float X, Y;
        public int Width, Height;
        public AnimationActivity Anim;
        public bool IsFinished => Anim.IsFinished;
        private string direction = "down";

        // --- SỬA: Thêm 2 biến mới ---
        private Image currentImage = null; // Biến lưu trữ ảnh hiện tại
        private bool isDisposed = false; // Cờ theo dõi

        public SpellEffect(AnimationActivity animTemplate, float x, float y)
        {
            // KHẮC PHỤC LỖI: Tạo instance mới của AnimationActivity
            Anim = new AnimationActivity(animTemplate.AnimationSpeed) { IsLooping = false };
            Anim.LoadImages(animTemplate.BackDir, animTemplate.FrontDir, animTemplate.LeftDir, animTemplate.RightDir);

            Width = 150;
            Height = 150;
            X = x - (Width / 2); // X và Y vẫn là tọa độ logic 1:1
            Y = y - (Height / 2) - 30;

            // --- SỬA: Lấy khung hình đầu tiên ngay lập tức ---
            this.currentImage = Anim.GetNextFrame(direction);
        }

        public void Update()
        {
            // --- SỬA: Logic Update chỉ cập nhật ảnh ---
            if (Anim.IsFinished || isDisposed) return; // Không làm gì nếu đã kết thúc hoặc đã bị hủy

            // 1. Giải phóng ảnh CŨ
            this.currentImage?.Dispose();

            // 2. Lấy ảnh MỚI và lưu trữ nó
            // Đây là nơi DUY NHẤT gọi GetNextFrame
            this.currentImage = Anim.GetNextFrame(direction);
        }

        public void Draw(Graphics canvas, int scale)
        {
            // --- SỬA: Hàm Draw CHỈ VẼ ảnh đã lưu ---
            // (Không gọi GetNextFrame ở đây nữa)
            if (this.currentImage != null && !isDisposed)
            {
                // ÁP DỤNG SCALE KHI VẼ
                canvas.DrawImage(this.currentImage, (int)X * scale, (int)Y * scale, Width * scale, Height * scale);
            }
        }

        // --- SỬA: Thêm hàm Dispose để dọn dẹp ảnh cuối cùng ---
        public void Dispose()
        {
            if (!isDisposed)
            {
                this.currentImage?.Dispose(); // Giải phóng ảnh đang giữ
                this.currentImage = null;
                isDisposed = true;
            }
        }
    }
}