using System.Drawing;

namespace Main.Tri
{
    // Interface này giúp frmSaveLoadMenu giao tiếp với các map
    // mà không cần biết đó là Water, Flame, hay Poison
    public interface IGameMap
    {
        GameState GetCurrentGameState();
        void LoadGameState(GameState state);

        // Các thuộc tính/phương thức đã có mà chúng ta cần
        Player Player { get; }
        void ResumeGame();
        void PauseGame();
        PointF GetPlayerSpawnPoint();
        string Name { get; } // Tên của Form (ví dụ: "Water")
        void Close();
    }
}
