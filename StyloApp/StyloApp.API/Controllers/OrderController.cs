using Microsoft.AspNetCore.Mvc;
using StyloApp.API.DTOs;
using StyloApp.API.Services;

namespace StyloApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service;

        public OrderController(OrderService service) => _service = service;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            try
            {
                int orderId = await _service.ProcessCheckoutAsync(request);
                return Ok(new { Message = "Thành công", OrderID = orderId });
            }
            catch (Exception ex)
            {
                // Lấy lỗi chi tiết (ví dụ: "Sản phẩm A hiện chỉ còn 2 món...")
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                Console.WriteLine($"ORDER ERROR: {realError}");

                return BadRequest(new
                {
                    // Gán lỗi thật vào Message để Flutter hiển thị lên UI
                    Message = realError,
                    Type = "ORDER_ERROR" // Thêm loại lỗi nếu bạn muốn xử lý logic khác nhau ở UI
                });
            }
        }
    }
}
