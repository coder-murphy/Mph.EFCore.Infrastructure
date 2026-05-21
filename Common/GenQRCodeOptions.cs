using System.ComponentModel.DataAnnotations;

namespace Mph.EFCore.Infrastructure.Common
{
    /// <summary>
    /// Represents the options used to generate a scan code, including the code type and associated tags.
    /// </summary>
    public class GenQRCodeOptions
    {
        /// <summary>
        /// Gets or sets the host name or address for the connection.
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the security identifier (SID) associated with the entity.
        /// </summary>
        [Required]
        public string Sid { get; set; }

        /// <summary>
        /// Gets or sets the command to be executed.
        /// </summary>
        /// <remarks>This property is required. The value should specify the exact command or instruction
        /// that will be processed by the consuming component.</remarks>
        [Required]
        public string Command  { get; set; }

        /// <summary>
        /// Gets or sets the collection of parameters to be passed to the command.
        /// </summary>
        public Dictionary<string, string> CommandParameters { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags associated with the current item.
        /// </summary>
        public Dictionary<string,string> MetaData { get; set; }
    }
}
