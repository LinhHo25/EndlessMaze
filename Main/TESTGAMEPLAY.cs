using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TESTT;

namespace Main
{
    public partial class TESTGAMEPLAY : Form
    {
        public TESTGAMEPLAY()
        {
            InitializeComponent();
            SetUp(); // Chạy hàm SetUp
        }

        // --- Các biến toàn cục ---
        Image player; // Ảnh hiện tại để vẽ

        // Các biến trạng thái di chuyển
        bool goRight, goLeft, goUp, goDown;
        string currentActivityState = "idle_down";
        string lastFacingDirection = "down";

        // Thông số nhân vật
        float playerX;
        float playerY;
        int playerHeight = 100;
        int playerWidth = 100;
        int playerSpeed = 6;
        int playerRunSpeed = 10;

        // --- Các đối tượng quản lý hoạt ảnh ---
        AnimationActivity walkActivity;
        AnimationActivity idleActivity;
        AnimationActivity runActivity;
        AnimationActivity attackActivity;       // Đứng yên đánh
        AnimationActivity walkAttackActivity;   // Đi bộ đánh
        AnimationActivity runAttackActivity;    // Chạy đánh
        AnimationActivity hurtActivity;
        AnimationActivity deathActivity;

        // --- Các biến trạng thái ---
        bool isRunning = false;
        bool isAttacking = false;
        bool isHurt = false;
        bool isDead = false;

        // --- MỚI: Biến trạng thái Dash ---
        bool isDashing = false;
        int dashTimer = 0;       // Thời gian lướt (số frame)
        int dashCooldown = 0;    // Thời gian hồi chiêu (số frame)
        float dashSpeed = 30f;   // Tốc độ lướt
        float dashDirectionX = 0;
        float dashDirectionY = 0;


        // --- Các hàm sự kiện (Đã đổi phím) ---

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = false;
            if (e.KeyCode == Keys.Right) goRight = false;
            if (e.KeyCode == Keys.Up) goUp = false;
            if (e.KeyCode == Keys.Down) goDown = false;
            // ĐỔI: Chạy bằng ControlKey
            if (e.KeyCode == Keys.ControlKey) isRunning = false;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (isDead) return;

            if (e.KeyCode == Keys.Left) goLeft = true;
            if (e.KeyCode == Keys.Right) goRight = true;
            if (e.KeyCode == Keys.Up) goUp = true;
            if (e.KeyCode == Keys.Down) goDown = true;
            // ĐỔI: Chạy bằng ControlKey
            if (e.KeyCode == Keys.ControlKey) isRunning = true;

            // Tấn công (Không đổi)
            if (e.KeyCode == Keys.Space && !isAttacking && !isHurt && !isDashing)
            {
                isAttacking = true;
                bool isMoving = goLeft || goRight || goUp || goDown;

                if (isRunning && isMoving) { runAttackActivity.ResetFrame(); }
                else if (isMoving) { walkAttackActivity.ResetFrame(); }
                else { attackActivity.ResetFrame(); }
            }

            // --- MỚI: Logic phím Dash (ShiftKey) ---
            if (e.KeyCode == Keys.ShiftKey && !isAttacking && !isHurt && !isDashing && dashCooldown <= 0)
            {
                float dashDirX = 0;
                float dashDirY = 0;

                // Lấy hướng di chuyển hiện tại
                if (goLeft) dashDirX = -1;
                if (goRight) dashDirX = 1;
                if (goUp) dashDirY = -1;
                if (goDown) dashDirY = 1;
                if (goLeft && goRight) dashDirX = 0;
                if (goUp && goDown) dashDirY = 0;

                bool isMoving = dashDirX != 0 || dashDirY != 0;

                if (isMoving) // Lướt theo hướng đang đi
                {
                    if (dashDirX != 0 && dashDirY != 0) // Chuẩn hóa nếu đi chéo
                    {
                        const float norm = 0.7071f;
                        dashDirectionX = dashDirX * norm;
                        dashDirectionY = dashDirY * norm;
                    }
                    else
                    {
                        dashDirectionX = dashDirX;
                        dashDirectionY = dashDirY;
                    }
                }
                else // Đứng yên, lướt theo hướng cuối cùng
                {
                    if (lastFacingDirection == "left") dashDirectionX = -1;
                    else if (lastFacingDirection == "right") dashDirectionX = 1;
                    else if (lastFacingDirection == "up") dashDirectionY = -1;
                    else if (lastFacingDirection == "down") dashDirectionY = 1;
                    else dashDirectionX = 1; // Mặc định lướt phải
                }

                isDashing = true;
                dashTimer = 5;      // Lướt trong 5 frame
                dashCooldown = 40;  // Hồi chiêu 40 frame
            }


