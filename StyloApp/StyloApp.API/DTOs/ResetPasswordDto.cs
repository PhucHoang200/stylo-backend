namespace StyloApp.API.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
