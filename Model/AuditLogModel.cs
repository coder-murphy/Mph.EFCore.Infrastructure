using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Mph.EFCore.Infrastructure.Enums;

namespace Mph.EFCore.Infrastructure.Model
{
    /// <summary>
    /// 审计日志
    /// </summary>
    [Index(nameof(TableName),nameof(RecordId))]
    [Index(nameof(OperatorId))]
    [Index(nameof(OperatedAt))]
    public class AuditLogModel : ModelBase
    {
        /// <summary>
        /// 表名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public required string TableName { get; set; }

        /// <summary>
        /// 记录ID
        /// </summary>
        [Required]
        public required long RecordId { get; set; }

        /// <summary>
        ///操作类型
        /// </summary>
        [Required]
        public required EDataActions Action { get; set; }

        /// <summary>
        /// 旧值
        /// </summary>
        public string OldValues { get; set; }

        /// <summary>
        /// 新值
        /// </summary>
        public string NewValues { get; set; }

        /// <summary>
        /// 变更字段列表
        /// </summary>
        public string ChangedFields { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long? OperatorId { get; set; }

        /// <summary>
        /// 操作IP
        /// </summary>
        public string OperatorIp { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Required]
        public required DateTime OperatedAt { get; set; } = DateTime.Now;
    }
}
