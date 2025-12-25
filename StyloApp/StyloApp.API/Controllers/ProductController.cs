using Microsoft.AspNetCore.Mvc;
using StyloApp.API.Services;
using StyloApp.API.DTOs;

namespace StyloApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // Endpoint cho màn hình Home (Tải 100 Biến thể)
        [HttpGet("home")] // Giả định bạn đã có một Endpoint gọi GetHomeScreenDataAsync
        public async Task<IActionResult> GetHomeScreenData()
        {
            var result = await _productService.GetHomeScreenDataAsync();
            return Ok(result);
        }

        // Endpoint MỚI: Tải 20 Biến thể theo PhanLoaiID
        [HttpGet("variants/by-phanloai/{phanLoaiId}")]
        public async Task<IActionResult> GetTop20VariantsByPhanLoai(int phanLoaiId)
        {
            if (phanLoaiId <= 0)
            {
                return BadRequest("Mã phân loại không hợp lệ.");
            }

            var variants = await _productService.GetTop50VariantsByPhanLoaiAsync(phanLoaiId);
            return Ok(variants);
        }

        // 1. API lấy chi tiết sản phẩm
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            var product = await _productService.GetProductDetailAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm");
            return Ok(product);
        }

        // 2. API lấy giá theo lựa chọn Màu và Size
        [HttpGet("get-price")]
        public async Task<IActionResult> GetVariantPrice(int sanPhamId, int mauId, int sizeId)
        {
            var price = await _productService.GetPriceByVariantAsync(sanPhamId, mauId, sizeId);

            if (price == null)
                return NotFound("Biến thể này hiện không tồn tại hoặc hết hàng.");

            return Ok(new { Price = price });
        }

        // API 1: Lấy toàn bộ phân loại (Dùng cho Menu chính)
        [HttpGet("phan-loai")]
        public async Task<IActionResult> GetAllPhanLoai()
            => Ok(await _productService.GetAllPhanLoaiAsync());

        // API 2: Lấy danh mục khi click vào phân loại
        [HttpGet("phan-loai/{id}/danh-muc")]
        public async Task<IActionResult> GetDanhMucs(int id)
            => Ok(await _productService.GetDanhMucsByPhanLoaiAsync(id));

        // API 3: Lấy 50 sản phẩm khi click vào danh mục
        [HttpGet("danh-muc/{id}/san-pham")]
        public async Task<IActionResult> GetProductsByDanhMuc(int id)
            => Ok(await _productService.GetProductsByDanhMucAsync(id));

        // API: Lấy danh sách gợi ý liên quan cho một sản phẩm
        [HttpGet("{id}/recommendations")]
        public async Task<IActionResult> GetRelatedRecommendations(int id)
        {
            if (id <= 0) return BadRequest("ID sản phẩm không hợp lệ.");

            var recommendations = await _productService.GetRecommendationsAsync(id);

            if (recommendations == null || !recommendations.Any())
                return Ok(new List<SanPhamBienTheHomeDto>()); // Trả về mảng rỗng nếu không có gợi ý

            return Ok(recommendations);
        }
        [HttpPost("search-by-image")]
        public async Task<IActionResult> SearchByImage(
           IFormFile file,
           [FromQuery] int k = 10)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Image file is required");

            var result = await _productService.SearchByImageAsync(file, k);
            return Ok(result);
        }
    }
}
