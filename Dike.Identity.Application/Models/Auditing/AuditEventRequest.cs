namespace Dike.Identity.Application.Models.Auditing
{
    public class AuditEventRequest
    {
        public string EventType { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? EntityName { get; set; }
        public string? EntityId { get; set; }

        public bool IsSuccessful { get; set; }
        public int? StatusCode { get; set; }

        public string? AdditionalData { get; set; }
    }
}
