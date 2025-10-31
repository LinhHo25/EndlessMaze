using DAL.Models;
using System;
using System.Linq;
using System.Security.Cryptography; // Cần thiết để băm mật khẩu
using System.Text;

namespace BLL.Services
{
    // Đã thay đổi từ 'internal' thành 'public' để GUI có thể truy cập
    public class AuthService
    {
        private ContextDB db = new ContextDB();

        /// <summary>
        /// Đăng nhập người dùng.
        /// </summary>
        /// <param name="username">Tên người dùng</param>
        /// <param name="password">Mật khẩu (dạng text thô)</param>
        /// <returns>Đối tượng Users nếu thành công, null nếu thất bại.</returns>
        public User Login(string username, string password)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return null; // Không tìm thấy người dùng
                }

                // Xác thực mật khẩu
                if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    return user; // Đăng nhập thành công
                }

                return null; // Sai mật khẩu
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới.
        /// </summary>
        /// <param name="username">Tên người dùng</param>
        /// <param name="password">Mật khẩu (dạng text thô)</param>
        /// <returns>True nếu đăng ký thành công, False nếu tên đã tồn tại.</returns>
        public bool Register(string username, string password)
        {
            try
            {
                // Kiểm tra xem tên người dùng đã tồn tại chưa
                if (db.Users.Any(u => u.Username == username))
                {
                    return false; // Tên đã tồn tại
                }

                // Băm mật khẩu
                string hashedPassword = PasswordHelper.HashPassword(password);

                // Tạo người dùng mới
                User newUser = new User
                {
                    Username = username,
                    PasswordHash = hashedPassword,
                    DateCreated = DateTime.Now
                };

                db.Users.Add(newUser);
                db.SaveChanges(); // Lưu vào CSDL

                return true; // Đăng ký thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        // Lớp nội bộ để xử lý băm mật khẩu
        // **LƯU Ý QUAN TRỌNG**: Đây là một ví dụ SHA256 đơn giản.
        // Trong ứng dụng thực tế, bạn NÊN sử dụng thư viện như
        // BCrypt.Net hoặc PBKDF2 để băm mật khẩu (salt and hash).
        private static class PasswordHelper
        {
            public static string HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }

            public static bool VerifyPassword(string password, string storedHash)
            {
                string hashOfInput = HashPassword(password);
                return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, storedHash) == 0;
            }
        }
    }
}
