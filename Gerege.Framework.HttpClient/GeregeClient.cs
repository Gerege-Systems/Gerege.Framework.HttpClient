﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

/////// date: 2021.12.23 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace Gerege.Framework.HttpClient;

/// <summary>
/// Гэрэгэ HTTP клиент хийсвэр класс.
/// <para>
/// Мэдээлэл JSON бүтэцтэй байна гэж үзнэ.
/// </para>
/// </summary>
public abstract class GeregeClient : System.Net.Http.HttpClient
{
    /// <inheritdoc />
    public GeregeClient(HttpMessageHandler handler) : base(handler)
    {
        DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
        DefaultRequestHeaders.Add(HttpRequestHeader.UserAgent.ToString(), "Gerege HTTP Client - C# V8");
    }

    /// <summary>
    /// RFC 6750 Bearer Token тооцоолж авах.
    /// <para>
    /// Энэ функцын зорилго нь серверлүү хандах токен шаардлагатай бол түүнийг тооцоолж өгөх юм.
    /// Хэрвээ токен амьдрах хугацаа нь дуусаж байвал шинэчлэх үйлдлийг мөн гүйцэтгэх шаардлагатай.
    /// </para>
    /// <remarks>
    /// <code>
    /// // override code sample
    /// GeregeToken? _currentToken = null;
    /// object? _fetchTokenPayload = null;
    /// 
    /// protected override GeregeToken? FetchToken(object? payload = null)
    /// {
    ///      if (payload is not null)
    ///         _fetchTokenPayload = payload;
    ///         
    ///      if (_currentToken is not null
    ///         AND _currentToken.IsExpiring)
    ///      {
    ///         _currentToken = null;
    ///         payload = _fetchTokenPayload;
    ///      }
    ///      
    ///     if (payload is not null)
    ///         _currentToken = RequestToken(payload);
    ///         
    ///     return _currentToken;
    /// }
    /// </code>
    /// </remarks>
    /// </summary>
    /// <exception cref="Exception">
    /// Token темплейт бүтэц/класс буруу зарлагдсан, хүсэлтийн параметрууд буруу өгөгдсөн, холболт тасарсан,
    /// серверээс хариу ирээгүй, серверээс токен авах эрх үүсээгүй байсан, ирсэн хариуны формат зөрсөн
    /// гэх мэтчилэн алдаануудын улмаас Exception үүсгэж шалтгааныг мэдэгдэнэ.
    /// </exception>
    /// <returns>
    /// RFC 6750 Bearer Token | null.
    /// </returns>
    protected virtual GeregeToken? FetchToken(object? payload = null)
    {
        return null;
    }

    /// <summary>
    /// HTTP хүсэлт үүсгэх.
    /// Өгөгдөл JSON бүтцээр дамжуулагдана гэж үзнэ.
    /// <para>
    /// Хэрвээ instance дээр Token утга олгогдсон байгаа бол RFC 6750 Bearer Token дүрмийн дагуу хүсэлтийн толгойд оруулна.
    /// </para>
    /// </summary>
    /// <param name="requestUri">Хүсэлт илгээх хаяг.</param>
    /// <param name="method">Хүсэлтийн HTTP дүрэм.</param>
    /// <param name="payload">Хүсэлтийн бие.</param>
    /// <returns>
    /// Амжилттай байгуулсан хүсэлтийг буцаана.
    /// </returns>
    protected virtual HttpRequestMessage CreateRequest<T>(string? requestUri, HttpMethod? method = null, object? payload = null)
    {
        if (string.IsNullOrEmpty(requestUri))
            throw new($"{GetType().Name}: Error on CreateRequest<T> -> Must be set requestUri or BaseAddress!");

        var request = new HttpRequestMessage
        {
            RequestUri = new(requestUri),
            Method = method ?? HttpMethod.Post,
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            )
        };

        GeregeToken? token = FetchToken();
        if (token is not null && token.IsValid)
            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {token.Value}");

