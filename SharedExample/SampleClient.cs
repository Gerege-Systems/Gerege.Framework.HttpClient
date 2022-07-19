using System.Net.Http;
using Gerege.Framework.HttpClient;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace SharedExample;

public class SampleClient : GeregeClient
{
    public SampleClient(HttpMessageHandler handler) : base(handler)
    {
        BaseAddress = new("http://mock-server/api");
    }
}
