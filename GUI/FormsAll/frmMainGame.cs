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
using System.Diagnostics; // <-- THÊM: Để theo dõi thời gian chơi

namespace Main
{
    // --- LƯU Ý: MazeGenerator và GameObjectType cần được định nghĩa trong file MazeGeneration.cs của bạn ---

    public partial class frmMainGame : Form
    {
        private readonly DAL.Models.PlayerCharacters _character;
        private readonly int _mapLevel;
        private int _currentHealth; // Máu hiện tại của nhân vật
        private int _currentStamina; // THÊM: Stamina hiện tại
        private int _currentDefense; // Giáp hiện tại của nhân vật

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
        private bool _isExitOpen = false;
        private bool _isAutoPiloting = false;
        private PointF _autoPilotTarget;

        // --- Dịch vụ BLL và Chỉ số Game ---
        private GameSessionService _gameSessionService;
        // THÊM: Service Bảng xếp hạng
        private LeaderboardService _leaderboardService;
        // THÊM: Đồng hồ bấm giờ
        private Stopwatch _mapTimer;

        private DAL.Models.PlayerSessionInventory _inventory;
        private BLL.Services.GameSessionService.CalculatedStats _calculatedStats;

        public int _chestsOpened = 0;

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
        public int playerWidth = 10;
        public int playerHeight = 10;

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
        private Image chestTexture;
        private Image potionIcon;

        // --- THÊM: Biến cho Loot System ---
        private List<GroundItem> _groundItems = new List<GroundItem>();
        private List<MonsterLootTables> _monsterLootTables_RAM; // Bảng loot trong RAM
        private List<Weapons> _allWeapons_RAM; // Định nghĩa vũ khí trong RAM
        private List<Armors> _allArmors_RAM; // Định nghĩa giáp trong RAM
        private GameDefinitionService _definitionService; // Service để tải RAM
        private Random _lootRandom = new Random(); // Random cho loot

        // --- THÊM: Biến cho Ngoại Cảnh ---
        private List<SceneryObject> _sceneryObjects = new List<SceneryObject>();
        private List<Image> _sceneryImages_RAM = new List<Image>();

        // --- Danh sách thực thể (Entity Lists) ---
        private List<Monster> monsters = new List<Monster>();
        private List<SpellEffect> spellEffects = new List<SpellEffect>();
        private Portal _portal = null;
        private List<Chest> chests = new List<Chest>();
        private List<LootEffect> lootEffects = new List<LootEffect>();

        private List<RectangleF> _mazeWallRects = new List<RectangleF>();
        private int[,] _mazeGrid;

        // Hiệu ứng làm tối
        private ImageAttributes _darkenAttributes = new ImageAttributes();
        private Font _uiFont;


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
            _leaderboardService = new LeaderboardService(); // <-- THÊM: Khởi tạo service BXH
            _definitionService = new GameDefinitionService();
            _inventory = _character.PlayerSessionInventory.FirstOrDefault();

            // Tính chỉ số
            UpdateCalculatedStats();

            // Cài đặt Form
            this.Text = $"Chơi - {_character.CharacterName} (Map: {_mapLevel})";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Cursor = Cursors.Cross;
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
            InitializeMonsters();
            InitializeChests();

            // KHỞI TẠO MỤC TIÊU AI CHO PLAYER
            _autoPilotTarget = new PointF(playerX + playerWidth / 2f, playerY + playerHeight / 2f); // Bắt đầu tại vị trí hiện tại

            // <-- THÊM: Khởi động đồng hồ bấm giờ
            _mapTimer = new Stopwatch();
            _mapTimer.Start();

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
                string chestPath = Path.Combine("ImgSource", "Structure", "chest", "chest_animation_1.png");
                string potionIconPath = Path.Combine("ImgSource", "Item", "Potion", "Healing_Potion.png"); // (Giả sử tên file là potion_red_1.png)

                floorTexture = Image.FromFile(floorPath);
                wallTexture = Image.FromFile(wallPath);
                chestTexture = LoadImageSafe(chestPath, wallTexture);
                potionIcon = LoadImageSafe(potionIconPath, new Bitmap(32, 32)); // (Dùng bitmap rỗng làm fallback)

                darkFloorTexture = floorTexture;
                darkWallTexture = wallTexture;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải texture map: " + ex.Message);
            }

