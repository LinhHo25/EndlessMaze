using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using System.Collections.Generic; // Thêm

namespace Main
{
    public partial class frmItem : System.Windows.Forms.Form
    {
        

        // Biến cho custom font
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font pixelFont;

        public frmItem() // Sửa constructor
        {
            InitializeComponent();
            
        }

        

        // Load form: Hiển thị túi đồ
        private void frmItem_Load(object sender, EventArgs e)
        {
            
        }

        // Hàm tải dữ liệu túi đồ
        

        

        // Nút ĐÓNG
        private void btnClose_Click(object sender, EventArgs e)
        {
            
        }

        // Nút SỬ DỤNG
        private void btnUseItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}

