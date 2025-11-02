using DAL.Models;
using Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// SỬA: Đổi namespace thành 'Main' để khớp với các lớp logic và form khác
namespace Main
{
    // LỚP GỐC: Chứa 99% logic game (Player, Quái vật, Chiến đấu, Hoạt ảnh)
    public abstract class GameMapBase
    {
        #region Biến Cốt Lõi

        // --- Tham chiếu ---
        protected frmMainGame _gameForm;
        protected PlayerCharacters _character;
        public PlayerSessionInventory CharacterInventory { get; private set; }

        // --- Trạng thái Game ---
        public DialogResult GameResult { get; protected set; } = DialogResult.None;
        public string MapName { get; protected set; } = "Endless Maze";
        public Size MapSize { get; protected set; } = new Size(3060, 1560); // SỬA: Kích thước map logic lớn (100x50 tiles @ 30px)

        // --- Player ---
        protected Image playerImage;
        protected float playerX = 100, playerY = 100;
        protected int playerWidth = 100, playerHeight = 100;
        public float PlayerCurrentHealth { get; set; }
        protected float playerMaxHealth;
        protected float playerStamina;
        protected float playerMaxStamina;

        // --- Hoạt ảnh Player ---
        protected AnimationActivity idleActivity, walkActivity, runActivity, attackActivity, walkAttackActivity, runAttackActivity, hurtActivity, deathActivity;

        // --- Input & Trạng thái Player ---
        protected bool goUp, goDown, goLeft, goRight, isRunning, isAttacking, isHurt, isDead;
        protected int hurtTimer = 0;
        protected bool canMove = true;
        protected string lastFacingDirection = "down";
        protected Point mousePos;

        // --- Dash Player ---
        protected int walkSpeed = 5, runSpeed = 8, dashSpeed = 15;
        protected bool isDashing = false;
        protected int dashTimer = 0, dashCooldown = 0;
        protected float dashDirectionX, dashDirectionY;

        // --- Quái vật ---
        protected List<Monster> monsters = new List<Monster>();
        protected Dictionary<string, MonsterAnimationSet> monsterAnimationSets = new Dictionary<string, MonsterAnimationSet>();
        protected Random rand = new Random();

        // --- Map & Collision ---
        protected int mapWidth; // Kích thước Form ClientSize.Width
        protected int mapHeight; // Kích thước Form ClientSize.Height
        protected Image floorTexture;
        protected Image wallTexture;
        protected int wallThickness = 50;
        protected int gapYPosition = 100;
        protected int gapSize = 150;
        protected List<RectangleF> _mazeWallRects = new List<RectangleF>(); // Tường mê cung
        protected const int TILE_SIZE = 30; // Kích thước mỗi ô mê cung (30px)
        protected const int MAZE_LOGIC_WIDTH = 100;
        protected const int MAZE_LOGIC_HEIGHT = 50;

        // --- Camera ---
        protected PointF _cameraOffset = new PointF(0, 0);

        // --- Cơ chế Map (Chung) ---
        protected Timer _specialMechanicTimer;
        protected bool _isPoisoned = false;
        protected int _dashCountSinceTouchGround = 0;

        public bool IsPlayerDead => isDead;

        #endregion

        #region Hàm Trừu tượng (Các map con PHẢI định nghĩa)

        /// <summary>
        /// Tải hình ảnh sàn và tường
        /// </summary>
        public abstract void LoadMapTextures();

        /// <summary>
        /// Khởi tạo các cơ chế đặc biệt (bẫy, timer, v.v.)
        /// </summary>
        public abstract void InitializeMapMechanics();

        /// <summary>
        /// Xử lý logic cơ chế map (chạy mỗi tick)
        /// </summary>
        public abstract void HandleMapMechanics();

        /// <summary>
        /// Vẽ các hiệu ứng/bẫy đặc biệt của map
        /// </summary>
        public abstract void DrawMapEffects(Graphics canvas);

        #endregion

        #region Khởi tạo (Chung)

        /// <summary>
        /// Khởi tạo Map (được gọi bởi frmMazeGame)
        /// </summary>
        public virtual void Initialize(frmMainGame form, PlayerCharacters character, int mapLevel, Size clientSize)
        {
            _gameForm = form;
            _character = character;
            CharacterInventory = _character.PlayerSessionInventory.FirstOrDefault();

            // Gán kích thước Form
            mapWidth = clientSize.Width;
            mapHeight = clientSize.Height;

            // TẠO MÊ CUNG LỚN
            GenerateMaze();

            // Khởi tạo Máu/Stamina
            playerMaxHealth = _character.BaseHealth;
            PlayerCurrentHealth = playerMaxHealth;
            playerMaxStamina = _character.BaseStamina;
            playerStamina = playerMaxStamina;

            // Tải Logic Chung
            LoadCharacterAnimations();
            LoadMonsterAnimations();

            // Tải Logic Map Cụ thể (được override)
            LoadMapTextures();
            InitializeMapMechanics(); // (Hàm này cũng sẽ đặt MapName)

            SpawnMonsters(5);

            // Đặt người chơi ở vị trí ngẫu nhiên trên sàn
            PointF spawnPos = FindValidSpawnPosition(100);
            playerX = spawnPos.X;
            playerY = spawnPos.Y;
        }

