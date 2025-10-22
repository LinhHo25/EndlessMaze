using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class frmLeaderboard : Form
    {
        public frmLeaderboard()
        {
            InitializeComponent();
        }

        private void frmLeaderboard_Load(object sender, EventArgs e)
        {
            // Thiết lập màu sắc và kiểu dáng cho cả hai DataGridView
            SetupDataGridViewStyles();

            // Tải dữ liệu mẫu cho cả hai tab
            LoadKhamPhaData();
            LoadPhieuLuuData();

            // Đặt tab đầu tiên là tab được chọn
            btnTabKhamPha_Click(null, null);
        }

        /// <summary>
        /// Tùy chỉnh giao diện cho DataGridView để có nền tối
        /// </summary>
        private void SetupDataGridViewStyles()
        {
            // Màu nền chung của form
            Color formBackColor = Color.FromArgb(30, 30, 40);
            this.BackColor = formBackColor;

            // Tùy chỉnh TabControl
            tabMain.Appearance = TabAppearance.FlatButtons;
            tabMain.ItemSize = new Size(0, 1);
            tabMain.SizeMode = TabSizeMode.Fixed;
            tabMain.BackColor = formBackColor;

            // Áp dụng kiểu dáng cho cả hai bảng
            ApplyModernGridStyles(dgvKhamPha);
            ApplyModernGridStyles(dgvPhieuLuu);
        }

        /// <summary>
        /// Hàm trợ giúp để áp dụng kiểu dáng cho một DataGridView
        /// </summary>
        private void ApplyModernGridStyles(DataGridView dgv)
        {
            Color formBackColor = Color.FromArgb(30, 30, 40);
            Color cellBackColor = Color.FromArgb(45, 45, 55);
            Color foreColor = Color.White;
            Color selectionColor = Color.FromArgb(0, 122, 204);

            dgv.BackgroundColor = cellBackColor;
            dgv.GridColor = formBackColor;
            dgv.BorderStyle = BorderStyle.None;

            // Tắt các kiểu giao diện mặc định của Windows
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Kiểu dáng Header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = formBackColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Cyan; // Màu chữ tiêu đề
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Kiểu dáng các dòng
            dgv.DefaultCellStyle.BackColor = cellBackColor;
            dgv.DefaultCellStyle.ForeColor = foreColor;
            dgv.DefaultCellStyle.SelectionBackColor = selectionColor;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = cellBackColor; // Giữ nguyên màu nền
        }


        /// <summary>
        /// Tải dữ liệu (giả lập) cho tab Khám Phá
        /// </summary>
        private void LoadKhamPhaData()
        {
            dgvKhamPha.Rows.Clear();

            // ===================================================================
            // CHÚ THÍCH DATABASE: TẢI DỮ LIỆU CHO BẢNG XẾP HẠNG KHÁM PHÁ
            // ===================================================================

            // Dữ liệu giả lập (mock data) - ĐÃ CẬP NHẬT TÊN MAP
            dgvKhamPha.Rows.Add("player_khampha_1", "00:15:30", "150", "Diệm Ngục", "10");
            dgvKhamPha.Rows.Add("player_khampha_2", "00:18:22", "120", "Hàn Thủy Vực", "8");
            dgvKhamPha.Rows.Add("player_khampha_3", "00:20:05", "95", "Rừng Chướng Khí", "7");

            // Giả sử highlight người chơi hiện tại (nếu cần)
            // DataGridViewRow playerRow = dgvKhamPha.Rows[1]; 
            // playerRow.DefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            // playerRow.DefaultCellStyle.ForeColor = Color.White;
        }

        /// <summary>
        /// Tải dữ liệu (giả lập) cho tab Phiêu Lưu
        /// </summary>
        private void LoadPhieuLuuData()
        {
            dgvPhieuLuu.Rows.Clear();

            // ===================================================================
            // CHÚ THÍCH DATABASE: TẢI DỮ LIỆU CHO BẢNG XẾP HẠNG PHIÊU LƯU
            // ===================================================================

            // Dữ liệu giả lập (mock data) - ĐÃ CẬP NHẬT TÊN MAP
            dgvPhieuLuu.Rows.Add("player_phieuluu_A", "00:10:00", "5000", "Diệm Ngục", "15");
            dgvPhieuLuu.Rows.Add("player_phieuluu_B", "00:12:30", "4500", "Hàn Thủy Vực", "12");
            dgvPhieuLuu.Rows.Add("player_phieuluu_C", "00:13:10", "4200", "Rừng Chướng Khí", "11");
        }


        // Các sự kiện click cho các nút tab (để chuyển tab)
        private void btnTabKhamPha_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabKhamPha;
            HighlightButton(btnTabKhamPha);
        }

        private void btnTabPhieuLuu_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabPhieuLuu;
            HighlightButton(btnTabPhieuLuu);
        }

        // Hàm helper để đổi màu nút được chọn
        private void HighlightButton(Button selectedButton)
        {
            Color activeColor = Color.Cyan;
            Color inactiveColor = Color.Gray;

            btnTabKhamPha.ForeColor = inactiveColor;
            btnTabPhieuLuu.ForeColor = inactiveColor;

            selectedButton.ForeColor = activeColor;
        }

        // Nút đóng form
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

