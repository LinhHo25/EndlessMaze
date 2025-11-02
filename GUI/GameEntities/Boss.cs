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
    public class Boss : Monster
    {
        private int castCooldown = 0;
        private int castCooldownDuration = 333;

        // ... [Nội dung Boss không đổi] ...
        public Boss(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();
        }

        protected override void SetStats()
        {
            Width = 100;
            Height = 100;
            MaxHealth = 500;
            Health = MaxHealth;
            attackDamage = 20;
            speed = 1.5f;
            patrolSpeed = 0.5f;
            attackRange = 150;
            aggroRange = 450;
            facingDirection = "down";
        }

        protected override void LoadAnimations()
        {
            string bossRoot = Path.Combine("ImgSource", "Char", "Monster", "Boss");

            string idleRoot = Path.Combine(bossRoot, "Stand");
            idleAnim = new AnimationActivity(12);
            idleAnim.LoadImages(null, idleRoot, null, null);

            string walkRoot = Path.Combine(bossRoot, "Walk");
            walkAnim = new AnimationActivity(10);
            walkAnim.LoadImages(null, walkRoot, null, null);

            runAnim = new AnimationActivity(walkAnim.AnimationSpeed);
            // SỬA LỖI CS1061: Thay thế 'walkAnim.DrawImages' bằng 'walkAnim.RightDir'
            runAnim.LoadImages(walkAnim.BackDir, walkAnim.FrontDir, walkAnim.LeftDir, walkAnim.RightDir);

            string attackRoot = Path.Combine(bossRoot, "Attack");
            attackAnim = new AnimationActivity(5) { IsLooping = false };
            attackAnim.LoadImages(null, attackRoot, null, null);

            string hurtRoot = Path.Combine(bossRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(null, hurtRoot, null, null);

            string deathRoot = Path.Combine(bossRoot, "Death");
            deathAnim = new AnimationActivity(7) { IsLooping = false };
            deathAnim.LoadImages(null, deathRoot, null, null);

            string castRoot = Path.Combine(bossRoot, "Cast");
            castAnim = new AnimationActivity(5) { IsLooping = false };
            castAnim.LoadImages(null, castRoot, null, null);

            string spellRoot = Path.Combine(bossRoot, "Spell");
            spellAnim = new AnimationActivity(4) { IsLooping = false };
            spellAnim.LoadImages(null, spellRoot, null, null);
        }

        public override void TakeDamage(int damage)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return; // BẢO VỆ

            if (State == MonsterState.Casting)
            {
                Health -= damage;
                if (Health <= 0)
                {
                    Health = 0;
                    State = MonsterState.Dead;
                    deathAnim.ResetFrame();
                }
                return;
            }
            base.TakeDamage(damage);
        }


        public override void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead) return;

            // KIỂM TRA ĐIỀU KIỆN FRIENDLY (CHỈ ÁP DỤNG TRONG BOSS.UPDATE)
            if (game._monstersKilled == 0 && State != MonsterState.Dead)
            {
                State = MonsterState.Friendly;
                return; // KHÔNG THỰC HIỆN LOGIC CHIẾN ĐẤU
            }
            else if (State == MonsterState.Friendly && game._monstersKilled > 0)
            {
                // Nếu đã giết quái vật, Boss chuyển sang chế độ Chiến đấu
                State = MonsterState.Idle;
            }

            if (State == MonsterState.Friendly) return; // BỎ QUA LOGIC DƯỚI NẾU FRIENDLY

            if (castCooldown > 0) castCooldown--;

            float distanceSq = GetDistanceToPlayer(playerX, playerY);

            if (State == MonsterState.Hurt)
            {
                if (hurtAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Attack)
            {
                if (attackAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Casting)
            {
                if (castAnim.IsFinished)
                {
                    // Spell effect at Player Center (playerX + 12.5, playerY + 12.5)
                    spellEffects.Add(new SpellEffect(spellAnim, playerX + 12.5f, playerY + 12.5f));

                    State = MonsterState.Idle;
                    patrolTimer = 0;
                    castCooldown = castCooldownDuration;
                }
                return;
            }

            bool canSeePlayer = false;
            if (distanceSq <= aggroRange * aggroRange)
            {
                // Kiểm tra tầm nhìn: Monster Center to Player Center (playerX + 12.5, playerY + 12.5)
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerX + 12.5f, playerY + 12.5f));
            }

            if (castCooldown == 0 && canSeePlayer)
            {
                State = MonsterState.Casting;
                castAnim.ResetFrame();
            }
            else if (distanceSq <= attackRange * attackRange && canSeePlayer)
            {
                // --- SỬA: KÍCH HOẠT TẤN CÔNG VÀ GÂY SÁT THƯƠNG ---
                // Chỉ kích hoạt nếu không đang tấn công
                if (State != MonsterState.Attack)
                {
                    State = MonsterState.Attack;
                    attackAnim.ResetFrame();
                    // GỌI HÀM GÂY SÁT THƯƠNG CHO PLAYER
                    game.ApplyDamageToPlayer(this.attackDamage);
                }
            }
            else if (distanceSq <= aggroRange * aggroRange && canSeePlayer)
            {
                State = MonsterState.Chase;
            }
            else if (State == MonsterState.Chase)
            {
                State = MonsterState.Patrol;
                // --- SỬA LỖI: Truyền 'game' vào ---
                SetNewPatrolTarget(game);
            }
            else if (State == MonsterState.Idle)
            {
                patrolTimer++;
                if (patrolTimer > rand.Next(100, 200))
                {
                    patrolTimer = 0;
                    State = MonsterState.Patrol;
                    // --- SỬA LỖI: Truyền 'game' vào ---
                    SetNewPatrolTarget(game);
                }
            }
            else if (State == MonsterState.Patrol)
            {
                MoveTowards(game, patrolTarget, patrolSpeed);
                if (GetDistanceToPoint(patrolTarget) < 10 * 10)
                {
                    State = MonsterState.Idle;
                }
            }

            if (State == MonsterState.Patrol)
            {
                MoveTowards(game, patrolTarget, patrolSpeed);
            }
            else if (State == MonsterState.Chase)
            {
                // Di chuyển về Player Center (playerX + 12.5, playerY + 12.5)
                MoveTowards(game, new PointF(playerX + 12.5f, playerY + 12.5f), speed);
            }

            this.facingDirection = "down";
        }

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        public override void Draw(Graphics canvas, int scale)
        {
            Image imageToDraw = null;
            string currentDirection = (State == MonsterState.Friendly) ? "down" : facingDirection; // Boss Friendly luôn nhìn xuống

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
                    imageToDraw = castAnim.GetNextFrame(currentDirection);
                    break;
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
                    int drawWidth = 150;
                    int drawHeight = 150;
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
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale + 20, Y * scale - 10);
            }
        }

    }
}
