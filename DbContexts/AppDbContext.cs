using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mph.EFCore.Infrastructure.DbContexts
{
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// 必须加空构造
        /// </summary>
        public AppDbContext() { }

        public AppDbContext(IConfiguration configuration, DbContextOptions<AppDbContext> options) : base(options)
        {
            Configuration = configuration;
        }

        protected readonly IConfiguration Configuration;

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // 强制所有 DateTime 类型自动转 UTC
            configurationBuilder.Properties<DateTime>()
                .HaveConversion<DateTimeToUtcConverter>();

            // 可空 DateTime 也一起处理
            configurationBuilder.Properties<DateTime?>()
                .HaveConversion<DateTimeToUtcConverter>();
        }

        /// <summary>
        /// 转换器：自动把 Local/Unspecified 转成 UTC
        /// </summary>
        public class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
        {
            public DateTimeToUtcConverter() : base(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            )
            { }
        }
    }
}
