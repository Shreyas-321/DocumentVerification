using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webApitest.Data;
using webApitest.Models;

namespace webApitest.Services
{
    public interface IVerificationService
    {
        Task<VerificationResults> VerifyAsync(int uploadId);
    }

    public class VerificationService : IVerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VerificationService> _logger;

        public VerificationService(ApplicationDbContext context, ILogger<VerificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<VerificationResults> VerifyAsync(int uploadId)
        {
            _logger.LogInformation($"Starting verification for uploadId: {uploadId}");

            // Step 2a: Get Extracted Data
            var extracted = await _context.ExtractedData
                .FirstOrDefaultAsync(e => e.UploadId == uploadId);

            if (extracted == null)
            {
                _logger.LogWarning($"No extracted data found for uploadId: {uploadId}");
                throw new Exception("No extracted data found for this upload ID. Please extract documents first.");
            }

            _logger.LogInformation($"Found extracted data for uploadId: {uploadId}");

            // Step 2b: Look up Original Data
            var originalAadhaar = await _context.OriginalAadhaarData
                .FirstOrDefaultAsync(a => a.AadhaarNo == extracted.AadhaarNo);

            var originalPAN = await _context.OriginalPANData
                .FirstOrDefaultAsync(p => p.PANNo == extracted.PANNo);

            var originalEC = await _context.OriginalECData
                .FirstOrDefaultAsync(e => e.SurveyNo == extracted.SurveyNo);

            _logger.LogInformation($"Original data lookup completed for uploadId: {uploadId}");

            // Step 2c: Compare Fields
            var verificationResult = new VerificationResults
            {
                UploadId = uploadId,
                AadhaarNameMatch = originalAadhaar != null ? 
                    string.Equals(extracted.AadhaarName?.Trim(), originalAadhaar.AadhaarName?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                AadhaarNoMatch = originalAadhaar != null ? 
                    string.Equals(extracted.AadhaarNo?.Trim(), originalAadhaar.AadhaarNo?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                DOBMatch = originalAadhaar != null ? 
                    CompareDates(extracted.DOB, originalAadhaar.DOB.ToString("dd/MM/yyyy")) : null,
                PANNameMatch = originalPAN != null ? 
                    string.Equals(extracted.PANName?.Trim(), originalPAN.PANName?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                PANNoMatch = originalPAN != null ? 
                    string.Equals(extracted.PANNo?.Trim(), originalPAN.PANNo?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                ApplicationNumberMatch = !string.IsNullOrEmpty(extracted.ApplicationNumber),
                ApplicantNameMatch = !string.IsNullOrEmpty(extracted.ApplicantName),
                ApplicantAddressMatch = !string.IsNullOrEmpty(extracted.ApplicantAddress),
                SurveyNoMatch = originalEC != null ? 
                    string.Equals(extracted.SurveyNo?.Trim(), originalEC.SurveyNo?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                MeasuringAreaMatch = originalEC != null ? 
                    string.Equals(extracted.MeasuringArea?.Trim(), originalEC.MeasuringArea?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                VillageMatch = originalEC != null ? 
                    string.Equals(extracted.Village?.Trim(), originalEC.Village?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                HobliMatch = originalEC != null ? 
                    string.Equals(extracted.Hobli?.Trim(), originalEC.Hobli?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                TalukMatch = originalEC != null ? 
                    string.Equals(extracted.Taluk?.Trim(), originalEC.Taluk?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                DistrictMatch = originalEC != null ? 
                    string.Equals(extracted.District?.Trim(), originalEC.District?.Trim(), StringComparison.OrdinalIgnoreCase) : null,
                VerifiedAt = DateTime.UtcNow
            };

            _logger.LogInformation($"Field comparison completed for uploadId: {uploadId}");

            // Step 2d: Calculate Results
            var matches = new List<bool>();
            if (verificationResult.AadhaarNameMatch.HasValue) matches.Add(verificationResult.AadhaarNameMatch.Value);
            if (verificationResult.AadhaarNoMatch.HasValue) matches.Add(verificationResult.AadhaarNoMatch.Value);
            if (verificationResult.DOBMatch.HasValue) matches.Add(verificationResult.DOBMatch.Value);
            if (verificationResult.PANNameMatch.HasValue) matches.Add(verificationResult.PANNameMatch.Value);
            if (verificationResult.PANNoMatch.HasValue) matches.Add(verificationResult.PANNoMatch.Value);
            if (verificationResult.SurveyNoMatch.HasValue) matches.Add(verificationResult.SurveyNoMatch.Value);
            if (verificationResult.VillageMatch.HasValue) matches.Add(verificationResult.VillageMatch.Value);
            if (verificationResult.DistrictMatch.HasValue) matches.Add(verificationResult.DistrictMatch.Value);

            var matchCount = matches.Count(m => m);
            var totalChecks = matches.Count;
            var matchPercentage = totalChecks > 0 ? (decimal)matchCount / totalChecks * 100 : 0;

            verificationResult.OverallMatch = matchPercentage >= 70;
            verificationResult.RiskScore = 100 - matchPercentage;
            verificationResult.Status = verificationResult.OverallMatch == true ? "Verified" : "Rejected";

            _logger.LogInformation($"Results calculated for uploadId: {uploadId} - Status: {verificationResult.Status}, Risk Score: {verificationResult.RiskScore}");

            // Step 2e: Save to Database
            _context.VerificationResults.Add(verificationResult);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Verification results saved to database for uploadId: {uploadId}");

            return verificationResult;
        }

        private bool CompareDates(string extractedDate, string originalDate)
        {
            try
            {
                if (string.IsNullOrEmpty(extractedDate) || string.IsNullOrEmpty(originalDate))
                    return false;

                // Try to parse extracted date (might be in different formats)
                DateTime extracted;
                DateTime original;

                if (DateTime.TryParse(extractedDate, out extracted) && DateTime.TryParse(originalDate, out original))
                {
                    return extracted.Date == original.Date;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}