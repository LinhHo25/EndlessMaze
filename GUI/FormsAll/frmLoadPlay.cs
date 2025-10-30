using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main
{
    // LỚP NÀY PHẢI ĐỨNG ĐẦU TIÊN TRONG FILE
    public partial class frmLoadPlay : System.Windows.Forms.Form
    {
        private frmMain _mainForm;

        // Constructor mới nhận tham chiếu đến frmMain
        public frmLoadPlay(frmMain mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            this.Text = "Danh sách lượt chơi đã lưu";

            // Tải dữ liệu khi form khởi tạo
            LoadSavedGames();
        }

        // Constructor cũ
        public frmLoadPlay()
        {
            InitializeComponent();
            LoadSavedGames();
        }

        // Sự kiện Load Form
        private void frmLoadPlay_Load(object sender, EventArgs e)
        {
            // Đảm bảo ListView có các cột nếu Designer không tự làm
            if (lvSavedGames.Columns.Count == 0)
            {
                lvSavedGames.View = View.Details;
                lvSavedGames.Columns.Add("ID", 50, HorizontalAlignment.Left);
                lvSavedGames.Columns.Add("Tên Lượt Chơi", 200, HorizontalAlignment.Left);
                lvSavedGames.Columns.Add("Thời gian Lưu", 150, HorizontalAlignment.Left);
                lvSavedGames.Columns.Add("Tiến độ", 250, HorizontalAlignment.Left);
                lvSavedGames.FullRowSelect = true;
                lvSavedGames.GridLines = true;
            }
        }

        // Hàm giả lập việc tải dữ liệu từ database (Có chú thích)
        private void LoadSavedGames()
        {
            // Sử dụng lvSavedGames đã được định nghĩa trong Designer
            lvSavedGames.Items.Clear();

            List<SavedGame> savedGames = new List<SavedGame>();

            // ===================================================================
            // CHÚ THÍCH DATABASE: PHẦN CẦN THAY THẾ KHI CÓ KẾT NỐI DB THẬT
            // 1. KẾT NỐI DB
            // 2. TRUY VẤN VÀ LẤY TẤT CẢ LƯỢT CHƠI ĐÃ LƯU CỦA TÀI KHOẢN ĐANG ĐĂNG NHẬP
            //    (Sử dụng User ID đã lưu sau khi đăng nhập thành công)
            // 3. ĐỌC DỮ LIỆU và ĐƯA VÀO LIST savedGames
            // ===================================================================

            // DỮ LIỆU GIẢ ĐỊNH (MOCK DATA) ĐỂ DEMO GIAO DIỆN
            savedGames.Add(new SavedGame { SaveId = 1, Title = "Thử thách Labyrinth: Tầng 5", SavedTime = DateTime.Now.AddDays(-1), ProgressInfo = "Vị trí: (10, 15), Máu: 80/100, Vật phẩm: 3" });
            savedGames.Add(new SavedGame { SaveId = 2, Title = "Phòng thủ Pháo đài: Wave 12", SavedTime = DateTime.Now.AddHours(-5), ProgressInfo = "Vị trí: (5, 8), Máu: 100/100, Vật phẩm: 0" });
            savedGames.Add(new SavedGame { SaveId = 3, Title = "Khám phá Rừng cổ: Ngày thứ 7", SavedTime = DateTime.Now.AddDays(-3), ProgressInfo = "Vị trí: (30, 2), Máu: 50/100, Vật phẩm: 10" });


            // Đổ dữ liệu vào ListView
            foreach (var game in savedGames)
            {
                ListViewItem item = new ListViewItem(game.SaveId.ToString());
                item.SubItems.Add(game.Title);
                item.SubItems.Add(game.SavedTime.ToString("dd/MM/yyyy HH:mm"));
                item.SubItems.Add(game.ProgressInfo);
                item.Tag = game; // Lưu toàn bộ đối tượng vào Tag để dễ dàng lấy ra khi double click
                lvSavedGames.Items.Add(item);
            }
        }

        // Sự kiện Double Click để tải game
        private void ListViewSaves_DoubleClick(object sender, EventArgs e)
        {
            ListView listViewSaves = (ListView)sender;
            if (listViewSaves.SelectedItems.Count > 0)
            {
                // Lấy đối tượng SavedGame từ Tag của Item đã chọn
                SavedGame selectedGame = listViewSaves.SelectedItems[0].Tag as SavedGame;

                if (selectedGame != null)
                {
                    MessageBox.Show($"Đang tải lượt chơi: {selectedGame.Title} (ID: {selectedGame.SaveId})...", "Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();

                    // NẾU TẢI THÀNH CÔNG, CHUYỂN ĐẾN FORM GAME CHÍNH
                }
            }
        }

        // Sự kiện click cho nút "QUAY LẠI"
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form hiện tại
        }

        // Quay về frmMain khi form bị đóng
        private void frmLoadPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Khi frmLoadPlay đóng, form cha (frmPlay) đã bị ẩn. 
            // Ta cần hiển thị lại frmPlay (hoặc frmMain nếu frmPlay cũng đóng)

            // Hiện tại, ta chỉ đảm bảo frmMain hiển thị lại nếu nó đang bị ẩn.
            if (_mainForm != null && _mainForm.Visible == false)
            {
                _mainForm.Show();
            }
        }
    }

    // LỚP HỖ TRỢ ĐÃ ĐƯỢC DI CHUYỂN XUỐNG DƯỚI LỚP FORM
    public class SavedGame
    {
        public int SaveId { get; set; }
        public string Title { get; set; }
        public DateTime SavedTime { get; set; }
        public string ProgressInfo { get; set; }
    }
}
