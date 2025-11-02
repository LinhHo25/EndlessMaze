using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL.Models; // <-- SỬ DỤNG LỚP MODEL THẬT CỦA BẠN

namespace Main
{
    // --- LƯU Ý: MazeGenerator và GameObjectType cần được định nghĩa trong MazeGeneration.cs ---

    public partial class frmMainGame : Form // Đổi tên lớp Form thành frmMainGame
    {
        private readonly DAL.Models.PlayerCharacters _character; // SỬ DỤNG DAL.Models.PlayerCharacters
        private readonly int _mapLevel;

        // --- HẰNG SỐ MÊ CUNG VÀ GAME ---
        private const int TILE_SIZE = 30; // Kích thước mỗi ô mê cung (pixel)
        private const int MAZE_LOGIC_WIDTH = 51; // Kích thước lưới logic (STEP=2)
        private const int MAZE_LOGIC_HEIGHT = 31; // Kích thước lưới logic (STEP=2)
        private const float REVEAL_RADIUS = 7.0f; // KHÔI PHỤC: Bán kính khám phá rộng hơn

        // THAY ĐỔI CẤP ĐỘ BẢO VỆ TỪ private SANG public (KHẮC PHỤC CS0122)
        public const float BASE_SLIDE_TOLERANCE = 4.0f; // Độ trượt cơ sở (1:1)
        public const int RENDER_SCALE = 3; // TỶ LỆ PHÓNG TO: Mỗi ô logic sẽ được vẽ thành 3x3 ô vật lý

        // Kích thước Map logic sau khi nhân tỷ lệ
        private int _mapLogicalWidthScaled => MAZE_LOGIC_WIDTH * TILE_SIZE * RENDER_SCALE;
        private int _mapLogicalHeightScaled => MAZE_LOGIC_HEIGHT * TILE_SIZE * RENDER_SCALE;
        // -----------------------------

        // --- THÊM: Biến Camera, Fog of War, và LOGIC GAME MỚI ---
        private float _cameraX = 0;
        private float _cameraY = 0;
        private bool[,] _isTileVisible = new bool[MAZE_LOGIC_HEIGHT, MAZE_LOGIC_WIDTH];

        public int _monstersKilled = 0; // ĐIỀU KIỆN KẾT THÚC: Đếm số quái đã giết
        public bool _isBossDefeated = false; // ĐIỀU KIỆN KẾT THÚC: Cờ Boss đã chết

        private Image playerImage;
        private float playerX = 1.5f * TILE_SIZE;
        private float playerY = 1.5f * TILE_SIZE;

        private int playerWidth = 25;
        private int playerHeight = 25;

        // LƯU Ý: Lớp AnimationActivity cần phải được định nghĩa ở đâu đó trong project của bạn.
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

        // ĐIỀU CHỈNH TỐC ĐỘ DI CHUYỂN
        private int walkSpeed = 6;
        private int runSpeed = 10;
        private int dashSpeed = 25;
        private bool isDashing = false;
        private int dashTimer = 0;
        private int dashCooldown = 0;
        private float dashDirectionX, dashDirectionY;

        private int mapWidth;
        private int mapHeight;
        private Image floorTexture;
        private Image wallTexture;

        // KHẮC PHỤC LỖI CS0103: Thêm lại khai báo biến
        private Image darkFloorTexture;
        private Image darkWallTexture;

        // LOẠI BỎ TEXTURE TỐI (Vì chúng không có sẵn), THAY BẰNG LOGIC VẼ MỜ
        private int wallThickness = 50;
        private int gapYPosition = 100;
        private int gapSize = 150;

        private List<Monster> monsters = new List<Monster>();

        private List<SpellEffect> spellEffects = new List<SpellEffect>();

        // LƯU TRỮ CÁC Ô TƯỜNG CỦA MÊ CUNG
        private List<RectangleF> _mazeWallRects = new List<RectangleF>();
        // LƯU TRỮ LƯỚI LOGIC ĐỂ VẼ 3X3 SAU NÀY
        private int[,] _mazeGrid;

        // --- CÁC THAM SỐ VẼ MỜ ---
        private ImageAttributes _darkenAttributes = new ImageAttributes();

