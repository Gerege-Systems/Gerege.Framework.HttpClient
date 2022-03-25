using System;
using System.Net.Http;

using Gerege.Framework.HttpClient;

namespace HttpClientExample
{
#pragma warning disable CA1416 // Validate platform compatibility
    public class SampleClient : GeregeClient
    {
        public SampleClient(HttpMessageHandler handler) : base(handler)
        {
            BaseAddress = new Uri("http://mock-server/api");
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility
}
