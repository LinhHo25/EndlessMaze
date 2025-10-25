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

        private int fallDelay = 500;
        private bool isRaining = false;
        private Random rand = new Random();

        public Player Player { get { return player; } }

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
            player = new Player(startPos.X, startPos.Y, 28);
            player.SetUpAnimations();

            monsters.Add(new Monster(MonsterType.Water, GetRandomFloorPosition()));
            monsters.Add(new Monster(MonsterType.Water, GetRandomFloorPosition()));
            boss = new Boss(MonsterType.Water, GetRandomFloorPosition(true));

            this.timerFall.Interval = fallDelay;
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
            player.Position = GetAllowedMovement(player.BoundingBox, playerMove);

            foreach (var monster in monsters)
            {
                monster.Update();
                PointF monsterMove = monster.CalculateMovementVector(player.Position);
                monster.Position = GetAllowedMovement(monster.BoundingBox, monsterMove);
                // TODO: Xử lý quái tấn công Player
            }
            boss.Update();
            PointF bossMove = boss.CalculateMovementVector(player.Position);
            boss.Position = GetAllowedMovement(boss.BoundingBox, bossMove);

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

            if (isRaining && player.IsDashing && player.DashCount > 3)
            {
                player.IsFallen = true;
                this.timerFall.Start();
            }

            this.Invalidate();
        }

        // ... (Hàm GetAllowedMovement, timerRain, timerFall giữ nguyên) ...
        private PointF GetAllowedMovement(RectangleF bounds, PointF moveVector)
        {
            float dx = moveVector.X;
            float dy = moveVector.Y;

            RectangleF nextBounds = bounds;
            nextBounds.X += dx;
            nextBounds.Y += dy;

            RectangleF nextXBounds = bounds;
            nextXBounds.X += dx;

            RectangleF nextYBounds = bounds;
            nextYBounds.Y += dy;

            foreach (var wall in wallBounds)
            {
                if (nextXBounds.IntersectsWith(wall)) dx = 0;
                if (nextYBounds.IntersectsWith(wall)) dy = 0;
            }

            return new PointF(bounds.X + dx, bounds.Y + dy);
        }
        private void timerRain_Tick(object sender, EventArgs e)
        {
            isRaining = !isRaining;
            if (isRaining)
            {
                player.DashCount = 0;
            }
        }
        private void timerFall_Tick(object sender, EventArgs e)
        {
            player.StopDash();
            player.IsFallen = false;
            this.timerFall.Stop();
        }

        // Xử lý vẽ (ĐÃ CẬP NHẬT)
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
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
                for (int i = 0; i < 50; i++)
                {
                    int x = rand.Next(this.Width);
                    int y = rand.Next(this.Height);
                    g.DrawLine(Pens.Cyan, x, y, x + 1, y + 5);
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

        // ... (Các hàm xử lý phím ProcessCmdKey, OnKeyUp, Water_Load giữ nguyên) ...
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
            if (player.IsFallen) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.A: player.MoveLeft = true; return true;
                case Keys.D: player.MoveRight = true; return true;
                case Keys.W: player.MoveUp = true; return true;
                case Keys.S: player.MoveDown = true; return true;
                case Keys.ShiftKey: player.Dash(); return true;
                case Keys.Q: player.Attack(); return true;
                case Keys.E: player.Block(true); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_isGamePaused) return;

            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.A: player.MoveLeft = false; break;
                case Keys.D: player.MoveRight = false; break;
                case Keys.W: player.MoveUp = false; break;
                case Keys.S: player.MoveDown = false; break;
                case Keys.ShiftKey: player.AttemptDash = false; break;
                case Keys.E: player.Block(false); break;
            }
        }
        private void Water_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        // ... (Các hàm IGameMap: GetCurrentGameState, LoadGameState, GetPlayerSpawnPoint, PauseGame, ResumeGame giữ nguyên) ...
        public GameState GetCurrentGameState()
        {
            return new GameState
            {
                MapName = this.Name,
                PlayerX = player.X,
                PlayerY = player.Y,
                PlayerHealth = player.CurrentHealth,
                PlayerStamina = player.CurrentStamina,
                Inventory = player.Inventory // Thêm Inventory vào save
            };
        }
        public void LoadGameState(GameState state)
        {
            if (state.MapName != this.Name)
            {
                MessageBox.Show($"Lỗi: Không thể tải save của map '{state.MapName}' lên map '{this.Name}'.");
                return;
            }

            player.Position = new PointF(state.PlayerX, state.PlayerY);
            player.CurrentHealth = state.PlayerHealth;
            player.CurrentStamina = state.PlayerStamina;
            // Tải Inventory
            if (state.Inventory != null) player.Inventory.Clear();
            foreach (var item in state.Inventory) { player.AddItem(item.Key, item.Value); }


            MessageBox.Show("Tải game thành công!");
            ResumeGame();
        }
        public PointF GetPlayerSpawnPoint()
        {
            return new PointF(
               mazeGen.StartGridPosition.X * tileSize,
               mazeGen.StartGridPosition.Y * tileSize
           );
        }
        public void PauseGame()
        {
            _isGamePaused = true;
            if (_pauseMenu == null || _pauseMenu.IsDisposed)
            {
                _pauseMenu = new frmMenu(_mainMenuForm, this);
            }
            _pauseMenu.Show(this);
        }
        public void ResumeGame()
        {
            _isGamePaused = false;
            if (_pauseMenu != null && !_pauseMenu.IsDisposed)
            {
                _pauseMenu.Close();
            }
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

