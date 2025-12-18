using System.Security.Claims;

namespace StyloApp.API.Core.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // Lấy giá trị NameIdentifier từ Token và ép kiểu về int
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId != null ? int.Parse(userId) : 0;
        }
    }
}
