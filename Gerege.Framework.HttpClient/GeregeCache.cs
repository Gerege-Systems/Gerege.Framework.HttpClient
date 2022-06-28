﻿namespace Gerege.Framework.HttpClient;

/////// date: 2019.11.02 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Cache файл обьекттэй ажиллах класс.
/// </summary>
public class GeregeCache
{
    /// <summary>
    /// Cache файлын нэр зам.
    /// </summary>
    public string? FilePath = null;

    /// <summary>
    /// Cache файл обьект үүсгэх байгуулагч.
    /// </summary>
    /// <param name="msg">Мэдээллийн мессеж дугаар.</param>
    /// <param name="payload">Мэдээллийн хүсэлтийн бие.</param>
    /// <param name="folderPath">Файл хадгалагдах хавтасны зам.</param>
    public GeregeCache(int msg, dynamic? payload = null, string? folderPath = null)
    {
        try
        {
            char slash = Path.DirectorySeparatorChar;
            if (string.IsNullOrEmpty(folderPath))
            {
                // Хэрэглэгчээс cache хадгалах хавтас замыг заагаагүй бол 
                // App оршиж байгаа хавтас дотор Cache нэртэй фолдер сонгоё
                DirectoryInfo currentDir = Directory.GetParent(
                    Process.GetCurrentProcess().MainModule.FileName);
                folderPath = $"{currentDir.FullName}{slash}Cache";
            }
            
            string filePath = $"{folderPath}{slash}[{msg}]";
            
            if (payload is not null)
            {
                JObject jobj = JObject.FromObject(payload);

                filePath += " " + string.Join(" ",
                    jobj.Children().Cast<JProperty>()
                    .Select(jp => jp.Name + "=" + jp.Value.ToString()));
            }
            filePath += ".json";

            FileInfo fi = new(filePath);
            if (fi.Directory != null
                && !fi.Directory.Exists
                && fi.DirectoryName != null)
                Directory.CreateDirectory(fi.DirectoryName);
            FilePath = filePath;
        }
        catch { }
    }

    /// <summary>
    /// Cache обьектын мэдээлэл бүхий файл үүссэн байгаа эсэхийг шалгана.
    /// </summary>
    /// <returns>
    /// Тухайн Cache мэдээлэлтэй файл үүссэн байгаа бол үнэн эсрэг тохиолдолд худал утга буцаана.
    /// </returns>
    public bool Exists() => !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath) && new FileInfo(FilePath).Length != 0;

    /// <summary>
    /// Cache мэдээллийг файлаас унших.
    /// </summary>
    /// <returns>
    /// Амжилттай уншсан тохиолдолд .NET обьект утга буцаана.
    /// Бусад тохиолдолд CacheLoadException үүсэх тул заавал try {} catch (Exception) {} код блок дунд ашиглана.
    /// </returns>
    public dynamic Load()
    {
        if (!Exists()) throw new("Cache байдгүй шүү. Яахуу найзаа?");

        using StreamReader r = new(FilePath);
        string fileDataDecrypted = UnProtect(r.ReadToEnd(), DataProtectionScope.CurrentUser);

        dynamic? response = JsonConvert.DeserializeObject(fileDataDecrypted);

        if (response is null) throw new(FilePath + ": Null result!");

        return response;
    }

    /// <summary>
    /// Cache мэдээллийг файлаас унших.
    /// </summary>
    /// <typeparam name="T">Тодорхой бүтэцтэй зарласан класс/төрөл обьектийн нэр.</typeparam>
    /// <returns>
    /// Амжилттай уншсан тохиолдолд Т темплейт бүхий бүтэцтэй класс/төрөл утга буцаана.
    /// Бусад тохиолдолд Exception үүсэх тул заавал try {} catch (Exception) {} код блок дунд ашиглана.
    /// </returns>
    public T Load<T>()
    {
        if (!Exists()) throw new(FilePath + ": Cache байдгүй шүү. Яахуу найзаа?");

        using StreamReader r = new(FilePath);
        string fileDataDecrypted = UnProtect(r.ReadToEnd(), DataProtectionScope.CurrentUser);

        dynamic? response = JsonConvert.DeserializeObject<T>(fileDataDecrypted);

        if (response is null) throw new(FilePath + ": Null result!");

        return response;
    }

    /// <summary>
    /// Cache мэдээллийг файл үүсгэн бичиж хадгална. Нэр бүхий файл аль хэдийн үүссэн байсан тохиолдолд тухайн файлыг устгаад шинээр үүсгэх болно.
    /// </summary>
    /// <param name="data">Хадгалах мэдээлэл бүхий обьект.</param>
    public bool Create(dynamic data)
    {
        if (string.IsNullOrEmpty(FilePath))
            throw new("Cache тохируулга буруу хийгдсэн байна!");

        if (Exists()) File.Delete(FilePath);

        using (StreamWriter file = File.CreateText(FilePath))
        {
            string plainData = JsonConvert.SerializeObject(data);
            string encryptedData = Protect(plainData, DataProtectionScope.CurrentUser);

            file.WriteLine(encryptedData);
        }

        return true;
    }

    string Protect(string stringToEncrypt, DataProtectionScope scope)
    {
        return Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(stringToEncrypt), null, scope));
    }

    string UnProtect(string encryptedString, DataProtectionScope scope)
    {
        return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(encryptedString), null, scope));
    }
}
