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
            // Thêm kiểm tra null trước khi truy cập Player
            _playerRef = mapForm?.Player; // Lấy tham chiếu Player từ map

            LoadPixelFont();
            ApplyPixelStyle();

            // --- XÓA XỬ LÝ ESCAPE TRONG frmMenu ---
            // this.KeyPreview = true; // Không cần nữa
            // this.KeyDown -= FrmMenu_KeyDown; // Gỡ bỏ (nếu đã đăng ký trong Designer)
            // this.KeyDown += FrmMenu_KeyDown; // Xóa dòng này
            // ------------------------------------
        }

        private void LoadPixelFont()
        {
            try
            {
                string fontPath = "pixel.ttf"; // Đảm bảo file này có trong thư mục output
                if (System.IO.File.Exists(fontPath))
                {
                    byte[] fontData = System.IO.File.ReadAllBytes(fontPath);
                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                    Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                    uint dummy = 0;
                    fonts.AddMemoryFont(fontPtr, fontData.Length);
                    AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
                    Marshal.FreeCoTaskMem(fontPtr);
                    // Cập nhật: Tạo font với kích thước mặc định trước
                    pixelFont = new Font(fonts.Families[0], 12F); // Kích thước mặc định
                }
                else
                {
                    Console.WriteLine("Warning: pixel.ttf not found. Using default font.");
                    pixelFont = new Font("Arial", 10F, FontStyle.Bold); // Font dự phòng
                }
            }
            catch (Exception ex) // Bắt lỗi cụ thể hơn
            {
                Console.WriteLine($"Error loading pixel font: {ex.Message}");
                pixelFont = new Font("Arial", 10F, FontStyle.Bold); // Font dự phòng
            }
        }

        private void ApplyPixelStyle()
        {
            // Kiểm tra pixelFont null trước khi dùng
            if (pixelFont == null) LoadPixelFont(); // Thử tải lại nếu null
            if (pixelFont == null) return; // Thoát nếu vẫn null

            this.BackColor = Color.FromArgb(204, 179, 132); // Màu giấy da
            lblPaused.ForeColor = Color.FromArgb(50, 50, 50); // Màu chữ tối
            lblVolume.ForeColor = Color.FromArgb(50, 50, 50);

            // Sử dụng Clone() để tạo font mới với kích thước khác nhau
            lblPaused.Font = new Font(pixelFont.FontFamily, 16F, FontStyle.Bold); // Clone không cần ép kiểu
            lblVolume.Font = new Font(pixelFont.FontFamily, 10F); // Clone không cần ép kiểu


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
            if (pixelFont == null) return; // Kiểm tra null
            // Sử dụng Clone()
            btn.Font = new Font(pixelFont.FontFamily, 12F, FontStyle.Bold); // Clone không cần ép kiểu
            btn.BackColor = Color.FromArgb(52, 143, 118); // Xanh lá
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            // Thêm hiệu ứng khi hover (tùy chọn)
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(72, 163, 138);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 143, 118);
        }

        // --- XÓA HÀM XỬ LÝ ESCAPE TRONG frmMenu ---
        // private void FrmMenu_KeyDown(object sender, KeyEventArgs e) { ... }
        // ------------------------------------

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
            // Cần truyền Player vào frmStatus nếu muốn hiển thị chỉ số thực
            // frmStatus statusForm = new frmStatus(_playerRef);
            frmStatus statusForm = new frmStatus(); // Hiện tại chưa truyền
            statusForm.ShowDialog(this);
        }

        // Nút EQUIPMENT
        private void btnEquipment_Click(object sender, EventArgs e)
        {
            // Cần truyền Player vào frmEquipment nếu muốn hiển thị trang bị thực
            // frmEquipment equipForm = new frmEquipment(_playerRef);
            frmEquipment equipForm = new frmEquipment(); // Hiện tại chưa truyền
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
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Warning); // Thêm Icon
            if (confirmResult == DialogResult.Yes)
            {
                // Đảm bảo _gameMap không null
                if (_gameMap != null)
                {
                    _gameMap.ResumeGame(); // Đảm bảo game map không bị pause nữa (quan trọng!)
                                           // Kiểm tra kiểu trước khi ép kiểu và đóng
                    if (_gameMap is Form mapForm)
                    {
                        mapForm.Close(); // Đóng form game map hiện tại
                    }
                }
                // Đảm bảo _mainMenuForm không null
                if (_mainMenuForm != null)
                {
                    _mainMenuForm.Show(); // Hiện lại menu chính
                }
                this.Close(); // Đóng form menu này
            }
        }

        // Thanh trượt Âm lượng
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            // TODO: Thêm code xử lý âm lượng ở đây
            // Ví dụ: SoundManager.MasterVolume = trackBarVolume.Value / 10f;
            Console.WriteLine("Âm lượng: " + trackBarVolume.Value); // In ra console để test
        }

        // --- SỬA LỖI STACKOVERFLOW ---
        // Xóa bỏ việc gọi ResumeGame() ở đây để tránh lặp vô hạn
        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            // KHÔNG gọi _gameMap.ResumeGame() ở đây nữa.
            // Việc resume đã được thực hiện bởi map khi người dùng nhấn Esc
            // hoặc khi menu này bị đóng bởi code trong ResumeGame() của map.
        }
        // -----------------------------
    }
}

