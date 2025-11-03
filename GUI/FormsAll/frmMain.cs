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
using System.Drawing.Drawing2D;
using System.Drawing.Text; // <-- Thư viện cho font
using System.Drawing.Imaging; // <-- MỚI: Thêm thư viện này để sửa lỗi GDI+

namespace Main
{
    public partial class frmMain : System.Windows.Forms.Form
    {
        // --- Biến cho nhân vật (Swordsman) ---
        Image menuCharacterImage;
        float menuCharacterX;
        float menuCharacterY;
        int menuCharacterSpeed = 3;
        AnimationActivity menuWalkRightActivity;
        int charWidth = 100;
        int charHeight = 100;

        // --- Biến cho Slime ---
        Image menuSlimeImage;
        float menuSlimeX;
        float menuSlimeY;
        float menuSlimeSpeedX = -2;
        float menuSlimeSpeedY = -1;
        AnimationActivity menuSlimeStandActivity;
        int slimeWidth = 300;
        int slimeHeight = 300;

        // --- Biến cho logic "lăn bánh xe" ---
        private int slimeFlipIndex = 0;
        private RotateFlipType[] slimeFlips = {
            RotateFlipType.RotateNoneFlipNone,
            RotateFlipType.Rotate90FlipNone,
            RotateFlipType.Rotate180FlipNone,
            RotateFlipType.Rotate270FlipNone
        };
        private int slimeRotateTimer = 0;
        private int slimeRotateSpeed = 25;

        // --- SỬA: Cần 2 biến lưu font ---
        private PrivateFontCollection customFonts = new PrivateFontCollection();
        private FontFamily titleFontFamily; // Font cho lblTitle (ANDALAS)
        private FontFamily buttonFontFamily; // Font cho 3 nút (Pixelletters)


        public frmMain()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.DoubleBuffered = true;

            // --- Tải font tùy chỉnh ---
            LoadCustomFont(); // Hàm này giờ sẽ tải CẢ HAI font

            // --- SỬA: Áp dụng font cho lblTitle (giữ nguyên ANDALAS) ---
            if (titleFontFamily != null)
            {
                // Giữ nguyên font ANDALAS 65F Regular của bạn
                this.lblTitle.Font = new Font(titleFontFamily, 65F, FontStyle.Regular);
            }
            // --- HẾT PHẦN SỬA ---

            // --- SỬA: Áp dụng font cho các nút (dùng Pixelletters) ---
            if (buttonFontFamily != null)
            {
                // Dùng font Pixel cho 3 nút
                this.btnLogin.Font = new Font(buttonFontFamily, 18F, FontStyle.Bold);
                this.btnLeaderboard.Font = new Font(buttonFontFamily, 18F, FontStyle.Bold);
                this.btnExit.Font = new Font(buttonFontFamily, 18F, FontStyle.Bold);
            }
            // --- HẾT PHẦN SỬA ---

            SetUpMenuAnimation();
            SetUpSlimeAnimation();

            menuTimer.Start();
        }

        // --- SỬA: Hàm tải CẢ HAI font tùy chỉnh (Logic mới, an toàn hơn) ---
        private void LoadCustomFont()
        {
            try
            {
                // Đường dẫn tới 2 file font
                string titleFontPath = Path.Combine("Fonts", "ANDALAS.ttf");
                string buttonFontPath = Path.Combine("Fonts", "Pixellettersfull-BnJ5.ttf");

                // Tải Font Tiêu đề (ANDALAS)
                if (File.Exists(titleFontPath))
                {
                    customFonts.AddFontFile(titleFontPath);
                    // Lấy font vừa được thêm vào (nó sẽ nằm ở vị trí cuối cùng)
                    titleFontFamily = customFonts.Families[customFonts.Families.Length - 1];
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy file font: {titleFontPath}.");
                }

                // Tải Font Nút (Pixelletters)
                if (File.Exists(buttonFontPath))
                {
                    customFonts.AddFontFile(buttonFontPath);
                    // Lấy font vừa được thêm vào (nó sẽ nằm ở vị trí cuối cùng)
                    buttonFontFamily = customFonts.Families[customFonts.Families.Length - 1];
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy file font: {buttonFontPath}.");
                }

                // Gán dự phòng nếu font bị lỗi
                if (titleFontFamily == null) titleFontFamily = new FontFamily("Arial");
                if (buttonFontFamily == null) buttonFontFamily = new FontFamily("Arial");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải font: " + ex.Message);
            }
        }
        // --- HẾT PHẦN SỬA ---

