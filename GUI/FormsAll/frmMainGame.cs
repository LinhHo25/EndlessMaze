using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Services;
using DAL.Models;
using static BLL.Services.GameSessionService;
using GUI.GameEntities; // <-- Thêm using để tìm thấy Monster, Boss, SpellEffect...

namespace Main
{
    // --- LƯU Ý: MazeGenerator và GameObjectType cần được định nghĩa trong file MazeGeneration.cs của bạn ---

    public partial class frmMainGame : Form
    {
        private readonly DAL.Models.PlayerCharacters _character;
        private readonly int _mapLevel;

        // --- HẰNG SỐ MÊ CUNG VÀ GAME ---
        public const int TILE_SIZE = 30;
        public const int MAZE_LOGIC_WIDTH = 51;
        public const int MAZE_LOGIC_HEIGHT = 31;
        public const float REVEAL_RADIUS = 7.0f;
        public const float BASE_SLIDE_TOLERANCE = 4.0f;
        public const int RENDER_SCALE = 3;

        // Kích thước Map logic sau khi nhân tỷ lệ
        private int _mapLogicalWidthScaled => MAZE_LOGIC_WIDTH * TILE_SIZE * RENDER_SCALE;
        private int _mapLogicalHeightScaled => MAZE_LOGIC_HEIGHT * TILE_SIZE * RENDER_SCALE;

        // --- Biến Camera, Fog of War, và Logic Game ---
        private float _cameraX = 0;
        private float _cameraY = 0;
        private bool[,] _isTileVisible = new bool[MAZE_LOGIC_HEIGHT, MAZE_LOGIC_WIDTH];

        public int _monstersKilled = 0;
        public bool _isBossDefeated = false;
        private bool _isExiting = false;

        // --- Dịch vụ BLL và Chỉ số Game ---
        private GameSessionService _gameSessionService;
        private DAL.Models.PlayerSessionInventory _inventory;
        private BLL.Services.GameSessionService.CalculatedStats _calculatedStats;

        private int _currentHealth;
        private int _currentStamina;
        public int _chestsOpened = 0; // **MERGE: Thêm từ file 1**

        // Fonts và Brushes cho HUD
        private Font _hudFont = new Font("Segoe UI", 10, FontStyle.Bold);
        private Font _hudTitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
        private SolidBrush _hudBrush = new SolidBrush(Color.White);
        private SolidBrush _hudBackgroundBrush = new SolidBrush(Color.FromArgb(170, 0, 0, 0));
        private SolidBrush _healthBrush = new SolidBrush(Color.FromArgb(220, 50, 50));
        private SolidBrush _staminaBrush = new SolidBrush(Color.FromArgb(50, 220, 50));
        private SolidBrush _barBackgroundBrush = new SolidBrush(Color.FromArgb(100, 50, 50, 50));

        // --- Biến Player ---
        private Image playerImage;
        private float playerX = 1.5f * TILE_SIZE;
        private float playerY = 1.5f * TILE_SIZE;
        public int playerWidth = 10; // **PUBLIC: Để Monster.cs có thể truy cập (nếu cần)**
        public int playerHeight = 10; // **PUBLIC: Để Monster.cs có thể truy cập (nếu cần)**

        // Hoạt ảnh Player
        private AnimationActivity idleActivity;
        private AnimationActivity walkActivity;
        private AnimationActivity runActivity;
        private AnimationActivity attackActivity;
        private AnimationActivity walkAttackActivity;
        private AnimationActivity runAttackActivity;
        private AnimationActivity hurtActivity;
        private AnimationActivity deathActivity;

        // Trạng thái Player
        private bool goUp, goDown, goLeft, goRight, isRunning, isAttacking, isHurt, isDead;
        private bool canMove = true;
        private string lastFacingDirection = "down";
        private Point mousePos;

        // Tốc độ Player
        private int walkSpeed = 6;
        private int runSpeed = 10;
        private int dashSpeed = 25;
        private bool isDashing = false;
        private int dashTimer = 0;
        private int dashCooldown = 0;
        private float dashDirectionX, dashDirectionY;

        // --- Biến Map ---
        private int mapWidth;
        private int mapHeight;
        private Image floorTexture;
        private Image wallTexture;
        private Image darkFloorTexture;
        private Image darkWallTexture;
        private Image chestTexture; // **MERGE: Thêm từ file 1**

        // --- Danh sách thực thể (Entity Lists) ---
        private List<Monster> monsters = new List<Monster>();
        private List<SpellEffect> spellEffects = new List<SpellEffect>();
        private Portal _portal = null; // **GIỮ LẠI: Từ file gốc**
        private List<Chest> chests = new List<Chest>(); // **MERGE: Thêm từ file 1**
        private List<LootEffect> lootEffects = new List<LootEffect>(); // **MERGE: Thêm từ file 1**

        private List<RectangleF> _mazeWallRects = new List<RectangleF>();
        private int[,] _mazeGrid;

        // Hiệu ứng làm tối
        private ImageAttributes _darkenAttributes = new ImageAttributes();


        public frmMainGame(DAL.Models.PlayerCharacters character, int mapLevel)
        {
            InitializeComponent();

            // Thiết lập ma trận màu làm tối
            float[][] colorMatrixElements = {
                new float[] {.5f,  .0f,  .0f,  .0f, 0.0f},
                new float[] {.0f,  .5f,  .0f,  .0f, 0.0f},
                new float[] {.0f,  .0f,  .5f,  .0f, 0.0f},
                new float[] {.0f,  .0f,  .0f, 1.0f, 0.0f},
                new float[] {.0f, 0.0f, 0.0f, 0.0f, 1.0f}
            };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            _darkenAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            _character = character;
            _mapLevel = mapLevel;

            // Khởi tạo BLL
            _gameSessionService = new GameSessionService();
            _inventory = _character.PlayerSessionInventory.FirstOrDefault();

            // Tính chỉ số
            UpdateCalculatedStats();

            // Cài đặt Form
            this.Text = $"Chơi - {_character.CharacterName} (Map: {_mapLevel})";

            // **THÊM: Bật chức năng Phóng to, Thu nhỏ, và Thay đổi kích thước**
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            // **KẾT THÚC THÊM**

            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Cursor = Cursors.Cross;

            // **THÊM: Đăng ký sự kiện Resize**
            this.Resize += new System.EventHandler(this.frmMainGame_Resize);

            mapWidth = this.ClientSize.Width;
            mapHeight = this.ClientSize.Height;

            // Tải tài nguyên
            LoadCharacterAnimations();
            LoadMapTextures();
            GenerateMaze();

            // Đặt vị trí bắt đầu của Player
            float passageCenterX = 1 * TILE_SIZE + TILE_SIZE / 2f;
            float passageCenterY = 1 * TILE_SIZE + TILE_SIZE / 2f;
            playerX = (float)Math.Floor(passageCenterX - playerWidth / 2f);
            playerY = (float)Math.Floor(passageCenterY - playerHeight / 2f);

            // Tạo quái
            InitializeMonsters(); // **SỬ DỤNG: Logic spawn mới (đã bị thay thế)**
            InitializeChests(); // **MERGE: Thêm từ file 1**

            gameTimer.Start();
        }

