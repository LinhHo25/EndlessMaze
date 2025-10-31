using DAL.Models;
using System;
using System.Linq;
using System.Data.Entity; // Cần cho .Include()

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
        /// Tính toán tổng chỉ số (cơ bản + trang bị).
        /// </summary>
        public CalculatedStats GetCalculatedStats(int characterId)
        {
            try
            {
                var character = db.PlayerCharacters.Find(characterId);
                var inventory = db.PlayerSessionInventories
                                  .Include(inv => inv.Weapon)
                                  .Include(inv => inv.Armor)
                                  .FirstOrDefault(inv => inv.CharacterID == characterId);

                if (character == null || inventory == null) return null;

                // Lấy chỉ số từ trang bị (sử dụng '?? 0' để tránh lỗi Null)
                int weaponAttack = inventory.Weapon?.AttackBonus ?? 0;
                int armorDefense = inventory.Armor?.DefensePoints ?? 0;

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

        /// <summary>
        /// Sử dụng một bình máu.
        /// </summary>
        /// <param name="characterId">ID nhân vật</param>
        /// <param name="currentHealth">Máu hiện tại của nhân vật</param>
        /// <returns>Máu mới sau khi hồi, hoặc máu hiện tại nếu không dùng được.</returns>
        public int UseHealthPotion(int characterId, int currentHealth)
        {
            try
            {
                var inventory = db.PlayerSessionInventories.FirstOrDefault(inv => inv.CharacterID == characterId);
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
                var inventory = db.PlayerSessionInventories.FirstOrDefault(inv => inv.CharacterID == characterId);
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
                var inventory = db.PlayerSessionInventories.FirstOrDefault(inv => inv.CharacterID == characterId);
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
