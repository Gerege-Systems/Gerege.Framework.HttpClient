using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Gerege.Framework.Logger;

namespace Gerege.Framework.HttpClient
{
    /// <author>
    /// codesaur - 2022.01.23
    /// </author>
    /// <package>
    /// Gerege Application Development Framework V5
    /// </package>

    /// <summary>
    /// HTTP хүсэлт гүйцэтгэх үед лог мэдээлэл хадгалах удирдлага.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        private readonly DatabaseLogger _db_logger;

        /// <summary>
        /// HTTP хүсэлт гүйцэтгэх үед лог хадгалах удирдлага үүсгэх.
        /// <para>
        /// Лог мэдээллийг http хүснэгтэд хадгална гэж үзнэ.
        /// </para>
        /// </summary>
        /// <param name="logger">Лог мэдээллийг хадгалагч объект.</param>
        public LoggingHandler(DatabaseLogger logger)
        {
            _db_logger = logger;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Хүсэлт үүссэн огноогоор лог мсж үүсгэе
                string id = DateTime.Now.ToString("yyyyMMdd_Hmmss_");
                
                string message_code = string.Join("", request.Headers.GetValues("message_code"));

                // Хүсэлтийн толгойд мсж код байвал лог мсж-д хадгалъя
                if (!string.IsNullOrEmpty(message_code)
                    || request.Method != HttpMethod.Post) id += message_code + "_";

                // Илгээж буй хүсэлтийн мэдээллийг логын хэвийн түвшинд хадгалъяа
                _db_logger.Notice("http", id + "request", await request.Content?.ReadAsStringAsync());

                // base.SendAsync нь голын боловсруулагчийг ажиллуулж байна
                var response = await base.SendAsync(request, cancellationToken);

                // Амжилттай хүлээн авсан хариу мэдээллийг логын хэвийн түвшинд хадгалъяа
                _db_logger.Notice("http", id + "response", await response.Content.ReadAsStringAsync());

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
}
