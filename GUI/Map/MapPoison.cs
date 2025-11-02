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
    public class MapPoison : GameMapBase
    {
        // --- Các biến riêng của Map Độc ---
        private List<RectangleF> _poisonSwamps = new List<RectangleF>();

        public override void LoadMapTextures()
        {
            // SỬA: Dùng "Grass_Style" cho Map Độc
            // (LƯU Ý: Sửa "floor_1.png" và "wall_1.png" nếu tên tệp của bạn khác)
            floorTexture = LoadImageSafe(Path.Combine("ImgSource", "Block", "Grass_Style", "Floor", "floor_1.png"));
            wallTexture = LoadImageSafe(Path.Combine("ImgSource", "Block", "Grass_Style", "Wall", "wall_1.png"));
        }

        public override void InitializeMapMechanics()
        {
            MapName = "Rừng Chướng Khí";

            // Tạo bẫy
            for (int i = 0; i < 50; i++)
            {
                _poisonSwamps.Add(GetRandomFloorRect(100, 100)); // Bẫy 100x100 px
            }

            // Timer chạy mỗi 2 giây để trừ 0.5% máu
            _specialMechanicTimer = new Timer();
            _specialMechanicTimer.Interval = 2000;
            _specialMechanicTimer.Tick += PoisonDamage_Tick;
            _specialMechanicTimer.Start();
        }

        // --- Timer Map Độc (từ v6) ---
        private void PoisonDamage_Tick(object sender, EventArgs e)
        {
            if (isDead) return;
            if (_isPoisoned)
            {
                float damage = playerMaxHealth * 0.005f; // 0.5% Máu
                PlayerTakesDamage(damage);
            }
        }

        public override void HandleMapMechanics()
        {
            // Reset trạng thái
            _isPoisoned = false;

            foreach (var swamp in _poisonSwamps)
            {
                if (GetPlayerHitbox().IntersectsWith(swamp))
                {
                    _isPoisoned = true; // Bị độc
                    break;
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
                canvas.Clear(Color.DarkGreen); // Màu dự phòng
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

            // 3. Vẽ Bẫy (Đầm lầy độc)
            using (Brush b = new SolidBrush(Color.FromArgb(100, Color.Purple)))
            {
                foreach (var swamp in _poisonSwamps) canvas.FillRectangle(b, swamp);
            }
        }
    }
}

