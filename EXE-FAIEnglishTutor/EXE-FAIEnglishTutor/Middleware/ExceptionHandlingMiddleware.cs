namespace EXE_FAIEnglishTutor.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Gọi middleware tiếp theo
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Timeout");
                context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                await context.Response.WriteAsJsonAsync(new { message = "Request timed out." });
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết hơn
                _logger.LogError(ex, "Unhandled exception: {Message}\nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception: {Message}\nStackTrace: {StackTrace}",
                        ex.InnerException.Message, ex.InnerException.StackTrace);
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message, // Thêm chi tiết lỗi (cẩn thận với thông tin nhạy cảm trong production)
                    stackTrace = ex.StackTrace // Thêm stack trace (chỉ dùng trong môi trường dev)
                });
            }
        }
    }
}
