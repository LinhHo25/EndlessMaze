using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Main.Tri; // <- Đảm bảo using Main.Tri

namespace Main.Map
{
    public partial class Water : Form
    {
        private Player player;
        private List<Monster> monsters = new List<Monster>();
        private Boss boss;

        // --- LOGIC MÊ CUNG ---
        private MazeGenerator mazeGen;
        private List<RectangleF> wallBounds = new List<RectangleF>();
        private int tileSize = (int)GameObjectType.TileSize; // 32
        // ---------------------

        private int fallDelay = 500;
        private bool isRaining = false;
        private Random rand = new Random();

        public Water()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Map Thủy: Hàn Thủy Vực";

            // Khởi tạo Mê cung
            // Kích thước 25x15 tiles (800/32, 480/32)
            mazeGen = new MazeGenerator(25, 15);
            mazeGen.GenerateMaze();
            InitializeWalls(); // Tạo tường

            // Đặt Player vào vị trí bắt đầu của mê cung
            PointF startPos = new PointF(
                mazeGen.StartGridPosition.X * tileSize,
                mazeGen.StartGridPosition.Y * tileSize
            );
            player = new Player(startPos.X, startPos.Y, 28);

            // Khởi tạo quái (ở vị trí ngẫu nhiên không phải tường)
            monsters.Add(new Monster(MonsterType.Water, GetRandomFloorPosition()));
            monsters.Add(new Monster(MonsterType.Water, GetRandomFloorPosition()));
            boss = new Boss(MonsterType.Water, GetRandomFloorPosition(true)); // Boss ở gần cuối

            // Bắt đầu các timer (Đã được khởi tạo trong Designer.cs)
            this.timerFall.Interval = fallDelay;
            this.timerGameLoop.Start();
            this.timerRain.Start();
        }

        // Tạo danh sách tường để kiểm tra va chạm
        private void InitializeWalls()
        {
            wallBounds.Clear();
            for (int y = 0; y < mazeGen.Height; y++)
            {
                for (int x = 0; x < mazeGen.Width; x++)
                {
                    if (mazeGen.Maze[y, x] == (int)GameObjectType.Wall)
                    {
                        wallBounds.Add(new RectangleF(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                }
            }
        }

        // Lấy vị trí sàn ngẫu nhiên
        private PointF GetRandomFloorPosition(bool nearEnd = false)
        {
            int x, y;
            do
            {
                if (nearEnd) // Lấy nửa sau map
                {
                    x = rand.Next(mazeGen.Width / 2, mazeGen.Width);
                    y = rand.Next(mazeGen.Height / 2, mazeGen.Height);
                }
                else // Lấy nửa đầu map
                {
                    x = rand.Next(1, mazeGen.Width / 2);
                    y = rand.Next(1, mazeGen.Height / 2);
                }
            } while (mazeGen.Maze[y, x] == (int)GameObjectType.Wall);

            return new PointF(x * tileSize, y * tileSize);
        }

        // Timer chính (60 FPS)
        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            // 1. Cập nhật logic Player
            player.Update();
            HandleMovementCollision(player); // Xử lý va chạm tường

            // 2. Cập nhật logic Quái
            foreach (var monster in monsters)
            {
                monster.Update();
                // (Chưa xử lý quái va chạm tường)
            }
            boss.Update();

            // Logic map (trơn trượt)
            if (isRaining && player.IsDashing && player.DashCount > 3)
            {
                player.IsFallen = true;
                this.timerFall.Start();
            }

            // 2. Vẽ lại
            this.Invalidate();
        }

        // Xử lý di chuyển và va chạm (Tương tự GameManager)
        private void HandleMovementCollision(Player p)
        {
            PointF moveVector = p.GetMovementVector();
            float dx = moveVector.X;
            float dy = moveVector.Y;

            // Vị trí dự kiến
            RectangleF nextXBounds = p.BoundingBox;
            nextXBounds.X += dx;

            RectangleF nextYBounds = p.BoundingBox;
            nextYBounds.Y += dy;

            // Kiểm tra va chạm
            foreach (var wall in wallBounds)
            {
                if (nextXBounds.IntersectsWith(wall)) dx = 0; // Chặn di chuyển X
                if (nextYBounds.IntersectsWith(wall)) dy = 0; // Chặn di chuyển Y
            }

            // Di chuyển Player
            p.Position = new PointF(p.Position.X + dx, p.Position.Y + dy);
        }


        // Timer hiệu ứng map (mưa)
        private void timerRain_Tick(object sender, EventArgs e)
        {
            isRaining = !isRaining;
            if (isRaining)
            {
                player.DashCount = 0;
            }
        }

        // Timer xử lý ngã
        private void timerFall_Tick(object sender, EventArgs e)
        {
            player.StopDash();
            player.IsFallen = false;
            this.timerFall.Stop();
        }

        // Xử lý vẽ
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.DarkBlue); // Nền map thủy

            // Vẽ Tường Mê Cung
            using (SolidBrush wallBrush = new SolidBrush(Color.FromArgb(50, 50, 150)))
            {
                foreach (var wall in wallBounds)
                {
                    g.FillRectangle(wallBrush, wall);
                }
            }

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

            // Vẽ các đối tượng
            player.Draw(g);
            foreach (var monster in monsters)
            {
                monster.Draw(g);
            }
            boss.Draw(g);
        }

        // Xử lý sự kiện nhấn phím
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (player.IsFallen) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.A: player.MoveLeft = true; return true;
                case Keys.D: player.MoveRight = true; return true;
                case Keys.W: player.MoveUp = true; return true;
                case Keys.S: player.MoveDown = true; return true;
                case Keys.Space: player.Dash(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Xử lý nhả phím (Rất quan trọng cho game mê cung)
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.A: player.MoveLeft = false; break;
                case Keys.D: player.MoveRight = false; break;
                case Keys.W: player.MoveUp = false; break;
                case Keys.S: player.MoveDown = false; break;
                case Keys.Space: player.AttemptDash = false; break;
            }
        }

        private void Water_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            // Đảm bảo các timer được khởi động (đã làm trong constructor)
        }

        // KHỐI CODE TIMER ĐÃ ĐƯỢC XÓA KHỎI ĐÂY
        // VÀ CHUYỂN QUA FILE DESIGNER
    }
}

