using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq; // <-- Giữ từ Code 2 (cần cho ApplyStyles)
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BLL.Services; // <-- THÊM
using DAL.Models; // <-- THÊM
using static BLL.Services.GameSessionService; // <-- THÊM để dùng CalculatedStats

namespace Main
{
    // --- CHỨC NĂNG 4 (TỪ CODE 1) ---
    public partial class frmMenu : System.Windows.Forms.Form
    {
        // --- THÊM: Dữ liệu game ---
        private readonly PlayerCharacters _character;
        private readonly PlayerSessionInventory _inventory;
        private readonly CalculatedStats _stats;
        private readonly GameSessionService _gameSessionService;
        private int _currentHealth;

        // --- THÊM: Property để form game chính đọc máu mới sau khi dùng item ---
        public int UpdatedHealth { get; private set; }
        // -------------------------


        // Biến cho custom font
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font pixelFont;
        Font pixelFont_Button; // Font cho nút (Từ Code 2)

        // Constructor mặc định (cho Designer)
        public frmMenu()
        {
            InitializeComponent();
            LoadCustomFont(); // Tải font (Từ Code 2)
            ApplyStyles(); // Áp dụng style cơ bản (Từ Code 2)
        }

        // --- THÊM: Constructor chính để nhận dữ liệu game ---
        public frmMenu(PlayerCharacters character, PlayerSessionInventory inventory, CalculatedStats stats, GameSessionService gameSessionService, int currentHealth)
        {
            InitializeComponent();
            LoadCustomFont(); // Tải font (Từ Code 2)
            ApplyStyles(); // Áp dụng style (Từ Code 2)

            // Lưu dữ liệu
            _character = character;
            _inventory = inventory;
            _stats = stats;
            _gameSessionService = gameSessionService;
            _currentHealth = currentHealth;
            UpdatedHealth = currentHealth; // Khởi tạo máu
        }
        // ----------------------------------------------------

        // --- THÊM: Tải font (Từ Code 2) ---
        private void LoadCustomFont()
        {
            try
            {
                // (Tạm thời dùng font dự phòng)
                pixelFont = new Font("Segoe UI", 18F, FontStyle.Bold);
                pixelFont_Button = new Font("Segoe UI", 12F, FontStyle.Bold);
            }
            catch
            {
                pixelFont = new Font("Arial", 18F, FontStyle.Bold);
                pixelFont_Button = new Font("Arial", 12F, FontStyle.Bold);
            }
        }

        // --- SỬA: Hàm helper để áp dụng style (Từ Code 2) ---
        private void ApplyStyles()
        {
            // --- Style cho Form ---
            this.FormBorderStyle = FormBorderStyle.None; // Bỏ viền
            this.StartPosition = FormStartPosition.CenterParent; // Ra giữa
            this.BackColor = Color.FromArgb(20, 20, 30); // Màu nền xanh đậm/đen
            this.ForeColor = Color.White; // Chữ trắng

            // --- Style cho Tiêu đề (Label Paused) ---
            // (Giả sử bạn có một Label tên là 'lblPaused')
            Control lblPaused = this.Controls.Find("lblPaused", true).FirstOrDefault();
            if (lblPaused is Label label)
            {
                label.Font = pixelFont;
                label.ForeColor = Color.FromArgb(255, 215, 0); // Màu vàng Gold
                label.BackColor = Color.Transparent;
            }

            // --- Style cho các Nút bấm ---
            // (Tìm tất cả các nút trong form)
            foreach (Control control in this.Controls)
            {
                if (control is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 120); // Viền xám
                    btn.BackColor = Color.FromArgb(40, 40, 60); // Nền nút xanh đậm
                    btn.ForeColor = Color.FromArgb(220, 220, 255); // Chữ màu tím nhạt/trắng
                    btn.Font = pixelFont_Button;

                    // Thêm hiệu ứng Hover (rê chuột)
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = Color.FromArgb(60, 60, 90); // Sáng hơn
                        btn.FlatAppearance.BorderColor = Color.White; // Viền trắng
                    };
                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = Color.FromArgb(40, 40, 60); // Trở lại
                        btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 120);
                    };

                    // Nút Thoát (MainMenu) có màu đỏ
                    if (btn.Name.Contains("btnMainMenu"))
                    {
                        btn.BackColor = Color.FromArgb(100, 20, 20); // Nền đỏ đậm
                        btn.ForeColor = Color.White;
                        btn.MouseEnter += (s, e) => { btn.BackColor = Color.FromArgb(150, 30, 30); };
                        btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(100, 20, 20); };
                    }
                }
            }
        }


        // Nút ITEM (Giữ từ Code 2)
        private void btnItem_Click(object sender, EventArgs e)
        {
            if (_character == null) return; // Không có dữ liệu

            // Mở form Item và truyền dữ liệu
            frmItem itemForm = new frmItem(_inventory, _gameSessionService, _character.CharacterID, _currentHealth);
            itemForm.ShowDialog();

            // Lấy lại máu mới sau khi form Item đóng
            _currentHealth = itemForm.NewHealth;
            this.UpdatedHealth = itemForm.NewHealth;
        }

        // Nút STATUS (Giữ từ Code 2)
        private void btnStatus_Click(object sender, EventArgs e)
        {
            if (_character == null) return; // Không có dữ liệu

            // Mở form Status và truyền dữ liệu
            frmStatus statusForm = new frmStatus(_character, _stats, _currentHealth);
            statusForm.ShowDialog();
        }

        // Nút EQUIPMENT (Giữ từ Code 2)
        private void btnEquipment_Click(object sender, EventArgs e)
        {
            if (_character == null) return; // Không có dữ liệu

            // Mở form Equipment và truyền dữ liệu
            frmEquipment equipmentForm = new frmEquipment(_character, _inventory);
            equipmentForm.ShowDialog();
        }

        // --- CHỨC NĂNG 2 (TỪ CODE 1) ---
        // Nút SAVE/LOAD
        private void btnSaveLoad_Click(object sender, EventArgs e)
        {
            // Mở form Save/Load
            frmSaveLoadMenu saveLoadForm = new frmSaveLoadMenu();
            saveLoadForm.ShowDialog();
        }

        // Nút MAIN MENU (Giữ từ Code 2)
        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            // Hỏi người dùng có muốn thoát không
            var result = MessageBox.Show("Bạn có chắc muốn thoát về Menu chính?\nMọi tiến trình chưa lưu sẽ bị mất.", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // (Thêm logic để thoát game ở đây,
                // ví dụ: đặt một cờ (flag) để form game chính đọc)
                this.DialogResult = DialogResult.Abort; // Gửi tín hiệu 'Abort' về frmMazeGame
                this.Close();
            }
        }

        // Thanh trượt Âm lượng (Giữ từ Code 2)
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            // (Thêm logic điều chỉnh âm lượng ở đây)
            // Ví dụ: lblVolume.Text = $"Âm lượng: {trackBarVolume.Value * 10}%";
        }

        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            // (Không cần code ở đây, form game chính sẽ xử lý)
        }
    }
}