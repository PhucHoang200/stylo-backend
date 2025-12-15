using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StyloApp.API.Data;
using StyloApp.API.DTOs;
using StyloApp.API.Entities;

namespace StyloApp.API.Services
{
    public class AuthService
    {
        private readonly FashionShopContext _context;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;
        private readonly PasswordHasher<TaiKhoan> _hasher = new();

        public AuthService(
            FashionShopContext context,
            IMemoryCache cache,
            EmailService emailService)
        {
            _context = context;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (await _context.TaiKhoans.AnyAsync(x => x.TenDangNhap == dto.Email))
                throw new Exception("Email already exists");

            // 1. Tạo tài khoản
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = dto.Email,
                EmailConfirmed = false,
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            taiKhoan.MatKhauHash =
                _hasher.HashPassword(taiKhoan, dto.Password);

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // 2. Tạo khách hàng
            _context.KhachHangs.Add(new KhachHang
            {
                HoTen = dto.FullName,
                Email = dto.Email,
                TaiKhoanId = taiKhoan.TaiKhoanId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // 3. Sinh OTP
            var otp = new Random().Next(1000, 9999).ToString();

            _cache.Set(
                $"OTP_{dto.Email}",
                otp,
                TimeSpan.FromMinutes(5)
            );

            // 4. Gửi email
            await _emailService.SendOtpAsync(dto.Email, otp);
        }
        public async Task VerifyOtpAsync(VerifyOtpDto dto)
        {
            if (!_cache.TryGetValue($"OTP_{dto.Email}", out string? cachedOtp))
                throw new Exception("OTP expired");

            if (cachedOtp != dto.Code)
                throw new Exception("Invalid OTP");

            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == dto.Email);

            if (user == null)
                throw new Exception("User not found");

            user.EmailConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;

            _cache.Remove($"OTP_{dto.Email}");
            await _context.SaveChangesAsync();
        }
        public async Task ResendOtpAsync(string email)
        {
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == email);

            if (user == null)
                throw new Exception("User not found");

            if (user.EmailConfirmed)
                throw new Exception("Email already verified");

            var otp = new Random().Next(1000, 9999).ToString();

            _cache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(5));
            await _emailService.SendOtpAsync(email, otp);
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var taiKhoan = await _context.TaiKhoans
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.TenDangNhap == dto.Email);

            if (taiKhoan == null)
                throw new Exception("Account not found");

            if (!taiKhoan.EmailConfirmed)
                throw new Exception("Email is not verified");

            var result = _hasher.VerifyHashedPassword(
                taiKhoan,
                taiKhoan.MatKhauHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid password");

            return new LoginResponseDto
            {
                TaiKhoanId = taiKhoan.TaiKhoanId,
                Email = taiKhoan.TenDangNhap,
                Role = taiKhoan.Role?.Name ?? "Customer"
            };
        }
        public async Task ForgotPasswordAsync(string email)
        {
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == email);

            if (taiKhoan == null)
                throw new Exception("Account not found");

            if (!taiKhoan.EmailConfirmed)
                throw new Exception("Email is not verified");

            // Sinh OTP
            var otp = new Random().Next(1000, 9999).ToString();

            _cache.Set(
                $"RESET_OTP_{email}",
                otp,
                TimeSpan.FromMinutes(5)
            );

            await _emailService.SendOtpAsync(email, otp);
        }
        public void VerifyResetOtp(string email, string code)
        {
            if (!_cache.TryGetValue($"RESET_OTP_{email}", out string? cachedOtp))
                throw new Exception("OTP expired");

            if (cachedOtp != code)
                throw new Exception("Invalid OTP");

            _cache.Remove($"RESET_OTP_{email}");
        }
        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == dto.Email);

            if (taiKhoan == null)
                throw new Exception("Account not found");

            taiKhoan.MatKhauHash =
                _hasher.HashPassword(taiKhoan, dto.NewPassword);

            taiKhoan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


    }
}
