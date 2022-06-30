using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace HttpClientExample.NET_6;

/// <summary>
/// HTTP хүсэлт гүйцэтгэх үед Application Busy төлөвийг удирдах.
/// </summary>
public class BusyHandler : DelegatingHandler
{
    // App Busy төлөв заах
    private void SetAppBusy(bool value)
    {
        this.App().Busy = value;
        
        Debug.WriteLine("[BUSYHANDLER] Application " + (value ? "завгүй" : "завтай") + " боллоо.");
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
