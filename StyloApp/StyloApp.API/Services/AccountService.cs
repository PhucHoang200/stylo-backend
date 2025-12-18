using StyloApp.API.Data;
using StyloApp.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace StyloApp.API.Services
{
    public class AccountService
    {
        private readonly FashionShopContext _context;

        public AccountService(FashionShopContext context) => _context = context;

        // Lấy thông tin cá nhân
        public async Task<ProfileDto?> GetProfileAsync(int userId)
        {
            var kh = await _context.KhachHangs
                .FirstOrDefaultAsync(x => x.TaiKhoanId == userId);

            if (kh == null) return null;

            return new ProfileDto
            {
                FullName = kh.HoTen ?? "",
                Email = kh.Email ?? "",
                // Chuyển từ DateOnly sang DateTime
                DateOfBirth = kh.NgaySinh.HasValue
                              ? kh.NgaySinh.Value.ToDateTime(TimeOnly.MinValue)
                              : null,
                Gender = kh.GioiTinh ?? "",
                Phone = kh.SoDienThoai ?? ""
            };
        }

        // Cập nhật thông tin cá nhân
        public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var kh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.TaiKhoanId == userId);
            if (kh != null)
            {
                kh.HoTen = dto.FullName;
                kh.NgaySinh = dto.DateOfBirth.HasValue
               ? DateOnly.FromDateTime(dto.DateOfBirth.Value)
               : null;
                kh.GioiTinh = dto.Gender;
                kh.SoDienThoai = dto.Phone;
                kh.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Quản lý địa chỉ
        public async Task<List<AddressDto>> GetAddressesAsync(int userId)
        {
            return await _context.DiaChis
                .Where(a => a.TaiKhoanId == userId)
                .Select(a => new AddressDto
                {
                    DiaChiId = a.DiaChiId,
                    DiaChiChiTiet = a.DiaChiChiTiet,
                    LoaiDiaChi = a.LoaiDiaChi,
                    IsDefault = a.IsDefault ?? false
                }).ToListAsync();
        }

        public async Task SetDefaultAddressAsync(int userId, int addressId)
        {
            var all = await _context.DiaChis.Where(a => a.TaiKhoanId == userId).ToListAsync();
            foreach (var addr in all)
            {
                addr.IsDefault = (addr.DiaChiId == addressId);
            }
            await _context.SaveChangesAsync();
        }

    }
}