        public frmMainGame()
        {
            InitializeComponent();
            // THIẾT LẬP COLOR MATRIX LÀM TỐI (DARKEN)
            float[][] colorMatrixElements = {
                new float[] {.5f,  .0f,  .0f,  .0f, 0.0f}, // red scale factor
                new float[] {.0f,  .5f,  .0f,  .0f, 0.0f}, // green scale factor
                new float[] {.0f,  .0f,  .5f,  .0f, 0.0f}, // blue scale factor
                new float[] {.0f,  .0f,  .0f, 1.0f, 0.0f}, // alpha scale factor
                new float[] {.0f, 0.0f, 0.0f, 0.0f, 1.0f}  // translations
            };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            _darkenAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        public frmMainGame(DAL.Models.PlayerCharacters character, int mapLevel) // SỬA: Nhận DAL.Models.PlayerCharacters
        {
            InitializeComponent();
            // THIẾT LẬP COLOR MATRIX LÀM TỐI (DARKEN)
            float[][] colorMatrixElements = {
                new float[] {.5f,  .0f,  .0f,  .0f, 0.0f},
                new float[] {.0f,  .5f,  .0f,  .0f, 0.0f},
                new float[] {.0f,  .0f,  .5f,  .0f, 0.0f},
                new float[] {.0f,  .0f,  .0f, 1.0f, 0.0f},
                new float[] {.0f, 0.0f, 0.0f, 0.0f, 1.0f}
            };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            _darkenAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

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

            // SỬ DỤNG LOGIC TẠO MÊ CUNG
            GenerateMaze();

            // SỬA LỖI KẸT CỨNG: Đảm bảo vị trí xuất phát nằm chính giữa ô Passage đầu tiên (1, 1) logic VÀ LÀM TRÒN
            float passageCenterX = 1 * TILE_SIZE + TILE_SIZE / 2f;
            float passageCenterY = 1 * TILE_SIZE + TILE_SIZE / 2f;

            // ÉP VỀ SỐ NGUYÊN để tránh lỗi làm tròn float khi khởi tạo
            playerX = (float)Math.Floor(passageCenterX - playerWidth / 2f);
            playerY = (float)Math.Floor(passageCenterY - playerHeight / 2f);

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

        private Image LoadImageSafe(string path, Image fallbackImage)
        {
            try
            {
                if (!File.Exists(path))
                {
                    // Nếu không tìm thấy, trả về ảnh dự phòng
                    return fallbackImage;
                }
                // Dùng MemoryStream để Image không bị khóa file
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải ảnh {path}: {ex.Message}");
                return fallbackImage;
            }
        }

        private void LoadMapTextures()
        {
            try
            {
                string floorPath = Path.Combine("ImgSource", "Block", "default", "Floor", "catacombs_0.png");
                string wallPath = Path.Combine("ImgSource", "Block", "default", "Wall", "Stone_Wall", "stone_brick_1.png");

                // 1. Tải ảnh sáng (gốc)
                floorTexture = Image.FromFile(floorPath);
                wallTexture = Image.FromFile(wallPath);

                // 2. Tải ảnh tối (Dark Texture) - SỬA LỖI LOAD FILE
                // Do file _dark.png không tồn tại, ta chỉ cần gán lại bằng file gốc để tránh lỗi
                darkFloorTexture = floorTexture;
                darkWallTexture = wallTexture;

                // LƯU Ý: Nếu ảnh gốc của bạn là gạch sáng/trắng, bạn có thể thấy gạch sáng/trắng ở vùng đã khám phá.

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

        // --- THAY THẾ: InitializeInternalWalls bằng GenerateMaze (Cập nhật cho Scale 3x3) ---
        private void GenerateMaze()
        {
            // LƯU Ý: MazeGenerator (trong MazeGeneration.cs) cần được tham chiếu từ Main namespace.
            // Nếu bạn không có MazeGenerator, code này sẽ lỗi.
            MazeGenerator mazeGen = new MazeGenerator(MAZE_LOGIC_WIDTH, MAZE_LOGIC_HEIGHT);
            mazeGen.GenerateMaze();
            _mazeGrid = mazeGen.Maze; // LƯU LƯỚI LOGIC
            _mazeWallRects.Clear();

            int currentLogicWidth = _mazeGrid.GetLength(1);
            int currentLogicHeight = _mazeGrid.GetLength(0);

            // BƯỚC QUAN TRỌNG: TẠO WALL RECTANGLE DỰ TRÊN TỶ LỆ RENDER_SCALE
            // Wall logic (STEP=2) sẽ là 1 ô vật lý (TILE_SIZE), Passage logic sẽ là 3 ô vật lý (3*TILE_SIZE)
            for (int y = 0; y < currentLogicHeight; y++)
            {
                for (int x = 0; x < currentLogicWidth; x++)
                {
                    // LƯU Ý: GameObjectType.Wall được định nghĩa trong MazeGeneration.cs
                    if (_mazeGrid[y, x] == (int)GameObjectType.Wall)
                    {
                        // Wall logic (Wall = Tường): 1x1 ô vật lý
                        _mazeWallRects.Add(new RectangleF(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                    }
                    else // Passage logic
                    {
                        // Passage logic (Passage = Lối đi)
                        // Chúng ta chỉ tạo Wall rects để kiểm tra va chạm.
                    }
                }
            }
        }
        // --- KẾT THÚC THAY THẾ ---

        private void InitializeMonsters()
        {
            // SỬ DỤNG TILE_SIZE ĐỂ ĐẶT VỊ TRÍ QUÁI VẬT
            // Vị trí vẫn dựa trên lưới 1:1 logic, nhưng sẽ được vẽ bằng tỷ lệ 3x
            monsters.Add(new Slime(10 * TILE_SIZE, 10 * TILE_SIZE));
            monsters.Add(new Slime(20 * TILE_SIZE, 15 * TILE_SIZE));

            monsters.Add(new Orc(30 * TILE_SIZE, 5 * TILE_SIZE));
            monsters.Add(new Orc(40 * TILE_SIZE, 20 * TILE_SIZE));
            monsters.Add(new Orc(25 * TILE_SIZE, 25 * TILE_SIZE));

            // ĐIỀU CHỈNH: Đặt Boss ngay trước cửa ra ở góc dưới bên phải
            float bossX = (MAZE_LOGIC_WIDTH - 4) * TILE_SIZE; // Đẩy boss lùi 4 ô so với biên
            float bossY = (MAZE_LOGIC_HEIGHT - 4) * TILE_SIZE; // Đẩy boss lùi 4 ô so với biên
            monsters.Add(new Boss(bossX, bossY));
        }

        // --- HÀM RemoveWall KHÔNG CẦN THIẾT NỮA ---
        // private void RemoveWall(int cellX, int cellY, char direction) { ... }


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
            // KHẮC PHỤC LỖI: Không nên gọi Dispose() cho playerImage
            // playerImage?.Dispose(); 

            UpdateFacingDirection();
            UpdateDashState();
            UpdateMovement();
            UpdateCamera();
            UpdateFogOfWar(); // CẬP NHẬT FOG OF WAR
            UpdateAnimation();

            foreach (var monster in monsters.ToList())
            {
                monster.Update(this, playerX, playerY, spellEffects);

                if (monster.State == Monster.MonsterState.Dead && monster.deathAnim.IsFinished)
                {
                    monsters.Remove(monster);
                    _monstersKilled++; // ĐIỀU KIỆN KẾT THÚC: Tăng biến đếm

                    // Nếu quái vật chết là Boss
                    if (monster is Boss)
                    {
                        _isBossDefeated = true; // ĐIỀU KIỆN KẾT THÚC: Cập nhật cờ Boss
                    }
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

        // --- HÀM MỚI: CẬP NHẬT FOG OF WAR ---
        private void UpdateFogOfWar()
        {
            // Tọa độ ô hiện tại của người chơi
            // CHIA CHO TILE_SIZE ĐỂ LẤY VỊ TRÍ TRÊN LƯỚI LOGIC 1:1
            int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
            int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);

            int startX = Math.Max(0, playerTileX - (int)Math.Ceiling(REVEAL_RADIUS));
            int endX = Math.Min(MAZE_LOGIC_WIDTH - 1, playerTileX + (int)Math.Ceiling(REVEAL_RADIUS));
            int startY = Math.Max(0, playerTileY - (int)Math.Ceiling(REVEAL_RADIUS));
            int endY = Math.Min(MAZE_LOGIC_HEIGHT - 1, playerTileY + (int)Math.Ceiling(REVEAL_RADIUS));

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    // Đánh dấu ô này là đã thấy
                    if (Math.Pow(x - playerTileX, 2) + Math.Pow(y - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2))
                    {
                        _isTileVisible[y, x] = true;
                    }
                }
            }
        }

        // --- HÀM MỚI: CẬP NHẬT VỊ TRÍ CAMERA ---
        private void UpdateCamera()
        {
            // Lấy tâm hitbox của người chơi
            float playerCenterX = playerX + playerWidth / 2;
            float playerCenterY = playerY + playerHeight / 2;

            // Vị trí tâm người chơi đã được scale
            float playerScaledCenterX = playerCenterX * RENDER_SCALE;
            float playerScaledCenterY = playerCenterY * RENDER_SCALE;

            // Tính toán vị trí camera để căn giữa nhân vật vào giữa màn hình
            _cameraX = playerScaledCenterX - mapWidth / 2;
            _cameraY = playerScaledCenterY - mapHeight / 2;

            // --- GIỚI HẠN CAMERA VÀO BIÊN MAP SCALED ---

            // Biên trái (min X = 0)
            if (_cameraX < 0) _cameraX = 0;

            // Biên trên (min Y = 0)
            if (_cameraY < 0) _cameraY = 0;

            // Biên phải (max X)
            if (_cameraX + mapWidth > _mapLogicalWidthScaled) _cameraX = _mapLogicalWidthScaled - mapWidth;

            // Biên dưới (max Y)
            if (_cameraY + mapHeight > _mapLogicalHeightScaled) _cameraY = _mapLogicalHeightScaled - mapHeight;

            // Xử lý trường hợp map nhỏ hơn màn hình (căn giữa map)
            if (_mapLogicalWidthScaled < mapWidth)
            {
                _cameraX = -(mapWidth - _mapLogicalWidthScaled) / 2;
            }
            if (_mapLogicalHeightScaled < mapHeight)
            {
                _cameraY = -(mapHeight - _mapLogicalHeightScaled) / 2;
            }
        }


        private void frmMainGame_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.InterpolationMode = InterpolationMode.NearestNeighbor;

            // Dịch chuyển canvas theo offset camera (Camera Translation)
            canvas.TranslateTransform(-_cameraX, -_cameraY);

            // 1. VẼ NỀN ĐEN TOÀN BỘ (Thể hiện Vùng chưa khám phá)
            canvas.FillRectangle(Brushes.Black, 0, 0, _mapLogicalWidthScaled, _mapLogicalHeightScaled);

            // 2. VẼ SÀN và TƯỜNG (Chỉ vẽ những ô đã được khám phá)
            DrawFloor(canvas);
            DrawWalls(canvas);

            // 3. VẼ QUÁI VẬT, SPELL VÀ NGƯỜI CHƠI (Áp dụng Scale 3x cho tất cả vị trí)

            // VẼ TẤT CẢ VỊ TRÍ VẬT LÝ x RENDER_SCALE

            // VẼ NGƯỜI CHƠI
            if (playerImage != null)
            {
                // Vị trí vẽ được căn chỉnh để hình ảnh 100x100 khớp với hitbox 25x25
                // drawX = playerScaledX - (image_width - hitbox_width)/2
                int drawX = (int)(playerX * RENDER_SCALE) - (100 - playerWidth) / 2;
                int drawY = (int)(playerY * RENDER_SCALE) - (100 - playerHeight) / 2;
                canvas.DrawImage(playerImage, drawX, drawY, 100, 100);
            }

            // VẼ MONSTER
            RectangleF viewportRect = new RectangleF(_cameraX, _cameraY, mapWidth, mapHeight);

            foreach (var monster in monsters)
            {
                // Lấy vị trí logic của quái vật
                int monsterTileX = (int)((monster.X + monster.Width / 2) / TILE_SIZE);
                int monsterTileY = (int)((monster.Y + monster.Height / 2) / TILE_SIZE);

                // Vị trí vẽ (Scaled)
                float monsterDrawX = monster.X * RENDER_SCALE;
                float monsterDrawY = monster.Y * RENDER_SCALE;

                // Chỉ vẽ quái vật nếu ô logic của nó đã được khám phá VÀ nó nằm trong viewport
                if (monsterTileY >= 0 && monsterTileY < MAZE_LOGIC_HEIGHT &&
                    monsterTileX >= 0 && monsterTileX < MAZE_LOGIC_WIDTH &&
                    _isTileVisible[monsterTileY, monsterTileX])
                {
                    // Tạo một Hitbox tạm thời đã được scale để kiểm tra viewport
                    RectangleF scaledHitbox = new RectangleF(monsterDrawX, monsterDrawY, monster.Width * RENDER_SCALE, monster.Height * RENDER_SCALE);

                    if (viewportRect.IntersectsWith(scaledHitbox))
                    {
                        // Truyền vị trí đã scale vào Draw
                        monster.Draw(canvas, RENDER_SCALE);
                    }
                }
            }

            // VẼ SPELL EFFECTS
            foreach (var spell in spellEffects)
            {
                // Truyền vị trí đã scale vào Draw
                spell.Draw(canvas, RENDER_SCALE);
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

            // Vị trí Hitbox Attack cần được nhân Scale để kiểm tra va chạm với quái vật đã scale
            RectangleF scaledAttackHitbox = new RectangleF(
                attackHitbox.X * RENDER_SCALE,
                attackHitbox.Y * RENDER_SCALE,
                attackHitbox.Width * RENDER_SCALE,
                attackHitbox.Height * RENDER_SCALE
            );


            foreach (var monster in monsters)
            {
                // Hitbox của quái vật (monster.Hitbox) cần được đọc là đã scale
                if (monster.State != Monster.MonsterState.Dead && scaledAttackHitbox.IntersectsWith(monster.ScaledHitbox(RENDER_SCALE)))
                {
                    int playerDamage = 10;
                    monster.TakeDamage(playerDamage);
                }
            }
        }


        private void DrawFloor(Graphics canvas)
        {
            if (floorTexture == null || _mazeGrid == null) return;

            // VẼ SÀN (PASSAGE)
            using (TextureBrush floorBrush = new TextureBrush(floorTexture, WrapMode.Tile))
            {
                int currentLogicHeight = _mazeGrid.GetLength(0);
                int currentLogicWidth = _mazeGrid.GetLength(1);

                for (int y = 0; y < currentLogicHeight; y++)
                {
                    for (int x = 0; x < currentLogicWidth; x++)
                    {
                        // LƯU Ý: GameObjectType.Passage được định nghĩa trong MazeGeneration.cs
                        if (_mazeGrid[y, x] == (int)GameObjectType.Passage || _mazeGrid[y, x] == (int)GameObjectType.Start || _mazeGrid[y, x] == (int)GameObjectType.Exit)
                        {
                            // Xác định xem ô có nằm trong phạm vi REVEAL_RADIUS (hiện tại) không
                            int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
                            int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);
                            bool isCurrentlyVisible = Math.Pow(x - playerTileX, 2) + Math.Pow(y - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2);

                            Image imageToDraw = floorTexture;
                            ImageAttributes attributesToUse = null;

                            if (_isTileVisible[y, x])
                            {
                                // Đã từng thấy (vùng đã khám phá - màu tối)
                                attributesToUse = _darkenAttributes;
                            }

                            if (isCurrentlyVisible)
                            {
                                // Đang thấy (vùng hiện tại - màu sáng)
                                attributesToUse = null; // Không áp dụng hiệu ứng làm tối
                            }

                            if (attributesToUse != null || _isTileVisible[y, x]) // Chỉ vẽ nếu đã thấy hoặc đang thấy
                            {
                                // VẼ DÙNG DRAWIMAGE VỚI IMAGEATTRIBUTES ĐỂ LÀM TỐI
                                canvas.DrawImage(
                                    imageToDraw,
                                    new Rectangle(
                                        x * TILE_SIZE * RENDER_SCALE,
                                        y * TILE_SIZE * RENDER_SCALE,
                                        TILE_SIZE * RENDER_SCALE,
                                        TILE_SIZE * RENDER_SCALE
                                    ),
                                    0, 0, imageToDraw.Width, imageToDraw.Height, // Source Rectangle
                                    GraphicsUnit.Pixel,
                                    attributesToUse
                                );
                            }
                        }
                    }
                }
            }
        }

        private void DrawWalls(Graphics canvas)
        {
            if (wallTexture == null) return;

            using (TextureBrush wallBrush = new TextureBrush(wallTexture, WrapMode.Tile))
            {
                // VẼ TƯỜNG MÊ CUNG NỘI BỘ (Chỉ vẽ các tường nằm trong vùng đã khám phá)
                foreach (var wall in _mazeWallRects)
                {
                    // Chuyển vị trí tường sang tọa độ ô logic
                    int wallTileX = (int)Math.Floor(wall.X / TILE_SIZE);
                    int wallTileY = (int)Math.Floor(wall.Y / TILE_SIZE);

                    // SỬA LỖI System.IndexOutOfRangeException: Thêm kiểm tra biên
                    if (wallTileY >= 0 && wallTileY < MAZE_LOGIC_HEIGHT &&
                        wallTileX >= 0 && wallTileX < MAZE_LOGIC_WIDTH)
                    {
                        // Xác định xem ô có nằm trong phạm vi REVEAL_RADIUS (hiện tại) không
                        int playerTileX = (int)((playerX + playerWidth / 2) / TILE_SIZE);
                        int playerTileY = (int)((playerY + playerHeight / 2) / TILE_SIZE);
                        bool isCurrentlyVisible = Math.Pow(wallTileX - playerTileX, 2) + Math.Pow(wallTileY - playerTileY, 2) <= Math.Pow(REVEAL_RADIUS, 2);

                        ImageAttributes attributesToUse = null;

                        if (_isTileVisible[wallTileY, wallTileX])
                        {
                            // Đã từng thấy (vùng đã khám phá - màu tối)
                            attributesToUse = _darkenAttributes;
                        }

                        if (isCurrentlyVisible)
                        {
                            // Đang thấy (vùng hiện tại - màu sáng)
                            attributesToUse = null;
                        }

                        if (attributesToUse != null || _isTileVisible[wallTileY, wallTileX]) // Chỉ vẽ nếu đã thấy hoặc đang thấy
                        {
                            Image imageToDraw = wallTexture;

                            // Tường chỉ rộng 1x1 ô logic (30x30)
                            // Áp dụng scale: wall.X * RENDER_SCALE, wall.Y * RENDER_SCALE, TILE_SIZE * RENDER_SCALE, TILE_SIZE * RENDER_SCALE
                            canvas.DrawImage(
                                imageToDraw,
                                new Rectangle(
                                    (int)(wall.X * RENDER_SCALE),
                                    (int)(wall.Y * RENDER_SCALE),
                                    (int)(wall.Width * RENDER_SCALE), // KHẮC PHỤC LỖI CS1503
                                    (int)(wall.Height * RENDER_SCALE) // KHẮC PHỤC LỖI CS1503
                                ),
                                0, 0, imageToDraw.Width, imageToDraw.Height, // Source Rectangle
                                GraphicsUnit.Pixel,
                                attributesToUse
                            );
                        }
                    }
                }

                // KHÔNG CẦN VẼ TƯỜNG NGOÀI (MazeGenerator đã tự vẽ tường bao)
            }
        }


        private void UpdateFacingDirection()
        {
            if (isDashing) return;

            // Tính toán vị trí chuột tương đối với map, không phải form
            // Cần phải thêm _cameraX và _cameraY để chuyển đổi tọa độ màn hình (mousePos) sang tọa độ thế giới (Map coordinates)
            float mouseXOnMap = mousePos.X + _cameraX;
            float mouseYOnMap = mousePos.Y + _cameraY;

            // Vị trí người chơi đã được scale (playerX * 3, playerY * 3)
            float playerScaledX = playerX * RENDER_SCALE;
            float playerScaledY = playerY * RENDER_SCALE;

            float deltaX = mouseXOnMap - (playerScaledX + playerWidth / 2);
            float deltaY = mouseYOnMap - (playerScaledY + playerHeight / 2);

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

            // KHÁNG TRƯỢT ĐÃ ĐƯỢC CHIA TỶ LỆ (truy cập bằng tên kiểu)
            float scaledSlideTolerance = frmMainGame.BASE_SLIDE_TOLERANCE / frmMainGame.RENDER_SCALE;

            // 1. Thử di chuyển X
            // Hitbox logic (1:1)
            RectangleF nextHitboxX = new RectangleF(playerX + dx, playerY, playerWidth, playerHeight);

            // Hitbox đã scale (3:1)
            RectangleF scaledNextHitboxX = new RectangleF(
                nextHitboxX.X * frmMainGame.RENDER_SCALE,
                nextHitboxX.Y * frmMainGame.RENDER_SCALE,
                nextHitboxX.Width * frmMainGame.RENDER_SCALE,
                nextHitboxX.Height * frmMainGame.RENDER_SCALE
            );

            if (!IsCollidingWithWallScaled(scaledNextHitboxX))
            {
                playerX += dx; // Di chuyển X (giữ nguyên ở lưới logic 1:1)
            }
            else
            {
                // Nếu va chạm, thử trượt Y
                if (dy != 0)
                {
                    // Sử dụng giá trị trượt đã được chia tỷ lệ (scaledSlideTolerance)
                    float slideY = dy > 0 ? scaledSlideTolerance : -scaledSlideTolerance;

                    RectangleF slideHitboxY = new RectangleF(playerX + dx, playerY + slideY, playerWidth, playerHeight);
                    RectangleF scaledSlideHitboxY = new RectangleF(
                        slideHitboxY.X * frmMainGame.RENDER_SCALE,
                        slideHitboxY.Y * frmMainGame.RENDER_SCALE,
                        slideHitboxY.Width * frmMainGame.RENDER_SCALE,
                        slideHitboxY.Height * frmMainGame.RENDER_SCALE
                    );

                    if (!IsCollidingWithWallScaled(scaledSlideHitboxY))
                    {
                        playerY += slideY;
                    }
                }
            }

            // 2. Thử di chuyển Y
            RectangleF nextHitboxY = new RectangleF(playerX, playerY + dy, playerWidth, playerHeight);
            RectangleF scaledNextHitboxY = new RectangleF(
                nextHitboxY.X * frmMainGame.RENDER_SCALE,
                nextHitboxY.Y * frmMainGame.RENDER_SCALE,
                nextHitboxY.Width * frmMainGame.RENDER_SCALE,
                nextHitboxY.Height * frmMainGame.RENDER_SCALE
            );

            if (!IsCollidingWithWallScaled(scaledNextHitboxY))
            {
                playerY += dy; // Di chuyển Y (giữ nguyên ở lưới logic 1:1)
            }
            else
            {
                // Nếu va chạm, thử trượt X
                if (dx != 0)
                {
                    // Sử dụng giá trị trượt đã được chia tỷ lệ (scaledSlideTolerance)
                    float slideX = dx > 0 ? scaledSlideTolerance : -scaledSlideTolerance;

                    RectangleF slideHitboxX = new RectangleF(playerX + slideX, playerY + dy, playerWidth, playerHeight);
                    RectangleF scaledSlideHitboxX = new RectangleF(
                        slideHitboxX.X * frmMainGame.RENDER_SCALE,
                        slideHitboxX.Y * frmMainGame.RENDER_SCALE,
                        slideHitboxX.Width * frmMainGame.RENDER_SCALE,
                        slideHitboxX.Height * frmMainGame.RENDER_SCALE
                    );

                    if (!IsCollidingWithWallScaled(scaledSlideHitboxX))
                    {
                        playerX += slideX;
                    }
                }
            }
        }

        // HÀM KIỂM TRA VA CHẠM SCALED MỚI (Dùng cho UpdateMovement)
        public bool IsCollidingWithWallScaled(RectangleF futureHitboxScaled)
        {
            // 1. Va chạm với TƯỜNG MÊ CUNG NỘI BỘ (Đã được scale trong _mazeWallRects)

            // CO NHỎ HITBOX SCALED ĐI 1 PIXEL (2 PIXEL TỔNG) ĐỂ TRÁNH LỖI LÀM TRÒN VÀ KẸT GÓC
            float shrink = 2f;
            RectangleF shrunkenHitbox = new RectangleF(
                futureHitboxScaled.X + shrink / 2f,
                futureHitboxScaled.Y + shrink / 2f,
                futureHitboxScaled.Width - shrink,
                futureHitboxScaled.Height - shrink
            );

            foreach (var wall in _mazeWallRects)
            {
                // Wall Rect đã được tạo ở kích thước 1x1 tile logic (30x30)
                // Phải SCALE kích thước Wall lên 3x trước khi kiểm tra.
                RectangleF scaledWall = new RectangleF(
                    wall.X * frmMainGame.RENDER_SCALE,
                    wall.Y * frmMainGame.RENDER_SCALE,
                    wall.Width * frmMainGame.RENDER_SCALE,
                    wall.Height * frmMainGame.RENDER_SCALE
                );

                if (shrunkenHitbox.IntersectsWith(scaledWall))
                {
                    return true;
                }
            }

            // 2. Va chạm với TƯỜNG BIÊN (BORDER WALLS) - Dựa trên kích thước Scaled

            // Va chạm tường Trái (x=0)
            if (futureHitboxScaled.Left < 0) return true;
            // Va chạm tường Trên (y=0)
            if (futureHitboxScaled.Top < 0) return true;
            // Va chạm tường Dưới (y=_mapLogicalHeightScaled)
            if (futureHitboxScaled.Bottom > _mapLogicalHeightScaled) return true;

            // Va chạm tường Phải (cửa ra)
            if (futureHitboxScaled.Right > _mapLogicalWidthScaled)
            {
                // Cửa ra nằm ở góc dưới bên phải (vị trí cuối cùng trong mê cung)

                // Giả định ô cửa ra là ô cuối cùng (được tạo sàn bởi MazeGenerator)
                float exitTileX = (_mapLogicalWidthScaled - TILE_SIZE * frmMainGame.RENDER_SCALE);
                float exitTileY = (_mapLogicalHeightScaled - TILE_SIZE * frmMainGame.RENDER_SCALE);

                // KIỂM TRA ĐIỀU KIỆN KẾT THÚC MÀN CHƠI MỚI (Dựa trên tọa độ Scaled)
                // Kiểm tra xem người chơi có nằm gần ô cửa ra không (ví dụ: trong 3 ô tile cuối cùng)
                if (futureHitboxScaled.X > exitTileX - TILE_SIZE * frmMainGame.RENDER_SCALE * 3 && futureHitboxScaled.Y > exitTileY - TILE_SIZE * frmMainGame.RENDER_SCALE * 3)
                {
                    // Lấy Boss (Giả định Boss là Monster cuối cùng)
                    Boss boss = monsters.OfType<Boss>().FirstOrDefault();

                    bool isPeacefulExit = _monstersKilled == 0;
                    bool isBattleExit = _isBossDefeated;

                    if (isPeacefulExit)
                    {
                        HandleMapExit("Hòa bình!");
                        return false; // Cho phép đi qua
                    }
                    else if (isBattleExit)
                    {
                        HandleMapExit("Chiến thắng!");
                        return false; // Cho phép đi qua
                    }
                    else if (boss != null && boss.State != Monster.MonsterState.Dead)
                    {
                        // THÔNG BÁO: Phải đánh bại Boss trước
                        MessageBox.Show("Boss vẫn còn sống! Không thể qua màn.");
                        return true; // Chặn
                    }
                }

                return true; // Chặn nếu không phải cửa ra
            }

            return false;
        }

        // HÀM ISCOLLIDINGWITHWALL BỊ BỎ (CHỈ DÙNG SCALED)
        // public bool IsCollidingWithWall(RectangleF futureHitbox) { ... }


        public bool HasLineOfSight(PointF start, PointF end)
        {
            // Logic Line of Sight vẫn dùng tọa độ logic 1:1, vì nó chỉ kiểm tra lưới
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return true;

            // Thay đổi bước nhảy thành 1/2 TILE_SIZE để tăng độ chính xác
            int checkInterval = TILE_SIZE / 2;
            int steps = (int)(distance / checkInterval);
            if (steps < 2) return true;

            float stepX = dx / steps;
            float stepY = dy / steps;

            for (int i = 1; i < steps; i++)
            {
                float checkX = start.X + stepX * i;
                float checkY = start.Y + stepY * i;

                // Kiểm tra va chạm với TƯỜNG MÊ CUNG (chỉ cần kiểm tra các ô vuông 1x1)
                // Chuyển vị trí điểm kiểm tra sang tọa độ ô (tile coordinates)
                int tileX = (int)(checkX / TILE_SIZE);
                int tileY = (int)(checkY / TILE_SIZE);

                // Tìm RectangleF tương ứng với ô tile.
                RectangleF checkRect = new RectangleF(tileX * TILE_SIZE, tileY * TILE_SIZE, TILE_SIZE, TILE_SIZE);

                // Kiểm tra xem checkRect này có phải là một bức tường trong _mazeWallRects không
                // LƯU Ý: Dùng IntersectsWith để kiểm tra chính xác hơn so với Equals
                if (_mazeWallRects.Any(wall => wall.IntersectsWith(checkRect)))
                {
                    return false;
                }
            }

            return true;
        }


        private void HandleMapExit(string message)
        {
            // Khi vượt qua cửa ra, có thể load map mới hoặc kết thúc game
            MessageBox.Show($"Đã tìm thấy cửa ra! Bạn đã hoàn thành map này ({message}).");
            gameTimer.Stop();
            this.Close();
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
        // Vị trí logic 1:1
        public float X, Y;
        public int Width, Height;
        public AnimationActivity Anim;
        public bool IsFinished => Anim.IsFinished;
        private string direction = "down";

        public SpellEffect(AnimationActivity animTemplate, float x, float y)
        {
            // KHẮC PHỤC LỖI: Tạo instance mới của AnimationActivity
            Anim = new AnimationActivity(animTemplate.AnimationSpeed) { IsLooping = false };
            Anim.LoadImages(animTemplate.BackDir, animTemplate.FrontDir, animTemplate.LeftDir, animTemplate.RightDir);

            Width = 150;
            Height = 150;
            X = x - (Width / 2); // X và Y vẫn là tọa độ logic 1:1
            Y = y - (Height / 2) - 30;
        }

        public void Update()
        {
            // KHẮC PHỤC LỖI: Không nên dùng using cho Image trong Update, chỉ cần gọi GetNextFrame
            Anim.GetNextFrame(direction);
        }

        public void Draw(Graphics canvas, int scale)
        {
            using (var image = Anim.GetNextFrame(direction))
            {
                if (image != null)
                {
                    // ÁP DỤNG SCALE KHI VẼ (SỬA LỖI CS0176 - Không áp dụng trong SpellEffect vì 'scale' được truyền vào)
                    canvas.DrawImage(image, (int)X * scale, (int)Y * scale, Width * scale, Height * scale);
                }
            }
        }
    }

    public abstract class Monster
    {
        public enum MonsterState { Idle, Patrol, Chase, Attack, Casting, Hurt, Dead, Friendly } // THÊM TRẠNG THÁI FRIENDLY
        public MonsterState State { get; protected set; }

        public float X, Y; // Vị trí logic 1:1
        public int Width { get; protected set; } // Kích thước logic 1:1
        public int Height { get; protected set; } // Kích thước logic 1:1
        public RectangleF Hitbox => new RectangleF(X, Y, Width, Height); // Hitbox logic 1:1

        // SỬA LỖI CS0176
        public RectangleF ScaledHitbox(int scale) => new RectangleF(X * scale, Y * scale, Width * scale, Height * scale);


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
            SetNewPatrolTarget();
            State = MonsterState.Idle;
        }

        protected abstract void LoadAnimations();
        protected abstract void SetStats();

        protected float GetDistanceToPlayer(float playerX, float playerY)
        {
            // Player hitbox is 25x25 starting at (playerX, playerY). Center is at (playerX + 12.5, playerY + 12.5).
            float playerCenterX = playerX + 12.5f;
            float playerCenterY = playerY + 12.5f;

            // Monster center is at (X + Width/2, Y + Height/2).
            float monsterCenterX = X + Width / 2;
            float monsterCenterY = Y + Height / 2;

            float dx = playerCenterX - monsterCenterX;
            float dy = playerCenterY - monsterCenterY;
            return (dx * dx) + (dy * dy);
        }

        public virtual void Update(frmMainGame game, float playerX, float playerY, List<SpellEffect> spellEffects)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return; // BỎ QUA NẾU FRIENDLY

            if (game == null) return; // Bảo vệ nếu game context null

            float distanceSq = GetDistanceToPlayer(playerX, playerY);

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
                // Logic tấn công ở đây (ví dụ: gây sát thương)
                if (attackAnim.IsFinished)
                {
                    State = MonsterState.Chase;
                }
                return;
            }

            if (State == MonsterState.Casting)
            {
                // Chờ cho hoạt ảnh cast kết thúc
                return;
            }

            bool canSeePlayer = false;
            if (distanceSq <= aggroRange * aggroRange)
            {
                // Kiểm tra tầm nhìn: Monster Center to Player Center (playerX + 12.5, playerY + 12.5)
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerX + 12.5f, playerY + 12.5f));
            }

            if (distanceSq <= attackRange * attackRange && canSeePlayer)
            {
                State = MonsterState.Attack;
                attackAnim.ResetFrame();
            }
            else if (distanceSq <= aggroRange * aggroRange && canSeePlayer)
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
                // Di chuyển về Player Center (playerX + 12.5, playerY + 12.5)
                MoveTowards(game, new PointF(playerX + 12.5f, playerY + 12.5f), speed);
            }
        }

