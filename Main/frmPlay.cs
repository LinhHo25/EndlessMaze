using System;
using System.Windows.Forms;
using Main.Map;
using Main.Tri;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic; // <-- THÊM DÒNG NÀY

namespace Main
{
    public partial class frmPlay : Form
    {
        private frmMain _mainForm;
        private Random rand = new Random();

        public frmPlay(frmMain mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            this.Text = "Chọn Chế Độ Chơi";
        }

        public frmPlay()
        {
            InitializeComponent();
        }

        // Nút CHƠI MỚI
        private void btnNewGame_Click(object sender, EventArgs e)
        {
            int mapIndex = rand.Next(0, 3);
            IGameMap mapToPlay; // Dùng Interface

            switch (mapIndex)
            {
                case 0:
                    mapToPlay = new Water(_mainForm); // Truyền _mainForm
                    break;
                case 1:
                    mapToPlay = new Flame(_mainForm); // Truyền _mainForm
                    break;
                case 2:
                default:
                    mapToPlay = new Poison(_mainForm); // Truyền _mainForm
                    break;
            }

            StartMap((Form)mapToPlay); // Ép kiểu về Form
        }

        // Nút CHƠI TIẾP (ĐÃ CẬP NHẬT LOGIC)
        private void btnContinue_Click(object sender, EventArgs e)
        {
            // --- BƯỚC 1: Lấy UserID hoặc Username hiện tại ---
            // LƯU Ý: Đảm bảo lớp CurrentUser có thuộc tính public static string Username
            string currentUserIdentifier = CurrentUser.Username; // Lấy từ lớp tĩnh
            if (string.IsNullOrEmpty(currentUserIdentifier))
            {
                MessageBox.Show("Lỗi: Không thể xác định người chơi hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: Truy vấn Database để lấy lượt lưu mới nhất ---
            SavedGameData latestSave = GetLatestSaveFromDatabase(currentUserIdentifier);

            if (latestSave == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu lưu nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // --- BƯỚC 3: Khởi tạo Map tương ứng và Load GameState ---
            try
            {
                // Giải mã GameState từ dữ liệu DB (giả sử lưu dạng JSON)
                GameState stateToLoad = JsonConvert.DeserializeObject<GameState>(latestSave.GameStateJson);

                if (stateToLoad == null)
                {
                    MessageBox.Show("Lỗi: Dữ liệu lưu không hợp lệ.", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                IGameMap mapToLoad;

                // Xác định loại map cần tạo dựa vào MapName đã lưu
                switch (stateToLoad.MapName)
                {
                    case "Water": // Đảm bảo tên này khớp với this.Name trong Water.cs
                        mapToLoad = new Water(_mainForm);
                        break;
                    case "Flame": // Đảm bảo tên này khớp với this.Name trong Flame.cs
                        mapToLoad = new Flame(_mainForm);
                        break;
                    case "Poison": // Đảm bảo tên này khớp với this.Name trong Poison.cs
                        mapToLoad = new Poison(_mainForm);
                        break;
                    default:
                        MessageBox.Show($"Lỗi: Không nhận dạng được map '{stateToLoad.MapName}'.", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                // --- BƯỚC 4: Tải dữ liệu và bắt đầu chơi ---
                mapToLoad.LoadGameState(stateToLoad); // Gọi hàm Load của map
                StartMap((Form)mapToLoad);          // Bắt đầu chơi map đó

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải game: {ex.Message}", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút TẢI LƯỢT CHƠI (mở form frmLoadPlay cũ)
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            // Nếu bạn muốn giữ form frmLoadPlay (dùng file json) thì giữ nguyên
            // Nếu muốn đổi sang frmSaveLoadMenu (dùng DB) thì đổi ở đây:
            // frmSaveLoadMenu saveLoadForm = new frmSaveLoadMenu(_mainForm, null); // Truyền null vì chưa vào map
            // saveLoadForm.ShowDialog(this);

            // Hiện tại đang dùng form cũ (frmLoadPlay)
            this.Hide();
            frmLoadPlay loadPlayForm = new frmLoadPlay(_mainForm);
            loadPlayForm.Show();
        }

        // Hàm chung để khởi chạy map
        private void StartMap(Form mapForm)
        {
            this.Hide();
            mapForm.ShowDialog();
            // KHÔNG Close() ở đây nữa, để khi quay lại từ map thì frmPlay vẫn còn
            // Thay vào đó, xử lý ở frmPlay_VisibleChanged
        }

        // Nút QUAY LẠI MENU
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form này sẽ tự động hiển thị frmMain (do xử lý ở FormClosed)
        }

        // Sự kiện khi form bị đóng, hiển thị lại frmMain
        private void frmPlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Chỉ hiện lại frmMain nếu nó đang bị ẩn (tránh trường hợp thoát game)
            if (_mainForm != null && !_mainForm.Visible && Application.OpenForms.Count > 0)
            {
                _mainForm.Show();
            }
        }

        // Sự kiện khi form này được hiện lại (sau khi map đóng)
        private void frmPlay_VisibleChanged(object sender, EventArgs e)
        {
            // Nếu form này được hiện lại VÀ frmMain đang ẩn, thì hiện lại frmMain
            // (Xử lý trường hợp quay về Main Menu từ trong map)
            if (this.Visible && _mainForm != null && !_mainForm.Visible)
            {
                _mainForm.Show();
                this.Hide(); // Ẩn form này đi
            }
        }


        // =============================================================
        // HÀM GIẢ LẬP TRUY VẤN DATABASE (Cần thay thế bằng code thật)
        // =============================================================
        private SavedGameData GetLatestSaveFromDatabase(string userId)
        {
            // === CODE DATABASE THẬT SẼ Ở ĐÂY ===
            // 1. // Kết nối Database (SQL Server, Firebase, ...)
            // 2. // Tạo câu truy vấn:
            //    // SELECT TOP 1 SaveSlotName, GameStateData, Timestamp
            //    // FROM SavedGames
            //    // WHERE UserID = @userId
            //    // ORDER BY Timestamp DESC
            // 3. // Thực thi truy vấn, đọc dữ liệu trả về
            // 4. // Nếu có kết quả:
            //    //    Tạo đối tượng SavedGameData và gán giá trị
            //    //    return savedGameDataObject;
            // 5. // Nếu không có kết quả:
            //    //    return null;
            // =====================================

            // ----- Dữ liệu giả lập để test -----
            // Giả sử có 1 lượt lưu
            if (userId == "testuser") // Thay bằng Username thật đã đăng nhập
            {
                // Tạo GameState mẫu
                GameState fakeState = new GameState
                {
                    MapName = "Water", // Map mặc định khi test
                    PlayerX = 100,
                    PlayerY = 150,
                    PlayerHealth = 85,
                    PlayerStamina = 90,
                    Inventory = new Dictionary<ItemType, int> { { ItemType.HealthPotion, 2 } }
                };
                // Chuyển GameState thành JSON (giống cách lưu)
                string fakeJson = JsonConvert.SerializeObject(fakeState);

                return new SavedGameData
                {
                    SaveSlotName = "Lưu tự động",
                    GameStateJson = fakeJson, // Lưu dạng JSON string
                    Timestamp = DateTime.Now.AddMinutes(-10) // Thời gian lưu
                };
            }
            // ----------------------------------

            return null; // Không tìm thấy save
        }

        // Lớp phụ trợ (giống trong frmSaveLoadMenu) để chứa dữ liệu đọc từ DB
        public class SavedGameData
        {
            public int SaveId { get; set; } // ID trong database (nếu có)
            public string SaveSlotName { get; set; }
            public string GameStateJson { get; set; } // Dữ liệu game (dạng JSON)
            public DateTime Timestamp { get; set; }
        }
        // =============================================================

    }
}

