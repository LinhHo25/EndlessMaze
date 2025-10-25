using System;
using System.Drawing;
using System.Drawing.Text; // Thêm
using System.Runtime.InteropServices; // Thêm
using System.Windows.Forms;
using Main.Tri; // Thêm

namespace Main
{
    public partial class frmMenu : Form
    {
        private frmMain _mainMenuForm;
        private IGameMap _gameMap;
        private Player _playerRef; // Thêm biến lưu tham chiếu Player

        // Biến cho custom font
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font pixelFont;

        // Sửa constructor để nhận Player
        public frmMenu(frmMain mainMenuForm, IGameMap mapForm)
        {
            InitializeComponent();
            _mainMenuForm = mainMenuForm;
            _gameMap = mapForm;
            _playerRef = mapForm.Player; // Lấy tham chiếu Player từ map

            LoadPixelFont();
            ApplyPixelStyle();

            // Bật KeyPreview để bắt phím ESC
            this.KeyPreview = true;
            this.KeyDown += FrmMenu_KeyDown; // Đăng ký sự kiện KeyDown
        }

        private void LoadPixelFont()
        {
            try
            {
                string fontPath = "pixel.ttf";
                if (System.IO.File.Exists(fontPath))
                {
                    byte[] fontData = System.IO.File.ReadAllBytes(fontPath);
                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                    Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                    uint dummy = 0;
                    fonts.AddMemoryFont(fontPtr, fontData.Length);
                    AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
                    Marshal.FreeCoTaskMem(fontPtr);
                    pixelFont = new Font(fonts.Families[0], 12F);
                }
                else
                {
                    pixelFont = new Font("Arial", 10F, FontStyle.Bold); // Font dự phòng
                }
            }
            catch
            {
                pixelFont = new Font("Arial", 10F, FontStyle.Bold); // Font dự phòng
            }
        }

        private void ApplyPixelStyle()
        {
            this.BackColor = Color.FromArgb(204, 179, 132); // Màu giấy da
            lblPaused.ForeColor = Color.FromArgb(50, 50, 50); // Màu chữ tối
            lblVolume.ForeColor = Color.FromArgb(50, 50, 50);

            lblPaused.Font = new Font(pixelFont.FontFamily, 16F, FontStyle.Bold);
            lblVolume.Font = new Font(pixelFont.FontFamily, 10F);

            // Áp dụng font và màu cho các nút
            ApplyButtonStyle(btnItem);
            ApplyButtonStyle(btnStatus);
            ApplyButtonStyle(btnEquipment);
            ApplyButtonStyle(btnSaveLoad);
            ApplyButtonStyle(btnMainMenu);

            // Trackbar (có thể tùy chỉnh thêm)
            trackBarVolume.BackColor = this.BackColor;
        }

        // Hàm helper để áp dụng style cho nút
        private void ApplyButtonStyle(Button btn)
        {
            btn.Font = new Font(pixelFont.FontFamily, 12F, FontStyle.Bold);
            btn.BackColor = Color.FromArgb(52, 143, 118); // Xanh lá
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            // Thêm hiệu ứng khi hover (tùy chọn)
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(72, 163, 138);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 143, 118);
        }

        // Sự kiện KeyDown để bắt phím ESC
        private void FrmMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close(); // Đóng form menu (sẽ resume game)
                e.Handled = true; // Ngăn không cho form map xử lý ESC nữa
            }
        }

        // Nút ITEM (ĐÃ SỬA)
        private void btnItem_Click(object sender, EventArgs e)
        {
            if (_playerRef != null) // Kiểm tra player không bị null
            {
                frmItem itemForm = new frmItem(_playerRef); // Truyền player vào
                itemForm.ShowDialog(this); // Hiển thị form item
            }
            else
            {
                MessageBox.Show("Lỗi: Không tìm thấy thông tin người chơi.");
            }
        }

        // Nút STATUS
        private void btnStatus_Click(object sender, EventArgs e)
        {
            frmStatus statusForm = new frmStatus();
            statusForm.ShowDialog(this);
        }

        // Nút EQUIPMENT
        private void btnEquipment_Click(object sender, EventArgs e)
        {
            frmEquipment equipForm = new frmEquipment();
            equipForm.ShowDialog(this);
        }

        // Nút SAVE/LOAD
        private void btnSaveLoad_Click(object sender, EventArgs e)
        {
            frmSaveLoadMenu saveLoadForm = new frmSaveLoadMenu(_mainMenuForm, _gameMap);
            saveLoadForm.ShowDialog(this);
        }

        // Nút MAIN MENU
        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc muốn thoát về Menu chính?\nMọi tiến trình chưa lưu sẽ bị mất.",
                                     "Xác nhận thoát",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                _gameMap.ResumeGame(); // Đảm bảo game map không bị pause nữa
                ((Form)_gameMap).Close(); // Đóng form game map hiện tại
                _mainMenuForm.Show(); // Hiện lại menu chính
                this.Close(); // Đóng form menu này
            }
        }

        // Thanh trượt Âm lượng
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            // TODO: Thêm code xử lý âm lượng ở đây
            // Ví dụ: SoundManager.MasterVolume = trackBarVolume.Value / 10f;
            // (Giả sử Value từ 0-10, Volume từ 0.0-1.0)
            Console.WriteLine("Âm lượng: " + trackBarVolume.Value); // In ra console để test
        }

        // Đảm bảo game resume khi form menu đóng (bằng nút X hoặc ESC)
        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            _gameMap.ResumeGame();
        }
    }
}

