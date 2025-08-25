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
        /// <summary>
        /// Handles WebSocket ping-pong communication for connection health monitoring.
        /// Accepts WebSocket connections and responds to "ping" messages with "pong" responses.
        /// For any other messages, echoes the received message back to the client.
        /// </summary>
        /// <remarks>
        /// This endpoint requires a WebSocket upgrade request. If a regular HTTP request is made,
        /// it will return a 400 Bad Request status code.
        /// 
        /// The method maintains the WebSocket connection until the client initiates closure,
        /// continuously listening for incoming messages and responding appropriately.
        /// </remarks>
        /// <returns>
        /// A task that represents the asynchronous WebSocket communication operation.
        /// The task completes when the WebSocket connection is closed by the client.
        /// </returns>
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
