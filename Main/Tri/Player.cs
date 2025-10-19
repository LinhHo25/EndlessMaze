using System.Drawing;
using System.Windows.Forms;

namespace Main.Tri // Đã cập nhật namespace
{
    public class Player : GameObject
    {
        // Trạng thái nhân vật
        private const float BASE_SPEED = 4.0f;
        private const float DASH_SPEED = 18.0f;
        private const int DASH_DURATION = 8; // Số frame lướt
        private const float MAX_STAMINA = 100f;
        private const float DASH_COST = 40f;
        private const float STAMINA_REGEN = 0.5f;

        public float MoveSpeed { get; private set; }
        public float Stamina { get; private set; }
        private int dashTimer = 0; // Đếm ngược thời gian lướt
        public bool IsDashing => dashTimer > 0;

        // Trạng thái input
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool AttemptDash { get; set; }

        // Hướng di chuyển cuối cùng (để biết hướng lướt)
        private PointF lastMoveDirection = new PointF(0, 0);

        public Player(float x, float y, int size) :
            base(x, y, size, size, GameObjectType.Player)
        {
            Stamina = MAX_STAMINA;
            MoveSpeed = BASE_SPEED;
        }

        /// <summary>
        /// Cập nhật logic di chuyển và lướt.
        /// </summary>
        public override void Update()
        {
            // 1. Xử lý trạng thái Lướt (Dash)
            if (IsDashing)
            {
                MoveSpeed = DASH_SPEED;
                dashTimer--;
                if (dashTimer <= 0)
                {
                    // Kết thúc lướt
                    MoveSpeed = BASE_SPEED;
                }
            }
            else
            {
                // 2. Phục hồi thể lực (Stamina Regen)
                if (Stamina < MAX_STAMINA)
                {
                    Stamina += STAMINA_REGEN;
                    if (Stamina > MAX_STAMINA) Stamina = MAX_STAMINA;
                }

                // 3. Xử lý cố gắng Lướt
                if (AttemptDash && Stamina >= DASH_COST)
                {
                    // Bắt đầu lướt nếu có đủ thể lực
                    Stamina -= DASH_COST;
                    dashTimer = DASH_DURATION;
                    // Tắt cờ Dash ngay sau khi kích hoạt để tránh lặp lại
                    AttemptDash = false;
                }
            }
        }

        /// <summary>
        /// Trả về vector di chuyển dựa trên input và trạng thái Dash.
        /// </summary>
        public PointF GetMovementVector()
        {
            float dx = 0, dy = 0;
            if (MoveUp) dy -= 1;
            if (MoveDown) dy += 1;
            if (MoveLeft) dx -= 1;
            if (MoveRight) dx += 1;

            // Nếu có input, chuẩn hóa và lưu hướng mới
            if (dx != 0 || dy != 0)
            {
                float length = (float)System.Math.Sqrt(dx * dx + dy * dy);
                dx /= length;
                dy /= length;
                lastMoveDirection = new PointF(dx, dy);
            }
            // Nếu không có input nhưng đang Dash, dùng hướng cuối cùng
            else if (IsDashing)
            {
                dx = lastMoveDirection.X;
                dy = lastMoveDirection.Y;
            }

            return new PointF(dx * MoveSpeed, dy * MoveSpeed);
        }

        /// <summary>
        /// Vẽ nhân vật và thanh thể lực.
        /// </summary>
        public override void Draw(Graphics g)
        {
            // Vẽ hình chữ nhật của Player
            Color playerColor = IsDashing ? Color.Cyan : Color.Blue;
            using (SolidBrush brush = new SolidBrush(playerColor))
            {
                g.FillEllipse(brush, X, Y, Width, Height);
            }

            // Vẽ Thanh Thể Lực (Stamina Bar) ngay trên đầu nhân vật
            const int barWidth = 32;
            const int barHeight = 4;
            float barX = X;
            float barY = Y - barHeight - 2;
            float staminaRatio = Stamina / MAX_STAMINA;

            // Vẽ nền thanh thể lực
            g.FillRectangle(Brushes.Gray, barX, barY, barWidth, barHeight);

            // Vẽ thể lực hiện tại
            g.FillRectangle(Brushes.LimeGreen, barX, barY, barWidth * staminaRatio, barHeight);
        }
    }
}
