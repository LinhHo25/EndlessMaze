using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic; // <-- THÊM DÒNG NÀY
using DAL.Models; // <-- THÊM
using BLL.Services; // <-- THÊM
using System.Drawing; // <-- THÊM (Cho InputBox)

namespace Main
{
    public partial class frmPlay : System.Windows.Forms.Form
    {
        private frmMain _mainForm;
        private Random rand = new Random();

        // --- THÊM: Dữ liệu User và BLL ---
        private readonly Users _currentUser;
        private readonly PlayerCharacterService _characterService;

        // --- SỬA: Constructor nhận Users ---
        public frmPlay(frmMain mainForm, Users user)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _currentUser = user; // Lưu user
            _characterService = new PlayerCharacterService(); // Khởi tạo BLL
            this.Text = $"Chọn Chế Độ Chơi (User: {_currentUser.Username})";
        }

        public frmPlay()
        {
            InitializeComponent();
            // (Constructor này chỉ dùng cho Designer)
            _characterService = new PlayerCharacterService();
        }

        // Nút CHƠI MỚI
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            if (_currentUser == null) return;

            // Hỏi tên nhân vật
            string charName = InputBox.Show("Tạo Nhân Vật Mới", "Nhập tên nhân vật của bạn:", "Nhà Thám Hiểm");

            if (!string.IsNullOrWhiteSpace(charName))
            {
                try
                {
                    // --- SỬA: Thay 'bool success' bằng 'string errorMessage' ---
                    string errorMessage = _characterService.CreateCharacter(_currentUser.UserID, charName);

                    if (string.IsNullOrEmpty(errorMessage)) // <-- SỬA: Kiểm tra lỗi
                    {
                        MessageBox.Show($"Tạo nhân vật '{charName}' thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Tự động tải nhân vật vừa tạo
                        var newChar = _characterService.GetCharactersByUserId(_currentUser.UserID)
                                                    .OrderByDescending(c => c.CharacterID) // Lấy nhân vật mới nhất
                                                    .FirstOrDefault(c => c.CharacterName == charName);
                        if (newChar != null)
                        {
                            // Bắt đầu game mới ở Map 1
                            StartGame(newChar.CharacterID, 1);
                        }
                    }
                    else
                    {
                        // --- SỬA: Hiển thị lỗi chi tiết từ BLL ---
                        MessageBox.Show("Lỗi khi tạo nhân vật:\n" + errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // --- SỬA: Hiển thị INNER EXCEPTION (Lỗi Gốc) ---
                    // Đây là thay đổi quan trọng nhất để chẩn đoán lỗi
                    string detailedError = ex.Message;
                    if (ex.InnerException != null)
                    {
                        // Đi sâu vào 2 lớp để lấy lỗi CSDL thực tế
                        detailedError += "\n\nLỗi Gốc (Inner Exception):\n" + (ex.InnerException?.InnerException?.Message ?? ex.InnerException.Message);
                    }
                    MessageBox.Show("Lỗi nghiêm trọng khi tạo nhân vật:\n" + detailedError, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // --- KẾT THÚC SỬA ---
                }
            }
        }

        // Nút CHƠI TIẾP (Tải nhân vật đầu tiên)
        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (_currentUser == null) return;
            try
            {
                // Tải nhân vật đầu tiên của user
                var firstChar = _characterService.GetCharactersByUserId(_currentUser.UserID).FirstOrDefault();
                if (firstChar != null)
                {
                    // Tạm thời, chúng ta giả định bắt đầu từ Map 1 nếu 'Continue'
                    int lastMapLevel = 1; // (Nên thay bằng logic đọc file save)
                    StartGame(firstChar.CharacterID, lastMapLevel);
                }
                else
                {
                    MessageBox.Show("Bạn chưa có nhân vật nào. Vui lòng chọn 'Chơi Mới'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải nhân vật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút TẢI LƯỢT CHƠI (Mở form chọn nhân vật)
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (_currentUser == null) return;

            frmLoadPlay loadForm = new frmLoadPlay(_mainForm, _currentUser);
            var result = loadForm.ShowDialog();

            if (result == DialogResult.OK && loadForm.SelectedCharacter != null)
            {
                // Tạm thời, chúng ta giả định bắt đầu từ Map 1
                int mapLevel = 1;
                StartGame(loadForm.SelectedCharacter.CharacterID, mapLevel);
            }
        }

        // --- SỬA: Hàm khởi chạy game (nhận thêm mapLevel) ---
        private void StartGame(int characterId, int mapLevel)
        {
            try
            {
                var characterDetails = _characterService.GetCharacterDetails(characterId);
                if (characterDetails == null)
                {
                    MessageBox.Show("Không thể tải thông tin chi tiết của nhân vật.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- SỬA: Mở frmMazeGame thay vì MessageBox ---
                // (Giả sử bạn có frmMazeGame_v3.cs trong dự án)
                frmMainGame mainGame = new frmMainGame(characterDetails, mapLevel);
                this.Hide();
                mainGame.ShowDialog();
                // Sau khi game đóng (thua hoặc thoát), hiển thị lại form này
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi bắt đầu game: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Nút QUAY LẠI MENU
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Sự kiện khi form bị đóng, hiển thị lại frmMain
        private void frmPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_mainForm != null && !_mainForm.IsDisposed)
            {
                _mainForm.Show();
            }
        }

        // (Bỏ trống)
        private void frmPlay_VisibleChanged(object sender, EventArgs e)
        {
        }

        private void frmPlay_Load(object sender, EventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Lỗi: Không có thông tin người dùng. Vui lòng đăng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnNewGame.Enabled = false;
                btnContinue.Enabled = false;
                BtnLoad.Enabled = false;
                lblTitle.Text = "LỖI: KHÔNG CÓ USER";
            }
        }

        // --- THÊM: Lớp helper để tạo InputBox ---
        public static class InputBox
        {
            public static string Show(string title, string promptText, string defaultValue = "")
            {
                Form form = new Form();
                Label label = new Label();
                TextBox textBox = new TextBox();
                Button buttonOk = new Button();
                Button buttonCancel = new Button();

                form.Text = title;
                label.Text = promptText;
                textBox.Text = defaultValue;

                buttonOk.Text = "OK";
                buttonCancel.Text = "Cancel";
                buttonOk.DialogResult = DialogResult.OK;
                buttonCancel.DialogResult = DialogResult.Cancel;

                label.SetBounds(9, 20, 372, 13);
                textBox.SetBounds(12, 36, 372, 20);
                buttonOk.SetBounds(228, 72, 75, 23);
                buttonCancel.SetBounds(309, 72, 75, 23);

                label.AutoSize = true;
                textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
                buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                form.ClientSize = new Size(396, 107);
                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                DialogResult dialogResult = form.ShowDialog();
                return dialogResult == DialogResult.OK ? textBox.Text : "";
            }
        }
    }
}

