using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HttpClientExample
{
    /// <summary>
    /// Туршилтын зорилгоор хуурамч сервер хандалт үүсгэв
    /// </summary>
    public sealed class MockServerHandler : HttpMessageHandler
    {
        protected sealed override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                Thread.Sleep(500); // Интернетээр хандаж буй мэт сэтгэгдэл төрүүлэх үүднээс хором хүлээлгэе

                string? requestTarget = request.RequestUri?.ToString();
                if (requestTarget != "http://mock-server/api")
                    throw new Exception("Unknown route pattern [" + requestTarget + "]");

                Task<string>? input = request.Content?.ReadAsStringAsync(cancellationToken);
                dynamic? payload = JsonConvert.DeserializeObject(input?.Result!);

                return HandleMessages(payload);
            }
            catch (Exception ex)
            {
                return Respond(new
                {
                    code = 404,
                    status = "Error 404",
                    message = ex.Message,
                    result = new { }
                },
                HttpStatusCode.NotFound);
            }
        }

        private Task<HttpResponseMessage> Respond(dynamic content, HttpStatusCode StatusCode = HttpStatusCode.OK)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = StatusCode,
                Content = new StringContent(JsonConvert.SerializeObject(content))
            });
        }

        private Task<HttpResponseMessage> HandleMessages(dynamic? payload)
        {
            if (payload?.get == null)
                throw new Exception("Invalid payload");

            if (Convert.ToString(payload.get) == "title")
                return Respond(new
                {
                    code = 200,
                    status = "success",
                    message = "",
                    result = new
                    {
                        title = "Gerege System-д тавтай морил"
                    }
                });
            
            throw new Exception("Unknown message");
        }
    }
}
