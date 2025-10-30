using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Main.Tri // Đã cập nhật namespace
{
    public class GameManager
    {
        // Kích thước Tile (32x32)
        public int TileSize { get; private set; }

        // Đối tượng chính
        public Player Player { get; private set; }
        public MazeGenerator MazeGen { get; private set; }

        // Danh sách các bức tường (Rectangle đại diện cho từng Tile Tường)
        public List<RectangleF> WallBounds { get; private set; } = new List<RectangleF>();

        // Vị trí Kết thúc (Tile Exit)
        public RectangleF ExitBounds { get; private set; }

        // Trạng thái game
        public enum GameState { Running, Paused, Win, GameOver }
        public GameState CurrentState { get; private set; } = GameState.Running;
        public int Score { get; private set; } = 0;

        public GameManager(int mazeWidth, int mazeHeight, int tileSize)
        {
            TileSize = tileSize;

            // 1. Tạo Mê cung
            MazeGen = new MazeGenerator(mazeWidth, mazeHeight);
            MazeGen.GenerateMaze();

            // 2. Khởi tạo Người chơi và Tường
            InitializeGameObjects();
        }

        /// <summary>
        /// Khởi tạo Player và trích xuất ranh giới (bounds) của các bức tường.
        /// </summary>
        private void InitializeGameObjects()
        {
            // Thiết lập Người chơi
            int startX = MazeGen.StartGridPosition.X * TileSize;
            int startY = MazeGen.StartGridPosition.Y * TileSize;
            Player = new Player(startX, startY, TileSize - 4); // Nhỏ hơn Tile một chút để dễ va chạm

            // TẢI ANIMATION CHO PLAYER (NẾU DÙNG GAMEMANAGER)
            // Player.SetUpAnimations(); // Bạn có thể kích hoạt dòng này nếu cần

            // Thiết lập vị trí Kết thúc
            int exitX = MazeGen.EndGridPosition.X * TileSize;
            int exitY = MazeGen.EndGridPosition.Y * TileSize;
            ExitBounds = new RectangleF(exitX, exitY, TileSize, TileSize);

            // Trích xuất các ranh giới của Tường
            WallBounds.Clear();
            for (int y = 0; y < MazeGen.Height; y++)
            {
                for (int x = 0; x < MazeGen.Width; x++)
                {
                    // Lớp GameObjectType nằm trong Main.Tri (cũng là namespace này)
                    if (MazeGen.Maze[y, x] == (int)GameObjectType.Wall)
                    {
                        WallBounds.Add(new RectangleF(x * TileSize, y * TileSize, TileSize, TileSize));
                    }
                }
            }
        }

        /// <summary>
        /// Game Loop: Cập nhật trạng thái của tất cả đối tượng trong game.
        /// </summary>
        public void Update()
        {
            if (CurrentState != GameState.Running) return;

            // 1. Cập nhật Player (tính toán vector di chuyển)
            Player.Update();

            // --- SỬA LỖI CS1061 TẠI ĐÂY ---
            // Gọi đúng tên hàm mới từ Player.cs
            PointF moveVector = Player.CalculateMovementVector(Player.Position);
            // ------------------------------

            // 2. Xử lý Va chạm và Di chuyển
            HandlePlayerMovementAndCollision(moveVector);

            // 3. Kiểm tra điều kiện Chiến thắng
            if (Player.GetBounds().IntersectsWith(ExitBounds))
            {
                CurrentState = GameState.Win;
            }

            // 4. (Tương lai) Cập nhật Quái vật, đạn...
        }

        /// <summary>
        /// Xử lý di chuyển và va chạm hình chữ nhật giữa Player và Tường (WallBounds).
        /// </summary>
        private void HandlePlayerMovementAndCollision(PointF moveVector)
        {
            float dx = moveVector.X;
            float dy = moveVector.Y;

            // Vị trí dự kiến sau khi di chuyển theo X
            float nextX = Player.X + dx;
            RectangleF nextXBounds = new RectangleF(nextX, Player.Y, Player.Width, Player.Height);

            bool collisionX = false;
            foreach (var wall in WallBounds)
            {
                if (nextXBounds.IntersectsWith(wall))
                {
                    collisionX = true;
                    break;
                }
            }

            if (!collisionX)
            {
                Player.X = nextX;
            }

            // Vị trí dự kiến sau khi di chuyển theo Y
            float nextY = Player.Y + dy;
            RectangleF nextYBounds = new RectangleF(Player.X, nextY, Player.Width, Player.Height);

            bool collisionY = false;
            foreach (var wall in WallBounds)
            {
                if (nextYBounds.IntersectsWith(wall))
                {
                    collisionY = true;
                    break;
                }
            }

            if (!collisionY)
            {
                Player.Y = nextY;
            }
        }


        /// <summary>
        /// Vẽ toàn bộ trạng thái game.
        /// </summary>
        public void Draw(Graphics g, PictureBox canvas)
        {
            // Thiết lập chất lượng vẽ tốt
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 1. Vẽ Mê cung
            DrawMaze(g);

            // 2. Vẽ Điểm kết thúc
            using (SolidBrush exitBrush = new SolidBrush(Color.Gold))
            {
                g.FillRectangle(exitBrush, ExitBounds);
            }

            // 3. Vẽ Player
            Player.Draw(g);

            // 4. Vẽ trạng thái Game (Win/Pause/Game Over)
            if (CurrentState == GameState.Win)
            {
                DrawMessage(g, canvas, "CHIẾN THẮNG! (Nhấn R để chơi lại)");
            }
            else if (CurrentState == GameState.Paused)
            {
                DrawMessage(g, canvas, "TẠM DỪNG (Nhấn P để tiếp tục)");
            }
        }

        private void DrawMaze(Graphics g)
        {
            // Vẽ Sàn (Lớp màu tối bên dưới)
            g.FillRectangle(Brushes.DarkGray, 0, 0, MazeGen.Width * TileSize, MazeGen.Height * TileSize);

            // Vẽ Tường
            using (SolidBrush wallBrush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                for (int y = 0; y < MazeGen.Height; y++)
                {
                    for (int x = 0; x < MazeGen.Width; x++)
                    {
                        if (MazeGen.Maze[y, x] == (int)GameObjectType.Wall)
                        {
                            g.FillRectangle(wallBrush, x * TileSize, y * TileSize, TileSize, TileSize);
                            // Thêm hiệu ứng 3D nhẹ
                            g.DrawLine(Pens.Gray, x * TileSize, y * TileSize, x * TileSize + TileSize, y * TileSize + TileSize);
                        }
                    }
                }
            }
        }

        private void DrawMessage(Graphics g, PictureBox canvas, string message)
        {
            // Vẽ lớp phủ mờ
            using (SolidBrush semiTransparentBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            {
                g.FillRectangle(semiTransparentBrush, 0, 0, canvas.Width, canvas.Height);
            }

            // Vẽ thông báo
            using (Font font = new Font("Segoe UI", 24, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(message, font, Brushes.White, new RectangleF(0, 0, canvas.Width, canvas.Height), sf);
            }
        }

        /// <summary>
        /// Đặt lại game và tạo mê cung mới.
        /// </summary>
        public void ResetGame()
        {
            MazeGen.GenerateMaze();
            InitializeGameObjects();
            CurrentState = GameState.Running;
            Score = 0;
        }

        /// <summary>
        /// Xử lý các sự kiện nhấn phím để điều khiển Player và Game.
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up: Player.MoveUp = true; break;
                case Keys.S:
                case Keys.Down: Player.MoveDown = true; break;
                case Keys.A:
                case Keys.Left: Player.MoveLeft = true; break;
                case Keys.D:
                case Keys.Right: Player.MoveRight = true; break;
                case Keys.Space: Player.AttemptDashInput = true; break; // Lướt
                case Keys.P: // Tạm dừng/Tiếp tục
                    CurrentState = (CurrentState == GameState.Running) ? GameState.Paused : GameState.Running;
                    break;
                case Keys.R: // Chơi lại
                    if (CurrentState == GameState.Win || CurrentState == GameState.GameOver) ResetGame();
                    break;
            }
        }

        /// <summary>
        /// Xử lý các sự kiện nhả phím.
        /// </summary>
        public void HandleKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up: Player.MoveUp = false; break;
                case Keys.S:
                case Keys.Down: Player.MoveDown = false; break;
                case Keys.A:
                case Keys.Left: Player.MoveLeft = false; break;
                case Keys.D:
                case Keys.Right: Player.MoveRight = false; break;
                case Keys.Space: Player.AttemptDashInput = false; break;
            }
        }
    }
}
