using System;
using System.Text.Json.Serialization;

/////// date: 2020.12.11 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace Gerege.Framework.HttpClient;

/// <summary>
/// Гэрэгэ диспетчер серверээс ирсэн хариу.
/// </summary>
[Serializable]
public class GeregeResponse(int code, string status, string message, object result)
{
    /// <summary>Статус код.</summary>
    [JsonPropertyName("code")]
    public int Code { get; internal set; } = code;

    /// <summary>Статус мэдэгдэл.</summary>
    [JsonPropertyName("status")]
    public string Status { get; internal set; } = status;

    /// <summary>Тайлбар.</summary>
    [JsonPropertyName("message")]
    public string Message { get; internal set; } = message;

    /// <summary>Үр дүн.</summary> 
    [JsonPropertyName("result")]
    public object Result { get; internal set; } = result;

    /// <summary>Серверээс хүлээн авсан үр дүн амжилттай төлөвт буй эсэх.</summary>
    [JsonIgnore]
    public bool IsSuccess => Code == 200 && Status == "success";
}
