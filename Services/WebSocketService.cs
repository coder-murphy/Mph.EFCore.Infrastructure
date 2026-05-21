using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Mph.EFCore.Infrastructure.Services
{
    public static class WebSocketService
    {
        /// <summary>
        /// 全局客户端socket
        /// </summary>
        public static ConcurrentDictionary<string, WebSocket> ClientSockets { get; set; } = [];

        /// <summary>
        /// 添加websocket链接
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ws"></param>
        public static async void AddSocket(string sid, WebSocket ws)
        {
            if (ClientSockets.TryGetValue(sid, out var oldWs))
            {
                if (!ReferenceEquals(oldWs, ws) && oldWs.State == WebSocketState.Open)
                {
                    try
                    {
                        await oldWs.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Replaced by new connection",
                            CancellationToken.None);
                    }
                    catch
                    {
                        // 可以记录日志
                    }
                }
            }

            ClientSockets.AddOrUpdate(sid, ws, (_, _) => ws);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task SendMessage(string sid, object data)
        {
            if (ClientSockets.TryGetValue(sid, out var ws))
            {
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                await ws.SendAsync(new ArraySegment<byte>(buffer,0,buffer.Length), 
                    WebSocketMessageType.Text, 
                    true, 
                    CancellationToken.None);
            }
        }

        /// <summary>
        /// 启用监听
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static async Task Listen(string sid, System.Net.WebSockets.WebSocket ws)
        {
            var buffer = new byte[1024 * 4];
            while (ws.State == System.Net.WebSockets.WebSocketState.Open)
            {
                await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            ClientSockets.TryRemove(sid, out _);
        }
    }

    /// <summary>
    /// web socket 请求体
    /// </summary>
    /// <param name="Sid"></param>
    /// <param name="Cmd"></param>
    /// <param name="MetaData"></param>
    public record WsRequest(string Sid, string Cmd, Dictionary<string, string> MetaData);
}
