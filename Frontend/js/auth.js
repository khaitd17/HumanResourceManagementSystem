// ===== AUTH UTILITIES =====

const AUTH_KEY = 'hrms_auth_token';
const USER_KEY = 'hrms_user_info';

// Get token from localStorage
function getToken() {
    return localStorage.getItem(AUTH_KEY);
}

// Set token to localStorage
function setToken(token) {
    localStorage.setItem(AUTH_KEY, token);
}

// Remove token from localStorage
function removeToken() {
    localStorage.removeItem(AUTH_KEY);
    localStorage.removeItem(USER_KEY);
}

// Get user info from localStorage
function getUserInfo() {
    const userJson = localStorage.getItem(USER_KEY);
    return userJson ? JSON.parse(userJson) : null;
}

// Set user info to localStorage
function setUserInfo(user) {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
}

// Check if user is authenticated
function isAuthenticated() {
    return !!getToken();
}

// Decode JWT token (simple base64 decode)
function decodeToken(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
}

// Check if token is expired
function isTokenExpired(token) {
    const decoded = decodeToken(token);
    if (!decoded || !decoded.exp) return true;
    
    const currentTime = Date.now() / 1000;
    return decoded.exp < currentTime;
}

// Logout function
function logout() {
    removeToken();
    window.location.href = '../index.html';
}

// Protect page (redirect to login if not authenticated)
function protectPage() {
    if (!isAuthenticated()) {
        window.location.href = '../index.html';
        return false;
    }
    
    const token = getToken();
    if (isTokenExpired(token)) {
        alert('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
        logout();
        return false;
    }
    
    return true;
}

// Initialize user info on dashboard pages
function initUserInfo() {
    const user = getUserInfo();
    if (user) {
        const userNameEl = document.getElementById('userName');
        const userRoleEl = document.getElementById('userRole');
        
        if (userNameEl) userNameEl.textContent = user.fullName || user.username;
        if (userRoleEl) {
            const roleMap = {
                'Admin': 'Quản trị viên',
                'HR': 'Nhân sự',
                'Staff': 'Nhân viên'
            };
            userRoleEl.textContent = roleMap[user.role] || user.role;
        }
    }
}

// Toggle sidebar
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    if (sidebar) {
        sidebar.classList.toggle('active');
    }
}

// Toggle password visibility
function togglePassword() {
    const passwordInput = document.getElementById('password');
    const toggleIcon = document.getElementById('toggleIcon');
    
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.remove('fa-eye');
        toggleIcon.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.remove('fa-eye-slash');
        toggleIcon.classList.add('fa-eye');
    }
}

// Fill demo credentials
function fillDemo(username, password) {
    document.getElementById('username').value = username;
    document.getElementById('password').value = password;
}

// Format currency (VND)
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

// Format date
function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

// Format datetime
function formatDateTime(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleString('vi-VN');
}

// Show loading
function showLoading(element) {
    if (element) {
        element.innerHTML = '<div class="loading"><i class="fas fa-spinner fa-spin"></i> Đang tải...</div>';
    }
}

// Show error message
function showError(message, elementId = 'errorMessage') {
    const errorEl = document.getElementById(elementId);
    if (errorEl) {
        errorEl.textContent = message;
        errorEl.style.display = 'block';
        setTimeout(() => {
            errorEl.style.display = 'none';
        }, 5000);
    }
}

// Show success message (you can create a success message element)
function showSuccess(message) {
    // Simple alert for now, can be improved with custom toast
    alert(message);
}

// Confirm action
function confirmAction(message) {
    return confirm(message);
}
