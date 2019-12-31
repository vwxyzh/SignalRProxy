using System.Net.Http;

namespace SampleServer
{
    public static class HttpContentExtensions
    {
        public static T WithContentType<T>(this T content)
            where T : HttpContent
        {
            content.Headers.ContentType.MediaType = "application/json";
            return content;
        }
    }
}
