using BLL.Services; // <-- Sử dụng BLL
using DAL.Models; // <-- Sử dụng Models
using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace Main // <-- SỬA: Đổi namespace về 'Main' cho nhất quán
{
    // Form này sẽ hiển thị trang bị của nhân vật
    public partial class frmEquipment : Form
    {
        private PrivateFontCollection pfc = new PrivateFontCollection();
        private Font pixelFontSmall;

        // --- Dữ liệu nhân vật và BLL Service ---
        private readonly PlayerCharacters _character;
        private readonly PlayerSessionInventory _inventory;
        private readonly GameDefinitionService _definitionService; // Dùng để lấy tên item từ ID

        // --- SỬA: Constructor để nhận dữ liệu game ---
        public frmEquipment(PlayerCharacters character, PlayerSessionInventory inventory)
        {
            InitializeComponent();
            this.KeyPreview = true;

            // --- Lưu trữ dữ liệu và BLL ---
            _character = character;
            _inventory = inventory;
            _definitionService = new GameDefinitionService(); // Khởi tạo BLL
            // ------------------------------------


            // --- Tải font pixel ---
            try
            {
                // (Code tải font custom của bạn ở đây nếu có)
                // Tạm thời dùng font dự phòng:
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

            // Style cho các label (BỎ QUA HELMET VÀ ACCESSORY)
            ApplyLabelStyles(lblWeapon);
            ApplyLabelStyles(lblArmor);

            // Style cho các ô trang bị (PictureBox)
            ApplySlotStyles(picWeapon);
            ApplySlotStyles(picArmor);

            // Style cho nút đóng
            ApplyCloseButtonStyles(btnClose);

            // --- Tải trang bị thực tế vào các Label ---
            LoadEquipmentData();
            // ---------------------------------------------
        }
        // --- THÊM: HÀM TẢI ẢNH (Tương tự GroundItem.cs) ---
        private Image LoadEquipmentImage(object itemData)
        {
            string path = "";
            try
            {
                if (itemData is Weapons w)
                {
                    string folder = Path.Combine("ImgSource", "Item", "Weapon", w.WeaponRank);
                    if (Directory.Exists(folder))
                    {
                        path = Directory.GetFiles(folder, "*.png").FirstOrDefault();
                    }
                }
                else if (itemData is Armors a)
                {
                    string fileName = "";
                    if (a.ArmorRank == "A") fileName = "vwsmmfyu747f1_0.png";
                    else if (a.ArmorRank == "B") fileName = "vwsmmfyu747f1_1.png";
                    else if (a.ArmorRank == "C") fileName = "vwsmmfyu747f1_2.png";
                    else fileName = "vwsmmfyu747f1_3.png"; // Rank D

                    path = Path.Combine("ImgSource", "Armors", fileName);
                }

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    byte[] imageData = File.ReadAllBytes(path);
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể tải ảnh Equipment: " + ex.Message);
            }
            return null; // Trả về null nếu lỗi
        }
        // --- HÀM MỚI: Tải dữ liệu trang bị ---
        private void LoadEquipmentData()
        {
            try
            {
                // Kiểm tra null
                if (_character == null || _inventory == null)
                {
                    lblWeapon.Text = "Vũ khí: (Lỗi)";
                    lblArmor.Text = "Áo giáp: (Lỗi)";
                    return;
                }

                // 1. Tải Vũ khí (Weapon)
                var weapon = _definitionService.GetWeaponById(_inventory.EquippedWeaponID);
                if (weapon != null)
                {
                    lblWeapon.Text = $"Vũ khí: {weapon.WeaponName} (Rank {weapon.WeaponRank})";
                    // TODO: Gán hình ảnh cho picWeapon (nếu có)
                    picWeapon.Image = LoadEquipmentImage(weapon);
                }
                else
                {
                    lblWeapon.Text = "Vũ khí: (Trống)";
                }

                // 2. Tải Áo giáp (Armor)
                var armor = _definitionService.GetArmorById(_inventory.EquippedArmorID);
                if (armor != null)
                {
                    lblArmor.Text = $"Áo giáp: {armor.ArmorName} (Rank {armor.ArmorRank})";
                    // TODO: Gán hình ảnh cho picArmor (nếu có)
                    picArmor.Image = LoadEquipmentImage(armor);
                }
                else
                {
                    lblArmor.Text = "Áo giáp: (Trống)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải trang bị: " + ex.Message);
            }
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
            pic.SizeMode = PictureBoxSizeMode.Zoom;
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

        // --- Constructor mặc định (để Designer không lỗi) ---
        public frmEquipment()
        {
            InitializeComponent();
            this.KeyPreview = true;

            // Tải dữ liệu giả
            lblWeapon.Text = "Vũ khí: (Trống)";
            lblArmor.Text = "Áo giáp: (Trống)";
        }
        // -----------------------------------------------------------------
    }
}
