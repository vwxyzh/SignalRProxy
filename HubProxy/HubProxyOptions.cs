using System;

namespace HubProxy
{
    public class HubProxyOptions
    {
        public Uri ForwardTo { get; set; }
        public ArgumentsFormatter Formatter { get; set; }
    }
}
