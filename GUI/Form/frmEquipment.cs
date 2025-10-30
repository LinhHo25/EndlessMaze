using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Main
{
    // Form này sẽ hiển thị trang bị của nhân vật
    public partial class frmEquipment : Form
    {
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private Font pixelFontSmall;

        public frmEquipment()
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

            // Style cho các label
            ApplyLabelStyles(lblWeapon);
            ApplyLabelStyles(lblArmor);
            ApplyLabelStyles(lblHelmet);
            ApplyLabelStyles(lblAccessory);

            // Style cho các ô trang bị (PictureBox)
            ApplySlotStyles(picWeapon);
            ApplySlotStyles(picArmor);
            ApplySlotStyles(picHelmet);
            ApplySlotStyles(picAccessory);

            // Style cho nút đóng
            ApplyCloseButtonStyles(btnClose);

            // TODO: Tải trang bị thực tế vào các PictureBox và cập nhật Label
            lblWeapon.Text = "Vũ khí: (Trống)";
            lblArmor.Text = "Áo giáp: (Trống)";
            lblHelmet.Text = "Mũ: (Trống)";
            lblAccessory.Text = "Phụ kiện: (Trống)";
        }

        private void ApplyLabelStyles(Label lbl)
        {
            lbl.Font = pixelFontSmall;
            lbl.ForeColor = Color.FromArgb(40, 40, 40);
        }

        private void ApplySlotStyles(PictureBox pic)
        {
            pic.BackColor = Color.FromArgb(220, 198, 158); // Màu giấy da tối hơn
            pic.BorderStyle = BorderStyle.FixedSingle;
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
