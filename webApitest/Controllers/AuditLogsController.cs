using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using webApitest.Services;
using System.Security.Claims;

namespace webApitest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Only authenticated users can access
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(IAuditService auditService, ILogger<AuditLogsController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        // Get current user ID from JWT token
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new UnauthorizedAccessException("Invalid user token");
        }

        // Check if current user is admin
        private bool IsAdmin()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim == "Admin";
        }

        /// <summary>
        /// Get all audit logs (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int? userId = null,
            [FromQuery] string? activity = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // Only admins can view all audit logs
                if (!IsAdmin())
                {
                    return StatusCode(403, "Only administrators can view audit logs");
                }

                var currentUserId = GetCurrentUserId();
                
                // Log admin viewing audit logs
                await _auditService.LogActivityAsync(currentUserId, "View Audit Logs", 
                    $"Admin viewed audit logs with filters: UserId={userId}, Activity={activity}, StartDate={startDate}, EndDate={endDate}", 
                    HttpContext, null, null, "Success", 
                    $"Page: {page}, PageSize: {pageSize}");

                var logs = await _auditService.GetActivityLogsAsync(userId, activity, startDate, endDate, page, pageSize);

                return Ok(new
                {
                    logs = logs.Select(log => new
                    {
                        log.Id,
                        log.UserId,
                        UserName = log.User?.FullName,
                        UserEmail = log.User?.Email,
                        log.Activity,
                        log.Description,
                        log.IPAddress,
                        log.UserAgent,
                        log.RelatedEntityType,
                        log.RelatedEntityId,
                        log.ActionResult,
                        log.AdditionalData,
                        log.Timestamp
                    }),
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount = logs.Count
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get audit logs for a specific user (Admin only)
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAuditLogsByUser(int userId)
        {
            try
            {
                // Only admins can view audit logs
                if (!IsAdmin())
                {
                    return StatusCode(403, "Only administrators can view audit logs");
                }

                var currentUserId = GetCurrentUserId();
                
                // Log admin viewing user-specific audit logs
                await _auditService.LogActivityAsync(currentUserId, "View User Audit Logs", 
                    $"Admin viewed audit logs for user ID: {userId}", 
                    HttpContext, "User", userId, "Success");

                var logs = await _auditService.GetActivityLogsAsync(userId, null, null, null, 1, 100);

                return Ok(new
                {
                    userId,
                    logs = logs.Select(log => new
                    {
                        log.Id,
                        log.UserId,
                        UserName = log.User?.FullName,
                        UserEmail = log.User?.Email,
                        log.Activity,
                        log.Description,
                        log.IPAddress,
                        log.UserAgent,
                        log.RelatedEntityType,
                        log.RelatedEntityId,
                        log.ActionResult,
                        log.AdditionalData,
                        log.Timestamp
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving audit logs for user {userId}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get current user's own audit logs (User can see their own activity)
        /// </summary>
        [HttpGet("my-activity")]
        public async Task<IActionResult> GetMyAuditLogs(
            [FromQuery] string? activity = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Log user viewing their own audit logs
                await _auditService.LogActivityAsync(currentUserId, "View My Activity", 
                    $"User viewed their own audit logs with filters: Activity={activity}, StartDate={startDate}, EndDate={endDate}", 
                    HttpContext, null, null, "Success", 
                    $"Page: {page}, PageSize: {pageSize}");

                var logs = await _auditService.GetActivityLogsAsync(currentUserId, activity, startDate, endDate, page, pageSize);

                return Ok(new
                {
                    userId = currentUserId,
                    logs = logs.Select(log => new
                    {
                        log.Id,
                        log.UserId,
                        UserName = log.User?.FullName,
                        UserEmail = log.User?.Email,
                        log.Activity,
                        log.Description,
                        log.IPAddress,
                        log.UserAgent,
                        log.RelatedEntityType,
                        log.RelatedEntityId,
                        log.ActionResult,
                        log.AdditionalData,
                        log.Timestamp
                    }),
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount = logs.Count
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs for current user");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get audit logs for a specific activity type (Admin only)
        /// </summary>
        [HttpGet("activity/{activity}")]
        public async Task<IActionResult> GetAuditLogsByActivity(string activity)
        {
            try
            {
                // Only admins can view audit logs
                if (!IsAdmin())
                {
                    return StatusCode(403, "Only administrators can view audit logs");
                }

                var currentUserId = GetCurrentUserId();
                
                // Log admin viewing activity-specific audit logs
                await _auditService.LogActivityAsync(currentUserId, "View Activity Audit Logs", 
                    $"Admin viewed audit logs for activity: {activity}", 
                    HttpContext, null, null, "Success");

                var logs = await _auditService.GetActivityLogsAsync(null, activity, null, null, 1, 100);

                return Ok(new
                {
                    activity,
                    logs = logs.Select(log => new
                    {
                        log.Id,
                        log.UserId,
                        UserName = log.User?.FullName,
                        UserEmail = log.User?.Email,
                        log.Activity,
                        log.Description,
                        log.IPAddress,
                        log.UserAgent,
                        log.RelatedEntityType,
                        log.RelatedEntityId,
                        log.ActionResult,
                        log.AdditionalData,
                        log.Timestamp
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving audit logs for activity {activity}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get audit logs for a specific entity (Admin only)
        /// </summary>
        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<IActionResult> GetAuditLogsByEntity(string entityType, int entityId)
        {
            try
            {
                // Only admins can view audit logs
                if (!IsAdmin())
                {
                    return StatusCode(403, "Only administrators can view audit logs");
                }

                var currentUserId = GetCurrentUserId();
                
                // Log admin viewing entity-specific audit logs
                await _auditService.LogActivityAsync(currentUserId, "View Entity Audit Logs", 
                    $"Admin viewed audit logs for {entityType} ID: {entityId}", 
                    HttpContext, entityType, entityId, "Success");

                var logs = await _auditService.GetActivityLogsByEntityAsync(entityType, entityId);

                return Ok(new
                {
                    entityType,
                    entityId,
                    logs = logs.Select(log => new
                    {
                        log.Id,
                        log.UserId,
                        UserName = log.User?.FullName,
                        UserEmail = log.User?.Email,
                        log.Activity,
                        log.Description,
                        log.IPAddress,
                        log.UserAgent,
                        log.RelatedEntityType,
                        log.RelatedEntityId,
                        log.ActionResult,
                        log.AdditionalData,
                        log.Timestamp
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving audit logs for {entityType} {entityId}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get audit logs within a date range (Admin only)
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetAuditLogsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // Only admins can view audit logs
                if (!IsAdmin())
                {
                    return StatusCode(403, "Only administrators can view audit logs");
                }

                var currentUserId = GetCurrentUserId();
                
                // Log admin viewing date-range audit logs
                await _auditService.LogActivityAsync(currentUserId, "View Date Range Audit Logs", 
                    $"Admin viewed audit logs from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", 
                    HttpContext, null, null, "Success");

                var logs = await _auditService.GetActivityLogsAsync(null, null, startDate, endDate, page, pageSize);

                return Ok(new
                {
                    startDate,
                    endDate,
                    logs = logs.Select(log => new
                    {
                        log.Id,
                        log.UserId,
                        UserName = log.User?.FullName,
                        UserEmail = log.User?.Email,
                        log.Activity,
                        log.Description,
                        log.IPAddress,
                        log.UserAgent,
                        log.RelatedEntityType,
                        log.RelatedEntityId,
                        log.ActionResult,
                        log.AdditionalData,
                        log.Timestamp
                    }),
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount = logs.Count
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving audit logs for date range {startDate} to {endDate}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
