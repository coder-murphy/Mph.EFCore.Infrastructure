using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using System.ComponentModel;
using System.Net;
using System.Text;
using Yitter.IdGenerator;
using StackExchange.Redis;
using Microsoft.AspNetCore.WebSockets;
using Mph.EFCore.Infrastructure.Common;
using Mph.EFCore.Infrastructure.DbContexts;
using Mph.EFCore.Infrastructure.Services;
using Mph.EFCore.Infrastucture.Middlewares;
using DateTimeConverter = Mph.EFCore.Infrastructure.Common.DateTimeConverter;
using Mph.EFCore.Infrastructure;

// 初始化雪花ID
YitIdHelper.SetIdGenerator(new IdGeneratorOptions
{
    WorkerId = 1,         // 机器ID（1-1023）
    WorkerIdBitLength = 6 // 默认即可
});

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddNLog(); // 加载 NLog.config

builder.Services.AddScoped<LogService>();
// 必须加这个！否则 WebRootPath = null
builder.Services.AddSingleton(builder.Environment);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse("your db config string");
    return ConnectionMultiplexer.Connect(config);
});

// 启用Redis
builder.Services.AddScoped<RedisService>();

// 配置跨域（必须放在最前面）
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()    // 允许所有来源（开发用）
              .AllowAnyHeader()    // 允许所有请求头
              .AllowAnyMethod()   // 允许所有请求方法 GET/POST/PUT
                                  // 加上这句，放行预检请求
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
    });
});
builder.Services.AddHttpContextAccessor();

// JWT 验证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddWebSockets(options =>
{

});

builder.Services.AddControllers(options =>
{
    // 让 .NET 自动放行 OPTIONS 预检请求
    options.Filters.Add<OptionsFilter>();
}).AddJsonOptions(options =>
{
    // 强制支持 "yyyy-MM-dd HH:mm:ss" 格式
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});

// Debug/开发环境禁用认证
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllers(options =>
    {
        // 全局允许匿名，忽略所有 [Authorize]
        options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter());
    });
}

// 以postgreSQL为例
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connStr = builder.Configuration["DefaultConnection"];
    // options.UseNpgsql(connStr);
});

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<AutoMapperProfile>();
});

// 添加文件清理服务
builder.Services.AddHostedService<FileCleanBackgroundService>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // 信任本机代理（nginx/iis）
    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
    options.KnownProxies.Add(IPAddress.Parse("::1"));
});

var app = builder.Build();


// 启用转发
app.UseForwardedHeaders();

// 🔥 注册全局异常拦截
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

// app.UseHttpsRedirection();

// 启用静态文件（wwwroot 生效）
app.UseStaticFiles();

// 启用websocket用于推送
app.UseWebSockets();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        SQLBuilder.InitAuditSql(dbContext.Database);
        //dbContext.Database.ExecuteSqlRaw();
    }
    catch (Exception ex)
    {
        // 记录错误，不影响程序启动
        Console.WriteLine("审计日志初始化失败: " + ex.Message);
    }
}

// 维护 WebSocket 连接

// WebSocket 中间件
app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var sid = context.Request.Query["sid"].ToString();
    var ws = await context.WebSockets.AcceptWebSocketAsync();

    // 添加到管理
    WebSocketService.AddSocket(sid, ws);

    // 保持连接
    await WebSocketService.Listen(sid, ws);
});

// 推送接口（以qrcode为例 如果有手机调用）
app.MapPost("/api/scan/push", async (WsRequest req) =>
{
    await WebSocketService.SendMessage(req.Sid, req);
    return Results.Ok();
});

app.Run();