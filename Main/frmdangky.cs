using System;
using System.Windows.Forms;

namespace Main
{
    public partial class frmDangKy : Form
    {
        private frmMain _mainForm;

        // Constructor mới nhận tham chiếu đến frmMain
        public frmDangKy(frmMain mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            this.Text = "Đăng Nhập / Đăng Ký Tài Khoản";
        }

        // Constructor cũ (giữ lại nếu cần)
        public frmDangKy()
        {
            InitializeComponent();
        }

        // Sự kiện click cho nút ĐĂNG NHẬP
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // ===================================================================
            // CHÚ THÍCH DATABASE: PHẦN CẦN THÊM LOGIC ĐĂNG NHẬP/XÁC THỰC
            // ===================================================================

            // // 1. Kết nối DB và kiểm tra tài khoản/mật khẩu
            // bool isValid = CheckCredentialsInDatabase(username, password);

            bool isValid = true; // Giả định đăng nhập thành công để demo luồng

            if (isValid)
            {
                MessageBox.Show($"Đăng nhập thành công với tài khoản: {username}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();

                // Chuyển sang frmPlay, truyền tham chiếu frmMain để có thể quay lại
                frmPlay playForm = new frmPlay(_mainForm);
                playForm.Show();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Lỗi Đăng Nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện click cho nút ĐĂNG KÝ (ĐÃ THÊM LOGIC LƯU DỮ LIỆU GIẢ ĐỊNH)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Lỗi Đăng Ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ===================================================================
            // CHÚ THÍCH DATABASE: LOGIC LƯU DỮ LIỆU ĐĂNG KÝ
            // ===================================================================

            try
            {
                // // 1. Kết nối đến DB
                // var dbConnection = GetDatabaseConnection(); 

                // // 2. Kiểm tra tên đăng nhập đã tồn tại chưa
                // if (IsUsernameExists(dbConnection, username))
                // {
                //     MessageBox.Show("Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.", "Lỗi Đăng Ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //     return;
                // }

                // // 3. Mã hóa mật khẩu (NÊN DÙNG HASHING TRONG THỰC TẾ)
                // string hashedPassword = HashPassword(password);

                // // 4. Lưu tài khoản mới vào DB
                // SaveNewUserToDatabase(dbConnection, username, hashedPassword);

                MessageBox.Show($"Đăng ký tài khoản '{username}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Sau khi đăng ký thành công, có thể tự động đăng nhập
                // (Giả định rằng việc lưu dữ liệu thành công)
                this.btnLogin_Click(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi lưu vào DB
                MessageBox.Show($"Đăng ký thất bại do lỗi hệ thống (DB): {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện đóng Form, quay lại frmMain
        private void frmDangKy_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Nếu người dùng đóng form bằng nút X, quay lại Menu chính
            if (_mainForm != null && _mainForm.Visible == false)
            {
                _mainForm.Show();
            }
        }

        // Sự kiện Load Form
        private void frmdangky_Load(object sender, EventArgs e)
        {
            // Thêm code khởi tạo (nếu có) khi form được tải
        }
    }
}
