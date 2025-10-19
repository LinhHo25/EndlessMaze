using System;
using System.Drawing;
using System.Windows.Forms;

namespace EndlessMaze.Tri
{
    // Lớp quản lý cấu trúc và hiển thị mê cung
    public class Maze
    {
        public int[,] MazeData { get; set; }
        public int TileSize { get; private set; }
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        private Brush wallBrush = new SolidBrush(Color.FromArgb(50, 50, 50)); // Màu Tường
        private Brush floorBrush = new SolidBrush(Color.FromArgb(24, 24, 24)); // Màu Sàn
        private Brush endBrush = new SolidBrush(Color.Gold); // Màu Đích

        public Maze(int rows, int cols, int tileSize)
        {
            this.Rows = rows;
            this.Cols = cols;
            this.TileSize = tileSize;
            // Khởi tạo mảng, sử dụng kích thước đã được điều chỉnh ở MazeGenerator
            this.MazeData = new int[rows, cols];
        }

        /// <summary>
        /// Kiểm tra xem một hitbox có va chạm với bất kỳ ô tường nào không.
        /// Sử dụng va chạm hình chữ nhật (Rectangle-based).
        /// </summary>
        public bool CheckWallCollision(Rectangle bounds)
        {
            if (MazeData == null) return false;

            // Xác định phạm vi các ô mà hitbox che phủ
            int startCol = bounds.Left / TileSize;
            int endCol = (bounds.Right - 1) / TileSize; // Trừ 1 để tránh lỗi lấy ô bên phải khi boundary chạm đúng mép
            int startRow = bounds.Top / TileSize;
            int endRow = (bounds.Bottom - 1) / TileSize; // Trừ 1 tương tự

            // Giới hạn phạm vi kiểm tra trong mảng
            startCol = Math.Max(0, startCol);
            endCol = Math.Min(Cols - 1, endCol);
            startRow = Math.Max(0, startRow);
            endRow = Math.Min(Rows - 1, endRow);

            // Kiểm tra từng ô trong phạm vi
            for (int r = startRow; r <= endRow; r++)
            {
                for (int c = startCol; c <= endCol; c++)
                {
                    // Đảm bảo không truy cập ra ngoài biên mảng
                    if (r >= Rows || c >= Cols) continue;

                    // Kiểm tra xem ô đó có phải là Tường (1) không
                    if (MazeData[r, c] == 1)
                    {
                        // Tạo hitbox cho ô tường đó
                        Rectangle wallBounds = new Rectangle(c * TileSize, r * TileSize, TileSize, TileSize);

                        // Kiểm tra va chạm giữa hitbox nhân vật và hitbox tường
                        if (bounds.IntersectsWith(wallBounds))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Vẽ toàn bộ mê cung và cổng kết thúc lên Graphics object.
        /// </summary>
        public void Draw(Graphics g, Point endPosition)
        {
            if (MazeData == null) return;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    // Tính toán vị trí ô
                    int x = c * TileSize;
                    int y = r * TileSize;
                    Rectangle tileRect = new Rectangle(x, y, TileSize, TileSize);

                    if (MazeData[r, c] == 1)
                    {
                        // Vẽ Tường
                        g.FillRectangle(wallBrush, tileRect);
                    }
                    else
                    {
                        // Vẽ Sàn
                        g.FillRectangle(floorBrush, tileRect);
                    }
                }
            }

            // Vẽ Cổng kết thúc (vị trí endPosition là góc trên bên trái của ô)
            g.FillEllipse(endBrush, new Rectangle(endPosition.X, endPosition.Y, TileSize, TileSize));
            g.DrawString("END", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, endPosition.X + 5, endPosition.Y + 10);
        }
    }
}
