using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D; // Thêm để dùng TextureBrush
using System.Windows.Forms;
using Main.Tri;

namespace Main.Map
{
    public partial class Water : Form, IGameMap
    {
        // ... (Các biến _mainMenuForm, _pauseMenu, player, monsters, boss... giữ nguyên) ...
        private frmMain _mainMenuForm;
        private frmMenu _pauseMenu;
        private bool _isGamePaused = false;

        private Player player;
        private List<Monster> monsters = new List<Monster>();
        private Boss boss;

        // --- THÊM MỚI: Vật phẩm và Đồ họa ---
        private List<CollectibleItem> itemsOnMap = new List<CollectibleItem>();
        private TextureBrush wallBrush;
        private TextureBrush floorBrush;
        private List<Particle> particles = new List<Particle>(); // Cho hiệu ứng bong bóng
        // ------------------------------------

        private MazeGenerator mazeGen;
        private List<RectangleF> wallBounds = new List<RectangleF>();
        private List<PointF> floorTiles = new List<PointF>();
        private const int MAZE_WIDTH = 25;
        private const int MAZE_HEIGHT = 15;
        private int tileSize = (int)GameObjectType.TileSize;

        // XÓA: fallDelay không còn dùng nữa
        // private int fallDelay = 500;
        private bool isRaining = false;
        private Random rand = new Random();

        // Kích thước hitbox cố định của Player
        private const int PLAYER_HITBOX_SIZE = 28;


        public Player Player { get { return player; } }
        // FIX CS0108: Add 'new' keyword
        public new string Name => this.Text; // Implement Name property

        public Water(frmMain mainMenuForm)
        {
            InitializeComponent();
            _mainMenuForm = mainMenuForm;
            this.DoubleBuffered = true;
            this.Text = "Map Thủy: Hàn Thủy Vực";

            // --- THÊM MỚI: Khởi tạo họa tiết ---
            InitializeTextures();
            // ----------------------------------

            mazeGen = new MazeGenerator(MAZE_WIDTH, MAZE_HEIGHT);
            mazeGen.GenerateMaze();
            InitializeWallsAndItems(); // Đổi tên hàm

            PointF startPos = GetPlayerSpawnPoint();
            // Sử dụng kích thước cố định khi tạo Player
            player = new Player(startPos.X, startPos.Y, PLAYER_HITBOX_SIZE);
            // BẮT BUỘC: Gọi SetUpAnimations sau khi khởi tạo Player
            player.SetUpAnimations();


            monsters.Add(new Monster(MonsterType.Water, GetRandomFloorPosition()));
            monsters.Add(new Monster(MonsterType.Water, GetRandomFloorPosition()));
            boss = new Boss(MonsterType.Water, GetRandomFloorPosition(true));

            // XÓA: timerFall không còn dùng nữa
            // this.timerFall.Interval = fallDelay;
            this.timerGameLoop.Start();
            this.timerRain.Start();
        }

        // --- HÀM MỚI: Tạo Họa tiết (Texture) ---
        private void InitializeTextures()
        {
            // Tạo texture cho tường (vảy cá xanh)
            Bitmap wallTexture = new Bitmap(tileSize, tileSize);
            using (Graphics g = Graphics.FromImage(wallTexture))
            {
                g.Clear(Color.FromArgb(20, 40, 90)); // Nền xanh đậm
                for (int i = 0; i < tileSize; i += 8)
                {
                    g.DrawArc(Pens.Cyan, -5, i, 15, 15, 30, 120);
                    g.DrawArc(Pens.Cyan, 11, i + 4, 15, 15, 30, 120);
                }
            }
            wallBrush = new TextureBrush(wallTexture);

            // Tạo texture cho sàn (nước gợn sóng)
            Bitmap floorTexture = new Bitmap(tileSize, tileSize);
            using (Graphics g = Graphics.FromImage(floorTexture))
            {
                g.Clear(Color.FromArgb(40, 60, 120)); // Nền xanh
                g.DrawLine(Pens.LightBlue, 5, 5, 27, 10);
                g.DrawLine(Pens.LightBlue, 5, 20, 27, 15);
            }
            floorBrush = new TextureBrush(floorTexture);
        }

