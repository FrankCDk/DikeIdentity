namespace Dike.Identity.Core.Entities
{
    public class ApplicationCorsOrigins
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string OriginUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navegation
        public Application? Application { get; set; }
    }
}
