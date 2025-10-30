using System;
using System.Drawing;
using System.Drawing.Text; // Thêm
using System.Runtime.InteropServices; // Thêm
using System.Windows.Forms;


namespace Main
{
    public partial class frmMenu : System.Windows.Forms.Form
    {
        

        // Biến cho custom font
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font pixelFont;

        // Sửa constructor để nhận Player
        public frmMenu()
        {
            InitializeComponent();
            





        }



        // Hàm helper để áp dụng style cho nút


        // --- XÓA HÀM XỬ LÝ ESCAPE TRONG frmMenu ---
        // private void FrmMenu_KeyDown(object sender, KeyEventArgs e) { ... }
        // ------------------------------------

        // Nút ITEM (ĐÃ SỬA)
        private void btnItem_Click(object sender, EventArgs e)
        {

        }

        // Nút STATUS
        private void btnStatus_Click(object sender, EventArgs e)
        {

        }

        // Nút EQUIPMENT
        private void btnEquipment_Click(object sender, EventArgs e)
        {

        }

        // Nút SAVE/LOAD
        private void btnSaveLoad_Click(object sender, EventArgs e)
        {

        }

        // Nút MAIN MENU
        private void btnMainMenu_Click(object sender, EventArgs e)
        {

        }

        // Thanh trượt Âm lượng
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {

        }

        // --- SỬA LỖI STACKOVERFLOW ---
        // Xóa bỏ việc gọi ResumeGame() ở đây để tránh lặp vô hạn
        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {

        } 
        // -----------------------------
    }
}

