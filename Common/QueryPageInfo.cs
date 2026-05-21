using System.ComponentModel.DataAnnotations;

namespace Mph.EFCore.Infrastructure.Common
{
    public class QueryPageInfo
    {
        public QueryPageInfo(int pageNum, int pageSize, long total)
        {
            PageNum = pageNum;
            PageSize = pageSize;
            Total = total;
        }

        /// <summary>
        /// 当前页索引
        /// </summary>
        public int PageNum { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 返回一个计算后的分页信息对象
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static QueryPageInfo Calculate(int pageNum, int pageSize, long total)
        {
            var info = new QueryPageInfo(pageNum, pageSize, total);
            if (pageSize > 0)
            {
                info.PageCount = (int)Math.Ceiling((double)total / pageSize);
            }
            else
            {
                info.PageCount = 0;
            }
            return info;
        }
    }
}