        protected virtual void GenerateMaze()
        {
            MazeGenerator mazeGen = new MazeGenerator(MAZE_LOGIC_WIDTH, MAZE_LOGIC_HEIGHT);
            mazeGen.GenerateMaze();
            int[,] mazeGrid = mazeGen.Maze;
            _mazeWallRects.Clear();

            // Duyệt lưới để tạo tường (rects)
            for (int y = 0; y < MAZE_LOGIC_HEIGHT; y++)
            {
                for (int x = 0; x < MAZE_LOGIC_WIDTH; x++)
                {
                    if (mazeGrid[y, x] == (int)GameObjectType.Wall)
                    {
                        // Kích thước ô mê cung là TILE_SIZE x TILE_SIZE (30x30)
                        _mazeWallRects.Add(new RectangleF(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                    }
                }
            }

            // THÊM: Đặt cửa ra ở góc trên bên trái của mê cung logic
            // (Vị trí 1, 1 của lưới logic)
            // Chúng ta không cần cửa ra ở mép phải Form nữa, chỉ cần cửa ra trong mê cung.
        }

        protected virtual PointF FindValidSpawnPosition(float minDistanceToWall)
        {
            int maxAttempts = 100;
            float targetX, targetY;

            // Đặt người chơi ở góc dưới bên phải của mê cung logic (MAZE_LOGIC_WIDTH-2, MAZE_LOGIC_HEIGHT-2)
            targetX = (MAZE_LOGIC_WIDTH - 2) * TILE_SIZE;
            targetY = (MAZE_LOGIC_HEIGHT - 2) * TILE_SIZE;

            // Vị trí mặc định này là hợp lệ, trừ khi mê cung bị chặn hoàn toàn.
            return new PointF(targetX, targetY);
        }

        // (Tất cả các hàm Load... và Spawn... được copy y hệt từ v7)
        private void LoadCharacterAnimations()
        {
            try
            {
                string playerRoot = Path.Combine("ImgSource", "Char", "Player", "SwordMan");
                idleActivity = new AnimationActivity(10);
                idleActivity.LoadImages(Path.Combine(playerRoot, "Stand", "Back"), Path.Combine(playerRoot, "Stand", "Front"), Path.Combine(playerRoot, "Stand", "Left"), Path.Combine(playerRoot, "Stand", "Right"));
                walkActivity = new AnimationActivity(6);
                walkActivity.LoadImages(Path.Combine(playerRoot, "Walk", "Back"), Path.Combine(playerRoot, "Walk", "Front"), Path.Combine(playerRoot, "Walk", "Left"), Path.Combine(playerRoot, "Walk", "Right"));
                runActivity = new AnimationActivity(4);
                runActivity.LoadImages(Path.Combine(playerRoot, "Run", "Back"), Path.Combine(playerRoot, "Run", "Front"), Path.Combine(playerRoot, "Run", "Left"), Path.Combine(playerRoot, "Run", "Right"));
                attackActivity = new AnimationActivity(3) { IsLooping = false };
                attackActivity.LoadImages(Path.Combine(playerRoot, "Atk", "Back"), Path.Combine(playerRoot, "Atk", "Front"), Path.Combine(playerRoot, "Atk", "Left"), Path.Combine(playerRoot, "Atk", "Right"));
                walkAttackActivity = new AnimationActivity(3) { IsLooping = false };
                walkAttackActivity.LoadImages(Path.Combine(playerRoot, "Walk_Atk", "Back"), Path.Combine(playerRoot, "Walk_Atk", "Front"), Path.Combine(playerRoot, "Walk_Atk", "Left"), Path.Combine(playerRoot, "Walk_Atk", "Right"));
                runAttackActivity = new AnimationActivity(3) { IsLooping = false };
                runAttackActivity.LoadImages(Path.Combine(playerRoot, "Run_Atk", "Back"), Path.Combine(playerRoot, "Run_Atk", "Front"), Path.Combine(playerRoot, "Run_Atk", "Left"), Path.Combine(playerRoot, "Run_Atk", "Right"));
                hurtActivity = new AnimationActivity(5) { IsLooping = false };
                hurtActivity.LoadImages(Path.Combine(playerRoot, "Hurt"), Path.Combine(playerRoot, "Hurt"), Path.Combine(playerRoot, "Hurt"), Path.Combine(playerRoot, "Hurt"));
                deathActivity = new AnimationActivity(5) { IsLooping = false };
                deathActivity.LoadImages(Path.Combine(playerRoot, "Death"), Path.Combine(playerRoot, "Death"), Path.Combine(playerRoot, "Death"), Path.Combine(playerRoot, "Death"));
                playerImage = idleActivity.GetDefaultFrame("down");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải hoạt ảnh Player: " + ex.Message);
            }
        }
        private void LoadMonsterAnimations()
        {
            try
            {
                string slimeRoot = Path.Combine("ImgSource", "Char", "Monster", "Slime", "Slime_Flame");
                MonsterAnimationSet slimeAnims = new MonsterAnimationSet();
                slimeAnims.Idle = new AnimationActivity(10);
                slimeAnims.Idle.LoadImages(Path.Combine(slimeRoot, "Stand"), Path.Combine(slimeRoot, "Stand"), Path.Combine(slimeRoot, "Stand"), Path.Combine(slimeRoot, "Stand"));
                slimeAnims.Walk = new AnimationActivity(7);
                slimeAnims.Walk.LoadImages(Path.Combine(slimeRoot, "Walk"), Path.Combine(slimeRoot, "Walk"), Path.Combine(slimeRoot, "Walk"), Path.Combine(slimeRoot, "Walk"));
                slimeAnims.Attack = new AnimationActivity(5) { IsLooping = false };
                slimeAnims.Attack.LoadImages(Path.Combine(slimeRoot, "Atk", "Back"), Path.Combine(slimeRoot, "Atk", "Front"), Path.Combine(slimeRoot, "Atk", "Left"), Path.Combine(slimeRoot, "Atk", "Right"));
                slimeAnims.Hurt = new AnimationActivity(4) { IsLooping = false };
                slimeAnims.Hurt.LoadImages(Path.Combine(slimeRoot, "Hurt"), Path.Combine(slimeRoot, "Hurt"), Path.Combine(slimeRoot, "Hurt"), Path.Combine(slimeRoot, "Hurt"));
                slimeAnims.Death = new AnimationActivity(6) { IsLooping = false };
                slimeAnims.Death.LoadImages(Path.Combine(slimeRoot, "Death"), Path.Combine(slimeRoot, "Death"), Path.Combine(slimeRoot, "Death"), Path.Combine(slimeRoot, "Death"));
                monsterAnimationSets.Add("Slime_Flame", slimeAnims);

                string orcRoot = Path.Combine("ImgSource", "Char", "Monster", "Orc", "Orc_Flame");
                MonsterAnimationSet orcAnims = new MonsterAnimationSet();
                orcAnims.Idle = new AnimationActivity(8);
                orcAnims.Idle.LoadImages(Path.Combine(orcRoot, "Stand"), Path.Combine(orcRoot, "Stand"), Path.Combine(orcRoot, "Stand"), Path.Combine(orcRoot, "Stand"));
                orcAnims.Walk = new AnimationActivity(6);
                orcAnims.Walk.LoadImages(Path.Combine(orcRoot, "Walk"), Path.Combine(orcRoot, "Walk"), Path.Combine(orcRoot, "Walk"), Path.Combine(orcRoot, "Walk"));
                orcAnims.Attack = new AnimationActivity(4) { IsLooping = false };
                orcAnims.Attack.LoadImages(Path.Combine(orcRoot, "Attack"), Path.Combine(orcRoot, "Attack"), Path.Combine(orcRoot, "Attack"), Path.Combine(orcRoot, "Attack"));
                orcAnims.Hurt = new AnimationActivity(3) { IsLooping = false };
                orcAnims.Hurt.LoadImages(Path.Combine(orcRoot, "Hurt"), Path.Combine(orcRoot, "Hurt"), Path.Combine(orcRoot, "Hurt"), Path.Combine(orcRoot, "Hurt"));
                orcAnims.Death = new AnimationActivity(5) { IsLooping = false };
                orcAnims.Death.LoadImages(Path.Combine(orcRoot, "Death"), Path.Combine(orcRoot, "Death"), Path.Combine(orcRoot, "Death"), Path.Combine(orcRoot, "Death"));
                monsterAnimationSets.Add("Orc_Flame", orcAnims);

                string bossRoot = Path.Combine("ImgSource", "Char", "Monster", "Boss");
                MonsterAnimationSet bossAnims = new MonsterAnimationSet();
                bossAnims.Idle = new AnimationActivity(10);
                bossAnims.Idle.LoadImages(Path.Combine(bossRoot, "Stand"), Path.Combine(bossRoot, "Stand"), Path.Combine(bossRoot, "Stand"), Path.Combine(bossRoot, "Stand"));
                bossAnims.Walk = new AnimationActivity(8);
                bossAnims.Walk.LoadImages(Path.Combine(bossRoot, "Walk"), Path.Combine(bossRoot, "Walk"), Path.Combine(bossRoot, "Walk"), Path.Combine(bossRoot, "Walk"));
                bossAnims.Attack = new AnimationActivity(5) { IsLooping = false };
                bossAnims.Attack.LoadImages(Path.Combine(bossRoot, "Attack"), Path.Combine(bossRoot, "Attack"), Path.Combine(bossRoot, "Attack"), Path.Combine(bossRoot, "Attack"));
                bossAnims.Hurt = new AnimationActivity(4) { IsLooping = false };
                bossAnims.Hurt.LoadImages(Path.Combine(bossRoot, "Hurt"), Path.Combine(bossRoot, "Hurt"), Path.Combine(bossRoot, "Hurt"), Path.Combine(bossRoot, "Hurt"));
                bossAnims.Death = new AnimationActivity(6) { IsLooping = false };
                bossAnims.Death.LoadImages(Path.Combine(bossRoot, "Death"), Path.Combine(bossRoot, "Death"), Path.Combine(bossRoot, "Death"), Path.Combine(bossRoot, "Death"));
                monsterAnimationSets.Add("Boss", bossAnims);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải hoạt ảnh Quái vật: " + ex.Message +
                    "\n(Hãy đảm bảo bạn đã đặt 'Copy to Output Directory' cho thư mục ImgSource)");
            }
        }
        private void SpawnMonsters(int count)
        {
            string[] monsterTypes = { "Slime_Flame", "Orc_Flame", "Boss" };
            for (int i = 0; i < count; i++)
            {
                string type = monsterTypes[rand.Next(monsterTypes.Length)];
                MonsterAnimationSet anims;
                if (!monsterAnimationSets.TryGetValue(type, out anims)) continue;

                PointF spawnPos = FindValidSpawnPosition(100);
                monsters.Add(new Monster(this, spawnPos.X, spawnPos.Y, type, anims));
            }
        }

        #endregion

        #region Vòng lặp (Chung)

        /// <summary>
        /// Vòng lặp Update (được gọi bởi Timer của Form)
        /// </summary>
        public virtual void Update()
        {
            UpdateFacingDirection();
            UpdatePlayerStates();
            UpdateDashState();
            UpdateMovement();
            UpdateAnimation();
            UpdateMonsters();
            HandleMapMechanics(); // Gọi hàm logic map (đã override)

            // Kiểm tra Game Over
            if (isDead && deathActivity.IsFinished)
            {
                GameResult = DialogResult.Cancel; // Báo hiệu thua
            }
        }

        /// <summary>
        /// Vòng lặp Vẽ (được gọi bởi Paint của Form)
        /// </summary>
        public virtual void Draw(Graphics canvas)
        {
            GraphicsState gState = canvas.Save(); // LƯU trạng thái ban đầu (để UI không bị dịch chuyển)

            // --- XỬ LÝ CAMERA ---
            // 1. Tính toán vị trí camera (đặt Player ở giữa màn hình)
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            // Vị trí camera mong muốn (đặt Player ở tâm)
            float camX = playerX + playerWidth / 2f - halfWidth;
            float camY = playerY + playerHeight / 2f - halfHeight;

            // 2. Kẹp (Clamp) camera để không ra ngoài Map Logic
            float mapLogicalWidth = MAZE_LOGIC_WIDTH * TILE_SIZE;
            float mapLogicalHeight = MAZE_LOGIC_HEIGHT * TILE_SIZE;

            // Clamp X (đảm bảo cam không hiển thị khoảng trống ở trái/phải map)
            if (mapLogicalWidth > mapWidth)
                camX = Math.Max(0, Math.Min(camX, mapLogicalWidth - mapWidth));
            else
                camX = 0; // Nếu map nhỏ hơn cửa sổ, không cần dịch chuyển

            // Clamp Y
            if (mapLogicalHeight > mapHeight)
                camY = Math.Max(0, Math.Min(camY, mapLogicalHeight - mapHeight));
            else
                camY = 0; // Nếu map nhỏ hơn cửa sổ, không cần dịch chuyển

            // 3. Cập nhật Offset Camera (Offset là dịch chuyển ngược lại)
            _cameraOffset = new PointF(-camX, -camY);

            // 4. Dịch chuyển toàn bộ Canvas (THẾ GIỚI)
            canvas.TranslateTransform(_cameraOffset.X, _cameraOffset.Y);

            // --- KẾT THÚC XỬ LÝ CAMERA ---

            // VẼ THẾ GIỚI
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;

            // 1. Vẽ Map (Sàn, Tường) và Hiệu ứng (do lớp con override)
            DrawMapEffects(canvas);

            // 2. Vẽ Quái vật
            foreach (var monster in monsters)
            {
                monster.Draw(canvas);
            }

            // 3. Vẽ Player
            if (playerImage != null)
            {
                canvas.DrawImage(playerImage, (int)playerX, (int)playerY, playerWidth, playerHeight);
            }

            // --- KHÔI PHỤC TRẠNG THÁI (ĐỂ VẼ UI) ---
            canvas.Restore(gState);

            // 4. Vẽ UI (Không bị dịch chuyển)
            DrawUI(canvas);
        }

        #endregion

        #region Input Handlers (Chung)

        public virtual void HandleKeyDown(KeyEventArgs e)
        {
            if (isDead) return;
            if (e.KeyCode == Keys.W) goUp = true;
            if (e.KeyCode == Keys.S) goDown = true;
            if (e.KeyCode == Keys.A) goLeft = true;
            if (e.KeyCode == Keys.D) goRight = true;
            if (e.KeyCode == Keys.ControlKey) isRunning = true;

            if (e.KeyCode == Keys.ShiftKey && !isDashing && dashCooldown == 0 && playerStamina >= 20)
            {
                playerStamina -= 20;
                isDashing = true;
                dashTimer = 10;
                dashCooldown = 60;
                _dashCountSinceTouchGround++;
                GetDashDirection();
            }
            if (e.KeyCode == Keys.Space)
            {
                PerformAttack();
            }
        }
        public virtual void HandleKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) goUp = false;
            if (e.KeyCode == Keys.S) goDown = false;
            if (e.KeyCode == Keys.A) goLeft = false;
            if (e.KeyCode == Keys.D) goRight = false;
            if (e.KeyCode == Keys.ControlKey) isRunning = false;
        }
        public virtual void HandleMouseDown(MouseEventArgs e)
        {
            if (isDead || isAttacking || isDashing) return;
            if (e.Button == MouseButtons.Left)
            {
                PerformAttack();
            }
        }
        public virtual void HandleMouseMove(MouseEventArgs e)
        {
            // --- SỬA LỖI INPUT CHUỘT ---
            // Input chuột cần được dịch chuyển NGƯỢC lại camera để tìm đúng hướng nhìn
            float camX = -_cameraOffset.X;
            float camY = -_cameraOffset.Y;
            mousePos = new Point((int)(e.Location.X + camX), (int)(e.Location.Y + camY));
        }

