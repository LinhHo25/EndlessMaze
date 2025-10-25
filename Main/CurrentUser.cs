using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    // Lớp tĩnh đơn giản để lưu thông tin người dùng đang đăng nhập
    public static class CurrentUser
    {
        // Sử dụng Username thay vì UserId để khớp với logic đăng ký/đăng nhập hiện tại
        public static string Username { get; set; }
        public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);

        public static void Logout()
        {
            Username = null;
        }
    }
}

