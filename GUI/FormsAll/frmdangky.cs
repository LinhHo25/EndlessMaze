using BLL.Services; // <-- THÊM: Sử dụng BLL
using DAL.Models; // <-- THÊM: Dùng Models
using System;
using System.Collections.Generic;
using System.Linq; // <-- THÊM: Cần cho .FirstOrDefault()
using System.Windows.Forms;

namespace Main
{
    // ĐỔI TÊN file này thành frmDangKy.cs khi sử dụng
    public partial class frmDangKy : Form
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
            // Giả sử các controls của bạn có tên là txtUsername và txtPassword
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- SỬ DỤNG BLL: Gọi AuthService để đăng nhập ---
            var user = _authService.Login(username, password);
            // -----------------------------------------------

            if (user != null)
            {
                MessageBox.Show($"Chào mừng trở lại, {user.Username}!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- SỬA: Mở frmPlay và truyền đối tượng Users ---
                frmPlay playForm = new frmPlay(_mainForm, user);
                playForm.Show();
                // ----------------------------------------------

                _mainForm.Hide(); // Ẩn form chính
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
            // Giả sử các controls của bạn có tên là txtUsername và txtPassword
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- SỬA: Gọi AuthService (đã sửa) để đăng ký ---
            var newUser = _authService.Register(username, password);
            // ---------------------------------------------

            if (newUser != null)
            {
                MessageBox.Show("Đăng ký tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // --- THÊM: Tự động tạo nhân vật mặc định ---
                // (Giả sử bạn có hàm SendWelcomeEmail trong AuthService)
                // _authService.SendWelcomeEmail(newUser.Username); 
                try
                {
                    // --- SỬA: Đã vô hiệu hóa việc tự động tạo nhân vật ---
                    // _characterService.CreateCharacter(newUser.UserID, "Nhà Thám Hiểm");
                    // MessageBox.Show("Đã tạo nhân vật mặc định 'Nhà Thám Hiểm' cho bạn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // --- KẾT THÚC SỬA ---

                    // Tự động đăng nhập
                    frmPlay playForm = new frmPlay(_mainForm, newUser);
                    playForm.Show();
                    _mainForm.Hide(); // Ẩn form chính
                    this.Close();
                }
                catch (Exception ex)
                {
                    // --- SỬA: Thay đổi thông báo lỗi ---
                    MessageBox.Show("Lỗi khi tự động đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // -----------------------------------------
            }
            else
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại. Vui lòng chọn tên khác.", "Đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện đóng Form, quay lại frmMain
        private void frmDangKy_FormClosed(object sender, FormClosedEventArgs e)
        {
            // --- SỬA: Hiển thị lại main form khi đóng ---
            if (_mainForm != null && !_mainForm.IsDisposed)
            {
                // Chỉ hiển thị nếu không có form nào khác (như frmPlay) đang mở
                if (Application.OpenForms.Count == 1 && Application.OpenForms[0].Name == "frmMain")
                {
                    _mainForm.Show();
                }
            }
            // -------------------------------------------
        }

        // Sự kiện Load Form
        private void frmdangky_Load(object sender, EventArgs e)
        {
            // Tự động tìm control txtPassword và đặt thuộc tính PasswordChar
            // (Giả sử control của bạn tên là txtPassword)
            if (this.Controls.Find("txtPassword", true).FirstOrDefault() is TextBox txtPass)
            {
                txtPass.PasswordChar = '*';
            }
        }

        // --- XÓA: Toàn bộ region #region Database Functions đã được chuyển sang BLL ---
    }
}