        #endregion

        #region Logic Game (Chung)

        // (Toàn bộ logic game chung được copy từ v7)
        protected virtual void UpdateMonsters()
        {
            RectangleF playerHitbox = GetPlayerHitbox();
            for (int i = monsters.Count - 1; i >= 0; i--)
            {
                monsters[i].Update(playerHitbox);
                if (monsters[i].IsDead && monsters[i].CanBeRemoved)
                {
                    monsters.RemoveAt(i);
                }
            }
        }
        protected virtual void UpdateFacingDirection()
        {
            if (isDashing) return;
            // SỬA: Đã dịch chuyển chuột trong HandleMouseMove, dùng mousePos bình thường
            float deltaX = mousePos.X - (playerX + playerWidth / 2);
            float deltaY = mousePos.Y - (playerY + playerHeight / 2);
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                lastFacingDirection = (deltaX > 0) ? "right" : "left";
            }
            else
            {
                lastFacingDirection = (deltaY > 0) ? "down" : "up";
            }
        }
        protected virtual void UpdatePlayerStates()
        {
            if (hurtTimer > 0)
            {
                hurtTimer--;
                if (hurtTimer == 0) isHurt = false;
            }
            if (PlayerCurrentHealth <= 0 && !isDead)
            {
                isDead = true;
                isHurt = false;
                canMove = false;
                deathActivity.ResetFrame();
            }
            bool isBusy = isRunning || isDashing || isAttacking;
            bool isMoving = goUp || goDown || goLeft || goRight;
            if (!isBusy && !isMoving && playerStamina < playerMaxStamina)
            {
                playerStamina = Math.Min(playerMaxStamina, playerStamina + 0.5f);
            }
            if (!isMoving && !isDashing)
            {
                _dashCountSinceTouchGround = 0;
            }
        }
        public virtual void PlayerTakesDamage(float damage)
        {
            if (isDead || isHurt || isDashing) return;
            PlayerCurrentHealth -= damage;
            isHurt = true;
            hurtTimer = 20;
            hurtActivity.ResetFrame();
        }
        public virtual RectangleF GetPlayerHitbox()
        {
            return new RectangleF(playerX + 20, playerY + 20, playerWidth - 40, playerHeight - 40);
        }
        protected virtual void GetDashDirection()
        {
            dashDirectionX = 0;
            dashDirectionY = 0;
            if (goUp) dashDirectionY = -1;
            if (goDown) dashDirectionY = 1;
            if (goLeft) dashDirectionX = -1;
            if (goRight) dashDirectionX = 1;
            if (dashDirectionX == 0 && dashDirectionY == 0)
            {
                if (lastFacingDirection == "up") dashDirectionY = -1;
                if (lastFacingDirection == "down") dashDirectionY = 1;
                if (lastFacingDirection == "left") dashDirectionX = -1;
                if (lastFacingDirection == "right") dashDirectionX = 1;
            }
        }
        protected virtual void UpdateDashState()
        {
            if (dashCooldown > 0) dashCooldown--;
            if (isDashing)
            {
                dashTimer--;
                if (dashTimer <= 0) isDashing = false;
            }
        }
        protected virtual void UpdateMovement()
        {
            float dx = 0, dy = 0;
            if (isDashing)
            {
                dx = dashDirectionX * dashSpeed;
                dy = dashDirectionY * dashSpeed;
            }
            else
            {
                bool isMoving = goUp || goDown || goLeft || goRight;
                canMove = !isDead;
                if (isAttacking && !isMoving) canMove = false;
                if (isHurt) canMove = false;

                if (canMove && isMoving)
                {
                    float moveX = 0, moveY = 0;
                    if (goLeft) moveX = -1;
                    if (goRight) moveX = 1;
                    if (goUp) moveY = -1;
                    if (goDown) moveY = 1;

                    if (moveX != 0 && moveY != 0)
                    {
                        float factor = 0.7071f;
                        moveX *= factor;
                        moveY *= factor;
                    }
                    int currentSpeed = walkSpeed;
                    if (isRunning && playerStamina > 0)
                    {
                        currentSpeed = runSpeed;
                        playerStamina -= 0.5f;
                    }
                    dx = moveX * currentSpeed;
                    dy = moveY * currentSpeed;
                }
            }
            float nextX = playerX + dx;
            if (!IsCollidingWithWall(nextX, playerY)) playerX = nextX;
            float nextY = playerY + dy;
            if (!IsCollidingWithWall(playerX, nextY)) playerY = nextY;
        }

        /// <summary>
        /// Kiểm tra va chạm với TƯỜNG MÊ CUNG (hoặc 4 cạnh)
        /// </summary>
        protected virtual bool IsCollidingWithWall(float futureX, float futureY)
        {
            RectangleF playerHitbox = GetPlayerHitbox();
            playerHitbox.X = futureX + 20; // Áp dụng vị trí mới
            playerHitbox.Y = futureY + 20;

            // 1. Va chạm với CÁC TƯỜNG MÊ CUNG BÊN TRONG
            foreach (var wall in _mazeWallRects)
            {
                if (playerHitbox.IntersectsWith(wall))
                {
                    return true;
                }
            }

            // 2. Va chạm với CÁC CẠNH BAO VÀ CỬA RA (Góc 0, 0, 100x50 tiles)
            float mapLogicalWidth = MAZE_LOGIC_WIDTH * TILE_SIZE;
            float mapLogicalHeight = MAZE_LOGIC_HEIGHT * TILE_SIZE;

            // Va chạm tường Trái (x=0)
            if (playerHitbox.Left < 0) return true;
            // Va chạm tường Trên (y=0)
            if (playerHitbox.Top < 0) return true;
            // Va chạm tường Dưới (y=mapLogicalHeight)
            if (playerHitbox.Bottom > mapLogicalHeight) return true;

            // Va chạm tường Phải (CỬA RA)
            if (playerHitbox.Right > mapLogicalWidth)
            {
                // Giả định cửa ra nằm ở (1, 1) logic tile, nhưng mê cung đã tự tạo lối đi.
                // Chúng ta sẽ giả định cửa ra là TẤT CẢ các ô ở mép trên bên trái.
                // Nếu người chơi đi qua ô (TILE_SIZE, TILE_SIZE) đầu tiên
                if (playerX < TILE_SIZE * 2 && playerY < TILE_SIZE * 2)
                {
                    HandleMapExit();
                    return false;
                }
                return true; // Nếu va chạm mép phải ngoài cùng
            }

            return false;
        }

        protected virtual void HandleMapExit()
        {
            GameResult = DialogResult.OK; // Báo hiệu thắng
        }
        protected virtual void UpdateAnimation()
        {
            bool isMoving = goUp || goDown || goLeft || goRight;
            string direction = lastFacingDirection;
            if (isDead)
            {
                playerImage = deathActivity.GetNextFrame(direction);
                return;
            }
            if (isHurt)
            {
                playerImage = hurtActivity.GetNextFrame(direction);
                return;
            }
            if (isDashing)
            {
                playerImage = runActivity.GetNextFrame(direction);
                // SỬA: Đặt lại mousePos để tránh dịch chuyển không cần thiết sau dash
                mousePos.X = (int)playerX + playerWidth / 2;
                mousePos.Y = (int)playerY + playerHeight / 2;
            }
            else if (isAttacking)
            {
                if (isRunning && isMoving)
                {
                    playerImage = runAttackActivity.GetNextFrame(direction);
                    if (runAttackActivity.IsFinished) isAttacking = false;
                }
                else if (isMoving)
                {
                    playerImage = walkAttackActivity.GetNextFrame(direction);
                    if (walkAttackActivity.IsFinished) isAttacking = false;
                }
                else
                {
                    playerImage = attackActivity.GetNextFrame(direction);
                    if (attackActivity.IsFinished) isAttacking = false;
                }
            }
            else if (isMoving)
            {
                if (isRunning)
                {
                    playerImage = runActivity.GetNextFrame(direction);
                }
                else
                {
                    playerImage = walkActivity.GetNextFrame(direction);
                }
            }
            else
            {
                playerImage = idleActivity.GetNextFrame(direction);
            }
        }
        protected virtual void PerformAttack()
        {
            if (isDead || isAttacking || isDashing) return;
            isAttacking = true;
            attackActivity.ResetFrame();
            walkAttackActivity.ResetFrame();
            runAttackActivity.ResetFrame();
            float attackRange = 50;
            RectangleF attackHitbox = GetPlayerHitbox();
            if (lastFacingDirection == "up") attackHitbox.Y -= attackRange;
            if (lastFacingDirection == "down") attackHitbox.Y += attackRange;
            if (lastFacingDirection == "left") attackHitbox.X -= attackRange;
            if (lastFacingDirection == "right") attackHitbox.X += attackRange;
            foreach (var monster in monsters)
            {
                if (!monster.IsDead && attackHitbox.IntersectsWith(monster.Hitbox))
                {
                    monster.TakeDamage(10);
                }
            }
        }
        protected virtual void DrawUI(Graphics g)
        {
            using (Font uiFont = new Font("Arial", 10, FontStyle.Bold))
            {
                g.FillRectangle(Brushes.DarkRed, 10, 10, 200, 20);
                float hpPercent = (PlayerCurrentHealth > 0) ? PlayerCurrentHealth / playerMaxHealth : 0;
                // --- SỬA LỖI CS0266: Thêm (int) ---
                g.FillRectangle(Brushes.Red, 10, 10, (int)(200 * hpPercent), 20);
                g.DrawString($"HP: {(int)PlayerCurrentHealth} / {(int)playerMaxHealth}", uiFont, Brushes.White, 15, 12);

                g.FillRectangle(Brushes.DarkGreen, 10, 35, 150, 15);
                float staminaPercent = (playerStamina > 0) ? playerStamina / playerMaxStamina : 0;
                // --- SỬA LỖI CS0266: Thêm (int) ---
                g.FillRectangle(Brushes.Green, 10, 35, (int)(150 * staminaPercent), 15);
                g.DrawString($"Stamina: {(int)playerStamina} / {(int)playerMaxStamina}", uiFont, Brushes.White, 15, 36);

                // (Hiệu ứng map được vẽ bởi các lớp con)
            }
        }
        protected virtual RectangleF GetRandomFloorRect(int rectWidth, int rectHeight)
        {
            float spawnX = rand.Next(wallThickness, (int)(MAZE_LOGIC_WIDTH * TILE_SIZE) - wallThickness - rectWidth);
            float spawnY = rand.Next(wallThickness, (int)(MAZE_LOGIC_HEIGHT * TILE_SIZE) - wallThickness - rectHeight);
            return new RectangleF(spawnX, spawnY, rectWidth, rectHeight);
        }
        protected virtual Image LoadImageSafe(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"Texture không tìm thấy: {path}");
                    return null;
                }
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;
                        return Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải ảnh {path}: {ex.Message}");
                return null;
            }
        }
        protected virtual async void FallOver(int delayMs)
        {
            if (isHurt || isDead) return;
            isHurt = true;
            hurtTimer = 30; // 0.5s @ 60fps
            hurtActivity.ResetFrame();
            isDashing = false;
            _dashCountSinceTouchGround = 0;
            await System.Threading.Tasks.Task.Delay(delayMs);
        }

        // Dừng/Chạy timer của map
        public virtual void PauseTimers(bool isPaused)
        {
            if (isPaused)
                _specialMechanicTimer?.Stop();
            else
                _specialMechanicTimer?.Start();
        }

        // Cần cho AnimationActivity
        public Image LoadImageSafe_FromActivity(string path)
        {
            return LoadImageSafe(path);
        }

        #endregion

        #region Lớp Con (Monster)

        // (Lớp Monster y hệt v7)
        protected class Monster
        {
            private enum State { Idle, Chase, Attack, Hurt, Dead }
            private State currentState = State.Idle;
            private GameMapBase gameMap; // SỬA: Dùng lớp Gốc
            private string monsterType;
            public float x, y, width, height;
            public RectangleF Hitbox => new RectangleF(x + 20, y + 20, width - 40, height - 40);
            public float health, maxHealth;
            private float moveSpeed, attackRange, detectionRange;
            private int attackDamage;
            private MonsterAnimationSet anims;
            private Image currentImage;
            private string facingDirection = "down";
            private int hurtTimer = 0;
            private int deathTimer = 100;
            public bool CanBeRemoved { get; private set; } = false;
            public bool IsDead => currentState == State.Dead;

            public Monster(GameMapBase form, float startX, float startY, string type, MonsterAnimationSet animationSet) // SỬA: Dùng lớp Gốc
            {
                gameMap = form;
                x = startX;
                y = startY;
                monsterType = type;
                anims = animationSet;
                SetStatsByType(type);
                currentImage = anims.Idle.GetDefaultFrame("down");
            }

            private void SetStatsByType(string type)
            {
                switch (type)
                {
                    case "Slime_Flame":
                        width = 80; height = 80; maxHealth = 30; health = 30; moveSpeed = 1.5f; attackRange = 60f; detectionRange = 250f; attackDamage = 5;
                        break;
                    case "Orc_Flame":
                        width = 100; height = 100; maxHealth = 60; health = 60; moveSpeed = 1.0f; attackRange = 80f; detectionRange = 350f; attackDamage = 10;
                        break;
                    case "Boss":
                        width = 150; height = 150; maxHealth = 200; health = 200; moveSpeed = 0.8f; attackRange = 100f; detectionRange = 500f; attackDamage = 20;
                        break;
                }
            }

            public void Update(RectangleF playerHitbox)
            {
                if (CanBeRemoved) return;
                if (currentState == State.Dead)
                {
                    deathTimer--;
                    if (deathTimer <= 0) CanBeRemoved = true;
                    UpdateAnimation();
                    return;
                }
                if (currentState == State.Hurt)
                {
                    hurtTimer--;
                    if (hurtTimer <= 0) currentState = State.Idle;
                    UpdateAnimation();
                    return;
                }

                float playerCenterX = playerHitbox.X + playerHitbox.Width / 2;
                float playerCenterY = playerHitbox.Y + playerHitbox.Height / 2;
                float monsterCenterX = x + width / 2;
                float monsterCenterY = y + height / 2;
                float deltaX = playerCenterX - monsterCenterX;
                float deltaY = playerCenterY - monsterCenterY;
                float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                if (Math.Abs(deltaX) > Math.Abs(deltaY))
                    facingDirection = (deltaX > 0) ? "right" : "left";
                else
                    facingDirection = (deltaY > 0) ? "down" : "up";

                if (currentState == State.Attack)
                {
                    if (anims.Attack.IsFinished) currentState = State.Idle;
                }
                else if (distance <= attackRange)
                {
                    currentState = State.Attack;
                    anims.Attack.ResetFrame();
                    if (playerHitbox.IntersectsWith(this.Hitbox))
                    {
                        gameMap.PlayerTakesDamage(attackDamage); // SỬA: Dùng lớp Gốc
                    }
                }
                else if (distance <= detectionRange)
                {
                    currentState = State.Chase;
                    float moveX = (deltaX / distance) * moveSpeed;
                    float moveY = (deltaY / distance) * moveSpeed;
                    if (!gameMap.IsCollidingWithWall(x + moveX, y)) x += moveX; // SỬA: Dùng lớp Gốc
                    if (!gameMap.IsCollidingWithWall(x, y + moveY)) y += moveY; // SỬA: Dùng lớp Gốc
                }
                else
                {
                    currentState = State.Idle;
                }
                UpdateAnimation();
            }

            private void UpdateAnimation()
            {
                switch (currentState)
                {
                    case State.Idle: currentImage = anims.Idle.GetNextFrame(facingDirection); break;
                    case State.Chase: currentImage = anims.Walk.GetNextFrame(facingDirection); break;
                    case State.Attack: currentImage = anims.Attack.GetNextFrame(facingDirection); break;
                    case State.Hurt: currentImage = anims.Hurt.GetNextFrame(facingDirection); break;
                    case State.Dead: currentImage = anims.Death.GetNextFrame(facingDirection); break;
                }
            }

            public void Draw(Graphics canvas)
            {
                if (currentImage == null) return;
                if (IsDead && anims.Death.IsFinished) return;
                canvas.DrawImage(currentImage, (int)x, (int)y, width, height);
                if (IsDead) return;
                float healthPercent = (health > 0) ? health / maxHealth : 0;
                float barWidth = width - 20, barHeight = 5;
                float barX = x + 10, barY = y - 10;
                // SỬA: Ép kiểu tất cả các tham số FillRectangle sang float
                canvas.FillRectangle(Brushes.DarkRed, barX, barY, (float)barWidth, (float)barHeight);
                canvas.FillRectangle(Brushes.Red, barX, barY, (float)(barWidth * healthPercent), (float)barHeight);
            }

            public void TakeDamage(int damage)
            {
                if (currentState == State.Hurt || IsDead) return;
                health -= damage;
                if (health <= 0)
                {
                    currentState = State.Dead;
                    anims.Death.ResetFrame();
                }
                else
                {
                    currentState = State.Hurt;
                    hurtTimer = 15;
                    anims.Hurt.ResetFrame();
                }
            }
        }

        protected struct MonsterAnimationSet
        {
            public AnimationActivity Idle;
            public AnimationActivity Walk;
            public AnimationActivity Attack;
            public AnimationActivity Hurt;
            public AnimationActivity Death;
        }

        #endregion
    }
}
