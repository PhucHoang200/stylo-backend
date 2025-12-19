namespace StyloApp.API.DTOs
{
    public class OrderHistoryDto
    {
        public int DonHangId { get; set; }
        public string TrangThai { get; set; }
        public decimal TongThanhToan { get; set; }
        public DateTime NgayDat { get; set; }
        public string? MaVanDon { get; set; }
        public string? TrangThaiGiao { get; set; }
        public List<OrderDetailDto> ChiTietItems { get; set; } = new();
    }
}