        // --- SỬA: Logic va chạm trượt (Sliding Collision) ---
        protected void MoveTowards(frmMainGame game, PointF target, float speed)
        {
            if (game == null) return;

            float dx = target.X - (X + Width / 2);
            float dy = target.Y - (Y + Height / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;

                float moveX = dx * speed;
                float moveY = dy * speed;

                // 1. Thử di chuyển X
                RectangleF nextHitboxX = new RectangleF(X + moveX, Y, Width, Height);
                // SỬA LỖI CS0176: Truy cập bằng tên kiểu
                RectangleF scaledNextHitboxX = new RectangleF(
                    nextHitboxX.X * frmMainGame.RENDER_SCALE,
                    nextHitboxX.Y * frmMainGame.RENDER_SCALE,
                    nextHitboxX.Width * frmMainGame.RENDER_SCALE,
                    nextHitboxX.Height * frmMainGame.RENDER_SCALE
                );

                if (!game.IsCollidingWithWallScaled(scaledNextHitboxX))
                {
                    X += moveX;
                }

                // 2. Thử di chuyển Y
                RectangleF nextHitboxY = new RectangleF(X, Y + moveY, Width, Height);
                // SỬA LỖI CS0176: Truy cập bằng tên kiểu
                RectangleF scaledNextHitboxY = new RectangleF(
                    nextHitboxY.X * frmMainGame.RENDER_SCALE,
                    nextHitboxY.Y * frmMainGame.RENDER_SCALE,
                    nextHitboxY.Width * frmMainGame.RENDER_SCALE,
                    nextHitboxY.Height * frmMainGame.RENDER_SCALE
                );

                if (!game.IsCollidingWithWallScaled(scaledNextHitboxY))
                {
                    Y += moveY;
                }

                // 3. Cập nhật hướng nhìn
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

        // --- HÀM TẠO MỤC TIÊU PATROL KHÔNG KIỂM TRA VA CHẠM (chỉ dùng cho init) ---
        protected void SetNewPatrolTarget()
        {
            int range = 150;
            float targetX = patrolOrigin.X + rand.Next(-range, range);
            float targetY = patrolOrigin.Y + rand.Next(-range, range);
            patrolTarget = new PointF(targetX, targetY);
        }

        // --- HÀM TẠO MỤC TIÊU PATROL CÓ KIỂM TRA VA CHẠM ---
        protected void SetNewPatrolTarget(frmMainGame game)
        {
            if (game == null) return;

            int range = 150;
            // Hitbox logic
            RectangleF testHitbox = new RectangleF(X, Y, Width, Height);

            for (int i = 0; i < 5; i++)
            {
                float targetX = patrolOrigin.X + rand.Next(-range, range);
                float targetY = patrolOrigin.Y + rand.Next(-range, range);

                testHitbox.X = targetX;
                testHitbox.Y = targetY;

                // Scaled Hitbox (SỬA LỖI CS0176)
                RectangleF scaledTestHitbox = new RectangleF(
                    testHitbox.X * frmMainGame.RENDER_SCALE,
                    testHitbox.Y * frmMainGame.RENDER_SCALE,
                    testHitbox.Width * frmMainGame.RENDER_SCALE,
                    testHitbox.Height * frmMainGame.RENDER_SCALE
                );

                // CHỈ kiểm tra va chạm.
                if (!game.IsCollidingWithWallScaled(scaledTestHitbox))
                {
                    patrolTarget = new PointF(targetX, targetY);
                    return;
                }
            }

            patrolTarget = patrolOrigin;
        }

        public virtual void TakeDamage(int damage)
        {
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return; // BẢO VỆ

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

        public virtual void Draw(Graphics canvas, int scale)
        {
            Image imageToDraw = null;

            switch (State)
            {
                case MonsterState.Idle:
                case MonsterState.Friendly: // DÙNG IDLE CHO FRIENDLY
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
                    // LƯU Ý: DrawWidth/Height được cố định hoặc dựa trên kích thước Vẽ lớn hơn Hitbox.
                    // Vẫn cần dùng X, Y đã được SCALE
                    int drawWidth = Width;
                    int drawHeight = Height;
                    float drawX = X * scale - (drawWidth - Width) / 2; // Dùng float X
                    float drawY = Y * scale - (drawHeight - Height) / 2; // Dùng float Y
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            // VẼ THANH MÁU VÀ LABEL FRIENDLY
            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead && State != MonsterState.Friendly)
            {
                DrawHealthBar(canvas, scale);
            }
            else if (State == MonsterState.Friendly)
            {
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale - 10, Y * scale - 20);
            }
        }

        protected void DrawHealthBar(Graphics canvas, int scale)
        {
            int barWidth = Width * scale;
            int barHeight = 10;
            float barY = Y * scale - barHeight - 5;
            float barX = X * scale;

            canvas.FillRectangle(Brushes.Black, barX, barY, barWidth, barHeight);

            float healthPercentage = (float)Health / MaxHealth;
            int currentHealthWidth = (int)(barWidth * healthPercentage);

            canvas.FillRectangle(Brushes.LawnGreen, barX, barY, currentHealthWidth, barHeight);

            canvas.DrawRectangle(Pens.Black, barX, barY, barWidth, barHeight);
        }
    }

    public class Slime : Monster
    {
        // ... [Nội dung Slime không đổi] ...
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
            deathAnim = new AnimationActivity(7) { IsLooping = false };
            deathAnim.LoadImages(Path.Combine(deathRoot, "Back"), Path.Combine(deathRoot, "Front"), Path.Combine(deathRoot, "Left"), Path.Combine(deathRoot, "Right"));
        }

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        public override void Draw(Graphics canvas, int scale)
        {
            Image imageToDraw = null;
            string currentDirection = (State == MonsterState.Friendly) ? "down" : facingDirection;

            switch (State)
            {
                case MonsterState.Idle:
                case MonsterState.Friendly:
                    imageToDraw = idleAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Casting:
                    break; // Slime không có Cast
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(currentDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    // LƯU Ý: Slime vẽ lớn hơn hitbox 40x40. Kích thước vẽ không cần scale 3x
                    int drawWidth = 80;
                    int drawHeight = 80;
                    // VỊ TRÍ DRAW ĐÃ ĐƯỢC SCALE
                    float drawX = X * scale - (drawWidth - Width) / 2;
                    float drawY = Y * scale - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead && State != MonsterState.Friendly)
            {
                DrawHealthBar(canvas, scale);
            }
            else if (State == MonsterState.Friendly)
            {
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale - 10, Y * scale - 20);
            }
        }
    }