            // --- THÊM: Tải ảnh Ngoại Cảnh (Scenery) vào RAM ---
            try
            {
                string sceneryPath = Path.Combine("ImgSource", "Structure", "Water_Style", "Objects_separately");
                if (Directory.Exists(sceneryPath))
                {
                    // Lấy tất cả các file .png trong thư mục
                    var sceneryFiles = Directory.GetFiles(sceneryPath, "*.png");
                    foreach (var filePath in sceneryFiles)
                    {
                        // Dùng hàm LoadImageSafe (đã có) để tránh khóa file
                        _sceneryImages_RAM.Add(LoadImageSafe(filePath, null));
                    }
                    // Lọc bỏ các ảnh null nếu tải lỗi
                    _sceneryImages_RAM = _sceneryImages_RAM.Where(img => img != null).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tải ảnh ngoại cảnh: " + ex.Message);
            }

            // --- THÊM: Tải định nghĩa Game vào RAM ---
            // (Tuân thủ quy tắc "Không dùng DB trong Game Loop")
            try
            {
                _monsterLootTables_RAM = _definitionService.GetMonsterLootTables();
                _allWeapons_RAM = _definitionService.GetAllWeapons();
                _allArmors_RAM = _definitionService.GetAllArmors();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng: Không thể tải định nghĩa game từ CSDL: " + ex.Message);
                this.Close();
            }

        }

