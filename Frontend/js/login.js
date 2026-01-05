// ===== LOGIN PAGE =====

document.addEventListener('DOMContentLoaded', function () {
    // Check if already logged in
    if (isAuthenticated()) {
        const user = getUserInfo();
        if (user) {
            redirectToDashboard(user.role);
        }
    }

    // Handle login form submission
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }
});

// Handle login
async function handleLogin(e) {
    e.preventDefault();

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value;
    const rememberMe = document.getElementById('rememberMe').checked;

    // Validate inputs
    if (!username || !password) {
        showError('Vui lòng nhập đầy đủ thông tin');
        return;
    }

    // Show loading state
    const submitBtn = e.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang đăng nhập...';
    submitBtn.disabled = true;

    try {
        // Call login API
        const response = await loginAPI(username, password);

        // Check if response has data property (ServiceResult wrapper)
        const loginData = response.data || response;

        if (loginData && loginData.token) {
            // Save token
            setToken(loginData.token);

            // Decode token to get user info
            const decoded = decodeToken(loginData.token);

            // Get full user info from API
            try {
                const userInfo = await getMeAPI();
                setUserInfo(userInfo);

                // Redirect based on role
                redirectToDashboard(userInfo.role);
            } catch (error) {
                // If can't get user info, use decoded token
                const userFromToken = {
                    username: decoded.unique_name || username,
                    role: decoded.role || 'Staff',
                    fullName: decoded.unique_name || username
                };
                setUserInfo(userFromToken);
                redirectToDashboard(userFromToken.role);
            }
        } else {
            showError('Đăng nhập thất bại. Vui lòng thử lại.');
        }
    } catch (error) {
        console.error('Login error:', error);
        showError(error.message || 'Tên đăng nhập hoặc mật khẩu không đúng');
    } finally {
        // Restore button state
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    }
}

// Redirect to dashboard based on role
function redirectToDashboard(role) {
    switch (role) {
        case 'Admin':
            window.location.href = 'pages/dashboard.html';
            break;
        case 'HR':
            window.location.href = 'pages/dashboard.html';
            break;
        case 'Staff':
            window.location.href = 'pages/dashboard.html';
            break;
        default:
            window.location.href = 'pages/dashboard.html';
    }
}