        // Đổi tên: Khởi tạo tường VÀ vật phẩm
        private void InitializeWallsAndItems()
        {
            wallBounds.Clear();
            floorTiles.Clear();
            itemsOnMap.Clear();

            for (int y = 0; y < mazeGen.Height; y++)
            {
                for (int x = 0; x < mazeGen.Width; x++)
                {
                    if (mazeGen.Maze[y, x] == (int)GameObjectType.Wall)
                    {
                        wallBounds.Add(new RectangleF(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                    else
                    {
                        floorTiles.Add(new PointF(x * tileSize, y * tileSize));
                    }
                }
            }

            // --- THÊM MỚI: Rải vật phẩm ra map ---
            SpawnItem(ItemType.HealthPotion);
            SpawnItem(ItemType.HealthPotion);
            SpawnItem(ItemType.DefensePotion);
            // -------------------------------------
        }

        // Hàm rải vật phẩm
        private void SpawnItem(ItemType type)
        {
            PointF pos = GetRandomFloorPosition();
            itemsOnMap.Add(new CollectibleItem(type, pos));
        }

        // ... (Hàm GetRandomFloorPosition giữ nguyên) ...
        private PointF GetRandomFloorPosition(bool nearEnd = false)
        {
            if (floorTiles.Count == 0) return new PointF(tileSize, tileSize);
            List<PointF> validTiles;
            if (nearEnd)
            {
                float midX = (MAZE_WIDTH * tileSize) / 2;
                float midY = (MAZE_HEIGHT * tileSize) / 2;
                validTiles = floorTiles.FindAll(p => p.X >= midX && p.Y >= midY);
            }
            else
            {
                float midX = (MAZE_WIDTH * tileSize) / 2;
                float midY = (MAZE_HEIGHT * tileSize) / 2;
                validTiles = floorTiles.FindAll(p => p.X < midX && p.Y < midY);
            }
            if (validTiles.Count == 0) validTiles = floorTiles;
            return validTiles[rand.Next(validTiles.Count)];
        }


        // Timer chính (ĐÃ CẬP NHẬT)
        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            if (_isGamePaused) return;

            // --- THÊM MỚI: Cập nhật hiệu ứng hạt ---
            UpdateParticles();
            // -------------------------------------

            player.Update();
            PointF playerMove = player.CalculateMovementVector(player.Position);
            PointF nextPlayerPos = new PointF(player.X + playerMove.X, player.Y + playerMove.Y);
            player.Position = GetAllowedMovement(player.BoundingBox, nextPlayerPos);


            foreach (var monster in monsters)
            {
                monster.Update();
                PointF monsterMove = monster.CalculateMovementVector(player.Position);
                PointF nextMonsterPos = new PointF(monster.X + monsterMove.X, monster.Y + monsterMove.Y);
                monster.Position = GetAllowedMovement(monster.BoundingBox, nextMonsterPos);
                // TODO: Xử lý quái tấn công Player
            }
            boss.Update();
            PointF bossMove = boss.CalculateMovementVector(player.Position);
            PointF nextBossPos = new PointF(boss.X + bossMove.X, boss.Y + bossMove.Y);
            boss.Position = GetAllowedMovement(boss.BoundingBox, nextBossPos);

            // --- THÊM MỚI: Xử lý nhặt vật phẩm ---
            for (int i = itemsOnMap.Count - 1; i >= 0; i--)
            {
                if (player.BoundingBox.IntersectsWith(itemsOnMap[i].BoundingBox))
                {
                    player.AddItem(itemsOnMap[i].Item);
                    itemsOnMap.RemoveAt(i);
                    // TODO: Thêm âm thanh nhặt đồ
                }
            }
            // -----------------------------------

            // XÓA: Logic ngã khi lướt trong mưa (đã chuyển vào Player.cs)
            /*
            if (isRaining && player.IsDashing && player.DashCount > 3)
            {
                player.IsFallen = true;
                this.timerFall.Start();
            }
            */

            this.Invalidate();
        }

        // Sửa hàm GetAllowedMovement để nhận vị trí MỚI thay vì vector
        private PointF GetAllowedMovement(RectangleF currentBounds, PointF nextPosition)
        {
            float currentX = currentBounds.X;
            float currentY = currentBounds.Y;
            float nextX = nextPosition.X;
            float nextY = nextPosition.Y;
            float dx = nextX - currentX;
            float dy = nextY - currentY;


            RectangleF nextXBounds = currentBounds;
            nextXBounds.X = nextX; // Di chuyển theo X trước

            // FIX CS0219: Remove unused variable collisionX
            // bool collisionX = false;
            foreach (var wall in wallBounds)
            {
                if (nextXBounds.IntersectsWith(wall))
                {
                    // collisionX = true; // No longer needed
                    // Điều chỉnh vị trí X để vừa chạm tường
                    if (dx > 0) nextX = wall.Left - currentBounds.Width;
                    else if (dx < 0) nextX = wall.Right;
                    break;
                }
            }


            RectangleF nextYBounds = currentBounds;
            nextYBounds.X = nextX; // Dùng X đã điều chỉnh (nếu có)
            nextYBounds.Y = nextY; // Di chuyển Y sau

            // FIX CS0219: Remove unused variable collisionY
            // bool collisionY = false;
            foreach (var wall in wallBounds)
            {
                if (nextYBounds.IntersectsWith(wall))
                {
                    // collisionY = true; // No longer needed
                    // Điều chỉnh vị trí Y để vừa chạm tường
                    if (dy > 0) nextY = wall.Top - currentBounds.Height;
                    else if (dy < 0) nextY = wall.Bottom;
                    break;
                }
            }

            return new PointF(nextX, nextY); // Trả về vị trí cuối cùng hợp lệ
        }

        // Timer mưa
        private void timerRain_Tick(object sender, EventArgs e)
        {
            isRaining = !isRaining;
            if (isRaining)
            {
                player.DashCount = 0;
            }
        }

        // XÓA: timerFall_Tick không còn dùng nữa
        /*
        private void timerFall_Tick(object sender, EventArgs e)
        {
            // player.StopDash(); // Hàm này không còn nữa
            player.IsFallen = false;
            this.timerFall.Stop();
        }
        */

        // Xử lý vẽ (ĐÃ CẬP NHẬT)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Vẽ mượt hơn
            g.Clear(Color.DarkBlue);

            // --- THAY ĐỔI: Vẽ Sàn bằng Họa tiết ---
            g.FillRectangle(floorBrush, 0, 0, this.Width, this.Height);

            // --- THAY ĐỔI: Vẽ Tường bằng Họa tiết ---
            foreach (var wall in wallBounds)
            {
                g.FillRectangle(wallBrush, wall);
            }

            // --- THÊM MỚI: Vẽ Hiệu ứng (Bong bóng) ---
            DrawParticles(g);
            // ----------------------------------------

            // Vẽ mưa
            if (isRaining)
            {
                using (Pen rainPen = new Pen(Color.Cyan, 1)) // Dùng using để giải phóng Pen
                {
                    for (int i = 0; i < 50; i++)
                    {
                        int x = rand.Next(this.Width);
                        int y = rand.Next(this.Height);
                        g.DrawLine(rainPen, x, y, x + 1, y + 5);
                    }
                }
            }

            // --- THÊM MỚI: Vẽ Vật phẩm ---
            foreach (var item in itemsOnMap)
            {
                item.Draw(g);
            }
            // -----------------------------

            // Vẽ các đối tượng
            player.Draw(g);
            foreach (var monster in monsters)
            {
                monster.Draw(g);
            }
            boss.Draw(g);
        }

        // --- XỬ LÝ NHẤN PHÍM (ĐÃ SỬA) ---
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (_isGamePaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
                return true;
            }

            if (_isGamePaused) return base.ProcessCmdKey(ref msg, keyData);
            // XÓA: Kiểm tra IsFallen
            // if (player.IsFallen) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                // Di chuyển
                case Keys.A: player.MoveLeft = true; return true;
                case Keys.D: player.MoveRight = true; return true;
                case Keys.W: player.MoveUp = true; return true;
                case Keys.S: player.MoveDown = true; return true;

                // Hành động
                // SỬA: Set cờ input thay vì gọi hàm
                case Keys.ShiftKey: player.AttemptDashInput = true; return true;
                case Keys.Space: player.AttemptAttackInput = true; return true;
                case Keys.ControlKey: player.AttemptRun = true; return true; // Chạy

                    //case Keys.E: player.IsBlocking = true; return true; // Đỡ đòn (nếu cần)

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // --- XỬ LÝ NHẢ PHÍM (ĐÃ SỬA) ---
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_isGamePaused) return;

            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                // Di chuyển
                case Keys.A: player.MoveLeft = false; break;
                case Keys.D: player.MoveRight = false; break;
                case Keys.W: player.MoveUp = false; break;
                case Keys.S: player.MoveDown = false; break;