        // --- TẠO MÊ CUNG ---
        private void GenerateMaze()
        {
            try
            {
                MazeGenerator mazeGen = new MazeGenerator(MAZE_LOGIC_WIDTH, MAZE_LOGIC_HEIGHT);
                mazeGen.GenerateMaze();
                _mazeGrid = mazeGen.Maze;
                _mazeWallRects.Clear();
                _sceneryObjects.Clear();

                int currentLogicWidth = _mazeGrid.GetLength(1);
                int currentLogicHeight = _mazeGrid.GetLength(0);

                for (int y = 0; y < currentLogicHeight; y++)
                {
                    for (int x = 0; x < currentLogicWidth; x++)
                    {
                        if (_mazeGrid[y, x] == (int)GameObjectType.Wall)
                        {
                            _mazeWallRects.Add(new RectangleF(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));

                            // --- THÊM: Đặt ngoại cảnh ngẫu nhiên ---
                            // Chỉ đặt nếu có ảnh và tỉ lệ 10% trúng
                            if (_sceneryImages_RAM.Count > 0 && _lootRandom.Next(100) < 10) // 10% tỉ lệ
                            {
                                // Chọn 1 ảnh ngẫu nhiên từ RAM
                                Image randomSceneryImage = _sceneryImages_RAM[_lootRandom.Next(_sceneryImages_RAM.Count)];

                                // Tạo vật thể tại vị trí TILE của tường
                                _sceneryObjects.Add(new SceneryObject(x * TILE_SIZE, y * TILE_SIZE, randomSceneryImage));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tạo mê cung: {ex.Message}. Hãy đảm bảo bạn có file MazeGeneration.cs.");
                _mazeGrid = new int[MAZE_LOGIC_HEIGHT, MAZE_LOGIC_WIDTH];
            }
        }

        private void InitializeMonsters()
        {
            monsters.Clear();

            int slimeWidth = 20, slimeHeight = 20;
            int orcWidth = 25, orcHeight = 25;
            int bossWidth = 40, bossHeight = 40;

            for (int i = 0; i < 25; i++)
            {
                PointF pos = FindRandomPassagePosition(slimeWidth, slimeHeight);
                monsters.Add(new Slime(pos.X, pos.Y));
            }

            for (int i = 0; i < 25; i++)
            {
                PointF pos = FindRandomPassagePosition(orcWidth, orcHeight);
                monsters.Add(new Orc(pos.X, pos.Y));
            }

            int bossTileX = MAZE_LOGIC_WIDTH - 2;
            int bossTileY = MAZE_LOGIC_HEIGHT - 2;

            float bossCenterX = bossTileX * TILE_SIZE + TILE_SIZE / 2f;
            float bossCenterY = bossTileY * TILE_SIZE + TILE_SIZE / 2f;

            float bossSpawnX = bossCenterX - bossWidth / 2f;
            float bossSpawnY = bossCenterY - bossHeight / 2f;

            PointF bossPos = new PointF(bossSpawnX, bossSpawnY);

            monsters.Add(new Boss(bossPos.X, bossPos.Y));
        }

        private PointF FindRandomPassagePosition(int entityWidth, int entityHeight, bool isBoss = false)
        {
            Random rand = new Random();
            int currentLogicHeight = _mazeGrid.GetLength(0);
            int currentLogicWidth = _mazeGrid.GetLength(1);

            float playerCenterX = playerX + playerWidth / 2f;
            float playerCenterY = playerY + playerHeight / 2f;

            float minBossDistanceSq = (MAZE_LOGIC_WIDTH * TILE_SIZE / 2f);
            minBossDistanceSq *= minBossDistanceSq;

            float repelRange = TILE_SIZE * 15;
            float repelRangeSq = repelRange * repelRange;

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

                    if (IsCollidingWithWallScaled(scaledTestHitbox))
                        continue;

                    float distToPlayerSq = (centerX - playerCenterX) * (centerX - playerCenterX) + (centerY - playerCenterY) * (centerY - playerCenterY);

                    if (isBoss && distToPlayerSq < minBossDistanceSq)
                        continue;

                    int nearbyMonsters = monsters.Count(m =>
                        m.Hitbox.IntersectsWith(testHitbox) ||
                        (m.X >= centerX - TILE_SIZE && m.X <= centerX + TILE_SIZE &&
                         m.Y >= centerY - TILE_SIZE && m.Y <= centerY + TILE_SIZE)
                    );

                    if (nearbyMonsters <= 3)
                    {
                        return new PointF(testHitbox.X, testHitbox.Y);
                    }
                }
            }

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

            return new PointF(1.5f * TILE_SIZE - entityWidth / 2f, 1.5f * TILE_SIZE - entityHeight / 2f);
        }

        private void InitializeChests()
        {
            chests.Clear();
            chests.Add(new Chest(1 * TILE_SIZE + TILE_SIZE / 2f, 3 * TILE_SIZE + TILE_SIZE / 2f));
            chests.Add(new Chest((MAZE_LOGIC_WIDTH - 2) * TILE_SIZE + TILE_SIZE / 2f, 1 * TILE_SIZE + TILE_SIZE / 2f));
            chests.Add(new Chest(1 * TILE_SIZE + TILE_SIZE / 2f, (MAZE_LOGIC_HEIGHT - 2) * TILE_SIZE + TILE_SIZE / 2f));
            chests.Add(new Chest(45 * TILE_SIZE + TILE_SIZE / 2f, 27 * TILE_SIZE + TILE_SIZE / 2f));
            chests.Add(new Chest(25 * TILE_SIZE + TILE_SIZE / 2f, 15 * TILE_SIZE + TILE_SIZE / 2f));
            chests.Add(new Chest(35 * TILE_SIZE + TILE_SIZE / 2f, 7 * TILE_SIZE + TILE_SIZE / 2f));
        }

        #region Input Events

        private void frmMainGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (isDead) return;

            // THÊM: Tạm dừng game bằng Escape
            if (e.KeyCode == Keys.Escape)
            {
                PauseGame();
                return;
            }

            if (e.KeyCode == Keys.E)
            {
                CheckChestInteraction();
                return;
            }

            if (e.KeyCode == Keys.F)
            {
                UseHealthPotionFromGame();
                return; // Không làm gì khác
            }

            if (e.KeyCode == Keys.W) goUp = true;
            if (e.KeyCode == Keys.S) goDown = true;
            if (e.KeyCode == Keys.A) goLeft = true;
            if (e.KeyCode == Keys.D) goRight = true;
            if (e.KeyCode == Keys.ControlKey) isRunning = true;

            if (e.KeyCode == Keys.ShiftKey && !isDashing && dashCooldown == 0 && _currentStamina > 10)
            {
                if (_currentStamina >= 50)
                {
                    _currentStamina -= 50;
                    isDashing = true;
                    dashTimer = 10;
                    dashCooldown = 60;
                    GetDashDirection();
                }
            }
        }

        private void frmMainGame_KeyUp(object sender, KeyEventArgs e)
        {
            if (_isAutoPiloting) return;

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

        // THÊM: Hàm tạm dừng game
        private void PauseGame()
        {
            gameTimer.Stop();
            _mapTimer.Stop();

            // Mở frmMenu và truyền dữ liệu
            using (frmMenu pauseMenu = new frmMenu(_character, _inventory, _calculatedStats, _gameSessionService, _currentHealth))
            {
                DialogResult result = pauseMenu.ShowDialog();

                // Xử lý kết quả sau khi menu đóng
                if (result == DialogResult.Abort) // Người chơi chọn "Main Menu"
                {
                    this.DialogResult = DialogResult.Cancel; // Báo hiệu cho frmPlay là không thắng
                    this.Close();
                    return;
                }
                else
                {
                    // Người chơi nhấn "Resume" (hoặc đóng form)
                    // Cập nhật lại máu (phòng trường hợp dùng item)
                    _currentHealth = pauseMenu.UpdatedHealth;
                    gameTimer.Start();
                    _mapTimer.Start();
                }
            }
        }

        // --- THÊM: HÀM SỬ DỤNG THUỐC TỪ TRONG GAME ---
        /// <summary>
        /// Được gọi khi người chơi nhấn phím F
        /// </summary>
        private void UseHealthPotionFromGame()
        {
            if (isDead || _inventory == null || _gameSessionService == null) return;

            // Kiểm tra xem còn thuốc không
            if (_inventory.HealthPotionCount > 0)
            {
                // Gọi BLL để sử dụng thuốc (BLL sẽ tự kiểm tra máu đầy)
                int newHealth = _gameSessionService.UseHealthPotion(_character.CharacterID, _currentHealth);

                if (newHealth > _currentHealth) // Nếu dùng thành công (máu tăng)
                {
                    _currentHealth = newHealth; // Cập nhật máu hiện tại
                    _inventory.HealthPotionCount--; // Cập nhật số lượng local cho HUD

                    // (Tùy chọn: Thêm hiệu ứng âm thanh/hình ảnh dùng thuốc ở đây)
                }
                else
                {
                    // (Tùy chọn: Thêm âm thanh báo lỗi - ví dụ: máu đã đầy)
                }
            }
            else
            {
                // (Tùy chọn: Thêm âm thanh báo lỗi - hết thuốc)
            }
        }


        private void CheckChestInteraction()
        {
            RectangleF playerHitboxLogic = new RectangleF(playerX, playerY, playerWidth, playerHeight);

            foreach (var chest in chests.ToList())
            {
                if (!chest.IsOpened && playerHitboxLogic.IntersectsWith(chest.Hitbox))
                {
                    OpenChest(chest);
                    return;
                }
            }
        }

        private void OpenChest(Chest chest)
        {
            chest.IsOpened = true;
            chest.StartOpenAnimation();
            _chestsOpened++;

            int potionCount = 1;
            int roll = new Random().Next(1, 101);

            if (roll <= 10) potionCount = 4;
            else if (roll <= 30) potionCount = 3;
            else if (roll <= 70) potionCount = 2;
            else potionCount = 1;

            MessageBox.Show($"Bạn tìm thấy {potionCount} bình Thuốc Hồi Máu!", "Rương đã mở");

            for (int i = 0; i < potionCount; i++)
            {
                float lootX = chest.X + new Random().Next(-10, 10);
                float lootY = chest.Y + new Random().Next(-10, 10);
                lootEffects.Add(new LootEffect(lootX, lootY, chest.Width, chest.Height));
            }

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

            foreach (var monster in monsters.ToList())
            {
                monster.Update(this, playerX, playerY, spellEffects);

                if (monster.State == Monster.MonsterState.Dead && monster.deathAnim.IsFinished)
                {
                    float monsterCenterX = monster.X + monster.Width / 2;
                    float monsterCenterY = monster.Y + monster.Height / 2;

                    monsters.Remove(monster);
                    _monstersKilled++;

                    // --- SỬA: Logic Rơi Vật phẩm (Loot) ---
                    // Xóa logic LootEffect cũ:
                    // int lootCount = (monster is Boss) ? 5 : 1;
                    // for (int i = 0; i < lootCount; i++)
                    // {
                    //    lootEffects.Add(new LootEffect(monster.X, monster.Y, monster.Width, monster.Height));
                    // }

                    // THÊM: Logic Roll Loot Mới (từ RAM)
                    List<object> drops = RollLootFromRAM(monster);
                    foreach (var item in drops)
                    {
                        // Rơi vật phẩm tại vị trí quái chết, hơi ngẫu nhiên một chút
                        float dropX = monsterCenterX + _lootRandom.Next(-5, 6);
                        float dropY = monsterCenterY + _lootRandom.Next(-5, 6);
                        _groundItems.Add(new GroundItem(item, dropX, dropY));
                    }

                    if (monster is Boss)
                    {
                        _isBossDefeated = true;
                        _portal = new Portal(monsterCenterX, monsterCenterY);
                        _isExitOpen = true;
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
                    spell.Dispose();
                }
            }

            // --- THÊM LOGIC CỔNG "FRIENDLY BOSS" ---
            // Nếu Boss chưa bị đánh bại VÀ cổng chưa xuất hiện
            if (!_isBossDefeated && _portal == null)
            {
                // Tìm Boss
                var boss = monsters.OfType<Boss>().FirstOrDefault();

                // Kiểm tra xem Boss có tồn tại và đang ở trạng thái Friendly không
                // (Logic trong Boss.cs sẽ đặt trạng thái này nếu _monstersKilled == 0)
                if (boss != null && boss.State == Monster.MonsterState.Friendly)
                {
                    // Nếu Boss thân thiện, tạo cổng ngay tại chỗ Boss
                    float bossCenterX = boss.X + boss.Width / 2;
                    float bossCenterY = boss.Y + boss.Height / 2;
                    _portal = new Portal(bossCenterX, bossCenterY);
                    _isExitOpen = true; // Cho phép qua màn
                }
            }

            // Cập nhật hiệu ứng Loot
            foreach (var loot in lootEffects.ToList())
            {
                loot.Update();
                if (loot.IsFinished)
                {
                    lootEffects.Remove(loot);
                }
            }

            foreach (var loot in lootEffects.ToList())
            {
                loot.Update();
                if (loot.IsFinished)
                {
                    lootEffects.Remove(loot);
                }
            }

            UpdateGroundItems();

            foreach (var chest in chests)
            {
                chest.UpdateAnimation();
            }

            if (_portal != null && !_isExiting)
            {
                _portal.Update();
                RectangleF playerHitbox = new RectangleF(playerX, playerY, 10, 10);

                if (_portal.CheckCollision(playerHitbox))
                {
                    _isExiting = true;
                    // SỬA: Gọi hàm HandleMapExit
                    HandleMapExit("Đã qua cổng!");
                }
            }

            UpdateStaminaRegen();

            if (_isExitOpen && IsCollidingWithExit(playerX, playerY))
            {
                // SỬA: Gọi hàm HandleMapExit
                HandleMapExit("Chiến thắng!");
            }

            this.Invalidate();
        }

        /// <summary>
        /// Xử lý hồi Stamina
        /// </summary>
        private void UpdateStaminaRegen()
        {
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
            // --- THÊM: VẼ NGOẠI CẢNH ---
            // (Vẽ sau tường, nhưng trước rương và vật phẩm)
            DrawScenery(canvas);
            DrawChests(canvas);

            // --- THÊM: VẼ VẬT PHẨM TRÊN ĐẤT ---
            // (Vẽ trước Player để Player đè lên trên)
            foreach (var item in _groundItems)
            {
                // TODO: Thêm kiểm tra Fog of War nếu muốn
                item.Draw(canvas, RENDER_SCALE);
            }

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

            foreach (var loot in lootEffects)
            {
                loot.Draw(canvas, RENDER_SCALE);
            }

            if (_portal != null)
            {
                _portal.Draw(canvas, RENDER_SCALE);
            }

            // 4. VẼ HUD
            canvas.ResetTransform();
            DrawHUD(canvas);
        }

        private void DrawChests(Graphics canvas)
        {
            RectangleF viewportRect = new RectangleF(_cameraX, _cameraY, mapWidth, mapHeight);
            int chestDrawSize = 60;

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
                            attributesToUse = null;
                        }
                        else
                        {
                            attributesToUse = _darkenAttributes;
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

                canvas.DrawString($"Quái: {_monstersKilled}", _hudFont, _hudBrush, x + 10, y + 130);
                canvas.DrawString($"Rương: {_chestsOpened}", _hudFont, _hudBrush, x + 90, y + 130);

                // THÊM: Vẽ thời gian
                canvas.DrawString($"Map: {_mapLevel} | Thời gian: {_mapTimer.Elapsed:mm\\:ss}", _hudFont, _hudBrush, x + hudWidth + 20, y + 10);

                // --- THÊM: VẼ HUD THUỐC (GÓC DƯỚI BÊN PHẢI) ---
                if (potionIcon != null && _inventory != null)
                {
                    int iconSize = 48; // Kích thước icon
                    int padding = 15; // Khoảng cách tới viền
                    int iconX = mapWidth - iconSize - padding;
                    int iconY = mapHeight - iconSize - padding;

                    // 1. Vẽ Icon
                    canvas.DrawImage(potionIcon, iconX, iconY, iconSize, iconSize);

                    // 2. Vẽ số lượng
                    string potionCount = _inventory.HealthPotionCount.ToString();
                    SizeF stringSize = canvas.MeasureString(potionCount, _hudTitleFont);

                    // Vị trí văn bản (ở góc dưới bên phải của icon)
                    PointF textPos = new PointF(
                        iconX + iconSize - stringSize.Width - 5,
                        iconY + iconSize - stringSize.Height - 5
                    );

                    // Vẽ viền đen (shadow) cho dễ đọc
                    canvas.DrawString(potionCount, _hudTitleFont, Brushes.Black, textPos.X + 1, textPos.Y + 1);
                    // Vẽ chữ trắng
                    canvas.DrawString(potionCount, _hudTitleFont, _hudBrush, textPos.X, textPos.Y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi vẽ HUD: " + ex.Message);
            }
        }


        #region Logic Phụ (Va chạm, Tầm nhìn, Di chuyển...)

        private void frmMainGame_Resize(object sender, EventArgs e)
        {
            mapWidth = this.ClientSize.Width;
            mapHeight = this.ClientSize.Height;
            this.Invalidate();
        }

        private bool IsCollidingWithExit(float pX, float pY)
        {
            int currentLogicHeight = _mazeGrid.GetLength(0);
            int currentLogicWidth = _mazeGrid.GetLength(1);

            for (int y = 0; y < currentLogicHeight; y++)
            {
                for (int x = 0; x < currentLogicWidth; x++)
                {
                    if (_mazeGrid[y, x] == (int)GameObjectType.Exit)
                    {
                        RectangleF exitRect = new RectangleF(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                        RectangleF playerHitbox = new RectangleF(pX, pY, playerWidth, playerHeight);
                        if (playerHitbox.IntersectsWith(exitRect))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
                    // SỬA: Lấy sát thương từ chỉ số đã tính
                    int playerDamage = _calculatedStats.TotalAttack;
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
                                        (int)(x * TILE_SIZE * RENDER_SCALE),
                                        (int)(y * TILE_SIZE * RENDER_SCALE),
                                        (int)(TILE_SIZE * RENDER_SCALE),
                                        (int)(TILE_SIZE * RENDER_SCALE)
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

        // --- THÊM: HÀM VẼ NGOẠI CẢNH ---
        private void DrawScenery(Graphics canvas)
        {
            if (_sceneryImages_RAM.Count == 0) return;

            foreach (var scenery in _sceneryObjects)
            {
                // Lấy vị trí Tile của vật thể
                int tileX = (int)Math.Floor(scenery.X / TILE_SIZE);
                int tileY = (int)Math.Floor(scenery.Y / TILE_SIZE);

                // Kiểm tra an toàn
                if (tileY >= 0 && tileY < MAZE_LOGIC_HEIGHT &&
                    tileX >= 0 && tileX < MAZE_LOGIC_WIDTH)
                {
                    // Lấy vị trí Tile của Player
                    int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
                    int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);

                    // Kiểm tra tầm nhìn (Fog of War)
                    bool isCurrentlyVisible = Math.Pow(tileX - playerTileX, 2) + Math.Pow(tileY - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2);

                    ImageAttributes attributesToUse = null;

                    if (_isTileVisible[tileY, tileX]) // Đã từng thấy
                    {
                        attributesToUse = _darkenAttributes; // Dùng ảnh mờ
                    }
                    if (isCurrentlyVisible) // Đang thấy
                    {
                        attributesToUse = null; // Dùng ảnh rõ
                    }

                    // Chỉ vẽ nếu đã thấy hoặc đang thấy
                    if (attributesToUse != null || _isTileVisible[tileY, tileX])
                    {
                        scenery.Draw(canvas, RENDER_SCALE, attributesToUse);
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

            float playerScaledCenterX = playerScaledX + playerWidth * RENDER_SCALE / 2;
            float playerScaledCenterY = playerScaledY + playerHeight * RENDER_SCALE / 2;


            float deltaX = mouseXOnMap - playerScaledCenterX;
            float deltaY = mouseYOnMap - playerScaledCenterY;

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
            if (_isAutoPiloting)
            {
                if (Math.Pow(_autoPilotTarget.X - (playerX + playerWidth / 2f), 2) + Math.Pow(_autoPilotTarget.Y - (playerY + playerHeight / 2f), 2) < (0.5f * TILE_SIZE) * (0.5f * TILE_SIZE))
                {
                    SetNewAutoPilotTarget();
                }

                float dxTarget = _autoPilotTarget.X - (playerX + playerWidth / 2f);
                float dyTarget = _autoPilotTarget.Y - (playerY + playerHeight / 2f);

                goUp = dyTarget < 0;
                goDown = dyTarget > 0;
                goLeft = dxTarget < 0;
                goRight = dxTarget > 0;
            }

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

                    int currentSpeed = isRunning && _currentStamina > 0 ? runSpeed : walkSpeed;
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

            if (futureHitboxScaled.Left < 0) return true;
            if (futureHitboxScaled.Top < 0) return true;
            if (futureHitboxScaled.Bottom > _mapLogicalHeightScaled) return true;
            if (futureHitboxScaled.Right > _mapLogicalWidthScaled) return true;

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


        // SỬA: Hàm xử lý qua màn
        private void HandleMapExit(string message)
        {
            // Dừng game và thời gian TRƯỚC TIÊN
            gameTimer.Stop();
            _mapTimer.Stop();
            _isExiting = true; // Đặt cờ để ngăn gọi lại

            // 1. Lưu điểm vào Bảng Xếp Hạng
            try
            {
                int timeInSeconds = (int)_mapTimer.Elapsed.TotalSeconds;
                _leaderboardService.AddAdventureScore(
                    _character.UserID,
                    _character.CharacterName,
                    timeInSeconds,
                    _monstersKilled,
                    _mapLevel
                );
            }
            catch (Exception ex)
            {
                // Không làm dừng game nếu lưu điểm thất bại, chỉ thông báo
                Console.WriteLine("Lỗi lưu bảng xếp hạng: " + ex.Message);
                MessageBox.Show("Lỗi khi lưu điểm: " + ex.Message, "Lỗi BXH");
            }

            // 2. Thông báo cho người chơi
            MessageBox.Show($"Đã hoàn thành Map {_mapLevel}! ({message})\nThời gian: {_mapTimer.Elapsed:mm\\:ss}\nQuái đã diệt: {_monstersKilled}", "Qua Màn!");

            // 3. Gửi tín hiệu "Thắng" (OK) về cho frmPlay
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateAnimation()
        {
            bool isMoving = goUp || goDown || goLeft || goRight;
            string direction = lastFacingDirection;

            if (isDead)
            {
                playerImage = deathActivity.GetNextFrame(direction);
                // THÊM: Logic Game Over
                if (deathActivity.IsFinished && gameTimer.Enabled) // Chỉ chạy 1 lần
                {
                    gameTimer.Stop();
                    _mapTimer.Stop();
                    MessageBox.Show("Bạn đã gục ngã...", "Game Over");
                    this.DialogResult = DialogResult.Cancel; // Báo hiệu thua
                    this.Close();
                }
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

        // --- THÊM: CÁC HÀM XỬ LÝ LOOT (VẬT PHẨM) ---

        /// <summary>
        /// Quyết định xem quái vật rơi ra vật phẩm gì (Sử dụng dữ liệu RAM)
        /// </summary>
        private List<object> RollLootFromRAM(Monster monster)
        {
            List<object> drops = new List<object>();
            if (_monsterLootTables_RAM == null) return drops;

            // 1. Xác định ID của quái
            // (Dựa trên MonsterName trong CSDL)
            int monsterId = 0;
            if (monster is Slime) monsterId = 1; // "Slime"
            else if (monster is Orc) monsterId = 2; // "Orc"
            else if (monster is Boss) monsterId = 3; // "Boss-Maze"
            else return drops; // Quái không xác định

            // 2. Roll cho Giáp (ItemType == 1)
            var armorRules = _monsterLootTables_RAM
                .Where(r => r.MonsterID == monsterId && r.ItemType == 1)
                .ToList();
            var armorDrop = GetDropFromRules(armorRules, 1);
            if (armorDrop != null) drops.Add(armorDrop);

            // 3. Roll cho Vũ khí (ItemType == 2)
            var weaponRules = _monsterLootTables_RAM
                .Where(r => r.MonsterID == monsterId && r.ItemType == 2)
                .ToList();
            var weaponDrop = GetDropFromRules(weaponRules, 2);
            if (weaponDrop != null) drops.Add(weaponDrop);

            return drops;
        }

        /// <summary>
        /// Helper: Quay số dựa trên 1 bảng quy tắc (ví dụ: tất cả quy tắc của Giáp)
        /// </summary>
        private object GetDropFromRules(List<MonsterLootTables> rules, int itemType)
        {
            if (rules.Count == 0) return null;

            // Tổng tỉ lệ (ví dụ: 60% + 30% + 10% = 100% (1.0))
            double totalChance = rules.Sum(r => r.DropChance);
            // Roll từ 0 đến tổng tỉ lệ
            double roll = _lootRandom.NextDouble() * totalChance;
            double cumulative = 0.0;

            foreach (var rule in rules.OrderBy(r => r.ItemRank)) //
            {
                cumulative += rule.DropChance;
                if (roll < cumulative)
                {
                    // Trúng! Lấy item từ RAM
                    if (itemType == 1) // Giáp
                        return _allArmors_RAM.FirstOrDefault(a => a.ArmorRank == rule.ItemRank);
                    if (itemType == 2) // Vũ khí
                        return _allWeapons_RAM.FirstOrDefault(w => w.WeaponRank == rule.ItemRank);

                    return null;
                }
            }
            return null; // Không trúng (nếu tổng tỉ lệ < 1.0)
        }

        /// <summary>
        /// Cập nhật các vật phẩm trên đất và kiểm tra va chạm để nhặt
        /// </summary>
        private void UpdateGroundItems()
        {
            // Hitbox logic của người chơi
            RectangleF playerHitbox = new RectangleF(playerX, playerY, playerWidth, playerHeight);

            foreach (var item in _groundItems.ToList())
            {
                item.Update(); // Cập nhật hiệu ứng (ví dụ: nhấp nhô)

                // Kiểm tra va chạm
                if (playerHitbox.IntersectsWith(item.Hitbox))
                {
                    HandleItemPickup(item); // Xử lý nhặt
                    item.IsMarkedForDeletion = true; // Đánh dấu xóa
                }
            }

            // Xóa các item đã nhặt
            _groundItems.RemoveAll(item => item.IsMarkedForDeletion);
        }

        /// <summary>
        /// Xử lý logic khi nhặt một vật phẩm
        /// </summary>
        private void HandleItemPickup(GroundItem item)
        {
            // (Âm thanh nhặt đồ nên được thêm ở đây)

            if (item.ItemData is Weapons newWeapon)
            {
                // Lấy vũ khí hiện tại (đã được BLL tải vào RAM)
                var currentWeapon = _inventory.Weapons;

                // So sánh chỉ số
                if (newWeapon.AttackBonus > currentWeapon.AttackBonus)
                {
                    // Trang bị vũ khí mới TỐT HƠN
                    _gameSessionService.EquipWeapon(_character.CharacterID, newWeapon.WeaponID);

                    // Cập nhật bản sao trong RAM (cho HUD và lần so sánh tiếp theo)
                    _inventory.EquippedWeaponID = newWeapon.WeaponID;
                    _inventory.Weapons = newWeapon; // Quan trọng: Cập nhật tham chiếu
                    UpdateCalculatedStats(); // Tính lại chỉ số cho HUD

                    // (Thêm thông báo cho người chơi)
                }
            }
            else if (item.ItemData is Armors newArmor)
            {
                // Lấy giáp hiện tại
                var currentArmor = _inventory.Armors;

                // So sánh chỉ số
                if (newArmor.DefensePoints > currentArmor.DefensePoints)
                {
                    // Trang bị giáp mới TỐT HƠN
                    _gameSessionService.EquipArmor(_character.CharacterID, newArmor.ArmorID);

                    // Cập nhật bản sao trong RAM
                    _inventory.EquippedArmorID = newArmor.ArmorID;
                    _inventory.Armors = newArmor;
                    UpdateCalculatedStats();

                    // (Thêm thông báo cho người chơi)
                }
            }
            // (Logic nhặt thuốc có thể thêm ở đây nếu quái rơi thuốc)
        }

        private void SetNewAutoPilotTarget()
        {
            Random rand = new Random();
            int patrolRange = 5 * TILE_SIZE;

            float randomAngle = (float)(rand.NextDouble() * 2 * Math.PI);
            float randomDist = (float)(rand.NextDouble() * patrolRange);

            float targetX = (playerX + playerWidth / 2f) + (float)Math.Cos(randomAngle) * randomDist;
            float targetY = (playerY + playerHeight / 2f) + (float)Math.Sin(randomAngle) * randomDist;

            float mapLogicWidth = MAZE_LOGIC_WIDTH * TILE_SIZE;
            float mapLogicHeight = MAZE_LOGIC_HEIGHT * TILE_SIZE;

            targetX = Math.Max(TILE_SIZE, Math.Min(mapLogicWidth - TILE_SIZE, targetX));
            targetY = Math.Max(TILE_SIZE, Math.Min(mapLogicHeight - TILE_SIZE, targetY));

            _autoPilotTarget = new PointF(targetX, targetY);
        }

        public class LootEffect
        {
            private float X, Y;
            private int Width, Height;
            private float opacity = 1.0f;
            private int duration = 60;
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


