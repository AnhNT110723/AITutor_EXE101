using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class HttpResponseMessageResult : IActionResult
    {
        private readonly HttpResponseMessage _responseMessage;

        public HttpResponseMessageResult(HttpResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            using (var responseMessage = _responseMessage)
            {
                context.HttpContext.Response.StatusCode = (int)responseMessage.StatusCode;

                foreach (var header in responseMessage.Headers)
                {
                    context.HttpContext.Response.Headers[header.Key] = header.Value.ToArray();
                }

                if (responseMessage.Content != null)
                {
                    var contentHeaders = responseMessage.Content.Headers;
                    foreach (var header in contentHeaders)
                    {
                        context.HttpContext.Response.Headers[header.Key] = header.Value.ToArray();
                    }

                    using (var stream = await responseMessage.Content.ReadAsStreamAsync())
                    {
                        await stream.CopyToAsync(context.HttpContext.Response.Body);
                    }
                }
            }
        }
    }
}