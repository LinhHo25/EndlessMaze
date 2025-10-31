using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DAL.Models; // <-- THÊM
using BLL.Services; // <-- THÊM
using static BLL.Services.GameSessionService; // <-- THÊM để dùng CalculatedStats

namespace Main
{
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

        // Constructor mặc định (cho Designer)
        public frmMenu()
        {
            InitializeComponent();
            ApplyStyles(); // Áp dụng style cơ bản
        }

        // --- THÊM: Constructor chính để nhận dữ liệu game ---
        public frmMenu(PlayerCharacters character, PlayerSessionInventory inventory, CalculatedStats stats, GameSessionService gameSessionService, int currentHealth)
        {
            InitializeComponent();
            ApplyStyles(); // Áp dụng style

            // Lưu dữ liệu
            _character = character;
            _inventory = inventory;
            _stats = stats;
            _gameSessionService = gameSessionService;
            _currentHealth = currentHealth;
            UpdatedHealth = currentHealth; // Khởi tạo máu
        }
        // ----------------------------------------------------

        // Hàm helper để áp dụng style (giữ code gọn gàng)
        private void ApplyStyles()
        {
            // (Bạn có thể thêm code tải font pixel và style nút ở đây)
            // Ví dụ:
            this.BackColor = Color.FromArgb(50, 50, 50);
            this.ForeColor = Color.White;
            lblPaused.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        }

        // Nút ITEM
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

        // Nút STATUS
        private void btnStatus_Click(object sender, EventArgs e)
        {
            if (_character == null) return; // Không có dữ liệu

            // Mở form Status và truyền dữ liệu
            frmStatus statusForm = new frmStatus(_character, _stats, _currentHealth);
            statusForm.ShowDialog();
        }

        // Nút EQUIPMENT
        private void btnEquipment_Click(object sender, EventArgs e)
        {
            if (_character == null) return; // Không có dữ liệu

            // Mở form Equipment và truyền dữ liệu
            frmEquipment equipmentForm = new frmEquipment(_character, _inventory);
            equipmentForm.ShowDialog();
        }

        // Nút SAVE/LOAD
        private void btnSaveLoad_Click(object sender, EventArgs e)
        {
            // Mở form Save/Load
            frmSaveLoadMenu saveLoadForm = new frmSaveLoadMenu();
            saveLoadForm.ShowDialog();
        }

        // Nút MAIN MENU
        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            // Hỏi người dùng có muốn thoát không
            var result = MessageBox.Show("Bạn có chắc muốn thoát về Menu chính?\nMọi tiến trình chưa lưu sẽ bị mất.", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                // (Thêm logic để thoát game ở đây,
                // ví dụ: đặt một cờ (flag) để form game chính đọc)
                // this.DialogResult = DialogResult.OK; // Ví dụ
                this.Close();
            }
        }

        // Thanh trượt Âm lượng
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
