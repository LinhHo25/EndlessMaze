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
    public class MapFire : GameMapBase
    {
        // --- Các biến riêng của Map Lửa ---
        private List<RectangleF> _unstableGrounds = new List<RectangleF>();
        // (Bạn có thể thêm hiệu ứng khói/lửa ở đây)

        public override void LoadMapTextures()
        {
            // (LƯU Ý: Sửa "floor_1.png" và "wall_1.png" nếu tên tệp của bạn khác)
            floorTexture = LoadImageSafe(Path.Combine("ImgSource", "Block", "Flame_Style", "Floor", "floor_1.png"));
            wallTexture = LoadImageSafe(Path.Combine("ImgSource", "Block", "Flame_Style", "Wall", "wall_1.png"));
        }

        public override void InitializeMapMechanics()
        {
            MapName = "Diệm Ngục";

            // Tạo bẫy
            for (int i = 0; i < 50; i++)
            {
                _unstableGrounds.Add(GetRandomFloorRect(80, 80)); // Bẫy 80x80 px
            }

            // Giảm 10% stamina
            float staminaLoss = playerMaxStamina * 0.10f;
            playerMaxStamina -= staminaLoss;
            playerStamina = Math.Min(playerStamina, playerMaxStamina);

            _specialMechanicTimer = null; // Không cần timer
        }

        public override void HandleMapMechanics()
        {
            // Cơ chế sụt lún
            foreach (var ground in _unstableGrounds)
            {
                if (GetPlayerHitbox().IntersectsWith(ground))
                {
                    FallOver(500); // Ngã 0.5s
                }
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
                canvas.Clear(Color.DarkRed); // Màu dự phòng
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

            // 3. Vẽ Bẫy (Sụt lún)
            using (Brush b = new SolidBrush(Color.FromArgb(100, Color.Orange)))
            {
                foreach (var ground in _unstableGrounds) canvas.FillRectangle(b, ground);
            }

            // (Bạn có thể thêm hiệu ứng khói/đổ mồ hôi ở đây)
        }
    }
}

