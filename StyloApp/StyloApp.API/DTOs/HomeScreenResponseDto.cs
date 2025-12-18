namespace StyloApp.API.DTOs
{
    public class HomeScreenResponseDto
    {
        public IEnumerable<PhanLoaiDto> PhanLoaiList { get; set; } = new List<PhanLoaiDto>();

        // THAY THẾ: InitialProducts -> InitialVariants
        public IEnumerable<SanPhamBienTheHomeDto> InitialVariants { get; set; } = new List<SanPhamBienTheHomeDto>();

        public int TotalVariantCount { get; set; } // Tổng số biến thể
    }
}
