using System.Drawing;
using System.Drawing.Imaging;

namespace GUI.GameEntities
{
    // Đây là code bạn đã viết, được chuyển từ frmMainGame.cs
    public class LootEffect
    {
        private float X, Y;
        private int Width, Height;
        private Image _itemImage;
        private float opacity = 1.0f;
        private int duration = 60;
        private int timer = 0;
        private float floatSpeed = 0.5f;
        public bool IsFinished => timer >= duration;

        public LootEffect(float itemX, float itemY, int itemWidth, int itemHeight, Image itemImage = null)
        {
            Width = 15;
            Height = 15;
            X = itemX + (itemWidth / 2f) - (Width / 2f);
            Y = itemY + (itemHeight / 2f) - (Height / 2f);
            _itemImage = itemImage;
        }

        public void Update()
        {
            timer++;
            Y -= floatSpeed;
            if (timer > duration / 2)
            {
                opacity = 1.0f - (float)(timer - duration / 2) / (duration / 2);
            }
        }
        public void Draw(Graphics canvas, int scale)
        {
            if (IsFinished) return;
            int alpha = (int)(opacity * 255);
            float drawX = X * scale;
            float drawY = Y * scale;
            float drawSize = Width * scale;

            if (_itemImage != null)
            {
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = opacity;
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                canvas.DrawImage(
                    _itemImage,
                    new Rectangle((int)drawX, (int)drawY, (int)drawSize, (int)drawSize),
                    0, 0, _itemImage.Width, _itemImage.Height,
                    GraphicsUnit.Pixel,
                    ia
                );
            }
            else
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(alpha, 255, 215, 0)))
                {
                    canvas.FillRectangle(brush, drawX, drawY, drawSize, drawSize);
                }
            }
        }
    }
}