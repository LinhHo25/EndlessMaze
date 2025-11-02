using BLL.Services; // <-- THÊM
using System.IO;
using DAL.Models; // <-- THÊM
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using static BLL.Services.GameSessionService; // <-- THÊM

namespace Main // <-- SỬA: Đổi namespace
{
    // Form này sẽ hiển thị chỉ số nhân vật
    public partial class frmStatus : Form
    {
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private Font pixelFontSmall;

        // --- SỬA: Constructor để nhận dữ liệu thật ---
        public frmStatus(PlayerCharacters character, CalculatedStats stats, int currentHealth)
        {
            InitializeComponent();
            this.KeyPreview = true;

            // --- Tải font pixel ---
            try
            {
                pixelFontSmall = new Font("Consolas", 10F, FontStyle.Bold);
            }
            catch
            {
                pixelFontSmall = new Font("Arial", 10F, FontStyle.Bold);
            }
            // -----------------------

            // Áp dụng style
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 222, 179); // Màu giấy da

            lblTitle.Font = new Font(pixelFontSmall.FontFamily, 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 40);

            // Style cho các label chỉ số
            ApplyLabelStyles(lblHP);
            ApplyLabelStyles(lblMP);
            ApplyLabelStyles(lblStrength);
            ApplyLabelStyles(lblDefense);
            ApplyLabelStyles(lblSpeed);

            // Style cho nút đóng
            ApplyCloseButtonStyles(btnClose);

            // --- THÊM: Tải ảnh Avatar ---
            try
            {
                string avatarPath = Path.Combine("ImgSource", "Avt", "AVT3.jpg");
                if (File.Exists(avatarPath))
                {
                    picPlayerAvatar.Image = Image.FromFile(avatarPath);
                    picPlayerAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tải avatar: " + ex.Message);
            }

            // --- Tải chỉ số thực tế của Player vào các Label ---
            if (character != null && stats != null)
            {
                lblHP.Text = $"HP: {currentHealth} / {stats.TotalHealth}";
                lblMP.Text = $"Thể Lực (Stamina): ??? / {character.BaseStamina}"; // Giả sử chưa có Thể Lực hiện tại
                lblStrength.Text = $"Sức mạnh: {stats.TotalAttack}";
                lblDefense.Text = $"Phòng thủ: {stats.TotalDefense}";
                lblSpeed.Text = "Tốc độ: (chưa định nghĩa)"; // CalculatedStats chưa có Tốc độ
            }
            else
            {
                lblTitle.Text = "LỖI TẢI DỮ LIỆU";
            }
        }

        // --- THÊM: Constructor mặc định (cho Designer) ---
        public frmStatus()
        {
            InitializeComponent();
            this.KeyPreview = true;
            ApplyStylesAndLoadFakeData();
        }

        // Hàm style cho constructor mặc định
        private void ApplyStylesAndLoadFakeData()
        {
            try { pixelFontSmall = new Font("Consolas", 10F, FontStyle.Bold); }
            catch { pixelFontSmall = new Font("Arial", 10F, FontStyle.Bold); }

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 222, 179);
            lblTitle.Font = new Font(pixelFontSmall.FontFamily, 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 40, 40);
            ApplyLabelStyles(lblHP);
            ApplyLabelStyles(lblMP);
            ApplyLabelStyles(lblStrength);
            ApplyLabelStyles(lblDefense);
            ApplyLabelStyles(lblSpeed);
            ApplyCloseButtonStyles(btnClose);

            lblHP.Text = "HP: 100 / 100";
            lblMP.Text = "MP: 50 / 50";
            lblStrength.Text = "Sức mạnh: 10";
            lblDefense.Text = "Phòng thủ: 5";
            lblSpeed.Text = "Tốc độ: 8";
        }


        private void ApplyLabelStyles(Label lbl)
        {
            lbl.Font = pixelFontSmall;
            lbl.ForeColor = Color.FromArgb(40, 40, 40);
        }

        private void ApplyCloseButtonStyles(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(192, 57, 43); // Màu đỏ
            btn.ForeColor = Color.White;
            btn.Font = new Font(pixelFontSmall.FontFamily, 10F, FontStyle.Bold);
            btn.Text = "X";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
