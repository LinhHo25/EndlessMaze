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
        // ... [Nội dung Slime không đổi] ...
        public Slime(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();
        }

        protected override void SetStats()
        {
            Width = 40;
            Height = 40;
            MaxHealth = 50;
            Health = MaxHealth;
            attackDamage = 5;
            speed = 2.5f;
            patrolSpeed = 1f;
            attackRange = 30;
            aggroRange = 220;
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
            attackAnim = new AnimationActivity(10) { IsLooping = false };
            attackAnim.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

            string hurtRoot = Path.Combine(slimeRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

            string deathRoot = Path.Combine(slimeRoot, "Death");
            deathAnim = new AnimationActivity(7) { IsLooping = false };
            deathAnim.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));
        }

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        public override void Draw(Graphics canvas, int scale)
        {
            Image imageToDraw = null;
            string currentDirection = (State == MonsterState.Friendly) ? "down" : facingDirection;

            switch (State)
            {
                case MonsterState.Idle:
                case MonsterState.Friendly:
                    imageToDraw = idleAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Casting:
                    break; // Slime không có Cast
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(currentDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    // LƯU Ý: Slime vẽ lớn hơn hitbox 40x40. Kích thước vẽ không cần scale 3x
                    int drawWidth = 80;
                    int drawHeight = 80;
                    // VỊ TRÍ DRAW ĐÃ ĐƯỢC SCALE
                    float drawX = X * scale - (drawWidth - Width) / 2;
                    float drawY = Y * scale - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead && State != MonsterState.Friendly)
            {
                DrawHealthBar(canvas, scale);
            }
            else if (State == MonsterState.Friendly)
            {
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale - 10, Y * scale - 20);
            }
        }
    }
}
