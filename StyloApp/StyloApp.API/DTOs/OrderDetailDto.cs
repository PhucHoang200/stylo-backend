namespace StyloApp.API.DTOs
{
    public class OrderDetailDto
    {
        public int BienTheId { get; set; }
        public string TenSanPham { get; set; }
        public string ImageUrl { get; set; }
        public string Size { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public MauSacDto MauSac { get; set; }
    }
}
