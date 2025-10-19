using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Text.Json.Serialization; // Thư viện này cần thiết cho tính năng Lưu/Tải Game (nhiệm vụ của Linh)
using EndlessMaze.Tri; // ĐÃ CẬP NHẬT: Thay thế Main.Tri bằng EndlessMaze.Tri

namespace EndlessMaze
{
    // Các lớp Player, Monster, Maze, MazeGenerator, GameData đã được chuyển sang các file riêng biệt.

    public partial class EndlessMaze : Form
    {
        // =================================================================
        // BIẾN TOÀN CỤC VÀ CÁC ĐỐI TƯỢNG GAME
        // =================================================================

        private Timer gameTimer; // Timer chính điều khiển game loop
        private Player player; // Đối tượng người chơi (Được định nghĩa trong Character.cs)
        private List<Monster> monsters; // Danh sách quái vật (Được định nghĩa trong Character.cs)
        private Maze gameMaze; // Đối tượng mê cung (Được định nghĩa trong Maze.cs)
        private GameState currentGameState; // Trạng thái hiện tại của game

        // Biến theo dõi trạng thái input của người chơi
        private bool isMovingUp, isMovingDown, isMovingLeft, isMovingRight;
        private bool isAttacking;

        // >>> BIẾN LOGIC CHO TẤN CÔNG (ATTACK)
        private DateTime lastAttackTime = DateTime.MinValue;
        private const int ATTACK_COOLDOWN_MS = 500;
        private const int ATTACK_DAMAGE = 20;
        private const int ATTACK_RANGE = 40;
        private Rectangle attackHitbox;
        // <<< BIẾN LOGIC CHO TẤN CÔNG (ATTACK)

        // >>> DASH VÀ STAMINA (THỂ LỰC)
        private bool isDashing;
        private DateTime lastDashTime = DateTime.MinValue;
        private const int DASH_COOLDOWN_MS = 2000;
        private const int DASH_DURATION_MS = 100;
        private const int DASH_SPEED_MULTIPLIER = 3;
        private const int DASH_STAMINA_COST = 25;

        // BIẾN MÊ CUNG VÀ KÍCH THƯỚC Ô
        private const int TILE_SIZE = 40;
        private Point endPosition;
        // <<< DASH VÀ STAMINA (THỂ LỰC)

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

            // Khởi tạo hitbox tấn công rỗng ban đầu
            attackHitbox = Rectangle.Empty;

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
                    gameTimer.Stop();
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
            lastAttackTime = DateTime.MinValue;
            lastDashTime = DateTime.MinValue;

            // Khởi tạo người chơi
            player = new Player
            {
                X = 50,
                Y = 50,
                Health = 100,
                MaxHealth = 100,
                Speed = 5,
                Stamina = 100,
                MaxStamina = 100
            };

            // Khởi tạo mê cung và quái vật
            GenerateLevel();

            UpdateUI();

            // Bắt đầu game loop
            gameTimer.Start();
        }

