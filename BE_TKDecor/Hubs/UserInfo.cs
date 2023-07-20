using System.Security.Claims;

namespace BE_TKDecor.Hubs
{
    public class UserInfo
    {
        public Guid UserId { get; }

        public UserInfo(HttpContext httpContext)
        {
            UserId = GetUserIdFromToken(httpContext);
        }

        private Guid GetUserIdFromToken(HttpContext httpContext)
        {
            // Lấy thông tin từ Claims trong HttpContext để xác thực token
            var claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst("UserId");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            // Trường hợp không tìm thấy UserId hoặc có lỗi xảy ra, bạn có thể xử lý phù hợp ở đây.
            // Ví dụ, quăng một ngoại lệ hoặc trả về giá trị mặc định.

            return Guid.Empty; // Hoặc trả về giá trị mặc định phù hợp trong trường hợp không tìm thấy UserId.
        }
    }

}
