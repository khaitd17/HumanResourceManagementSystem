# HRMS Frontend

## 📋 Tổng quan

Frontend cho Hệ thống Quản lý Nhân sự (HRMS) được xây dựng với **HTML, CSS, JavaScript thuần**.

## ✅ Đã hoàn thành

### 🔐 **Authentication**
- ✅ Trang đăng nhập với JWT
- ✅ Demo accounts (Admin, HR, Staff)
- ✅ Token management
- ✅ Auto redirect based on role
- ✅ Session protection

### 👨‍💼 **Admin Dashboard**
- ✅ Sidebar navigation
- ✅ Statistics cards (Tổng nhân viên, Đang làm việc, Đơn nghỉ chờ duyệt, Chấm công hôm nay)
- ✅ Quick actions
- ✅ Recent activities
- ✅ Responsive design

### 👥 **Employee Management**
- ✅ Danh sách nhân viên (table view)
- ✅ Thêm nhân viên mới
- ✅ Sửa thông tin nhân viên
- ✅ Xóa nhân viên
- ✅ Tìm kiếm nhân viên
- ✅ Lọc theo phòng ban
- ✅ Lọc theo trạng thái (Đang làm việc / Đã nghỉ)
- ✅ Modal form với validation

## 🎨 Features

### **Modern UI/UX**
- Gradient colors
- Smooth animations
- Hover effects
- Responsive layout
- Clean and professional design

### **Security**
- JWT token authentication
- Token expiration check
- Auto logout on token expire
- Protected routes

### **User Experience**
- Loading states
- Error messages
- Success notifications
- Confirm dialogs
- Demo account quick fill

## 📂 Cấu trúc

```
Frontend/
├── index.html              # Login page
├── pages/
│   ├── dashboard.html      # Admin dashboard
│   └── employees.html      # Employee management
├── css/
│   └── style.css          # All styles
└── js/
    ├── auth.js            # Authentication utilities
    ├── api.js             # API client
    ├── login.js           # Login page logic
    ├── dashboard.js       # Dashboard logic
    └── employees.js       # Employee management logic
```

## 🚀 Cách chạy

### **Bước 1: Chạy Backend API**
```bash
cd Backend/HRM.API
dotnet run
```
API sẽ chạy tại: `http://localhost:5286`

### **Bước 2: Mở Frontend**

**Option 1: Live Server (VS Code)**
1. Cài extension "Live Server"
2. Right-click vào `index.html`
3. Chọn "Open with Live Server"

**Option 2: Python HTTP Server**
```bash
cd Frontend
python -m http.server 8000
```
Mở: `http://localhost:8000`

**Option 3: Node.js HTTP Server**
```bash
cd Frontend
npx http-server -p 8000
```
Mở: `http://localhost:8000`

## 🔑 Test Accounts

Tất cả accounts đều dùng password: **Password123!**

| Username | Role | Mô tả |
|----------|------|-------|
| admin | Admin | Quản trị viên - Full quyền |
| hr_user | HR | Nhân sự - Quản lý nhân viên, duyệt nghỉ phép |
| cuong.lv | Staff | Nhân viên - Xem thông tin cá nhân |

## 🎯 Tính năng theo Role

### **Admin**
- ✅ Xem dashboard
- ✅ Quản lý nhân viên (CRUD)
- ✅ Quản lý chấm công
- ✅ Duyệt nghỉ phép
- ✅ Tính lương
- ✅ Xem báo cáo
- ✅ Cài đặt hệ thống

### **HR**
- ✅ Xem dashboard
- ✅ Quản lý nhân viên (CRUD)
- ✅ Quản lý chấm công
- ✅ Duyệt nghỉ phép
- ✅ Tính lương
- ✅ Xem báo cáo

### **Staff**
- ✅ Xem dashboard cá nhân
- ✅ Xem thông tin cá nhân
- ✅ Chấm công
- ✅ Xin nghỉ phép
- ✅ Xem lương
- ✅ Chat nội bộ

## 📝 API Integration

Frontend kết nối với Backend API tại: `http://localhost:5286/api`

### **Endpoints đã tích hợp:**
- ✅ POST `/auth/login` - Đăng nhập
- ✅ GET `/auth/me` - Lấy thông tin user
- ✅ GET `/employee` - Lấy danh sách nhân viên
- ✅ GET `/employee/{id}` - Lấy thông tin nhân viên
- ✅ POST `/employee` - Thêm nhân viên
- ✅ PUT `/employee/{id}` - Cập nhật nhân viên
- ✅ DELETE `/employee/{id}` - Xóa nhân viên

## 🔧 Configuration

File `js/api.js` chứa cấu hình API:

```javascript
const API_BASE_URL = 'http://localhost:5286/api';
```

Nếu Backend chạy ở port khác, thay đổi URL này.

## 🎨 Customization

### **Colors**
Thay đổi colors trong `css/style.css`:

```css
:root {
    --primary: #4f46e5;
    --success: #10b981;
    --warning: #f59e0b;
    --danger: #ef4444;
    /* ... */
}
```

### **Logo**
Thay đổi icon trong `.logo`:

```html
<i class="fas fa-users-cog"></i>
```

## 🚧 Cần làm tiếp

### **Pages chưa có:**
- [ ] Attendance Management
- [ ] Leave Request Management
- [ ] Payroll Management
- [ ] Chat Module
- [ ] Reports
- [ ] Settings
- [ ] User Profile

### **Features cần thêm:**
- [ ] Pagination cho table
- [ ] Export to Excel
- [ ] Import from Excel
- [ ] Advanced search
- [ ] Notifications
- [ ] Dark mode
- [ ] Multi-language

## 📱 Responsive

Frontend đã responsive cho:
- ✅ Desktop (> 1024px)
- ✅ Tablet (768px - 1024px)
- ✅ Mobile (< 768px)

## 🐛 Known Issues

- Pagination chưa hoạt động (cần implement)
- Export Excel chưa có (cần implement)
- Notifications chưa real-time (cần SignalR)

## 📞 Support

Nếu có vấn đề, kiểm tra:
1. Backend API đang chạy
2. CORS đã được config đúng
3. Console log để xem errors
4. Network tab để xem API calls

---

**Created:** 2026-01-02
**Status:** Login + Admin Dashboard + Employee Management Complete
**Next:** Attendance, Leave Request, Payroll modules
