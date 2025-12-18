using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyloApp.API.DTOs;
using StyloApp.API.Services;
using System.Security.Claims;
using StyloApp.API.Core.Extensions;

namespace StyloApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService) => _accountService = accountService;

        // Helper lấy UserId từ Token
        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Tự động lấy ID từ Token, cực kỳ bảo mật
            int userId = User.GetUserId();

            var profile = await _accountService.GetProfileAsync(GetUserId());
            return profile == null ? NotFound() : Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            await _accountService.UpdateProfileAsync(GetUserId(), dto);
            return Ok(new { message = "Update thành công" });
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            int userId = User.GetUserId();
            var addresses = await _accountService.GetAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpPut("addresses/{id}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            await _accountService.SetDefaultAddressAsync(GetUserId(), id);
            return Ok();
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(AddressDto dto)
        {
            await _accountService.AddAddressAsync(User.GetUserId(), dto);
            return Ok(new { message = "Thêm địa chỉ thành công" });
        }

        [HttpDelete("addresses/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _accountService.DeleteAddressAsync(User.GetUserId(), id);
            if (!result) return NotFound(new { message = "Không tìm thấy địa chỉ hoặc bạn không có quyền xóa" });

            return Ok(new { message = "Xóa địa chỉ thành công" });
        }
    }
}
