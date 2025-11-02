using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Main
{
    public class AnimationActivity
    {
        private List<string> BackImages = new List<string>();
        private List<string> FrontImages = new List<string>();
        private List<string> LeftImages = new List<string>();
        private List<string> RightImages = new List<string>();

        private int steps = 0;
        private int slowDownFrameRate = 0;

        public int AnimationSpeed { get; private set; }
        public string BackDir { get; private set; }
        public string FrontDir { get; private set; }
        public string LeftDir { get; private set; }
        public string RightDir { get; private set; }

        public bool IsLooping { get; set; } = true;
        public bool IsFinished { get; private set; } = false;

        public AnimationActivity(int speed = 10)
        {
            AnimationSpeed = speed;
        }

        public void LoadImages(string backDir, string frontDir, string leftDir, string rightDir)
        {
            BackDir = backDir;
            FrontDir = frontDir;
            LeftDir = leftDir;
            RightDir = rightDir;
            try
            {
                if (backDir != null && Directory.Exists(backDir))
                {
                    BackImages.AddRange(Directory.GetFiles(backDir, "*.png").OrderBy(f => f));
                }
                if (frontDir != null && Directory.Exists(frontDir))
                {
                    FrontImages.AddRange(Directory.GetFiles(frontDir, "*.png").OrderBy(f => f));
                }
                if (leftDir != null && Directory.Exists(leftDir))
                {
                    LeftImages.AddRange(Directory.GetFiles(leftDir, "*.png").OrderBy(f => f));
                }
                if (rightDir != null && Directory.Exists(rightDir))
                {
                    RightImages.AddRange(Directory.GetFiles(rightDir, "*.png").OrderBy(f => f));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải đường dẫn ảnh: {ex.Message}");
            }
        }

        // --- SỬA LỖI: Hàm mới để tải ảnh an toàn (tránh "Out of Memory") ---
        private Image LoadImageFromFile(string path)
        {
            try
            {
                // Đọc file vào mảng byte, sau đó vào MemoryStream.
                // Cách này không khóa file và GDI+ có thể giải phóng dễ dàng.
                byte[] imageData = File.ReadAllBytes(path);
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                // Nếu file bị lỗi, trả về null
                return null;
            }
        }
        public Image GetNextFrame(string direction)
        {
            List<string> animationFrames;
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

            if (!IsFinished)
            {
                slowDownFrameRate++;
                if (slowDownFrameRate > AnimationSpeed)
                {
                    steps++;
                    if (steps >= animationFrames.Count)
                    {
                        if (IsLooping)
                        {
                            steps = 0;
                        }
                        else
                        {
                            steps = animationFrames.Count - 1;
                            IsFinished = true;
                        }
                    }
                    slowDownFrameRate = 0;
                }
            }

            if (steps >= animationFrames.Count)
            {
                steps = animationFrames.Count - 1;
            }

            // --- SỬA LỖI: Dùng hàm tải ảnh an toàn ---
            return LoadImageFromFile(animationFrames[steps]);
        }

        public Image GetDefaultFrame(string direction = "down")
        {
            try
            {
                string filePath = null;
                switch (direction)
                {
                    case "up": if (BackImages.Count > 0) filePath = BackImages[0]; break;
                    case "down": if (FrontImages.Count > 0) filePath = FrontImages[0]; break;
                    case "left": if (LeftImages.Count > 0) filePath = LeftImages[0]; break;
                    case "right": if (RightImages.Count > 0) filePath = RightImages[0]; break;
                    default: if (FrontImages.Count > 0) filePath = FrontImages[0]; break;
                }

                if (filePath != null)
                {
                    // --- SỬA LỖI: Dùng hàm tải ảnh an toàn ---
                    return LoadImageFromFile(filePath);
                }
                return null;
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
