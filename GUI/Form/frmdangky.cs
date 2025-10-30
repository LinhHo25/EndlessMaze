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
            
        }

        // Sự kiện click cho nút ĐĂNG KÝ (ĐÃ SỬA)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            
        }

        // Sự kiện đóng Form, quay lại frmMain
        private void frmDangKy_FormClosed(object sender, FormClosedEventArgs e)
        {
            
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

