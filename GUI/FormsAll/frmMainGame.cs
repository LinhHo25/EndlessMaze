using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL.Models;
using Main;

namespace GUI.FormsAll
{
    public partial class frmMainGame : Form
    {
        private readonly PlayerCharacter _character;
        private readonly int _mapLevel;

        private Image playerImage;
        private float playerX = 100;
        private float playerY = 100;
        private int playerWidth = 40;
        private int playerHeight = 40;

        private AnimationActivity idleActivity;
        private AnimationActivity walkActivity;
        private AnimationActivity runActivity;
        private AnimationActivity attackActivity;
        private AnimationActivity walkAttackActivity;
        private AnimationActivity runAttackActivity;
        private AnimationActivity hurtActivity;
        private AnimationActivity deathActivity;

        private bool goUp, goDown, goLeft, goRight, isRunning, isAttacking, isHurt, isDead;
        private bool canMove = true;
        private string lastFacingDirection = "down";
        private Point mousePos;

        private int walkSpeed = 5;
        private int runSpeed = 8;
        private int dashSpeed = 25;
        private bool isDashing = false;
        private int dashTimer = 0;
        private int dashCooldown = 0;
        private float dashDirectionX, dashDirectionY;

        private int mapWidth;
        private int mapHeight;
        private Image floorTexture;
        private Image wallTexture;
        private int wallThickness = 50;
        private int gapYPosition = 100;
        private int gapSize = 150;

        private List<Monster> monsters = new List<Monster>();

        private List<SpellEffect> spellEffects = new List<SpellEffect>();

        private List<RectangleF> internalWalls = new List<RectangleF>();


        public frmMainGame()
        {
            InitializeComponent();
        }

        public frmMainGame(PlayerCharacter character, int mapLevel)
        {
            InitializeComponent();

            _character = character;
            _mapLevel = mapLevel;
            this.Text = $"Chơi - {_character.CharacterName} (Map: {_mapLevel})";

            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Cursor = Cursors.Cross;

            mapWidth = this.ClientSize.Width;
            mapHeight = this.ClientSize.Height;

            LoadCharacterAnimations();
            LoadMapTextures();

            InitializeInternalWalls();
            InitializeMonsters();

            gameTimer.Start();
        }

        private void LoadCharacterAnimations()
        {
            try
            {
                string playerRoot = Path.Combine("ImgSource", "Char", "Player", "SwordMan");

                string idleRoot = Path.Combine(playerRoot, "Stand");
                idleActivity = new AnimationActivity(10);
                idleActivity.LoadImages(Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"), Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right"));

                string walkRoot = Path.Combine(playerRoot, "Walk");
                walkActivity = new AnimationActivity(6);
                walkActivity.LoadImages(Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"), Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right"));

                string runRoot = Path.Combine(playerRoot, "Run");
                runActivity = new AnimationActivity(4);
                runActivity.LoadImages(Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"), Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right"));

                string attackRoot = Path.Combine(playerRoot, "Atk");
                attackActivity = new AnimationActivity(3) { IsLooping = false };
                attackActivity.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

                string walkAtkRoot = Path.Combine(playerRoot, "Walk_Atk");
                walkAttackActivity = new AnimationActivity(3) { IsLooping = false };
                walkAttackActivity.LoadImages(Path.Combine(walkAtkRoot, "Back"), Path.Combine(walkAtkRoot, "Front"), Path.Combine(walkAtkRoot, "Left"), Path.Combine(walkAtkRoot, "Right"));

                string runAtkRoot = Path.Combine(playerRoot, "Run_Atk");
                runAttackActivity = new AnimationActivity(3) { IsLooping = false };
                runAttackActivity.LoadImages(Path.Combine(runAtkRoot, "Back"), Path.Combine(runAtkRoot, "Front"), Path.Combine(runAtkRoot, "Left"), Path.Combine(runAtkRoot, "Right"));

                string hurtRoot = Path.Combine(playerRoot, "Hurt");
                hurtActivity = new AnimationActivity(5) { IsLooping = false };
                hurtActivity.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

