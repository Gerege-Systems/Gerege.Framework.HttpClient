using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HttpClientExample
{
    /// <author>
    /// codesaur - 2022.01.23
    /// </author>
    /// <package>
    /// Gerege Windows Application Development v5
    /// </package>

    /// <summary>
    /// HTTP хүсэлт гүйцэтгэх үед Application Busy төлөвийг удирдах.
    /// </summary>
    public class BusyHandler : DelegatingHandler
    {
        // App Busy төлөв заах
        private void SetAppBusy(bool value)
        {
            this.App().Busy = value;
            
            Debug.WriteLine("Application " + (value ? "завгүй" : "завтай") + " боллоо.");
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // App Busy төлөвт заая
                SetAppBusy(true);

                // base.SendAsync нь голын боловсруулагчийг ажиллуулж байна
                var response = await base.SendAsync(request, cancellationToken);

                // Амжилттай хүлээн авсан хариу мэдээллийг дараагийн боловсруулагчид шилжүүлье
                return response;
            }
            catch (Exception)
            {
                // Алдааг дараагийн боловсруулагчид мэдээллээ
                throw;
            }
            finally
            {
                // Үйлдэл дууссан тул App Busy төлвийг арилгая
                SetAppBusy(false);
            }
        }
    }
}
