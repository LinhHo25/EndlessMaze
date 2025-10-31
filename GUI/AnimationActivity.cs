using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Main // <-- Đã đổi namespace thành 'Main'
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

        // --- Biến cho hoạt ảnh chạy 1 lần (one-shot) ---
        public bool IsLooping { get; set; } = true; // Mặc định là lặp lại
        public bool IsFinished { get; private set; } = false; // Báo khi nào chạy xong


        public AnimationActivity(int speed = 10)
        {
            animationSpeed = speed;
        }

        /// <summary>
        /// Tải ảnh từ 4 thư mục con (Back, Front, Left, Right)
        /// </summary>
        public void LoadImages(string backDir, string frontDir, string leftDir, string rightDir)
        {
            try
            {
                // Tải Back
                if (backDir != null && Directory.Exists(backDir))
                {
                    BackImages.AddRange(Directory.GetFiles(backDir, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }
                // Tải Front
                if (frontDir != null && Directory.Exists(frontDir))
                {
                    FrontImages.AddRange(Directory.GetFiles(frontDir, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }
                // Tải Left
                if (leftDir != null && Directory.Exists(leftDir))
                {
                    LeftImages.AddRange(Directory.GetFiles(leftDir, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }
                // Tải Right
                if (rightDir != null && Directory.Exists(rightDir))
                {
                    RightImages.AddRange(Directory.GetFiles(rightDir, "*.png").OrderBy(f => f).Select(f => Image.FromFile(f)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải ảnh: {ex.Message}");
            }
        }

        public Image GetNextFrame(string direction)
        {
            List<Image> animationFrames;
            switch (direction)
            {
                case "up": animationFrames = BackImages; break;
                case "down": animationFrames = FrontImages; break;
                case "left": animationFrames = LeftImages; break;
                case "right": animationFrames = RightImages; break;
                default: animationFrames = FrontImages; break;
            }

            if (animationFrames == null || animationFrames.Count == 0)
            {
                return null;
            }

            // Logic chạy frame
            if (!IsFinished)
            {
                slowDownFrameRate++;
                if (slowDownFrameRate > animationSpeed)
                {
                    steps++;
                    if (steps >= animationFrames.Count)
                    {
                        if (IsLooping)
                        {
                            steps = 0; // Lặp lại
                        }
                        else
                        {
                            steps = animationFrames.Count - 1; // Dừng ở frame cuối
                            IsFinished = true; // Báo đã xong
                        }
                    }
                    slowDownFrameRate = 0;
                }
            }

            if (steps >= animationFrames.Count)
            {
                steps = animationFrames.Count - 1; // An toàn
            }

            return animationFrames[steps];
        }

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

        public void ResetFrame()
        {
            steps = 0;
            slowDownFrameRate = 0;
            IsFinished = false;
        }
    }
}