        private void GenerateLevel()
        {
            // TÍNH TOÁN KÍCH THƯỚC MÊ CUNG DỰA TRÊN TILE_SIZE
            int cols = pbGameCanvas.Width / TILE_SIZE;
            int rows = pbGameCanvas.Height / TILE_SIZE;

            // KHỞI TẠO VÀ TẠO MÊ CUNG NGẪU NHIÊN
            gameMaze = new Maze(rows, cols, TILE_SIZE);

            // Gọi thuật toán Recursive Backtracking
            gameMaze.MazeData = MazeGenerator.GenerateMaze(rows, cols, out Point startTile, out Point endTile);

            // ĐẶT VỊ TRÍ NGƯỜI CHƠI VÀ ĐÍCH ĐẾN
            // Vị trí người chơi (căn giữa trong ô bắt đầu)
            player.X = startTile.X * TILE_SIZE + (TILE_SIZE - player.Width) / 2;
            player.Y = startTile.Y * TILE_SIZE + (TILE_SIZE - player.Height) / 2;

            // Vị trí kết thúc
            endPosition = new Point(endTile.X * TILE_SIZE, endTile.Y * TILE_SIZE);

            // Tạo quái vật dựa trên level hiện tại
            monsters = new List<Monster>();
            Random rand = new Random();
            int monsterCount = 3 + level * 2;

            for (int i = 0; i < monsterCount; i++)
            {
                int mx, my;
                // Đảm bảo quái vật không đặt ở tường, điểm bắt đầu hoặc điểm kết thúc
                do
                {
                    mx = rand.Next(1, cols - 1);
                    my = rand.Next(1, rows - 1);
                } while (gameMaze.MazeData[my, mx] == 1 ||
                         (mx == startTile.X && my == startTile.Y) ||
                         (mx == endTile.X && my == endTile.Y));

                monsters.Add(new Monster
                {
                    X = mx * TILE_SIZE + (TILE_SIZE - 30) / 2, // Căn giữa quái vật trong ô
                    Y = my * TILE_SIZE + (TILE_SIZE - 30) / 2,
                    Health = 20 + (level * 5),
                    MaxHealth = 20 + (level * 5),
                    Speed = 2 + (level / 2)
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
            int oldX = player.X;
            int oldY = player.Y;
            int baseSpeed = player.Speed;

            // LOGIC DASH: Áp dụng tốc độ lướt nếu đang trong thời gian Dash
            if (isDashing)
            {
                if ((DateTime.Now - lastDashTime).TotalMilliseconds < DASH_DURATION_MS)
                {
                    baseSpeed *= DASH_SPEED_MULTIPLIER;
                }
                else
                {
                    isDashing = false; // Kết thúc Dash
                }
            }

            // Tính toán vị trí tiếp theo tiềm năng
            int nextX = oldX;
            int nextY = oldY;
            if (isMovingUp) nextY -= baseSpeed;
            if (isMovingDown) nextY += baseSpeed;
            if (isMovingLeft) nextX -= baseSpeed;
            if (isMovingRight) nextX += baseSpeed;

            // XỬ LÝ VA CHẠM TƯỜNG (SLIDE MOVEMENT)

            // 1. Thử di chuyển theo X và kiểm tra va chạm tường
            player.X = nextX;
            if (gameMaze.CheckWallCollision(player.GetBounds()))
            {
                player.X = oldX; // Giữ nguyên X nếu va chạm
            }

            // 2. Thử di chuyển theo Y và kiểm tra va chạm tường
            player.Y = nextY;
            if (gameMaze.CheckWallCollision(player.GetBounds()))
            {
                player.Y = oldY; // Giữ nguyên Y nếu va chạm
            }

            // Giới hạn di chuyển trong canvas (va chạm tường biên)
            player.X = Math.Max(0, Math.Min(pbGameCanvas.Width - player.Width, player.X));
            player.Y = Math.Max(0, Math.Min(pbGameCanvas.Height - player.Height, player.Y));

            // LOGIC HỒI PHỤC THỂ LỰC (STAMINA)
            if (!isDashing && player.Stamina < player.MaxStamina)
            {
                // Hồi 1 điểm Stamina mỗi tick (~60/s)
                player.Stamina = Math.Min(player.MaxStamina, player.Stamina + 1);
            }

            // LOGIC XỬ LÝ TẤN CÔNG VÀ HỒI CHIÊU
            attackHitbox = Rectangle.Empty; // Reset vùng tấn công mỗi tick
            TimeSpan timeSinceLastAttack = DateTime.Now - lastAttackTime;

            if (isAttacking && timeSinceLastAttack.TotalMilliseconds >= ATTACK_COOLDOWN_MS)
            {
                lastAttackTime = DateTime.Now;

                // Xác định vùng tấn công hình vuông (tạm thời)
                int hitboxSize = player.Width + ATTACK_RANGE;
                attackHitbox = new Rectangle(
                    player.X + player.Width / 2 - hitboxSize / 2,
                    player.Y + player.Height / 2 - hitboxSize / 2,
                    hitboxSize,
                    hitboxSize
                );
                isAttacking = false;
            }
            else if (isAttacking)
            {
                isAttacking = false; // Reset cờ nếu đang hồi chiêu
            }
        }

        private void UpdateMonsterState()
        {
            // AI đơn giản: di chuyển về phía người chơi (Đã cập nhật kiểm tra va chạm tường)
            foreach (var monster in monsters)
            {
                int nextX = monster.X;
                int nextY = monster.Y;

                // Tính toán vị trí tiếp theo tiềm năng
                if (monster.X < player.X) nextX += monster.Speed;
                else if (monster.X > player.X) nextX -= monster.Speed;

                if (monster.Y < player.Y) nextY += monster.Speed;
                else if (monster.Y > player.Y) nextY -= monster.Speed;

                // Thử di chuyển X và kiểm tra va chạm tường
                Rectangle nextBoundsX = new Rectangle(nextX, monster.Y, monster.Width, monster.Height);
                if (!gameMaze.CheckWallCollision(nextBoundsX))
                {
                    monster.X = nextX;
                }

                // Thử di chuyển Y và kiểm tra va chạm tường
                Rectangle nextBoundsY = new Rectangle(monster.X, nextY, monster.Width, monster.Height);
                if (!gameMaze.CheckWallCollision(nextBoundsY))
                {
                    monster.Y = nextY;
                }
            }
        }

        private void HandleCollisions()
        {
            // 1. Va chạm Tấn công (Player Attack -> Monster)
            if (attackHitbox != Rectangle.Empty)
            {
                for (int i = monsters.Count - 1; i >= 0; i--)
                {
                    Monster monster = monsters[i];

                    if (attackHitbox.IntersectsWith(monster.GetBounds()))
                    {
                        monster.Health -= ATTACK_DAMAGE; // Quái bị trúng đòn

                        if (monster.Health <= 0)
                        {
                            monsters.RemoveAt(i);
                            score += 10; // Tăng điểm
                        }
                    }
                }
            }

            // 2. Va chạm Quái vật -> Người chơi (Monster -> Player)
            for (int i = 0; i < monsters.Count; i++)
            {
                if (player.GetBounds().IntersectsWith(monsters[i].GetBounds()))
                {
                    // Người chơi mất máu khi chạm vào quái
                    player.Health -= 1;
                }
            }
        }

        private void CheckGameConditions()
        {
            // Kiểm tra nếu người chơi hết máu -> GameOver
            if (player.Health <= 0)
            {
                SwitchGameState(GameState.GameOver);
            }

            // Kiểm tra nếu người chơi chạm đến đích (End Position)
            Rectangle playerBounds = player.GetBounds();
            Rectangle endBounds = new Rectangle(endPosition.X, endPosition.Y, TILE_SIZE, TILE_SIZE);
            if (playerBounds.IntersectsWith(endBounds))
            {
                level++;
                score += 100; // Thưởng điểm khi hoàn thành màn
                GenerateLevel();
                levelStartTime = DateTime.Now; // Reset thời gian cho màn mới
            }
        }

        private void UpdateUI()
        {
            // Cập nhật hiển thị Stamina (Thể lực)
            lblPlayerHP.Text = $"HP: {player.Health} / {player.MaxHealth} | Thể lực: {player.Stamina}";
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

            // Vẽ mê cung và Cổng kết thúc
            gameMaze?.Draw(g, endPosition);

            // Vẽ vùng tấn công (để debug)
            if (attackHitbox != Rectangle.Empty)
            {
                g.FillEllipse(new SolidBrush(Color.FromArgb(50, Color.Yellow)), attackHitbox);
            }

            // Vẽ quái vật
            foreach (var monster in monsters)
            {
                monster.Draw(g);
            }

            // Vẽ người chơi
            player?.Draw(g);

            // Vẽ thanh Stamina
            player?.DrawStamina(g);
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
                    // LOGIC DASH CÓ HỒI CHIÊU VÀ THỂ LỰC
                    TimeSpan timeSinceLastDash = DateTime.Now - lastDashTime;
                    if (!isDashing && timeSinceLastDash.TotalMilliseconds >= DASH_COOLDOWN_MS && player.Stamina >= DASH_STAMINA_COST)
                    {
                        isDashing = true;
                        lastDashTime = DateTime.Now;
                        player.Stamina -= DASH_STAMINA_COST;
                    }
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

        private void pbGameCanvas_Click(object sender, EventArgs e)
        {

        }

        private void EndlessMaze_Load(object sender, EventArgs e)
        {

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
}
