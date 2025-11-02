using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace Main
{
    public class MapWater : GameMapBase
    {
        // --- Các biến riêng của Map Nước ---
        private bool _isRaining = false;
        private List<RectangleF> _rainDrops = new List<RectangleF>(); // (Hiệu ứng mưa)

        public override void LoadMapTextures()
        {
            // (LƯU Ý: Sửa "floor_1.png" và "wall_1.png" nếu tên tệp của bạn khác)
            floorTexture = LoadImageSafe(Path.Combine("ImgSource", "Block", "Water_Style", "Floor", "floor_1.png"));
            wallTexture = LoadImageSafe(Path.Combine("ImgSource", "Block", "Water_Style", "Wall", "wall_1.png"));
        }

        public override void InitializeMapMechanics()
        {
            MapName = "Hàn Thủy Vực";

            _isRaining = true; // Kích hoạt cơ chế logic

            // Khởi tạo 100 giọt mưa
            for (int i = 0; i < 100; i++)
            {
                _rainDrops.Add(new RectangleF(rand.Next(0, mapWidth), rand.Next(0, mapHeight), 2, 10));
            }

            _specialMechanicTimer = null;
        }

        public override void HandleMapMechanics()
        {
            // Cơ chế trơn trượt (ngã)
            if (_isRaining && isDashing && _dashCountSinceTouchGround > 3)
            {
                FallOver(500); // Ngã 0.5s
            }

            // Cập nhật hiệu ứng mưa (cho đẹp)
            for (int i = 0; i < _rainDrops.Count; i++)
            {
                var drop = _rainDrops[i];
                drop.Y += 15; // Tốc độ rơi
                if (drop.Y > mapHeight)
                {
                    drop.Y = -10; // Reset
                    drop.X = rand.Next(0, mapWidth);
                }
                _rainDrops[i] = drop;
            }
        }

        public override void DrawMapEffects(Graphics canvas)
        {
            // 1. Vẽ Sàn
            if (floorTexture != null)
            {
                // SỬA: Vẽ toàn bộ sàn của map lớn
                using (TextureBrush textureBrush = new TextureBrush(floorTexture, WrapMode.Tile))
                {
                    canvas.FillRectangle(textureBrush, wallThickness, wallThickness,
                        mapWidth - (wallThickness * 2), mapHeight - (wallThickness * 2));
                }
            }
            else
            {
                canvas.Clear(Color.DarkBlue); // Màu dự phòng
            }

            // 2. SỬA: Vẽ Tường (Tường Mê Cung)
            if (wallTexture != null)
            {
                // Vẽ các bức tường mê cung bên trong
                foreach (var wallRect in _mazeWallRects)
                {
                    canvas.DrawImage(wallTexture, wallRect);
                }

                // Vẽ 4 bức tường bao bên ngoài (dùng TextureBrush)
                using (TextureBrush textureBrush = new TextureBrush(wallTexture, WrapMode.Tile))
                {
                    canvas.FillRectangle(textureBrush, 0, 0, mapWidth, wallThickness); // Trên
                    canvas.FillRectangle(textureBrush, 0, mapHeight - wallThickness, mapWidth, wallThickness); // Dưới
                    canvas.FillRectangle(textureBrush, 0, 0, wallThickness, mapHeight); // Trái
                    // Phải (có lỗ hổng)
                    canvas.FillRectangle(textureBrush, mapWidth - wallThickness, 0, wallThickness, gapYPosition);
                    canvas.FillRectangle(textureBrush, mapWidth - wallThickness, gapYPosition + gapSize, wallThickness, mapHeight - (gapYPosition + gapSize));
                }
            }

            // 3. Vẽ Hiệu ứng (Mưa)
            if (_isRaining)
            {
                using (Brush rainBrush = new SolidBrush(Color.FromArgb(150, 170, 200, 255)))
                {
                    foreach (var drop in _rainDrops)
                    {
                        canvas.FillRectangle(rainBrush, drop);
                    }
                }
            }
        }
    }
}