    public class Orc : Monster
    {
        // ... [Nội dung Orc không đổi] ...
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

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        public override void Draw(Graphics canvas, int scale)
        {
            Image imageToDraw = null;
            string currentDirection = (State == MonsterState.Friendly) ? "down" : facingDirection;

            switch (State)
            {
                case MonsterState.Idle:
                case MonsterState.Friendly:
                    imageToDraw = idleAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Casting:
                    break; // Orc không có Cast
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(currentDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    // LƯU Ý: Orc vẽ lớn hơn hitbox 45x45
                    int drawWidth = 110;
                    int drawHeight = 110;
                    // VỊ TRÍ DRAW ĐÃ ĐƯỢC SCALE
                    float drawX = X * scale - (drawWidth - Width) / 2;
                    float drawY = Y * scale - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead && State != MonsterState.Friendly)
            {
                DrawHealthBar(canvas, scale);
            }
            else if (State == MonsterState.Friendly)
            {
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale - 10, Y * scale - 20);
            }
        }
    }

    public class Boss : Monster
    {
        private int castCooldown = 0;
        private int castCooldownDuration = 333;

        // ... [Nội dung Boss không đổi] ...
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
            // SỬA LỖI CS1061: Thay thế 'walkAnim.DrawImages' bằng 'walkAnim.RightDir'
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
            if (State == MonsterState.Dead || State == MonsterState.Friendly) return; // BẢO VỆ

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

