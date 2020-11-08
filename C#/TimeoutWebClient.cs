using System;
using System.Net;

internal class TimeoutWebClient : WebClient
{
    protected override WebRequest GetWebRequest(Uri uri)
    {
        WebRequest w = base.GetWebRequest(uri);
        // 20 minutes, could probably make it 10
        w.Timeout = 3 * 60 * 1000;
        return w;
    }
}