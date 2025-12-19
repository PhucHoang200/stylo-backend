namespace StyloApp.API.DTOs
{
    public class CheckoutRequestDto
    {
        public int KhachHangId { get; set; }
        public string KenhBan { get; set; } = "ONLINE";
        public decimal PhiVanChuyen { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
