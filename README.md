# Document Verification System

A comprehensive document verification system with React frontend, ASP.NET Core backend, and DLL integration for EC, Aadhaar, and PAN document processing.

## 🎯 Features

### User Dashboard
- Document upload (EC, Aadhaar, PAN)
- Real-time verification status
- Risk score calculation
- Upload history tracking
- Extracted data display
- Profile management
- Password change functionality

### Admin Dashboard
- User management with search
- Document monitoring with filtering
- Analytics and reports
- Activity logs and audit trails
- Verification oversight
- Risk score analysis
- Document extraction and verification

### Document Processing
- PDF extraction using DLL
- Field-by-field comparison
- Risk score calculation
- Verification notes
- Mismatch detection
- Cross-document verification

## 🛠️ Technology Stack

### Frontend
- **React 18.2.0** - UI framework
- **Bootstrap 5.3.0** - Styling and components
- **Axios 1.4.0** - HTTP client
- **React Router 6.11.0** - Navigation
- **Vite** - Build tool

### Backend
- **ASP.NET Core 8.0** - Web API
- **Entity Framework Core** - Database ORM
- **SQL Server** - Database
- **JWT Authentication** - Security
- **BCrypt.Net** - Password hashing

### Document Processing
- **Custom DLL** - PDF extraction
- **Risk Calculation** - Scoring algorithm
- **File Management** - Document storage
- **Tesseract OCR** - Text extraction

## 📁 Project Structure

```
DocumentVerificationSystem/
├── webApitest/                 # ASP.NET Core Backend
│   ├── Controllers/            # API Controllers
│   │   ├── AuthController.cs
│   │   ├── AdminDashboardController.cs
│   │   ├── UserDashboardController.cs
│   │   └── AuditLogsController.cs
│   ├── Models/                 # Data Models
│   │   ├── User.cs
│   │   ├── UserActivityLog.cs
│   │   ├── UserUploadedDocuments.cs
│   │   └── VerificationResults.cs
│   ├── Services/               # Business Logic
│   │   ├── IUserService.cs
│   │   ├── UserService.cs
│   │   ├── IJwtService.cs
│   │   ├── JwtService.cs
│   │   ├── IAuditService.cs
│   │   └── AuditService.cs
│   ├── Data/                   # Database Context
│   │   └── ApplicationDbContext.cs
│   └── DTOs/                   # Data Transfer Objects
│       ├── UserLoginDto.cs
│       ├── UserRegisterDto.cs
│       └── LoginResponseDto.cs
├── react-contacts-app/         # React Frontend
│   ├── src/
│   │   ├── Components/        # React Components
│   │   │   ├── Auth/          # Authentication components
│   │   │   ├── Dashboard/     # Dashboard components
│   │   │   ├── Layout/        # Layout components
│   │   │   └── Common/        # Shared components
│   │   ├── Services/           # API Services
│   │   │   ├── AuthService.jsx
│   │   │   ├── UserService.jsx
│   │   │   ├── DocumentService.jsx
│   │   │   └── AuditLogsService.jsx
│   │   ├── Routes/             # Navigation
│   │   │   └── AppRoutes.jsx
│   │   └── config/             # Configuration
│   │       └── api.jsx
│   └── public/                 # Static Files
└── DocumentVerificationDLL/    # Document Processing DLL
    └── DocumentVerificationDLL/
        ├── IDocumentVerificationDLL.cs
        └── DocumentVerificationDLL.cs
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- SQL Server
- Visual Studio Code or Visual Studio

### Backend Setup
```bash
cd webApitest
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend Setup
```bash
cd react-contacts-app
npm install
npm start
```

### Database Setup
1. Update connection string in `appsettings.json`
2. Run Entity Framework migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. Seed initial admin data

## 📊 API Endpoints

