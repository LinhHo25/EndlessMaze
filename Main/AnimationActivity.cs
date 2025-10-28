using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TESTT
{
    // Class này quản lý MỘT bộ hoạt ảnh 4 hướng
    public class AnimationActivity
    {
        // 1. Lưu trữ ảnh
        private List<Image> BackImages = new List<Image>();
        private List<Image> FrontImages = new List<Image>();
        private List<Image> LeftImages = new List<Image>();
        private List<Image> RightImages = new List<Image>();

        // 2. Trạng thái hoạt ảnh
        private int steps = 0;
        private int slowDownFrameRate = 0;
        private int animationSpeed;

        // Hoạt ảnh này có lặp lại không? (Mặc định là có)
        public bool IsLooping { get; set; } = true;
        // Hoạt ảnh (không lặp) đã chạy xong chưa?
        public bool IsFinished { get; private set; } = false;


        public AnimationActivity(int speed = 10)
        {
            animationSpeed = speed;
        }

        /// <summary>
        /// Tải ảnh từ 4 thư mục hướng riêng biệt
        /// </summary>
        public void LoadImages(string backDirectory, string frontDirectory, string leftDirectory, string rightDirectory)
        {
            try
            {
                // Tải ảnh Back
                if (Directory.Exists(backDirectory))
                {
                    BackImages.AddRange(Directory.GetFiles(backDirectory, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }

                // Tải ảnh Front
                if (Directory.Exists(frontDirectory))
                {
                    FrontImages.AddRange(Directory.GetFiles(frontDirectory, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }

                // Tải ảnh Left
                if (Directory.Exists(leftDirectory))
                {
                    LeftImages.AddRange(Directory.GetFiles(leftDirectory, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }

                // Tải ảnh Right
                if (Directory.Exists(rightDirectory))
                {
                    RightImages.AddRange(Directory.GetFiles(rightDirectory, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải ảnh: {ex.Message}");
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
                IsFinished = true; // Nếu không có ảnh, coi như "xong"
                return null; // Không có ảnh để hiển thị
            }

            // 3. Logic chạy frame (Đã cập nhật cho IsLooping)
            if (!IsFinished) // Chỉ tăng frame nếu hoạt ảnh chưa kết thúc
            {
                slowDownFrameRate++;
                if (slowDownFrameRate > animationSpeed)
                {
                    steps++;
                    if (steps >= animationFrames.Count)
                    {
                        if (IsLooping)
                        {
                            steps = 0; // Lặp lại từ đầu
                        }
                        else
                        {
                            steps = animationFrames.Count - 1; // Dừng ở frame cuối
                            IsFinished = true; // Báo là đã xong
                        }
                    }
                    slowDownFrameRate = 0;
                }
            }

            // 4. Đảm bảo 'steps' không bị lỗi
            if (steps >= animationFrames.Count)
            {
                steps = animationFrames.Count - 1;
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
            IsFinished = false; // Reset lại cờ 'IsFinished'
        }
    }
}

