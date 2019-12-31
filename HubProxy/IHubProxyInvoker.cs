using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace HubProxy
{
    public interface IHubProxyInvoker
    {
        Task OnConnected(HubConnectionContext hubConnectionContext);
        Task OnDisconnected(HubConnectionContext hubConnectionContext, Exception ex);
        Task Invoke(HubConnectionContext hubConnectionContext, string methodName, object[] arguments);
    }
}
