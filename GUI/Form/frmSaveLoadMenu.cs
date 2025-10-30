using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Main.Tri;
using Main.Map; // <-- THÊM DÒNG NÀY ĐỂ SỬA LỖI CS0246
using Newtonsoft.Json;
using System.Linq;

namespace Main
{
    public partial class frmSaveLoadMenu : Form
    {
        private frmMain _mainMenuForm;
        private IGameMap _currentGameMap;
        private List<SavedGameData> loadedSaves = new List<SavedGameData>();

        public frmSaveLoadMenu(frmMain mainMenuForm, IGameMap mapInstance)
        {
            InitializeComponent();
            _mainMenuForm = mainMenuForm;
            _currentGameMap = mapInstance;

            // Đảm bảo nút Save/Load có tồn tại trước khi dùng
            if (btnSave != null) btnSave.Enabled = (_currentGameMap != null);

            LoadSaveSlots();
        }

        private void LoadSaveSlots()
        {
            // Đảm bảo ListView và Label tồn tại
            if (lvSaveSlots == null || lblInfo == null) return;

            lvSaveSlots.Items.Clear();
            loadedSaves.Clear();

            if (!CurrentUser.IsLoggedIn)
            {
                lblInfo.Text = "Bạn cần đăng nhập để xem danh sách lưu.";
                if (btnLoad != null) btnLoad.Enabled = false;
                if (btnSave != null) btnSave.Enabled = false;
                return;
            }
            string currentUserIdentifier = CurrentUser.Username;

            // === CODE DATABASE: LẤY DANH SÁCH LƯU ===
            // ... (Phần code database giữ nguyên) ...

            // ----- Dữ liệu giả lập -----
            if (currentUserIdentifier == "testuser")
            {
                GameState fakeState1 = new GameState { MapName = "Water", PlayerX = 100, PlayerY = 150, PlayerHealth = 85, PlayerStamina = 90, Inventory = new Dictionary<ItemType, int>() };
                string fakeJson1 = JsonConvert.SerializeObject(fakeState1);
                loadedSaves.Add(new SavedGameData { SaveId = 1, SaveSlotName = "Slot 1 - Map Nước", GameStateJson = fakeJson1, Timestamp = DateTime.Now.AddHours(-1) });

                GameState fakeState2 = new GameState { MapName = "Flame", PlayerX = 200, PlayerY = 50, PlayerHealth = 100, PlayerStamina = 50, Inventory = new Dictionary<ItemType, int> { { ItemType.HealthPotion, 3 } } };
                string fakeJson2 = JsonConvert.SerializeObject(fakeState2);
                loadedSaves.Add(new SavedGameData { SaveId = 2, SaveSlotName = "Slot 2 - Map Lửa", GameStateJson = fakeJson2, Timestamp = DateTime.Now.AddMinutes(-30) });
            }
            // ---------------------------

            if (loadedSaves.Count > 0)
            {
                foreach (var save in loadedSaves)
                {
                    ListViewItem item = new ListViewItem(save.SaveSlotName);
                    item.SubItems.Add(save.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"));
                    item.Tag = save;
                    lvSaveSlots.Items.Add(item);
                }
                lblInfo.Text = $"Tìm thấy {loadedSaves.Count} lượt lưu.";
                if (btnLoad != null) btnLoad.Enabled = true;
            }
            else
            {
                lblInfo.Text = "Không tìm thấy lượt lưu nào.";
                if (btnLoad != null) btnLoad.Enabled = false;
            }
        }

        // ĐỔI TÊN HÀM CHO KHỚP VỚI DESIGNER
        private void btnSaveGame_Click(object sender, EventArgs e)
        {
            if (_currentGameMap == null || !CurrentUser.IsLoggedIn) return;

            GameState currentState = _currentGameMap.GetCurrentGameState();
            if (currentState == null)
            {
                MessageBox.Show("Lỗi: Không thể lấy trạng thái game hiện tại.", "Lỗi Lưu Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string gameStateJson = JsonConvert.SerializeObject(currentState, Formatting.Indented);
            string saveSlotName = PromptForSaveName($"Slot {DateTime.Now:dd-MM HH:mm}");

            if (string.IsNullOrWhiteSpace(saveSlotName)) return;

            string currentUserIdentifier = CurrentUser.Username;

            // === CODE DATABASE: LƯU GAME ===
            // ... (Phần code database giữ nguyên) ...

            MessageBox.Show($"Đã lưu game vào slot '{saveSlotName}'.", "Lưu Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadSaveSlots();
        }

        // ĐỔI TÊN HÀM CHO KHỚP VỚI DESIGNER
        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            if (lvSaveSlots == null || lvSaveSlots.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một slot để tải.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SavedGameData selectedSave = lvSaveSlots.SelectedItems[0].Tag as SavedGameData;
            if (selectedSave == null)
            {
                MessageBox.Show("Lỗi: Không thể đọc dữ liệu của slot đã chọn.", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                GameState stateToLoad = JsonConvert.DeserializeObject<GameState>(selectedSave.GameStateJson);

                if (stateToLoad == null)
                {
                    MessageBox.Show("Lỗi: Dữ liệu lưu không hợp lệ.", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_currentGameMap != null)
                {
                    if (((Form)_currentGameMap).Name == stateToLoad.MapName)
                    {
                        _currentGameMap.LoadGameState(stateToLoad);
                        this.Close();
                    }
                    else
                    {
                        var confirmResult = MessageBox.Show($"Bạn đang ở map '{((Form)_currentGameMap).Name}'.\nLượt lưu này thuộc về map '{stateToLoad.MapName}'.\nBạn có muốn thoát map hiện tại và tải game không?",
                                                         "Xác Nhận Tải Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirmResult == DialogResult.Yes)
                        {
                            LoadMapAndState(stateToLoad);
                        }
                    }
                }
                else
                {
                    LoadMapAndState(stateToLoad);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải game: {ex.Message}", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMapAndState(GameState stateToLoad)
        {
            if (!CurrentUser.IsLoggedIn) return;

            IGameMap mapToLoad;
            switch (stateToLoad.MapName)
            {
                case "Water": mapToLoad = new Water(_mainMenuForm); break;
                case "Flame": mapToLoad = new Flame(_mainMenuForm); break;
                case "Poison": mapToLoad = new Poison(_mainMenuForm); break;
                default:
                    MessageBox.Show($"Lỗi: Không nhận dạng được map '{stateToLoad.MapName}'.", "Lỗi Tải Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            if (_currentGameMap != null)
            {
                ((Form)_currentGameMap).Close();
            }
            this.Close();

            mapToLoad.LoadGameState(stateToLoad);
            if (Application.OpenForms["frmPlay"] != null) Application.OpenForms["frmPlay"].Hide();
            ((Form)mapToLoad).ShowDialog(_mainMenuForm);

        }

        // Nút QUAY LẠI (giữ nguyên)
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Hàm PromptForSaveName (giữ nguyên)
        private string PromptForSaveName(string defaultName)
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 300;
                prompt.Height = 150;
                prompt.Text = "Nhập tên lượt lưu";
                prompt.StartPosition = FormStartPosition.CenterParent;
                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Tên slot:", AutoSize = true };
                TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 250, Text = defaultName };
                Button confirmation = new Button() { Text = "Lưu", Left = 190, Width = 80, Top = 80, DialogResult = DialogResult.OK };
                confirmation.Click += (s, ev) => { prompt.Close(); }; // Sửa lại lambda
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        // HÀM MỚI ĐỂ XỬ LÝ DOUBLE CLICK (theo Designer)
        private void lvSaveSlots_DoubleClick(object sender, EventArgs e)
        {
            // Gọi luôn hàm xử lý của nút Load
            btnLoadGame_Click(sender, e);
        }

        // HÀM MỚI ĐỂ XỬ LÝ LOAD FORM (theo Designer)
        private void frmSaveLoadMenu_Load(object sender, EventArgs e)
        {
            // Hàm LoadSaveSlots() đã được gọi trong constructor rồi
            // Có thể thêm code khác nếu cần khi form load
        }


        // Lớp SavedGameData (giữ nguyên)
        public class SavedGameData
        {
            public int SaveId { get; set; }
            public string SaveSlotName { get; set; }
            public string GameStateJson { get; set; }
            public DateTime Timestamp { get; set; }
        }

    }
}

