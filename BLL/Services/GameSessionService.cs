using DAL.Models;
using System;
using System.Data.Entity; // Cần cho .Include()
using System.Linq;

namespace BLL.Services
{
    // Đã thay đổi từ 'internal' thành 'public'
    /// <summary>
    /// Quản lý logic của một phiên chơi game đang hoạt động.
    /// Tính toán chỉ số, chiến đấu, sử dụng vật phẩm.
    /// </summary>
    public class GameSessionService
    {
        private ContextDB db = new ContextDB();

        // DTO (Data Transfer Object) để giữ các chỉ số đã tính toán
        public class CalculatedStats
        {
            public int TotalHealth { get; set; }
            public int TotalAttack { get; set; }
            public int TotalDefense { get; set; }
        }

        /// <summary>
        /// Tính toán tổng chỉ số (cơ bản + trang bị) (Lấy từ DB).
        /// </summary>
        public CalculatedStats GetCalculatedStats(int characterId)
        {
            try
            {
                var character = db.PlayerCharacters.Find(characterId);
                var inventory = db.PlayerSessionInventory
                                    .Include(inv => inv.Weapons)
                                    .Include(inv => inv.Armors)
                                    .FirstOrDefault(inv => inv.CharacterID == characterId);

                if (character == null || inventory == null) return null;

                // Lấy chỉ số từ trang bị (sử dụng '?? 0' để tránh lỗi Null)
                int weaponAttack = inventory.Weapons?.AttackBonus ?? 0;
                int armorDefense = inventory.Armors?.DefensePoints ?? 0;

                return new CalculatedStats
                {
                    TotalHealth = character.BaseHealth, // (Bạn có thể cộng thêm máu từ giáp nếu muốn)
                    TotalAttack = character.BaseAttack + weaponAttack,
                    TotalDefense = character.BaseDefense + armorDefense
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // --- THÊM: Hàm mới cho frmMazeGame (dùng dữ liệu có sẵn) ---
        /// <summary>
        /// Tính toán tổng chỉ số (cơ bản + trang bị) TỪ DỮ LIỆU CÓ SẴN.
        /// (Dùng cho frmMazeGame để tránh gọi DB khi pause)
        /// </summary>
        public CalculatedStats CalculateStats(PlayerCharacters character, PlayerSessionInventory inventory)
        {
            try
            {
                if (character == null) return null;

                // (inventory có thể null nếu code PauseGame bị lỗi, nhưng ta cứ kiểm tra)
                int weaponAttack = inventory?.Weapons?.AttackBonus ?? 0;
                int armorDefense = inventory?.Armors?.DefensePoints ?? 0;

                return new CalculatedStats
                {
                    TotalHealth = character.BaseHealth,
                    TotalAttack = character.BaseAttack + weaponAttack,
                    TotalDefense = character.BaseDefense + armorDefense
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        // --- KẾT THÚC THÊM ---


        /// <summary>
        /// Sử dụng một bình máu.
        /// </summary>
        // ... (Phần còn lại của tệp không đổi) ...
        public int UseHealthPotion(int characterId, int currentHealth)
        {
            try
            {
                var inventory = db.PlayerSessionInventory.FirstOrDefault(inv => inv.CharacterID == characterId);
                if (inventory == null || inventory.HealthPotionCount <= 0)
                {
                    return currentHealth; // Không có máu để dùng
                }

                // Giả sử Potion ID 1 là bình máu cơ bản
                var potion = db.Potions.Find(1);
                var character = db.PlayerCharacters.Find(characterId);
                if (potion == null || character == null) return currentHealth;

                // Không cho hồi máu nếu đã đầy
                if (currentHealth >= character.BaseHealth) return currentHealth;

                // Trừ 1 bình máu và lưu
                inventory.HealthPotionCount--;
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();

                // Tính máu mới
                int newHealth = currentHealth + potion.HealAmount;
                // Không cho hồi vượt quá máu tối đa
                if (newHealth > character.BaseHealth)
                {
                    newHealth = character.BaseHealth;
                }

                return newHealth;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return currentHealth;
            }
        }

        /// <summary>
        /// Trang bị vũ khí mới.
        /// </summary>
        public bool EquipWeapon(int characterId, int weaponId)
        {
            try
            {
                // (Bạn có thể thêm logic kiểm tra xem nhân vật có sở hữu vũ khí này không)
                var inventory = db.PlayerSessionInventory.FirstOrDefault(inv => inv.CharacterID == characterId);
                if (inventory == null) return false;

                inventory.EquippedWeaponID = weaponId;
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Trang bị giáp mới.
        /// </summary>
        public bool EquipArmor(int characterId, int armorId)
        {
            try
            {
                var inventory = db.PlayerSessionInventory.FirstOrDefault(inv => inv.CharacterID == characterId);
                if (inventory == null) return false;

                inventory.EquippedArmorID = armorId;
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

