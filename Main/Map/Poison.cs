using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D; // Thêm
using System.Windows.Forms;
using Main.Tri;

namespace Main.Map
{
    public partial class Poison : Form, IGameMap
    {
        // ... (Các biến khác) ...
        private frmMain _mainMenuForm;
        private frmMenu _pauseMenu;
        private bool _isGamePaused = false;

        private Player player;
        private List<Monster> monsters = new List<Monster>();
        private Boss boss;

        private List<CollectibleItem> itemsOnMap = new List<CollectibleItem>();
        private TextureBrush wallBrush;
        private TextureBrush floorBrush;
        private List<Particle> particles = new List<Particle>();

        private MazeGenerator mazeGen;
        private List<RectangleF> wallBounds = new List<RectangleF>();
        private List<PointF> floorTiles = new List<PointF>();
        private const int MAZE_WIDTH = 25;
        private const int MAZE_HEIGHT = 15;
        private int tileSize = (int)GameObjectType.TileSize;

        private List<Rectangle> poisonSwamps = new List<Rectangle>();
        private Random rand = new Random(); // Biến rand của Form

        // Kích thước hitbox cố định của Player (Thêm vào)
        private const int PLAYER_HITBOX_SIZE = 28;

        public Player Player { get { return player; } }
        // FIX CS0108: Add 'new' keyword
        public new string Name => this.Text; // Implement Name property
        // ... (Constructor và các hàm khác) ...
        public Poison(frmMain mainMenuForm)
        {
            InitializeComponent();
            _mainMenuForm = mainMenuForm;
            this.DoubleBuffered = true;
            this.Text = "Map Độc: Rừng Chướng Khí";

            InitializeTextures();

            mazeGen = new MazeGenerator(MAZE_WIDTH, MAZE_HEIGHT);
            mazeGen.GenerateMaze();
            InitializeWallsAndItems();

            PointF startPos = GetPlayerSpawnPoint();
            // Sử dụng kích thước cố định
            player = new Player(startPos.X, startPos.Y, PLAYER_HITBOX_SIZE);
            player.SetUpAnimations(); // Gọi sau khi tạo player

            monsters.Add(new Monster(MonsterType.Poison, GetRandomFloorPosition()));
            boss = new Boss(MonsterType.Poison, GetRandomFloorPosition(true));

            for (int i = 0; i < 5; i++)
            {
                PointF swampPos = GetRandomFloorPosition();
                poisonSwamps.Add(new Rectangle((int)swampPos.X, (int)swampPos.Y, tileSize * 3, tileSize * 2));
            }

            this.timerGameLoop.Start();
            this.timerPoisonDamage.Start();
        }
        #region Maze Logic & Visuals

        private void InitializeTextures()
        {
            Bitmap wallTexture = new Bitmap(tileSize, tileSize);
            using (Graphics g = Graphics.FromImage(wallTexture))
            {
                g.Clear(Color.FromArgb(20, 50, 20));
                using (Pen p = new Pen(Color.FromArgb(40, 80, 40), 3))
                {
                    g.DrawBezier(p, 0, 0, 10, 16, 20, 16, 32, 32);
                    g.DrawBezier(p, 32, 0, 22, 16, 12, 16, 0, 32);
                }
            }
            wallBrush = new TextureBrush(wallTexture);

            Bitmap floorTexture = new Bitmap(tileSize, tileSize);
            using (Graphics g = Graphics.FromImage(floorTexture))
            {
                g.Clear(Color.FromArgb(40, 60, 40));
                g.DrawLine(Pens.Green, 5, 5, 7, 10);
                g.DrawLine(Pens.Green, 20, 20, 22, 25);
            }
            floorBrush = new TextureBrush(floorTexture);
        }

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

            SpawnItem(ItemType.HealthPotion);
            SpawnItem(ItemType.PoisonResistPotion);
            SpawnItem(ItemType.PoisonResistPotion);
        }

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

        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            if (_isGamePaused) return;

