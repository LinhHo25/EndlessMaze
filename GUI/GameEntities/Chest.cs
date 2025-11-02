using System.Drawing;
using System.IO;
using Main; // Cần thiết cho AnimationActivity

namespace GUI.GameEntities
{
    // Đây là code bạn đã viết, được chuyển từ frmMainGame.cs
    public class Chest
    {
        public float X, Y; // Vị trí TÂM logic (1:1)
        public int Width { get; private set; } = 20;
        public int Height { get; private set; } = 20;
        public bool IsOpened { get; set; } = false;
        public bool IsAnimationFinished { get; set; } = false;
        public AnimationActivity OpenAnimation { get; private set; }

        public RectangleF Hitbox => new RectangleF(X - Width / 2f, Y - Height / 2f, Width, Height);

        public Chest(float centerX, float centerY)
        {
            X = centerX;
            Y = centerY;
            string chestAnimDir = Path.Combine("ImgSource", "Structure", "chest");
            OpenAnimation = new AnimationActivity(5) { IsLooping = false };
            OpenAnimation.LoadImages(chestAnimDir, chestAnimDir, chestAnimDir, chestAnimDir);
        }

        public void StartOpenAnimation()
        {
            IsOpened = true;
            OpenAnimation.ResetFrame();
        }

        public void UpdateAnimation()
        {
            if (IsOpened && !IsAnimationFinished)
            {
                OpenAnimation.GetNextFrame("down");
                if (OpenAnimation.IsFinished)
                {
                    IsAnimationFinished = true;
                }
            }
        }
    }
}