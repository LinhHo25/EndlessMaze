using System.Drawing;

namespace Main.Tri
{
    /// <summary>
    /// Định nghĩa các loại đối tượng trong game.
    /// </summary>
    public enum GameObjectType
    {
        Wall = 1,
        Floor = 0,
        Player = 2,
        Exit = 3,
        Monster = 4,
        Item = 5,    // <-- THÊM DÒNG NÀY
        TileSize = 32
    }

    /// <summary>
    /// Lớp cơ sở cho tất cả các đối tượng có thể tương tác trong game.
    /// </summary>
    public class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public GameObjectType Type { get; protected set; }

        // --- THUỘC TÍNH THÊM VÀO ĐỂ TƯƠNG THÍCH ---
        public PointF Position
        {
            get { return new PointF(X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public RectangleF BoundingBox
        {
            get { return GetBounds(); }
        }
        // ------------------------------------------

        public GameObject(float x, float y, int width, int height, GameObjectType type)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Type = type;
        }

        /// <summary>
        /// Trả về Rectangle đại diện cho vị trí và kích thước của đối tượng.
        /// </summary>
        public RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Width, Height);
        }

        /// <summary>
        /// Cập nhật logic của đối tượng (sẽ được override bởi Player, Monster...).
        /// </summary>
        public virtual void Update()
        {
            // Logic cập nhật cơ bản (nếu có)
        }

        /// <summary>
        /// Vẽ đối tượng (sẽ được override để vẽ hình ảnh cụ thể).
        /// </summary>
        public virtual void Draw(Graphics g)
        {
            // Mặc định, vẽ một hình chữ nhật màu dựa trên loại đối tượng
            Color color = Color.Magenta; // Màu mặc định nếu thiếu
            switch (Type)
            {
                case GameObjectType.Wall: color = Color.SaddleBrown; break;
                case GameObjectType.Player: color = Color.Blue; break;
                case GameObjectType.Exit: color = Color.Gold; break;
                case GameObjectType.Monster: color = Color.Red; break;
                case GameObjectType.Item: color = Color.Yellow; break; // Màu cho Item (có thể đổi)
            }
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, X, Y, Width, Height);
            }

            // Vẽ đường viền để dễ nhìn va chạm
            g.DrawRectangle(Pens.Black, X, Y, Width, Height);
        }
    }
}