                // Hành động
                case Keys.ControlKey: player.AttemptRun = false; break; // Ngừng chạy
                                                                        //case Keys.ShiftKey: // Không cần làm gì khi nhả Shift nữa
                                                                        //case Keys.E: player.IsBlocking = false; break; // Ngừng đỡ đòn
            }
        }
        private void Water_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        // ... (Các hàm IGameMap: GetCurrentGameState, LoadGameState, PauseGame, ResumeGame giữ nguyên) ...

        // --- SỬA GetPlayerSpawnPoint ---
        public PointF GetPlayerSpawnPoint()
        {
            // Đảm bảo trả về tọa độ pixel, không phải grid
            if (mazeGen != null)
            {
                // SỬA: Sử dụng hằng số PLAYER_HITBOX_SIZE thay vì player.Width/Height
                return new PointF(
                    mazeGen.StartGridPosition.X * tileSize + (tileSize - PLAYER_HITBOX_SIZE) / 2, // Căn giữa ô
                    mazeGen.StartGridPosition.Y * tileSize + (tileSize - PLAYER_HITBOX_SIZE) / 2 // Căn giữa ô
                );
            }
            return new PointF(tileSize, tileSize); // Fallback
        }
        public GameState GetCurrentGameState()
        {
            // Kiểm tra player null trước khi truy cập thuộc tính
            if (player == null) return null; // Hoặc trả về một GameState mặc định

            return new GameState
            {
                MapName = this.Name, // Sử dụng Name của Form
                PlayerX = player.X,
                PlayerY = player.Y,
                PlayerHealth = player.CurrentHealth,
                PlayerStamina = player.CurrentStamina,
                Inventory = player.Inventory // Thêm Inventory vào save
            };
        }
        public void LoadGameState(GameState state)
        {
            // Kiểm tra player null trước khi load
            if (player == null)
            {
                MessageBox.Show("Lỗi: Người chơi chưa được khởi tạo để load game.");
                this.Close();
                return;
            }

            if (state == null)
            {
                MessageBox.Show("Lỗi: Dữ liệu save không hợp lệ.");
                return;
            }

            if (state.MapName != this.Name)
            {
                MessageBox.Show($"Lỗi: Không thể tải save của map '{state.MapName}' lên map '{this.Name}'.");
                this.Close(); // Đóng map hiện tại nếu load sai
                return;
            }

            player.Position = new PointF(state.PlayerX, state.PlayerY);
            player.CurrentHealth = state.PlayerHealth;
            player.CurrentStamina = state.PlayerStamina;
            // Tải Inventory
            if (state.Inventory != null)
            {
                player.Inventory.Clear(); // Xóa đồ cũ trước khi load
                foreach (var item in state.Inventory)
                {
                    // Hàm AddItem giờ chỉ cần ItemType và số lượng
                    player.AddItem(item.Key, item.Value);
                }
            }


            MessageBox.Show("Tải game thành công!");
            ResumeGame(); // Đảm bảo game chạy sau khi load
        }
        public void PauseGame()
        {
            _isGamePaused = true;
            timerGameLoop.Stop(); // Dừng timer chính khi pause
            if (_pauseMenu == null || _pauseMenu.IsDisposed)
            {
                // Kiểm tra player null trước khi truyền vào frmMenu
                if (Player == null)
                {
                    MessageBox.Show("Lỗi: Không thể mở menu vì Player chưa được khởi tạo.");
                    _isGamePaused = false; // Bỏ pause
                    timerGameLoop.Start(); // Chạy lại timer
                    return;
                }
                _pauseMenu = new frmMenu(_mainMenuForm, this);
            }
            _pauseMenu.Show(this); // Hiển thị menu pause
        }
        public void ResumeGame()
        {
            _isGamePaused = false;
            timerGameLoop.Start(); // Chạy lại timer chính khi resume
            if (_pauseMenu != null && !_pauseMenu.IsDisposed)
            {
                _pauseMenu.Close(); // Đóng menu pause
            }
            this.Focus(); // Lấy lại focus cho form map để nhận input
        }


        // --- LỚP PARTICLE (HIỆU ỨNG) ---
        private class Particle
        {
            public PointF Position;
            public float VelocityY;
            public int Lifetime;
            public float Size;
            public Color Color;

            public Particle(PointF pos, float velY, int life, float size, Color color)
            {
                Position = pos;
                VelocityY = velY;
                Lifetime = life;
                Size = size;
                Color = color;
            }

            public bool Update()
            {
                Position.Y += VelocityY;
                Lifetime--;
                return Lifetime <= 0;
            }
        }

        // Cập nhật và tạo hạt
        private void UpdateParticles()
        {
            // Tạo bong bóng mới
            if (rand.Next(10) == 0) // Tần suất
            {
                float x = rand.Next(this.Width);
                float y = rand.Next(this.Height / 2, this.Height); // Bắt đầu từ nửa dưới
                float velY = -rand.Next(1, 4) * 0.5f; // Đi lên chậm
                int lifetime = rand.Next(60, 120);
                float size = rand.Next(2, 6);
                particles.Add(new Particle(new PointF(x, y), velY, lifetime, size, Color.FromArgb(100, 173, 216, 230))); // Màu xanh mờ
            }

            // Cập nhật các hạt
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].Update())
                {
                    particles.RemoveAt(i);
                }
            }
        }

        // Vẽ hạt
        private void DrawParticles(Graphics g)
        {
            using (SolidBrush b = new SolidBrush(Color.White)) // Dùng 1 brush
            {
                foreach (var p in particles)
                {
                    b.Color = p.Color;
                    g.FillEllipse(b, p.Position.X, p.Position.Y, p.Size, p.Size);
                }
            }
        }
    }
}

