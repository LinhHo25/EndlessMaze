using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Services; // <-- THÊM: Sử dụng BLL
using DAL.Models; // <-- THÊM: Sử dụng Models

namespace Main
{
    public partial class frmLeaderboard : System.Windows.Forms.Form
    {
        // --- THÊM: Khởi tạo Service ---
        private readonly LeaderboardService _leaderboardService;

        public frmLeaderboard()
        {
            InitializeComponent();
            // --- THÊM: Khởi tạo BLL Service ---
            _leaderboardService = new LeaderboardService();
        }

        private void frmLeaderboard_Load(object sender, EventArgs e)
        {
            // --- THÊM: Tải dữ liệu khi form mở ---
            SetupGridStyles();
            LoadExplorationData(); // Tải tab Khám Phá
            LoadAdventureData();   // Tải tab Phiêu Lưu
        }

        /// <summary>
        /// Hàm helper để định dạng GridView cho đẹp
        /// </summary>
        private void SetupGridStyles()
        {
            // Style chung
            var grids = new[] { dgvKhamPha, dgvPhieuLuu };
            foreach (var dgv in grids)
            {
                dgv.AllowUserToAddRows = false;
                dgv.ReadOnly = true;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.RowHeadersVisible = false;

                // Style header
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.EnableHeadersVisualStyles = false;

                // Style cell
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
                dgv.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
                dgv.DefaultCellStyle.ForeColor = Color.Gainsboro;
                dgv.DefaultCellStyle.SelectionBackColor = Color.DodgerBlue;
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.BackgroundColor = Color.FromArgb(30, 30, 30);
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 60);
            }
        }


        /// <summary>
        /// Tải dữ liệu cho Bảng Xếp Hạng Khám Phá
        /// </summary>
        private void LoadExplorationData()
        {
            try
            {
                dgvKhamPha.Rows.Clear(); // Xóa dữ liệu cũ
                var scores = _leaderboardService.GetExplorationLeaderboard();

                foreach (var score in scores)
                {
                    // Định dạng lại thời gian
                    string formattedTime = FormatTime(score.CompletionTimeSeconds);
                    // Thêm vào các cột đã định nghĩa trong Designer
                    dgvKhamPha.Rows.Add(score.Username, formattedTime, score.ItemsCollected, score.MapsCompleted);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải BXH Khám Phá: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tải dữ liệu cho Bảng Xếp Hạng Phiêu Lưu
        /// </summary>
        private void LoadAdventureData()
        {
            try
            {
                dgvPhieuLuu.Rows.Clear(); // Xóa dữ liệu cũ
                var scores = _leaderboardService.GetAdventureLeaderboard();

                foreach (var score in scores)
                {
                    // Định dạng lại thời gian
                    string formattedTime = FormatTime(score.CompletionTimeSeconds);
                    // Thêm vào các cột đã định nghĩa trong Designer
                    dgvPhieuLuu.Rows.Add(score.Username, formattedTime, score.MonstersKilled, score.MapsCompleted);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải BXH Phiêu Lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hàm helper chuyển đổi giây sang định dạng "mm:ss"
        /// </summary>
        private string FormatTime(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            // Sử dụng định dạng mm\:ss để luôn hiển thị 2 chữ số cho phút và giây
            return time.ToString(@"mm\:ss");
        }


        // Các sự kiện click cho các nút tab (để chuyển tab)
        // (Nếu bạn dùng TabControl, các nút này có thể không cần thiết)
        private void btnTabKhamPha_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabKhamPha;
        }

        private void btnTabPhieuLuu_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabPhieuLuu;
        }

        // Nút đóng form
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