        // --- Hàm thiết lập hoạt ảnh cho Swordsman ---
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

                menuCharacterX = -charWidth;
                menuCharacterY = this.ClientSize.Height - charHeight - 10;
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
                string slimeRoot = Path.Combine("ImgSource", "Char", "Monster", "Slime", "Slime_Water", "Stand");

                menuSlimeStandActivity = new AnimationActivity(8);

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

                menuSlimeX = this.ClientSize.Width;
                menuSlimeY = this.ClientSize.Height;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SetUpSlimeAnimation: " + ex.Message);
            }
        }

        // --- SỬA: Hàm Tick của Timer (Sửa lỗi Memory Leak) ---
        private void MenuTimer_Tick(object sender, EventArgs e)
        {
            // --- 1. Cập nhật Swordsman ---
            menuCharacterX += menuCharacterSpeed;
            if (menuCharacterX > this.ClientSize.Width)
            {
                menuCharacterX = -charWidth;
            }

            // --- SỬA LỖI GDI+: Giải phóng (Dispose) ảnh cũ trước khi lấy ảnh mới ---
            menuCharacterImage?.Dispose();
            menuCharacterImage = menuWalkRightActivity.GetNextFrame("right");
            // --- HẾT PHẦN SỬA ---


            // --- 2. Cập nhật Slime ---
            menuSlimeX += menuSlimeSpeedX; // Đi trái
            menuSlimeY += menuSlimeSpeedY; // Đi LÊN

            if (menuSlimeX < -slimeWidth || menuSlimeY < -slimeHeight)
            {
                menuSlimeX = this.ClientSize.Width; // Reset về bên phải
                menuSlimeY = this.ClientSize.Height; // Reset về bên DƯỚI
            }

            // Logic "lật/xoay" Slime
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
            this.Invalidate();
        }

        // --- SỬA: Sự kiện Paint (Sửa lỗi GDI+ RotateFlip) ---
        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;

            // 1. Vẽ Swordsman
            if (menuCharacterImage != null)
            {
                canvas.DrawImage(menuCharacterImage, (int)menuCharacterX, (int)menuCharacterY, charWidth, charHeight);
            }

            // 2. Vẽ Slime
            if (menuSlimeImage != null)
            {
                // --- SỬA LỖI GDI+ ---
                // Tạo một bitmap 32bpp mới và vẽ ảnh gốc lên đó.
                // Điều này chuẩn hóa định dạng pixel và cho phép RotateFlip.
                using (Bitmap imageToDraw = new Bitmap(slimeWidth, slimeHeight, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(imageToDraw))
                    {
                        // Vẽ ảnh gốc (menuSlimeImage) lên bitmap mới
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.DrawImage(menuSlimeImage, 0, 0, slimeWidth, slimeHeight);
                    }

                    // Bây giờ xoay (RotateFlip) an toàn
                    imageToDraw.RotateFlip(slimeFlips[slimeFlipIndex]);

                    // Vẽ ảnh đã xoay
                    canvas.DrawImage(imageToDraw, (int)menuSlimeX, (int)menuSlimeY, slimeWidth, slimeHeight);
                }
                // imageToDraw và g sẽ tự động được Dispose.
                // --- HẾT PHẦN SỬA ---
            }
        }

        // --- SỬA LỖI: Đã xóa hàm Dispose() bị trùng lặp ở đây ---

        // (Code các nút bấm của bạn giữ nguyên)
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

