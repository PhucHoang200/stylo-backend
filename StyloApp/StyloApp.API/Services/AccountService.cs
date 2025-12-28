using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StyloApp.API.Data;
using StyloApp.API.DTOs;
using StyloApp.API.Entities;

namespace StyloApp.API.Services
{
    public class AccountService
    {
        private readonly FashionShopContext _context;
        private readonly PasswordHasher<TaiKhoan> _hasher = new();

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

        // Thêm địa chỉ mới
        public async Task AddAddressAsync(int userId, AddressDto dto)
        {
            // Kiểm tra xem người dùng đã có địa chỉ nào chưa
            var hasAddress = await _context.DiaChis.AnyAsync(a => a.TaiKhoanId == userId);

            var newAddress = new DiaChi
            {
                TaiKhoanId = userId,
                DiaChiChiTiet = dto.DiaChiChiTiet,
                LoaiDiaChi = dto.LoaiDiaChi,
                // Nếu là địa chỉ đầu tiên thì đặt làm mặc định luôn
                IsDefault = !hasAddress || (dto.IsDefault),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Nếu người dùng chọn địa chỉ này là mặc định, phải bỏ mặc định các cái cũ
            if (newAddress.IsDefault == true)
            {
                var currentDefault = await _context.DiaChis
                    .FirstOrDefaultAsync(a => a.TaiKhoanId == userId && a.IsDefault == true);
                if (currentDefault != null) currentDefault.IsDefault = false;
            }

            _context.DiaChis.Add(newAddress);
            await _context.SaveChangesAsync();
        }

        // Xóa địa chỉ
        public async Task<bool> DeleteAddressAsync(int userId, int addressId)
        {
            var address = await _context.DiaChis
                .FirstOrDefaultAsync(a => a.DiaChiId == addressId && a.TaiKhoanId == userId);

            if (address == null) return false;

            _context.DiaChis.Remove(address);

            // Logic nâng cao: Nếu xóa địa chỉ mặc định, hãy đặt một địa chỉ khác làm mặc định
            if (address.IsDefault == true)
            {
                await _context.SaveChangesAsync(); // Xóa trước
                var nextAddress = await _context.DiaChis
                    .FirstOrDefaultAsync(a => a.TaiKhoanId == userId);
                if (nextAddress != null) nextAddress.IsDefault = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            if (taiKhoan == null) throw new ApplicationException("Tài khoản không tồn tại");

            // 1. Kiểm tra mật khẩu cũ
            var result = _hasher.VerifyHashedPassword(taiKhoan, taiKhoan.MatKhauHash, dto.OldPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new ApplicationException("Mật khẩu cũ không chính xác");

            // 2. Hash mật khẩu mới và cập nhật
            taiKhoan.MatKhauHash = _hasher.HashPassword(taiKhoan, dto.NewPassword);
            taiKhoan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int userId)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            if (taiKhoan == null) throw new Exception("Tài khoản không tồn tại");

            // Lưu ý: Nếu có bảng KhachHang liên kết, bạn cần xóa KhachHang trước hoặc dùng Cascade Delete
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(x => x.TaiKhoanId == userId);
            if (khachHang != null) _context.KhachHangs.Remove(khachHang);

            _context.TaiKhoans.Remove(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderHistoryDto>> GetPurchaseHistoryAsync(int userId)
        {
            // 1. Tìm Khách hàng từ Tài khoản ID
            var khachHang = await _context.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.TaiKhoanId == userId);

            if (khachHang == null) return new List<OrderHistoryDto>();

            // 2. Query dữ liệu
            var orders = await _context.DonHangs
                .AsNoTracking()
                .Where(d => d.KhachHangId == khachHang.KhachHangId)
                .OrderByDescending(d => d.NgayDat)
                .Select(d => new OrderHistoryDto
                {
                    DonHangId = d.DonHangId.ToString(),
                    TrangThai = d.TrangThai,
                    TongThanhToan = d.TongThanhToan,
                    NgayDat = d.NgayDat,

                    // Lấy MaVanDon từ bảng VanDon
                    MaVanDon = d.VanDons.Select(v => v.MaVanDon).FirstOrDefault(),
                    TrangThaiGiao = d.VanDons.Select(v => v.TrangThaiGiao).FirstOrDefault(),

                    ChiTietItems = d.DonHangChiTiets.Select(ct => new OrderDetailDto
                    {
                        BienTheId = ct.BienTheId,
                        SanPhamId = ct.BienThe.SanPhamId,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,

                        // TenSanPham: DonHang_ChiTiet -> SanPham_BienThe -> SanPham
                        TenSanPham = ct.BienThe.SanPham.TenSanPham.Replace(",", "").Replace(".", "").Trim(),

                        // Size: SanPham_BienThe -> Size
                        Size = ct.BienThe.Size.KyHieu,
                        SizeId = ct.BienThe.SizeId,

                        // MauSac: SanPham_BienThe -> MauSac
                        MauSac = new MauSacDto
                        {
                            Id = ct.BienThe.Mau.MauId,
                            Ten = ct.BienThe.Mau.Ten,
                            MaHex = ct.BienThe.Mau.MaHex ?? ""
                        },

                        // ImageUrl: Lấy từ bảng AnhSanPham liên kết với SanPhamID
                        ImageUrl = ct.BienThe.SanPham.AnhSanPhams
                            .OrderByDescending(a => a.IsPrimary)
                            .Select(a => a.Url)
                            .FirstOrDefault() ?? "default-product.jpg"
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }

    }
}
