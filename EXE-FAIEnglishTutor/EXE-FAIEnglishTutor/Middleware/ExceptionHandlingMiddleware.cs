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
                await HandleError(context, StatusCodes.Status404NotFound, ex, "Resource not found");
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleError(context, StatusCodes.Status401Unauthorized, ex, "Unauthorized access");
            }
            catch (TimeoutException ex)
            {
                await HandleError(context, StatusCodes.Status408RequestTimeout, ex, "Request timed out");
            }
            catch (Exception ex)
            {
                await HandleError(context, StatusCodes.Status500InternalServerError, ex, "An unexpected error occurred");
            }
        }
        private async Task HandleError(HttpContext context, int statusCode, Exception ex, string message)
        {
            _logger.LogError(ex, "{Message}\nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "Inner exception: {Message}\nStackTrace: {StackTrace}",
                    ex.InnerException.Message, ex.InnerException.StackTrace);
            }

            // Nếu yêu cầu là API (JSON), trả về JSON
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(new
                {
                    message,
                    error = ex.Message,
                    stackTrace = ex.StackTrace // Chỉ dùng trong dev
                });
            }
            else
            {
                // Chuyển hướng đến trang lỗi cho các yêu cầu MVC
                context.Response.Redirect($"/Error/{statusCode}");
            }
        }
    }
}
