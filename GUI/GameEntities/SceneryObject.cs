using Main;
using System;
using System.Drawing;
using System.Drawing.Imaging; // Cần cho ImageAttributes

namespace GUI.GameEntities
{
    /// <summary>
    /// Đại diện cho một vật thể trang trí (ví dụ: rêu, đuốc) gắn trên tường.
    /// </summary>
    public class SceneryObject
    {
        public float X, Y; // Vị trí logic (1:1) (Gốc trên bên trái của TILE)
        private Image _image;
        private int _drawWidth;
        private int _drawHeight;

        public SceneryObject(float tileX, float tileY, Image image)
        {
            this.X = tileX;
            this.Y = tileY;
            this._image = image;

            // Giả sử vật thể trang trí có kích thước bằng 1 TILE
            this._drawWidth = frmMainGame.TILE_SIZE;
            this._drawHeight = frmMainGame.TILE_SIZE;
        }

        /// <summary>
        /// Vẽ vật thể trang trí
        /// </summary>
        public void Draw(Graphics canvas, int scale, ImageAttributes attributes)
        {
            if (_image == null) return;

            // Tính toán vị trí vẽ đã scale
            float drawX = X * scale;
            float drawY = Y * scale;
            float drawWidthScaled = _drawWidth * scale;
            float drawHeightScaled = _drawHeight * scale;

            // Vẽ hình ảnh với ImageAttributes (để xử lý Fog of War)
            canvas.DrawImage(
                _image,
                new Rectangle((int)drawX, (int)drawY, (int)drawWidthScaled, (int)drawHeightScaled),
                0, 0, _image.Width, _image.Height,
                GraphicsUnit.Pixel,
                attributes
            );
        }
    }
}