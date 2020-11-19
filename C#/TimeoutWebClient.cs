using System;
using System.Net;

internal class TimeoutWebClient : WebClient
{
    protected override WebRequest GetWebRequest(Uri uri)
    {
        WebRequest w = base.GetWebRequest(uri);
        // 3 minutes
        w.Timeout = 3 * 60 * 1000;
        return w;
    }
}

internal class TimeoutWebClientShort : WebClient
{
    protected override WebRequest GetWebRequest(Uri uri)
    {
        WebRequest w = base.GetWebRequest(uri);
        // 10 seconds
        w.Timeout = 10 * 1000;
        return w;
    }
}