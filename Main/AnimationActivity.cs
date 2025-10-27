using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Main
{
    internal class AnimationActivity
    {
        // 1. Lưu trữ ảnh
        private List<Image> BackImages = new List<Image>();
        private List<Image> FrontImages = new List<Image>();
        private List<Image> LeftImages = new List<Image>();
        private List<Image> RightImages = new List<Image>();

        // 2. Trạng thái hoạt ảnh (riêng cho class này)
        private int steps = 0;
        private int slowDownFrameRate = 0;
        private int animationSpeed; // Tốc độ (ví dụ: 4, 10)

        /// <summary>
        /// Khởi tạo một hoạt động hoạt ảnh mới.
        /// </summary>
        /// <param name="speed">Tốc độ. Số càng lớn, hoạt ảnh càng chậm.</param>
        public AnimationActivity(int speed = 10)
        {
            animationSpeed = speed;
        }

        /// <summary>
        /// Tải các file ảnh từ một thư mục dựa trên các từ khóa.
        /// </summary>
        public void LoadImages(string directory, string backKey, string frontKey, string leftKey, string rightKey)
        {
            try
            {
                var files = Directory.GetFiles(directory, "*.png");
                BackImages.AddRange(files.Where(f => f.Contains(backKey)).OrderBy(f => f).Select(f => Image.FromFile(f)));
                FrontImages.AddRange(files.Where(f => f.Contains(frontKey)).OrderBy(f => f).Select(f => Image.FromFile(f)));
                LeftImages.AddRange(files.Where(f => f.Contains(leftKey)).OrderBy(f => f).Select(f => Image.FromFile(f)));
                RightImages.AddRange(files.Where(f => f.Contains(rightKey)).OrderBy(f => f).Select(f => Image.FromFile(f)));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải ảnh từ thư mục '{directory}': {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy khung hình (frame) tiếp theo dựa trên hướng.
        /// </summary>
        public Image GetNextFrame(string direction)
        {
            // 1. Chọn đúng danh sách ảnh dựa trên hướng
            List<Image> animationFrames;
            switch (direction)
            {
                case "up": animationFrames = BackImages; break;
                case "down": animationFrames = FrontImages; break;
                case "left": animationFrames = LeftImages; break;
                case "right": animationFrames = RightImages; break;
                default: animationFrames = FrontImages; break; // Mặc định
            }

            // 2. Kiểm tra an toàn
            if (animationFrames == null || animationFrames.Count == 0)
            {
                return null; // Không có ảnh để hiển thị
            }

            // 3. Logic chạy frame
            slowDownFrameRate++;
            if (slowDownFrameRate > animationSpeed)
            {
                steps++;
                if (steps >= animationFrames.Count)
                {
                    steps = 0;
                }
                slowDownFrameRate = 0;
            }

            // 4. Đảm bảo 'steps' không bị lỗi (sau khi đổi hướng)
            if (steps >= animationFrames.Count)
            {
                steps = 0;
            }

            // 5. Trả về ảnh hiện tại
            return animationFrames[steps];
        }

        /// <summary>
        /// Lấy khung hình đầu tiên (mặc định) của một hướng.
        /// </summary>
        public Image GetDefaultFrame(string direction = "down")
        {
            try
            {
                switch (direction)
                {
                    case "up": return BackImages.Count > 0 ? BackImages[0] : null;
                    case "down": return FrontImages.Count > 0 ? FrontImages[0] : null;
                    case "left": return LeftImages.Count > 0 ? LeftImages[0] : null;
                    case "right": return RightImages.Count > 0 ? RightImages[0] : null;
                    default: return FrontImages.Count > 0 ? FrontImages[0] : null;
                }
            }
            catch { return null; }
        }

        /// <summary>
        /// Reset bộ đếm frame về 0 (dùng khi chuyển hành động).
        /// </summary>
        public void ResetFrame()
        {
            steps = 0;
            slowDownFrameRate = 0;
        }
    }
}