using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Gerege.Framework.Logger;

/////// date: 2022.01.23 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace Gerege.Framework.HttpClient;

/// <summary>
/// HTTP хүсэлт гүйцэтгэх үед лог мэдээлэл хадгалах удирдлага.
/// </summary>
/// <remarks>
/// HTTP хүсэлт гүйцэтгэх үед лог хадгалах удирдлага үүсгэх.
/// <para>
/// Лог мэдээллийг http хүснэгтэд хадгална гэж үзнэ.
/// </para>
/// </remarks>
/// <param name="logger">Лог мэдээллийг хадгалагч объект.</param>
public class LoggingHandler(DatabaseLogger logger) : DelegatingHandler
{
    private readonly DatabaseLogger _db_logger = logger;

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Хүсэлт үүссэн огноогоор лог мсж үүсгэе
            string id = DateTime.Now.ToString("yyyyMMdd_Hmmss_");

            // Хүсэлтийн зам ба дүрмийг лог мсж-д хадгалъя
            id += $"{request.RequestUri.AbsolutePath}_{request.Method}_";

            // Илгээж буй хүсэлтийн мэдээллийг логын хэвийн түвшинд хадгалъяа
            _db_logger.Notice("http", $"{id}request", await request.Content.ReadAsStringAsync());

            // base.SendAsync нь голын боловсруулагчийг ажиллуулж байна
            var response = await base.SendAsync(request, cancellationToken);

            // Амжилттай хүлээн авсан хариу мэдээллийг логын хэвийн түвшинд хадгалъяа
            _db_logger.Notice("http", $"{id}response", await response.Content.ReadAsStringAsync());

            // Амжилттай хүлээн авсан хариу мэдээллийг дараагийн боловсруулагчид шилжүүлье
            return response;
        }
        catch (Exception ex)
        {
            // Үйлдэл амжилтгүй болж хүсэлт гүйцэтгэх үед гарсан алдааг логын алдааны түвшинд хадгалъяа
            _db_logger.Error("http", "Failed to get response ", ex);

            // Алдааг дараагийн боловсруулагчид мэдээллээ
            throw;
        }
    }
}
