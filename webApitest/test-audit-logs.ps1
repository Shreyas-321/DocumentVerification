# PowerShell script to test audit logs system
# Make sure the API is running first: dotnet run

Write-Host "Testing Audit Logs System..." -ForegroundColor Green

# Step 1: Admin Login
Write-Host "`n1. Testing Admin Login..." -ForegroundColor Yellow
try {
    $loginResponse = Invoke-RestMethod -Uri "https://localhost:7000/api/auth/admin-login" -Method POST -ContentType "application/json" -Body '{"email":"admin@contactmanager.com","password":"admin123"}' -SkipCertificateCheck
    $token = $loginResponse.token
    Write-Host "Admin login successful! Token: $($token.Substring(0,20))..." -ForegroundColor Green
} catch {
    Write-Host "Admin login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Test Admin Dashboard Action (this will create audit logs)
Write-Host "`n2. Testing Admin Dashboard Action..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

try {
    $dashboardResponse = Invoke-RestMethod -Uri "https://localhost:7000/api/admindashboard/uploaded-documents" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "Admin dashboard action successful! Found $($dashboardResponse.Count) documents" -ForegroundColor Green
} catch {
    Write-Host "Dashboard action failed (this is normal if no documents exist): $($_.Exception.Message)" -ForegroundColor Yellow
}

# Step 3: Test Audit Logs Access
Write-Host "`n3. Testing Audit Logs Access..." -ForegroundColor Yellow
try {
    $auditLogsResponse = Invoke-RestMethod -Uri "https://localhost:7000/api/auditlogs" -Method GET -Headers $headers -SkipCertificateCheck
    Write-Host "Audit logs access successful!" -ForegroundColor Green
    Write-Host "Found $($auditLogsResponse.logs.Count) audit log entries" -ForegroundColor Cyan
    
    # Display first few audit logs
    if ($auditLogsResponse.logs.Count -gt 0) {
        Write-Host "`nSample Audit Logs:" -ForegroundColor Magenta
        $auditLogsResponse.logs | Select-Object -First 3 | ForEach-Object {
            Write-Host "  Activity: $($_.activity) by $($_.userName) at $($_.timestamp)" -ForegroundColor White
        }
    }
} catch {
    Write-Host "Audit logs access failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nAudit Logs Testing Complete!" -ForegroundColor Green
