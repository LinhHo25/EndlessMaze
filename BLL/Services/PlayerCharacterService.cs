using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity; // Cần cho .Include()
using System.Data.Entity.Validation; // <-- THÊM: Để bắt lỗi chi tiết
using System.Linq;

namespace BLL.Services
{
    // Đã thay đổi từ 'internal' thành 'public'
    public class PlayerCharacterService
    {
        private ContextDB db = new ContextDB();

        // ... (Hàm GetCharactersByUserId không đổi) ...
        public List<PlayerCharacters> GetCharactersByUserId(int userId)
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
                return new List<PlayerCharacters>();
            }
        }

        // ... (Hàm GetCharacterDetails không đổi) ...
        public PlayerCharacters GetCharacterDetails(int characterId)
        {
            try
            {
                // Sử dụng .Include() để tải thông tin liên quan từ các bảng khác
                return db.PlayerCharacters
                         .Include(c => c.PlayerSessionInventory.Select(inv => inv.Weapons))
                         .Include(c => c.PlayerSessionInventory.Select(inv => inv.Armors))
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
        /// --- SỬA: Trả về STRING (lỗi) thay vì BOOL ---
        /// </summary>
        public string CreateCharacter(int userId, string characterName)
        {
            // --- SỬA: Thêm kiểm tra tên trùng lặp ---
            if (db.PlayerCharacters.Any(c => c.CharacterName == characterName && c.UserID == userId))
            {
                return $"Tên nhân vật '{characterName}' đã tồn tại.";
            }

            // --- SỬA LỖI (theo ảnh): Kiểm tra ràng buộc UNIQUE KEY trên UserID ---
            // Lỗi "Violation of UNIQUE KEY constraint... The duplicate key value is (3)."
            // cho thấy CSDL của bạn có 1 ràng buộc UNIQUE trên cột UserID,
            // ngăn cản 1 user có nhiều hơn 1 nhân vật.
            if (db.PlayerCharacters.Any(c => c.UserID == userId))
            {
                return $"Lỗi Thiết kế CSDL: Người dùng (ID: {userId}) này đã có nhân vật. Ràng buộc UNIQUE KEY của CSDL ngăn cản việc tạo thêm nhân vật. (Bạn chỉ có thể có 1 nhân vật mỗi tài khoản).";
            }
            // --- KẾT THÚC SỬA LỖI ---


            // (Sử dụng Transaction để đảm bảo an toàn)
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // --- SỬA: Tìm hoặc Tạo Trang bị Mặc định ---
                    // 1. Vũ khí
                    Weapons defaultWeapon = db.Weapons.FirstOrDefault(w => w.WeaponName == "Tay không");
                    if (defaultWeapon == null)
                    {
                        // --- SỬA LỖI: Đổi "F" thành "D" (do CSDL không chấp nhận "F") ---
                        defaultWeapon = new Weapons { WeaponName = "Tay không", WeaponRank = "D", AttackBonus = 1 };
                        db.Weapons.Add(defaultWeapon);
                        db.SaveChanges(); // Lưu để lấy ID
                    }

                    // 2. Áo giáp
                    Armors defaultArmor = db.Armors.FirstOrDefault(a => a.ArmorName == "Áo vải");
                    if (defaultArmor == null)
                    {
                        // --- SỬA LỖI: Đổi "F" thành "D" (để đồng bộ) ---
                        defaultArmor = new Armors { ArmorName = "Áo vải", ArmorRank = "D", DefensePoints = 1 };
                        db.Armors.Add(defaultArmor);
                        db.SaveChanges(); // Lưu để lấy ID
                    }
                    // --- KẾT THÚC SỬA ---


                    // 1. Tạo nhân vật
                    PlayerCharacters newChar = new PlayerCharacters
                    {
                        UserID = userId,
                        CharacterName = characterName,
                        BaseHealth = 100, // Chỉ số mặc định
                        BaseAttack = 10,  // Chỉ số mặc định
                        BaseDefense = 5,    // Chỉ số mặc định
                        BaseStamina = 50    // Chỉ số mặc định
                    };
                    db.PlayerCharacters.Add(newChar);
                    db.SaveChanges(); // Lưu để lấy được CharacterID

                    // 2. Tạo inventory mặc định cho nhân vật này
                    PlayerSessionInventory newInventory = new PlayerSessionInventory
                    {
                        CharacterID = newChar.CharacterID,
                        // --- SỬA: Dùng ID động thay vì '1' ---
                        EquippedWeaponID = defaultWeapon.WeaponID,
                        EquippedArmorID = defaultArmor.ArmorID,
                        HealthPotionCount = 3 // 3 bình máu khởi điểm
                    };
                    db.PlayerSessionInventory.Add(newInventory);
                    db.SaveChanges();

                    // Mọi thứ thành công
                    transaction.Commit();
                    return null; // <-- SỬA: Trả về NULL (không có lỗi)
                }
                catch (DbEntityValidationException dbEx) // <-- THÊM: Bắt lỗi CSDL chi tiết
                {
                    transaction.Rollback();
                    var errorMessages = dbEx.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                    return "Lỗi CSDL: " + string.Join("; ", errorMessages);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // --- SỬA: Trả về lỗi GỐC (InnerException) ---
                    string innerError = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                    return "Lỗi Hệ thống: " + innerError;
                }
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
                var inventory = db.PlayerSessionInventory
                                    .Where(inv => inv.CharacterID == characterId)
                                    .ToList();

                db.PlayerSessionInventory.RemoveRange(inventory);
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

        // --- THÊM HÀM MỚI: RESET TRANG BỊ ---
        /// <summary>
        /// Reset trang bị của nhân vật về mặc định (Giáp D, Kiếm D, 3 Thuốc)
        /// </summary>
        public bool ResetCharacterToDefault(int characterId)
        {
            try
            {
                var inventory = db.PlayerSessionInventory.FirstOrDefault(i => i.CharacterID == characterId);
                if (inventory == null) return false;

                // 1. Tìm trang bị Rank D
                // (Giả sử tên mặc định là "Tay không" và "Áo vải" như trong CreateCharacter)
                var defaultWeapon = db.Weapons.FirstOrDefault(w => w.WeaponName == "Tay không" && w.WeaponRank == "D");
                var defaultArmor = db.Armors.FirstOrDefault(a => a.ArmorName == "Áo vải" && a.ArmorRank == "D");

                // Nếu không tìm thấy (lỗi), dùng ID=1 làm dự phòng
                int weaponId = defaultWeapon?.WeaponID ?? 1;
                int armorId = defaultArmor?.ArmorID ?? 1;

                // 2. Cập nhật Inventory
                inventory.EquippedWeaponID = weaponId;
                inventory.EquippedArmorID = armorId;
                inventory.HealthPotionCount = 3; // Số thuốc mặc định

                // 3. Lưu
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi reset trang bị: " + ex.Message);
                return false;
            }
        }
    }
}

