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
                // Lấy lỗi sâu nhất (InnerException)
                var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                // Log lỗi này ra Console để bạn đọc được trong Visual Studio
                Console.WriteLine($"FULL ERROR: {realError}");

                return BadRequest(new
                {
                    Message = "Lỗi lưu Database",
                    Detail = realError // Trả về chi tiết lỗi để kiểm tra
                });
            }
        }
    }
}
