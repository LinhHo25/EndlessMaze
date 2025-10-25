using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace Main.Tri
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Dashing,
        Attacking,
        Blocking,
        Fallen
    }

    public class Player : GameObject
    {
        // ... (Các thuộc tính MaxHealth, CurrentHealth... giữ nguyên) ...
        public float MaxHealth { get; set; } = 100;
        public float CurrentHealth { get; set; } = 100;
        public float MaxStamina { get { return 100f; } }
        public float CurrentStamina { get { return this.Stamina; } set { this.Stamina = value; } }
        public bool IsPoisoned { get; set; } = false;
        public int DashCount { get; set; } = 0;

        public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
        public bool IsFallen { get { return CurrentState == PlayerState.Fallen; } set { if (value) CurrentState = PlayerState.Fallen; else if (CurrentState == PlayerState.Fallen) CurrentState = PlayerState.Idle; } }
        public bool IsDashing { get { return CurrentState == PlayerState.Dashing; } }
        public bool IsAttacking { get { return CurrentState == PlayerState.Attacking; } }
        public bool IsBlocking { get; private set; } = false;

        // --- THÊM MỚI: TÚI ĐỒ VÀ HIỆU ỨNG ---
        public Dictionary<ItemType, int> Inventory { get; private set; } = new Dictionary<ItemType, int>();
        public Dictionary<BuffType, Buff> ActiveBuffs { get; private set; } = new Dictionary<BuffType, Buff>();
        // ------------------------------------

        // ... (Các hằng số const, MoveSpeed, Stamina... giữ nguyên) ...
        private const float BASE_SPEED = 4.0f;
        private const float DASH_SPEED = 18.0f;
        private const int DASH_DURATION_FRAMES = 8;
        private const float MAX_STAMINA = 100f;
        private const float DASH_COST = 40f;
        private const float STAMINA_REGEN = 0.5f;

        public float MoveSpeed { get; private set; }
        public float Stamina { get; private set; }
        private int actionTimer = 0;

        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public bool AttemptDash { get; set; }
        public bool AttemptAttack { get; set; }
        public bool AttemptBlock { get; set; }

        private string lastFacingDirection = "down";

        // ... (Các biến Animation giữ nguyên) ...
        private Image currentSprite;
        private int animationStep = 0;
        private int animationSlowDown = 0;
        private const int ANIMATION_SPEED = 10;
        private string currentAnimationName = "stand_down";

        private Dictionary<string, List<Image>> animations = new Dictionary<string, List<Image>>();

        public Player(float x, float y, int size) :
            base(x, y, size, size, GameObjectType.Player)
        {
            Stamina = MAX_STAMINA;
            MoveSpeed = BASE_SPEED;

            // --- THÊM MỚI: Cho vật phẩm test lúc bắt đầu ---
            AddItem(ItemType.HealthPotion, 3);
            AddItem(ItemType.AttackPotion, 1);
            // -----------------------------------------
        }

        // ... (Hàm SetUpAnimations, LoadAnimationFrames, CreateFallbackSprite giữ nguyên) ...
        public void SetUpAnimations()
        {
            animations.Clear();

            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
                string playerSpritePath = Path.Combine(projectRoot, "player");
                string swordsmanSpritePath = Path.Combine(projectRoot, "Swordsman");

                if (!Directory.Exists(playerSpritePath))
                {
                    MessageBox.Show($"Lỗi: Không tìm thấy thư mục '{playerSpritePath}'.\nHãy tạo thư mục 'player' trong dự án và đặt ảnh đi bộ vào đó.", "Lỗi tải ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateFallbackSprite();
                    return;
                }
                if (!Directory.Exists(swordsmanSpritePath))
                {
                    MessageBox.Show($"Lỗi: Không tìm thấy thư mục '{swordsmanSpritePath}'.\nHãy tạo thư mục 'Swordsman' trong dự án và đặt ảnh đứng yên/tấn công/đỡ vào đó.", "Lỗi tải ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateFallbackSprite();
                    return;
                }

                LoadAnimationFrames("walk_up", playerSpritePath, "*Back*.png");
                LoadAnimationFrames("walk_down", playerSpritePath, "*Front*.png");
                LoadAnimationFrames("walk_left", playerSpritePath, "*Left*.png");
                LoadAnimationFrames("walk_right", playerSpritePath, "*Right*.png");

                LoadAnimationFrames("stand_up", swordsmanSpritePath, "*Stand_Back*.png");
                LoadAnimationFrames("stand_down", swordsmanSpritePath, "*Stand_Font*.png");
                LoadAnimationFrames("stand_left", swordsmanSpritePath, "*Stand_Left*.png");
                LoadAnimationFrames("stand_right", swordsmanSpritePath, "*Stand_Right*.png");

                LoadAnimationFrames("attack_up", swordsmanSpritePath, "*Attack_Back*.png");
                LoadAnimationFrames("attack_down", swordsmanSpritePath, "*Attack_Font*.png");
                LoadAnimationFrames("attack_left", swordsmanSpritePath, "*Attack_Left*.png");
                LoadAnimationFrames("attack_right", swordsmanSpritePath, "*Attack_Right*.png");

                LoadAnimationFrames("block_up", swordsmanSpritePath, "*Block_Back*.png");
                LoadAnimationFrames("block_down", swordsmanSpritePath, "*Block_Font*.png");
                LoadAnimationFrames("block_left", swordsmanSpritePath, "*Block_Left*.png");
                LoadAnimationFrames("block_right", swordsmanSpritePath, "*Block_Right*.png");


                if (animations.ContainsKey("stand_down") && animations["stand_down"].Count > 0)
                {
                    currentSprite = animations["stand_down"][0];
                }
                else
                {
                    MessageBox.Show("Cảnh báo: Không tìm thấy ảnh đứng yên ('stand_down').", "Thiếu ảnh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (animations.Count > 0 && animations.First().Value.Count > 0)
                        currentSprite = animations.First().Value.First();
                    else
                        CreateFallbackSprite();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải ảnh (Kiểm tra thư mục 'player' và 'Swordsman'): " + ex.Message, "Lỗi tải ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CreateFallbackSprite();
            }
        }
        private void LoadAnimationFrames(string animationName, string folderPath, string searchPattern)
        {
            try
            {
                var files = Directory.GetFiles(folderPath, searchPattern)
                                     .OrderBy(f => f)
                                     .Select(f => Image.FromFile(f))
                                     .ToList();
                if (files.Count > 0)
                {
                    animations[animationName] = files;
                }
                else
                {
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải ảnh cho '{animationName}': {ex.Message}");
            }
        }
        private void CreateFallbackSprite()
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(Color.Red); }
            currentSprite = bmp;
            animations["fallback"] = new List<Image> { currentSprite };
            currentAnimationName = "fallback";
        }


        // HÀM UPDATE CHÍNH (ĐÃ CẬP NHẬT)
        public override void Update()
        {
            // --- THÊM MỚI: Xử lý Buffs ---
            UpdateBuffs();
            // -----------------------------

            if (CurrentState == PlayerState.Fallen)
            {
                UpdateAnimationState();
                return;
            }

            if (actionTimer > 0)
            {
                actionTimer--;
                if (actionTimer <= 0)
                {
                    if (CurrentState == PlayerState.Dashing)
                    {
                        CurrentState = PlayerState.Idle;
                        MoveSpeed = BASE_SPEED;
                    }
                    else if (CurrentState == PlayerState.Attacking)
                    {
                        CurrentState = PlayerState.Idle;
                    }
                }
            }

            if (actionTimer <= 0 || CurrentState == PlayerState.Idle || CurrentState == PlayerState.Walking)
            {
                if (AttemptBlock)
                {
                    IsBlocking = true;
                }
                else
                {
                    IsBlocking = false;
                }

                if (AttemptAttack && !IsBlocking)
                {
                    CurrentState = PlayerState.Attacking;
                    actionTimer = 15;
                    AttemptAttack = false;
                }
                else if (AttemptDash && Stamina >= DASH_COST && !IsBlocking)
                {
                    Stamina -= DASH_COST;
                    CurrentState = PlayerState.Dashing;
                    MoveSpeed = DASH_SPEED;
                    actionTimer = DASH_DURATION_FRAMES;
                    DashCount++;
                    AttemptDash = false;
                }
            }
            if (CurrentState == PlayerState.Attacking) AttemptAttack = false;
            if (CurrentState == PlayerState.Dashing) AttemptDash = false;


            if (CurrentState != PlayerState.Dashing && CurrentState != PlayerState.Attacking && !IsBlocking)
            {
                if (Stamina < MAX_STAMINA)
                {
                    Stamina += STAMINA_REGEN;
                    if (Stamina > MAX_STAMINA) Stamina = MAX_STAMINA;
                }
            }

            if (CurrentState != PlayerState.Dashing && CurrentState != PlayerState.Attacking)
            {
                if (MoveLeft || MoveRight || MoveUp || MoveDown)
                {
                    CurrentState = PlayerState.Walking;
                }
                else
                {
                    CurrentState = PlayerState.Idle;
                }
            }

            UpdateAnimationState();
        }

        // ... (Hàm CalculateMovementVector giữ nguyên) ...
        public PointF CalculateMovementVector(PointF currentPosition)
        {
            float dx = 0, dy = 0;

            if (CurrentState == PlayerState.Attacking || CurrentState == PlayerState.Fallen)
            {
                return PointF.Empty;
            }

            if (MoveUp) dy -= 1;
            if (MoveDown) dy += 1;
            if (MoveLeft) dx -= 1;
            if (MoveRight) dx += 1;

            if (dx > 0) lastFacingDirection = "right";
            else if (dx < 0) lastFacingDirection = "left";
            else if (dy > 0) lastFacingDirection = "down";
            else if (dy < 0) lastFacingDirection = "up";

            if (CurrentState == PlayerState.Dashing)
            {
                if (lastFacingDirection == "left") dx = -1;
                else if (lastFacingDirection == "right") dx = 1;
                else dx = 0;

                if (lastFacingDirection == "up") dy = -1;
                else if (lastFacingDirection == "down") dy = 1;
                else dy = 0;
            }

            if (dx != 0 && dy != 0)
            {
                float length = (float)System.Math.Sqrt(dx * dx + dy * dy);
                dx /= length;
                dy /= length;
            }

            float currentSpeed = (CurrentState == PlayerState.Dashing) ? DASH_SPEED : BASE_SPEED;

            // --- CẬP NHẬT: Kiểm tra Buff Phòng ngự ---
            if (IsBlocking || ActiveBuffs.ContainsKey(BuffType.Defense))
            {
                // Nếu đang đỡ đòn HOẶC có buff phòng ngự, giảm tốc độ
                currentSpeed *= 0.5f;
            }

            return new PointF(dx * currentSpeed, dy * currentSpeed);
        }

        // ... (Hàm Dash, Attack, Block, StopDash giữ nguyên) ...
        public void Dash()
        {
            if (CurrentState != PlayerState.Dashing && CurrentState != PlayerState.Attacking && CurrentState != PlayerState.Fallen)
            {
                AttemptDash = true;
            }
        }
        public void Attack()
        {
            if (CurrentState != PlayerState.Dashing && CurrentState != PlayerState.Attacking && CurrentState != PlayerState.Fallen)
            {
                AttemptAttack = true;
            }
        }
        public void Block(bool isBlocking)
        {
            if (CurrentState != PlayerState.Dashing && CurrentState != PlayerState.Attacking && CurrentState != PlayerState.Fallen)
            {
                AttemptBlock = isBlocking;
            }
            else
            {
                AttemptBlock = false;
            }
        }
        public void StopDash()
        {
            if (CurrentState == PlayerState.Dashing)
            {
                actionTimer = 0;
                CurrentState = PlayerState.Idle;
                MoveSpeed = BASE_SPEED;
            }
        }

        // --- CÁC HÀM MỚI VỀ VẬT PHẨM ---

        // Thêm vật phẩm vào túi
        public void AddItem(ItemType itemType, int quantity = 1)
        {
            if (Inventory.ContainsKey(itemType))
            {
                Inventory[itemType] += quantity;
            }
            else
            {
                Inventory[itemType] = quantity;
            }
            // TODO: Thông báo nhặt được đồ
        }

        // Sử dụng vật phẩm
        public void UseItem(ItemType itemType)
        {
            if (Inventory.ContainsKey(itemType) && Inventory[itemType] > 0)
            {
                Inventory[itemType]--; // Giảm số lượng
                if (Inventory[itemType] <= 0)
                {
                    Inventory.Remove(itemType); // Xóa khỏi túi nếu hết
                }

                // Kích hoạt hiệu ứng
                switch (itemType)
                {
                    case ItemType.HealthPotion:
                        CurrentHealth = Math.Min(CurrentHealth + 50, MaxHealth); // Hồi 50 máu
                        break;
                    case ItemType.AttackPotion:
                        AddBuff(BuffType.Attack, 30, 1.5f); // Tăng 50% sát thương trong 30 giây
                        break;
                    case ItemType.DefensePotion:
                        AddBuff(BuffType.Defense, 30, 0.5f); // Giảm 50% sát thương nhận vào trong 30 giây
                        break;
                    case ItemType.PoisonResistPotion:
                        AddBuff(BuffType.PoisonResist, 60, 0); // Kháng độc 60 giây
                        break;
                    case ItemType.CoolingWater:
                        AddBuff(BuffType.Cooling, 60, 0); // Giải nhiệt 60 giây
                        break;
                }
            }
        }

        // Thêm hoặc làm mới hiệu ứng
        private void AddBuff(BuffType type, int durationSeconds, float potency)
        {
            if (ActiveBuffs.ContainsKey(type))
            {
                // Làm mới thời gian
                ActiveBuffs[type].DurationFrames = durationSeconds * 60;
            }
            else
            {
                ActiveBuffs[type] = new Buff(type, durationSeconds, potency);
            }
        }

        // Cập nhật hiệu ứng (gọi mỗi frame trong Player.Update())
        private void UpdateBuffs()
        {
            // Xử lý kháng độc
            if (ActiveBuffs.ContainsKey(BuffType.PoisonResist))
            {
                IsPoisoned = false; // Luôn xóa độc nếu có buff
            }

            // Dùng ToList() để tránh lỗi "Collection was modified" khi xóa buff
            var keys = ActiveBuffs.Keys.ToList();
            foreach (var buffType in keys)
            {
                if (ActiveBuffs[buffType].Tick()) // Tick() đếm ngược và trả về true nếu hết hạn
                {
                    ActiveBuffs.Remove(buffType); // Xóa buff nếu hết hạn
                }
            }
        }

        // --- HÀM TÍNH SÁT THƯƠNG (VÍ DỤ) ---
        // (Bạn sẽ gọi hàm này từ Monster.Attack())
        public void TakeDamage(float damage)
        {
            float finalDamage = damage;

            // Giảm sát thương nếu đang đỡ
            if (IsBlocking)
            {
                finalDamage *= 0.25f; // Giảm 75% sát thương
            }
            // Giảm sát thương nếu có buff phòng ngự
            else if (ActiveBuffs.ContainsKey(BuffType.Defense))
            {
                finalDamage *= ActiveBuffs[BuffType.Defense].Potency; // ví dụ: 0.5f
            }

            CurrentHealth -= finalDamage;
            if (CurrentHealth < 0) CurrentHealth = 0;
            // TODO: Thêm logic chết
        }

        // (Bạn sẽ dùng hàm này khi Player tấn công)
        public float GetAttackDamage()
        {
            float baseDamage = 10; // Sát thương cơ bản
            if (ActiveBuffs.ContainsKey(BuffType.Attack))
            {
                baseDamage *= ActiveBuffs[BuffType.Attack].Potency; // ví dụ: 1.5f
            }
            return baseDamage;
        }

        // ... (Hàm UpdateAnimationState và Animate giữ nguyên) ...
        private void UpdateAnimationState()
        {
            string newAnimationName = "";

            switch (CurrentState)
            {
                case PlayerState.Walking:
                    newAnimationName = "walk_" + lastFacingDirection;
                    break;
                case PlayerState.Dashing:
                    newAnimationName = "walk_" + lastFacingDirection;
                    break;
                case PlayerState.Attacking:
                    newAnimationName = "attack_" + lastFacingDirection;
                    break;
                case PlayerState.Idle:
                case PlayerState.Blocking:
                default:
                    if (IsBlocking)
                    {
                        newAnimationName = "block_" + lastFacingDirection;
                    }
                    else if (CurrentState == PlayerState.Walking)
                    {
                        newAnimationName = "walk_" + lastFacingDirection;
                    }
                    else
                    {
                        newAnimationName = "stand_" + lastFacingDirection;
                    }
                    break;
                case PlayerState.Fallen:
                    newAnimationName = "stand_down";
                    break;

            }


            newAnimationName = newAnimationName.Replace("up", "up").Replace("down", "down").Replace("left", "left").Replace("right", "right");

            if (!animations.ContainsKey(newAnimationName) || animations[newAnimationName].Count == 0)
            {
                if (newAnimationName.StartsWith("block_") || newAnimationName.StartsWith("attack_"))
                {
                    newAnimationName = "stand_" + lastFacingDirection;
                    if (!animations.ContainsKey(newAnimationName) || animations[newAnimationName].Count == 0)
                    {
                        newAnimationName = "fallback";
                    }
                }
                else
                {
                    newAnimationName = "fallback";
                }

            }


            if (currentAnimationName != newAnimationName)
            {
                currentAnimationName = newAnimationName;
                animationStep = 0;
            }

            Animate();
        }
        private void Animate()
        {
            if (!animations.ContainsKey(currentAnimationName) || animations[currentAnimationName].Count == 0)
            {
                if (!animations.ContainsKey("fallback")) CreateFallbackSprite();
                currentSprite = animations["fallback"][0];
                return;
            }

            List<Image> currentFrames = animations[currentAnimationName];

            animationSlowDown++;
            if (animationSlowDown > ANIMATION_SPEED)
            {
                animationSlowDown = 0;
                animationStep++;
                if (animationStep >= currentFrames.Count)
                {
                    if (CurrentState == PlayerState.Attacking)
                    {
                        animationStep = currentFrames.Count - 1;
                    }
                    else
                    {
                        animationStep = 0;
                    }
                }
                currentSprite = currentFrames[animationStep];
            }
        }


        // HÀM VẼ (ĐÃ CẬP NHẬT ĐỂ VẼ HIỆU ỨNG BUFF)
        public override void Draw(Graphics g)
        {
            // ... (Phần vẽ currentSprite, thanh Máu/Stamina, hiệu ứng Độc giữ nguyên) ...
            if (currentSprite != null)
            {
                float drawWidth = this.Width; // * 1.0f; // Sửa lại kích thước 1:1
                float drawHeight = this.Height; // * 1.0f;
                float drawX = X - (drawWidth - this.Width) / 2;
                float drawY = Y - (drawHeight - this.Height) / 2;

                g.DrawImage(currentSprite, drawX, drawY, drawWidth, drawHeight);
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(Color.Blue))
                {
                    g.FillEllipse(brush, X, Y, Width, Height);
                }
            }
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
            float staminaRatio = CurrentStamina / MaxStamina;
            float healthRatio = CurrentHealth / MaxHealth;
            g.FillRectangle(Brushes.LimeGreen, barX, barY_Stamina, barWidth * staminaRatio, barHeight);
            g.FillRectangle(Brushes.Red, barX, barY_Health, barWidth * healthRatio, barHeight);
            if (IsPoisoned)
            {
                using (Pen poisonPen = new Pen(Color.FromArgb(150, Color.Purple), 2))
                {
                    g.DrawEllipse(poisonPen, X - 1, Y - 1, Width + 2, Height + 2);
                }
            }

            // --- THÊM MỚI: Vẽ Hiệu ứng Buff ---
            if (ActiveBuffs.ContainsKey(BuffType.Attack))
            {
                // Vẽ vòng tròn màu Cam (Tấn công) dưới chân
                using (Pen attackPen = new Pen(Color.FromArgb(150, Color.OrangeRed), 3))
                {
                    g.DrawEllipse(attackPen, X, Y + Height - 5, Width, 8);
                }
            }
            if (ActiveBuffs.ContainsKey(BuffType.Defense) || IsBlocking)
            {
                // Vẽ vòng tròn màu Xám (Phòng ngự) dưới chân
                using (Pen defensePen = new Pen(Color.FromArgb(150, Color.LightGray), 3))
                {
                    g.DrawEllipse(defensePen, X + 2, Y + Height - 7, Width - 4, 8);
                }
            }
            // ----------------------------------
        }
    }
}

