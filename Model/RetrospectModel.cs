
namespace Mph.EFCore.Infrastructure.Model
{
    /// <summary>
    /// 追溯模型
    /// </summary>
    public class RetrospectModel : ModelBase
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreatedBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 更新人ID
        /// </summary>
        public long UpdatedBy { get; set; }

        /// <summary>
        /// 软删除时间（NULL表示未删除）
        /// </summary>
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        public long DeletedBy { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
