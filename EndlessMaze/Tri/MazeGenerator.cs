using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace EndlessMaze.Tri
{
    // Lớp tĩnh chịu trách nhiệm tạo ra dữ liệu mê cung
    public static class MazeGenerator
    {
        private static Random rand = new Random();

        /// <summary>
        /// Tạo mê cung ngẫu nhiên bằng Recursive Backtracking.
        /// </summary>
        /// <param name="rows">Số hàng (chiều cao) của mê cung.</param>
        /// <param name="cols">Số cột (chiều rộng) của mê cung.</param>
        /// <param name="startTile">Vị trí ô bắt đầu.</param>
        /// <param name="endTile">Vị trí ô kết thúc.</param>
        /// <returns>Mảng 2D int đại diện cho mê cung (0=Sàn, 1=Tường).</returns>
        public static int[,] GenerateMaze(int rows, int cols, out Point startTile, out Point endTile)
        {
            // Đảm bảo rows/cols tối thiểu là 3 và là số lẻ để thuật toán hoạt động tốt.
            rows = Math.Max(3, rows);
            cols = Math.Max(3, cols);
            if (rows % 2 == 0) rows--;
            if (cols % 2 == 0) cols--;


            // Khởi tạo mê cung: tất cả đều là Tường (1)
            int[,] maze = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    maze[i, j] = 1;

            // Khởi tạo ô bắt đầu (tại (1, 1) của lưới tế bào thực)
            startTile = new Point(1, 1);

            // Stack cho Recursive Backtracking
            Stack<Point> stack = new Stack<Point>();
            stack.Push(startTile);

            // Đánh dấu ô bắt đầu là Sàn (0)
            maze[startTile.Y, startTile.X] = 0;

            // Khởi tạo điểm kết thúc (ban đầu là điểm xuất phát)
            endTile = startTile;
            int maxDistance = 0;

            // Thuật toán:
            while (stack.Count > 0)
            {
                Point current = stack.Peek();
                int currX = current.X;
                int currY = current.Y;

                // Tính khoảng cách (tạm thời) để tìm điểm kết thúc xa nhất
                // Sử dụng khoảng cách Manhattan (tổng trị tuyệt đối của hiệu tọa độ) có thể tốt hơn cho mê cung
                int distance = Math.Abs(currX - startTile.X) + Math.Abs(currY - startTile.Y);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    endTile = current;
                }

                // Các hướng di chuyển: (dx, dy, x_wall, y_wall)
                var directions = new List<(int dx, int dy, int wx, int wy)>
                {
                    (2, 0, 1, 0),    // Phải
                    (-2, 0, -1, 0),   // Trái
                    (0, 2, 0, 1),    // Xuống
                    (0, -2, 0, -1)   // Lên
                };

                // Xáo trộn hướng để tạo mê cung ngẫu nhiên
                directions = directions.OrderBy(a => rand.Next()).ToList();

                bool moved = false;
                foreach (var dir in directions)
                {
                    int nextX = currX + dir.dx;
                    int nextY = currY + dir.dy;
                    int wallX = currX + dir.wx;
                    int wallY = currY + dir.wy;

                    // Kiểm tra xem ô tiếp theo có hợp lệ và vẫn còn là Tường không
                    if (nextX > 0 && nextX < cols - 1 && nextY > 0 && nextY < rows - 1 && maze[nextY, nextX] == 1)
                    {
                        // Phá tường giữa ô hiện tại và ô tiếp theo
                        maze[wallY, wallX] = 0;
                        // Đánh dấu ô tiếp theo là Sàn
                        maze[nextY, nextX] = 0;

                        stack.Push(new Point(nextX, nextY));
                        moved = true;
                        break;
                    }
                }

                // Nếu không còn hướng di chuyển nào, quay lui (pop)
                if (!moved)
                {
                    stack.Pop();
                }
            }

            // Đảm bảo điểm bắt đầu và kết thúc là ô Sàn (0)
            maze[startTile.Y, startTile.X] = 0;
            maze[endTile.Y, endTile.X] = 0;

            return maze;
        }
    }
}