            // KIỂM TRA ĐIỀU KIỆN FRIENDLY (CHỈ ÁP DỤNG TRONG BOSS.UPDATE)
            if (game._monstersKilled == 0 && State != MonsterState.Dead)
            {
                State = MonsterState.Friendly;
                return; // KHÔNG THỰC HIỆN LOGIC CHIẾN ĐẤU
            }
            else if (State == MonsterState.Friendly && game._monstersKilled > 0)
            {
                // Nếu đã giết quái vật, Boss chuyển sang chế độ Chiến đấu
                State = MonsterState.Idle;
            }

            if (State == MonsterState.Friendly) return; // BỎ QUA LOGIC DƯỚI NẾU FRIENDLY

            if (castCooldown > 0) castCooldown--;

            float distanceSq = GetDistanceToPlayer(playerX, playerY);

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
                    // Spell effect at Player Center (playerX + 12.5, playerY + 12.5)
                    spellEffects.Add(new SpellEffect(spellAnim, playerX + 12.5f, playerY + 12.5f));

                    State = MonsterState.Idle;
                    patrolTimer = 0;
                    castCooldown = castCooldownDuration;
                }
                return;
            }

            bool canSeePlayer = false;
            if (distanceSq <= aggroRange * aggroRange)
            {
                // Kiểm tra tầm nhìn: Monster Center to Player Center (playerX + 12.5, playerY + 12.5)
                canSeePlayer = game.HasLineOfSight(new PointF(X + Width / 2, Y + Height / 2), new PointF(playerX + 12.5f, playerY + 12.5f));
            }

            if (castCooldown == 0 && canSeePlayer)
            {
                State = MonsterState.Casting;
                castAnim.ResetFrame();
            }
            else if (distanceSq <= attackRange * attackRange && canSeePlayer)
            {
                State = MonsterState.Attack;
                attackAnim.ResetFrame();
            }
            else if (distanceSq <= aggroRange * aggroRange && canSeePlayer)
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
                // Di chuyển về Player Center (playerX + 12.5, playerY + 12.5)
                MoveTowards(game, new PointF(playerX + 12.5f, playerY + 12.5f), speed);
            }

            this.facingDirection = "down";
        }

        // CHỈNH SỬA DRAW ĐỂ CHẤP NHẬN SCALE VÀ VẼ LỚN HƠN
        public override void Draw(Graphics canvas, int scale)
        {
            Image imageToDraw = null;
            string currentDirection = (State == MonsterState.Friendly) ? "down" : facingDirection; // Boss Friendly luôn nhìn xuống

            switch (State)
            {
                case MonsterState.Idle:
                case MonsterState.Friendly:
                    imageToDraw = idleAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Patrol:
                    imageToDraw = walkAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Chase:
                    imageToDraw = runAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Attack:
                    imageToDraw = attackAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Casting:
                    imageToDraw = castAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Hurt:
                    imageToDraw = hurtAnim.GetNextFrame(currentDirection);
                    break;
                case MonsterState.Dead:
                    imageToDraw = deathAnim.GetNextFrame(currentDirection);
                    break;
            }

            if (imageToDraw != null)
            {
                using (imageToDraw)
                {
                    int drawWidth = 150;
                    int drawHeight = 150;
                    // VỊ TRÍ DRAW ĐÃ ĐƯỢC SCALE
                    float drawX = X * scale - (drawWidth - Width) / 2;
                    float drawY = Y * scale - (drawHeight - Height) / 2;
                    canvas.DrawImage(imageToDraw, drawX, drawY, drawWidth, drawHeight);
                }
            }

            if (State != MonsterState.Idle && State != MonsterState.Patrol && State != MonsterState.Dead && State != MonsterState.Friendly)
            {
                DrawHealthBar(canvas, scale);
            }
            else if (State == MonsterState.Friendly)
            {
                canvas.DrawString("FRIENDLY", new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold), Brushes.LightGreen, X * scale + 20, Y * scale - 10);
            }
        }

    }

}
// KHÔNG CÓ CODE NÀO KHÁC SAU DÒNG NÀY ĐỂ TRÁNH TRÙNG LẶP NAMESPACE VÀ CLASS