        return request;
    }

    /// <summary>
    /// HTTP хүсэлт үүсгэж илгээн мэдээлэл хүлээж авах.
    /// <para>
    /// T темплейт бүтэц/класс буруу зарлагдсан, хүсэлтийн параметрууд буруу өгөгдсөн, холболт тасарсан, серверээс хариу ирээгүй, ирсэн хариуны формат зөрсөн
    /// гэх мэтчилэн болон өөр бусад шалтгаануудын улмаас Exception алдаа үүсэх боломжтой тул заавал try {} catch (Exception) {} код блок дунд ашиглана.
    /// Хөгжүүлэгч нь хүсвэл энэ функцийг удамшуулсан класс дээрээ override түлхүүр үгээр дахин функц болгон тодорхойлж шаардлагатай үйлдлүүд нэмж гүйцэтгэж болно.
    /// </para>
    /// </summary>
    /// <param name="payload">Хүсэлтийн бие.</param>
    /// <param name="method">Хүсэлтийн дүрэм. Анхны утга null үед POST дүрэм гэж үзнэ.</param>
    /// <param name="requestUri">Хүсэлт илгээх хаяг.</param>
    /// <exception cref="Exception">
    /// T темплейт бүтэц/класс буруу зарлагдсан, хүсэлтийн параметрууд буруу өгөгдсөн, холболт тасарсан, серверээс хариу ирээгүй,
    /// ирсэн хариуны формат зөрсөн гэх мэтчилэн алдаануудын улмаас Exception үүсгэж шалтгааныг мэдэгдэнэ.
    /// </exception>
    /// <returns>
    /// Серверээс ирсэн хариуг амжилттай авч тухайн зарласан T темплейт класс обьектэд хөрвүүлсэн утгыг буцаана. 
    /// </returns>
    public virtual T Request<T>(object? payload = null, HttpMethod? method = null, string? requestUri = null)
    {
        HttpRequestMessage requestMessage = CreateRequest<T>(requestUri ?? BaseAddress?.ToString(), method, payload);

        HttpStatusCode code = HttpStatusCode.Created;
        string contentString = Task.Run(async () =>
        {
            HttpResponseMessage responseMessage = await SendAsync(requestMessage);
            code = responseMessage.StatusCode;
            return await responseMessage.Content.ReadAsStringAsync();
        }).Result;

        if (code == HttpStatusCode.OK)
        {
            try
            {
                T? result = JsonSerializer.Deserialize<T>(contentString, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                if (result is not null) return result;
            }
            catch { }
        }

        string error;
        try
        {
            var doc = JsonDocument.Parse(contentString);
            if (doc.RootElement.TryGetProperty("message", out JsonElement message))
                error = message.ToString();
            else
                error = "Unknown response message from Gerege Server";
        }
        catch (Exception ex)
        {
            error = $"Error on response: {ex.Message}";
        }

        throw new Exception(error);
    }

    /// <summary>Cache хүсэлтийн хариу файлуудыг хадгалах хавтас зам.</summary>
    public string? CachePath { get; set; } = null;

    /// <summary>
    /// HTTP хүсэлт үүсгэж илгээн мэдээлэл хүлээж авах.
    /// Амжилттай биелсэн хүсэлтийн хариуг cache-д хадгална. 
    /// <para>
    /// T темплейт бүтэц/класс буруу зарлагдсан, хүсэлтийн параметрууд буруу өгөгдсөн, холболт тасарсан, серверээс хариу ирээгүй, ирсэн хариуны формат зөрсөн
    /// гэх мэтчилэн болон өөр бусад шалтгаануудын улмаас Exception алдаа үүсэх боломжтой тул заавал try {} catch (Exception) {} код блок дунд ашиглана.
    /// Хөгжүүлэгч нь хүсвэл энэ функцийг удамшуулсан класс дээрээ override түлхүүр үгээр дахин функц болгон тодорхойлж шаардлагатай үйлдлүүд нэмж гүйцэтгэж болно.
    /// </para>
    /// </summary>
    /// <param name="payload">Хүсэлтийн бие.</param>
    /// <param name="method">Хүсэлтийн дүрэм. Анхны утга null үед POST дүрэм гэж үзнэ.</param>
    /// <param name="requestUri">Хүсэлт илгээх хаяг.</param>
    /// <exception cref="Exception">
    /// T темплейт бүтэц/класс буруу зарлагдсан, хүсэлтийн параметрууд буруу өгөгдсөн, холболт тасарсан, серверээс хариу ирээгүй,
    /// ирсэн хариуны формат зөрсөн гэх мэтчилэн алдаануудын улмаас Exception үүсгэж шалтгааныг мэдэгдэнэ.
    /// </exception>
    /// <returns>
    /// Серверээс ирсэн хариуг амжилттай авсан эсвэл Cache дээрээс амжилттай уншсан мэдээллийг тухайн зарласан T темплейт класс обьектэд хөрвүүлсэн утгыг буцаана
    /// </returns>
    public virtual T CacheRequest<T>(object? payload = null, HttpMethod? method = null, string? requestUri = null)
    {
        if (payload is null) return Request<T>(payload, method, requestUri);

        GeregeCache cache = new(payload, CachePath);
        try
        {
            return cache.Load<T>();
        }
        catch { /* Cache байхгүй эсвэл уншиж чадаагүй */ }

        T result = Request<T>(payload, method, requestUri);
        bool cacheCreated = false;
        try
        {
            cacheCreated = cache.Create(result);
        }
        catch { /* Cache үүсгэж чадсангүй */ }
        finally
        {
            if (!cacheCreated && cache.Exists())
                File.Delete(cache.FilePath);
        }
        return result;
    }
}
