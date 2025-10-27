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
        string currentActivityState = "idle_down"; // Trạng thái đầy đủ (hành động + hướng)
        string lastFacingDirection = "down"; // Hướng cuối cùng mà nhân vật nhìn

        // --- THAY ĐỔI 1: Đổi sang float để tính toán chính xác ---
        float playerX;
        float playerY;
        int playerHeight = 100;
        int playerWidth = 100;
        int playerSpeed = 8;

        // --- MỚI: Các đối tượng quản lý hoạt ảnh ---
        AnimationActivity walkActivity;
        AnimationActivity idleActivity;

        // --- Các hàm sự kiện (Không đổi) ---

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = false;
            if (e.KeyCode == Keys.Right) goRight = false;
            if (e.KeyCode == Keys.Up) goUp = false;
            if (e.KeyCode == Keys.Down) goDown = false;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = true;
            if (e.KeyCode == Keys.Right) goRight = true;
            if (e.KeyCode == Keys.Up) goUp = true;
            if (e.KeyCode == Keys.Down) goDown = true;
        }

        // --- THAY ĐỔI 2: Ép kiểu về (int) khi vẽ ---
        private void frmPaintEvent(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            if (player != null) // Chỉ vẽ nếu 'player' không bị null
            {
                // Ép kiểu playerX/Y về (int) vì hàm DrawImage yêu cầu
                canvas.DrawImage(player, (int)playerX, (int)playerY, playerWidth, playerHeight);
            }
        }

        // --- Hàm logic chính (Không đổi) ---

        private void SetUp()
        {
            if (File.Exists("background.jpg"))
            {
                this.BackgroundImage = Image.FromFile("background.jpg");
            }
            this.BackgroundImageLayout = ImageLayout.Stretch;

            try
            {
                // --- MỚI: Khởi tạo và tải ảnh cho từng hoạt động ---

                // 1. Hoạt động đi bộ (Tốc độ 4, càng nhỏ càng nhanh)
                walkActivity = new AnimationActivity(4);
                walkActivity.LoadImages("player", "Back", "Front", "Left", "Right");

                // 2. Hoạt động đứng yên (Tốc độ 10, chậm hơn)
                idleActivity = new AnimationActivity(10);
                idleActivity.LoadImages("Swordsman",
                    "Swordsman_Stand_Back1",
                    "Swordsman_Stand_Font1",
                    "Swordsman_Stand_Left1",
                    "Swordsman_Stand_Right1");


                // Gán ảnh mặc định khi bắt đầu
                player = idleActivity.GetDefaultFrame("down"); // Lấy frame đầu tiên

                if (player == null)
                {
                    MessageBox.Show("Không tìm thấy ảnh đứng yên (Stand_Front)!");
                    // Tạo ảnh đỏ nếu lỗi
                    Bitmap bmp = new Bitmap(100, 100);
                    using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(Color.Red); }
                    player = bmp;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải ảnh: " + ex.Message);
            }

            // Gán vị trí ban đầu
            playerX = 150;
            playerY = 150;
            this.DoubleBuffered = true; // Bật chống giật
        }

        // --- THAY ĐỔI 3: Cập nhật logic TimerEvent ---
        private void TimerEvent(object sender, EventArgs e)
        {
            // --- 1. CẬP NHẬT VỊ TRÍ & HƯỚNG NHÌN ---

            // --- LOGIC MỚI ĐỂ CHUẨN HÓA TỐC ĐỘ ---
            float moveX = 0;
            float moveY = 0;

            // Xác định hướng di chuyển và hướng nhìn cuối cùng
            // (Giữ nguyên logic ưu tiên hướng của bạn)
            if (goLeft)
            {
                moveX = -1;
                lastFacingDirection = "left";
            }
            if (goRight)
            {
                moveX = 1;
                lastFacingDirection = "right";
            }
            if (goUp)
            {
                moveY = -1;
                lastFacingDirection = "up";
            }
            if (goDown)
            {
                moveY = 1;
                lastFacingDirection = "down";
            }

            // Hủy di chuyển nếu nhấn 2 phím đối nghịch
            if (goLeft && goRight) moveX = 0;
            if (goUp && goDown) moveY = 0;

            // Kiểm tra nếu đi chéo (moveX != 0 và moveY != 0)
            if (moveX != 0 && moveY != 0)
            {
                // Đây là hằng số 1 / Sqrt(2)
                const float normalizationFactor = 0.7071f;
                moveX *= normalizationFactor;
                moveY *= normalizationFactor;
            }

            // Áp dụng tốc độ vào hướng di chuyển
            playerX += moveX * playerSpeed;
            playerY += moveY * playerSpeed;

            // --- HẾT LOGIC MỚI ---

            // --- 2. CẬP NHẬT HOẠT ẢNH (ANIMATION) ---

            // Xác định 'isMoving' (chỉ di chuyển khi không nhấn 2 phím đối nghịch)
            bool isMoving = (goLeft || goRight || goUp || goDown)
                            && !(goLeft && goRight)
                            && !(goUp && goDown);

            string newActivityState = ""; // Trạng thái sẽ cập nhật

            if (isMoving)
            {
                // --- XỬ LÝ ĐI BỘ ---
                string moveDirection = ""; // Hướng cho hoạt ảnh

                // Xác định hướng ưu tiên (giống code cũ của bạn)
                if (goRight) { moveDirection = "right"; }
                else if (goLeft) { moveDirection = "left"; }
                else if (goUp) { moveDirection = "up"; }
                else if (goDown) { moveDirection = "down"; }

                // Cập nhật hướng nhìn cuối cùng DỰA TRÊN ƯU TIÊN HOẠT ẢNH
                // (Điều này quan trọng cho trạng thái 'idle')
                lastFacingDirection = moveDirection;

                newActivityState = "walk_" + moveDirection;

                // Nếu đổi trạng thái (ví dụ: idle -> walk, hoặc walk_left -> walk_right)
                if (currentActivityState != newActivityState)
                {
                    currentActivityState = newActivityState;
                    walkActivity.ResetFrame(); // Reset frame đếm
                }

                // Lấy frame tiếp theo từ đối tượng walkActivity
                player = walkActivity.GetNextFrame(moveDirection);
            }
            else
            {
                // --- XỬ LÝ ĐỨNG YÊN ---
                newActivityState = "idle_" + lastFacingDirection;

                // Nếu đổi trạng thái (ví dụ: walk -> idle)
                if (currentActivityState != newActivityState)
                {
                    currentActivityState = newActivityState;
                    idleActivity.ResetFrame(); // Reset frame đếm
                }

                // Lấy frame tiếp theo từ đối tượng idleActivity
                player = idleActivity.GetNextFrame(lastFacingDirection);
            }

            // Cập nhật Label (để debug)
            label1.Text = "Anim: " + currentActivityState;

            // YÊU CẦU VẼ LẠI FORM
            this.Invalidate();
        }
    }
}
