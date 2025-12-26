using StyloApp.API.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StyloApp.API.DTOs;

namespace StyloApp.API.Services
{
    public class ProductService
    {
        private readonly FashionShopContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(FashionShopContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HomeScreenResponseDto> GetHomeScreenDataAsync()
        {
            _context.Database.SetCommandTimeout(120);

            // 1. Lấy danh sách Phân loại
            var phanLoaiDtos = await _context.PhanLoais
                .AsNoTracking()
                .OrderBy(pl => pl.PhanLoaiId)
                .Select(pl => new PhanLoaiDto
                {
                    Id = pl.PhanLoaiId,
                    Ten = pl.Ten
                }).ToListAsync();

            // 2. Lấy 50 sản phẩm mới nhất (với giá thấp nhất) cho trang Home
            // Chúng ta bắt đầu truy vấn từ SanPham để đảm bảo tính duy nhất của Sản phẩm
            var products = await GetProductDataQuery(null, 50);

            return new HomeScreenResponseDto
            {
                PhanLoaiList = phanLoaiDtos,
                InitialVariants = products, // Dù tên là Variants nhưng dữ liệu là Product đại diện
                TotalVariantCount = 0
            };
        }

        public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetTop50VariantsByPhanLoaiAsync(int phanLoaiId)
        {
            _context.Database.SetCommandTimeout(60);
            return await GetProductDataQuery(phanLoaiId, 50);
        }

        // HÀM DÙNG CHUNG ĐỂ XỬ LÝ LOGIC LẤY GIÁ MIN VÀ ẢNH
        private async Task<List<SanPhamBienTheHomeDto>> GetProductDataQuery(int? phanLoaiId, int takeCount)
        {
            var query = _context.SanPhams.AsNoTracking();

            // Nếu có lọc theo phân loại
            if (phanLoaiId.HasValue)
            {
                var danhMucIds = _context.DanhMucs
                    .Where(dm => dm.PhanLoaiId == phanLoaiId.Value)
                    .Select(dm => dm.DanhMucId);

                query = query.Where(sp => sp.DanhMucId.HasValue && danhMucIds.Contains(sp.DanhMucId.Value));
            }

            return await query
                .OrderByDescending(sp => sp.SanPhamId)
                .Take(takeCount)
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham.Replace(",", "").Replace(".", "").Trim(),
                    DanhMucId = sp.DanhMucId,

                    // LẤY GIÁ BÁN THẤP NHẤT TỪ BẢNG BIẾN THỂ
                    GiaBan = sp.SanPhamBienThes.Any()
                             ? sp.SanPhamBienThes.Max(bt => bt.GiaBan)
                             : 0,

                    // LẤY BIENTHEID CỦA THẰNG CÓ GIÁ THẤP NHẤT ĐÓ
                    BienTheId = sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.BienTheId).FirstOrDefault(),

                    // LẤY SKU ĐẠI DIỆN
                    Sku = sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.Sku).FirstOrDefault(),

