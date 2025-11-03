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

        public Boss(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();

            // SỬA LỖI CONSTRUCTOR: Khởi tạo sau khi SetStats
            patrolOrigin = new PointF(X + Width / 2, Y + Height / 2);
            _moveTarget = new PointF(X + Width / 2, Y + Height / 2);

            // --- THÊM: Đặt Cooldown Tấn Công cho Boss ---
            base.attackCooldownDuration = 60; // Boss đánh thường 1 giây
            // ------------------------------------
        }

        protected override void SetStats()
        {
            Width = 30; // Hitbox logic nhỏ
            Height = 30; // Hitbox logic nhỏ
            MaxHealth = 500;
            Health = MaxHealth;
            attackDamage = 20;
            speed = 1.5f;
            patrolSpeed = 0.5f;

            // SỬA LỖI: Tầm đánh/nhìn phải là BÌNH PHƯƠNG
            attackRange = 150 * 150; // Tầm đánh (bình phương)
            aggroRange = 10 * 10; // Tầm nhìn (bình phương)

            facingDirection = "down";
            base.patrolRange = 5 * frmMainGame.TILE_SIZE; // Tầm tuần tra
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
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return;

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

        // ===================================================================================
        // HÀM UPDATE QUAN TRỌNG VỚI LOGIC A* (CHO BOSS) - (THAY THẾ HÀM CŨ)
        // ===================================================================================
        public override void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead) return;

            if (castCooldown > 0) castCooldown--;
            if (_pathRecalculateTimer > 0) _pathRecalculateTimer--;

            // --- THÊM: Giảm Cooldown Tấn Công Thường ---
            if (attackCooldown > 0) attackCooldown--;
            // ------------------------------------

            // 1. KIỂM TRA ĐIỀU KIỆN FRIENDLY
            if (game._monstersKilled == 0 && State != MonsterState.Dead)
            {
                State = MonsterState.Friendly;
                return;
            }
            else if (State == MonsterState.Friendly && game._monstersKilled > 0)
            {
                State = MonsterState.Idle;
            }
            if (State == MonsterState.Friendly) return;

            // 2. KIỂM TRA TRẠNG THÁI (HURT, ATTACK, CASTING)
            if (State == MonsterState.Hurt)
            {
                if (hurtAnim.IsFinished)
                {
                    if (State != MonsterState.Casting)
                        State = MonsterState.Chase;
                }
                return;
            }
            // --- THÊM: Logic Tấn Công Thường (KHI HOẠT ẢNH KẾT THÚC) ---
            if (State == MonsterState.Attack)
            {
                if (attackAnim.IsFinished)
                {
                    // [KHẮC PHỤC LỖI CS7036] - Thêm 3 tham số
                    if (GetDistanceToPlayer(game, playerX, playerY) <= attackRange * attackRange)
                    {
                        game.ApplyDamageToPlayer(this.attackDamage); // DÒNG GÂY SÁT THƯƠNG
                    }
                    State = MonsterState.Chase;
                }
                return;
            }
            // ------------------------------------
            if (State == MonsterState.Casting)
            {
                if (castAnim.IsFinished)
                {
                    // SỬA LỖI: Cập nhật PointF cho đúng tâm player
                    spellEffects.Add(new SpellEffect(spellAnim, playerX + game.playerWidth / 2f, playerY + game.playerHeight / 2f));
                    State = MonsterState.Idle;
                    patrolTimer = 0;
                    castCooldown = castCooldownDuration;
                }
                return;
            }

            // 3. LOGIC AI MỚI (TẦM NHÌN & TÌM ĐƯỜNG)
            float playerCenterX = playerX + game.playerWidth / 2f;
            float playerCenterY = playerY + game.playerHeight / 2f;
            // [KHẮC PHỤC LỖI CS7036] - Thêm 3 tham số
            float distanceSq = GetDistanceToPlayer(game, playerX, playerY);
            bool canSeePlayer = false;

            if (distanceSq <= aggroRange * aggroRange)
            {
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerCenterX, playerCenterY));
            }

            // 4. QUYẾT ĐỊNH TRẠNG THÁI BOSS
            if (castCooldown == 0 && canSeePlayer)
            {
                State = MonsterState.Casting;
                castAnim.ResetFrame();
                _currentPath = null;
            }
            else if (distanceSq <= attackRange * attackRange && canSeePlayer)
            {
                State = MonsterState.Attack;
                attackAnim.ResetFrame();
                _currentPath = null;
            }
            else if (canSeePlayer && distanceSq <= aggroRange * aggroRange)
            {
                State = MonsterState.Chase;
                _moveTarget = new PointF(playerCenterX, playerCenterY);
                _currentPath = null;
            }
            else if (State == MonsterState.Chase || (distanceSq <= aggroRange * aggroRange && State != MonsterState.Patrol))
            {
                State = MonsterState.Chase;
                if (_pathRecalculateTimer <= 0)
                {
                    Point startTile = new Point((int)((X + Width / 2) / TILE_SIZE_LOGIC), (int)((Y + Height / 2) / TILE_SIZE_LOGIC));
                    Point endTile = new Point((int)(playerCenterX / TILE_SIZE_LOGIC), (int)(playerCenterY / TILE_SIZE_LOGIC));

                    if (startTile.X != endTile.X || startTile.Y != endTile.Y)
                    {
                        _currentPath = game.FindPath(startTile, endTile);
                    }
                    _pathRecalculateTimer = _pathRecalculateCooldown;
                }

                if (_currentPath != null && _currentPath.Count > 0)
                {
                    Point nextTile = _currentPath[0];
                    _moveTarget = new PointF(nextTile.X * TILE_SIZE_LOGIC + TILE_SIZE_LOGIC / 2f, nextTile.Y * TILE_SIZE_LOGIC + TILE_SIZE_LOGIC / 2f);

                    if (GetDistanceToPoint(_moveTarget) < (TILE_SIZE_LOGIC / 2f) * (TILE_SIZE_LOGIC / 2f))
                    {
                        _currentPath.RemoveAt(0);
                        if (_currentPath.Count == 0) _currentPath = null;
                    }
                }
                else
                {
                    if (GetDistanceToPoint(_moveTarget) < (TILE_SIZE_LOGIC / 2f) * (TILE_SIZE_LOGIC / 2f))
                    {
                        State = MonsterState.Idle;
                    }
                }
            }
            else
            {
                float distToOriginSq = GetDistanceToPoint(patrolOrigin);
                float maxPatrolDistSq = this.patrolRange * this.patrolRange;

                if (State == MonsterState.Patrol && distToOriginSq > maxPatrolDistSq)
                {
                    _moveTarget = patrolOrigin;
                }
                else if (State == MonsterState.Patrol && (distToOriginSq < 10 * 10 || GetDistanceToPoint(patrolTarget) < 10 * 10))
                {
                    State = MonsterState.Idle;
                }
                else if (State == MonsterState.Idle)
                {
                    patrolTimer++;
                    if (patrolTimer > rand.Next(100, 200))
                    {
                        patrolTimer = 0;
                        State = MonsterState.Patrol;
                        SetNewPatrolTarget(game);
                        _moveTarget = patrolTarget;
                    }
                }
                else if (State != MonsterState.Patrol)
                {
                    State = MonsterState.Patrol;
                    _moveTarget = patrolOrigin;
                }
            }

            // 5. THỰC HIỆN DI CHUYỂN
            if (State == MonsterState.Chase)
            {
                MoveTowards(game, _moveTarget, speed);
            }
            else if (State == MonsterState.Patrol)
            {
                MoveTowards(game, _moveTarget, patrolSpeed);
            }

            this.facingDirection = "down";
        }

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        // (Ghi đè hàm ảo của Monster)
        protected override void AdjustDrawSize(ref float drawX, ref float drawY, ref int drawWidth, ref int drawHeight, int scale)
        {
            // LƯU Ý: Boss vẽ lớn hơn hitbox
            drawWidth = 300;
            drawHeight = 300;

            // Căn lại theo logic scale
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
