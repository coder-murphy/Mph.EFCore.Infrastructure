namespace Mph.EFCore.Infrastructure.Common
{
    /// <summary>
    /// 批量删除信息
    /// </summary>
    public class BatchDeleteInfo
    {
        /// <summary>
        /// Gets or sets the collection of identifiers representing the items to be deleted.
        /// </summary>
        public required long[] DeleteList { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public long UserId { get; set; }
    }
}