        /// <summary>
        /// Tính toán lại chỉ số của người chơi
        /// </summary>
        private void UpdateCalculatedStats()
        {
            if (_character == null || _gameSessionService == null)
            {
                _calculatedStats = new BLL.Services.GameSessionService.CalculatedStats { TotalAttack = 0, TotalDefense = 0, TotalHealth = 1 };
                _currentHealth = 1;
                _currentStamina = 1;
                return;
            }

            _calculatedStats = _gameSessionService.CalculateStats(_character, _inventory);
            _currentHealth = _calculatedStats.TotalHealth;
            _currentStamina = _character.BaseStamina;
        }

        private void LoadCharacterAnimations()
        {
            try
            {
                string playerRoot = Path.Combine("ImgSource", "Char", "Player", "SwordMan");

                string idleRoot = Path.Combine(playerRoot, "Stand");
                idleActivity = new AnimationActivity(10);
                idleActivity.LoadImages(Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"), Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right"));

                string walkRoot = Path.Combine(playerRoot, "Walk");
                walkActivity = new AnimationActivity(6);
                walkActivity.LoadImages(Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"), Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right"));

                string runRoot = Path.Combine(playerRoot, "Run");
                runActivity = new AnimationActivity(4);
                runActivity.LoadImages(Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"), Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right"));

                string attackRoot = Path.Combine(playerRoot, "Atk");
                attackActivity = new AnimationActivity(3) { IsLooping = false };
                attackActivity.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

                string walkAtkRoot = Path.Combine(playerRoot, "Walk_Atk");
                walkAttackActivity = new AnimationActivity(3) { IsLooping = false };
                walkAttackActivity.LoadImages(Path.Combine(walkAtkRoot, "Back"), Path.Combine(walkAtkRoot, "Front"), Path.Combine(walkAtkRoot, "Left"), Path.Combine(walkAtkRoot, "Right"));

                string runAtkRoot = Path.Combine(playerRoot, "Run_Atk");
                runAttackActivity = new AnimationActivity(3) { IsLooping = false };
                runAttackActivity.LoadImages(Path.Combine(runAtkRoot, "Back"), Path.Combine(runAtkRoot, "Front"), Path.Combine(runAtkRoot, "Left"), Path.Combine(runAtkRoot, "Right"));

                string hurtRoot = Path.Combine(playerRoot, "Hurt");
                hurtActivity = new AnimationActivity(5) { IsLooping = false };
                hurtActivity.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

