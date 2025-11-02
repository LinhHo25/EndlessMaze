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
    public class Orc : Monster
    {
        // ... [Nội dung Orc không đổi] ...
        public Orc(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();
        }

        protected override void SetStats()
        {
            Width = 45;
            Height = 45;
            MaxHealth = 120;
            Health = MaxHealth;
            attackDamage = 10;
            speed = 2.0f;
            patrolSpeed = 0.8f;
            attackRange = 120;
            aggroRange = 250;
        }

        protected override void LoadAnimations()
        {
            string orcRoot = Path.Combine("ImgSource", "Char", "Monster", "Orc", "Orc_Water");

            string idleRoot = Path.Combine(orcRoot, "Stand");
            idleAnim = new AnimationActivity(12);
            idleAnim.LoadImages(Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"), Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right"));

            string walkRoot = Path.Combine(orcRoot, "Walk");
            walkAnim = new AnimationActivity(10);
            walkAnim.LoadImages(Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"), Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right"));

            string runRoot = Path.Combine(orcRoot, "Run");
            runAnim = new AnimationActivity(6);
            runAnim.LoadImages(Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"), Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right"));

            string attackRoot = Path.Combine(orcRoot, "Atk");
            attackAnim = new AnimationActivity(5) { IsLooping = false };
            attackAnim.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

            string hurtRoot = Path.Combine(orcRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

            string deathRoot = Path.Combine(orcRoot, "Death");
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
                    break; // Orc không có Cast
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
                    // LƯU Ý: Orc vẽ lớn hơn hitbox 45x45
                    int drawWidth = 110;
                    int drawHeight = 110;
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
