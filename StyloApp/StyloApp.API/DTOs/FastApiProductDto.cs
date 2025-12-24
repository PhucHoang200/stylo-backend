namespace StyloApp.API.DTOs
{

    public class FastApiProductDto
    {
        public int sanPhamId { get; set; }
        public string tenSanPham { get; set; } = "";
        public decimal giaBan { get; set; }
        public string imageUrl { get; set; } = "";
    }

    public class FastApiResponse
    {
        public int count { get; set; }
        public List<FastApiProductDto> products { get; set; } = new();
    }

}
