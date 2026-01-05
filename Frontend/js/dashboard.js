// ===== DASHBOARD PAGE =====

document.addEventListener('DOMContentLoaded', function () {
    // Protect page
    if (!protectPage()) return;

    // Initialize user info
    initUserInfo();

    // Load dashboard data
    loadDashboardStats();

    // Set active nav item
    setActiveNavItem();
});

// Load dashboard statistics
async function loadDashboardStats() {
    try {
        // Get all employees
        const response = await getEmployeesAPI();
        
        // Handle ServiceResult wrapper (response.data contains the actual array)
        const employees = response.data || response || [];

        // Update stats
        updateStat('totalEmployees', employees.length);

        // Count active employees
        const activeCount = employees.filter(emp => emp.isActive).length;
        updateStat('activeEmployees', activeCount);

        // Get pending leave requests (if user is Admin or HR)
        const user = getUserInfo();
        if (user && (user.role === 'Admin' || user.role === 'HR')) {
            try {
                const leavesResponse = await getPendingLeavesAPI();
                const pendingLeaves = leavesResponse.data || leavesResponse || [];
                updateStat('pendingLeaves', pendingLeaves.length);
            } catch (error) {
                console.error('Error loading pending leaves:', error);
                updateStat('pendingLeaves', 0);
            }
        }

        // Get today's attendance count
        // Note: This would need a specific API endpoint
        // For now, we'll show a placeholder
        updateStat('todayAttendance', 0);

    } catch (error) {
        console.error('Error loading dashboard stats:', error);
        showError('Không thể tải dữ liệu dashboard');
    }
}

// Update stat card
function updateStat(statId, value) {
    const element = document.getElementById(statId);
    if (element) {
        // Animate number
        animateNumber(element, 0, value, 1000);
    }
}

// Animate number counting up
function animateNumber(element, start, end, duration) {
    const range = end - start;
    const increment = range / (duration / 16); // 60fps
    let current = start;

    const timer = setInterval(() => {
        current += increment;
        if (current >= end) {
            current = end;
            clearInterval(timer);
        }
        element.textContent = Math.floor(current);
    }, 16);
}

// Set active navigation item
function setActiveNavItem() {
    const currentPage = window.location.pathname.split('/').pop();
    const navItems = document.querySelectorAll('.nav-item');

    navItems.forEach(item => {
        const href = item.getAttribute('href');
        if (href === currentPage) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
}

// Search functionality
const searchInput = document.getElementById('searchInput');
if (searchInput) {
    searchInput.addEventListener('input', function (e) {
        const searchTerm = e.target.value.toLowerCase();
        // Implement search logic based on current page
        console.log('Searching for:', searchTerm);
    });
}
