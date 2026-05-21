using System.Net;
using System.Text.Json;

namespace Mph.EFCore.Infrastucture.Middlewares
{
  /// <summary>
  /// 全局异常处理中间件
  /// </summary>
  public class GlobalExceptionMiddleware
  {
      private readonly RequestDelegate _next;
      private readonly ILogger<GlobalExceptionMiddleware> _logger;
  
      public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
      {
          _next = next;
          _logger = logger;
      }
  
      public async Task InvokeAsync(HttpContext context)
      {
          try
          {
              await _next(context); // 正常执行请求
          }
          catch (Exception ex)
          {
              // 拦截所有异常
              await HandleExceptionAsync(context, ex);
          }
      }
  
      /// <summary>
      /// 统一处理异常
      /// </summary>
      private async Task HandleExceptionAsync(HttpContext context, Exception ex)
      {
          // 1. 记录日志
          _logger.LogError(ex, "全局异常：{Message}", ex.Message);
  
          // 2. 设置返回格式
          context.Response.ContentType = "application/json";
          context.Response.StatusCode = (int)HttpStatusCode.OK;
  
          // 3. 统一返回错误信息
          var result = HttpResult.Fail(ex.Message);
          var json = JsonSerializer.Serialize(result);
  
          await context.Response.WriteAsync(json);
      }
  }
}
