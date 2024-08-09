using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace SharedExample;

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
                throw new("Unknown route pattern [" + requestTarget + "]");

            Task<string>? input = request.Content?.ReadAsStringAsync(cancellationToken);
            return input?.Result is null || input?.Result == "null"
                ? throw new("Invalid input!") : HandleMessages(JsonSerializer.Deserialize<JsonElement>(input!.Result));
        }
        catch (Exception ex)
        {
            return Respond(new
            {
                message = ex.Message,
            },
            HttpStatusCode.NotFound);
        }
    }

    private Task<HttpResponseMessage> Respond(object content, HttpStatusCode StatusCode = HttpStatusCode.OK)
    {
        return Task.FromResult(new HttpResponseMessage()
        {
            StatusCode = StatusCode,
            Content = new StringContent(JsonSerializer.Serialize(content))
        });
    }

    private Task<HttpResponseMessage> HandleMessages(JsonElement payload)
    {
        if (!payload.TryGetProperty("get", out JsonElement get))
            throw new("Invalid payload");
        else if(get.ToString() == "title")
            return Respond(new
            {
                title = "Gerege System-д тавтай морил."
            });

        throw new("Unknown message");
    }
}
