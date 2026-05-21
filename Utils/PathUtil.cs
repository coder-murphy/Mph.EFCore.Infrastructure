namespace Mph.EFCore.Infrastructure.Utils
{
    /// <summary>
    /// 路径工具类 获取各类服务路径
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// 获取 wwwroot 路径
        /// </summary>
        /// <returns></returns>
        public static string GetWebRootPath()
        {
            // 取程序基目录
            var basePath = AppContext.BaseDirectory;

            // 拼接 wwwroot（稳定、安全、永远不为null）
            var wwwroot = Path.Combine(basePath, "wwwroot");

            // 不存在就创建
            if (!Directory.Exists(wwwroot))
                Directory.CreateDirectory(wwwroot);

            return wwwroot;
        }
    }
}
