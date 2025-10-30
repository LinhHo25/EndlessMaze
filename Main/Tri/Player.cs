using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace Main.Tri
{
    // LƯU Ý: Enum PlayerState cũ đã bị XÓA
    // Logic mới sử dụng các biến boolean (isAttacking, isDashing...)

    public class Player : GameObject
    {
        // --- CÁC THUỘC TÍNH CŨ (Túi đồ, Máu, Buff) VẪN GIỮ NGUYÊN ---
        public float MaxHealth { get; set; } = 100;
        public float CurrentHealth { get; set; } = 100;
        public float MaxStamina { get { return 100f; } }
        public float CurrentStamina { get { return this.Stamina; } set { this.Stamina = value; } }
        public bool IsPoisoned { get; set; } = false;
        public int DashCount { get; set; } = 0; // Vẫn giữ cho map Water
        public Dictionary<ItemType, int> Inventory { get; private set; } = new Dictionary<ItemType, int>();
        public Dictionary<BuffType, Buff> ActiveBuffs { get; private set; } = new Dictionary<BuffType, Buff>();

        // --- CÁC THUỘC TÍNH INPUT (SẼ ĐƯỢC SET TỪ FORM MAP) ---
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool AttemptRun { get; set; } = false; // Input chạy (ControlKey)
        public bool AttemptAttackInput { get; set; } = false; // Input tấn công (Space)
        public bool AttemptDashInput { get; set; } = false; // Input lướt (Shift)
        public bool IsBlocking { get; set; } = false; // Input đỡ đòn (E) -> Logic này chưa được dùng trong Update mới

        // --- CÁC BIẾN LOGIC MỚI (TỪ FORM1.CS) ---
        private Image currentSprite;
        private string currentActivityState = "idle_down";
        private string lastFacingDirection = "down";

        // Tốc độ
        private const float playerSpeed = 6;
        private const float playerRunSpeed = 10;
        private const float dashSpeed = 25f; // Giảm tốc độ dash một chút

        // Kích thước vẽ (Thêm vào)
        private const float DRAW_SCALE = 2.0f; // Tỷ lệ vẽ so với hitbox

        // Các đối tượng quản lý hoạt ảnh
        private AnimationActivity walkActivity;
        private AnimationActivity idleActivity;
        private AnimationActivity runActivity;
        private AnimationActivity attackActivity;
        private AnimationActivity walkAttackActivity;
        private AnimationActivity runAttackActivity;
        private AnimationActivity hurtActivity;
        private AnimationActivity deathActivity;
        // (Bạn có thể thêm AnimationActivity cho 'Block' nếu muốn)

        // Biến trạng thái
        private bool isAttacking = false;
        private bool isHurt = false; // (Hàm TakeDamage có thể set = true)
        private bool isDead = false; // (Hàm TakeDamage có thể set = true)
        private bool isDashing = false;

        // Biến tính toán
        private int dashTimer = 0;
        private const int DASH_DURATION_FRAMES = 6; // Giảm thời gian lướt
        private int dashCooldown = 0;
        private const int DASH_COOLDOWN_FRAMES = 30; // Giảm cooldown
        private float dashDirectionX = 0;
        private float dashDirectionY = 0;
        private float calculatedMoveX = 0;
        private float calculatedMoveY = 0;

        // Biến stamina (lấy từ logic cũ)
        private const float MAX_STAMINA = 100f;
        private const float DASH_COST = 35f; // Giảm chi phí dash
        private const float STAMINA_REGEN = 0.8f; // Tăng hồi stamina
        public float Stamina { get; private set; }


        // --- CONSTRUCTOR (ĐÃ SỬA) ---
        public Player(float x, float y, int size) :
            base(x, y, size, size, GameObjectType.Player)
        {
            Stamina = MAX_STAMINA;
            AddItem(ItemType.HealthPotion, 3);
            AddItem(ItemType.AttackPotion, 1);

            // Gọi hàm SetUp mới
            SetUpAnimations();
        }

        // --- HÀM SET UP ANIMATION MỚI (TỪ FORM1.CS) ---
        public void SetUpAnimations()
        {
            // Lấy đường dẫn gốc của dự án
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));

            // SỬA ĐƯỜNG DẪN Ở ĐÂY: Thêm "SwordMan" vào đường dẫn Player
            string playerRootBase = Path.Combine(projectRoot, "ImgSource", "Char", "Player");
            string playerRoot = Path.Combine(playerRootBase, "SwordMan"); // Đường dẫn cụ thể tới SwordMan

            if (!Directory.Exists(playerRoot))
            {
                // Thông báo lỗi chỉ đường dẫn cụ thể hơn
                MessageBox.Show($"Lỗi: Không tìm thấy thư mục '{playerRoot}'.\nHãy chắc chắn bạn có thư mục 'ImgSource/Char/Player/SwordMan' trong project và đã thiết lập 'Copy to Output Directory'.", "Lỗi tải ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateFallbackSprite(); // Tạo sprite thay thế
                return; // Thoát khỏi hàm nếu không có thư mục gốc
            }

            try
            {
                // 1. Stand
                string idleRoot = Path.Combine(playerRoot, "Stand");
                idleActivity = new AnimationActivity(10);
                idleActivity.LoadImages(
                    Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"),
                    Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right")
                );

                // 2. Walk
                string walkRoot = Path.Combine(playerRoot, "Walk");
                walkActivity = new AnimationActivity(6);
                walkActivity.LoadImages(
                    Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"),
                    Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right")
                );

                // 3. Run
                string runRoot = Path.Combine(playerRoot, "Run");
                runActivity = new AnimationActivity(4);
                runActivity.LoadImages(
                    Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"),
                    Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right")
                );

                // 4. Attack (Stand)
                string attackRoot = Path.Combine(playerRoot, "Atk");
                attackActivity = new AnimationActivity(3);
                attackActivity.IsLooping = false;
                attackActivity.LoadImages(
                    Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"),
                    Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right")
                );

                // 5. Walk_Atk
                string walkAtkRoot = Path.Combine(playerRoot, "Walk_Atk");
                walkAttackActivity = new AnimationActivity(3);
                walkAttackActivity.IsLooping = false;
                walkAttackActivity.LoadImages(
                    Path.Combine(walkAtkRoot, "Back"), Path.Combine(walkAtkRoot, "Front"),
                    Path.Combine(walkAtkRoot, "Left"), Path.Combine(walkAtkRoot, "Right")
                );

                // 6. Run_Atk
                string runAtkRoot = Path.Combine(playerRoot, "Run_Atk");
                runAttackActivity = new AnimationActivity(3);
                runAttackActivity.IsLooping = false;
                runAttackActivity.LoadImages(
                    Path.Combine(runAtkRoot, "Back"), Path.Combine(runAtkRoot, "Front"),
                    Path.Combine(runAtkRoot, "Left"), Path.Combine(runAtkRoot, "Right")
                );

                // 7. Hurt
                string hurtRoot = Path.Combine(playerRoot, "Hurt");
                hurtActivity = new AnimationActivity(5);
                hurtActivity.IsLooping = false;
                hurtActivity.LoadImages(
                    Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"),
                    Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right")
                );

                // 8. Death
                string deathRoot = Path.Combine(playerRoot, "Death");
                deathActivity = new AnimationActivity(8);
                deathActivity.IsLooping = false;
                deathActivity.LoadImages(
                    Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"),
                    Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right")
                );

                // Kiểm tra lại idleActivity sau khi load
                if (idleActivity != null)
                {
                    currentSprite = idleActivity.GetDefaultFrame("down");
                    if (currentSprite == null)
                    {
                        // Sửa đường dẫn trong thông báo lỗi
                        MessageBox.Show("Không tìm thấy ảnh đứng yên mặc định (Front)! Hãy kiểm tra lại thư mục 'ImgSource/Char/Player/SwordMan/Stand/Front'.");
                        CreateFallbackSprite();
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi: Không thể khởi tạo idleActivity. Kiểm tra đường dẫn và file ảnh.");
                    CreateFallbackSprite();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải ảnh: " + ex.Message);
                CreateFallbackSprite(); // Tạo sprite thay thế nếu có lỗi
            }
        }

        // --- HÀM TẠO SPRITE THAY THẾ ---
        private void CreateFallbackSprite()
        {
            // Tạo 1 ảnh màu đỏ đơn giản nếu không tải được ảnh
            Bitmap bmp = new Bitmap(this.Width > 0 ? this.Width : 32, this.Height > 0 ? this.Height : 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Blue); // Vẽ màu xanh để dễ nhận biết
                using (Pen p = new Pen(Color.Yellow, 2))
                {
                    g.DrawRectangle(p, 0, 0, bmp.Width - 1, bmp.Height - 1);
                    g.DrawLine(p, 0, 0, bmp.Width, bmp.Height);
                    g.DrawLine(p, bmp.Width, 0, 0, bmp.Height);
                }
            }
            currentSprite = bmp;

            // Đảm bảo idleActivity không null để tránh lỗi sau này
            if (idleActivity == null) idleActivity = new AnimationActivity();
            if (walkActivity == null) walkActivity = new AnimationActivity();
            if (runActivity == null) runActivity = new AnimationActivity();
            if (attackActivity == null) { attackActivity = new AnimationActivity(); attackActivity.IsLooping = false; }
            if (walkAttackActivity == null) { walkAttackActivity = new AnimationActivity(); walkAttackActivity.IsLooping = false; }
            if (runAttackActivity == null) { runAttackActivity = new AnimationActivity(); runAttackActivity.IsLooping = false; }
            if (hurtActivity == null) { hurtActivity = new AnimationActivity(); hurtActivity.IsLooping = false; }
            if (deathActivity == null) { deathActivity = new AnimationActivity(); deathActivity.IsLooping = false; }
        }


        // --- HÀM UPDATE CHÍNH (LOGIC TỪ FORM1.CS) ---
        public override void Update()
        {
            // 0. Cập nhật Buffs (từ logic cũ)
            UpdateBuffs();

            // 1. QUẢN LÝ COOLDOWN & INPUT
            if (dashCooldown > 0) dashCooldown--;

            bool triggerDash = false;
            bool triggerAttack = false;

            // Chỉ nhận input hành động nếu không bị khóa (chết, bị thương, đang lướt, đang tấn công)
            if (!isDead && !isHurt && !isDashing && !isAttacking)
            {
                // Xử lý Input Dash (Lướt)
                if (AttemptDashInput && Stamina >= DASH_COST && dashCooldown <= 0)
                {
                    triggerDash = true; // Đánh dấu sẽ lướt
                }
                // Xử lý Input Attack (Tấn công)
                else if (AttemptAttackInput)
                {
                    triggerAttack = true; // Đánh dấu sẽ tấn công
                }
            }
            // Reset cờ input ngay lập tức sau khi kiểm tra
            AttemptDashInput = false;
            AttemptAttackInput = false; // Quan trọng: Reset ngay để không bị lặp lại ở frame sau

            // 2. XÁC ĐỊNH INPUT DI CHUYỂN
            float moveX = 0;
            float moveY = 0;
            bool isMovingInput = false;

            // Cho phép nhận input di chuyển ngay cả khi đang tấn công/lướt (để đổi hướng nhìn)
            // nhưng không cho di chuyển khi chết/bị thương
            if (!isDead && !isHurt)
            {
                if (MoveLeft) moveX = -1;
                if (MoveRight) moveX = 1;
                if (MoveUp) moveY = -1;
                if (MoveDown) moveY = 1;
                if (MoveLeft && MoveRight) moveX = 0;
                if (MoveUp && MoveDown) moveY = 0;

                if (moveX != 0 && moveY != 0)
                {
                    const float normalizationFactor = 0.7071f;
                    moveX *= normalizationFactor;
                    moveY *= normalizationFactor;
                }
            }
            isMovingInput = moveX != 0 || moveY != 0;


            // 3. CẬP NHẬT TRẠNG THÁI VÀ HOẠT ẢNH (STATE MACHINE)
            bool canMove = true;
            string newActivityState = "";
            string currentDirection = lastFacingDirection;

            // Cập nhật hướng nhìn dựa trên input di chuyển (ưu tiên hơn hướng nhìn cũ)
            if (isMovingInput)
            {
                if (moveX > 0) currentDirection = "right";
                else if (moveX < 0) currentDirection = "left";
                else if (moveY < 0) currentDirection = "up";
                else if (moveY > 0) currentDirection = "down";
            }
            // Nếu không di chuyển nhưng đang tấn công hoặc lướt, giữ hướng lướt/tấn công
            else if (isDashing)
            {
                if (dashDirectionX > 0) currentDirection = "right";
                else if (dashDirectionX < 0) currentDirection = "left";
                else if (dashDirectionY < 0) currentDirection = "up";
                else if (dashDirectionY > 0) currentDirection = "down";
            }
            // Nếu không làm gì cả, giữ hướng nhìn cuối cùng
            else
            {
                currentDirection = lastFacingDirection;
            }
            lastFacingDirection = currentDirection; // Lưu lại hướng nhìn cuối cùng


            // Xử lý State Machine theo thứ tự ưu tiên
            // ƯU TIÊN 1: CHẾT
            if (isDead)
            {
                canMove = false;
                newActivityState = "dead_" + currentDirection;
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; deathActivity?.ResetFrame(); }
                currentSprite = deathActivity?.GetNextFrame(currentDirection);
            }
            // ƯU TIÊN 2: BỊ THƯƠNG
            else if (isHurt)
            {
                canMove = false;
                newActivityState = "hurt_" + currentDirection;
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; hurtActivity?.ResetFrame(); }
                currentSprite = hurtActivity?.GetNextFrame(currentDirection);
                if (hurtActivity != null && hurtActivity.IsFinished) { isHurt = false; }
            }
            // ƯU TIÊN 3: LƯỚT (DASHING) - Kiểm tra triggerDash trước
            else if (triggerDash)
            {
                Stamina -= DASH_COST;
                isDashing = true; // Kích hoạt trạng thái lướt
                dashTimer = DASH_DURATION_FRAMES;
                dashCooldown = DASH_COOLDOWN_FRAMES;
                DashCount++;

                // Xác định hướng lướt (ưu tiên hướng di chuyển nếu có)
                if (isMovingInput)
                {
                    dashDirectionX = moveX; // Đã chuẩn hóa
                    dashDirectionY = moveY; // Đã chuẩn hóa
                }
                else // Lướt theo hướng nhìn cuối cùng
                {
                    if (lastFacingDirection == "left") { dashDirectionX = -1; dashDirectionY = 0; }
                    else if (lastFacingDirection == "right") { dashDirectionX = 1; dashDirectionY = 0; }
                    else if (lastFacingDirection == "up") { dashDirectionX = 0; dashDirectionY = -1; }
                    else if (lastFacingDirection == "down") { dashDirectionX = 0; dashDirectionY = 1; }
                    else { dashDirectionX = 1; dashDirectionY = 0; } // Fallback
                }
                // Cập nhật hướng nhìn theo hướng lướt
                currentDirection = lastFacingDirection;

                canMove = false; // Không di chuyển bình thường khi lướt
                newActivityState = "run_" + currentDirection; // Dùng anim chạy khi lướt
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; runActivity?.ResetFrame(); }
                currentSprite = runActivity?.GetNextFrame(currentDirection);
            }
            else if (isDashing) // Nếu đang trong trạng thái lướt (tiếp tục từ frame trước)
            {
                canMove = false;
                // Hướng nhìn đã được set khi bắt đầu lướt
                newActivityState = "run_" + currentDirection;
                // Không cần reset frame ở đây
                currentSprite = runActivity?.GetNextFrame(currentDirection);
                dashTimer--;
                if (dashTimer <= 0) { isDashing = false; } // Kết thúc lướt
            }
            // ƯU TIÊN 4: ĐỠ ĐÒN (Blocking)
            else if (IsBlocking)
            {
                canMove = true; // Cho phép di chuyển chậm
                newActivityState = "idle_" + currentDirection; // Tạm dùng idle anim
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; idleActivity?.ResetFrame(); }
                currentSprite = idleActivity?.GetNextFrame(currentDirection);
            }
            // ƯU TIÊN 5: TẤN CÔNG - Kiểm tra triggerAttack trước
            else if (triggerAttack)
            {
                isAttacking = true; // Kích hoạt trạng thái tấn công
                canMove = !(AttemptRun && isMovingInput); // Chỉ không di chuyển khi Đứng-Đánh

                if (AttemptRun && isMovingInput)
                { // Chạy-Đánh
                    newActivityState = "run_attack_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; runAttackActivity?.ResetFrame(); }
                    currentSprite = runAttackActivity?.GetNextFrame(currentDirection);
                }
                else if (isMovingInput)
                { // Đi-Đánh
                    newActivityState = "walk_attack_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; walkAttackActivity?.ResetFrame(); }
                    currentSprite = walkAttackActivity?.GetNextFrame(currentDirection);
                }
                else
                { // Đứng-Đánh
                    newActivityState = "attack_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; attackActivity?.ResetFrame(); }
                    currentSprite = attackActivity?.GetNextFrame(currentDirection);
                }
            }
            else if (isAttacking) // Nếu đang trong trạng thái tấn công (tiếp tục từ frame trước)
            {
                canMove = !(AttemptRun && isMovingInput); // Kiểm tra lại di chuyển
                AnimationActivity currentAttackAnim = attackActivity; // Anim mặc định
                if (AttemptRun && isMovingInput)
                {
                    newActivityState = "run_attack_" + currentDirection;
                    currentAttackAnim = runAttackActivity;
                }
                else if (isMovingInput)
                {
                    newActivityState = "walk_attack_" + currentDirection;
                    currentAttackAnim = walkAttackActivity;
                }
                else
                {
                    newActivityState = "attack_" + currentDirection;
                    currentAttackAnim = attackActivity;
                }
                // Không reset frame ở đây
                currentSprite = currentAttackAnim?.GetNextFrame(currentDirection);
                // Kiểm tra kết thúc animation
                if (currentAttackAnim != null && currentAttackAnim.IsFinished) { isAttacking = false; }
            }
            // ƯU TIÊN 6: DI CHUYỂN (Chạy hoặc Đi bộ)
            else if (isMovingInput)
            {
                canMove = true;
                if (AttemptRun) // Chạy
                {
                    newActivityState = "run_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; runActivity?.ResetFrame(); }
                    currentSprite = runActivity?.GetNextFrame(currentDirection);
                }
                else // Đi bộ
                {
                    newActivityState = "walk_" + currentDirection;
                    if (currentActivityState != newActivityState) { currentActivityState = newActivityState; walkActivity?.ResetFrame(); }
                    currentSprite = walkActivity?.GetNextFrame(currentDirection);
                }
            }
            // ƯU TIÊN 7: ĐỨNG YÊN
            else
            {
                canMove = false;
                newActivityState = "idle_" + currentDirection;
                if (currentActivityState != newActivityState) { currentActivityState = newActivityState; idleActivity?.ResetFrame(); }
                currentSprite = idleActivity?.GetNextFrame(currentDirection);
            }

            // --- Fallback cuối cùng ---
            if (currentSprite == null) { CreateFallbackSprite(); }

            // 4. HỒI STAMINA
            if (!isDashing && !isAttacking && !AttemptRun && !IsBlocking)
            {
                if (Stamina < MAX_STAMINA)
                {
                    Stamina += STAMINA_REGEN;
                    if (Stamina > MAX_STAMINA) Stamina = MAX_STAMINA;
                }
            }

            // 5. TÍNH TOÁN VECTOR DI CHUYỂN CUỐI CÙNG
            calculatedMoveX = 0;
            calculatedMoveY = 0;
            if (isDashing)
            {
                calculatedMoveX = dashDirectionX * dashSpeed;
                calculatedMoveY = dashDirectionY * dashSpeed;
            }
            else if (canMove) // Chỉ di chuyển nếu state cho phép
            {
                // Chỉ chạy nếu đang giữ phím chạy VÀ có input di chuyển VÀ không đang tấn công/đỡ
                bool isActuallyRunning = AttemptRun && isMovingInput && !isAttacking && !IsBlocking;
                float currentSpeed = isActuallyRunning ? playerRunSpeed : playerSpeed;
                // Giảm tốc khi đỡ đòn
                if (IsBlocking || ActiveBuffs.ContainsKey(BuffType.Defense))
                {
                    currentSpeed *= 0.5f;
                }
                calculatedMoveX = moveX * currentSpeed;
                calculatedMoveY = moveY * currentSpeed;
            }
        }

        // --- HÀM TÍNH TOÁN DI CHUYỂN (ĐÃ SỬA) ---
        // Hàm này giờ chỉ trả về vector đã tính toán trong Update()
        public PointF CalculateMovementVector(PointF currentPosition)
        {
            return new PointF(calculatedMoveX, calculatedMoveY);
        }

        // --- HÀM VẼ (ĐÃ SỬA) ---
        public override void Draw(Graphics g)
        {
            // 1. Vẽ nhân vật
            if (currentSprite != null)
            {
                // SỬA KÍCH THƯỚC VẼ: Dùng tỷ lệ thay vì cố định 100x100
                float drawWidth = this.Width * DRAW_SCALE; // Ví dụ: 28 * 2.0 = 56
                float drawHeight = this.Height * DRAW_SCALE; // Ví dụ: 28 * 2.0 = 56

                // Vẽ ảnh căn giữa hitbox (X, Y)
                float drawX = X - (drawWidth - this.Width) / 2;
                float drawY = Y - (drawHeight - this.Height) / 2; // Có thể trừ thêm để sprite cao hơn hitbox

                g.DrawImage(currentSprite, drawX, drawY, drawWidth, drawHeight);
            }
            else
            {
                // Nếu currentSprite vẫn null sau tất cả kiểm tra, vẽ fallback
                using (SolidBrush brush = new SolidBrush(Color.Blue)) // Màu xanh để dễ nhận biết
                {
                    // Vẽ tại vị trí hitbox
                    g.FillRectangle(brush, X, Y, Width, Height);
                    using (Pen p = new Pen(Color.Yellow, 2))
                    {
                        g.DrawRectangle(p, X, Y, Width - 1, Height - 1);
                        g.DrawLine(p, X, Y, X + Width, Y + Height);
                        g.DrawLine(p, X + Width, Y, X, Y + Height);
                    }
                }
            }

            // 2. Vẽ thanh Máu/Stamina (Logic cũ)
            const int barWidth = 32;
            const int barHeight = 4;
            float barX = X + (Width - barWidth) / 2;
            float barY_Stamina = Y - barHeight - 2;
            float barY_Health = Y - (barHeight * 2) - 4;
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(100, 50, 50, 50)))
            {
                g.FillRectangle(bgBrush, barX, barY_Stamina, barWidth, barHeight);
                g.FillRectangle(bgBrush, barX, barY_Health, barWidth, barHeight);
            }
            // Đảm bảo MaxStamina/MaxHealth không phải là 0 để tránh lỗi chia cho 0
            float staminaRatio = (MaxStamina > 0) ? (CurrentStamina / MaxStamina) : 0;
            float healthRatio = (MaxHealth > 0) ? (CurrentHealth / MaxHealth) : 0;
            // Đảm bảo tỷ lệ không âm
            staminaRatio = Math.Max(0, staminaRatio);
            healthRatio = Math.Max(0, healthRatio);

            g.FillRectangle(Brushes.LimeGreen, barX, barY_Stamina, barWidth * staminaRatio, barHeight);
            g.FillRectangle(Brushes.Red, barX, barY_Health, barWidth * healthRatio, barHeight);

            // 3. Vẽ hiệu ứng (Logic cũ)
            if (IsPoisoned)
            {
                using (Pen poisonPen = new Pen(Color.FromArgb(150, Color.Purple), 2))
                {
                    g.DrawEllipse(poisonPen, X - 1, Y - 1, Width + 2, Height + 2);
                }
            }
            if (ActiveBuffs.ContainsKey(BuffType.Attack))
            {
                using (Pen attackPen = new Pen(Color.FromArgb(150, Color.OrangeRed), 3))
                {
                    g.DrawEllipse(attackPen, X, Y + Height - 5, Width, 8);
                }
            }
            if (ActiveBuffs.ContainsKey(BuffType.Defense) || IsBlocking)
            {
                using (Pen defensePen = new Pen(Color.FromArgb(150, Color.LightGray), 3))
                {
                    g.DrawEllipse(defensePen, X + 2, Y + Height - 7, Width - 4, 8);
                }
            }
        }


        // --- CÁC HÀM LOGIC CŨ (GIỮ NGUYÊN) ---
        // (Bao gồm AddItem, UseItem, AddBuff, UpdateBuffs, TakeDamage, GetAttackDamage)

        public void AddItem(ItemType itemType, int quantity = 1) { /* ... */ if (Inventory.ContainsKey(itemType)) { Inventory[itemType] += quantity; } else { Inventory[itemType] = quantity; } }
        public void UseItem(ItemType itemType) { /* ... */ if (Inventory.ContainsKey(itemType) && Inventory[itemType] > 0) { Inventory[itemType]--; if (Inventory[itemType] <= 0) { Inventory.Remove(itemType); } switch (itemType) { case ItemType.HealthPotion: CurrentHealth = Math.Min(CurrentHealth + 50, MaxHealth); break; case ItemType.AttackPotion: AddBuff(BuffType.Attack, 30, 1.5f); break; case ItemType.DefensePotion: AddBuff(BuffType.Defense, 30, 0.5f); break; case ItemType.PoisonResistPotion: AddBuff(BuffType.PoisonResist, 60, 0); break; case ItemType.CoolingWater: AddBuff(BuffType.Cooling, 60, 0); break; } } }
        private void AddBuff(BuffType type, int durationSeconds, float potency) { /* ... */ if (ActiveBuffs.ContainsKey(type)) { ActiveBuffs[type].DurationFrames = durationSeconds * 60; } else { ActiveBuffs[type] = new Buff(type, durationSeconds, potency); } }
        private void UpdateBuffs() { /* ... */ if (ActiveBuffs.ContainsKey(BuffType.PoisonResist)) { IsPoisoned = false; } var keys = ActiveBuffs.Keys.ToList(); foreach (var buffType in keys) { if (ActiveBuffs.ContainsKey(buffType)) { if (ActiveBuffs[buffType].Tick()) { ActiveBuffs.Remove(buffType); } } } }
        public void TakeDamage(float damage) { /* ... */ if (!isHurt && !isDead && hurtActivity != null) { isHurt = true; hurtActivity.ResetFrame(); } float finalDamage = damage; if (IsBlocking) { finalDamage *= 0.25f; } else if (ActiveBuffs.ContainsKey(BuffType.Defense)) { finalDamage *= ActiveBuffs[BuffType.Defense].Potency; } CurrentHealth -= finalDamage; if (CurrentHealth <= 0 && !isDead) { CurrentHealth = 0; isDead = true; deathActivity?.ResetFrame(); } }
        public float GetAttackDamage() { /* ... */ float baseDamage = 10; if (ActiveBuffs.ContainsKey(BuffType.Attack)) { baseDamage *= ActiveBuffs[BuffType.Attack].Potency; } return baseDamage; }

        // Xóa các hàm logic di chuyển cũ
    }
}

