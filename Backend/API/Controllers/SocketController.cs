using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/ws")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SocketController : ControllerBase
    {
        [Route("ping")]
        [AllowAnonymous]
        public async Task Ping()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None
                );

                while (!result.CloseStatus.HasValue)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (message.Equals("ping", StringComparison.CurrentCultureIgnoreCase))
                    {
                        byte[] pingBuffer = Encoding.UTF8.GetBytes("pong");
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(pingBuffer, 0, result.Count),
                            result.MessageType,
                            result.EndOfMessage,
                            CancellationToken.None
                        );
                    }
                    else
                    {
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(buffer, 0, result.Count),
                            result.MessageType,
                            result.EndOfMessage,
                            CancellationToken.None
                        );
                    }

                    result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None
                    );
                }

                await webSocket.CloseAsync(
                    result.CloseStatus.Value,
                    result.CloseStatusDescription,
                    CancellationToken.None
                );
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
