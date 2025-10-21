using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;using Main.Tri; // Thêm using để sử dụng các lớp logic
namespace Main
{
    // Đổi tên lớp từ Form1 thành frmMain để dễ quản lý
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            // Cấu hình ban đầu cho Form (ví dụ: kích thước, tiêu đề...)
            this.KeyPreview = true;

            // Gợi ý: Để thêm hình nền, bạn có thể bỏ comment dòng dưới đây
            // và thay thế "your_image.jpg" bằng tên tệp hình ảnh của bạn.
            // Đừng quên thêm hình ảnh vào project và thiết lập "Copy to Output Directory".
            // try
            // {
            //     this.BackgroundImage = Image.FromFile("your_image.jpg");
            //     this.BackgroundImageLayout = ImageLayout.Stretch;
            // }
            // catch (Exception ex)
            // {
            //     MessageBox.Show("Không thể tải hình nền: " + ex.Message);
            // }
        }

        // Sự kiện click cho nút "Chế độ Hành động" (Giả định là nút ĐĂNG NHẬP)
        private void btnActionMode_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Khởi tạo và hiển thị Form Đăng ký/Đăng nhập mới
            frmDangKy dangKyForm = new frmDangKy(this);
            dangKyForm.Show();
        }

        // Sự kiện click cho nút "Chế độ Khám phá" (Nếu có)
        private void btnExploreMode_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Khởi tạo và hiển thị Form Khám phá
            //frmExploreMode exploreForm = new frmExploreMode(this);
            //exploreForm.Show();
        }

        // Sự kiện click cho nút "Bảng Xếp Hạng"
        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            // Khởi tạo và hiển thị Form Bảng Xếp Hạng dưới dạng hộp thoại
            frmLeaderboard leaderboardForm = new frmLeaderboard();
            leaderboardForm.ShowDialog();
        }

        // Sự kiện click cho nút "Thoát"
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Đóng ứng dụng
            Application.Exit();
        }
    }
}
