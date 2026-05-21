using Mph.EFCore.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mph.EFCore.Infrastructure.DbContexts;

namespace Mph.EFCore.Infrastructure.Services
{
    /// <summary>
    /// 后台文件自动清理服务
    /// </summary>
    public class FileCleanBackgroundService : BackgroundService
    {
        public FileCleanBackgroundService(IServiceProvider serviceProvider,ILogger<FileCleanBackgroundService> logger) 
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {   
            // 启动后延迟10秒再开始，不阻塞启动
            await Task.Delay(10000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    // 每天凌晨 4 点才执行
                    if (now.Hour == 4 && now.Minute == 0)
                    {
                        _logger.LogInformation("开始执行每日文件清理");
                        await ClearOrphanFilesAsync();

                        // 避免1分钟内重复执行
                        await Task.Delay(60000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Clear orphan files failed.");
                }

                // 每 20 秒检查一次时间，不卡启动
                await Task.Delay(20000, stoppingToken);
            }
        }

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileCleanBackgroundService> _logger;

        private async Task ClearOrphanFilesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            string uploadRoot = Path.Combine(PathUtil.GetWebRootPath(), "Uploads");
            if (!Directory.Exists(uploadRoot)) return;

            var allFilePaths = Directory.GetFiles(uploadRoot, "*.*", SearchOption.AllDirectories);
            if (allFilePaths.Length == 0) return;

            // 数据库合法文件路径集合
            // eg.
            var dbPaths = new string[] { "Cache" };
            //await db.Files
            //    .Select(x => x.FilePath)
            //    .ToListAsync();

            // 只删：无数据库记录 + 超过12小时
            foreach (var file in allFilePaths)
            {
                if (!dbPaths.Contains(file))
                {
                    var fi = new FileInfo(file);
                    if (fi.CreationTime < DateTime.Now.AddHours(-12))
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }

            // 清理空文件夹
            ClearEmptyDir(uploadRoot);
        }

        private static void ClearEmptyDir(string path)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                ClearEmptyDir(dir);
                if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                {
                    Directory.Delete(dir);
                }
            }
        }
    }
}
