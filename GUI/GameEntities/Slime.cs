using Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.GameEntities
{
    public class Slime : Monster
    {
        public Slime(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();

            // SỬA LỖI CONSTRUCTOR: Khởi tạo sau khi SetStats
            patrolOrigin = new PointF(X + Width / 2, Y + Height / 2);
            _moveTarget = new PointF(X + Width / 2, Y + Height / 2);
        }

        protected override void SetStats()
        {
            Width = 10; // Hitbox logic nhỏ
            Height = 10; // Hitbox logic nhỏ
            MaxHealth = 50;
            Health = MaxHealth;
            attackDamage = 5;
            speed = 2.5f;
            patrolSpeed = 1f;
            attackRange = 40 * 40; // Tầm đánh (bình phương)
            aggroRange = 100 * 100; // Tầm nhìn (bình phương)
            base.patrolRange = 5 * frmMainGame.TILE_SIZE; // Tầm tuần tra
        }

        protected override void LoadAnimations()
        {
            string slimeRoot = Path.Combine("ImgSource", "Char", "Monster", "Slime", "Slime_Water");

            string idleRoot = Path.Combine(slimeRoot, "Stand");
            idleAnim = new AnimationActivity(10);
            idleAnim.LoadImages(Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"), Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right"));

            string walkRoot = Path.Combine(slimeRoot, "Walk");
            walkAnim = new AnimationActivity(8);
            walkAnim.LoadImages(Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"), Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right"));

            string runRoot = Path.Combine(slimeRoot, "Run");
            runAnim = new AnimationActivity(5);
            runAnim.LoadImages(Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"), Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right"));

            string attackRoot = Path.Combine(slimeRoot, "Atk");
            attackAnim = new AnimationActivity(5) { IsLooping = false };
            attackAnim.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

            string hurtRoot = Path.Combine(slimeRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

            string deathRoot = Path.Combine(slimeRoot, "Death");
            deathAnim = new AnimationActivity(7) { IsLooping = false };
            deathAnim.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));
        }

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        // (Ghi đè hàm ảo của Monster)
        protected override void AdjustDrawSize(ref float drawX, ref float drawY, ref int drawWidth, ref int drawHeight, int scale)
        {
            // Kích thước vẽ (pixel) * scale
            drawWidth = 100; // Kích thước vẽ (pixel)
            drawHeight = 100; // Kích thước vẽ (pixel)

            // Căn lại theo logic scale
            // (Vị trí hitbox đã scale) - ( (Kích thước vẽ - Kích thước hitbox đã scale) / 2 )
            drawX = (X * scale) - ((drawWidth - (Width * scale)) / 2f);
            drawY = (Y * scale) - ((drawHeight - (Height * scale)) / 2f);
        }

        // SỬA LỖI: Xóa hàm 'Draw' vì nó đã được xử lý ở lớp Monster
        /*
        public override void Draw(Graphics canvas, int scale)
        {
           // ... (CODE CŨ BỊ XÓA) ...
        }
        */
    }
}