                    // LẤY ẢNH: Ưu tiên ảnh của Biến thể giá thấp nhất, nếu không có thì lấy ảnh của Sản phẩm, 
                    // nếu vẫn không có lấy ảnh đầu tiên bất kỳ.
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary) // Ưu tiên ảnh chính
                                .ThenBy(a => a.BienTheId == sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.BienTheId).FirstOrDefault() ? 0 : 1)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "default-product.jpg"
                })
                .ToListAsync();
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(int sanPhamId)
        {
            _context.Database.SetCommandTimeout(60);

            return await _context.SanPhams
                .AsNoTracking()
                .Where(sp => sp.SanPhamId == sanPhamId)
                .Select(sp => new ProductDetailDto
                {
                    SanPhamId = sp.SanPhamId,
                    Name = sp.TenSanPham.Replace(",", "").Replace(".", "").Trim(),
                    Description = sp.MoTa ?? "",
                    // Giá thấp nhất để hiển thị ban đầu
                    BasePrice = sp.SanPhamBienThes.Min(bt => bt.GiaBan),
                    // Ảnh chính của sản phẩm
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "default-product.jpg",

                    // Lấy danh sách màu sắc độc nhất từ các biến thể
                    AvailableColors = sp.SanPhamBienThes
                        .Select(bt => bt.Mau) // Giả định Navigation Property là 'Mau'
                        .Distinct()
                        .Select(m => new MauSacDto { Id = m.MauId, Ten = m.Ten, MaHex = m.MaHex ?? "" })
                        .ToList(),

                    // Lấy danh sách size độc nhất từ các biến thể
                    AvailableSizes = sp.SanPhamBienThes
                        .Select(bt => bt.Size) // Giả định Navigation Property là 'Size'
                        .Distinct()
                        .OrderBy(s => s.ThuTu)
                        .Select(s => new SizeDto { Id = s.SizeId, KyHieu = s.KyHieu })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<decimal?> GetPriceByVariantAsync(int sanPhamId, int mauId, int sizeId)
        {
            // API này dùng khi người dùng đã chọn đủ cả Màu và Size
            return await _context.SanPhamBienThes
                .AsNoTracking()
                .Where(bt => bt.SanPhamId == sanPhamId && bt.MauId == mauId && bt.SizeId == sizeId)
                .Select(bt => bt.GiaBan)
                .FirstOrDefaultAsync();
        }

        // 1. Lấy toàn bộ Phân loại
        public async Task<IEnumerable<PhanLoaiDto>> GetAllPhanLoaiAsync()
        {
            return await _context.PhanLoais
                .AsNoTracking()
                .OrderBy(pl => pl.Ten)
                .Select(pl => new PhanLoaiDto
                {
                    Id = pl.PhanLoaiId,
                    Ten = pl.Ten
                }).ToListAsync();
        }

        // 2. Lấy toàn bộ Danh mục theo Phân loại ID
        public async Task<IEnumerable<DanhMucDto>> GetDanhMucsByPhanLoaiAsync(int phanLoaiId)
        {
            return await _context.DanhMucs
                .AsNoTracking()
                .Where(dm => dm.PhanLoaiId == phanLoaiId)
                .OrderBy(dm => dm.Ten)
                .Select(dm => new DanhMucDto
                {
                    Id = dm.DanhMucId,
                    Ten = dm.Ten
                }).ToListAsync();
        }

        // 3. Lấy 50 sản phẩm theo Danh mục (Lấy kèm giá min và ảnh đại diện)
        public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetProductsByDanhMucAsync(int danhMucId)
        {
            // Thiết lập timeout để xử lý dữ liệu lớn (45.000+ bản ghi)
            _context.Database.SetCommandTimeout(60);

            return await _context.SanPhams
                .AsNoTracking()
                .Where(sp => sp.DanhMucId == danhMucId)
                .OrderByDescending(sp => sp.SanPhamId)
                .Take(50)
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    // Lấy thông tin cơ bản từ SanPham
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    DanhMucId = sp.DanhMucId,

                    // KỸ THUẬT: Lấy biến thể có giá thấp nhất của sản phẩm này
                    // Sắp xếp các biến thể theo GiaBan tăng dần, lấy thằng đầu tiên
                    BienTheId = sp.SanPhamBienThes
                                .OrderBy(bt => bt.GiaBan)
                                .Select(bt => bt.BienTheId)
                                .FirstOrDefault(),

                    GiaBan = sp.SanPhamBienThes
                                .OrderBy(bt => bt.GiaBan)
                                .Select(bt => bt.GiaBan)
                                .FirstOrDefault(),

                    Sku = sp.SanPhamBienThes
                                .OrderBy(bt => bt.GiaBan)
                                .Select(bt => bt.Sku)
                                .FirstOrDefault(),

                    // Lấy ảnh: Ưu tiên ảnh chính (IsPrimary), nếu không có lấy ảnh bất kỳ của SP đó
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "https://via.placeholder.com/600x800?text=No+Image"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SanPhamBienTheHomeDto>> GetRecommendationsAsync(int sanPhamId)
        {
            var client = _httpClientFactory.CreateClient();

            // 1. Gọi tới FastAPI (Thay URL bằng URL thực tế của bạn)
            // Lưu ý: ID truyền vào FastAPI thường là chuỗi theo logic pkl của bạn
            var fastApiUrl = $"http://127.0.0.1:8000/recommend/{sanPhamId}";

            try
            {
                var response = await client.GetAsync(fastApiUrl);
                if (!response.IsSuccessStatusCode) return new List<SanPhamBienTheHomeDto>();

                var content = await response.Content.ReadAsStringAsync();

                // Parse JSON để lấy danh sách ID
                using var doc = JsonDocument.Parse(content);
                var idStrings = doc.RootElement.GetProperty("recommendations")
                                    .EnumerateArray()
                                    .Select(x => x.GetString())
                                    .ToList();

                // Chuyển ID từ string sang int
                var recommendedIds = idStrings.Select(id => int.Parse(id)).ToList();

                // 2. Lấy thông tin sản phẩm từ database dựa trên danh sách ID này
                // Tái sử dụng logic lấy giá Min và Ảnh từ hàm GetProductDataQuery nhưng thay đổi điều kiện lọc
                return await GetProductInfoByIdsAsync(recommendedIds);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gọi FastAPI: {ex.Message}");
                return new List<SanPhamBienTheHomeDto>();
            }
        }

        // Hàm phụ trợ để lấy data theo danh sách ID cụ thể
        private async Task<List<SanPhamBienTheHomeDto>> GetProductInfoByIdsAsync(List<int> ids)
        {
            return await _context.SanPhams
                .AsNoTracking()
                .Where(sp => ids.Contains(sp.SanPhamId))
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    DanhMucId = sp.DanhMucId,
                    GiaBan = sp.SanPhamBienThes.Any() ? sp.SanPhamBienThes.Min(bt => bt.GiaBan) : 0,
                    BienTheId = sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.BienTheId).FirstOrDefault(),
                    Sku = sp.SanPhamBienThes.OrderBy(bt => bt.GiaBan).Select(bt => bt.Sku).FirstOrDefault(),
                    ImageUrl = sp.AnhSanPhams
                                .OrderByDescending(a => a.IsPrimary)
                                .Select(a => a.Url)
                                .FirstOrDefault() ?? "default-product.jpg"
                })
                .ToListAsync();
        }

        public async Task<(List<SanPhamBienTheHomeDto> Items, int Total)> SearchAsync(ProductSearchDto dto)
        {
            var query = _context.SanPhams
                .AsNoTracking()
                .Include(sp => sp.DanhMuc)
                .AsQueryable();

            // SEARCH THEO TÊN SP + TÊN DANH MỤC
            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                string keyword = dto.Keyword.Trim().ToLower();

                query = query.Where(sp =>
                    sp.TenSanPham.ToLower().Contains(keyword) ||
                    (sp.DanhMuc != null && sp.DanhMuc.Ten.ToLower().Contains(keyword))
                );
            }

            int total = await query.CountAsync();

            var items = await query
                .OrderByDescending(sp => sp.SanPhamId)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    DanhMucId = sp.DanhMucId,

                    // GIÁ THẤP NHẤT
                    GiaBan = sp.SanPhamBienThes.Any()
                        ? sp.SanPhamBienThes.Min(bt => bt.GiaBan)
                        : 0,

                    // BIẾN THỂ ĐẠI DIỆN
                    BienTheId = sp.SanPhamBienThes
                        .OrderBy(bt => bt.GiaBan)
                        .Select(bt => bt.BienTheId)
                        .FirstOrDefault(),

                    Sku = sp.SanPhamBienThes
                        .OrderBy(bt => bt.GiaBan)
                        .Select(bt => bt.Sku)
                        .FirstOrDefault(),

                    // ẢNH ĐẠI DIỆN
                    ImageUrl = sp.AnhSanPhams
                        .OrderByDescending(a => a.IsPrimary)
                        .Select(a => a.Url)
                        .FirstOrDefault() ?? "default-product.jpg"
                })
                .ToListAsync();

            return (items, total);
        }

        public async Task<IEnumerable<SanPhamBienTheHomeDto>> SearchByImageAsync(IFormFile file, int k = 10)
        {
            var client = _httpClientFactory.CreateClient();
            var fastApiUrl = $"http://127.0.0.1:8000/search-by-image?k={k}";

            using var form = new MultipartFormDataContent();

            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            form.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync(fastApiUrl, form);
            if (!response.IsSuccessStatusCode)
                return new List<SanPhamBienTheHomeDto>();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            // FastAPI trả SanPhamID
            var productIds = doc.RootElement
                .GetProperty("products")
                .EnumerateArray()
                .Select(p => p.GetProperty("SanPhamID").GetInt32())
                .ToList();

            if (!productIds.Any())
                return new List<SanPhamBienTheHomeDto>();

            // LẤY DATA GIỐNG HỆT RECOMMEND
            return await GetProductInfoByIdsAsync(productIds);
        }

        public async Task<IEnumerable<SanPhamBienTheHomeDto>> SearchByKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<SanPhamBienTheHomeDto>();

            keyword = keyword.Trim().ToLower();

            return await _context.SanPhams
                .AsNoTracking()
                .Where(sp =>
                    sp.TenSanPham.ToLower().Contains(keyword) ||
                    (sp.MoTa != null && sp.MoTa.ToLower().Contains(keyword))
                )
                .OrderByDescending(sp => sp.SanPhamId)
                .Take(50)
                .Select(sp => new SanPhamBienTheHomeDto
                {
                    SanPhamId = sp.SanPhamId,
                    TenSanPham = sp.TenSanPham,
                    DanhMucId = sp.DanhMucId,

                    GiaBan = sp.SanPhamBienThes.Any()
                        ? sp.SanPhamBienThes.Min(bt => bt.GiaBan)
                        : 0,

                    BienTheId = sp.SanPhamBienThes
                        .OrderBy(bt => bt.GiaBan)
                        .Select(bt => bt.BienTheId)
                        .FirstOrDefault(),

                    Sku = sp.SanPhamBienThes
                        .OrderBy(bt => bt.GiaBan)
                        .Select(bt => bt.Sku)
                        .FirstOrDefault(),

                    ImageUrl = sp.AnhSanPhams
                        .OrderByDescending(a => a.IsPrimary)
                        .Select(a => a.Url)
                        .FirstOrDefault() ?? "/images/default-product.jpg"
                })
                .ToListAsync();
        }
    }
}
