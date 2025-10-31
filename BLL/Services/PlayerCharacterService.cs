using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; // Cần cho .Include()

namespace BLL.Services
{
    // Đã thay đổi từ 'internal' thành 'public'
    public class PlayerCharacterService
    {
        private ContextDB db = new ContextDB();

        /// <summary>
        /// Lấy tất cả các nhân vật của một người dùng.
        /// </summary>
        public List<PlayerCharacter> GetCharactersByUserId(int userId)
        {
            try
            {
                return db.PlayerCharacters
                         .Where(c => c.UserID == userId)
                         .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<PlayerCharacter>();
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của MỘT nhân vật, bao gồm cả túi đồ.
        /// </summary>
        public PlayerCharacter GetCharacterDetails(int characterId)
        {
            try
            {
                // Sử dụng .Include() để tải thông tin liên quan từ các bảng khác
                return db.PlayerCharacters
                         .Include(c => c.PlayerSessionInventories.Select(inv => inv.Weapon))
                         .Include(c => c.PlayerSessionInventories.Select(inv => inv.Armor))
                         .FirstOrDefault(c => c.CharacterID == characterId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Tạo nhân vật mới với chỉ số cơ bản và inventory mặc định.
        /// </summary>
        public bool CreateCharacter(int userId, string characterName)
        {
            try
            {
                // 1. Tạo nhân vật
                PlayerCharacter newChar = new PlayerCharacter
                {
                    UserID = userId,
                    CharacterName = characterName,
                    BaseHealth = 100, // Chỉ số mặc định
                    BaseAttack = 10,  // Chỉ số mặc định
                    BaseDefense = 5,   // Chỉ số mặc định
                    BaseStamina = 50   // Chỉ số mặc định
                };
                db.PlayerCharacters.Add(newChar);
                db.SaveChanges(); // Lưu để lấy được CharacterID

                // 2. Tạo inventory mặc định cho nhân vật này
                // (Giả sử ID 1 là "Tay không" hoặc "Dao găm", ID 1 là "Áo vải")
                PlayerSessionInventory newInventory = new PlayerSessionInventory
                {
                    CharacterID = newChar.CharacterID,
                    EquippedWeaponID = 1, // ID Vũ khí mặc định
                    EquippedArmorID = 1,  // ID Giáp mặc định
                    HealthPotionCount = 3 // 3 bình máu khởi điểm
                };
                db.PlayerSessionInventories.Add(newInventory);
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
        /// Xóa một nhân vật (và inventory liên quan - CSDL sẽ tự xử lý nếu có Cascade Delete)
        /// </summary>
        public bool DeleteCharacter(int characterId)
        {
            try
            {
                var character = db.PlayerCharacters.Find(characterId);
                if (character == null) return false;

                // Cũng nên xóa inventory liên quan (nếu CSDL không tự xóa)
                var inventory = db.PlayerSessionInventories
                                  .Where(inv => inv.CharacterID == characterId)
                                  .ToList();

                db.PlayerSessionInventories.RemoveRange(inventory);
                db.PlayerCharacters.Remove(character);

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
