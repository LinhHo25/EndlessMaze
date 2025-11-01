using BLL.Services; // <-- THÊM: Sử dụng BLL
using DAL.Models; // <-- THÊM: Dùng Models
using System;
using System.Collections.Generic;
using System.Linq; // <-- THÊM: Cần cho .FirstOrDefault()
using System.Windows.Forms;
// --- THÊM: Các thư viện cho Hashing (Băm mật khẩu) ---
using System.Security.Cryptography;
using System.Text;
// --- HẾT PHẦN THÊM ---

namespace Main
{
    public partial class frmDangKy : System.Windows.Forms.Form
    {
        private frmMain _mainForm;

        // --- THÊM: Khởi tạo AuthService từ BLL ---
        private readonly AuthService _authService;
        private readonly PlayerCharacterService _characterService; // <-- THÊM: Để tạo nhân vật
        // ------------------------------------------

        // Constructor mới nhận tham chiếu đến frmMain
        public frmDangKy(frmMain mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            this.Text = "Đăng Nhập / Đăng Ký Tài Khoản";

            // --- THÊM: Khởi tạo BLL Service ---
            _authService = new AuthService();
            _characterService = new PlayerCharacterService(); // <-- THÊM
            // ------------------------------------
        }

        // Constructor cũ
        public frmDangKy()
        {
            InitializeComponent();
            // --- THÊM: Khởi tạo BLL Service ---
            _authService = new AuthService();
            _characterService = new PlayerCharacterService(); // <-- THÊM
            // ------------------------------------
        }

        // Sự kiện click cho nút ĐĂNG NHẬP (ĐÃ SỬA)
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- SỬ DỤNG BLL: Gọi AuthService để đăng nhập ---
            // (Chúng ta giả định AuthService tự băm mật khẩu)
            var user = _authService.Login(username, password);
            // -----------------------------------------------

            if (user != null)
            {
                MessageBox.Show($"Chào mừng trở lại, {user.Username}!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- SỬA: Mở frmPlay và truyền đối tượng Users ---
                frmPlay playForm = new frmPlay(_mainForm, user);
                playForm.Show();
                // ----------------------------------------------

                this.Close(); // Đóng form đăng nhập
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện click cho nút ĐĂNG KÝ (ĐÃ SỬA)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- SỬA LỖI 1: Hàm Register trả về 'bool' ---
            // var newUser = _authService.Register(username, password);
            bool registrationSuccess = _authService.Register(username, password);
            // ---------------------------------------------

            if (registrationSuccess) // Sửa: if (newUser != null) -> if (registrationSuccess)
            {
                MessageBox.Show("Đăng ký tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- SỬA: Đăng nhập ngay lập tức để lấy User object ---
                User justRegisteredUser = _authService.Login(username, password);
                if (justRegisteredUser == null)
                {
                    MessageBox.Show("Lỗi: Không thể tự động đăng nhập sau khi đăng ký.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // --- HẾT PHẦN SỬA ---

                // --- THÊM: Tự động tạo nhân vật mặc định ---
                try
                {
                    // SỬA: Dùng justRegisteredUser.UserID
                    _characterService.CreateCharacter(justRegisteredUser.UserID, "Nhà Thám Hiểm");
                    MessageBox.Show("Đã tạo nhân vật mặc định 'Nhà Thám Hiểm' cho bạn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tự động đăng nhập
                    // SỬA: Dùng justRegisteredUser
                    frmPlay playForm = new frmPlay(_mainForm, justRegisteredUser);
                    playForm.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo nhân vật mặc định: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // -----------------------------------------
            }
            else
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.", "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // (Giữ nguyên các hàm frmDangKy_FormClosed và frmdangky_Load)
        // ... (các hàm khác) ...
        // Sự kiện đóng Form, quay lại frmMain
        private void frmDangKy_FormClosed(object sender, FormClosedEventArgs e)
        {
            // --- SỬA: Hiển thị lại main form khi đóng ---
            if (_mainForm != null && !_mainForm.IsDisposed)
            {
                // Chỉ hiển thị nếu form play chưa mở
                // (Kiểm tra xem có form nào khác ngoài Main đang mở không)
                if (Application.OpenForms.Count == 1)
                {
                    _mainForm.Show();
                }
            }
            // -------------------------------------------
        }

        // Sự kiện Load Form
        private void frmdangky_Load(object sender, EventArgs e)
        {
            if (this.Controls.Find("txtPassword", true).FirstOrDefault() is TextBox txtPass)
            {
                txtPass.PasswordChar = '*';
            }
        }
    }
}

