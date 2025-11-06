using webApitest.Models;

namespace webApitest.Services
{
    public interface IAuditService
    {
        Task LogActivityAsync(int userId, string activity, string description, string? relatedEntityType = null, int? relatedEntityId = null, string? actionResult = null, string? additionalData = null);
        Task LogActivityAsync(int userId, string activity, string description, HttpContext httpContext, string? relatedEntityType = null, int? relatedEntityId = null, string? actionResult = null, string? additionalData = null);
        Task<List<UserActivityLog>> GetActivityLogsAsync(int? userId = null, string? activity = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50);
        Task<List<UserActivityLog>> GetActivityLogsByEntityAsync(string entityType, int entityId);
    }
}
