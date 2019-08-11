using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Blog.Jwt.Extensions
{
    public static class HttpExtension
    {
        public static async Task WriteJsonAsync(this HttpResponse response, string json, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json; charset=UTF-8";
            await response.WriteAsync(json);
            await response.Body.FlushAsync();
        }

        public static async Task WriteJsonAsync(this HttpResponse response, object o,
            HttpStatusCode code = HttpStatusCode.OK, string contentType = null)
        {
            var json = JsonConvert.SerializeObject(o);
            response.StatusCode = (int)code;
            await response.WriteJsonAsync(json, contentType);
        }
    }
}
