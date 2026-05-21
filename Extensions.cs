using System.Text;

namespace Mph.EFCore.Infrastructure
{
    public static class Extensions
    {
        /// <summary>
        /// 分页处理快捷方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryPage<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            return query.Skip(pageIndex * pageSize).Take(pageSize);
        }

        public static string GetClientIp(this HttpContext context)
        {
            // 1. 优先取代理真实IP
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip) && ip != "::1" && ip != "127.0.0.1")
            {
                // 处理多IP格式
                if (ip.Contains(','))
                    ip = ip.Split(',')[0].Trim();

                return ip;
            }

            // 2. 取不到再取远程IP
            ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            return ip == "::1" ? "127.0.0.1" : ip;
        }

        /// <summary>
        /// 获取请求参数（GET+POST 都支持）
        /// </summary>
        public static string GetRequestParam(this HttpContext context)
        {
            var request = context.Request;

            // 1. GET 请求：从 Query 取
            if (request.Method == "GET")
            {
                return request.QueryString.ToString();
            }

            // 2. POST/PUT 请求：从 Body 取（必须启用倒带）
            request.EnableBuffering();
            var buffer = new byte[request.ContentLength ?? 0];
            request.Body.ReadExactly(buffer);
            request.Body.Position = 0; // 倒回，让控制器能正常读取

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
