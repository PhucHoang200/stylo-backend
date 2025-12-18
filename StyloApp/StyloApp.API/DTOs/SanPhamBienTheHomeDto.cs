namespace StyloApp.API.DTOs
{
    public class SanPhamBienTheHomeDto
    {
        // Thông tin Biến Thể
        public int BienTheId { get; set; }
        public decimal GiaBan { get; set; }
        public string? Sku { get; set; }

        // Thông tin Sản Phẩm cha (để hiển thị tên và ID cha)
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = string.Empty;

        // Thông tin hình ảnh/phân loại
        public string ImageUrl { get; set; } = "placeholder.jpg";
        public int? DanhMucId { get; set; }

        // Bạn có thể thêm Màu/Size nếu cần hiển thị ngay
        // public string? TenMau { get; set; }
    }
}
