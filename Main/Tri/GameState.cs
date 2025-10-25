using System.Collections.Generic; // Thêm
using Main.Tri; // Thêm để dùng ItemType

namespace Main.Tri
{
    // Lớp chứa dữ liệu trạng thái game để lưu/tải
    public class GameState
    {
        public string MapName { get; set; }
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public float PlayerHealth { get; set; }
        public float PlayerStamina { get; set; }
        public Dictionary<ItemType, int> Inventory { get; set; } // <-- THÊM DÒNG NÀY

        // Có thể thêm các thông tin khác cần lưu (ví dụ: vị trí quái, trạng thái nhiệm vụ...)
    }
}

