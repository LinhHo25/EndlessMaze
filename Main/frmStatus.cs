using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Main
{
    // Form này sẽ hiển thị chỉ số nhân vật
    public partial class frmStatus : Form
    {
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private Font pixelFontSmall;

        public frmStatus()
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

            // TODO: Tải chỉ số thực tế của Player vào các Label
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
