using System.ComponentModel.DataAnnotations;

namespace Mph.EFCore.Infrastructure.Common
{
    public class ScanCodeOptions
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string Sid { get; set; }

        [Required]
        public string Command { get; set; }

        public Dictionary<string, string> CommandParameters { get; set; }

        public Dictionary<string, string> MetaData { get; set; }
    }
}
