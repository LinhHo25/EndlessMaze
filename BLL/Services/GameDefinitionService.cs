using DAL.Models; // <-- THÊM: Sử dụng Models
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Lớp public để GUI có thể truy cập
    public class GameDefinitionService
    {
        // Khởi tạo DbContext
        private readonly ContextDB _context;

        public GameDefinitionService()
        {
            _context = new ContextDB();
        }

        // --- CÁC HÀM TẢI DỮ LIỆU ĐỊNH NGHĨA GAME ---

        /// <summary>
        /// Lấy tất cả quái vật từ CSDL
        /// </summary>
        public List<Monster> GetAllMonsters()
        {
            try
            {
                return _context.Monsters.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách quái vật: " + ex.Message);
                return new List<Monster>();
            }
        }

        /// <summary>
        /// Lấy tất cả vũ khí từ CSDL
        /// </summary>
        public List<Weapon> GetAllWeapons()
        {
            try
            {
                return _context.Weapons.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách vũ khí: " + ex.Message);
                return new List<Weapon>();
            }
        }

        /// <summary>
        /// Lấy tất cả áo giáp từ CSDL
        /// </summary>
        public List<Armor> GetAllArmors()
        {
            try
            {
                return _context.Armors.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách áo giáp: " + ex.Message);
                return new List<Armor>();
            }
        }

        // --- HÀM MỚI ĐỂ SỬA LỖI ---

        /// <summary>
        /// Lấy thông tin một vũ khí bằng ID
        /// </summary>
        public Weapon GetWeaponById(int weaponId)
        {
            try
            {
                // Sử dụng Find để tìm theo khóa chính, nhanh hơn
                return _context.Weapons.Find(weaponId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tìm vũ khí ID " + weaponId + ": " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy thông tin một áo giáp bằng ID
        /// </summary>
        public Armor GetArmorById(int armorId)
        {
            try
            {
                // Sử dụng Find để tìm theo khóa chính
                return _context.Armors.Find(armorId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tìm áo giáp ID " + armorId + ": " + ex.Message);
                return null;
            }
        }

        // --- KẾT THÚC HÀM MỚI ---

        // (Bạn có thể thêm các hàm khác như GetPotionById, GetLootTables... ở đây)
    }
}

