using System;
using System.Net.Http;

using Gerege.Framework.HttpClient;

namespace HttpClientExample
{
    public class SampleClient : GeregeClient
    {
        public SampleClient(HttpMessageHandler handler) : base(handler)
        {
            BaseAddress = new("http://mock-server/api");
        }
    }
}
