using StyloApp.API.Data;
using StyloApp.API.DTOs;
using StyloApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace StyloApp.API.Services
{
    public class OrderService
    {
        private readonly FashionShopContext _context;
        public OrderService(FashionShopContext context)
        {
            _context = context;
        }
        public async Task<int> ProcessCheckoutAsync(CheckoutRequestDto request)
        {
            // Sử dụng Transaction để đảm bảo an toàn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Tính toán tổng tiền
                decimal tongTienHang = request.Items.Sum(x => x.SoLuong * x.DonGia);
                decimal tongThanhToan = tongTienHang + request.PhiVanChuyen;

                // 2. Tạo đối tượng Đơn hàng (DonHang)
                var donHang = new DonHang
                {
                    KhachHangId = request.KhachHangId,
                    TrangThai = "",
                    KenhBan = request.KenhBan,
                    TongTienHang = tongTienHang,
                    TongGiamGia = 0,
                    Thue = 0,
                    PhiVanChuyen = request.PhiVanChuyen,
                    TongThanhToan = tongThanhToan,
                    NgayDat = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync(); // Lưu để lấy DonHangID

                // 3. Duyệt qua từng sản phẩm để lưu chi tiết và trừ kho
                foreach (var item in request.Items)
                {
                    // 3a. Thêm vào bảng Chi tiết đơn hàng
                    var chiTiet = new DonHangChiTiet
                    {
                        DonHangId = donHang.DonHangId,
                        BienTheId = item.BienTheId,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        GiamGia = 0,
                        Thue = 0
                    };
                    _context.DonHangChiTiets.Add(chiTiet);

                    //// 3b. TRỪ TỒN KHO (Quan trọng nhất)
                    //var tonKho = await _context.TonKhos
                    //    .FirstOrDefaultAsync(tk => tk.BienTheId == item.BienTheId);

                    //if (tonKho == null || (tonKho.OnHand - tonKho.Reserved) < item.SoLuong)
                    //{
                    //    throw new Exception($"Sản phẩm ID {item.BienTheId} không đủ tồn kho.");
                    //}
                    // 3b. TRỪ TỒN KHO (Đã cập nhật để lấy thông tin hiển thị)
                    var tonKho = await _context.TonKhos
                        .Include(tk => tk.BienThe)
                            .ThenInclude(bt => bt.SanPham)
                        .Include(tk => tk.BienThe)
                            .ThenInclude(bt => bt.Size)
                        .Include(tk => tk.BienThe)
                            .ThenInclude(bt => bt.Mau)
                        .FirstOrDefaultAsync(tk => tk.BienTheId == item.BienTheId);

                    if (tonKho == null)
                    {
                        throw new Exception($"Sản phẩm có mã biến thể {item.BienTheId} không tồn tại trong hệ thống.");
                    }

                    int soLuongKhaDung = tonKho.OnHand - tonKho.Reserved;

                    if (soLuongKhaDung < item.SoLuong)
                    {
                        // Lấy thông tin chi tiết để hiển thị lên giao diện
                        string tenSanPham = tonKho.BienThe?.SanPham?.TenSanPham ?? "Sản phẩm";
                        string size = tonKho.BienThe?.Size?.KyHieu ?? "";
                        string mau = tonKho.BienThe?.Mau?.Ten ?? "";

                        // Tạo thông báo lỗi thân thiện với người dùng
                        string thongBao = $"Rất tiếc, sản phẩm '{tenSanPham}' (Size: {size}, Màu: {mau}) " +
                                         $"chỉ còn {soLuongKhaDung} món trong kho. Vui lòng điều chỉnh số lượng.";

                        throw new Exception(thongBao);
                    }

                    // Trừ số lượng thực tế trong kho
                    tonKho.OnHand -= item.SoLuong;
                    tonKho.UpdatedAt = DateTime.Now;
                }

                // 4. Tự động tạo một Vận đơn (VanDon) ở trạng thái chờ
                var vanDon = new VanDon
                {
                    DonHangId = donHang.DonHangId,
                    TrangThaiGiao = "CREATED",
                    PhiVanChuyen = request.PhiVanChuyen
                };
                _context.VanDons.Add(vanDon);

                await _context.SaveChangesAsync();

                // Hoàn tất giao dịch
                await transaction.CommitAsync();

                return donHang.DonHangId;
            }
            catch (Exception)
            {
                // Nếu có bất kỳ lỗi nào, hủy bỏ toàn bộ thay đổi trong DB
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
