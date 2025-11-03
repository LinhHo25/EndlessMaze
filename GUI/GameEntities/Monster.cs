using Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.GameEntities
{
    public abstract class Monster
    {
        public enum MonsterState { Idle, Patrol, Chase, Attack, Casting, Hurt, Dead, Friendly } // THÊM TRẠNG THÁI FRIENDLY
        public MonsterState State { get; protected set; }

        public float X, Y; // Vị trí logic 1:1
        public int Width { get; protected set; } // Kích thước logic 1:1
        public int Height { get; protected set; } // Kích thước logic 1:1
        public RectangleF Hitbox => new RectangleF(X, Y, Width, Height); // Hitbox logic 1:1

        // SỬA LỖI CS0176
        public RectangleF ScaledHitbox(int scale) => new RectangleF(X * scale, Y * scale, Width * scale, Height * scale);


        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        protected int attackDamage;
        protected float speed;
        protected float patrolSpeed;

        // --- THÊM: Biến tấn công và Cooldown ---
        protected int attackCooldown = 0;
        protected int attackCooldownDuration = 60; // 1 giây
        // ------------------------------------

        public AnimationActivity idleAnim, walkAnim, runAnim, attackAnim, hurtAnim, deathAnim;
        public AnimationActivity castAnim, spellAnim;

        protected string facingDirection = "down";
        protected float aggroRange = 210 * 210; // Tầm phát hiện (BÌNH PHƯƠNG) - ĐÃ GIẢM TỪ 300
        protected float attackRange = 90 * 90; // Tầm đánh (BÌNH PHƯƠNG)
        protected PointF patrolOrigin; // Mốc spawn
        protected PointF patrolTarget; // Điểm tuần tra
        protected int patrolTimer = 0;
        protected static Random rand = new Random();

        // Sử dụng hằng số TILE_SIZE từ frmMainGame
        protected const int TILE_SIZE_LOGIC = frmMainGame.TILE_SIZE;
        // Tầm tuần tra (ghi đè ở lớp con)
        protected float patrolRange = 5 * TILE_SIZE_LOGIC;
        // Tầm truy đuổi tối đa (mặc định 20 ô) - SỬA LỖI: PHẢI LÀ BÌNH PHƯƠNG
        protected float chaseRange = (20 * TILE_SIZE_LOGIC) * (20 * TILE_SIZE_LOGIC);


        // --- BIẾN MỚI CHO A* (PATHFINDING) ---
        protected List<Point> _currentPath = null;
        protected int _pathRecalculateCooldown = 30; // 0.5 giây
        protected int _pathRecalculateTimer = 0;
        protected PointF _moveTarget; // Mục tiêu di chuyển (logic 1:1)

        public Monster(float startX, float startY)
        {
            X = startX;
            Y = startY;
            // SỬA LỖI: Xóa 'patrolOrigin' và '_moveTarget' khỏi đây
            // Chúng sẽ được khởi tạo ở lớp CON (Slime, Orc...) SAU KHI SetStats() chạy.
            State = MonsterState.Idle;
        }

        protected abstract void LoadAnimations();
        protected abstract void SetStats();

        // THÊM: Hàm công khai để frmMainGame đọc damage (cho Body Damage)
        public int GetAttackDamage() => attackDamage;

        // SỬA LỖI: Cập nhật hàm này để lấy KÍCH THƯỚC PLAYER động từ 'game'
        protected float GetDistanceToPlayer(frmMainGame game, float playerX, float playerY)
        {
            // Lấy kích thước player từ frmMainGame (public)
            float playerCenterX = playerX + game.playerWidth / 2f;
            float playerCenterY = playerY + game.playerHeight / 2f;

            float monsterCenterX = X + Width / 2;
            float monsterCenterY = Y + Height / 2;

            float dx = playerCenterX - monsterCenterX;
            float dy = playerCenterY - monsterCenterY;
            return (dx * dx) + (dy * dy); // Trả về bình phương khoảng cách
        }

        // ===================================================================================
        // HÀM UPDATE QUAN TRỌNG VỚI LOGIC A* (THAY THẾ HÀM CŨ)
        // ===================================================================================
        public virtual void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return;
            if (game == null) return;

            if (_pathRecalculateTimer > 0) _pathRecalculateTimer--;

            // --- THÊM: Giảm Cooldown Tấn Công ---
            if (attackCooldown > 0) attackCooldown--;
            // ------------------------------------

            // --- KIỂM TRA TRẠNG THÁI (HURT, ATTACK, CASTING) ---
            if (State == MonsterState.Hurt)
            {
                if (hurtAnim.IsFinished) State = MonsterState.Chase;
                return;
            }
            // --- THÊM: Logic Tấn Công Thường (KHI HOẠT ẢNH KẾT THÚC) ---
            if (State == MonsterState.Attack)
            {
                if (attackAnim.IsFinished)
                {
                    // [KHẮC PHỤC LỖI CS7036] - Thêm 3 tham số
                    if (GetDistanceToPlayer(game, playerX, playerY) <= attackRange)
                    {
                        game.ApplyDamageToPlayer(this.attackDamage); // <--- DÒNG GÂY SÁT THƯƠNG
                    }
                    State = MonsterState.Chase; // Quay lại truy đuổi
                }
                return; // Không làm gì khác khi đang tấn công
            }
            // ------------------------------------
            if (State == MonsterState.Casting)
            {
                return; // Logic cast của Boss sẽ override
            }

            // --- LOGIC AI MỚI (TẦM NHÌN & TÌM ĐƯỜNG) ---
            float playerCenterX = playerX + game.playerWidth / 2f;
            float playerCenterY = playerY + game.playerHeight / 2f;
            // [KHẮC PHỤC LỖI CS7036] - Thêm 3 tham số
            float distanceSq = GetDistanceToPlayer(game, playerX, playerY);
            bool canSeePlayer = false;

            // 1. Kiểm tra tầm nhìn (Line of Sight)
            if (distanceSq <= chaseRange) // Chỉ kiểm tra tầm nhìn trong phạm vi truy đuổi (đã bình phương)
            {
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerCenterX, playerCenterY));
            }

            // 2. QUYẾT ĐỊNH TRẠNG THÁI (FSM) - ĐÃ SẮP XẾP LẠI

            // TRƯỜNG HỢP 1: TẤN CÔNG (Gần nhất, ưu tiên cao nhất)
            if (distanceSq <= attackRange && canSeePlayer && attackCooldown == 0)
            {
                State = MonsterState.Attack;
                attackAnim.ResetFrame();
                attackCooldown = attackCooldownDuration; // Bắt đầu cooldown
                _currentPath = null;
            }
            // TRƯỜNG HỢP 2: TRUY ĐUỔI (Nhìn thấy và trong tầm aggro)
            else if (canSeePlayer && distanceSq <= aggroRange)
            {
                State = MonsterState.Chase;
                _moveTarget = new PointF(playerCenterX, playerCenterY);
                _currentPath = null; // Hủy pathfinding vì thấy
            }
            // TRƯỜNG HỢP 3: TÌM ĐƯỜNG (Không thấy, nhưng ở gần)
            else if (!canSeePlayer && distanceSq <= aggroRange)
            {
                State = MonsterState.Chase; // Vẫn là Chase, nhưng dùng A*
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
                    // Không tìm thấy đường, quay về mốc
                    State = MonsterState.Patrol;
                    _moveTarget = patrolOrigin;
                }
            }
            // TRƯỜNG HỢP 4: NGỪNG ĐUỔI (Player đã chạy xa) HOẶC ĐANG TUẦN TRA/RẢNH
            else
            {
                // Nếu đang Chase, chuyển sang Patrol (để quay về)
                if (State == MonsterState.Chase)
                {
                    State = MonsterState.Patrol;
                    _moveTarget = patrolOrigin; // Mục tiêu là quay về mốc
                }

                float distToOriginSq = GetDistanceToPoint(patrolOrigin);
                float maxPatrolDistSq = this.patrolRange * this.patrolRange;

                // Nếu đang tuần tra và đi quá xa mốc
                if (State == MonsterState.Patrol && distToOriginSq > maxPatrolDistSq)
                {
                    _moveTarget = patrolOrigin; // Quay về mốc
                }
                // Nếu đã về gần mốc (hoặc gần điểm tuần tra)
                else if (State == MonsterState.Patrol && (GetDistanceToPoint(_moveTarget) < 10 * 10))
                {
                    State = MonsterState.Idle; // Về đến nơi, đứng chờ
                }
                // Nếu đang rảnh (Idle)
                else if (State == MonsterState.Idle)
                {
                    patrolTimer++;
                    if (patrolTimer > rand.Next(100, 200))
                    {
                        patrolTimer = 0;
                        State = MonsterState.Patrol;
                        SetNewPatrolTarget(game); // Tìm điểm tuần tra mới
                        _moveTarget = patrolTarget;
                    }
                }
            }

            // --- THỰC HIỆN DI CHUYỂN ---
            if (State == MonsterState.Chase)
            {
                MoveTowards(game, _moveTarget, speed);
            }
            else if (State == MonsterState.Patrol)
            {
                MoveTowards(game, _moveTarget, patrolSpeed);
            }
        }

        protected void MoveTowards(frmMainGame game, PointF target, float speed)
        {
            if (game == null) return;

            float dx = target.X - (X + Width / 2);
            float dy = target.Y - (Y + Height / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 1.0f) // Thêm ngưỡng nhỏ để tránh rung lắc
            {
                dx /= distance;
                dy /= distance;

                float moveX = dx * speed;
                float moveY = dy * speed;

                // 1. Thử di chuyển X
                RectangleF nextHitboxX = new RectangleF(X + moveX, Y, Width, Height);
                RectangleF scaledNextHitboxX = new RectangleF(
                    nextHitboxX.X * frmMainGame.RENDER_SCALE,
                    nextHitboxX.Y * frmMainGame.RENDER_SCALE,
                    nextHitboxX.Width * frmMainGame.RENDER_SCALE,
                    nextHitboxX.Height * frmMainGame.RENDER_SCALE
                );

                if (!game.IsCollidingWithWallScaled(scaledNextHitboxX))
                {
                    X += moveX;
                }
                else
                {
                    // Logic trượt Y
                    float scaledSlideTolerance = frmMainGame.BASE_SLIDE_TOLERANCE / frmMainGame.RENDER_SCALE;
                    if (moveY != 0)
                    {
                        float slideY = moveY > 0 ? scaledSlideTolerance : -scaledSlideTolerance;
                        RectangleF slideHitboxY = new RectangleF(X + moveX, Y + slideY, Width, Height);
                        RectangleF scaledSlideHitboxY = new RectangleF(
                            slideHitboxY.X * frmMainGame.RENDER_SCALE,
                            slideHitboxY.Y * frmMainGame.RENDER_SCALE,
                            slideHitboxY.Width * frmMainGame.RENDER_SCALE,
                            slideHitboxY.Height * frmMainGame.RENDER_SCALE
                        );
                        if (!game.IsCollidingWithWallScaled(scaledSlideHitboxY))
                        {
                            Y += slideY;
                        }
                    }
                }

                // 2. Thử di chuyển Y
                RectangleF nextHitboxY = new RectangleF(X, Y + moveY, Width, Height);
                RectangleF scaledNextHitboxY = new RectangleF(
                    nextHitboxY.X * frmMainGame.RENDER_SCALE,
                    nextHitboxY.Y * frmMainGame.RENDER_SCALE,
                    nextHitboxY.Width * frmMainGame.RENDER_SCALE,
                    nextHitboxY.Height * frmMainGame.RENDER_SCALE
                );

                if (!game.IsCollidingWithWallScaled(scaledNextHitboxY))
                {
                    Y += moveY;
                }
                else
                {
                    // Logic trượt X
                    float scaledSlideTolerance = frmMainGame.BASE_SLIDE_TOLERANCE / frmMainGame.RENDER_SCALE;
                    if (moveX != 0)
                    {
                        float slideX = moveX > 0 ? scaledSlideTolerance : -scaledSlideTolerance;
                        RectangleF slideHitboxX = new RectangleF(X + slideX, Y + moveY, Width, Height);
                        RectangleF scaledSlideHitboxX = new RectangleF(
                            slideHitboxX.X * frmMainGame.RENDER_SCALE,
                            slideHitboxX.Y * frmMainGame.RENDER_SCALE,
                            slideHitboxX.Width * frmMainGame.RENDER_SCALE,
                            slideHitboxX.Height * frmMainGame.RENDER_SCALE
                        );
                        if (!game.IsCollidingWithWallScaled(scaledSlideHitboxX))
                        {
                            X += slideX;
                        }
                    }
                }

                // 3. Cập nhật hướng nhìn
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    facingDirection = (dx > 0) ? "right" : "left";
                }
                else
                {
                    facingDirection = (dy > 0) ? "down" : "up";
                }
            }
        }

        protected float GetDistanceToPoint(PointF target)
        {
            float monsterCenterX = X + Width / 2;
            float monsterCenterY = Y + Height / 2;
            float dx = target.X - monsterCenterX;
            float dy = target.Y - monsterCenterY;
            return (dx * dx) + (dy * dy);
        }

        // --- HÀM TẠO MỤC TIÊU PATROL CÓ KIỂM TRA VA CHẠM ---
        protected void SetNewPatrolTarget(frmMainGame game)
        {
            if (game == null)
            {
                patrolTarget = patrolOrigin;
                return;
            }

            // Dùng patrolRange (được ghi đè bởi Orc, Slime...)
            int range = (int)this.patrolRange;
            RectangleF testHitbox = new RectangleF(X, Y, Width, Height);

            for (int i = 0; i < 5; i++)
            {
                // SỬA LỖI: Lấy tâm (center) của hitbox, không phải top-left (X, Y)
                // Lấy vị trí tuần tra ngẫu nhiên xung quanh mốc (patrolOrigin)
                float targetX = patrolOrigin.X + rand.Next(-range, range);
                float targetY = patrolOrigin.Y + rand.Next(-range, range);

                testHitbox.X = targetX - Width / 2f; // Căn tâm
                testHitbox.Y = targetY - Height / 2f; // Căn tâm

                RectangleF scaledTestHitbox = new RectangleF(
                    testHitbox.X * frmMainGame.RENDER_SCALE,
                    testHitbox.Y * frmMainGame.RENDER_SCALE,
                    testHitbox.Width * frmMainGame.RENDER_SCALE,
                    testHitbox.Height * frmMainGame.RENDER_SCALE
                );

                if (!game.IsCollidingWithWallScaled(scaledTestHitbox))
                {
                    patrolTarget = new PointF(targetX, targetY);
                    return;
                }
            }
            patrolTarget = patrolOrigin; // Quay về mốc nếu không tìm thấy
        }

        public virtual void TakeDamage(int damage)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return;

            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                State = MonsterState.Dead;
                deathAnim.ResetFrame();
            }
            else
            {
                State = MonsterState.Hurt;
                hurtAnim.ResetFrame();
            }
        }

        public virtual void Draw(Graphics canvas, int scale)
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
                    imageToDraw = (castAnim != null) ? castAnim.GetNextFrame(currentDirection) : idleAnim.GetNextFrame(currentDirection);
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
                // SỬA LỖI: Không dùng 'using' vì ảnh được cache
                int drawWidth = Width;
                int drawHeight = Height;
                float drawX = X * scale;
                float drawY = Y * scale;

                AdjustDrawSize(ref drawX, ref drawY, ref drawWidth, ref drawHeight, scale);
                canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
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

        // Hàm ảo để các lớp con ghi đè (cho phép vẽ ảnh to hơn hitbox)
        protected virtual void AdjustDrawSize(ref float drawX, ref float drawY, ref int drawWidth, ref int drawHeight, int scale)
        {
            // Lớp Monster cha vẽ 1:1 với hitbox (đã nhân scale)
            drawWidth = Width * scale;
            drawHeight = Height * scale;
        }

        protected void DrawHealthBar(Graphics canvas, int scale)
        {
            int barWidth = Width * scale;
            int barHeight = 10;
            float barY = Y * scale - barHeight - 5;
            float barX = X * scale;

            canvas.FillRectangle(Brushes.Black, barX, barY, barWidth, barHeight);
            float healthPercentage = (float)Health / MaxHealth;
            int currentHealthWidth = (int)(barWidth * healthPercentage);
            canvas.FillRectangle(Brushes.LawnGreen, barX, barY, currentHealthWidth, barHeight);
            canvas.DrawRectangle(Pens.Black, barX, barY, barWidth, barHeight);
        }
    }
}

