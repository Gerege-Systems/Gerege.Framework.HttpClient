using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gerege.Framework.HttpClient
{
    /// <author>
    /// codesaur - 2022.01.23
    /// </author>
    /// <package>
    /// Gerege Application Development Framework V5
    /// </package>

    /// <summary>
    /// HTTP хүсэлт илгээх үед амжилтгүй болсон тохиолдолд дахин оролдох удирдлага.
    /// </summary>
    public class RetryHandler : DelegatingHandler
    {
        // Нийт хэдэн удаа оролдлого хийх тоо
        private readonly uint MaxTries;

        /// <summary>
        /// HTTP хүсэлт илгээх үед амжилтгүй болсон тохиолдолд дахин хүсэлт илгээх үзэх зорилготой удирдлага.
        /// </summary>
        /// <param name="tryCount">Нийт дээд тал нь хэдэн удаа оролдлого хийх тоо.</param>
        public RetryHandler(uint tryCount = 3)
        {
            MaxTries = tryCount;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage?> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int tryCount = -1;
            Exception? errorOccurs = null;
            while (true)
            {
                tryCount++;
                if (MaxTries == tryCount)
                {
                    // За надад бол хийж чадах зүйл үлдсэнгүй хэлсэн тоогоор чинь оролдлоошдээ,
                    // оролдлого хийх явцад ямар нэг алдаа гарсан байгаа бол хүлээн авагчруу мэдээлнэ
                    // эсвэл одоо null утга буцаахаас даа
                    if (errorOccurs is null) return null;

                    throw errorOccurs;
                }

                try
                {
                    if (tryCount > 0)
                        Debug.WriteLine(GetType().FullName + " нь " + (tryCount + 1) + " удаа оролдож байна.");

                    // base.SendAsync нь голын боловсруулагчийг ажиллуулж байна
                    var response = await base.SendAsync(request, cancellationToken);

                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        // 503 Серверийн алдаа учир хэсэг хүлээгээд дахин оролдоё
                        await Task.Delay(5000, cancellationToken);
                        continue;
                    }

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        // 429 Хэт олон хүсэлт давхацсан учир хэсэг хүлээгээд дахин оролдоё
                        await Task.Delay(1000, cancellationToken);
                        continue;
                    }

                    // Ямартай ч хариу ирсэн тул үүнийг нь хүлээн авагчруу буцаая
                    return response;
                }
                catch (Exception ex)
                {
                    // илэрсэн алдааг түр хадгалъя
                    errorOccurs = ex;

                    if (IsNetworkError(ex))
                    {
                        // Сүлжээний алдаа учир хэсэг хүлээгээд дахин оролдоё
                        await Task.Delay(2000, cancellationToken);
                    }

                    continue;
                }
            }
        }

        private static bool IsNetworkError(Exception ex)
        {
            // Сүлжээний алдаа?
            if (ex is SocketException)
                return true;

            // нягтлах өөр ямар нэг алдаа гараагүй?
            if (ex.InnerException is null)
                return false;
            
            return IsNetworkError(ex.InnerException);
        }
    }
}
