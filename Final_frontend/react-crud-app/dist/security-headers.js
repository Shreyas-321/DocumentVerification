// Security Headers Configuration
// This file helps configure security headers for the application

const SECURITY_CONFIG = {
  // Content Security Policy
  csp: {
    "default-src": ["'self'"],
    "script-src": ["'self'", "'unsafe-inline'", "'unsafe-eval'"],
    "style-src": ["'self'", "'unsafe-inline'", "https://cdn.jsdelivr.net"],
    "img-src": ["'self'", "data:", "https:"],
    "font-src": ["'self'", "https://fonts.gstatic.com"],
    "connect-src": ["'self'", "https://localhost:50538", "https://localhost:5194"],
    "frame-src": ["'none'"],
    "object-src": ["'none'"],
    "base-uri": ["'self'"],
    "form-action": ["'self'"]
  },
  
  // Security Headers
  headers: {
    "X-Content-Type-Options": "nosniff",
    "X-Frame-Options": "DENY",
    "X-XSS-Protection": "1; mode=block",
    "Referrer-Policy": "strict-origin-when-cross-origin",
    "Permissions-Policy": "camera=(), microphone=(), geolocation=()"
  }
};

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
  module.exports = SECURITY_CONFIG;
} else if (typeof window !== 'undefined') {
  window.SECURITY_CONFIG = SECURITY_CONFIG;
}
