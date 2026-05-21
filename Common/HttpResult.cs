namespace Mph.EFCore.Infrastructure.Common
{
    public class HttpResult
    {
        public bool IsSuccess { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        /// <summary>
        /// 成功返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResult Success(string message = "",object? data = null)
        {
            var res = new HttpResult
            {
                Code = 200,
                Message = message,
                IsSuccess = true,
                Data = data
            };
            return res;
        }

        /// <summary>
        /// 失败返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResult Fail(string message = "",object? data = null)
        {
            var res = new HttpResult
            {
                Code = 400,
                Message = message,
                IsSuccess = false,
                Data = data
            };
            return res;
        }
    }
}
