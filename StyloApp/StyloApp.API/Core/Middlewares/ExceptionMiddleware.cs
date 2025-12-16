using StyloApp.API.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace StyloApp.API.Core.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi Server (500) chi tiết cho việc debug
                _logger.LogError(ex, "Một lỗi chưa được xử lý đã xảy ra: {Message}", ex.Message);

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Mặc định là lỗi Server 500
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorDetails = new ErrorDetails { StatusCode = context.Response.StatusCode };

            // Kiểm tra các Exception tùy chỉnh
            switch (exception)
            {
                case ConflictException conflictException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict; // 409
                    errorDetails.StatusCode = 409;
                    errorDetails.Message = conflictException.Message;
                    break;
                case NotFoundException notFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                    errorDetails.StatusCode = 404;
                    errorDetails.Message = notFoundException.Message;
                    break;
                // Thêm các loại lỗi khác (ví dụ: UnauthorizedException, ForbiddenException,...)

                default:
                    // Bảo mật: Không tiết lộ chi tiết lỗi Server cho Client
                    errorDetails.Message = "Internal Server Error. Vui lòng thử lại sau.";
                    break;
            }

            var result = JsonSerializer.Serialize(errorDetails);
            return context.Response.WriteAsync(result);
        }
    }
}