### Authentication APIs
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/admin-login` - Admin login
- `GET /api/auth/verify` - Token verification

### User Dashboard APIs
- `GET /api/UserDashboard/profile` - Get user profile
- `PUT /api/UserDashboard/profile` - Update profile
- `PUT /api/UserDashboard/change-password` - Change password
- `POST /api/UserDashboard/upload-documents` - Upload documents
- `GET /api/UserDashboard/document-status` - Get document status
- `GET /api/UserDashboard/upload-history` - Get upload history

### Admin Dashboard APIs
- `GET /api/AdminDashboard/users` - Get all users
- `GET /api/AdminDashboard/uploaded-documents` - Get all documents
- `GET /api/AdminDashboard/analytics` - Get analytics
- `POST /api/AdminDashboard/extract/{id}` - Extract document data
- `POST /api/AdminDashboard/verify/{id}` - Verify documents
- `GET /api/AdminDashboard/extracted-data/{id}` - Get extracted data
- `GET /api/AdminDashboard/users/{id}/documents` - Get user documents

### Audit Logs APIs
- `GET /api/AuditLogs` - Get all audit logs (Admin only)
- `GET /api/AuditLogs/user/{userId}` - Get user audit logs
- `GET /api/AuditLogs/activity/{activity}` - Get activity logs
- `GET /api/AuditLogs/entity/{entityType}/{entityId}` - Get entity logs
- `GET /api/AuditLogs/date-range` - Get logs by date range
- `GET /api/AuditLogs/my-activity` - Get current user's activity

## 🔧 Configuration

### Backend Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DocumentVerification;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "DocumentVerification",
    "Audience": "DocumentVerification",
    "ExpiryMinutes": 60
  }
}
```

### Frontend Configuration
```javascript
const API_BASE_URL = 'https://localhost:7001/api';
```

## 📱 Key Features

### User Dashboard
- **Document Upload**: Upload EC, Aadhaar, and PAN documents
- **Verification Status**: Real-time status updates
- **Risk Score**: Visual risk assessment
- **Upload History**: Track all document submissions
- **Profile Management**: Update personal information
- **Password Security**: Secure password change functionality

### Admin Dashboard
- **User Management**: View, edit, and manage users
- **Document Monitoring**: Track all document submissions
- **Analytics**: Comprehensive system statistics
- **Activity Logs**: Detailed audit trails
- **Verification Control**: Extract and verify documents
- **Risk Analysis**: Monitor risk scores and patterns

### Document Processing
- **PDF Extraction**: Extract text from PDF documents
- **Field Comparison**: Compare data across documents
- **Risk Calculation**: Calculate risk scores based on mismatches
- **Verification Notes**: Add admin notes and comments
- **Mismatch Detection**: Identify discrepancies between documents

## 🔒 Security Features

- **JWT Authentication**: Secure token-based authentication
- **Role-based Authorization**: Admin and user role separation
- **File Upload Validation**: Secure document upload
- **CORS Configuration**: Cross-origin request security
- **Input Sanitization**: Prevent injection attacks
- **Password Hashing**: BCrypt password encryption
- **Audit Logging**: Track all user activities

## 📈 Performance Features

- **Async/Await Patterns**: Non-blocking operations
- **Database Optimization**: Efficient queries and indexing
- **File Caching**: Optimized document storage
- **Response Compression**: Reduced bandwidth usage
- **Lazy Loading**: On-demand component loading
- **Error Handling**: Comprehensive error management

## 🎨 UI/UX Features

- **Responsive Design**: Mobile-friendly interface
- **Bootstrap Styling**: Professional UI components
- **Loading States**: Visual feedback during operations
- **Error Messages**: Clear, user-friendly error handling
- **Search Functionality**: Real-time search and filtering
- **Interactive Elements**: Hover effects and animations

## 🧪 Testing

### Backend Testing
```bash
cd webApitest
dotnet test
```

### Frontend Testing
```bash
cd react-contacts-app
npm test
```

## 📦 Deployment

### Backend Deployment
1. Build the project: `dotnet build --configuration Release`
2. Deploy to IIS or Azure App Service
3. Configure database connection
4. Set up SSL certificates

### Frontend Deployment
1. Build the project: `npm run build`
2. Deploy to static hosting (Netlify, Vercel, etc.)
3. Configure environment variables
4. Set up CDN for assets

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/new-feature`
3. Commit your changes: `git commit -am 'Add new feature'`
4. Push to the branch: `git push origin feature/new-feature`
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Siddling Reddy** - Initial work and development

## 🙏 Acknowledgments

- Document processing libraries and OCR technology
- React community for excellent documentation
- ASP.NET Core team for robust framework
- Bootstrap team for beautiful UI components
- Entity Framework team for database abstraction

## 📞 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation for common solutions

## 🔄 Version History

- **v1.0.0** - Initial release with basic functionality
- **v1.1.0** - Added audit logging and analytics
- **v1.2.0** - Enhanced UI/UX and error handling
- **v1.3.0** - Added advanced document processing features

---

**Built with ❤️ for secure document verification**