                string deathRoot = Path.Combine(playerRoot, "Death");
                deathActivity = new AnimationActivity(8) { IsLooping = false };
                deathActivity.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));

                playerImage = idleActivity.GetDefaultFrame("down");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải hoạt ảnh Player: " + ex.Message);
            }
        }

        private Image LoadImageSafe(string path, Image fallbackImage)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return fallbackImage;
                }
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải ảnh {path}: {ex.Message}");
                return fallbackImage;
            }
        }

        private void LoadMapTextures()
        {
            try
            {
                string floorPath = Path.Combine("ImgSource", "Block", "default", "Floor", "catacombs_0.png");
                string wallPath = Path.Combine("ImgSource", "Block", "default", "Wall", "Stone_Wall", "stone_brick_1.png");
                // **MERGE: Thêm đường dẫn Rương**
                string chestPath = Path.Combine("ImgSource", "Structure", "chest", "chest_animation_1.png");

                floorTexture = Image.FromFile(floorPath);
                wallTexture = Image.FromFile(wallPath);

                // **MERGE: Tải ảnh Rương (sử dụng LoadImageSafe)**
                chestTexture = LoadImageSafe(chestPath, wallTexture); // Dùng wall làm fallback

                // Gán texture tối bằng texture sáng để tránh lỗi
                darkFloorTexture = floorTexture;
                darkWallTexture = wallTexture;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải texture map: " + ex.Message);
            }
        }

        // --- TẠO MÊ CUNG ---
        private void GenerateMaze()
        {
            // (Giả định bạn có lớp MazeGenerator và GameObjectType)
            try
            {
                // Giả định lớp MazeGenerator nằm trong namespace GUI.GameEntities
                // Hoặc nếu bạn có file MazeGeneration.cs, hãy đảm bảo nó có namespace đúng
                MazeGenerator mazeGen = new MazeGenerator(MAZE_LOGIC_WIDTH, MAZE_LOGIC_HEIGHT);
                mazeGen.GenerateMaze();
                _mazeGrid = mazeGen.Maze;
                _mazeWallRects.Clear();

                int currentLogicWidth = _mazeGrid.GetLength(1);
                int currentLogicHeight = _mazeGrid.GetLength(0);

                for (int y = 0; y < currentLogicHeight; y++)
                {
                    for (int x = 0; x < currentLogicWidth; x++)
                    {
                        if (_mazeGrid[y, x] == (int)GameObjectType.Wall)
                        {
                            _mazeWallRects.Add(new RectangleF(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo mê cung: {ex.Message}. Hãy đảm bảo bạn có file MazeGeneration.cs.");
                // Tạo một mê cung rỗng để tránh crash
                _mazeGrid = new int[MAZE_LOGIC_HEIGHT, MAZE_LOGIC_WIDTH];
            }
        }

        // **MERGE: THAY THẾ HOÀN TOÀN HÀM NÀY BẰNG LOGIC TỪ FILE 1**
        private void InitializeMonsters()
        {
            monsters.Clear(); // Đảm bảo làm sạch

            // Kích thước hitbox logic (1:1)
            int slimeWidth = 20, slimeHeight = 20;
            int orcWidth = 25, orcHeight = 25;
            int bossWidth = 40, bossHeight = 40;

            // SINH SLIME (25 con)
            for (int i = 0; i < 25; i++)
            {
                PointF pos = FindRandomPassagePosition(slimeWidth, slimeHeight);
                monsters.Add(new Slime(pos.X, pos.Y));
            }

            // SINH ORC (25 con)
            for (int i = 0; i < 25; i++)
            {
                PointF pos = FindRandomPassagePosition(orcWidth, orcHeight);
                monsters.Add(new Orc(pos.X, pos.Y));
            }

            // SPAWN BOSS (cách xa ít nhất 1/2 map)
            // PointF bossPos = FindRandomPassagePosition(bossWidth, bossHeight, isBoss: true); // TẮT SPAWN NGẪU NHIÊN

            // --- SPAWN BOSS CỐ ĐỊNH Ở GÓC DƯỚI BÊN PHẢI ---
            // Chọn ô tile (49, 29) (vì map rộng 51 và cao 31, trừ 2 để tránh tường biên)
            int bossTileX = MAZE_LOGIC_WIDTH - 2;  // 51 - 2 = 49
            int bossTileY = MAZE_LOGIC_HEIGHT - 2; // 31 - 2 = 29

            // Tính toán vị trí TÂM (logic 1:1) của ô tile đó
            float bossCenterX = bossTileX * TILE_SIZE + TILE_SIZE / 2f;
            float bossCenterY = bossTileY * TILE_SIZE + TILE_SIZE / 2f;

            // Tính toán vị trí Top-Left (X, Y) của Boss dựa trên hitbox
            float bossSpawnX = bossCenterX - bossWidth / 2f;
            float bossSpawnY = bossCenterY - bossHeight / 2f;

            PointF bossPos = new PointF(bossSpawnX, bossSpawnY);
            // --- KẾT THÚC SPAWN CỐ ĐỊNH ---

            monsters.Add(new Boss(bossPos.X, bossPos.Y));
        }

        // **MERGE: THÊM HÀM HELPER TỪ FILE 1**
        private PointF FindRandomPassagePosition(int entityWidth, int entityHeight, bool isBoss = false)
        {
            Random rand = new Random();
            int currentLogicHeight = _mazeGrid.GetLength(0);
            int currentLogicWidth = _mazeGrid.GetLength(1);

            // Vị trí tâm Player logic (1:1)
            float playerCenterX = playerX + playerWidth / 2f;
            float playerCenterY = playerY + playerHeight / 2f;

            // Khoảng cách tối thiểu cho Boss (1/2 Map logic)
            float minBossDistanceSq = (MAZE_LOGIC_WIDTH * TILE_SIZE / 2f);
            minBossDistanceSq *= minBossDistanceSq;

            // Phạm vi đẩy ra khi quá 3 quái (15 ô logic)
            float repelRange = TILE_SIZE * 15;
            float repelRangeSq = repelRange * repelRange;

            for (int i = 0; i < 500; i++) // Thử 500 lần
            {
                int tileX = rand.Next(1, currentLogicWidth - 1);
                int tileY = rand.Next(1, currentLogicHeight - 1);

                if (_mazeGrid[tileY, tileX] == (int)GameObjectType.Wall || _mazeGrid[tileY, tileX] == (int)GameObjectType.Passage)
                {
                    float centerX = tileX * TILE_SIZE + TILE_SIZE / 2f;
                    float centerY = tileY * TILE_SIZE + TILE_SIZE / 2f;

                    // 1. Kiểm tra va chạm Tường
                    RectangleF testHitbox = new RectangleF(centerX - entityWidth / 2f, centerY - entityHeight / 2f, entityWidth, entityHeight);
                    RectangleF scaledTestHitbox = new RectangleF(
                        testHitbox.X * RENDER_SCALE, testHitbox.Y * RENDER_SCALE,
                        testHitbox.Width * RENDER_SCALE, testHitbox.Height * RENDER_SCALE
                    );

                    if (IsCollidingWithWallScaled(scaledTestHitbox))
                        continue; // Bỏ qua nếu va chạm tường

                    // 2. Kiểm tra khoảng cách tối thiểu cho Boss
                    float distToPlayerSq = (centerX - playerCenterX) * (centerX - playerCenterX) + (centerY - playerCenterY) * (centerY - playerCenterY);

                    if (isBoss && distToPlayerSq < minBossDistanceSq)
                        continue; // Boss phải cách xa hơn 1/2 map

                    // 3. Kiểm tra mật độ (Tối đa 3 quái vật trong 1 ô logic 30x30)
                    int nearbyMonsters = monsters.Count(m =>
                        m.Hitbox.IntersectsWith(testHitbox) ||
                        (m.X >= centerX - TILE_SIZE && m.X <= centerX + TILE_SIZE &&
                         m.Y >= centerY - TILE_SIZE && m.Y <= centerY + TILE_SIZE)
                    );

                    if (nearbyMonsters <= 3)
                    {
                        // Vị trí hợp lệ
                        return new PointF(testHitbox.X, testHitbox.Y);
                    }
                }
            }

            // Nếu không tìm được vị trí lý tưởng (hoặc mật độ quá cao), tìm một vị trí xa hơn
            // Đây là logic repelling (đẩy ra)
            for (int i = 0; i < 500; i++)
            {
                int tileX = rand.Next(1, currentLogicWidth - 1);
                int tileY = rand.Next(1, currentLogicHeight - 1);

                if (_mazeGrid[tileY, tileX] == (int)GameObjectType.Wall || _mazeGrid[tileY, tileX] == (int)GameObjectType.Passage)
                {
                    float centerX = tileX * TILE_SIZE + TILE_SIZE / 2f;
                    float centerY = tileY * TILE_SIZE + TILE_SIZE / 2f;

                    RectangleF testHitbox = new RectangleF(centerX - entityWidth / 2f, centerY - entityHeight / 2f, entityWidth, entityHeight);
                    RectangleF scaledTestHitbox = new RectangleF(
                        testHitbox.X * RENDER_SCALE, testHitbox.Y * RENDER_SCALE,
                        testHitbox.Width * RENDER_SCALE, testHitbox.Height * RENDER_SCALE
                    );

                    if (IsCollidingWithWallScaled(scaledTestHitbox)) continue;

                    // Kiểm tra xem vị trí mới này có cách xa các quái vật khác không (10-20 ô)
                    bool isFarEnough = true;
                    foreach (var m in monsters)
                    {
                        float dx = m.X - testHitbox.X;
                        float dy = m.Y - testHitbox.Y;
                        if ((dx * dx + dy * dy) < repelRangeSq)
                        {
                            isFarEnough = false;
                            break;
                        }
                    }

                    if (isFarEnough)
                    {
                        return new PointF(testHitbox.X, testHitbox.Y);
                    }
                }
            }

            // Fallback cuối cùng
            return new PointF(1.5f * TILE_SIZE - entityWidth / 2f, 1.5f * TILE_SIZE - entityHeight / 2f);
        }

        // **MERGE: THÊM HÀM TỪ FILE 1**
        private void InitializeChests()
        {
            chests.Clear(); // Xóa rương cũ

            // Rương 1: Góc trên bên trái, sát biên (1, 3)
            chests.Add(new Chest(1 * TILE_SIZE + TILE_SIZE / 2f, 3 * TILE_SIZE + TILE_SIZE / 2f));
            // Rương 2: Góc trên bên phải, sát biên (49, 1)
            chests.Add(new Chest((MAZE_LOGIC_WIDTH - 2) * TILE_SIZE + TILE_SIZE / 2f, 1 * TILE_SIZE + TILE_SIZE / 2f));
            // Rương 3: Góc dưới bên trái, sát biên (1, 29)
            chests.Add(new Chest(1 * TILE_SIZE + TILE_SIZE / 2f, (MAZE_LOGIC_HEIGHT - 2) * TILE_SIZE + TILE_SIZE / 2f));
            // Rương 4: Góc dưới bên phải (tránh Boss) (45, 27)
            chests.Add(new Chest(45 * TILE_SIZE + TILE_SIZE / 2f, 27 * TILE_SIZE + TILE_SIZE / 2f));
            // Rương 5: Trung tâm map, hốc chẵn (25, 15)
            chests.Add(new Chest(25 * TILE_SIZE + TILE_SIZE / 2f, 15 * TILE_SIZE + TILE_SIZE / 2f));
            // Rương 6: Gần góc (35, 7)
            chests.Add(new Chest(35 * TILE_SIZE + TILE_SIZE / 2f, 7 * TILE_SIZE + TILE_SIZE / 2f));
        }

        #region Input Events

        private void frmMainGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (isDead) return;

            // **MERGE: Thêm kiểm tra phím E cho Rương**
            if (e.KeyCode == Keys.E)
            {
                CheckChestInteraction(); // Gọi hàm kiểm tra tương tác rương
                return;
            }

            if (e.KeyCode == Keys.W) goUp = true;
            if (e.KeyCode == Keys.S) goDown = true;
            if (e.KeyCode == Keys.A) goLeft = true;
            if (e.KeyCode == Keys.D) goRight = true;
            if (e.KeyCode == Keys.ControlKey) isRunning = true;

            if (e.KeyCode == Keys.ShiftKey && !isDashing && dashCooldown == 0 && _currentStamina > 10)
            {
                isDashing = true;
                dashTimer = 10;
                dashCooldown = 60;
                GetDashDirection();
            }
        }

        private void frmMainGame_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) goUp = false;
            if (e.KeyCode == Keys.S) goDown = false;
            if (e.KeyCode == Keys.A) goLeft = false;
            if (e.KeyCode == Keys.D) goRight = false;
            if (e.KeyCode == Keys.ControlKey) isRunning = false;
        }

        private void frmMainGame_MouseDown(object sender, MouseEventArgs e)
        {
            if (isDead || isAttacking || isDashing) return;

            if (e.Button == MouseButtons.Left)
            {
                isAttacking = true;
                attackActivity.ResetFrame();
                walkAttackActivity.ResetFrame();
                runAttackActivity.ResetFrame();

                CheckPlayerAttackHit();
            }
        }

        private void frmMainGame_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        #endregion

        // **MERGE: THÊM HÀM TỪ FILE 1**
        private void CheckChestInteraction()
        {
            RectangleF playerHitboxLogic = new RectangleF(playerX, playerY, playerWidth, playerHeight);

            foreach (var chest in chests.ToList())
            {
                // Kiểm tra Player có va chạm với rương không
                if (!chest.IsOpened && playerHitboxLogic.IntersectsWith(chest.Hitbox))
                {
                    OpenChest(chest);
                    return; // Chỉ mở 1 rương mỗi lần nhấn phím
                }
            }
        }

        // **MERGE: THÊM HÀM TỪ FILE 1**
        private void OpenChest(Chest chest)
        {
            chest.IsOpened = true;
            chest.StartOpenAnimation(); // Bắt đầu hoạt ảnh mở
            _chestsOpened++;

            // 100% ra tối thiểu 1 bình
            int potionCount = 1;
            int roll = new Random().Next(1, 101); // Roll từ 1 đến 100

            if (roll <= 10) potionCount = 4;
            else if (roll <= 30) potionCount = 3;
            else if (roll <= 70) potionCount = 2;
            else potionCount = 1;

            MessageBox.Show($"Bạn tìm thấy {potionCount} bình Thuốc Hồi Máu!", "Rương đã mở");

            // Tạo hiệu ứng Loot tại vị trí rương
            for (int i = 0; i < potionCount; i++)
            {
                float lootX = chest.X + new Random().Next(-10, 10);
                float lootY = chest.Y + new Random().Next(-10, 10);
                lootEffects.Add(new LootEffect(lootX, lootY, chest.Width, chest.Height));
            }

            // HỒI MÁU (Giả định mỗi bình hồi 50 HP)
            _currentHealth = Math.Min(_calculatedStats.TotalHealth, _currentHealth + potionCount * 50);
        }

        // --- VÒNG LẶP GAME CHÍNH ---
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            UpdateFacingDirection();
            UpdateDashState();
            UpdateMovement();
            UpdateCamera();
            UpdateFogOfWar();
            UpdateAnimation();

            // Cập nhật quái vật
            foreach (var monster in monsters.ToList())
            {
                monster.Update(this, playerX, playerY, spellEffects);

                // **MERGE: Gộp logic xử lý quái vật chết**
                if (monster.State == Monster.MonsterState.Dead && monster.deathAnim.IsFinished)
                {
                    float monsterCenterX = monster.X + monster.Width / 2;
                    float monsterCenterY = monster.Y + monster.Height / 2;

                    monsters.Remove(monster);
                    _monstersKilled++;

                    // **MERGE: Thêm Loot Effect (từ file 1)**
                    int lootCount = (monster is Boss) ? 5 : 1;
                    for (int i = 0; i < lootCount; i++)
                    {
                        lootEffects.Add(new LootEffect(monster.X, monster.Y, monster.Width, monster.Height));
                    }

                    // **GIỮ LẠI: Logic tạo Portal (từ file gốc)**
                    if (monster is Boss)
                    {
                        _isBossDefeated = true;
                        _portal = new Portal(monsterCenterX, monsterCenterY);
                    }
                }
            }

            // Cập nhật hiệu ứng skill
            foreach (var spell in spellEffects.ToList())
            {
                spell.Update();
                if (spell.IsFinished)
                {
                    spellEffects.Remove(spell);
                    spell.Dispose(); // <-- GỌI DISPOSE
                }
            }

            // **MERGE: Thêm logic cập nhật Loot và Rương (từ file 1)**
            foreach (var loot in lootEffects.ToList())
            {
                loot.Update();
                if (loot.IsFinished)
                {
                    lootEffects.Remove(loot);
                }
            }

            foreach (var chest in chests)
            {
                chest.UpdateAnimation();
            }

            // **GIỮ LẠI: Logic cập nhật Portal (từ file gốc)**
            if (_portal != null && !_isExiting)
            {
                _portal.Update();
                RectangleF playerHitbox = new RectangleF(playerX, playerY, 10, 10);

                if (_portal.CheckCollision(playerHitbox))
                {
                    _isExiting = true;
                    // **GIỮ LẠI: Logic thoát an toàn (từ file gốc)**
                    HandleMapExit("Đã qua cổng!");
                }
            }

            // Hồi Stamina
            UpdateStaminaRegen();

            // Yêu cầu vẽ lại
            this.Invalidate();
        }

        /// <summary>
        /// Xử lý hồi Stamina
        /// </summary>
        private void UpdateStaminaRegen()
        {
            // **MERGE: Logic hồi stamina phức tạp hơn từ file 1 (đã được tích hợp vào file gốc)**
            // (File gốc đã có logic hồi stamina đơn giản, ta giữ nguyên)
            if (!isDashing && _currentStamina < _character.BaseStamina)
            {
                _currentStamina += 1;
                if (_currentStamina > _character.BaseStamina)
                    _currentStamina = _character.BaseStamina;
            }
        }

        // --- CẬP NHẬT FOG OF WAR ---
        private void UpdateFogOfWar()
        {
            int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
            int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);

            int startX = Math.Max(0, playerTileX - (int)Math.Ceiling(REVEAL_RADIUS));
            int endX = Math.Min(MAZE_LOGIC_WIDTH - 1, playerTileX + (int)Math.Ceiling(REVEAL_RADIUS));
            int startY = Math.Max(0, playerTileY - (int)Math.Ceiling(REVEAL_RADIUS));
            int endY = Math.Min(MAZE_LOGIC_HEIGHT - 1, playerTileY + (int)Math.Ceiling(REVEAL_RADIUS));

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    if (Math.Pow(x - playerTileX, 2) + Math.Pow(y - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2))
                    {
                        _isTileVisible[y, x] = true;
                    }
                }
            }
        }

        // --- CẬP NHẬT VỊ TRÍ CAMERA ---
        private void UpdateCamera()
        {
            float playerCenterX = playerX + playerWidth / 2;
            float playerCenterY = playerY + playerHeight / 2;

            float playerScaledCenterX = playerCenterX * RENDER_SCALE;
            float playerScaledCenterY = playerCenterY * RENDER_SCALE;

            _cameraX = playerScaledCenterX - mapWidth / 2;
            _cameraY = playerScaledCenterY - mapHeight / 2;

            if (_cameraX < 0) _cameraX = 0;
            if (_cameraY < 0) _cameraY = 0;
            if (_cameraX + mapWidth > _mapLogicalWidthScaled) _cameraX = _mapLogicalWidthScaled - mapWidth;
            if (_cameraY + mapHeight > _mapLogicalHeightScaled) _cameraY = _mapLogicalHeightScaled - mapHeight;

            if (_mapLogicalWidthScaled < mapWidth)
            {
                _cameraX = -(mapWidth - _mapLogicalWidthScaled) / 2;
            }
            if (_mapLogicalHeightScaled < mapHeight)
            {
                _cameraY = -(mapHeight - _mapLogicalHeightScaled) / 2;
            }
        }

        // --- HÀM VẼ CHÍNH ---
        private void frmMainGame_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;
            canvas.TranslateTransform(-_cameraX, -_cameraY);

            // 1. VẼ NỀN ĐEN
            canvas.FillRectangle(Brushes.Black, 0, 0, _mapLogicalWidthScaled, _mapLogicalHeightScaled);

            // 2. VẼ SÀN và TƯỜNG
            DrawFloor(canvas);
            DrawWalls(canvas);

            // **MERGE: Thêm DrawChests (từ file 1)**
            DrawChests(canvas);

            // 3. VẼ CÁC THỰC THỂ
            if (playerImage != null)
            {
                int drawX = (int)(playerX * RENDER_SCALE) - (100 - playerWidth) / 2;
                int drawY = (int)(playerY * RENDER_SCALE) - (100 - playerHeight) / 2;
                canvas.DrawImage(playerImage, drawX, drawY, 100, 100);
            }

            RectangleF viewportRect = new RectangleF(_cameraX, _cameraY, mapWidth, mapHeight);

            foreach (var monster in monsters)
            {
                int monsterTileX = (int)((monster.X + monster.Width / 2) / TILE_SIZE);
                int monsterTileY = (int)((monster.Y + monster.Height / 2) / TILE_SIZE);

                if (monsterTileY >= 0 && monsterTileY < MAZE_LOGIC_HEIGHT &&
                    monsterTileX >= 0 && monsterTileX < MAZE_LOGIC_WIDTH &&
                    _isTileVisible[monsterTileY, monsterTileX])
                {
                    RectangleF scaledHitbox = monster.ScaledHitbox(RENDER_SCALE);
                    if (viewportRect.IntersectsWith(scaledHitbox))
                    {
                        monster.Draw(canvas, RENDER_SCALE);
                    }
                }
            }

            foreach (var spell in spellEffects)
            {
                spell.Draw(canvas, RENDER_SCALE);
            }

            // **MERGE: Thêm Draw LootEffects (từ file 1)**
            foreach (var loot in lootEffects)
            {
                loot.Draw(canvas, RENDER_SCALE);
            }

            // **GIỮ LẠI: Draw Portal (từ file gốc)**
            if (_portal != null)
            {
                _portal.Draw(canvas, RENDER_SCALE);
            }

            // 4. VẼ HUD
            canvas.ResetTransform();
            DrawHUD(canvas);
        }

        // **MERGE: THÊM HÀM TỪ FILE 1**
        private void DrawChests(Graphics canvas)
        {
            RectangleF viewportRect = new RectangleF(_cameraX, _cameraY, mapWidth, mapHeight);
            int chestDrawSize = 60; // Kích thước vẽ rương (đã scale)

            foreach (var chest in chests)
            {
                int chestTileX = (int)((chest.X) / TILE_SIZE);
                int chestTileY = (int)((chest.Y) / TILE_SIZE);

                if (chestTileY >= 0 && chestTileY < MAZE_LOGIC_HEIGHT &&
                    chestTileX >= 0 && chestTileX < MAZE_LOGIC_WIDTH &&
                    _isTileVisible[chestTileY, chestTileX])
                {
                    float drawX = chest.X * RENDER_SCALE - (chestDrawSize - chest.Width * RENDER_SCALE) / 2f;
                    float drawY = chest.Y * RENDER_SCALE - (chestDrawSize - chest.Height * RENDER_SCALE) / 2f;

                    RectangleF scaledRect = new RectangleF(drawX, drawY, chestDrawSize, chestDrawSize);

                    if (viewportRect.IntersectsWith(scaledRect))
                    {
                        Image imageToDraw = chestTexture;
                        ImageAttributes attributesToUse = null;

                        if (chest.IsOpened)
                        {
                            imageToDraw = chest.OpenAnimation.GetNextFrame("down");
                            if (imageToDraw == null)
                            {
                                imageToDraw = chest.OpenAnimation.GetDefaultFrame("down");
                            }
                            attributesToUse = null; // Luôn sáng khi mở
                        }
                        else
                        {
                            attributesToUse = _darkenAttributes; // Tối nếu chưa mở
                        }

                        canvas.DrawImage(
                            imageToDraw,
                            new Rectangle((int)drawX, (int)drawY, chestDrawSize, chestDrawSize),
                            0, 0, imageToDraw.Width, imageToDraw.Height,
                            GraphicsUnit.Pixel,
                            attributesToUse
                        );
                    }
                }
            }
        }


        /// <summary>
        /// Vẽ giao diện người dùng (HUD)
        /// </summary>
        private void DrawHUD(Graphics canvas)
        {
            try
            {
                int x = 15;
                int y = 15;
                int hudWidth = 250;
                int hudHeight = 160;
                int barWidth = hudWidth - 20;
                int barHeight = 18;

                canvas.FillRectangle(_hudBackgroundBrush, x, y, hudWidth, hudHeight);
                canvas.DrawRectangle(Pens.Gray, x, y, hudWidth, hudHeight);

                canvas.DrawString(_character.CharacterName, _hudTitleFont, _hudBrush, x + 10, y + 10);

                float healthPercent = (_calculatedStats.TotalHealth > 0) ? (float)_currentHealth / _calculatedStats.TotalHealth : 0;
                int healthBarWidth = (int)(barWidth * healthPercent);
                canvas.DrawString($"HP: {_currentHealth} / {_calculatedStats.TotalHealth}", _hudFont, _hudBrush, x + 10, y + 35);
                canvas.FillRectangle(_barBackgroundBrush, x + 10, y + 55, barWidth, barHeight);
                canvas.FillRectangle(_healthBrush, x + 10, y + 55, healthBarWidth, barHeight);
                canvas.DrawRectangle(Pens.Black, x + 10, y + 55, barWidth, barHeight);

                float staminaPercent = (_character.BaseStamina > 0) ? (float)_currentStamina / _character.BaseStamina : 0;
                int staminaBarWidth = (int)(barWidth * staminaPercent);
                canvas.FillRectangle(_barBackgroundBrush, x + 10, y + 80, barWidth, barHeight);
                canvas.FillRectangle(_staminaBrush, x + 10, y + 80, staminaBarWidth, barHeight);
                canvas.DrawRectangle(Pens.Black, x + 10, y + 80, barWidth, barHeight);

                canvas.DrawString($"ATK: {_calculatedStats.TotalAttack}", _hudFont, _hudBrush, x + 10, y + 105);
                canvas.DrawString($"DEF: {_calculatedStats.TotalDefense}", _hudFont, _hudBrush, x + 90, y + 105);

                // **MERGE: Cập nhật text Rương (từ file 1)**
                canvas.DrawString($"Quái: {_monstersKilled}", _hudFont, _hudBrush, x + 10, y + 130);
                canvas.DrawString($"Rương: {_chestsOpened}", _hudFont, _hudBrush, x + 90, y + 130);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi vẽ HUD: " + ex.Message);
            }
        }


        #region Logic Phụ (Va chạm, Tầm nhìn, Di chuyển...)

        // **THÊM: Xử lý sự kiện Resize**
        private void frmMainGame_Resize(object sender, EventArgs e)
        {
            // Cập nhật lại kích thước mapWidth/mapHeight để camera tính toán lại
            mapWidth = this.ClientSize.Width;
            mapHeight = this.ClientSize.Height;
            // Yêu cầu vẽ lại để áp dụng kích thước mới
            this.Invalidate();
        }

        private void CheckPlayerAttackHit()
        {
            RectangleF attackHitbox = new RectangleF();
            int attackRange = 50;
            int attackWidth = 40;

            switch (lastFacingDirection)
            {
                case "up":
                    attackHitbox = new RectangleF(playerX + (playerWidth / 2) - (attackWidth / 2), playerY - attackRange, attackWidth, attackRange);
                    break;
                case "down":
                    attackHitbox = new RectangleF(playerX + (playerWidth / 2) - (attackWidth / 2), playerY + playerHeight, attackWidth, attackRange);
                    break;
                case "left":
                    attackHitbox = new RectangleF(playerX - attackRange, playerY, attackRange, playerHeight);
                    break;
                case "right":
                    attackHitbox = new RectangleF(playerX + playerWidth, playerY, attackRange, playerHeight);
                    break;
            }

            RectangleF scaledAttackHitbox = new RectangleF(
                attackHitbox.X * RENDER_SCALE,
                attackHitbox.Y * RENDER_SCALE,
                attackHitbox.Width * RENDER_SCALE,
                attackHitbox.Height * RENDER_SCALE
            );


            foreach (var monster in monsters)
            {
                if (monster.State != Monster.MonsterState.Dead && scaledAttackHitbox.IntersectsWith(monster.ScaledHitbox(RENDER_SCALE)))
                {
                    int playerDamage = 10; // (Sau này sẽ lấy từ _calculatedStats.TotalAttack)
                    monster.TakeDamage(playerDamage);
                }
            }
        }


        private void DrawFloor(Graphics canvas)
        {
            if (floorTexture == null || _mazeGrid == null) return;

            using (TextureBrush floorBrush = new TextureBrush(floorTexture, WrapMode.Tile))
            {
                int currentLogicHeight = _mazeGrid.GetLength(0);
                int currentLogicWidth = _mazeGrid.GetLength(1);

                for (int y = 0; y < currentLogicHeight; y++)
                {
                    for (int x = 0; x < currentLogicWidth; x++)
                    {
                        if (_mazeGrid[y, x] != (int)GameObjectType.Wall)
                        {
                            int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
                            int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);
                            bool isCurrentlyVisible = Math.Pow(x - playerTileX, 2) + Math.Pow(y - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2);

                            Image imageToDraw = floorTexture;
                            ImageAttributes attributesToUse = null;

                            if (_isTileVisible[y, x])
                            {
                                attributesToUse = _darkenAttributes;
                            }
                            if (isCurrentlyVisible)
                            {
                                attributesToUse = null;
                            }

                            if (attributesToUse != null || _isTileVisible[y, x])
                            {
                                canvas.DrawImage(
                                    imageToDraw,
                                    new Rectangle(
                                        x * TILE_SIZE * RENDER_SCALE,
                                        y * TILE_SIZE * RENDER_SCALE,
                                        TILE_SIZE * RENDER_SCALE,
                                        TILE_SIZE * RENDER_SCALE
                                    ),
                                    0, 0, imageToDraw.Width, imageToDraw.Height,
                                    GraphicsUnit.Pixel,
                                    attributesToUse
                                );
                            }
                        }
                    }
                }
            }
        }

        private void DrawWalls(Graphics canvas)
        {
            if (wallTexture == null) return;

            using (TextureBrush wallBrush = new TextureBrush(wallTexture, WrapMode.Tile))
            {
                foreach (var wall in _mazeWallRects)
                {
                    int wallTileX = (int)Math.Floor(wall.X / TILE_SIZE);
                    int wallTileY = (int)Math.Floor(wall.Y / TILE_SIZE);

                    if (wallTileY >= 0 && wallTileY < MAZE_LOGIC_HEIGHT &&
                        wallTileX >= 0 && wallTileX < MAZE_LOGIC_WIDTH)
                    {
                        int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
                        int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);
                        bool isCurrentlyVisible = Math.Pow(wallTileX - playerTileX, 2) + Math.Pow(wallTileY - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2);

                        ImageAttributes attributesToUse = null;

                        if (_isTileVisible[wallTileY, wallTileX])
                        {
                            attributesToUse = _darkenAttributes;
                        }
                        if (isCurrentlyVisible)
                        {
                            attributesToUse = null;
                        }

                        if (attributesToUse != null || _isTileVisible[wallTileY, wallTileX])
                        {
                            Image imageToDraw = wallTexture;
                            canvas.DrawImage(
                                imageToDraw,
                                new Rectangle(
                                    (int)(wall.X * RENDER_SCALE),
                                    (int)(wall.Y * RENDER_SCALE),
                                    (int)(wall.Width * RENDER_SCALE),
                                    (int)(wall.Height * RENDER_SCALE)
                                ),
                                0, 0, imageToDraw.Width, imageToDraw.Height,
                                GraphicsUnit.Pixel,
                                attributesToUse
                            );
                        }
                    }
                }
            }
        }


        private void UpdateFacingDirection()
        {
            if (isDashing) return;

            float mouseXOnMap = mousePos.X + _cameraX;
            float mouseYOnMap = mousePos.Y + _cameraY;

            float playerScaledX = playerX * RENDER_SCALE;
            float playerScaledY = playerY * RENDER_SCALE;

            float deltaX = mouseXOnMap - (playerScaledX + playerWidth / 2);
            float deltaY = mouseYOnMap - (playerScaledY + playerHeight / 2);

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                lastFacingDirection = (deltaX > 0) ? "right" : "left";
            }
            else
            {
                lastFacingDirection = (deltaY > 0) ? "down" : "up";
            }
        }

        private void GetDashDirection()
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

        private void UpdateDashState()
        {
            if (dashCooldown > 0) dashCooldown--;

            if (isDashing)
            {
                dashTimer--;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                }
            }
        }

        private void UpdateMovement()
        {
            float dx = 0;
            float dy = 0;

            if (isDashing)
            {
                dx = dashDirectionX * dashSpeed;
                dy = dashDirectionY * dashSpeed;

                if (_currentStamina > 0)
                {
                    _currentStamina -= 2;
                    if (_currentStamina < 0) _currentStamina = 0;
                }
                else
                {
                    isDashing = false;
                    dx = 0;
                    dy = 0;
                }
            }
            else
            {
                bool isMoving = goUp || goDown || goLeft || goRight;
                canMove = !isHurt && !isDead;

                if (isAttacking && !isMoving)
                {
                    canMove = false;
                }

                if (canMove && isMoving)
                {
                    float moveX = 0;
                    float moveY = 0;

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

                    int currentSpeed = isRunning ? runSpeed : walkSpeed;
                    dx = moveX * currentSpeed;
                    dy = moveY * currentSpeed;
                }
            }

            float scaledSlideTolerance = BASE_SLIDE_TOLERANCE / RENDER_SCALE;

            // 1. Thử di chuyển X
            RectangleF nextHitboxX = new RectangleF(playerX + dx, playerY, playerWidth, playerHeight);
            RectangleF scaledNextHitboxX = new RectangleF(
                nextHitboxX.X * RENDER_SCALE,
                nextHitboxX.Y * RENDER_SCALE,
                nextHitboxX.Width * RENDER_SCALE,
                nextHitboxX.Height * RENDER_SCALE
            );

            if (!IsCollidingWithWallScaled(scaledNextHitboxX))
            {
                playerX += dx;
            }
            else
            {
                if (dy != 0)
                {
                    float slideY = dy > 0 ? scaledSlideTolerance : -scaledSlideTolerance;
                    RectangleF slideHitboxY = new RectangleF(playerX + dx, playerY + slideY, playerWidth, playerHeight);
                    RectangleF scaledSlideHitboxY = new RectangleF(
                        slideHitboxY.X * RENDER_SCALE,
                        slideHitboxY.Y * RENDER_SCALE,
                        slideHitboxY.Width * RENDER_SCALE,
                        slideHitboxY.Height * RENDER_SCALE
                    );
                    if (!IsCollidingWithWallScaled(scaledSlideHitboxY))
                    {
                        playerY += slideY;
                    }
                }
            }

            // 2. Thử di chuyển Y
            RectangleF nextHitboxY = new RectangleF(playerX, playerY + dy, playerWidth, playerHeight);
            RectangleF scaledNextHitboxY = new RectangleF(
                nextHitboxY.X * RENDER_SCALE,
                nextHitboxY.Y * RENDER_SCALE,
                nextHitboxY.Width * RENDER_SCALE,
                nextHitboxY.Height * RENDER_SCALE
            );

            if (!IsCollidingWithWallScaled(scaledNextHitboxY))
            {
                playerY += dy;
            }
            else
            {
                if (dx != 0)
                {
                    float slideX = dx > 0 ? scaledSlideTolerance : -scaledSlideTolerance;
                    RectangleF slideHitboxX = new RectangleF(playerX + slideX, playerY + dy, playerWidth, playerHeight);
                    RectangleF scaledSlideHitboxX = new RectangleF(
                        slideHitboxX.X * RENDER_SCALE,
                        slideHitboxX.Y * RENDER_SCALE,
                        slideHitboxX.Width * RENDER_SCALE,
                        slideHitboxX.Height * RENDER_SCALE
                    );
                    if (!IsCollidingWithWallScaled(scaledSlideHitboxX))
                    {
                        playerX += slideX;
                    }
                }
            }
        }

        // HÀM KIỂM TRA VA CHẠM SCALED MỚI
        public bool IsCollidingWithWallScaled(RectangleF futureHitboxScaled)
        {
            float shrink = 2f;
            RectangleF shrunkenHitbox = new RectangleF(
                futureHitboxScaled.X + shrink / 2f,
                futureHitboxScaled.Y + shrink / 2f,
                futureHitboxScaled.Width - shrink,
                futureHitboxScaled.Height - shrink
            );

            foreach (var wall in _mazeWallRects)
            {
                RectangleF scaledWall = new RectangleF(
                    wall.X * RENDER_SCALE,
                    wall.Y * RENDER_SCALE,
                    wall.Width * RENDER_SCALE,
                    wall.Height * RENDER_SCALE
                );

                if (shrunkenHitbox.IntersectsWith(scaledWall))
                {
                    return true;
                }
            }

            // 2. Va chạm với TƯỜNG BIÊN
            if (futureHitboxScaled.Left < 0) return true;
            if (futureHitboxScaled.Top < 0) return true;
            if (futureHitboxScaled.Bottom > _mapLogicalHeightScaled) return true;
            if (futureHitboxScaled.Right > _mapLogicalWidthScaled) return true; // Chặn tường phải

            return false;
        }


        public bool HasLineOfSight(PointF start, PointF end)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return true;

            int checkInterval = TILE_SIZE / 2;
            int steps = (int)(distance / checkInterval);
            if (steps < 2) return true;

            float stepX = dx / steps;
            float stepY = dy / steps;

            for (int i = 1; i < steps; i++)
            {
                float checkX = start.X + stepX * i;
                float checkY = start.Y + stepY * i;

                int tileX = (int)(checkX / TILE_SIZE);
                int tileY = (int)(checkY / TILE_SIZE);

                RectangleF checkRect = new RectangleF(tileX * TILE_SIZE, tileY * TILE_SIZE, TILE_SIZE, TILE_SIZE);

                if (_mazeWallRects.Any(wall => wall.IntersectsWith(checkRect)))
                {
                    return false;
                }
            }
            return true;
        }


        // **GIỮ LẠI: Logic thoát an toàn (từ file gốc)**
        private void HandleMapExit(string message)
        {
            gameTimer.Stop(); // **QUAN TRỌNG: Dừng timer TRƯỚC KHI Mở MessageBox**
            MessageBox.Show($"Đã tìm thấy cửa ra! Bạn đã hoàn thành map này ({message}).");
            this.Close();
        }

        private void UpdateAnimation()
        {
            bool isMoving = goUp || goDown || goLeft || goRight;
            string direction = lastFacingDirection;

            if (isDead)
            {
                playerImage = deathActivity.GetNextFrame(direction);
                return;
            }
            else if (isHurt)
            {
                playerImage = hurtActivity.GetNextFrame(direction);
                if (hurtActivity.IsFinished) isHurt = false;
            }
            else if (isDashing)
            {
                playerImage = runActivity.GetNextFrame(direction);
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

        #endregion

        // --- XÓA TOÀN BỘ CÁC LỚP BÊN DƯỚI ĐÂY ---
        // (XÓA GameObjectType VÀ MazeGenerator)

        // **MERGE: Thêm các lớp (class) hỗ trợ từ file 1**

        // --- Lớp Hiệu ứng Rơi đồ ---
        public class LootEffect
        {
            private float X, Y;
            private int Width, Height;
            private float opacity = 1.0f;
            private int duration = 60; // 1 giây
            private int timer = 0;
            private float floatSpeed = 0.5f;
            public bool IsFinished => timer >= duration;

            public LootEffect(float monsterX, float monsterY, int monsterWidth, int monsterHeight)
            {
                Width = 15;
                Height = 15;
                X = monsterX + (monsterWidth / 2f) - (Width / 2f);
                Y = monsterY + (monsterHeight / 2f) - (Height / 2f);
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
                using (Brush brush = new SolidBrush(Color.FromArgb(alpha, 255, 215, 0))) // Màu vàng
                {
                    float drawX = X * scale;
                    float drawY = Y * scale;
                    float drawSize = Width * scale;
                    canvas.FillRectangle(brush, drawX, drawY, drawSize, drawSize);
                }
            }
        }

        // --- Lớp Rương (Chest) ---
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

    } // Kết thúc lớp frmMainGame


    /*
     
    --- BẮT ĐẦU XÓA TỪ ĐÂY ---

    // --- GIẢ ĐỊNH CÁC LỚP NÀY NẰM TRONG 1 FILE RIÊNG (VÍ DỤ: MazeGeneration.cs) ---
    // (Đây chỉ là placeholder để code frmMainGame có thể biên dịch)
    public enum GameObjectType
    {
        Wall = 0,
        Passage = 1,
        Start = 2,
        Exit = 3,
        Chest = 4
    }

    public class MazeGenerator
    {
        public int[,] Maze;
        public MazeGenerator(int width, int height)
        {
            Maze = new int[height, width];
            // Khởi tạo tất cả là tường
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    Maze[y, x] = (int)GameObjectType.Wall;
        }
        public void GenerateMaze()
        {
            // (Thêm thuật toán tạo mê cung thật ở đây...)
            // Tạo một đường đi đơn giản để test
            for (int y = 1; y < Maze.GetLength(0) - 1; y++)
                for (int x = 1; x < Maze.GetLength(1) - 1; x++)
                    Maze[y, x] = (int)GameObjectType.Passage;

            Maze[1, 1] = (int)GameObjectType.Start;
            Maze[Maze.GetLength(0) - 2, Maze.GetLength(1) - 2] = (int)GameObjectType.Exit;
        }
    }
    
    --- XÓA ĐẾN HẾT FILE ---
    
    */
}

