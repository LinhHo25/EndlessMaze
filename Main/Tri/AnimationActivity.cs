using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Main.Tri
{
    // Lớp này quản lý một bộ hoạt ảnh (ví dụ: "đi bộ" hoặc "tấn công")
    // Nó sẽ tải các ảnh từ 4 thư mục con (Back, Front, Left, Right)
    internal class AnimationActivity
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

        // Biến mới để "khóa" hướng cho hoạt ảnh không lặp
        private string lockedDirection = null;

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
            BackImages = LoadImageList(backDirectory);
            FrontImages = LoadImageList(frontDirectory);
            LeftImages = LoadImageList(leftDirectory);
            RightImages = LoadImageList(rightDirectory);
        }

        /// <summary>
        /// Hàm phụ để tải một danh sách ảnh từ một thư mục
        /// </summary>
        private List<Image> LoadImageList(string directory)
        {
            List<Image> images = new List<Image>();
            try
            {
                if (Directory.Exists(directory))
                {
                    // Lấy file theo thứ tự tên
                    var files = Directory.GetFiles(directory, "*.png")
                                        .OrderBy(f => f)
                                        .ToList();
                    foreach (string file in files)
                    {
                        images.Add(Image.FromFile(file));
                    }
                }
                else
                {
                    Console.WriteLine($"Lỗi: Không tìm thấy thư mục: {directory}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh từ {directory}: {ex.Message}");
            }
            return images;
        }

        /// <summary>
        /// Lấy khung hình tiếp theo của hoạt ảnh
        /// </summary>
        public Image GetNextFrame(string direction)
        {
            // 1. Chọn danh sách ảnh dựa trên hướng
            List<Image> animationFrames;
            switch (direction)
            {
                case "up":
                    animationFrames = BackImages;
                    break;
                case "down":
                    animationFrames = FrontImages;
                    break;
                case "left":
                    animationFrames = LeftImages;
                    break;
                case "right":
                    animationFrames = RightImages;
                    break;
                default:
                    animationFrames = FrontImages; // Mặc định
                    break;
            }

            if (animationFrames == null || animationFrames.Count == 0)
            {
                return null;
            }

            // 2. Xử lý cho hoạt ảnh KHÔNG LẶP LẠI (ví dụ: attack, death)
            if (!IsLooping)
            {
                if (lockedDirection == null)
                {
                    lockedDirection = direction;
                    IsFinished = false;
                }

                if (direction != lockedDirection)
                {
                    switch (lockedDirection)
                    {
                        case "up": animationFrames = BackImages; break;
                        case "down": animationFrames = FrontImages; break;
                        case "left": animationFrames = LeftImages; break;
                        case "right": animationFrames = RightImages; break;
                    }
                    if (animationFrames == null || animationFrames.Count == 0) return null;
                }
            }
            else
            {
                IsFinished = false;
                lockedDirection = null;
            }

            // 3. Tăng bộ đếm tốc độ
            slowDownFrameRate++;
            if (slowDownFrameRate > animationSpeed)
            {
                // 4. Tăng frame (bước)
                steps++;
                slowDownFrameRate = 0; // Reset bộ đếm tốc độ

                if (steps >= animationFrames.Count)
                {
                    if (IsLooping)
                    {
                        steps = 0; // Quay về frame đầu tiên
                    }
                    else
                    {
                        steps = animationFrames.Count - 1; // Giữ ở frame cuối
                        IsFinished = true; // Đánh dấu là đã chạy xong
                        lockedDirection = null; // Mở khóa hướng
                    }
                }
            }

            if (steps >= animationFrames.Count)
            {
                steps = animationFrames.Count - 1;
            }

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
            IsFinished = false;
            lockedDirection = null;
        }
    }
}