            UpdateParticles();

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
            }
            boss.Update();
            PointF bossMove = boss.CalculateMovementVector(player.Position);
            PointF nextBossPos = new PointF(boss.X + bossMove.X, boss.Y + bossMove.Y);
            boss.Position = GetAllowedMovement(boss.BoundingBox, nextBossPos);

            for (int i = itemsOnMap.Count - 1; i >= 0; i--)
            {
                if (player.BoundingBox.IntersectsWith(itemsOnMap[i].BoundingBox))
                {
                    player.AddItem(itemsOnMap[i].Item);
                    itemsOnMap.RemoveAt(i);
                }
            }

            // FIX CS0219: Remove unused inSwamp variable assignment
            // bool inSwamp = false; // Xóa dòng này
            // foreach (var swamp in poisonSwamps)
            // {
            //     if (player.BoundingBox.IntersectsWith(swamp))
            //     {
            //         inSwamp = true; // Xóa dòng này
            //         break;
            //     }
            // }

            // Player sẽ tự xử lý IsPoisoned dựa trên buff và timerPoisonDamage_Tick
            // if (inSwamp && !player.ActiveBuffs.ContainsKey(BuffType.PoisonResist))
            // {
            //     player.IsPoisoned = true;
            // }

            this.Invalidate();
        }

        private void timerPoisonDamage_Tick(object sender, EventArgs e)
        {
            if (_isGamePaused || player == null) return; // Thêm kiểm tra null

            // Kiểm tra nếu đang ở trong vũng độc VÀ không có buff kháng độc
            bool inSwamp = false;
            foreach (var swamp in poisonSwamps)
            {
                if (player.BoundingBox.IntersectsWith(swamp))
                {
                    inSwamp = true;
                    break;
                }
            }

            // Gây damage độc nếu ở trong vũng và không có kháng
            if (inSwamp && !player.ActiveBuffs.ContainsKey(BuffType.PoisonResist) && player.CurrentHealth > 0)
            {
                float damage = player.MaxHealth * 0.005f; // Có thể điều chỉnh % damage
                player.TakeDamage(damage); // Player tự xử lý việc giảm máu
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Thêm
            g.Clear(Color.DarkGreen);

            g.FillRectangle(floorBrush, 0, 0, this.Width, this.Height);

            foreach (var wall in wallBounds)
            {
                g.FillRectangle(wallBrush, wall);
            }

            using (var brush = new SolidBrush(Color.FromArgb(150, Color.Purple)))
            {
                foreach (var swamp in poisonSwamps)
                {
                    g.FillRectangle(brush, swamp);
                }
            }

            DrawParticles(g);

            foreach (var item in itemsOnMap)
            {
                item.Draw(g);
            }

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
        private void Poison_Load(object sender, EventArgs e)
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


        // --- LỚP PARTICLE (ĐÃ SỬA) ---
        private class Particle
        {
            public PointF Position;
            public float VelocityX;
            public float VelocityY;
            public int Lifetime;
            public Color Color;
            // SỬA LỖI CS0120: Thêm static vào đây
            private static Random _particleRand = new Random();

            public Particle(PointF pos, float velX, float velY, int life)
            {
                Position = pos;
                VelocityX = velX;
                VelocityY = velY;
                Lifetime = life;
                // Sử dụng Random tĩnh
                Color = Color.FromArgb(_particleRand.Next(100, 150), 150, 255, 150);
            }

            public bool Update()
            {
                Position.X += VelocityX;
                Position.Y += VelocityY;
                Lifetime--;
                return Lifetime <= 0;
            }
        }

        // ... (UpdateParticles và DrawParticles) ...
        private void UpdateParticles()
        {
            // Sử dụng rand của Form để quyết định có tạo hạt không
            if (rand.Next(5) == 0)
            {
                float x = rand.Next(this.Width);
                float y = rand.Next(this.Height);
                float velX = (float)(rand.NextDouble() * 2 - 1);
                float velY = (float)(rand.NextDouble() * 1 - 0.5);
                int lifetime = rand.Next(100, 200);
                // Tạo hạt mới, không cần truyền rand vào đây nữa
                particles.Add(new Particle(new PointF(x, y), velX, velY, lifetime));
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].Update())
                {
                    particles.RemoveAt(i);
                }
            }
        }
        private void DrawParticles(Graphics g)
        {
            using (SolidBrush b = new SolidBrush(Color.White))
            {
                foreach (var p in particles)
                {
                    b.Color = p.Color;
                    g.FillEllipse(b, p.Position.X, p.Position.Y, 4, 4);
                }
            }
        }
    }
}

