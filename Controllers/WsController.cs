using Mph.EFCore.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Mph.EFCore.Infrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WsController : ControllerBase
    {
        [HttpGet]
        public async Task Connect()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            // 获取当前电脑的 sid（你前端传的唯一ID）
            var sid = HttpContext.Request.Query["sid"].ToString();
            // 审计日志用
            var hostIp = HttpContext.Request.Query["host"].ToString();

            // 建立连接
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            WebSocketService.AddSocket(sid, socket);

            // 保持连接
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            // 断开后移除
            WebSocketService.ClientSockets.Remove(sid, out _);
        }
    }
}
