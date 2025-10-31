using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D; // <-- THÊM: Cần cho InterpolationMode

namespace Main
{
    public partial class frmMain : System.Windows.Forms.Form
    {
        // ... (Biến của Swordsman không đổi) ...
        // --- Biến cho nhân vật (Swordsman) ---
        Image menuCharacterImage;
        float menuCharacterX;
        float menuCharacterY;
        int menuCharacterSpeed = 3;
        AnimationActivity menuWalkRightActivity;
        // --- SỬA: Thêm biến kích thước ---
        int charWidth = 100;
        int charHeight = 100;

        // --- Biến cho Slime ---
        Image menuSlimeImage;
        float menuSlimeX;
        float menuSlimeY;
        // --- SỬA: Hướng di chuyển chéo (Trái và Lên) ---
        float menuSlimeSpeedX = -2;
        float menuSlimeSpeedY = -1; // Đổi từ 0 sang -1
        AnimationActivity menuSlimeStandActivity;
        // --- SỬA: Thêm biến kích thước ---
        int slimeWidth = 300;
        int slimeHeight = 300;

        // --- Biến cho logic "lăn bánh xe" ---
        private int slimeFlipIndex = 0;
        // ... (Biến slimeFlips không đổi) ...
        private RotateFlipType[] slimeFlips = {
            RotateFlipType.RotateNoneFlipNone,  // 0 độ (Bình thường)
            RotateFlipType.Rotate90FlipNone,   // 90 độ (Xoay phải)
            RotateFlipType.Rotate180FlipNone,  // 180 độ (Lộn ngược)
            RotateFlipType.Rotate270FlipNone   // 270 độ (Xoay trái)
        };
        private int slimeRotateTimer = 0;
        private int slimeRotateSpeed = 25; // Tốc độ lật

        public frmMain()
        // ... (Hàm frmMain() không đổi) ...
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.DoubleBuffered = true;

            SetUpMenuAnimation();
            SetUpSlimeAnimation();

            // Giả định 'menuTimer' đã được tạo trong frmMain.Designer.cs
            menuTimer.Start();
        }

        // --- Hàm thiết lập hoạt ảnh cho Swordsman ---
        // ... (Hàm SetUpMenuAnimation() không đổi) ...
        private void SetUpMenuAnimation()
        {
            try
            {
                string playerRoot = Path.Combine("ImgSource", "Char", "Player", "SwordMan");
                string walkRoot = Path.Combine(playerRoot, "Walk");

                menuWalkRightActivity = new AnimationActivity(6);

                menuWalkRightActivity.LoadImages(
                    null, null, null,
                    Path.Combine(walkRoot, "Right")
                );

                menuCharacterImage = menuWalkRightActivity.GetDefaultFrame("right");

                menuCharacterX = -charWidth; // Sửa: Dùng biến
                // SỬA: Dùng ClientSize.Height để có vị trí chính xác
                menuCharacterY = this.ClientSize.Height - charHeight - 50;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SetUpMenuAnimation: " + ex.Message);
            }
        }

        // --- Hàm thiết lập hoạt ảnh cho Slime ---
        private void SetUpSlimeAnimation()
        {
            try
            {
                // ... (Tải ảnh Slime không đổi) ...
                string slimeRoot = Path.Combine("ImgSource", "Char", "Monster", "Slime", "Slime_Water", "Stand");

                menuSlimeStandActivity = new AnimationActivity(8);

                // Chỉ tải 1 hoạt ảnh (Front)
                menuSlimeStandActivity.LoadImages(
                    null,
                    Path.Combine(slimeRoot, "Front"), // Chỉ tải Front
                    null,
                    null
                );

                menuSlimeImage = menuSlimeStandActivity.GetDefaultFrame("down");

                if (menuSlimeImage == null)
                {
                    MessageBox.Show("LỖI: Không tải được ảnh Slime. Kiểm tra đường dẫn '.../Stand/Front'");
                }

                // SỬA: Dùng ClientSize.Width
                menuSlimeX = this.ClientSize.Width;
                // SỬA: Bắt đầu từ góc DƯỚI-PHẢI
                menuSlimeY = this.ClientSize.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SetUpSlimeAnimation: " + ex.Message);
            }
        }

        // --- Hàm Tick của Timer (Cập nhật cho cả 2) ---
        private void MenuTimer_Tick(object sender, EventArgs e)
        {
            // ... (Logic Swordsman không đổi) ...
            // --- 1. Cập nhật Swordsman ---
            menuCharacterX += menuCharacterSpeed;
            // SỬA: Dùng ClientSize.Width
            if (menuCharacterX > this.ClientSize.Width)
            {
                menuCharacterX = -charWidth;
            }
            menuCharacterImage = menuWalkRightActivity.GetNextFrame("right");

            // --- 2. Cập nhật Slime ---
            menuSlimeX += menuSlimeSpeedX; // Đi trái
            menuSlimeY += menuSlimeSpeedY; // Đi LÊN

            // SỬA: Reset nếu đi ra khỏi lề TRÁI hoặc lề TRÊN
            if (menuSlimeX < -slimeWidth || menuSlimeY < -slimeHeight)
            {
                menuSlimeX = this.ClientSize.Width; // Reset về bên phải
                menuSlimeY = this.ClientSize.Height; // Reset về bên DƯỚI
            }

            // Logic "lật/xoay" Slime
            // ... (Logic xoay Slime không đổi) ...
            slimeRotateTimer++;
            if (slimeRotateTimer > slimeRotateSpeed)
            {
                slimeRotateTimer = 0;
                slimeFlipIndex++;
                if (slimeFlipIndex >= slimeFlips.Length)
                {
                    slimeFlipIndex = 0;
                }
            }

            // (Đã vô hiệu hóa)
            // menuSlimeImage = menuSlimeStandActivity.GetNextFrame("down"); 

            // --- 3. Yêu cầu vẽ lại ---
            // ... (Invalidate() không đổi) ...
            this.Invalidate();
        }

        // --- Sự kiện Paint để vẽ (Cập nhật cho Slime) ---
        // ... (Hàm frmMain_Paint() không đổi) ...
        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;

            // 1. Vẽ Swordsman
            if (menuCharacterImage != null)
            {
                // SỬA: Dùng biến
                canvas.DrawImage(menuCharacterImage, (int)menuCharacterX, (int)menuCharacterY, charWidth, charHeight);
            }

            // 2. Vẽ Slime
            if (menuSlimeImage != null)
            {
                Image imageToDraw = (Image)menuSlimeImage.Clone();
                imageToDraw.RotateFlip(slimeFlips[slimeFlipIndex]);

                // SỬA: Dùng biến
                canvas.DrawImage(imageToDraw, (int)menuSlimeX, (int)menuSlimeY, slimeWidth, slimeHeight);

                imageToDraw.Dispose();
            }
        }

        // (Code các nút bấm của bạn giữ nguyên)
        // ... (Các hàm sự kiện nút bấm không đổi) ...
        private void btnActionMode_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmDangKy dangKyForm = new frmDangKy(this);
            dangKyForm.Show();
        }

        private void btnExploreMode_Click(object sender, EventArgs e)
        {
            // (Đã comment)
        }

        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            frmLeaderboard leaderboardForm = new frmLeaderboard();
            leaderboardForm.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

