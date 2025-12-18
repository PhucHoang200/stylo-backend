namespace StyloApp.API.DTOs
{
    public class ProductDetailDto
    {
        public int SanPhamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } // Giá thấp nhất ban đầu
        public string ImageUrl { get; set; } = string.Empty;

        // Danh sách các tùy chọn để người dùng chọn
        public List<MauSacDto> AvailableColors { get; set; } = new();
        public List<SizeDto> AvailableSizes { get; set; } = new();
    }
}
