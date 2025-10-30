using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D; // Thêm
using System.Windows.Forms;
using Main.Tri;

namespace Main.Map
{
    public partial class Flame : Form, IGameMap
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
        private List<Particle> particles = new List<Particle>(); // Cho hiệu ứng tro/lửa
        // ------------------------------------

        private MazeGenerator mazeGen;
        private List<RectangleF> wallBounds = new List<RectangleF>();
        private List<PointF> floorTiles = new List<PointF>();
        private const int MAZE_WIDTH = 25;
        private const int MAZE_HEIGHT = 15;
        private int tileSize = (int)GameObjectType.TileSize;

        private List<Rectangle> unstableGrounds = new List<Rectangle>();
        private bool isSmoky = false;
        private Random rand = new Random();

        // Kích thước hitbox cố định của Player (Thêm vào)
        private const int PLAYER_HITBOX_SIZE = 28;

        public Player Player { get { return player; } }
        // FIX CS0108: Add 'new' keyword
        public new string Name => this.Text; // Implement Name property

        public Flame(frmMain mainMenuForm)
        {
            InitializeComponent();
            _mainMenuForm = mainMenuForm;
            this.DoubleBuffered = true;
            this.Text = "Map Hỏa: Diệm Ngục";

            // --- THÊM MỚI: Khởi tạo họa tiết ---
            InitializeTextures();
            // ----------------------------------

            mazeGen = new MazeGenerator(MAZE_WIDTH, MAZE_HEIGHT);
            mazeGen.GenerateMaze();
            InitializeWallsAndItems(); // Đổi tên

            PointF startPos = GetPlayerSpawnPoint();
            // Sử dụng kích thước cố định
            player = new Player(startPos.X, startPos.Y, PLAYER_HITBOX_SIZE);
            player.SetUpAnimations(); // Gọi sau khi tạo player

            monsters.Add(new Monster(MonsterType.Flame, GetRandomFloorPosition()));
            boss = new Boss(MonsterType.Flame, GetRandomFloorPosition(true));

            for (int i = 0; i < 5; i++)
            {
                PointF groundPos = GetRandomFloorPosition();
                unstableGrounds.Add(new Rectangle((int)groundPos.X, (int)groundPos.Y, tileSize * 2, tileSize * 2));
            }

            this.timerGameLoop.Start();
            this.timerLava.Start();
            this.timerSweat.Start();
        }

        #region Maze Logic & Visuals

