using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mph.EFCore.Infrastructure.Common
{
    /// <summary>
    /// 自动放行 OPTIONS 预检请求
    /// </summary>
    public class OptionsFilter : IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var request = context.HttpContext.Request;

            // 判断是否是 OPTIONS 预检请求
            if (request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                // 直接返回 200 OK，终止后续管道执行
                context.Result = new StatusCodeResult(200);
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // 无需处理
        }
    }
}
