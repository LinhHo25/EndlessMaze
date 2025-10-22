using System;
using System.Windows.Forms;
using Main.Map; // <- THÊM using này để truy cập các Map
using Main.Tri; // <- THÊM using này để truy cập GameLogic

namespace Main
{
    public partial class frmPlay : Form
    {
        private frmMain _mainForm;
        private Random rand = new Random(); // Biến random

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

        // SỰ KIỆN NÚT "CHƠI MỚI" (ĐÃ CẬP NHẬT)
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            // Random một số từ 0 đến 2
            int mapIndex = rand.Next(0, 3); // 0, 1, hoặc 2
            Form mapToPlay;

            switch (mapIndex)
            {
                case 0:
                    mapToPlay = new Water();
                    break;
                case 1:
                    mapToPlay = new Flame();
                    break;
                case 2:
                default:
                    mapToPlay = new Poison();
                    break;
            }

            // Bắt đầu map đã random
            StartMap(mapToPlay);
        }

        // SỰ KIỆN NÚT "CHƠI TIẾP" (MỚI)
        private void btnContinue_Click(object sender, EventArgs e)
        {
            // Tạm thời: Logic "Chơi tiếp" sẽ load lượt chơi gần nhất
            // Bạn có thể thay đổi logic này
            MessageBox.Show("Tính năng 'Chơi tiếp' đang được phát triển. Vui lòng 'Tải lượt chơi' thủ công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Hoặc, bạn có thể gọi thẳng BtnLoad_Click
            // BtnLoad_Click(sender, e);
        }

        // Sự kiện click cho nút "Tải Lượt Chơi Đã Lưu"
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmLoadPlay loadPlayForm = new frmLoadPlay(_mainForm);
            loadPlayForm.Show();
        }

        // Hàm chung để khởi chạy map
        private void StartMap(Form mapForm)
        {
            this.Hide();
            mapForm.ShowDialog();
            this.Close(); // Tự đóng form này
        }

        // Sự kiện click cho nút "Quay Lại Menu"
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form hiện tại
        }

        // Sự kiện khi form bị đóng, hiển thị lại frmMain (menu chính)
        private void frmPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_mainForm != null && _mainForm.Visible == false)
            {
                _mainForm.Show();
            }
        }
    }
}

