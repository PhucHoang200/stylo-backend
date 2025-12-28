namespace StyloApp.API.DTOs
{
    public class LoginResponseDto
    {
        public int TaiKhoanId { get; set; }
        public int? KhachHangId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
