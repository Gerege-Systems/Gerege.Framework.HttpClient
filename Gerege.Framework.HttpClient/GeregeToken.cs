using System;
using Newtonsoft.Json;

/////// date: 2021.12.17 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace Gerege.Framework.HttpClient;

/// <summary>
/// Гэрэгэ серверлүү хандах токен зөвшөөрөл.
/// </summary>
public class GeregeToken
{
    /// <summary>Токен утга.</summary>
    [JsonProperty("token", Required = Required.Always)]
    public string? Value { get; internal set; }

    /// <summary>Токен дуусах хүртэлх хүчинтэй огноо.</summary>
    [JsonProperty("expires", Required = Required.Always)]
    public DateTime ExpireDate { get; internal set; }

    /// <summary>Токен амьдрах хугацаа секундээр.</summary>
    [JsonProperty("expires_in", Required = Required.Always)]
    public int LifeSeconds { get; internal set; }

    /// <summary>Токен хүлээн авсан системийн локал огноо.</summary>
    [JsonIgnore]
    public DateTime CreatedLocalTime { get; internal set; }

    /// <summary>Токен үүсэх.</summary>
    public GeregeToken()
    {
        Update(null, DateTime.Now, 0);
    }

    /// <summary>
    /// Токен шинэчлэх.
    /// Токен чухал мэдээлэл тул internal set дүрмээр шинэчлэнэ.
    /// </summary>
    /// <param name="value">Токен утга.</param>
    /// <param name="expires">Токен дуусах хүртэлх хүчинтэй хугацаа.</param>
    /// <param name="life_seconds">Токен хүчинтэй хугацаа секундээр.</param>
    public void Update(string? value, DateTime expires, int life_seconds)
    {
        Value = value;
        ExpireDate = expires;
        LifeSeconds = life_seconds;

        CreatedLocalTime = DateTime.Now;
    }

    /// <summary>Токен хугацаа дууссан эсвэл дуусаж буй эсэх.</summary>
    [JsonIgnore]
    public bool IsExpiring => (DateTime.Now - CreatedLocalTime).TotalSeconds + 60 > LifeSeconds;
}
