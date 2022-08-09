using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
    /// <summary>Серверээс хамгийн сүүлд авсан гэрэгэ хариу.</summary>
    public GeregeResponse? HttpLastResponse { get; set; } = null;

    /// <inheritdoc />
    public GeregeClient(HttpMessageHandler handler) : base(handler)
    {
        DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
        DefaultRequestHeaders.Add(HttpRequestHeader.UserAgent.ToString(), "Gerege HTTP Client - C# V5");
    }

    /// <summary>
    /// T темплейт төрлөөс Гэрэгэ мессеж дугаарыг авах.
    /// <para>
    /// T темплейт төрөл нь Гэрэгэ мессеж дугаар өгөх public virtual int GeregeMessage() { return 564654; } function агуулсан байх ёстой.
    /// </para>
    /// </summary>
    /// <exception cref="Exception">
    /// T темплейт төрөл нь Гэрэгэ мессеж дугаарын function агуулаагүй байвал Exception үүсгэж шалтгааныг мэдэгдэнэ.
    /// </exception>
    /// <returns>
    /// Гэрэгэ мессеж дугаарыг амжилттай буцаана.
    /// </returns>
    protected virtual int GetMessageCode<T>()
    {
        Type type = typeof(T);

        object? instanceOfType = Activator.CreateInstance(type);
        if (instanceOfType is null)
            throw new(GetType().Name + ": Error on GetMessageCode<T> -> Invalid type!");

        MethodInfo? geregeMessage = type.GetMethod("GeregeMessage");
        if (geregeMessage is null)
            throw new(GetType().Name + ": Error on GetMessageCode<T> -> Unknown Gerege message!");

        object resultCode = geregeMessage.Invoke(instanceOfType, Array.Empty<object>());
        return resultCode is int @int ? @int :
            throw new(GetType().Name + ": Error on GetMessageCode<T> -> Invalid Gerege message defination!");
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
    ///         _currentToken = RequestSampleToken(payload);
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
    /// Өгөгдөл JSON бүтцээр дамжуулагдана гэж үзсэн байгаа.
    /// Хэрвээ instance дээр Token утга олгогдсон байгаа бол RFC 6750 Bearer Token дүрмийн дагуу хүсэлтийн толгойд оруулна.
    /// <para>
    /// Зөв зарлагдсан T темплейт класс/бүтцээс Гэрэгэ мессеж дугаарыг авч хүсэлтийн толгойн message_code талбарт мөн онооно.
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
            throw new(GetType().Name + ": Error on CreateRequest<T> -> Must be set requestUri or BaseAddress!");

        var request = new HttpRequestMessage
        {
            RequestUri = new(requestUri),
            Method = method ?? HttpMethod.Post,
            Content = new StringContent(JsonConvert.SerializeObject(payload))
        };

        int msg = GetMessageCode<T>();
        request.Headers.Add("message_code", Convert.ToString(msg));

        GeregeToken? token = FetchToken();
        if (token is not null && !string.IsNullOrEmpty(token.Value))
            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + token.Value);

        return request;
    }

    /// <summary>
    /// HTTP хүсэлт үүсгэж илгээн мэдээлэл хүлээж авах.
    /// Зөв зарлагдсан T темплейт класс/бүтцээс Гэрэгэ мессеж дугаарыг авч хүсэлтийн толгойн message_code талбарт онооно.
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
        HttpLastResponse = null;

        HttpRequestMessage requestMessage = CreateRequest<T>(requestUri ?? BaseAddress?.ToString(), method, payload);
        string contentString = Task.Run(async () =>
        {
            HttpResponseMessage responsMessage = await SendAsync(requestMessage);
            return await responsMessage.Content.ReadAsStringAsync();
        }).Result;

        dynamic? content = JsonConvert.DeserializeObject(contentString);
        if (content is null)
            throw new(GetType().Name + ": Error on Request<T> -> Invalid JSON response! response.content: " + contentString);

        if (content.code is null || content.status is null)
            throw new(GetType().Name + ": Error on Request<T> -> Invalid Gerege response! response.content: " + contentString);

        HttpLastResponse = new(
            Convert.ToInt32(content.code),
            Convert.ToString(content.status),
            Convert.ToString(content.message ?? "Empty message"),
            content.result ?? new { }
        );

        if (!HttpLastResponse.IsSuccess)
            throw new(HttpLastResponse.Message);

        return JsonConvert.DeserializeObject<T>(
            JsonConvert.SerializeObject(HttpLastResponse.Result),
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
        );
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
        GeregeCache cache = new(GetMessageCode<T>(), payload, CachePath);
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
