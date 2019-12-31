using Microsoft.AspNetCore.Mvc;

namespace SampleServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientStore _clientStore;

        public ClientsController(ClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        [HttpPut("{connectionId}")]
        public void Add(
            [FromRoute] string connectionId,
            [FromHeader(Name = "x-hub")] string hub,
            [FromHeader(Name = "x-user")] string user)
        {
            _clientStore.AddConnection(connectionId, user);
        }

        [HttpDelete("{connectionId}")]
        public void Remove(
            [FromRoute] string connectionId,
            [FromHeader(Name = "x-hub")] string hub,
            [FromHeader(Name = "x-user")] string user)
        {
            _clientStore.RemoveConnection(connectionId);
        }
    }
}
