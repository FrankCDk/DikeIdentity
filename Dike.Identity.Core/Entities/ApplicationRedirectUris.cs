namespace Dike.Identity.Core.Entities
{
    public class ApplicationRedirectUris
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string RedirectUri { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Application? Application { get; set; }
    }
}
