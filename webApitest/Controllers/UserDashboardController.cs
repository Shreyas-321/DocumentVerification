using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using webApitest.Data;
using webApitest.Models;
using System.Security.Claims;
using DocumentVerificationDLL;
using webApitest.Services;

namespace webApitest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDashboardController> _logger;
        private readonly IDocumentVerificationDLL _documentVerificationDLL;
        private readonly IAuditService _auditService;

        public UserDashboardController(ApplicationDbContext context, ILogger<UserDashboardController> logger, IDocumentVerificationDLL documentVerificationDLL, IAuditService auditService)
        {
            _context = context;
            _logger = logger;
            _documentVerificationDLL = documentVerificationDLL;
            _auditService = auditService;
        }

        // Get current user ID from JWT token
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;
            throw new UnauthorizedAccessException("Invalid user token");
        }

        [HttpPost("upload-documents")]
        [Authorize]
        public async Task<ActionResult> UploadDocuments([FromForm] DocumentUploadDto uploadDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Validate files
                if (uploadDto.EC == null || uploadDto.Aadhaar == null || uploadDto.PAN == null)
                    return BadRequest("All three documents (EC, Aadhaar, PAN) are required");

                // Validate EC document (PDF only)
                if (!IsValidPDF(uploadDto.EC))
                    return BadRequest("EC document must be a PDF file (max 10MB)");

                // Validate Aadhaar document (PNG only)
                if (!IsValidPNG(uploadDto.Aadhaar))
                    return BadRequest("Aadhaar document must be a PNG file (max 10MB)");

                // Validate PAN document (PNG only)
                if (!IsValidPNG(uploadDto.PAN))
                    return BadRequest("PAN document must be a PNG file (max 10MB)");

                // Create uploads directory if not exists
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "UserDocuments", userId.ToString());
                Directory.CreateDirectory(uploadsPath);

                // Save files with timestamp
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var ecPath = Path.Combine(uploadsPath, $"EC_{timestamp}.pdf");
                var aadhaarPath = Path.Combine(uploadsPath, $"Aadhaar_{timestamp}.png");
                var panPath = Path.Combine(uploadsPath, $"PAN_{timestamp}.png");

                using (var stream = new FileStream(ecPath, FileMode.Create))
                    await uploadDto.EC.CopyToAsync(stream);

                using (var stream = new FileStream(aadhaarPath, FileMode.Create))
                    await uploadDto.Aadhaar.CopyToAsync(stream);

                using (var stream = new FileStream(panPath, FileMode.Create))
                    await uploadDto.PAN.CopyToAsync(stream);

                // Create UserUploadedDocuments record
                var uploadedDoc = new UserUploadedDocuments
                {
                    UserId = userId,
                    ECPath = ecPath,
                    AadhaarPath = aadhaarPath,
                    PANPath = panPath,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserUploadedDocuments.Add(uploadedDoc);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Documents uploaded successfully for user {userId}. Files saved to: EC={ecPath}, Aadhaar={aadhaarPath}, PAN={panPath}");

                return Ok(new
                {
                    message = "Documents uploaded successfully",
                    uploadId = uploadedDoc.Id,
                    files = new
                    {
                        EC = Path.GetFileName(ecPath),
                        Aadhaar = Path.GetFileName(aadhaarPath),
                        PAN = Path.GetFileName(panPath)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading documents");
                return StatusCode(500, "Internal server error");
            }
        }

        // Validate PDF file
        private bool IsValidPDF(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size (10MB limit)
            if (file.Length > 10 * 1024 * 1024)
                return false;

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".pdf")
                return false;

            // Check MIME type
            if (file.ContentType != "application/pdf")
                return false;

            return true;
        }

        // Validate PNG file
        private bool IsValidPNG(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size (10MB limit)
            if (file.Length > 10 * 1024 * 1024)
                return false;

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".png")
                return false;

            // Check MIME type
            if (file.ContentType != "image/png")
                return false;

            return true;
        }

        [HttpGet("document-status")]
        [Authorize]
        public async Task<ActionResult> GetDocumentStatus()
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get the latest document upload for this user
                var latestUpload = await _context.UserUploadedDocuments
                    .Where(u => u.UserId == userId)
                    .OrderByDescending(u => u.CreatedAt)
                    .FirstOrDefaultAsync();

                if (latestUpload == null)
                {
                    return Ok(new
                    {
                        hasDocuments = false,
                        message = "No documents uploaded yet",
                        isVerified = false,
                        verificationStatus = "No documents",
                        uploadDate = (DateTime?)null
                    });
                }

                // Check if documents have been extracted
                var hasExtractedData = await _context.ExtractedData
                    .AnyAsync(e => e.UploadId == latestUpload.Id);

                // Check verification status
                var verificationResult = await _context.VerificationResults
                    .Where(v => v.UploadId == latestUpload.Id)
                    .FirstOrDefaultAsync();

                var isVerified = verificationResult?.OverallMatch ?? false;
                var verificationStatus = verificationResult?.Status ?? "Pending";
                var riskScore = verificationResult?.RiskScore ?? 0;

                return Ok(new
                {
                    hasDocuments = true,
                    uploadId = latestUpload.Id,
                    uploadDate = latestUpload.CreatedAt,
                    hasExtractedData = hasExtractedData,
                    isVerified = isVerified,
                    verificationStatus = verificationStatus,
                    riskScore = riskScore,
                    ecPath = !string.IsNullOrEmpty(latestUpload.ECPath),
                    aadhaarPath = !string.IsNullOrEmpty(latestUpload.AadhaarPath),
                    panPath = !string.IsNullOrEmpty(latestUpload.PANPath)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document status for user {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error while retrieving document status");
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult> GetUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();

                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new
                    {
                        u.Id,
                        u.FullName,
                        u.Email,
                        u.Phone,
                        u.City,
                        u.State,
                        u.Pincode,
                        u.Role,
                        u.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for user {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error while retrieving user profile");
            }
        }

        [HttpGet("upload-history")]
        [Authorize]
        public async Task<ActionResult> GetUploadHistory()
        {
            try
            {
                var userId = GetCurrentUserId();

                var uploadHistory = await _context.UserUploadedDocuments
                    .Where(u => u.UserId == userId)
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new
                    {
                        u.Id,
                        u.CreatedAt,
                        u.ECPath,
                        u.AadhaarPath,
                        u.PANPath,
                        HasExtractedData = _context.ExtractedData.Any(e => e.UploadId == u.Id),
                        VerificationStatus = _context.VerificationResults
                            .Where(v => v.UploadId == u.Id)
                            .Select(v => v.Status)
                            .FirstOrDefault() ?? "Pending",
                        RiskScore = _context.VerificationResults
                            .Where(v => v.UploadId == u.Id)
                            .Select(v => v.RiskScore)
                            .FirstOrDefault(),
                        IsVerified = _context.VerificationResults
                            .Where(v => v.UploadId == u.Id)
                            .Select(v => v.OverallMatch)
                            .FirstOrDefault() ?? false
                    })
                    .ToListAsync();

                return Ok(new
                {
                    uploads = uploadHistory,
                    totalCount = uploadHistory.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upload history for user {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error while retrieving upload history");
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileData)
        {
        try
        {
            var currentUserId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update user profile fields
            if (!string.IsNullOrEmpty(profileData.FullName))
                user.FullName = profileData.FullName;
            
            if (!string.IsNullOrEmpty(profileData.Phone))
                user.Phone = profileData.Phone;
            
            if (!string.IsNullOrEmpty(profileData.City))
                user.City = profileData.City;
            
            if (!string.IsNullOrEmpty(profileData.State))
                user.State = profileData.State;
            
            if (!string.IsNullOrEmpty(profileData.Pincode))
                user.Pincode = profileData.Pincode;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Log the activity
            await _auditService.LogActivityAsync(
                currentUserId,
                "Profile Updated",
                "User updated their profile information",
                "User",
                currentUserId,
                "Success",
                $"User updated their profile information"
            );

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile", error = ex.Message });
            }
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordData)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(passwordData.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            // Validate new password
            if (string.IsNullOrEmpty(passwordData.NewPassword) || passwordData.NewPassword.Length < 6)
            {
                return BadRequest(new { message = "New password must be at least 6 characters long" });
            }

            if (passwordData.NewPassword != passwordData.ConfirmPassword)
            {
                return BadRequest(new { message = "New password and confirm password do not match" });
            }

            // Hash and update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordData.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Log the activity
            await _auditService.LogActivityAsync(
                currentUserId,
                "Password Changed",
                "User changed their password",
                "User",
                currentUserId,
                "Success",
                "Password successfully updated"
            );

            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { message = "An error occurred while changing password", error = ex.Message });
        }
    }

    // DTO for change password
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // DTO for profile update
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
    }

        // DTO for document upload
        public class DocumentUploadDto
        {
            public IFormFile EC { get; set; } = null!;
            public IFormFile Aadhaar { get; set; } = null!;
            public IFormFile PAN { get; set; } = null!;
        }
    }
}