                string deathRoot = Path.Combine(playerRoot, "Death");
                deathActivity = new AnimationActivity(8) { IsLooping = false };
                deathActivity.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));

                playerImage = idleActivity.GetDefaultFrame("down");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nghiêm trọng khi tải hoạt ảnh Player: " + ex.Message);
            }
        }

        private void LoadMapTextures()
        {
            try
            {
                floorTexture = Image.FromFile(Path.Combine("ImgSource", "Block", "default", "Floor", "catacombs_0.png"));
                wallTexture = Image.FromFile(Path.Combine("ImgSource", "Block", "default", "Wall", "Stone_Wall", "stone_brick_1.png"));

                if (wallTexture != null)
                {
                    wallThickness = wallTexture.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải texture map: " + ex.Message);
            }
        }

        private void InitializeInternalWalls()
        {
            int wt = wallThickness;
            int mapW = this.ClientSize.Width;
            int mapH = this.ClientSize.Height;

            internalWalls.Clear();

            for (int x = 100; x < mapW - wt; x += 100)
            {
                internalWalls.Add(new RectangleF(x, wt, wt, mapH - wt * 2));
            }

            for (int y = 100; y < mapH - wt; y += 100)
            {
                internalWalls.Add(new RectangleF(wt, y, mapW - wt * 2, wt));
            }

            RemoveWall(1, 1, 'N'); RemoveWall(0, 1, 'E');
            RemoveWall(1, 1, 'E'); RemoveWall(2, 1, 'W');
            RemoveWall(2, 1, 'S'); RemoveWall(2, 2, 'N');
            RemoveWall(2, 2, 'E'); RemoveWall(3, 2, 'W');
            RemoveWall(3, 2, 'S'); RemoveWall(3, 3, 'N');
            RemoveWall(3, 3, 'W'); RemoveWall(2, 3, 'E');
            RemoveWall(2, 3, 'S'); RemoveWall(2, 4, 'N');
            RemoveWall(2, 4, 'W'); RemoveWall(1, 4, 'E');
            RemoveWall(1, 4, 'S'); RemoveWall(1, 5, 'N');
            RemoveWall(1, 5, 'E'); RemoveWall(2, 5, 'W');
            RemoveWall(2, 5, 'E'); RemoveWall(3, 5, 'W');
            RemoveWall(3, 5, 'E'); RemoveWall(4, 5, 'W');
            RemoveWall(4, 5, 'N'); RemoveWall(4, 4, 'S');
            RemoveWall(4, 4, 'E'); RemoveWall(5, 4, 'W');
            RemoveWall(5, 4, 'S'); RemoveWall(5, 5, 'N');
            RemoveWall(5, 5, 'S'); RemoveWall(5, 6, 'N');
            RemoveWall(5, 6, 'W'); RemoveWall(4, 6, 'E');
            RemoveWall(4, 6, 'S'); RemoveWall(4, 7, 'N');
            RemoveWall(4, 7, 'E'); RemoveWall(5, 7, 'W');
            RemoveWall(5, 7, 'E'); RemoveWall(6, 7, 'W');
            RemoveWall(6, 7, 'N'); RemoveWall(6, 6, 'S');
            RemoveWall(6, 6, 'E'); RemoveWall(7, 6, 'W');
            RemoveWall(7, 6, 'N'); RemoveWall(7, 5, 'S');
            RemoveWall(7, 5, 'E'); RemoveWall(8, 5, 'W');
            RemoveWall(8, 5, 'S'); RemoveWall(8, 6, 'N');
            RemoveWall(8, 6, 'E'); RemoveWall(9, 6, 'W');
            RemoveWall(9, 6, 'E'); RemoveWall(10, 6, 'W');
            RemoveWall(10, 6, 'N'); RemoveWall(10, 5, 'S');
            RemoveWall(10, 5, 'N'); RemoveWall(10, 4, 'S');
            RemoveWall(10, 4, 'W'); RemoveWall(9, 4, 'E');
            RemoveWall(9, 4, 'N'); RemoveWall(9, 3, 'S');
            RemoveWall(9, 3, 'E'); RemoveWall(10, 3, 'W');
            RemoveWall(10, 3, 'E'); RemoveWall(11, 3, 'W');
            RemoveWall(11, 3, 'S'); RemoveWall(11, 4, 'N');
            RemoveWall(11, 4, 'E'); RemoveWall(12, 4, 'W');
            RemoveWall(12, 4, 'N'); RemoveWall(12, 3, 'S');
            RemoveWall(12, 3, 'N'); RemoveWall(12, 2, 'S');
            RemoveWall(12, 2, 'E'); RemoveWall(13, 2, 'W');
            RemoveWall(13, 2, 'S'); RemoveWall(13, 3, 'N');
            RemoveWall(13, 3, 'E'); RemoveWall(14, 3, 'W');
            RemoveWall(14, 3, 'N'); RemoveWall(14, 2, 'S');
            RemoveWall(14, 2, 'E'); RemoveWall(15, 2, 'W');
            RemoveWall(15, 2, 'N'); RemoveWall(15, 1, 'S');
            RemoveWall(15, 1, 'E'); RemoveWall(16, 1, 'W');
            RemoveWall(16, 1, 'N');

            RemoveWall(0, 3, 'E'); RemoveWall(1, 3, 'W');
            RemoveWall(1, 3, 'N'); RemoveWall(1, 2, 'S');
            RemoveWall(4, 0, 'S'); RemoveWall(4, 1, 'N');
            RemoveWall(4, 1, 'E'); RemoveWall(5, 1, 'W');
            RemoveWall(5, 1, 'S'); RemoveWall(5, 2, 'N');
            RemoveWall(8, 0, 'S'); RemoveWall(8, 1, 'N');
            RemoveWall(8, 1, 'W'); RemoveWall(7, 1, 'E');
            RemoveWall(7, 1, 'S'); RemoveWall(7, 2, 'N');
            RemoveWall(11, 0, 'S'); RemoveWall(11, 1, 'N');
            RemoveWall(11, 1, 'W'); RemoveWall(10, 1, 'E');
            RemoveWall(10, 1, 'S'); RemoveWall(10, 2, 'N');
            RemoveWall(14, 0, 'S'); RemoveWall(14, 1, 'N');
            RemoveWall(0, 6, 'E'); RemoveWall(1, 6, 'W');
            RemoveWall(0, 7, 'E'); RemoveWall(1, 7, 'W');
            RemoveWall(1, 7, 'N'); RemoveWall(1, 6, 'S');
            RemoveWall(2, 6, 'E'); RemoveWall(3, 6, 'W');
            RemoveWall(3, 6, 'N'); RemoveWall(3, 5, 'S');
            RemoveWall(7, 7, 'E'); RemoveWall(8, 7, 'W');
            RemoveWall(9, 7, 'E'); RemoveWall(10, 7, 'W');
            RemoveWall(11, 7, 'E'); RemoveWall(12, 7, 'W');
            RemoveWall(12, 7, 'S'); RemoveWall(12, 8, 'N');
            RemoveWall(14, 7, 'E'); RemoveWall(15, 7, 'W');
            RemoveWall(15, 7, 'S'); RemoveWall(15, 8, 'N');
            RemoveWall(16, 7, 'E'); RemoveWall(17, 7, 'W');
            RemoveWall(17, 7, 'S'); RemoveWall(17, 8, 'N');
            RemoveWall(16, 5, 'E'); RemoveWall(17, 5, 'W');
            RemoveWall(17, 5, 'N'); RemoveWall(17, 4, 'S');
            RemoveWall(15, 5, 'E'); RemoveWall(16, 5, 'W');
            RemoveWall(13, 5, 'E'); RemoveWall(14, 5, 'W');
            RemoveWall(14, 5, 'S'); RemoveWall(14, 6, 'N');
        }

        // --- SỬA LỖI: Hàm RemoveWall (để nó hoạt động) ---
        private void RemoveWall(int cellX, int cellY, char direction)
        {
            int wt = wallThickness;
            int x = (cellX * 100) + wt;
            int y = (cellY * 100) + wt;
            RectangleF wallToRemove;

            if (direction == 'N')
            {
                wallToRemove = new RectangleF(x, y - wt, 100, wt);
            }
            else if (direction == 'S')
            {
                wallToRemove = new RectangleF(x, y + 100, 100, wt);
            }
            else if (direction == 'W')
            {
                wallToRemove = new RectangleF(x - wt, y, wt, 100);
            }
            else
            {
                wallToRemove = new RectangleF(x + 100, y, wt, 100);
            }

            // SỬA: Dùng 'RemoveAll' với 'Equals' để so sánh struct
            internalWalls.RemoveAll(rect => rect.Equals(wallToRemove));
        }
        // --- HẾT PHẦN SỬA ---

        private void InitializeMonsters()
        {
            monsters.Add(new Slime(150, 250));
            monsters.Add(new Slime(50, 450));

            monsters.Add(new Orc(650, 500));
            monsters.Add(new Orc(950, 800));
            monsters.Add(new Orc(1400, 100));

            float bossX = mapWidth - wallThickness - 150;
            float bossY = gapYPosition + (gapSize / 2) - 50;
            monsters.Add(new Boss(bossX, bossY));
        }


        #region Input Events (Sự kiện Input)

        private void frmMainGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (isDead) return;

            if (e.KeyCode == Keys.W) goUp = true;
            if (e.KeyCode == Keys.S) goDown = true;
            if (e.KeyCode == Keys.A) goLeft = true;
            if (e.KeyCode == Keys.D) goRight = true;
            if (e.KeyCode == Keys.ControlKey) isRunning = true;

            if (e.KeyCode == Keys.ShiftKey && !isDashing && dashCooldown == 0)
            {
                isDashing = true;
                dashTimer = 10;
                dashCooldown = 60;
                GetDashDirection();
            }
        }

        private void frmMainGame_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) goUp = false;
            if (e.KeyCode == Keys.S) goDown = false;
            if (e.KeyCode == Keys.A) goLeft = false;
            if (e.KeyCode == Keys.D) goRight = false;
            if (e.KeyCode == Keys.ControlKey) isRunning = false;
        }

        private void frmMainGame_MouseDown(object sender, MouseEventArgs e)
        {
            if (isDead || isAttacking || isDashing) return;

            if (e.Button == MouseButtons.Left)
            {
                isAttacking = true;
                attackActivity.ResetFrame();
                walkAttackActivity.ResetFrame();
                runAttackActivity.ResetFrame();

                CheckPlayerAttackHit();
            }
        }

        private void frmMainGame_MouseMove(object sender, MouseEventArgs e)
        {
            mousePos = e.Location;
        }

        #endregion

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            playerImage?.Dispose();

            UpdateFacingDirection();
            UpdateDashState();
            UpdateMovement();
            UpdateAnimation();

            foreach (var monster in monsters.ToList())
            {
                monster.Update(this, playerX, playerY, spellEffects);

                if (monster.State == Monster.MonsterState.Dead && monster.deathAnim.IsFinished)
                {
                    monsters.Remove(monster);
                }
            }

            foreach (var spell in spellEffects.ToList())
            {
                spell.Update();
                if (spell.IsFinished)
                {
                    spellEffects.Remove(spell);
                }
            }

            this.Invalidate();
        }

        private void frmMainGame_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;

            DrawFloor(canvas);
            DrawWalls(canvas);

            foreach (var monster in monsters)
            {
                monster.Draw(canvas);
            }

            foreach (var spell in spellEffects)
            {
                spell.Draw(canvas);
            }

            if (playerImage != null)
            {
                int drawX = (int)playerX - (100 - playerWidth) / 2;
                int drawY = (int)playerY - (100 - playerHeight) / 2;
                canvas.DrawImage(playerImage, drawX, drawY, 100, 100);
            }
        }


        #region Logic Phụ (Helper Logic)

        private void CheckPlayerAttackHit()
        {
            RectangleF attackHitbox = new RectangleF();
            int attackRange = 50;
            int attackWidth = 40;

            switch (lastFacingDirection)
            {
                case "up":
                    attackHitbox = new RectangleF(playerX + (playerWidth / 2) - (attackWidth / 2), playerY - attackRange, attackWidth, attackRange);
                    break;
                case "down":
                    attackHitbox = new RectangleF(playerX + (playerWidth / 2) - (attackWidth / 2), playerY + playerHeight, attackWidth, attackRange);
                    break;
                case "left":
                    attackHitbox = new RectangleF(playerX - attackRange, playerY, attackRange, playerHeight);
                    break;
                case "right":
                    attackHitbox = new RectangleF(playerX + playerWidth, playerY, attackRange, playerHeight);
                    break;
            }

            foreach (var monster in monsters)
            {
                if (monster.State != Monster.MonsterState.Dead && attackHitbox.IntersectsWith(monster.Hitbox))
                {
                    int playerDamage = 10;
                    monster.TakeDamage(playerDamage);
                }
            }
        }


        private void DrawFloor(Graphics canvas)
        {
            if (floorTexture == null)
            {
                canvas.Clear(Color.FromArgb(50, 50, 50));
                return;
            }

            using (TextureBrush floorBrush = new TextureBrush(floorTexture, WrapMode.Tile))
            {
                canvas.FillRectangle(floorBrush, 0, 0, mapWidth, mapHeight);
            }
        }

        private void DrawWalls(Graphics canvas)
        {
            if (wallTexture == null) return;

            int tileSize = wallThickness;
            using (TextureBrush wallBrush = new TextureBrush(wallTexture, WrapMode.Tile))
            {
                canvas.FillRectangle(wallBrush, 0, 0, mapWidth, tileSize);
                canvas.FillRectangle(wallBrush, 0, mapHeight - tileSize, mapWidth, tileSize);
                canvas.FillRectangle(wallBrush, 0, 0, tileSize, mapHeight);
                canvas.FillRectangle(wallBrush, mapWidth - tileSize, 0, tileSize, gapYPosition);
                canvas.FillRectangle(wallBrush, mapWidth - tileSize, gapYPosition + gapSize, tileSize, mapHeight - (gapYPosition + gapSize));

                foreach (var wall in internalWalls)
                {
                    canvas.FillRectangle(wallBrush, wall);
                }
            }
        }


        private void UpdateFacingDirection()
        {
            if (isDashing) return;

            float deltaX = mousePos.X - (playerX + playerWidth / 2);
            float deltaY = mousePos.Y - (playerY + playerHeight / 2);

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                lastFacingDirection = (deltaX > 0) ? "right" : "left";
            }
            else
            {
                lastFacingDirection = (deltaY > 0) ? "down" : "up";
            }
        }

        private void GetDashDirection()
        {
            dashDirectionX = 0;
            dashDirectionY = 0;

            if (goUp) dashDirectionY = -1;
            if (goDown) dashDirectionY = 1;
            if (goLeft) dashDirectionX = -1;
            if (goRight) dashDirectionX = 1;

            if (dashDirectionX == 0 && dashDirectionY == 0)
            {
                if (lastFacingDirection == "up") dashDirectionY = -1;
                if (lastFacingDirection == "down") dashDirectionY = 1;
                if (lastFacingDirection == "left") dashDirectionX = -1;
                if (lastFacingDirection == "right") dashDirectionX = 1;
            }
        }

        private void UpdateDashState()
        {
            if (dashCooldown > 0) dashCooldown--;

            if (isDashing)
            {
                dashTimer--;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                }
            }
        }

        // --- SỬA: Logic va chạm trượt (Sliding Collision) ---
        private void UpdateMovement()
        {
            float dx = 0;
            float dy = 0;

            if (isDashing)
            {
                dx = dashDirectionX * dashSpeed;
                dy = dashDirectionY * dashSpeed;
            }
            else
            {
                bool isMoving = goUp || goDown || goLeft || goRight;
                canMove = !isHurt && !isDead;

                if (isAttacking && !isMoving)
                {
                    canMove = false;
                }

                if (canMove && isMoving)
                {
                    float moveX = 0;
                    float moveY = 0;

                    if (goLeft) moveX = -1;
                    if (goRight) moveX = 1;
                    if (goUp) moveY = -1;
                    if (goDown) moveY = 1;

                    if (moveX != 0 && moveY != 0)
                    {
                        float factor = 0.7071f;
                        moveX *= factor;
                        moveY *= factor;
                    }

                    int currentSpeed = isRunning ? runSpeed : walkSpeed;
                    dx = moveX * currentSpeed;
                    dy = moveY * currentSpeed;
                }
            }

            // --- LOGIC TRƯỢT MỚI (SỬA LỖI KẸT GÓC) ---

            // 1. Thử di chuyển X
            RectangleF nextHitboxX = new RectangleF(playerX + dx, playerY, playerWidth, playerHeight);
            if (!IsCollidingWithWall(nextHitboxX))
            {
                playerX += dx; // Di chuyển X
            }

            // 2. Thử di chuyển Y
            RectangleF nextHitboxY = new RectangleF(playerX, playerY + dy, playerWidth, playerHeight);
            if (!IsCollidingWithWall(nextHitboxY))
            {
                playerY += dy; // Di chuyển Y
            }
            // --- HẾT LOGIC TRƯỢT MỚI ---
        }

        public bool IsCollidingWithWall(RectangleF futureHitbox)
        {
            if (futureHitbox.Left < wallThickness) return true;
            if (futureHitbox.Top < wallThickness) return true;
            if (futureHitbox.Bottom > mapHeight - wallThickness) return true;

            if (futureHitbox.Right > mapWidth - wallThickness)
            {
                if (futureHitbox.Top > gapYPosition && futureHitbox.Bottom < (gapYPosition + gapSize))
                {
                    if (futureHitbox.Width == playerWidth && futureHitbox.Height == playerHeight)
                    {
                        if (futureHitbox.Left > mapWidth + 5)
                        {
                            HandleMapExit();
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }

            foreach (var wall in internalWalls)
            {
                if (futureHitbox.IntersectsWith(wall))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasLineOfSight(PointF start, PointF end)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return true;

            int steps = (int)(distance / 10);
            if (steps < 2) return true;

            float stepX = dx / steps;
            float stepY = dy / steps;

            for (int i = 1; i < steps; i++)
            {
                float checkX = start.X + stepX * i;
                float checkY = start.Y + stepY * i;

                if (IsCollidingWithWall(new RectangleF(checkX, checkY, 1, 1)))
                {
                    return false;
                }
            }

            return true;
        }


        private void HandleMapExit()
        {
            playerX = -playerWidth + wallThickness + 5;
            playerY = gapYPosition + (gapSize / 2);
        }

        private void UpdateAnimation()
        {
            bool isMoving = goUp || goDown || goLeft || goRight;
            string direction = lastFacingDirection;

            if (isDead)
            {
                playerImage = deathActivity.GetNextFrame(direction);
                return;
            }
            else if (isHurt)
            {
                playerImage = hurtActivity.GetNextFrame(direction);
                if (hurtActivity.IsFinished) isHurt = false;
            }
            else if (isDashing)
            {
                playerImage = runActivity.GetNextFrame(direction);
            }
            else if (isAttacking)
            {
                if (isRunning && isMoving)
                {
                    playerImage = runAttackActivity.GetNextFrame(direction);
                    if (runAttackActivity.IsFinished) isAttacking = false;
                }
                else if (isMoving)
                {
                    playerImage = walkAttackActivity.GetNextFrame(direction);
                    if (walkAttackActivity.IsFinished) isAttacking = false;
                }
                else
                {
                    playerImage = attackActivity.GetNextFrame(direction);
                    if (attackActivity.IsFinished) isAttacking = false;
                }
            }
            else if (isMoving)
            {
                if (isRunning)
                {
                    playerImage = runActivity.GetNextFrame(direction);
                }
                else
                {
                    playerImage = walkActivity.GetNextFrame(direction);
                }
            }
            else
            {
                playerImage = idleActivity.GetNextFrame(direction);
            }
        }

        #endregion
    }

    public class SpellEffect
    {
        public float X, Y;
        public int Width, Height;
        public AnimationActivity Anim;
        public bool IsFinished => Anim.IsFinished;
        private string direction = "down";

        public SpellEffect(AnimationActivity animTemplate, float x, float y)
        {
            Anim = new AnimationActivity(animTemplate.AnimationSpeed) { IsLooping = false };
            Anim.LoadImages(animTemplate.BackDir, animTemplate.FrontDir, animTemplate.LeftDir, animTemplate.RightDir);

            Width = 150;
            Height = 150;
            X = x - (Width / 2);
            Y = y - (Height / 2) - 30;
        }

        public void Update()
        {
            using (var img = Anim.GetNextFrame(direction))
            {
            }
        }

        public void Draw(Graphics canvas)
        {
            using (var image = Anim.GetNextFrame(direction))
            {
                if (image != null)
                {
                    canvas.DrawImage(image, (int)X, (int)Y, Width, Height);
                }
            }
        }
    }

    public abstract class Monster
    {
        public enum MonsterState { Idle, Patrol, Chase, Attack, Casting, Hurt, Dead }
        public MonsterState State { get; protected set; }

        public float X, Y;
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public RectangleF Hitbox => new RectangleF(X, Y, Width, Height);

        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        protected int attackDamage;
        protected float speed;
        protected float patrolSpeed;

        public AnimationActivity idleAnim, walkAnim, runAnim, attackAnim, hurtAnim, deathAnim;
        public AnimationActivity castAnim, spellAnim;

        protected string facingDirection = "down";
        protected float aggroRange = 300;
        protected float attackRange = 90;
        protected PointF patrolOrigin;
        protected PointF patrolTarget;
        protected int patrolTimer = 0;
        protected static Random rand = new Random();

        public Monster(float startX, float startY)
        {
            X = startX;
            Y = startY;
            patrolOrigin = new PointF(X, Y);
            // --- SỬA LỖI: Gọi hàm không tham số ---
            SetNewPatrolTarget();
            State = MonsterState.Idle;
        }

        protected abstract void LoadAnimations();
        protected abstract void SetStats();

        protected float GetDistanceToPlayer(float playerX, float playerY)
        {
            float dx = (playerX + 50) - (X + Width / 2);
            float dy = (playerY + 50) - (Y + Height / 2);
            return (dx * dx) + (dy * dy);
        }

        public virtual void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead) return;

            float distance = GetDistanceToPlayer(playerX, playerY);

            if (State == MonsterState.Hurt)
            {
                if (hurtAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Attack)
            {
                if (attackAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Casting)
            {
                return;
            }

            bool canSeePlayer = false;
            if (distance <= aggroRange * aggroRange)
            {
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerX + 50, playerY + 50));
            }

            if (distance <= attackRange * attackRange && canSeePlayer)
            {
                State = MonsterState.Attack;
                attackAnim.ResetFrame();
            }
            else if (distance <= aggroRange * aggroRange && canSeePlayer)
            {
                State = MonsterState.Chase;
            }
            else if (State == MonsterState.Chase)
            {
                State = MonsterState.Patrol;
                SetNewPatrolTarget(game);
            }
            else if (State == MonsterState.Idle)
            {
                patrolTimer++;
                if (patrolTimer > rand.Next(100, 200))
                {
                    patrolTimer = 0;
                    State = MonsterState.Patrol;
                    SetNewPatrolTarget(game);
                }
            }
            else if (State == MonsterState.Patrol)
            {
                MoveTowards(game, patrolTarget, patrolSpeed);
                if (GetDistanceToPoint(patrolTarget) < 10 * 10)
                {
                    State = MonsterState.Idle;
                }
            }

            if (State == MonsterState.Chase)
            {
                MoveTowards(game, new PointF(playerX + 50, playerY + 50), speed);
            }
        }

        // --- SỬA: Logic va chạm trượt (Sliding Collision) ---
        protected void MoveTowards(frmMainGame game, PointF target, float speed)
        {
            float dx = target.X - (X + Width / 2);
            float dy = target.Y - (Y + Height / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > speed)
            {
                dx /= distance;
                dy /= distance;

                float moveX = dx * speed;
                float moveY = dy * speed;

                // --- LOGIC TRƯỢT MỚI ---
                RectangleF hitbox = new RectangleF(X, Y, Width, Height);

                // 1. Thử di chuyển X
                hitbox.X += moveX;
                if (game.IsCollidingWithWall(hitbox))
                {
                    hitbox.X = X; // Hủy di chuyển X
                }

                // 2. Thử di chuyển Y
                hitbox.Y += moveY;
                if (game.IsCollidingWithWall(hitbox))
                {
                    hitbox.Y = Y; // Hủy di chuyển Y
                }

                // 3. Cập nhật vị trí cuối cùng
                X = hitbox.X;
                Y = hitbox.Y;
                // --- HẾT LOGIC TRƯỢT MỚI ---

                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    facingDirection = (dx > 0) ? "right" : "left";
                }
                else
                {
                    facingDirection = (dy > 0) ? "down" : "up";
                }
            }
        }

        protected float GetDistanceToPoint(PointF target)
        {
            float dx = target.X - (X + Width / 2);
            float dy = target.Y - (Y + Height / 2);
            return (dx * dx) + (dy * dy);
        }

        // --- SỬA LỖI: Xóa tham số 'game' ---
        protected void SetNewPatrolTarget()
        {
            int range = 150;
            float targetX = patrolOrigin.X + rand.Next(-range, range);
            float targetY = patrolOrigin.Y + rand.Next(-range, range);
            patrolTarget = new PointF(targetX, targetY);
        }

        // --- SỬA LỖI: Thêm 'game' vào hàm quá tải (overload) ---
        protected void SetNewPatrolTarget(frmMainGame game)
        {
            int range = 150;
            RectangleF testHitbox = new RectangleF(X, Y, Width, Height);

            for (int i = 0; i < 5; i++)
            {
                float targetX = patrolOrigin.X + rand.Next(-range, range);
                float targetY = patrolOrigin.Y + rand.Next(-range, range);

                testHitbox.X = targetX;
                testHitbox.Y = targetY;

                if (game != null && !game.IsCollidingWithWall(testHitbox))
                {
                    patrolTarget = new PointF(targetX, targetY);
                    return;
                }
                else if (game == null)
                {
                    patrolTarget = new PointF(targetX, targetY);
                    return;
                }
            }

            patrolTarget = patrolOrigin;
        }

        public virtual void TakeDamage(int damage)
        {
            if (State == MonsterState.Dead) return;

            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                State = MonsterState.Dead;
                deathAnim.ResetFrame();
            }
            else
            {
                State = MonsterState.Hurt;
                hurtAnim.ResetFrame();
            }
        }

        // --- SỬA LỖI: Thêm 'virtual' ---
        public virtual void Draw(Graphics canvas)
        {
            Image imageToDraw = null;

            switch (State)
            {
                case MonsterState.Idle:
                    imageToDraw = idleAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Casting:
                    imageToDraw = castAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(facingDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    // SỬA: Vẽ quái (hitbox)
                    int drawWidth = Width;
                    int drawHeight = Height;
                    int drawX = (int)X - (drawWidth - Width) / 2;
                    int drawY = (int)Y - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead)
            {
                DrawHealthBar(canvas);
            }
        }

        protected void DrawHealthBar(Graphics canvas)
        {
            int barWidth = Width;
            int barHeight = 10;
            int barY = (int)Y - barHeight - 5;

            canvas.FillRectangle(Brushes.Black, (int)X, barY, barWidth, barHeight);

            float healthPercentage = (float)Health / MaxHealth;
            int currentHealthWidth = (int)(barWidth * healthPercentage);

            canvas.FillRectangle(Brushes.LawnGreen, (int)X, barY, currentHealthWidth, barHeight);

            canvas.DrawRectangle(Pens.Black, (int)X, barY, barWidth, barHeight);
        }
    }

    public class Slime : Monster
    {
        public Slime(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();
        }

        protected override void SetStats()
        {
            Width = 40;
            Height = 40;
            MaxHealth = 50;
            Health = MaxHealth;
            attackDamage = 5;
            speed = 2.5f;
            patrolSpeed = 1f;
            attackRange = 90;
            aggroRange = 220;
        }

        protected override void LoadAnimations()
        {
            string slimeRoot = Path.Combine("ImgSource", "Char", "Monster", "Slime", "Slime_Water");

            string idleRoot = Path.Combine(slimeRoot, "Stand");
            idleAnim = new AnimationActivity(10);
            idleAnim.LoadImages(Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"), Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right"));

            string walkRoot = Path.Combine(slimeRoot, "Walk");
            walkAnim = new AnimationActivity(8);
            walkAnim.LoadImages(Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"), Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right"));

            string runRoot = Path.Combine(slimeRoot, "Run");
            runAnim = new AnimationActivity(5);
            runAnim.LoadImages(Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"), Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right"));

            string attackRoot = Path.Combine(slimeRoot, "Atk");
            attackAnim = new AnimationActivity(5) { IsLooping = false };
            attackAnim.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

            string hurtRoot = Path.Combine(slimeRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

            string deathRoot = Path.Combine(slimeRoot, "Death");
            deathAnim = new AnimationActivity(6) { IsLooping = false };
            deathAnim.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));
        }

        // --- SỬA LỖI: Thêm 'override' ---
        public override void Draw(Graphics canvas)
        {
            Image imageToDraw = null;

            switch (State)
            {
                case MonsterState.Idle:
                    imageToDraw = idleAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Casting:
                    imageToDraw = castAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(facingDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    int drawWidth = 80;
                    int drawHeight = 80;
                    int drawX = (int)X - (drawWidth - Width) / 2;
                    int drawY = (int)Y - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead)
            {
                DrawHealthBar(canvas);
            }
        }
    }

    public class Orc : Monster
    {
        public Orc(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();
        }

        protected override void SetStats()
        {
            Width = 45;
            Height = 45;
            MaxHealth = 120;
            Health = MaxHealth;
            attackDamage = 15;
            speed = 2.0f;
            patrolSpeed = 0.8f;
            attackRange = 120;
            aggroRange = 250;
        }

        protected override void LoadAnimations()
        {
            string orcRoot = Path.Combine("ImgSource", "Char", "Monster", "Orc", "Orc_Water");

            string idleRoot = Path.Combine(orcRoot, "Stand");
            idleAnim = new AnimationActivity(12);
            idleAnim.LoadImages(Path.Combine(idleRoot, "Back"), Path.Combine(idleRoot, "Front"), Path.Combine(idleRoot, "Left"), Path.Combine(idleRoot, "Right"));

            string walkRoot = Path.Combine(orcRoot, "Walk");
            walkAnim = new AnimationActivity(10);
            walkAnim.LoadImages(Path.Combine(walkRoot, "Back"), Path.Combine(walkRoot, "Front"), Path.Combine(walkRoot, "Left"), Path.Combine(walkRoot, "Right"));

            string runRoot = Path.Combine(orcRoot, "Run");
            runAnim = new AnimationActivity(6);
            runAnim.LoadImages(Path.Combine(runRoot, "Back"), Path.Combine(runRoot, "Front"), Path.Combine(runRoot, "Left"), Path.Combine(runRoot, "Right"));

            string attackRoot = Path.Combine(orcRoot, "Atk");
            attackAnim = new AnimationActivity(5) { IsLooping = false };
            attackAnim.LoadImages(Path.Combine(attackRoot, "Back"), Path.Combine(attackRoot, "Front"), Path.Combine(attackRoot, "Left"), Path.Combine(attackRoot, "Right"));

            string hurtRoot = Path.Combine(orcRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(Path.Combine(hurtRoot, "Back"), Path.Combine(hurtRoot, "Front"), Path.Combine(hurtRoot, "Left"), Path.Combine(hurtRoot, "Right"));

            string deathRoot = Path.Combine(orcRoot, "Death");
            deathAnim = new AnimationActivity(7) { IsLooping = false };
            deathAnim.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));
        }

        // --- SỬA LỖI: Thêm 'override' ---
        public override void Draw(Graphics canvas)
        {
            Image imageToDraw = null;

            switch (State)
            {
                case MonsterState.Idle:
                    imageToDraw = idleAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Casting:
                    imageToDraw = castAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(facingDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    int drawWidth = 110;
                    int drawHeight = 110;
                    int drawX = (int)X - (drawWidth - Width) / 2;
                    int drawY = (int)Y - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead)
            {
                DrawHealthBar(canvas);
            }
        }
    }

    public class Boss : Monster
    {
        private int castCooldown = 0;
        private int castCooldownDuration = 333;

        public Boss(float startX, float startY) : base(startX, startY)
        {
            SetStats();
            LoadAnimations();
        }

        protected override void SetStats()
        {
            Width = 100;
            Height = 100;
            MaxHealth = 500;
            Health = MaxHealth;
            attackDamage = 20;
            speed = 1.5f;
            patrolSpeed = 0.5f;
            attackRange = 150;
            aggroRange = 450;
            facingDirection = "down";
        }

        protected override void LoadAnimations()
        {
            string bossRoot = Path.Combine("ImgSource", "Char", "Monster", "Boss");

            string idleRoot = Path.Combine(bossRoot, "Stand");
            idleAnim = new AnimationActivity(12);
            idleAnim.LoadImages(null, idleRoot, null, null);

            string walkRoot = Path.Combine(bossRoot, "Walk");
            walkAnim = new AnimationActivity(10);
            walkAnim.LoadImages(null, walkRoot, null, null);

            runAnim = new AnimationActivity(walkAnim.AnimationSpeed);
            runAnim.LoadImages(walkAnim.BackDir, walkAnim.FrontDir, walkAnim.LeftDir, walkAnim.RightDir);

            string attackRoot = Path.Combine(bossRoot, "Attack");
            attackAnim = new AnimationActivity(5) { IsLooping = false };
            attackAnim.LoadImages(null, attackRoot, null, null);

            string hurtRoot = Path.Combine(bossRoot, "Hurt");
            hurtAnim = new AnimationActivity(4) { IsLooping = false };
            hurtAnim.LoadImages(null, hurtRoot, null, null);

            string deathRoot = Path.Combine(bossRoot, "Death");
            deathAnim = new AnimationActivity(7) { IsLooping = false };
            deathAnim.LoadImages(null, deathRoot, null, null);

            string castRoot = Path.Combine(bossRoot, "Cast");
            castAnim = new AnimationActivity(5) { IsLooping = false };
            castAnim.LoadImages(null, castRoot, null, null);

            string spellRoot = Path.Combine(bossRoot, "Spell");
            spellAnim = new AnimationActivity(4) { IsLooping = false };
            spellAnim.LoadImages(null, spellRoot, null, null);
        }

        public override void TakeDamage(int damage)
        {
            if (State == MonsterState.Dead) return;

            if (State == MonsterState.Casting)
            {
                Health -= damage;
                if (Health <= 0)
                {
                    Health = 0;
                    State = MonsterState.Dead;
                    deathAnim.ResetFrame();
                }
                return;
            }
            base.TakeDamage(damage);
        }


        public override void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead) return;

            if (castCooldown > 0) castCooldown--;

            float distance = GetDistanceToPlayer(playerX, playerY);

            if (State == MonsterState.Hurt)
            {
                if (hurtAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Attack)
            {
                if (attackAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Casting)
            {
                if (castAnim.IsFinished)
                {
                    spellEffects.Add(new SpellEffect(spellAnim, playerX + 50, playerY + 50));

                    State = MonsterState.Idle;
                    patrolTimer = 0;
                    castCooldown = castCooldownDuration;
                }
                return;
            }

            bool canSeePlayer = false;
            if (distance <= aggroRange * aggroRange)
            {
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerX + 50, playerY + 50));
            }

            if (castCooldown == 0 && canSeePlayer)
            {
                State = MonsterState.Casting;
                castAnim.ResetFrame();
            }
            else if (distance <= attackRange * attackRange && canSeePlayer)
            {
                State = MonsterState.Attack;
                attackAnim.ResetFrame();
            }
            else if (distance <= aggroRange * aggroRange && canSeePlayer)
            {
                State = MonsterState.Chase;
            }
            else if (State == MonsterState.Chase)
            {
                State = MonsterState.Patrol;
                // --- SỬA LỖI: Truyền 'game' vào ---
                SetNewPatrolTarget(game);
            }
            else if (State == MonsterState.Idle)
            {
                patrolTimer++;
                if (patrolTimer > rand.Next(100, 200))
                {
                    patrolTimer = 0;
                    State = MonsterState.Patrol;
                    // --- SỬA LỖI: Truyền 'game' vào ---
                    SetNewPatrolTarget(game);
                }
            }
            else if (State == MonsterState.Patrol)
            {
                MoveTowards(game, patrolTarget, patrolSpeed);
                if (GetDistanceToPoint(patrolTarget) < 10 * 10)
                {
                    State = MonsterState.Idle;
                }
            }

            if (State == MonsterState.Patrol)
            {
                MoveTowards(game, patrolTarget, patrolSpeed);
            }
            else if (State == MonsterState.Chase)
            {
                MoveTowards(game, new PointF(playerX + 50, playerY + 50), speed);
            }

            this.facingDirection = "down";
        }

        // --- SỬA LỖI: Thêm 'override' ---
        public override void Draw(Graphics canvas)
        {
            Image imageToDraw = null;

            switch (State)
            {
                case MonsterState.Idle:
                    imageToDraw = idleAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Casting:
                    imageToDraw = castAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(facingDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(facingDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    int drawWidth = 150;
                    int drawHeight = 150;
                    int drawX = (int)X - (drawWidth - Width) / 2;
                    int drawY = (int)Y - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead)
            {
                DrawHealthBar(canvas);
            }
        }

    }

}

