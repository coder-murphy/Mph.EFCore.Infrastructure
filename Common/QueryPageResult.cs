namespace Mph.EFCore.Infrastructure.Common
{
    /// <summary>
    /// 分页查询结果
    /// </summary>
    public class QueryPageResult : HttpResult
    {
        public required QueryPageInfo PageInfo { get; set; }

        public static QueryPageResult Success(QueryPageInfo pageInfo, object data = null, string message = "")
        {
            return new QueryPageResult
            {
                IsSuccess = true,
                Code = 200,
                Data = data,
                Message = message,
                PageInfo = pageInfo,
            };
        }

        public static QueryPageResult Fail(QueryPageInfo pageInfo, object data = null, string message = "")
        {
            return new QueryPageResult
            {
                IsSuccess = false,
                Code = 400,
                Data = data,
                Message = message,
                PageInfo = pageInfo,
            };
        }
    }
}
