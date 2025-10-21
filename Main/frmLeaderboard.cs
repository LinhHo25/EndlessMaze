using System.Drawing;
using System.Windows.Forms;

namespace Main
{
    public partial class frmLeaderboard : Form
    {
        public frmLeaderboard()
        {
            InitializeComponent();
            this.Text = "Bảng Xếp Hạng";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 40, 40); // Nền tối
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        /// <summary>
        /// Gợi ý: Hàm này sẽ được gọi để tải dữ liệu từ SQL Server 2022.
        /// </summary>
        private void LoadLeaderboardData()
        {
            // TODO: Giai đoạn 3: Thực hiện kết nối SQL Server tại đây
            // và điền dữ liệu vào DataGridView hoặc ListBox.

            // Tạm thời hiển thị dữ liệu giả định
            string dummyData = "Hạng | Điểm | Tên\n";
            dummyData += "----------------------\n";
            dummyData += "1.   | 15000 | Anh Lính\n";
            dummyData += "2.   | 12500 | Chị Tiên\n";
            dummyData += "3.   | 10000 | Mr. Game";

            Label lblData = new Label
            {
                Text = dummyData,
                Font = new Font("Consolas", 14),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 70)
            };
            this.Controls.Add(lblData);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            // Tiêu đề
            Label lblTitle = new Label
            {
                Text = "BẢNG XẾP HẠNG (TOP 3)",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            // Tải dữ liệu
            LoadLeaderboardData();

            // Thiết lập kích thước Form phù hợp với nội dung tạm thời
            this.ClientSize = new Size(400, 300);
        }

        private void frmLeaderboard_Load(object sender, System.EventArgs e)
        {

        }
    }
}
