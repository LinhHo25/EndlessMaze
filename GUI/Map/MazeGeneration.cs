using System;
using System.Collections.Generic;
using System.Linq;

// Đặt trong namespace Main để lớp GameMapBase có thể dễ dàng truy cập.
namespace Main
{
    // --- ENUM CẦN THIẾT CHO MAZE GENERATOR VÀ GAME (Khắc phục lỗi CS0117) ---
    public enum GameObjectType
    {
        Wall = 0,
        Passage = 1,
        Start = 2,
        Exit = 3
    }

    /// <summary>
    /// Lớp tạo mê cung sử dụng thuật toán Recursive Backtracker (Quay lui Đệ quy) với đường đi rộng 1 ô.
    /// </summary>
    public class MazeGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private int[,] _maze;
        private readonly Random _rand = new Random();

        public int[,] Maze => _maze;

        // --- HẰNG SỐ QUAN TRỌNG: Bước nhảy để tạo đường đi 1 ô Wall giữa các Passage ---
        private const int STEP = 2; // Bước nhảy: Passage cách Passage 1 ô Wall (Passage - W - Passage)

        public MazeGenerator(int width, int height)
        {
            // Đảm bảo kích thước là 2k + 1 để phù hợp với STEP=2
            int adjustedWidth = width % STEP == 1 ? width : width + (STEP - (width % STEP)) + 1;
            int adjustedHeight = height % STEP == 1 ? height : height + (STEP - (height % STEP)) + 1;

            _width = Math.Max(STEP + 1, adjustedWidth);
            _height = Math.Max(STEP + 1, adjustedHeight);

            _maze = new int[_height, _width];
        }

        /// <summary>
        /// Khởi tạo và tạo mê cung.
        /// </summary>
        public void GenerateMaze()
        {
            // 1. Khởi tạo toàn bộ lưới là tường (Wall)
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _maze[y, x] = (int)GameObjectType.Wall;
                }
            }

            // 2. Bắt đầu thuật toán Recursive Backtracker từ vị trí (1, 1)
            // (1, 1) luôn là chỉ số hợp lệ.
            _maze[1, 1] = (int)GameObjectType.Passage;
            GenerateMazeRecursive(1, 1);

            // 3. Thiết lập cửa vào/ra hợp lệ (1x1)
            _maze[1, 1] = (int)GameObjectType.Start;
            _maze[_height - 2, _width - 2] = (int)GameObjectType.Passage;
        }

        private void GenerateMazeRecursive(int x, int y)
        {
            // Danh sách các hướng ngẫu nhiên: (dx, dy)
            var directions = new List<(int, int)> { (0, -STEP), (0, STEP), (-STEP, 0), (STEP, 0) };
            directions = directions.OrderBy(d => _rand.Next()).ToList(); // Trộn ngẫu nhiên

            foreach (var (dx, dy) in directions)
            {
                int nextX = x + dx;
                int nextY = y + dy;

                // KIỂM TRA BIÊN: nextX và nextY phải nằm trong [1, _width - 2]
                if (nextX >= 1 && nextX <= _width - 2 && nextY >= 1 && nextY <= _height - 2)
                {
                    // Kiểm tra ô đích (nếu vẫn là tường)
                    if (_maze[nextY, nextX] == (int)GameObjectType.Wall)
                    {
                        // 1. Đục lối đi chính đến ô đích
                        _maze[nextY, nextX] = (int)GameObjectType.Passage;

                        // 2. Đục 1 ô Tường giữa
                        int wallX = x + dx / 2;
                        int wallY = y + dy / 2;
                        _maze[wallY, wallX] = (int)GameObjectType.Passage;

                        // 3. Tiếp tục đệ quy từ ô đích
                        GenerateMazeRecursive(nextX, nextY);
                    }
                }
            }
        }
    }
}
