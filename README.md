building...

init:

``` csharp
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

// 初始化雪花ID
YitIdHelper.SetIdGenerator(new IdGeneratorOptions
{
    WorkerId = 1,         // 机器ID（1-1023）
    WorkerIdBitLength = 6 // 默认即可
});
```