        // --- HÀM MỚI: Tạo Họa tiết (Texture) ---
        private void InitializeTextures()
        {
            // Tạo texture cho tường (gạch địa ngục)
            Bitmap wallTexture = new Bitmap(tileSize, tileSize);
            using (Graphics g = Graphics.FromImage(wallTexture))
            {
                g.Clear(Color.FromArgb(50, 20, 20)); // Nền đỏ sẫm
                using (Pen p = new Pen(Color.FromArgb(100, 10, 10), 2))
                {
                    g.DrawRectangle(p, 1, 1, tileSize - 2, tileSize - 2);
                    g.DrawLine(p, tileSize / 2, 0, tileSize / 2, tileSize);
                    g.DrawLine(p, 0, tileSize / 2, tileSize, tileSize / 2);
                }
            }
            wallBrush = new TextureBrush(wallTexture);

            // Tạo texture cho sàn (đá nứt)
            Bitmap floorTexture = new Bitmap(tileSize, tileSize);
            using (Graphics g = Graphics.FromImage(floorTexture))
            {
                g.Clear(Color.FromArgb(80, 40, 40)); // Nền đỏ
                g.DrawLine(Pens.OrangeRed, 0, 0, tileSize, tileSize);
                g.DrawLine(Pens.OrangeRed, tileSize, 0, 0, tileSize);
                g.DrawLine(Pens.Yellow, 10, 0, 5, tileSize);
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
            SpawnItem(ItemType.AttackPotion);
            SpawnItem(ItemType.CoolingWater); // Vật phẩm đặc biệt
            SpawnItem(ItemType.CoolingWater);
            // -------------------------------------
        }

        // Hàm rải vật phẩm
        private void SpawnItem(ItemType type)
        {
            PointF pos = GetRandomFloorPosition();
            itemsOnMap.Add(new CollectibleItem(type, pos));
        }

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

        // Sửa hàm GetAllowedMovement
        private PointF GetAllowedMovement(RectangleF currentBounds, PointF nextPosition)
        {
            float currentX = currentBounds.X;
            float currentY = currentBounds.Y;
            float nextX = nextPosition.X;
            float nextY = nextPosition.Y;
            float dx = nextX - currentX;
            float dy = nextY - currentY;

            RectangleF nextXBounds = currentBounds;
            nextXBounds.X = nextX;

            // FIX CS0219: Remove unused collisionX
            // bool collisionX = false;
            foreach (var wall in wallBounds)
            {
                if (nextXBounds.IntersectsWith(wall))
                {
                    // collisionX = true;
                    if (dx > 0) nextX = wall.Left - currentBounds.Width;
                    else if (dx < 0) nextX = wall.Right;
                    break;
                }
            }

            RectangleF nextYBounds = currentBounds;
            nextYBounds.X = nextX;
            nextYBounds.Y = nextY;

            // FIX CS0219: Remove unused collisionY
            // bool collisionY = false;
            foreach (var wall in wallBounds)
            {
                if (nextYBounds.IntersectsWith(wall))
                {
                    // collisionY = true;
                    if (dy > 0) nextY = wall.Top - currentBounds.Height;
                    else if (dy < 0) nextY = wall.Bottom;
                    break;
                }
            }

            return new PointF(nextX, nextY);
        }
        #endregion

        // Game loop (ĐÃ CẬP NHẬT)
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
                }
            }
            // -----------------------------------

            foreach (var ground in unstableGrounds)
            {
                if (player.BoundingBox.IntersectsWith(ground))
                {
                    // (Logic sụt lún - Có thể thêm ở đây nếu cần)
                }
            }

            this.Invalidate();
        }

        // Timer sụt lún/khói
        private void timerLava_Tick(object sender, EventArgs e)
        {
            isSmoky = !isSmoky;
        }

        // Timer đổ mồ hôi (ĐÃ CẬP NHẬT)
        private void timerSweat_Tick(object sender, EventArgs e)
        {
            if (_isGamePaused || player == null) return; // Thêm kiểm tra null

            // Nếu không có buff giải nhiệt
            if (!player.ActiveBuffs.ContainsKey(BuffType.Cooling))
            {
                if (player.CurrentStamina > 0)
                {
                    float staminaLoss = player.MaxStamina * 0.10f;
                    player.CurrentStamina = Math.Max(0, player.CurrentStamina - staminaLoss);
                }
            }
            // Nếu có buff giải nhiệt, stamina sẽ tự hồi trong Player.Update()
        }

        // Vẽ (ĐÃ CẬP NHẬT)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Thêm
            g.Clear(Color.DarkRed);

            // --- THAY ĐỔI: Vẽ Sàn bằng Họa tiết ---
            g.FillRectangle(floorBrush, 0, 0, this.Width, this.Height);

            // --- THAY ĐỔI: Vẽ Tường bằng Họa tiết ---
            foreach (var wall in wallBounds)
            {
                g.FillRectangle(wallBrush, wall);
            }

            // Vẽ vùng sụt lún
            foreach (var ground in unstableGrounds)
            {
                g.FillRectangle(Brushes.Orange, ground);
            }

            // --- THÊM MỚI: Vẽ Hiệu ứng (Tro lửa) ---
            DrawParticles(g);
            // ---------------------------------------

            // Vẽ khói
            if (isSmoky)
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, Color.Gray)))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }

            // --- THÊM MỚI: Vẽ Vật phẩm ---
            foreach (var item in itemsOnMap)
            {
                item.Draw(g);
            }
            // -----------------------------

            // Vẽ đối tượng
            player.Draw(g);
            foreach (var monster in monsters)
            {
                monster.Draw(g);
            }
            boss.Draw(g);
        }

        // ... (Các hàm xử lý phím và IGameMap giữ nguyên như Water.cs đã sửa) ...
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (_isGamePaused) { ResumeGame(); } else { PauseGame(); }
                return true;
            }
            if (_isGamePaused) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                // Di chuyển
                case Keys.A: player.MoveLeft = true; return true;
                case Keys.D: player.MoveRight = true; return true;
                case Keys.W: player.MoveUp = true; return true;
                case Keys.S: player.MoveDown = true; return true;
                // Hành động
                case Keys.ShiftKey: player.AttemptDashInput = true; return true;
                case Keys.Space: player.AttemptAttackInput = true; return true;
                case Keys.ControlKey: player.AttemptRun = true; return true; // Chạy
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
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
            }
        }
        private void Flame_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }
        public GameState GetCurrentGameState()
        {
            if (player == null) return null;
            return new GameState
            {
                MapName = this.Name,
                PlayerX = player.X,
                PlayerY = player.Y,
                PlayerHealth = player.CurrentHealth,
                PlayerStamina = player.CurrentStamina,
                Inventory = player.Inventory
            };
        }
        public void LoadGameState(GameState state)
        {
            if (player == null) { MessageBox.Show("Lỗi: Player chưa khởi tạo."); this.Close(); return; }
            if (state == null) { MessageBox.Show("Lỗi: Save data không hợp lệ."); return; }

            if (state.MapName != this.Name)
            {
                MessageBox.Show($"Lỗi: Không thể tải save của map '{state.MapName}' lên map '{this.Name}'.");
                this.Close();
                return;
            }
            player.Position = new PointF(state.PlayerX, state.PlayerY);
            player.CurrentHealth = state.PlayerHealth;
            player.CurrentStamina = state.PlayerStamina;
            if (state.Inventory != null)
            {
                player.Inventory.Clear();
                foreach (var item in state.Inventory) { player.AddItem(item.Key, item.Value); }
            }
            MessageBox.Show("Tải game thành công!");
            ResumeGame();
        }
        public PointF GetPlayerSpawnPoint()
        {
            if (mazeGen != null)
            {
                return new PointF(
                    mazeGen.StartGridPosition.X * tileSize + (tileSize - PLAYER_HITBOX_SIZE) / 2,
                    mazeGen.StartGridPosition.Y * tileSize + (tileSize - PLAYER_HITBOX_SIZE) / 2
                );
            }
            return new PointF(tileSize, tileSize);
        }
        public void PauseGame()
        {
            _isGamePaused = true;
            timerGameLoop.Stop();
            if (_pauseMenu == null || _pauseMenu.IsDisposed)
            {
                if (Player == null) { MessageBox.Show("Lỗi: Player chưa khởi tạo."); _isGamePaused = false; timerGameLoop.Start(); return; }
                _pauseMenu = new frmMenu(_mainMenuForm, this);
            }
            _pauseMenu.Show(this);
        }
        public void ResumeGame()
        {
            _isGamePaused = false;
            timerGameLoop.Start();
            if (_pauseMenu != null && !_pauseMenu.IsDisposed)
            {
                _pauseMenu.Close();
            }
            this.Focus();
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

        // Cập nhật và tạo hạt (Tro bay)
        private void UpdateParticles()
        {
            if (rand.Next(5) == 0) // Tần suất
            {
                float x = rand.Next(this.Width);
                float y = this.Height; // Bắt đầu từ đáy
                float velY = -rand.Next(1, 5) * 0.5f; // Bay lên
                int lifetime = rand.Next(100, 200);
                float size = rand.Next(2, 5);
                Color c = rand.Next(3) == 0 ? Color.FromArgb(150, Color.Orange) : Color.FromArgb(100, Color.DarkGray);
                particles.Add(new Particle(new PointF(x, y), velY, lifetime, size, c));
            }

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
            using (SolidBrush b = new SolidBrush(Color.White))
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

