﻿using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using DAL.Models; // <-- THÊM
using BLL.Services; // <-- THÊM

namespace Main
{
    public partial class frmItem : System.Windows.Forms.Form
    {
        // --- THÊM: Dữ liệu game ---
        private readonly PlayerSessionInventory _inventory;
        private readonly GameSessionService _gameSessionService;
        private readonly int _characterId;
        private int _currentHealth;

        // Property để frmMenu đọc máu mới
        public int NewHealth { get; private set; }
        // -------------------------

        // Biến cho custom font
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font pixelFont;

        // Constructor mặc định (cho Designer)
        public frmItem()
        {
            InitializeComponent();
            ApplyStyles();
        }

        // --- THÊM: Constructor chính ---
        public frmItem(PlayerSessionInventory inventory, GameSessionService gameSessionService, int characterId, int currentHealth)
        {
            InitializeComponent();
            ApplyStyles();

            // Lưu dữ liệu
            _inventory = inventory;
            _gameSessionService = gameSessionService;
            _characterId = characterId;
            _currentHealth = currentHealth;
            NewHealth = currentHealth; // Khởi tạo
        }

        // Hàm áp dụng style
        private void ApplyStyles()
        {
            // (Code style của bạn ở đây)
            this.BackColor = Color.FromArgb(204, 179, 132);
            lblTitle.Font = new Font("Arial", 16F, FontStyle.Bold);
        }

        // Load form: Hiển thị túi đồ
        private void frmItem_Load(object sender, EventArgs e)
        {
            LoadInventoryData();
        }

        // Hàm tải dữ liệu túi đồ
        private void LoadInventoryData()
        {
            lvInventory.Items.Clear();

            // Kiểm tra nếu có inventory
            if (_inventory != null)
            {
                // Thêm bình máu
                // (Giả sử PotionID 1 là bình máu)
                ListViewItem potionItem = new ListViewItem("Bình Máu (Hồi 40 HP)");
                potionItem.SubItems.Add(_inventory.HealthPotionCount.ToString());
                potionItem.Tag = 1; // Tag là PotionID
                lvInventory.Items.Add(potionItem);

                // (Bạn có thể thêm các vật phẩm khác ở đây)
            }

            // Tự động chọn item đầu tiên nếu có
            if (lvInventory.Items.Count > 0)
            {
                lvInventory.Items[0].Selected = true;
            }
        }

        // Nút ĐÓNG
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form
        }

        // Nút SỬ DỤNG
        private void btnUseItem_Click(object sender, EventArgs e)
        {
            if (lvInventory.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một vật phẩm.", "Thông báo");
                return;
            }

            if (_inventory == null || _gameSessionService == null)
            {
                MessageBox.Show("Lỗi: Không có dữ liệu túi đồ.", "Lỗi");
                return;
            }

            // Lấy PotionID từ Tag
            int potionId = (int)lvInventory.SelectedItems[0].Tag;

            if (potionId == 1) // Nếu là bình máu
            {
                if (_inventory.HealthPotionCount <= 0)
                {
                    MessageBox.Show("Bạn đã hết bình máu.", "Không thể dùng");
                    return;
                }

                // --- SỬ DỤNG BLL ---
                // (Lưu ý: Hàm UseHealthPotion trong BLL cần được kiểm tra
                // xem nó có tự động SaveChanges() hay không)
                int newHealth = _gameSessionService.UseHealthPotion(_characterId, _currentHealth);

                if (newHealth == _currentHealth)
                {
                    // Không dùng được (ví dụ: máu đã đầy)
                    MessageBox.Show("Máu đã đầy.", "Không thể dùng");
                }
                else
                {
                    // Dùng thành công
                    _currentHealth = newHealth; // Cập nhật máu hiện tại (cho lần dùng tiếp)
                    NewHealth = newHealth;      // Cập nhật máu mới (cho frmMenu đọc)

                    // Cập nhật số lượng trong DB (Hàm BLL đã làm)
                    // Cập nhật số lượng local (để refresh UI)
                    _inventory.HealthPotionCount--;

                    // Tải lại UI
                    LoadInventoryData();
                }
            }
        }
    }
}
