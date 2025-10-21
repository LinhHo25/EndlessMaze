using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    public partial class frmPlay : Form
    {
        private frmMain _mainForm;

        // Constructor mới nhận tham chiếu đến frmMain
        public frmPlay(frmMain mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            this.Text = "Chọn Chế Độ Chơi";
        }

        // Constructor cũ (giữ lại nếu cần)
        public frmPlay()
        {
            InitializeComponent();
        }

        // Sự kiện click cho nút "Tải Lượt Chơi Đã Lưu"
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Khởi tạo và hiển thị frmLoadPlay
            // Truyền tham chiếu frmMain để có thể quay lại menu chính
            frmLoadPlay loadPlayForm = new frmLoadPlay(_mainForm);
            loadPlayForm.Show();
        }

        // Sự kiện click cho nút "Chơi Mới"
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Khởi tạo Game Mới...", "Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // TẠI ĐÂY BẠN CÓ THỂ CHUYỂN ĐẾN FORM GAME CHÍNH (VÍ DỤ: frmGame)
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
