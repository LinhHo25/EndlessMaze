using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Main.Tri; // <- Đảm bảo using Main.Tri

namespace Main.Map
{
    public partial class Flame : Form
    {
        private Player player;
        private List<Monster> monsters = new List<Monster>();
        private Boss boss;

        // --- LOGIC MÊ CUNG ---
        private MazeGenerator mazeGen;
        private List<RectangleF> wallBounds = new List<RectangleF>();
        private int tileSize = (int)GameObjectType.TileSize;
        // ---------------------

        private List<Rectangle> unstableGrounds = new List<Rectangle>();
        private bool isSmoky = false;
        private Random rand = new Random();

        public Flame()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Map Hỏa: Diệm Ngục";

            mazeGen = new MazeGenerator(25, 15);
            mazeGen.GenerateMaze();
            InitializeWalls();

            PointF startPos = new PointF(
                mazeGen.StartGridPosition.X * tileSize,
                mazeGen.StartGridPosition.Y * tileSize
            );
            player = new Player(startPos.X, startPos.Y, 28);

            monsters.Add(new Monster(MonsterType.Flame, GetRandomFloorPosition()));
            boss = new Boss(MonsterType.Flame, GetRandomFloorPosition(true));

            // Tạo các vùng sụt lún ngẫu nhiên trên sàn
            for (int i = 0; i < 5; i++)
            {
                PointF groundPos = GetRandomFloorPosition();
                unstableGrounds.Add(new Rectangle((int)groundPos.X, (int)groundPos.Y, tileSize * 2, tileSize * 2));
            }

            // (Đã được khởi tạo trong Designer.cs)
            this.timerGameLoop.Start();
            this.timerLava.Start();
            this.timerSweat.Start();
        }

        #region Maze Logic (Sao chép từ Water.cs)
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
        private PointF GetRandomFloorPosition(bool nearEnd = false)
        {
            int x, y;
            do
            {
                if (nearEnd)
                {
                    x = rand.Next(mazeGen.Width / 2, mazeGen.Width);
                    y = rand.Next(mazeGen.Height / 2, mazeGen.Height);
                }
                else
                {
                    x = rand.Next(1, mazeGen.Width / 2);
                    y = rand.Next(1, mazeGen.Height / 2);
                }
            } while (mazeGen.Maze[y, x] == (int)GameObjectType.Wall);
            return new PointF(x * tileSize, y * tileSize);
        }
        private void HandleMovementCollision(Player p)
        {
            PointF moveVector = p.GetMovementVector();
            float dx = moveVector.X;
            float dy = moveVector.Y;
            RectangleF nextXBounds = p.BoundingBox;
            nextXBounds.X += dx;
            RectangleF nextYBounds = p.BoundingBox;
            nextYBounds.Y += dy;
            foreach (var wall in wallBounds)
            {
                if (nextXBounds.IntersectsWith(wall)) dx = 0;
                if (nextYBounds.IntersectsWith(wall)) dy = 0;
            }
            p.Position = new PointF(p.Position.X + dx, p.Position.Y + dy);
        }
        #endregion

        // Game loop
        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            player.Update();
            HandleMovementCollision(player); // Va chạm tường

            foreach (var monster in monsters) monster.Update();
            boss.Update();

            // Xử lý va chạm sụt lún
            foreach (var ground in unstableGrounds)
            {
                if (player.BoundingBox.IntersectsWith(ground))
                {
                    // (Logic sụt lún)
                }
            }

            this.Invalidate();
        }

        // Timer sụt lún/khói
        private void timerLava_Tick(object sender, EventArgs e)
        {
            isSmoky = !isSmoky;
        }

        // Timer đổ mồ hôi
        private void timerSweat_Tick(object sender, EventArgs e)
        {
            if (player.CurrentStamina > 0)
            {
                float staminaLoss = player.MaxStamina * 0.10f;
                player.CurrentStamina = Math.Max(0, player.CurrentStamina - staminaLoss);
            }
        }

        // Vẽ
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.DarkRed); // Nền map hỏa

            // Vẽ Tường Mê Cung
            using (SolidBrush wallBrush = new SolidBrush(Color.FromArgb(100, 50, 50)))
            {
                foreach (var wall in wallBounds)
                {
                    g.FillRectangle(wallBrush, wall);
                }
            }

            // Vẽ vùng sụt lún
            foreach (var ground in unstableGrounds)
            {
                g.FillRectangle(Brushes.Orange, ground);
            }

            // Vẽ khói
            if (isSmoky)
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, Color.Gray)))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }

            // Vẽ đối tượng
            player.Draw(g);
            foreach (var monster in monsters)
            {
                monster.Draw(g);
            }
            boss.Draw(g);
        }

        // Xử lý phím
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
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

        private void Flame_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        // KHỐI CODE TIMER ĐÃ ĐƯỢC XÓA KHỎI ĐÂY
    }
}

