using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using StyloApp.API.Core.Exceptions;
using StyloApp.API.Data;
using StyloApp.API.DTOs;
using StyloApp.API.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StyloApp.API.Services
{
    public class AuthService
    {
        private readonly FashionShopContext _context;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;
        private readonly PasswordHasher<TaiKhoan> _hasher = new();
        // Khai báo lại key giống hệt bên Program.cs
        private readonly string _jwtSecret = "Day_La_Chuoi_Bi_Mat_Sieu_Cap_Vip_Pro_Cua_Stylo_App_Nam_2025_Ngon_Lanh_Canh_Dao";

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
                throw new ConflictException("Email này đã được sử dụng.");

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
                throw new Exception("Mã OTP đã hết hạn hoặc không tồn tại.");

            if (cachedOtp != dto.Code)
                throw new Exception("Mã OTP không chính xác.");

            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == dto.Email);

            if (user == null)
                throw new Exception("Không tìm thấy tài khoản người dùng.");

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
                throw new Exception("Tài khoản không tồn tại trên hệ thống.");

            if (user.EmailConfirmed)
                throw new Exception("Email này đã được xác thực trước đó.");

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
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");

            if (!taiKhoan.EmailConfirmed)
                throw new Exception("Tài khoản chưa được kích hoạt qua Email.");

            var result = _hasher.VerifyHashedPassword(
                taiKhoan,
                taiKhoan.MatKhauHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoan.TaiKhoanId);

            var token = GenerateJwtToken(taiKhoan);

            return new LoginResponseDto
            {
                TaiKhoanId = taiKhoan.TaiKhoanId,
                KhachHangId = khachHang?.KhachHangId,
                Email = taiKhoan.TenDangNhap,
                Role = taiKhoan.Role?.Name ?? "Customer",
                Token = token
            };
        }
        public async Task ForgotPasswordAsync(string email)
        {
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == email);

            if (taiKhoan == null)
                throw new Exception("Tài khoản không tồn tại.");

            if (!taiKhoan.EmailConfirmed)
                throw new Exception("Email của tài khoản này chưa được xác thực.");

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
                throw new Exception("Mã xác thực đã hết hạn.");

            if (cachedOtp != code)
                throw new Exception("Mã xác thực không chính xác.");

            _cache.Remove($"RESET_OTP_{email}");
        }
        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(x => x.TenDangNhap == dto.Email);

            if (taiKhoan == null)
                throw new Exception("Không tìm thấy tài khoản để đặt lại mật khẩu.");

            taiKhoan.MatKhauHash =
                _hasher.HashPassword(taiKhoan, dto.NewPassword);

            taiKhoan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        private string GenerateJwtToken(TaiKhoan taiKhoan)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.TaiKhoanId.ToString()),
                new Claim(ClaimTypes.Email, taiKhoan.TenDangNhap ?? ""),
                new Claim(ClaimTypes.Role, taiKhoan.Role?.Name ?? "Customer")
            };

            // Sử dụng cùng chuỗi bí mật
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

            // Đảm bảo thuật toán ký trùng khớp (HmacSha512 hoặc HmacSha256 đều được nhưng phải thống nhất)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


    }
}
