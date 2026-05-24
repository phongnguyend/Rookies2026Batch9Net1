using System.ComponentModel.DataAnnotations;

namespace NashAssetManagement.WebAPI.Configuration.Cors
{
    public sealed class CorsOptions
    {
        public const string SectionName = "Cors";

        [Required]
        public string[] AllowedOrigins { get; set; } = [];
    }
}