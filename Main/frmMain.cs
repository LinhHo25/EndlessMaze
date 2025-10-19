using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Main.Tri;
namespace Main
{
    // Đổi tên lớp từ Form1 thành frmMain để dễ quản lý
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

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

        // Sự kiện click cho nút "Chế độ Hành động"
        private void btnActionMode_Click(object sender, EventArgs e)
        {
            // Tạm thời hiển thị một thông báo.
            // Ở đây bạn sẽ viết code để bắt đầu game ở chế độ hành động.
            MessageBox.Show("Bắt đầu trò chơi ở Chế độ Hành động!", "Bắt đầu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Sự kiện click cho nút "Chế độ Khám phá"
        private void btnExploreMode_Click(object sender, EventArgs e)
        {
            // Tạm thời hiển thị một thông báo.
            // Ở đây bạn sẽ viết code để bắt đầu game ở chế độ khám phá.
            MessageBox.Show("Bắt đầu trò chơi ở Chế độ Khám phá!", "Bắt đầu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Sự kiện click cho nút "Bảng Xếp Hạng"
        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            // Tạm thời hiển thị một thông báo.
            // Ở đây bạn sẽ viết code để mở một form hoặc panel Bảng Xếp Hạng.
            MessageBox.Show("Hiển thị Bảng Xếp Hạng!", "Bảng Xếp Hạng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Sự kiện click cho nút "Thoát"
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Đóng ứng dụng
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
