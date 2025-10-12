using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EndlessMaze
{
    public partial class EndlessMaze : Form
    {
        // =================================================================
        // BIẾN TOÀN CỤC VÀ CÁC ĐỐI TƯỢNG GAME
        // =================================================================

        private Timer gameTimer; // Timer chính điều khiển game loop
        private Player player; // Đối tượng người chơi
        private List<Monster> monsters; // Danh sách quái vật
        private Maze gameMaze; // Đối tượng mê cung
        private GameState currentGameState; // Trạng thái hiện tại của game

        // Biến theo dõi trạng thái input của người chơi
        private bool isMovingUp, isMovingDown, isMovingLeft, isMovingRight;
        private bool isAttacking, isDashing;

        private int score;
        private int level;
        private TimeSpan timeElapsed;
        private DateTime levelStartTime;


        // Enum để quản lý các trạng thái của game
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }


        // =================================================================
        // KHỞI TẠO FORM VÀ CÁC THÀNH PHẦN
        // =================================================================

        public EndlessMaze()
        {
            InitializeComponent();
            SetupGame();
        }

        private void SetupGame()
        {
            // Cài đặt các giá trị ban đầu
            this.Text = "Endless Maze";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Cấu hình game loop timer
            gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;

            // Kích hoạt double buffering để giảm giật lag khi vẽ
            this.DoubleBuffered = true;

            // Đăng ký sự kiện bàn phím
            this.KeyDown += EndlessMaze_KeyDown;
            this.KeyUp += EndlessMaze_KeyUp;

            // Chuyển về màn hình chính
            SwitchGameState(GameState.MainMenu);
        }


        // =================================================================
        // QUẢN LÝ TRẠNG THÁI GAME
        // =================================================================

        private void SwitchGameState(GameState newState)
        {
            currentGameState = newState;

            // Ẩn tất cả các panel giao diện
            pnlMainMenu.Visible = false;
            pnlGameUI.Visible = false;
            pnlGameOver.Visible = false;
            pbGameCanvas.Visible = false;

            // Hiển thị panel tương ứng với trạng thái mới
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    pnlMainMenu.Visible = true;
                    break;
                case GameState.Playing:
                    pnlGameUI.Visible = true;
                    pbGameCanvas.Visible = true;
                    StartNewGame();
                    break;
                case GameState.GameOver:
                    pnlGameOver.Visible = true;
                    gameTimer.Stop();
                    // Hiển thị thông số cuối cùng
                    lblFinalScore.Text = "Điểm: " + score;
                    lblFinalTime.Text = "Thời gian: " + timeElapsed.ToString(@"mm\:ss");
                    break;
            }
        }

        private void StartNewGame()
        {
            // Reset các thông số game
            score = 0;
            level = 1;
            levelStartTime = DateTime.Now;
            timeElapsed = TimeSpan.Zero;

            // Khởi tạo người chơi
            player = new Player { X = 50, Y = 50, Health = 100, MaxHealth = 100, Speed = 5 };

            // Khởi tạo mê cung và quái vật
            GenerateLevel();

            UpdateUI();

            // Bắt đầu game loop
            gameTimer.Start();
        }

        private void GenerateLevel()
        {
            // Đây là nơi để gọi thuật toán tạo mê cung ngẫu nhiên
            // Tạm thời, chúng ta sẽ tạo một mê cung đơn giản
            gameMaze = new Maze(pbGameCanvas.Width, pbGameCanvas.Height);

            // Tạo quái vật dựa trên level hiện tại
            monsters = new List<Monster>();
            Random rand = new Random();
            int monsterCount = 3 + level; // Càng lên level cao, càng nhiều quái
            for (int i = 0; i < monsterCount; i++)
            {
                monsters.Add(new Monster
                {
                    X = rand.Next(100, pbGameCanvas.Width - 100),
                    Y = rand.Next(100, pbGameCanvas.Height - 100),
                    Health = 20 + (level * 5) // Máu quái tăng theo level
                });
            }
        }


        // =================================================================
        // GAME LOOP CHÍNH
        // =================================================================

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (currentGameState != GameState.Playing) return;

            // 1. Cập nhật thời gian
            timeElapsed = DateTime.Now - levelStartTime;

            // 2. Xử lý input và cập nhật trạng thái người chơi
            UpdatePlayerState();

            // 3. Cập nhật trạng thái quái vật (AI)
            UpdateMonsterState();

            // 4. Xử lý va chạm
            HandleCollisions();

            // 5. Kiểm tra điều kiện thắng/thua
            CheckGameConditions();

            // 6. Cập nhật giao diện
            UpdateUI();

            // 7. Yêu cầu vẽ lại canvas
            pbGameCanvas.Invalidate();
        }

        private void UpdatePlayerState()
        {
            // Xử lý di chuyển
            if (isMovingUp) player.Y -= player.Speed;
            if (isMovingDown) player.Y += player.Speed;
            if (isMovingLeft) player.X -= player.Speed;
            if (isMovingRight) player.X += player.Speed;

            // Giới hạn di chuyển trong canvas
            player.X = Math.Max(0, Math.Min(pbGameCanvas.Width - 30, player.X));
            player.Y = Math.Max(0, Math.Min(pbGameCanvas.Height - 30, player.Y));

            // Xử lý Lướt (Dash)
            if (isDashing)
            {
                // TODO: Thêm logic cho kỹ năng lướt
                // Ví dụ: tăng tốc độ trong một khoảng thời gian ngắn
                isDashing = false; // Tạm thời reset ngay lập tức
            }

            // Xử lý Tấn công
            if (isAttacking)
            {
                // TODO: Thêm logic tấn công, tạo vùng sát thương
                isAttacking = false; // Tạm thời reset ngay lập tức
            }
        }

        private void UpdateMonsterState()
        {
            // AI đơn giản: di chuyển về phía người chơi
            foreach (var monster in monsters)
            {
                if (monster.X < player.X) monster.X += monster.Speed;
                if (monster.X > player.X) monster.X -= monster.Speed;
                if (monster.Y < player.Y) monster.Y += monster.Speed;
                if (monster.Y > player.Y) monster.Y -= monster.Speed;
            }
        }

        private void HandleCollisions()
        {
            // TODO: Xử lý va chạm giữa người chơi và quái, người chơi và bẫy, đạn và mục tiêu...
        }

        private void CheckGameConditions()
        {
            // TODO: Kiểm tra nếu người chơi hết máu -> GameOver
            if (player.Health <= 0)
            {
                SwitchGameState(GameState.GameOver);
            }

            // TODO: Kiểm tra nếu tất cả quái đã bị tiêu diệt -> Qua màn
            if (monsters.Count == 0)
            {
                level++;
                GenerateLevel();
                levelStartTime = DateTime.Now; // Reset thời gian cho màn mới
            }
        }

        private void UpdateUI()
        {
            lblPlayerHP.Text = $"HP: {player.Health} / {player.MaxHealth}";
            progPlayerHP.Maximum = player.MaxHealth;
            progPlayerHP.Value = Math.Max(0, player.Health);
            lblScore.Text = "Điểm: " + score;
            lblLevel.Text = "Màn: " + level;
            lblTime.Text = timeElapsed.ToString(@"mm\:ss");
        }


        // =================================================================
        // VẼ ĐỐI TƯỢNG LÊN CANVAS
        // =================================================================

        private void pbGameCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Xóa canvas với màu nền
            g.Clear(Color.FromArgb(24, 24, 24));

            // Vẽ mê cung (nếu có)
            gameMaze?.Draw(g);

            // Vẽ quái vật
            foreach (var monster in monsters)
            {
                monster.Draw(g);
            }

            // Vẽ người chơi
            player?.Draw(g);
        }


        // =================================================================
        // XỬ LÝ SỰ KIỆN (BUTTON, BÀN PHÍM)
        // =================================================================

        private void EndlessMaze_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentGameState != GameState.Playing) return;

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    isMovingUp = true;
                    break;
                case Keys.S:
                case Keys.Down:
                    isMovingDown = true;
                    break;
                case Keys.A:
                case Keys.Left:
                    isMovingLeft = true;
                    break;
                case Keys.D:
                case Keys.Right:
                    isMovingRight = true;
                    break;
                case Keys.Space:
                    isDashing = true;
                    break;
                case Keys.J: // Nút đánh thường
                    isAttacking = true;
                    break;
            }
        }

        private void EndlessMaze_KeyUp(object sender, KeyEventArgs e)
        {
            if (currentGameState != GameState.Playing) return;

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    isMovingUp = false;
                    break;
                case Keys.S:
                case Keys.Down:
                    isMovingDown = false;
                    break;
                case Keys.A:
                case Keys.Left:
                    isMovingLeft = false;
                    break;
                case Keys.D:
                case Keys.Right:
                    isMovingRight = false;
                    break;
            }
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            SwitchGameState(GameState.Playing);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBackToMenu_Click(object sender, EventArgs e)
        {
            SwitchGameState(GameState.MainMenu);
        }
    }

    // =================================================================
    // CÁC LỚP ĐẠI DIỆN CHO ĐỐI TƯỢNG GAME
    // (Để đơn giản, tôi định nghĩa chúng trong cùng một file)
    // =================================================================

    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 30;
        public int Height { get; set; } = 30;
        public abstract void Draw(Graphics g);
    }

    public class Player : GameObject
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Speed { get; set; }

        public override void Draw(Graphics g)
        {
            // Vẽ người chơi là một hình chữ nhật màu xanh
            g.FillRectangle(Brushes.DodgerBlue, X, Y, Width, Height);
            // Vẽ thanh máu trên đầu
            g.FillRectangle(Brushes.Red, X, Y - 10, Width, 5);
            int healthWidth = (int)((double)Health / MaxHealth * Width);
            g.FillRectangle(Brushes.Green, X, Y - 10, healthWidth, 5);
        }
    }

    public class Monster : GameObject
    {
        public int Health { get; set; }
        public int Speed { get; set; } = 2;

        public override void Draw(Graphics g)
        {
            // Vẽ quái là một hình chữ nhật màu đỏ
            g.FillRectangle(Brushes.IndianRed, X, Y, Width, Height);
        }
    }

    public class Maze
    {
        // TODO: Thêm logic tạo và lưu trữ dữ liệu mê cung
        private int width;
        private int height;
        public Maze(int w, int h)
        {
            width = w;
            height = h;
        }

        public void Draw(Graphics g)
        {
            // Vẽ các bức tường của mê cung
            // Tạm thời chỉ vẽ một đường viền xung quanh
            g.DrawRectangle(Pens.Gray, 0, 0, width - 1, height - 1);
        }
    }
}
