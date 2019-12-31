using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HubProxy.Api
{
    [Route("[Controller]")]
    public class HubController : ControllerBase
    {
        private readonly IHubContext<Hub> _hubContext;

        public HubController(IHubContext<Hub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("all/{method}")]
        public async Task Broadcast([FromRoute] string method, [FromBody] Dictionary<string, object> arguments, CancellationToken cancellationToken = default)
        {
            await SendAsync(_hubContext.Clients.All, method, arguments, cancellationToken);
        }

        [HttpPost("clients/{connectionId}/{method}")]
        public async Task Send([FromRoute] string connectionId, [FromRoute] string method, [FromBody] Dictionary<string, object> arguments, CancellationToken cancellationToken = default)
        {
            await SendAsync(_hubContext.Clients.Client(connectionId), method, arguments, cancellationToken);
        }

        [HttpPost("users/{userId}/{method}")]
        public async Task SendUser([FromRoute] string userId, [FromRoute] string method, [FromBody] Dictionary<string, object> arguments, CancellationToken cancellationToken = default)
        {
            await SendAsync(_hubContext.Clients.User(userId), method, arguments, cancellationToken);
        }

        [HttpPost("groups/{groupName}/{method}")]
        public async Task SendGroup([FromRoute] string groupName, [FromRoute] string method, [FromBody] Dictionary<string, object> arguments, CancellationToken cancellationToken = default)
        {
            await SendAsync(_hubContext.Clients.Group(groupName), method, arguments, cancellationToken);
        }

        [HttpPut("groups/{groupName}/clients/{connectionId}")]
        public async Task JoinGroup([FromRoute] string groupName, [FromRoute] string connectionId, CancellationToken cancellationToken = default)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, groupName, cancellationToken);
        }

        [HttpDelete("groups/{groupName}/clients/{connectionId}")]
        public async Task LeaveGroup([FromRoute] string groupName, [FromRoute] string connectionId, CancellationToken cancellationToken = default)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName, cancellationToken);
        }

        private Task SendAsync(IClientProxy client, string method, Dictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            switch (arguments.Count)
            {
                case 0:
                    return client.SendAsync(method, cancellationToken);
                case 1:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        cancellationToken);
                case 2:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        cancellationToken);
                case 3:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        cancellationToken);
                case 4:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        cancellationToken);
                case 5:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        arguments.ElementAt(4).Value,
                        cancellationToken);
                case 6:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        arguments.ElementAt(4).Value,
                        arguments.ElementAt(5).Value,
                        cancellationToken);
                case 7:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        arguments.ElementAt(4).Value,
                        arguments.ElementAt(5).Value,
                        arguments.ElementAt(6).Value,
                        cancellationToken);
                case 8:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        arguments.ElementAt(4).Value,
                        arguments.ElementAt(5).Value,
                        arguments.ElementAt(6).Value,
                        arguments.ElementAt(7).Value,
                        cancellationToken);
                case 9:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        arguments.ElementAt(4).Value,
                        arguments.ElementAt(5).Value,
                        arguments.ElementAt(6).Value,
                        arguments.ElementAt(7).Value,
                        arguments.ElementAt(8).Value,
                        cancellationToken);
                case 10:
                    return client.SendAsync(
                        method,
                        arguments.First().Value,
                        arguments.ElementAt(1).Value,
                        arguments.ElementAt(2).Value,
                        arguments.ElementAt(3).Value,
                        arguments.ElementAt(4).Value,
                        arguments.ElementAt(5).Value,
                        arguments.ElementAt(6).Value,
                        arguments.ElementAt(7).Value,
                        arguments.ElementAt(8).Value,
                        arguments.ElementAt(9).Value,
                        cancellationToken);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
