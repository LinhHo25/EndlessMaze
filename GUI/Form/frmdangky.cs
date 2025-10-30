using System;
using System.Collections.Generic; // Thêm vào để dùng Dictionary mô phỏng DB
using System.Windows.Forms;

namespace Main
{
    public partial class frmDangKy : Form
    {
        private frmMain _mainForm;

        // ===================================================================
        // GIẢ LẬP DATABASE (TẠM THỜI)
        // Chúng ta dùng một Dictionary tĩnh để lưu tài khoản (username, password)
        // Khi bạn kết nối DB thật, bạn sẽ thay thế các hàm bên dưới.
        private static Dictionary<string, string> simulatedUserDatabase = new Dictionary<string, string>();
        // ===================================================================


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

        // Sự kiện click cho nút ĐĂNG NHẬP (ĐÃ SỬA)
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // --- BƯỚC 1: Kiểm tra thông tin nhập ---
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- BƯỚC 2: Kiểm tra trong Database ---
            bool isValid = CheckCredentialsInDatabase(username, password);

            if (isValid)
            {
                MessageBox.Show($"Đăng nhập thành công với tài khoản: {username}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Lưu thông tin người dùng hiện tại (SỬA LỖI CS0117)
                CurrentUser.Username = username; // Sử dụng Username

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

        // Sự kiện click cho nút ĐĂNG KÝ (ĐÃ SỬA)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Lỗi Đăng Ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- BƯỚC 1: Kiểm tra xem tên đăng nhập đã tồn tại chưa ---
            if (IsUsernameExists(username))
            {
                MessageBox.Show("Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.", "Lỗi Đăng Ký", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: Lưu tài khoản mới vào Database ---
            try
            {
                SaveNewUserToDatabase(username, password);

                MessageBox.Show($"Đăng ký tài khoản '{username}' thành công! \nVui lòng đăng nhập lại.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            // Nếu người dùng đóng form bằng nút X VÀ CHƯA ĐĂNG NHẬP, quay lại Menu chính
            if (_mainForm != null && _mainForm.Visible == false && !CurrentUser.IsLoggedIn) // SỬA LỖI CS0117
            {
                _mainForm.Show();
            }
        }

        // Sự kiện Load Form
        private void frmdangky_Load(object sender, EventArgs e)
        {
            // Thêm code khởi tạo (nếu có) khi form được tải
        }


        #region Database Placeholder Functions (Hàm giả lập DB)

        /// <summary>
        /// (Hàm giả lập) Kiểm tra tên đăng nhập và mật khẩu
        /// </summary>
        private bool CheckCredentialsInDatabase(string username, string password)
        {
            // === CODE DATABASE THẬT ===
            // Logic giả lập:
            if (simulatedUserDatabase.ContainsKey(username))
            {
                return simulatedUserDatabase[username] == password;
            }
            return false;
        }

        /// <summary>
        /// (Hàm giả lập) Kiểm tra tên đăng nhập đã tồn tại
        /// </summary>
        private bool IsUsernameExists(string username)
        {
            // === CODE DATABASE THẬT ===
            // Logic giả lập:
            return simulatedUserDatabase.ContainsKey(username);
        }

        /// <summary>
        /// (Hàm giả lập) Lưu người dùng mới
        /// </summary>
        private void SaveNewUserToDatabase(string username, string password)
        {
            // === CODE DATABASE THẬT ===
            // Logic giả lập:
            simulatedUserDatabase[username] = password;
        }

        #endregion
    }
}

