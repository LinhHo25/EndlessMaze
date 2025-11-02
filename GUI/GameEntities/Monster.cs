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

        public AnimationActivity idleAnim, walkAnim, runAnim, attackAnim, hurtAnim, deathAnim;
        public AnimationActivity castAnim, spellAnim;

        protected string facingDirection = "down";
        protected float aggroRange = 300; // Tầm phát hiện (sẽ bị ghi đè bởi Orc, Slime, Boss)
        protected float attackRange = 30;
        protected PointF patrolOrigin;
        protected PointF patrolTarget;
        protected int patrolTimer = 0;
        protected static Random rand = new Random();

        // --- THAY ĐỔI: patrolRange, chaseRange được thiết lập bởi lớp con (ví dụ Orc) hoặc mặc định ở đây ---
        protected float patrolRange = 10 * frmMainGame.TILE_SIZE; // 10 ô logic (mặc định)
        protected float chaseRange = 20 * frmMainGame.TILE_SIZE; // 20 ô logic (mặc định)
        // --- THÊM: Biến cho A* Pathfinding ---
        private List<Point> _currentPathTiles = new List<Point>();
        private int _pathUpdateTimer = 0; // Đếm ngược để làm mới đường đi
        private Point _lastPlayerTile = Point.Empty; // Vị trí tile cuối cùng của người chơi
        // --- THÊM: Biến Cooldown Tấn công ---
        protected int attackCooldown = 0;
        // (60 frames ~ 2 giây, có thể được lớp con ghi đè)
        protected virtual int attackCooldownDuration => 60;

        public Monster(float startX, float startY)
        {
            X = startX;
            Y = startY;
            // THAY ĐỔI: Lưu mốc spawn chính xác
            patrolOrigin = new PointF(X, Y);
            State = MonsterState.Idle; // Bắt đầu ở trạng thái Đứng yên
        }

        protected abstract void LoadAnimations();
        protected abstract void SetStats();

        // --- SỬA LỖI AI: Đổi tham số thành tâm người chơi ---
        // (Hàm Update đã truyền vào tâm, không cần cộng thêm ở đây)
        protected float GetDistanceToPlayer(float playerCenterX, float playerCenterY)
        {
            // Player hitbox is 25x25 starting at (playerX, playerY). Center is at (playerX + 12.5, playerY + 12.5).
            // float playerCenterX = playerX + 12.5f; // <-- XÓA DÒNG NÀY
            // float playerCenterY = playerY + 12.5f; // <-- XÓA DÒNG NÀY

            // Monster center is at (X + Width/2, Y + Height/2).
            float monsterCenterX = X + Width / 2;
            float monsterCenterY = Y + Height / 2;

            float dx = playerCenterX - monsterCenterX;
            float dy = playerCenterY - monsterCenterY;
            return (dx * dx) + (dy * dy);
        }

        public virtual void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return;
            if (game == null) return;
            // --- THÊM: Cập nhật Cooldown Tấn công ---
            if (attackCooldown > 0) attackCooldown--;

            // Tính khoảng cách (bình phương) đến người chơi
            // (Sử dụng tâm của hitbox nhân vật, giả sử hitbox là 10x10)
            float playerCenterX = playerX + 5;
            float playerCenterY = playerY + 5;
            // --- LỖI ĐÃ ĐƯỢC SỬA: Hàm GetDistanceToPlayer giờ đã chấp nhận (playerCenterX, playerCenterY) ---
            float distanceSq = GetDistanceToPlayer(playerCenterX, playerCenterY);

            // 1. XỬ LÝ TRẠNG THÁI (ƯU TIÊN CAO NHẤT)
            if (State == MonsterState.Hurt)
            {
                if (hurtAnim.IsFinished)
                {
                    State = MonsterState.Chase; // Chuyển sang truy đuổi ngay
                }
                return; // Không làm gì khác khi bị đau
            }

            if (State == MonsterState.Attack)
            {
                if (attackAnim.IsFinished)
                {
                    // --- THÊM: Gây sát thương KHI HOẠT ẢNH KẾT THÚC ---
                    // (Kiểm tra xem player còn trong tầm không)
                    float playerCenterX_now = playerX + 5;
                    float playerCenterY_now = playerY + 5;
                    if (GetDistanceToPlayer(playerCenterX_now, playerCenterY_now) <= attackRange * attackRange)
                    {
                        game.ApplyDamageToPlayer(this.attackDamage);
                    }
                    State = MonsterState.Chase; // Quay lại truy đuổi
                }
                return; // Không làm gì khác khi đang tấn công
            }

            if (State == MonsterState.Casting)
            {
                // (Dành riêng cho Boss)
                return;
            }

            // 2. KIỂM TRA TẦM NHÌN (LINE OF SIGHT - LOS)
            bool canSeePlayer = false;
            // Chỉ kiểm tra LOS nếu người chơi trong tầm truy đuổi (20 ô)
            if (distanceSq <= chaseRange * chaseRange)
            {
                canSeePlayer = game.HasLineOfSight(
                    new PointF(X + Width / 2, Y + Height / 2),
                    new PointF(playerCenterX, playerCenterY)
                );
            }

            // 3. QUYẾT ĐỊNH HÀNH ĐỘNG (FSM - Finite State Machine)

            // A. Nếu có thể thấy người chơi
            if (canSeePlayer)
            {
                // A1. Nếu trong tầm đánh (do lớp con (Orc) định nghĩa)
                if (distanceSq <= attackRange * attackRange && attackCooldown == 0)
                {
                    // --- SỬA: KÍCH HOẠT TẤN CÔNG VÀ GÂY SÁT THƯƠNG ---
                    // Chỉ kích hoạt nếu không đang tấn công
                    if (State != MonsterState.Attack)
                    {
                        State = MonsterState.Attack;
                        attackAnim.ResetFrame();
                        // GỌI HÀM GÂY SÁT THƯƠNG CHO PLAYER
                        //game.ApplyDamageToPlayer(this.attackDamage);
                        attackCooldown = attackCooldownDuration;
                    }
                }
                // --- SỬA LỖI AI: Dùng aggroRange (10 ô) thay vì perceptionRange (3 ô) ---
                // A2. Nếu trong tầm phát hiện (10 ô) hoặc đang truy đuổi
                else if (distanceSq <= aggroRange * aggroRange || State == MonsterState.Chase)
                {
                    //MoveTowards(game, patrolTarget, patrolSpeed);
                    State = MonsterState.Chase;
                }
                // A3. Nếu thấy người chơi nhưng ngoài tầm 10 ô và chưa truy đuổi
                else
                {
                    // Quay về tuần tra (nếu đang không làm gì)
                    if (State == MonsterState.Idle) State = MonsterState.Patrol;
                }
            }
            // B. Nếu không thể thấy người chơi (hoặc ngoài 20 ô)
            else
            {
                // Nếu đang truy đuổi, dừng lại và quay về tuần tra
                if (State == MonsterState.Chase)
                {
                    State = MonsterState.Patrol;
                }
            }

            // 4. XỬ LÝ TUẦN TRA (PATROL) VÀ ĐỨNG YÊN (IDLE)
            if (State == MonsterState.Idle)
            {
                patrolTimer++;
                if (patrolTimer > rand.Next(100, 200)) // Đứng yên 1-2 giây
                {
                    patrolTimer = 0;
                    State = MonsterState.Patrol;
                    SetNewPatrolTarget(game); // Tìm điểm tuần tra mới
                }
            }
            else if (State == MonsterState.Patrol)
            {
                //MoveTowards(game, patrolTarget, patrolSpeed);

                // Nếu đi gần đến mục tiêu
                if (GetDistanceToPoint(patrolTarget) < (frmMainGame.TILE_SIZE * frmMainGame.TILE_SIZE))
                {
                    State = MonsterState.Idle; // Đứng yên chờ
                }
            }
            // --- THÊM: LOGIC A* PATHFINDING & MOVEMENT ---
            if (_pathUpdateTimer > 0) _pathUpdateTimer--;

            // 1. Lấy đường đi mới nếu cần
            if (State == MonsterState.Chase)
            {
                // Lấy vị trí tile của quái và người chơi
                Point myTile = game.WorldToTile(new PointF(X + Width / 2, Y + Height / 2));
                Point playerTile = game.WorldToTile(new PointF(playerCenterX, playerCenterY));

                // Cần đường đi mới NẾU:
                // 1. Hết thời gian chờ (tránh việc 50 con quái tìm đường cùng lúc)
                // 2. Hoặc người chơi đã di chuyển sang ô tile khác
                if (_pathUpdateTimer <= 0 || playerTile != _lastPlayerTile)
                {
                    _currentPathTiles = game.FindPath(myTile, playerTile);
                    _lastPlayerTile = playerTile; // Lưu vị trí cuối
                    _pathUpdateTimer = 15; // Chờ 15 frame (khoảng 0.5s)
                }
            }
            else if (State == MonsterState.Patrol)
            {
                // Nếu đang tuần tra và không có đường đi (hoặc đã đi hết) -> Tìm đường mới
                if (_currentPathTiles.Count == 0)
                {
                    SetNewPatrolTarget(game);
                }
            }
            else
            {
                // Nếu Đứng yên, Tấn công, Bị thương... -> Xóa đường đi
                _currentPathTiles.Clear();
            }

            // 2. Di chuyển theo đường đi
            if (_currentPathTiles.Count > 0)
            {
                // Lấy node tiếp theo (đây là TILE)
                Point nextTile = _currentPathTiles[0];
                // Chuyển sang tọa độ Pixel (TÂM của TILE)
                PointF worldTarget = game.TileToWorld(nextTile);

                // Di chuyển đến TÂM của node đó
                float currentSpeed = (State == MonsterState.Chase) ? speed : patrolSpeed;
                MoveTowards(game, worldTarget, currentSpeed); // <-- Dùng lại hàm MoveTowards cũ

                // Kiểm tra xem đã đến gần node đó chưa
                if (GetDistanceToPoint(worldTarget) < (frmMainGame.TILE_SIZE / 2f) * (frmMainGame.TILE_SIZE / 2f))
                {
                    // Đến rồi -> Xóa node này khỏi danh sách và đi node tiếp theo
                    _currentPathTiles.RemoveAt(0);
                }
            }
        }

        // --- SỬA: Logic va chạm trượt (Sliding Collision) ---
        protected void MoveTowards(frmMainGame game, PointF target, float speed)
        {
            if (game == null) return;

            float dx = target.X - (X + Width / 2);
            float dy = target.Y - (Y + Height / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;

                float moveX = dx * speed;
                float moveY = dy * speed;

                // 1. Thử di chuyển X
                RectangleF nextHitboxX = new RectangleF(X + moveX, Y, Width, Height);
                // SỬA LỖI CS0176: Truy cập bằng tên kiểu
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

                // 2. Thử di chuyển Y
                RectangleF nextHitboxY = new RectangleF(X, Y + moveY, Width, Height);
                // SỬA LỖI CS0176: Truy cập bằng tên kiểu
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
            float dx = target.X - (X + Width / 2);
            float dy = target.Y - (Y + Height / 2);
            return (dx * dx) + (dy * dy);
        }

        // --- HÀM TẠO MỤC TIÊU PATROL KHÔNG KIỂM TRA VA CHẠM (chỉ dùng cho init) ---
        protected void SetNewPatrolTarget()
        {
            int range = 150;
            float targetX = patrolOrigin.X + rand.Next(-range, range);
            float targetY = patrolOrigin.Y + rand.Next(-range, range);
            patrolTarget = new PointF(targetX, targetY);
        }

        // --- HÀM TẠO MỤC TIÊU PATROL CÓ KIỂM TRA VA CHẠM ---
        protected void SetNewPatrolTarget(frmMainGame game)
        {
            if (game == null) return;
            // A* Pathfinding:
            // 1. Lấy vị trí tile hiện tại
            Point myTile = game.WorldToTile(new PointF(X + Width / 2, Y + Height / 2));
            // 2. Tìm 1 tile ngẫu nhiên có thể đi được
            Point randomTile = game.FindRandomWalkableTile();
            // 3. Tìm đường đi đến đó
            _currentPathTiles = game.FindPath(myTile, randomTile);
        }

        public virtual void TakeDamage(int damage)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return; // BẢO VỆ

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

            switch (State)
            {
                case MonsterState.Idle:
                case MonsterState.Friendly: // DÙNG IDLE CHO FRIENDLY
                    imageToDraw = idleAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Casting:
                    imageToDraw = castAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(facingDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    // LƯU Ý: DrawWidth/Height được cố định hoặc dựa trên kích thước Vẽ lớn hơn Hitbox.
                    // Vẫn cần dùng X, Y đã được SCALE
                    int drawWidth = Width;
                    int drawHeight = Height;
                    float drawX = X * scale - (drawWidth - Width) / 2; // Dùng float X
                    float drawY = Y * scale - (drawHeight - Height) / 2; // Dùng float Y
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            // VẼ THANH MÁU VÀ LABEL FRIENDLY
            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead && State != MonsterState.Friendly)
            {
                DrawHealthBar(canvas, scale);
            }
            else if (State == MonsterState.Friendly)
            {
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale - 10, Y * scale - 20);
            }
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


