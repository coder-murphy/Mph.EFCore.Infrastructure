using System.ComponentModel.DataAnnotations;

namespace Mph.EFCore.Infrastructure.Common
{
    public class QueryPageModel
    {
        /// <summary>
        /// 当前页索引
        /// </summary>
        [Required]
        public int PageNum { get; set; } = 0;

        /// <summary>
        /// 每页条数
        /// </summary>
        [Required]
        public int PageSize { get; set; } = 50;

        public DateTime CreatedFrom { get; set; }

        public DateTime? UpdatedFrom { get; set; }

        public DateTime? DeletedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public DateTime? UpdatedTo { get; set; }

        public DateTime? DeletedTo { get; set; }
    }
}
