using System;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Diagnostics;

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
    [JsonPropertyName("token")]
    public string? Value { get; internal set; } = null;

    /// <summary>Токен дуусах хүртэлх хүчинтэй огноо.</summary>
    [JsonIgnore]
    public DateTime ValidTo { get; internal set; }

    /// <summary>
    /// Токен шинэчлэх.
    /// Токен чухал мэдээлэл тул internal set дүрмээр шинэчлэнэ.
    /// </summary>
    /// <param name="jwt">JWT утга.</param>
    public void Update(string jwt)
    {
        try
        {
            var token = new JwtSecurityToken(jwt);

            Value = jwt;
            ValidTo = Convert.ToDateTime(token.ValidTo);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    /// <summary>JWT хүчинтэй заагдсан эсэх.</summary>
    [JsonIgnore]
    public bool IsValid => Value != null && !IsExpiring;

    /// <summary>Токен хугацаа дууссан эсвэл дуусаж буй эсэх.
    /// <para>
    /// Дууссан эсвэл дуусах хүртэл 20 секундээс бага хугацаа үлдсэн.
    /// </para>
    /// </summary>
    [JsonIgnore]
    public bool IsExpiring => DateTime.Now > ValidTo.AddSeconds(-20);

    /// <summary>Токен утгыг JWT обьект болгон авах.</summary>
    [JsonIgnore]
    public JwtSecurityToken? JWT => string.IsNullOrEmpty(Value) ? null : new JwtSecurityToken(Value);
}
