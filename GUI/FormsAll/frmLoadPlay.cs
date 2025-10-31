using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL.Models; // <-- THÊM
using BLL.Services; // <-- THÊM

namespace Main
{
    public partial class frmLoadPlay : System.Windows.Forms.Form
    {
        private frmMain _mainForm;

        // --- SỬA: Đổi User thành Users (số nhiều) ---
        private readonly Users _user;
        private readonly PlayerCharacterService _characterService;

        // --- THÊM: Property để form cha đọc ---
        public PlayerCharacters SelectedCharacter { get; private set; }

        // --- SỬA: Đổi User thành Users (số nhiều) ---
        public frmLoadPlay(frmMain mainForm, Users user)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _user = user; // Lưu User
            _characterService = new PlayerCharacterService(); // Khởi tạo BLL
            this.Text = "Chọn Nhân Vật";
        }

        // Constructor cũ (giữ lại cho Designer)
        public frmLoadPlay()
        {
            InitializeComponent();
            // Khởi tạo BLL nếu cần
            _characterService = new PlayerCharacterService();
        }

        // Sự kiện Load Form
        private void frmLoadPlay_Load(object sender, EventArgs e)
        {
            // (Xóa code cũ trong Designer.cs nếu có)
            if (lvSavedGames.Columns.Count == 0)
            {
                lvSavedGames.View = View.Details;
                lvSavedGames.Columns.Add("ID", 50, HorizontalAlignment.Left);
                lvSavedGames.Columns.Add("Tên Nhân Vật", 350, HorizontalAlignment.Left);
                lvSavedGames.Columns.Add("Chỉ Số (HP/ATK/DEF)", 450, HorizontalAlignment.Left);
                lvSavedGames.FullRowSelect = true;
                lvSavedGames.GridLines = true;
            }

            // Tải danh sách nhân vật
            LoadCharacters();
        }

        // --- SỬA: Tải nhân vật từ BLL thay vì dữ liệu giả ---
        private void LoadCharacters()
        {
            // Kiểm tra an toàn
            if (_user == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            lvSavedGames.Items.Clear();

            try
            {
                // Gọi BLL
                List<PlayerCharacters> characters = _characterService.GetCharactersByUserId(_user.UserID);

                if (characters.Count == 0)
                {
                    ListViewItem item = new ListViewItem("Không có nhân vật nào.");
                    item.SubItems.Add("Vui lòng quay lại và chọn 'Chơi Mới'.");
                    lvSavedGames.Items.Add(item);
                    return;
                }

                // Đổ dữ liệu vào ListView
                foreach (var character in characters)
                {
                    ListViewItem item = new ListViewItem(character.CharacterID.ToString());
                    item.SubItems.Add(character.CharacterName);

                    // Hiển thị chỉ số cơ bản
                    string stats = $"HP: {character.BaseHealth} / ATK: {character.BaseAttack} / DEF: {character.BaseDefense}";
                    item.SubItems.Add(stats);

                    item.Tag = character; // Lưu toàn bộ đối tượng vào Tag
                    lvSavedGames.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách nhân vật: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện Double Click để tải game
        private void ListViewSaves_DoubleClick(object sender, EventArgs e)
        {
            ListView listViewSaves = (ListView)sender;
            if (listViewSaves.SelectedItems.Count > 0)
            {
                // Lấy đối tượng PlayerCharacters từ Tag
                PlayerCharacters selectedChar = listViewSaves.SelectedItems[0].Tag as PlayerCharacters;

                if (selectedChar != null)
                {
                    // Lưu nhân vật đã chọn và đóng form
                    this.SelectedCharacter = selectedChar;
                    this.DialogResult = DialogResult.OK; // Báo hiệu chọn thành công
                    this.Close();
                }
            }
        }

        // Sự kiện click cho nút "QUAY LẠI"
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Báo hiệu hủy
            this.Close(); // Đóng form hiện tại
        }

        // Quay về frmMain khi form bị đóng
        private void frmLoadPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Không cần hiển thị lại _mainForm ở đây,
            // vì frmPlay (form gọi nó) vẫn đang mở
        }
    }

    // XÓA LỚP GIẢ LẬP SavedGame
    // public class SavedGame { ... }
}

