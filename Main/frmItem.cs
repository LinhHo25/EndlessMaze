using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Main.Tri; // Thêm
using System.Collections.Generic; // Thêm

namespace Main
{
    public partial class frmItem : Form
    {
        private Player _player; // Thêm tham chiếu đến Player

        // Biến cho custom font
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font pixelFont;

        public frmItem(Player player) // Sửa constructor
        {
            InitializeComponent();
            _player = player; // Lưu tham chiếu

            // Tải font pixel
            LoadPixelFont();

            // Áp dụng style
            ApplyPixelStyle();
        }

        private void LoadPixelFont()
        {
            try
            {
                // (Giả sử bạn có file font pixel.ttf trong project và set "Copy to Output")
                // Nếu không, nó sẽ dùng font Arial mặc định
                string fontPath = "pixel.ttf";
                if (System.IO.File.Exists(fontPath))
                {
                    byte[] fontData = System.IO.File.ReadAllBytes(fontPath);
                    IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
                    System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                    uint dummy = 0;
                    fonts.AddMemoryFont(fontPtr, fontData.Length);
                    AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
                    pixelFont = new Font(fonts.Families[0], 12F);
                }
                else
                {
                    pixelFont = new Font("Arial", 10F, FontStyle.Bold); // Font dự phòng
                }
            }
            catch
            {
                pixelFont = new Font("Arial", 10F, FontStyle.Bold); // Font dự phòng
            }
        }

        private void ApplyPixelStyle()
        {
            // Màu sắc
            this.BackColor = Color.FromArgb(204, 179, 132); // Màu giấy da
            lblTitle.ForeColor = Color.FromArgb(50, 50, 50); // Màu chữ tối

            // Font
            lblTitle.Font = new Font(pixelFont.FontFamily, 16F, FontStyle.Bold);
            btnUseItem.Font = new Font(pixelFont.FontFamily, 12F, FontStyle.Bold);
            btnClose.Font = new Font(pixelFont.FontFamily, 12F, FontStyle.Bold);
            lvInventory.Font = new Font(pixelFont.FontFamily, 10F);

            // Nút
            btnUseItem.BackColor = Color.FromArgb(52, 143, 118); // Xanh lá
            btnClose.BackColor = Color.Gray;
        }

        // Load form: Hiển thị túi đồ
        private void frmItem_Load(object sender, EventArgs e)
        {
            PopulateInventory();
        }

        // Hàm tải dữ liệu túi đồ
        private void PopulateInventory()
        {
            lvInventory.Items.Clear();
            if (_player == null) return;

            foreach (KeyValuePair<ItemType, int> item in _player.Inventory)
            {
                ListViewItem lvi = new ListViewItem(GetItemName(item.Key));
                lvi.SubItems.Add(item.Value.ToString());
                lvi.Tag = item.Key; // Lưu ItemType vào Tag
                lvInventory.Items.Add(lvi);
            }
        }

        // Hàm chuyển Enum sang tên Tiếng Việt
        private string GetItemName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.HealthPotion: return "Thuốc hồi máu";
                case ItemType.AttackPotion: return "Thuốc tăng tấn công";
                case ItemType.DefensePotion: return "Thuốc tăng phòng ngự";
                case ItemType.PoisonResistPotion: return "Thuốc kháng độc";
                case ItemType.CoolingWater: return "Nước giải nhiệt";
                default: return itemType.ToString();
            }
        }

        // Nút ĐÓNG
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Nút SỬ DỤNG
        private void btnUseItem_Click(object sender, EventArgs e)
        {
            if (lvInventory.SelectedItems.Count > 0)
            {
                // Lấy ItemType từ Tag
                ItemType selectedItem = (ItemType)lvInventory.SelectedItems[0].Tag;

                // Gọi hàm UseItem của Player
                _player.UseItem(selectedItem);

                // Cập nhật lại danh sách (vì số lượng đã thay đổi)
                PopulateInventory();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một vật phẩm để sử dụng.", "Thông báo");
            }
        }
    }
}

