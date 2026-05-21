using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Mph.EFCore.Infrastructure.Model
{
    /// <summary>
    /// 数据库通讯所需要模型基类
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public abstract class ModelBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        public required virtual long Id { get; set; }

        /// <summary>
        /// 冗余字段(备用)
        /// </summary>
        public string? Tag { get; set; }
    }
}
