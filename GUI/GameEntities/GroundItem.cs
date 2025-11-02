using DAL.Models;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GUI.GameEntities
{
    /// <summary>
    /// Đại diện cho một vật phẩm (Vũ khí, Giáp) rơi ra đất
    /// </summary>
    public class GroundItem
    {
        public float X, Y; // Vị trí logic (1:1)
        public int Width = 20;
        public int Height = 20;
        public RectangleF Hitbox => new RectangleF(X - Width / 2f, Y - Height / 2f, Width, Height);

        public object ItemData { get; private set; } // (Sẽ là Armors hoặc Weapons)
        private Image itemImage;
        public bool IsMarkedForDeletion = false;

        private float bobAngle = 0;
        private float bobHeight = 0;

        public GroundItem(object itemData, float spawnX, float spawnY)
        {
            this.ItemData = itemData;
            this.X = spawnX; // Tọa độ tâm
            this.Y = spawnY; // Tọa độ tâm

            this.itemImage = LoadItemImage(); // Tải ảnh dựa trên dữ liệu item
        }

        /// <summary>
        /// Tải hình ảnh của vật phẩm dựa trên dữ liệu (WeaponRank hoặc ArmorRank)
        /// </summary>
        private Image LoadItemImage()
        {
            string path = "";
            try
            {
                if (ItemData is Weapons w)
                {
                    // Đường dẫn theo yêu cầu: ...\Item\Weapon\A\
                    string folder = Path.Combine("ImgSource", "Item", "Weapon", w.WeaponRank);
                    if (Directory.Exists(folder))
                    {
                        // Lấy ảnh đầu tiên trong thư mục (ví dụ: sword_a_1.png)
                        path = Directory.GetFiles(folder, "*.png").FirstOrDefault();
                    }
                }
                else if (ItemData is Armors a)
                {
                    // Đường dẫn và tên file theo yêu cầu
                    string fileName = "";
                    if (a.ArmorRank == "A") fileName = "vwsmmfyu747f1_0.png";
                    else if (a.ArmorRank == "B") fileName = "vwsmmfyu747f1_1.png";
                    else if (a.ArmorRank == "C") fileName = "vwsmmfyu747f1_2.png";
                    else fileName = "vwsmmfyu747f1_3.png"; // Rank D

                    path = Path.Combine("ImgSource", "Armors", fileName);
                }

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    // Sử dụng phương pháp tải ảnh an toàn (giống AnimationActivity)
                    byte[] imageData = File.ReadAllBytes(path);
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể tải ảnh GroundItem: " + ex.Message);
            }

            // Ảnh dự phòng nếu lỗi
            var fallback = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(fallback))
            {
                g.FillEllipse(Brushes.Gold, 0, 0, Width, Height);
            }
            return fallback;
        }

        /// <summary>
        /// Cập nhật (ví dụ: hiệu ứng nhấp nhô)
        /// </summary>
        public void Update()
        {
            bobAngle += 0.1f; // Tốc độ nhấp nhô
            if (bobAngle > Math.PI * 2) bobAngle -= (float)(Math.PI * 2);
            bobHeight = (float)Math.Sin(bobAngle) * 3; // Biên độ 3 pixel
        }

        /// <summary>
        /// Vẽ vật phẩm lên màn hình
        /// </summary>
        public void Draw(Graphics canvas, int scale)
        {
            if (itemImage == null) return;

            int drawSize = 24; // Kích thước vẽ (logic 1:1)
            float drawX = (X * scale) - (drawSize * scale / 2f);
            // Áp dụng hiệu ứng nhấp nhô (bobHeight)
            float drawY = (Y * scale) - (drawSize * scale / 2f) + (bobHeight * scale);

            canvas.DrawImage(itemImage, drawX, drawY, drawSize * scale, drawSize * scale);
        }
    }
}