using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Main.Tri // Đã cập nhật namespace
{
    public class MazeGenerator
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int[,] Maze { get; private set; }
        private Random random = new Random();

        // Định nghĩa loại ô
        private const int WALL = 1;
        private const int FLOOR = 0;

        // Lưu trữ vị trí Bắt đầu và Kết thúc (tọa độ Grid)
        public Point StartGridPosition { get; private set; }
        public Point EndGridPosition { get; private set; }

        /// <summary>
        /// Khởi tạo bộ tạo mê cung với kích thước lẻ.
        /// </summary>
        /// <param name="width">Chiều rộng (lẻ).</param>
        /// <param name="height">Chiều cao (lẻ).</param>
        public MazeGenerator(int width, int height)
        {
            // Đảm bảo kích thước là số lẻ (Maze phải có viền tường)
            Width = (width % 2 == 0) ? width + 1 : width;
            Height = (height % 2 == 0) ? height + 1 : height;
            Maze = new int[Height, Width];
        }

        /// <summary>
        /// Tạo mê cung bằng thuật toán Recursive Backtracking.
        /// </summary>
        public void GenerateMaze()
        {
            // 1. Khởi tạo tất cả là Tường
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Maze[i, j] = WALL;
                }
            }

            // 2. Chọn điểm bắt đầu (luôn là ô lẻ) và bắt đầu đệ quy
            int startX = 1;
            int startY = 1;
            StartGridPosition = new Point(startX, startY);

            // Mở ô sàn tại vị trí bắt đầu
            Maze[startY, startX] = FLOOR;

            // Bắt đầu đệ quy
            RecursiveBacktrack(startX, startY);

            // 3. Thiết lập Điểm Bắt đầu và Kết thúc (Ở các góc)
            // Đảm bảo điểm bắt đầu và kết thúc là sàn và cách xa nhau

            // Điểm bắt đầu (đã được đặt ở (1, 1))
            StartGridPosition = new Point(1, 1);

            // Điểm kết thúc: cố gắng đặt ở góc đối diện (Gần (Width-2, Height-2))
            EndGridPosition = FindValidFarFloorPosition(StartGridPosition);
            Maze[EndGridPosition.Y, EndGridPosition.X] = (int)GameObjectType.Exit;
        }

        /// <summary>
        /// Tìm một vị trí sàn hợp lệ xa vị trí bắt đầu nhất có thể.
        /// </summary>
        private Point FindValidFarFloorPosition(Point start)
        {
            Point bestPos = new Point(Width - 2, Height - 2);
            double maxDist = 0;

            for (int y = Height - 2; y >= 1; y -= 2)
            {
                for (int x = Width - 2; x >= 1; x -= 2)
                {
                    if (Maze[y, x] == FLOOR)
                    {
                        double dist = Math.Sqrt(Math.Pow(x - start.X, 2) + Math.Pow(y - start.Y, 2));
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            bestPos = new Point(x, y);
                        }
                    }
                }
            }

            // Đảm bảo vị trí kết thúc là Sàn
            Maze[bestPos.Y, bestPos.X] = FLOOR;
            return bestPos;
        }


        /// <summary>
        /// Thuật toán đệ quy Backtracking để tạo đường đi.
        /// </summary>
        private void RecursiveBacktrack(int x, int y)
        {
            int[] directions = { 0, 1, 2, 3 }; // 0: Lên, 1: Xuống, 2: Trái, 3: Phải
            Shuffle(directions);

            foreach (int direction in directions)
            {
                int nx = x;
                int ny = y;
                int wx = x; // Tọa độ tường (Wall) cần phá
                int wy = y;

                switch (direction)
                {
                    case 0: // Lên
                        ny -= 2;
                        wy -= 1;
                        break;
                    case 1: // Xuống
                        ny += 2;
                        wy += 1;
                        break;
                    case 2: // Trái
                        nx -= 2;
                        wx -= 1;
                        break;
                    case 3: // Phải
                        nx += 2;
                        wx += 1;
                        break;
                }

                // Kiểm tra biên
                if (ny >= 0 && ny < Height && nx >= 0 && nx < Width)
                {
                    // Nếu ô đích là Tường (chưa được thăm)
                    if (Maze[ny, nx] == WALL)
                    {
                        // Phá tường giữa hai ô
                        Maze[wy, wx] = FLOOR;
                        // Đặt ô đích là Sàn
                        Maze[ny, nx] = FLOOR;

                        // Đệ quy
                        RecursiveBacktrack(nx, ny);
                    }
                }
            }
        }

        /// <summary>
        /// Xáo trộn mảng để chọn hướng ngẫu nhiên.
        /// </summary>
        private void Shuffle(int[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + random.Next(n - i);
                int temp = array[r];
                array[r] = array[i];
                array[i] = temp;
            }
        }
    }
}