            // Phím test (Không đổi)
            if (e.KeyCode == Keys.H && !isHurt) { isHurt = true; hurtActivity.ResetFrame(); }
            if (e.KeyCode == Keys.K && !isDead) { isDead = true; deathActivity.ResetFrame(); }
        }

        private void frmPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (player != null)
            {
                canvas.DrawImage(player, (int)playerX, (int)playerY, playerWidth, playerHeight);
            }
        }

        // --- HÀM SET UP (Không đổi) ---
        private void SetUp()
        {
            string bgPath = Path.Combine("Img_Source", "Background", "bunm.jpg");
            if (File.Exists(bgPath))
            {
                this.BackgroundImage = Image.FromFile(bgPath);
            }
            this.BackgroundImageLayout = ImageLayout.Stretch;

            try
            {
                string playerRoot = Path.Combine("Img_Source", "Player");

                // 1. Stand
                string idleRoot = Path.Combine(playerRoot, "Stand");
                idleActivity = new AnimationActivity(10);
                idleActivity.LoadImages(
                    Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"),
                    Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right")
                );

                // 2. Walk
                string walkRoot = Path.Combine(playerRoot, "Walk");
                walkActivity = new AnimationActivity(6);
                walkActivity.LoadImages(
                    Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"),
                    Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right")
                );

                // 3. Run
                string runRoot = Path.Combine(playerRoot, "Run");
                runActivity = new AnimationActivity(4);
                runActivity.LoadImages(
                    Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"),
                    Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right")
                );

                // 4. Attack (Stand)
                string attackRoot = Path.Combine(playerRoot, "Atk");
                attackActivity = new AnimationActivity(3);
                attackActivity.IsLooping = false;
                attackActivity.LoadImages(
                    Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"),
                    Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right")
                );

                // 5. Walk_Atk
                string walkAtkRoot = Path.Combine(playerRoot, "Walk_Atk");
                walkAttackActivity = new AnimationActivity(3);
                walkAttackActivity.IsLooping = false;
                walkAttackActivity.LoadImages(
                    Path.Combine(walkAtkRoot, "Back"), Path.Combine(walkAtkRoot, "Front"),
                    Path.Combine(walkAtkRoot, "Left"), Path.Combine(walkAtkRoot, "Right")
                );

                // 6. Run_Atk
                string runAtkRoot = Path.Combine(playerRoot, "Run_Atk");
                runAttackActivity = new AnimationActivity(3);
                runAttackActivity.IsLooping = false;
                runAttackActivity.LoadImages(
                    Path.Combine(runAtkRoot, "Back"), Path.Combine(runAtkRoot, "Front"),
                    Path.Combine(runAtkRoot, "Left"), Path.Combine(runAtkRoot, "Right")
                );

                // 7. Hurt
                string hurtRoot = Path.Combine(playerRoot, "Hurt");
                hurtActivity = new AnimationActivity(5);
                hurtActivity.IsLooping = false;
                hurtActivity.LoadImages(
                    Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"),
                    Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right")
                );

                // 8. Death
                string deathRoot = Path.Combine(playerRoot, "Death");
                deathActivity = new AnimationActivity(8);
                deathActivity.IsLooping = false;
                deathActivity.LoadImages(
                    Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"),
                    Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right")
                );

                player = idleActivity.GetDefaultFrame("down");

                if (player == null)
                {
                    MessageBox.Show("Không tìm thấy ảnh đứng yên (Front)! Hãy kiểm tra lại thư mục 'Img_Source/Player/Stand/Front'.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải ảnh: " + ex.Message);
            }

            playerX = 150;
            playerY = 150;
            this.DoubleBuffered = true;
        }

        // --- HÀM TIMER (GAME LOOP) - LOGIC ƯU TIÊN ĐÃ SỬA ---
        private void TimerEvent(object sender, EventArgs e)
        {
            // --- 0. QUẢN LÝ COOLDOWN ---
            if (dashCooldown > 0)
            {
                dashCooldown--;
            }

            // --- 1. XÁC ĐỊNH INPUT DI CHUYỂN ---
            float moveX = 0;
            float moveY = 0;
            bool isMovingInput = false; // Chỉ là input, chưa phải di chuyển thật

            // Chỉ nhận input di chuyển nếu không bị "khóa" (Chết, Bị thương, Lướt)
            if (!isDead && !isHurt && !isDashing)
            {
                if (goLeft) { moveX = -1; }
                if (goRight) { moveX = 1; }
                if (goUp) { moveY = -1; }
                if (goDown) { moveY = 1; }

                if (goLeft && goRight) moveX = 0;
                if (goUp && goDown) moveY = 0;

                if (moveX != 0 && moveY != 0)
                {
                    const float normalizationFactor = 0.7071f;
                    moveX *= normalizationFactor;
                    moveY *= normalizationFactor;
                }
            }
            isMovingInput = moveX != 0 || moveY != 0;

            // --- 2. CẬP NHẬT HOẠT ẢNH (LOGIC ƯU TIÊN) ---
            bool canMove = true; // Biến cờ, cho phép di chuyển hay không
            string newActivityState = "";
            string currentDirection = lastFacingDirection;

            // --- ƯU TIÊN 1: CHẾT ---
            if (isDead)
            {
                canMove = false;
                currentDirection = lastFacingDirection;
                newActivityState = "dead_" + currentDirection;
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; }
                player = deathActivity.GetNextFrame(currentDirection);
            }
            // --- ƯU TIÊN 2: BỊ THƯƠNG ---
            else if (isHurt)
            {
                canMove = false;
                currentDirection = lastFacingDirection;
                newActivityState = "hurt_" + currentDirection;
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; }
                player = hurtActivity.GetNextFrame(currentDirection);
                if (hurtActivity.IsFinished) { isHurt = false; }
            }
            // --- MỚI: ƯU TIÊN 3: LƯỚT (DASHING) ---
            else if (isDashing)
            {
                canMove = false; // Di chuyển sẽ được xử lý riêng
                currentDirection = lastFacingDirection; // Giữ hướng
                newActivityState = "run_" + currentDirection; // Dùng tạm hoạt ảnh 'Run'

                if (currentActivityState != newActivityState)
                {
                    currentActivityState = newActivityState;
                    runActivity.ResetFrame(); // Dùng hoạt ảnh run
                }
                player = runActivity.GetNextFrame(currentDirection);

                dashTimer--;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                }
            }
            // --- ƯU TIÊN 4: TẤN CÔNG ---
            else if (isAttacking)
            {
                currentDirection = lastFacingDirection;

                if (isRunning && isMovingInput) // Chạy-Đánh
                {
                    canMove = true;
                    newActivityState = "run_attack_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; }
                    player = runAttackActivity.GetNextFrame(currentDirection);
                    if (runAttackActivity.IsFinished) { isAttacking = false; }
                }
                else if (isMovingInput) // Đi-Đánh
                {
                    canMove = true;
                    newActivityState = "walk_attack_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; }
                    player = walkAttackActivity.GetNextFrame(currentDirection);
                    if (walkAttackActivity.IsFinished) { isAttacking = false; }
                }
                else // Đứng-Đánh
                {
                    canMove = false;
                    newActivityState = "attack_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; }
                    player = attackActivity.GetNextFrame(currentDirection);
                    if (attackActivity.IsFinished) { isAttacking = false; }
                }
            }
            // --- ƯU TIÊN 5: DI CHUYỂN (CHẠY / ĐI BỘ) ---
            else if (isMovingInput)
            {
                canMove = true;
                if (goRight) { currentDirection = "right"; }
                else if (goLeft) { currentDirection = "left"; }
                else if (goUp) { currentDirection = "up"; }
                else if (goDown) { currentDirection = "down"; }

                lastFacingDirection = currentDirection;

                if (isRunning)
                {
                    newActivityState = "run_" + currentDirection;
                    if (currentActivityState != newActivityState)
                    {
                        currentActivityState = newActivityState;
                        runActivity.ResetFrame();
                    }
                    player = runActivity.GetNextFrame(currentDirection);
                }
                else // Đang đi bộ
                {
                    newActivityState = "walk_" + currentDirection;
                    if (currentActivityState != newActivityState)
                    {
                        currentActivityState = newActivityState;
                        walkActivity.ResetFrame();
                    }
                    player = walkActivity.GetNextFrame(currentDirection);
                }
            }
            // --- ƯU TIÊN 6: ĐỨNG YÊN ---
            else
            {
                canMove = false; // Không có input di chuyển
                currentDirection = lastFacingDirection;
                newActivityState = "idle_" + currentDirection;

                if (currentActivityState != newActivityState)
                {
                    currentActivityState = newActivityState;
                    idleActivity.ResetFrame();
                }
                player = idleActivity.GetNextFrame(currentDirection);
            }

            // --- 3. CẬP NHẬT VỊ TRÍ (Đã sửa) ---
            if (isDashing) // Ưu tiên di chuyển lướt
            {
                playerX += dashDirectionX * dashSpeed;
                playerY += dashDirectionY * dashSpeed;
            }
            else if (canMove) // Di chuyển bình thường
            {
                float currentSpeed = (isRunning && isMovingInput) ? playerRunSpeed : playerSpeed;
                playerX += moveX * currentSpeed;
                playerY += moveY * currentSpeed;
            }

            label1.Text = "Anim: " + currentActivityState + " CD: " + dashCooldown;
            this.Invalidate();
        }
    }
}

