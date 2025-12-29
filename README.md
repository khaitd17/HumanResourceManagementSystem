# Human Resource Management System (HRMS)

## 📋 Tổng quan dự án

Hệ thống quản lý nhân sự toàn diện được xây dựng với **ASP.NET Core 8.0** (Backend) và **Angular 18** (Frontend).

### Tính năng chính:
- ✅ **Quản lý nhân viên**: CRUD nhân viên, phòng ban
- ✅ **Chấm công hằng ngày**: Check-in/out với GPS/IP tracking, import Excel
- ✅ **Nghỉ phép**: Tạo đơn, duyệt/từ chối, thông báo real-time
- ✅ **Chat nội bộ**: 1-1 và group chat với SignalR, file attachments
- ✅ **Tính lương hàng tháng**: Tự động tính dựa trên chấm công, export PDF

### Phân quyền:
- **Admin**: Toàn quyền quản lý hệ thống
- **HR**: Quản lý nhân viên, duyệt nghỉ phép, tính lương
- **Staff**: Xem thông tin cá nhân, chấm công, xin nghỉ phép, chat

---

## 🏗️ Cấu trúc Backend

```
HRM.Backend/
├── HRM.DataLayer/              # Entity Models, DbContext
│   ├── Entities/               # Entity classes (User, Employee, Attendance, etc.)
│   └── Data/                   # HRMDbContext
│
├── HRM.RepositoryLayer/        # Repository Pattern
│   ├── Interfaces/             # Repository interfaces
│   └── Repositories/           # Repository implementations + UnitOfWork
│
├── HRM.ServiceLayer/           # Business Logic (TODO)
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Services/               # Business logic services
│   ├── Validators/             # FluentValidation
│   └── Mappings/               # AutoMapper profiles
│
└── HRM.API/                    # Web API (TODO)
    ├── Controllers/            # API Controllers
    ├── Hubs/                   # SignalR Hubs (Chat, Notifications)
    ├── Middleware/             # Custom middleware
    └── Program.cs              # Application configuration
```

---

## ✅ Đã hoàn thành

### 1. **DataLayer** ✅
- [x] User entity (Authentication)
- [x] Employee entity (Employee management)
- [x] Department entity
- [x] Attendance entity (Check-in/out, GPS tracking)
- [x] LeaveRequest entity (Leave management)
- [x] Payroll entity (Salary calculation)
- [x] PayrollConfig entity (Payroll settings)
- [x] ChatRoom, ChatRoomMember, Message entities (Chat system)
- [x] AuditLog entity (Audit trail)
- [x] HRMDbContext with relationships and seed data

### 2. **RepositoryLayer** ✅
- [x] Generic Repository pattern
- [x] All specific repositories (User, Employee, Attendance, etc.)
- [x] Unit of Work pattern
- [x] Transaction support

---

## 🚧 Cần làm tiếp

### 3. **ServiceLayer** (Tiếp theo)
- [ ] DTOs (Request/Response models)
- [ ] AutoMapper profiles
- [ ] FluentValidation validators
- [ ] Service interfaces
- [ ] Service implementations:
  - [ ] AuthService (Login, Register, JWT)
  - [ ] EmployeeService
  - [ ] AttendanceService (Excel import/export)
  - [ ] LeaveRequestService
  - [ ] PayrollService (Auto calculation, PDF export)
  - [ ] ChatService

### 4. **API Layer** (Sau ServiceLayer)
- [ ] JWT Authentication & Authorization
- [ ] API Controllers
- [ ] SignalR Hubs (Chat, Notifications)
- [ ] File upload/download
- [ ] CORS configuration
- [ ] Swagger documentation
- [ ] appsettings.json configuration

### 5. **Database Migration**
- [ ] Create initial migration
- [ ] Update database

### 6. **Frontend (Angular 18)**
- [ ] Setup Angular project
- [ ] Authentication module
- [ ] Admin dashboard
- [ ] HR dashboard
- [ ] Staff dashboard
- [ ] Employee management
- [ ] Attendance module
- [ ] Leave request module
- [ ] Payroll module
- [ ] Chat module
- [ ] Responsive design

### 7. **Docker**
- [ ] Dockerfile for Backend
- [ ] Dockerfile for Frontend
- [ ] docker-compose.yml

---

## 🔧 Database Configuration

**Connection String:**
```
Server=localhost;uid=khaitd;pwd=123456;database=HRM_DB;TrustServerCertificate=True
```

---

## 📦 Packages đã cài đặt

### HRM.DataLayer:
- Microsoft.EntityFrameworkCore 8.0.11
- Microsoft.EntityFrameworkCore.SqlServer 8.0.11
- Microsoft.EntityFrameworkCore.Design 8.0.11

### HRM.ServiceLayer:
- AutoMapper 16.0.0
- AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
- FluentValidation 11.11.0
- FluentValidation.DependencyInjectionExtensions 11.11.0
- EPPlus 7.5.2 (Excel import/export)

### HRM.API:
- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11
- Microsoft.IdentityModel.Tokens 8.2.1
- System.IdentityModel.Tokens.Jwt 8.2.1
- Microsoft.EntityFrameworkCore.Tools 8.0.11

---

## 🚀 Các bước tiếp theo

### Bước 1: Tạo ServiceLayer
Tôi sẽ tạo:
1. DTOs cho tất cả entities
2. AutoMapper profiles
3. FluentValidation validators
4. Service interfaces và implementations

### Bước 2: Tạo API Layer
1. Configure JWT Authentication
2. Tạo Controllers
3. Setup SignalR Hubs
4. Configure CORS và Swagger

### Bước 3: Database Migration
```bash
cd HRM.API
dotnet ef migrations add InitialCreate --project ../HRM.DataLayer
dotnet ef database update
```

### Bước 4: Tạo Frontend (Angular)
1. Setup Angular 18 project
2. Install Angular Material + Tailwind CSS
3. Tạo modules và components
4. Implement authentication
5. Integrate với Backend API

### Bước 5: Docker Deployment
1. Tạo Dockerfiles
2. Tạo docker-compose.yml
3. Test deployment

---

## 📝 Notes

- **Authentication**: JWT Token-based
- **Real-time**: SignalR cho chat và notifications
- **File handling**: Excel import/export, PDF payslip
- **GPS/IP Tracking**: Lưu location khi check-in/out
- **Audit Trail**: Tất cả actions được log vào AuditLog

---

## 👨‍💻 Tác giả

Được phát triển bởi Antigravity AI Assistant

---

**Bạn muốn tôi tiếp tục với phần nào tiếp theo?**
1. ServiceLayer (DTOs, Services, Validators)
2. API Layer (Controllers, JWT, SignalR)
3. Database Migration
4. Frontend (Angular)
