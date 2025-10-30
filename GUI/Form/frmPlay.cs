using System;
using System.Windows.Forms;

using System.Linq;
using System.Collections.Generic; // <-- THÊM DÒNG NÀY

namespace Main
{
    public partial class frmPlay : Form
    {
        private frmMain _mainForm;
        private Random rand = new Random();

        public frmPlay(frmMain mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            this.Text = "Chọn Chế Độ Chơi";
        }

        public frmPlay()
        {
            InitializeComponent();
        }

        // Nút CHƠI MỚI
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            
        }

        // Nút CHƠI TIẾP (ĐÃ CẬP NHẬT LOGIC)
        private void btnContinue_Click(object sender, EventArgs e)
        {
            
        }

        // Nút TẢI LƯỢT CHƠI (mở form frmLoadPlay cũ)
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            
        }

        // Hàm chung để khởi chạy map
        

        // Nút QUAY LẠI MENU
        private void btnBack_Click(object sender, EventArgs e)
        {
           
        }

        // Sự kiện khi form bị đóng, hiển thị lại frmMain
        private void frmPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        // Sự kiện khi form này được hiện lại (sau khi map đóng)
        private void frmPlay_VisibleChanged(object sender, EventArgs e)
        {
            
        }


        // =============================================================
        // HÀM GIẢ LẬP TRUY VẤN DATABASE (Cần thay thế bằng code thật)
        // =============================================================
       
        

        private void frmPlay_Load(object sender, EventArgs e)
        {

        }
        // =============================================================

    }
}

