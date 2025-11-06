using Microsoft.EntityFrameworkCore;
using webApitest.Data;
using webApitest.Models;

namespace webApitest.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(ApplicationDbContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogActivityAsync(int userId, string activity, string description, string? relatedEntityType = null, int? relatedEntityId = null, string? actionResult = null, string? additionalData = null)
        {
            try
            {
                var activityLog = new UserActivityLog
                {
                    UserId = userId,
                    Activity = activity,
                    Description = description,
                    RelatedEntityType = relatedEntityType,
                    RelatedEntityId = relatedEntityId,
                    ActionResult = actionResult,
                    AdditionalData = additionalData,
                    Timestamp = DateTime.UtcNow
                };

                _context.UserActivityLogs.Add(activityLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Activity logged: {activity} by user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to log activity: {activity} by user {userId}");
            }
        }

        public async Task LogActivityAsync(int userId, string activity, string description, HttpContext httpContext, string? relatedEntityType = null, int? relatedEntityId = null, string? actionResult = null, string? additionalData = null)
        {
            try
            {
                var ipAddress = GetClientIPAddress(httpContext);
                var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

                var activityLog = new UserActivityLog
                {
                    UserId = userId,
                    Activity = activity,
                    Description = description,
                    IPAddress = ipAddress,
                    UserAgent = userAgent,
                    RelatedEntityType = relatedEntityType,
                    RelatedEntityId = relatedEntityId,
                    ActionResult = actionResult,
                    AdditionalData = additionalData,
                    Timestamp = DateTime.UtcNow
                };

                _context.UserActivityLogs.Add(activityLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Activity logged: {activity} by user {userId} from IP {ipAddress}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to log activity: {activity} by user {userId}");
            }
        }

        public async Task<List<UserActivityLog>> GetActivityLogsAsync(int? userId = null, string? activity = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var query = _context.UserActivityLogs
                    .Include(log => log.User)
                    .AsQueryable();

                if (userId.HasValue)
                    query = query.Where(log => log.UserId == userId.Value);

                if (!string.IsNullOrEmpty(activity))
                    query = query.Where(log => log.Activity == activity);

                if (startDate.HasValue)
                    query = query.Where(log => log.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(log => log.Timestamp <= endDate.Value);

                return await query
                    .OrderByDescending(log => log.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve activity logs");
                return new List<UserActivityLog>();
            }
        }

        public async Task<List<UserActivityLog>> GetActivityLogsByEntityAsync(string entityType, int entityId)
        {
            try
            {
                return await _context.UserActivityLogs
                    .Include(log => log.User)
                    .Where(log => log.RelatedEntityType == entityType && log.RelatedEntityId == entityId)
                    .OrderByDescending(log => log.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve activity logs for {entityType} {entityId}");
                return new List<UserActivityLog>();
            }
        }

        private string GetClientIPAddress(HttpContext context)
        {
            // Check for forwarded IP first (for load balancers/proxies)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                    return ips[0].Trim();
            }

            // Check for real IP header
            var realIP = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
                return realIP;

            // Fall back to connection remote IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
