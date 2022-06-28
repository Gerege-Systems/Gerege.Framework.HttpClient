namespace Gerege.Framework.HttpClient;

/////// date: 2020.12.11 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

using System;

/// <summary>
/// Гэрэгэ диспетчер серверээс ирсэн хариу dynamic бүтцээр.
/// </summary>
[Serializable]
public class GeregeResponse
{
    /// <summary>Статус код.</summary>
    public int Code { get; internal set; }

    /// <summary>Статус мэдэгдэл.</summary>
    public string Status { get; internal set; }

    /// <summary>Тайлбар.</summary>
    public string Message { get; internal set; }

    /// <summary>Үр дүн.</summary> 
    public dynamic Result { get; internal set; }

    /// <summary>Серверээс хүлээн авсан үр дүн амжилттай төлөвт буй эсэх.</summary>
    public bool IsSuccess => Code == 200 && Status == "success";

    /// <summary>Гэрэгэ серверээс ирсэн хариу.</summary>
    public GeregeResponse(int code, string status, string message, dynamic result)
    {
        Code = code;
        Status = status;
        Message = message;
        Result = result;
    }
}
