using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VerificationDLL.Models;

namespace VerificationDLL
{
    public class VerificationDLL : IVerificationDLL
    {
        private readonly VerificationDbContext _context;
        private readonly ILogger<VerificationDLL> _logger;

        public VerificationDLL(VerificationDbContext context, ILogger<VerificationDLL> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Models.VerificationResults> VerifyAsync(int uploadId)
        {
            _logger.LogInformation($"Starting verification for uploadId: {uploadId}");

            var extracted = await _context.ExtractedData
                .FirstOrDefaultAsync(e => e.UploadId == uploadId);

            if (extracted == null)
            {
                _logger.LogWarning($"No extracted data found for uploadId: {uploadId}");
                throw new Exception("No extracted data found for this upload ID. Please extract documents first.");
            }

            var originalAadhaar = await _context.OriginalAadhaarData
                .FirstOrDefaultAsync(a => a.AadhaarNo == extracted.AadhaarNo);

            var originalPAN = await _context.OriginalPANData
                .FirstOrDefaultAsync(p => p.PANNo == extracted.PANNo);

            var originalEC = await _context.OriginalECData
                .FirstOrDefaultAsync(e => e.SurveyNo == extracted.SurveyNo);

            var verificationResult = await _context.VerificationResults
                .FirstOrDefaultAsync(v => v.UploadId == uploadId);

            if (verificationResult == null)
            {
                verificationResult = new Models.VerificationResults
                {
                    UploadId = uploadId
                };
                _context.VerificationResults.Add(verificationResult);
            }

            verificationResult.AadhaarNameMatch = originalAadhaar != null ?
                string.Equals(extracted.AadhaarName?.Trim(), originalAadhaar.AadhaarName?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.AadhaarNoMatch = originalAadhaar != null ?
                string.Equals(extracted.AadhaarNo?.Trim(), originalAadhaar.AadhaarNo?.Trim(), StringComparison.OrdinalIgnoreCase) : null;

            verificationResult.DOBMatch = originalAadhaar != null ?
                    string.Equals(extracted.DOB, originalAadhaar.DOB.Trim(), StringComparison.OrdinalIgnoreCase) : null;

            verificationResult.PANNameMatch = originalPAN != null ?
                string.Equals(extracted.PANName?.Trim(), originalPAN.PANName?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.PANNoMatch = originalPAN != null ?
                string.Equals(extracted.PANNo?.Trim(), originalPAN.PANNo?.Trim(), StringComparison.OrdinalIgnoreCase) : null;

            verificationResult.ApplicationNumberMatch = !string.IsNullOrEmpty(extracted.ApplicationNumber);
            verificationResult.ApplicantNameMatch = !string.IsNullOrEmpty(extracted.ApplicantName);
            verificationResult.ApplicantAddressMatch = !string.IsNullOrEmpty(extracted.ApplicantAddress);

            verificationResult.SurveyNoMatch = originalEC != null ?
                string.Equals(extracted.SurveyNo?.Trim(), originalEC.SurveyNo?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.MeasuringAreaMatch = originalEC != null ?
                string.Equals(extracted.MeasuringArea?.Trim(), originalEC.MeasuringArea?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.VillageMatch = originalEC != null ?
                string.Equals(extracted.Village?.Trim(), originalEC.Village?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.HobliMatch = originalEC != null ?
                string.Equals(extracted.Hobli?.Trim(), originalEC.Hobli?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.TalukMatch = originalEC != null ?
                string.Equals(extracted.Taluk?.Trim(), originalEC.Taluk?.Trim(), StringComparison.OrdinalIgnoreCase) : null;
            verificationResult.DistrictMatch = originalEC != null ?
                string.Equals(extracted.District?.Trim(), originalEC.District?.Trim(), StringComparison.OrdinalIgnoreCase) : null;

            verificationResult.VerifiedAt = DateTime.UtcNow;

            var matches = new List<bool>();
            if (verificationResult.AadhaarNameMatch.HasValue) matches.Add(verificationResult.AadhaarNameMatch.Value);
            if (verificationResult.AadhaarNoMatch.HasValue) matches.Add(verificationResult.AadhaarNoMatch.Value);
            if (verificationResult.DOBMatch.HasValue) matches.Add(verificationResult.DOBMatch.Value);
            if (verificationResult.PANNameMatch.HasValue) matches.Add(verificationResult.PANNameMatch.Value);
            if (verificationResult.PANNoMatch.HasValue) matches.Add(verificationResult.PANNoMatch.Value);
            if (verificationResult.SurveyNoMatch.HasValue) matches.Add(verificationResult.SurveyNoMatch.Value);
            if (verificationResult.VillageMatch.HasValue) matches.Add(verificationResult.VillageMatch.Value);
            if (verificationResult.DistrictMatch.HasValue) matches.Add(verificationResult.DistrictMatch.Value);
            if (verificationResult.TalukMatch.HasValue) matches.Add(verificationResult.TalukMatch.Value);
            if (verificationResult.HobliMatch.HasValue) matches.Add
               (verificationResult.HobliMatch.Value);
            if (verificationResult.MeasuringAreaMatch.HasValue) matches.Add
               (verificationResult.MeasuringAreaMatch.Value);
            if (verificationResult.ApplicationNumberMatch.HasValue) matches.Add
               (verificationResult.ApplicationNumberMatch.Value);
            if (verificationResult.ApplicantNameMatch.HasValue) matches.Add
               (verificationResult.ApplicantNameMatch.Value);


            var matchCount = matches.Count(m => m);
            var totalChecks = matches.Count;
            var matchPercentage = totalChecks > 0 ? (decimal)matchCount / totalChecks * 100 : 0;

            verificationResult.OverallMatch = matchPercentage >= 70;
            verificationResult.RiskScore = 100 - matchPercentage;
            verificationResult.Status = verificationResult.OverallMatch == true ? "Verified" : "Rejected";

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Verification results saved to database for uploadId: {uploadId}");

            return verificationResult;
        }
    }
}