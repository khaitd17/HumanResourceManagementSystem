// ===== EMPLOYEES PAGE =====

let allEmployees = [];
let departments = [];
let currentEmployee = null;

document.addEventListener('DOMContentLoaded', function () {
    // Protect page
    if (!protectPage()) return;

    // Initialize user info
    initUserInfo();

    // Load data
    loadEmployees();
    loadDepartments();

    // Set active nav item
    setActiveNavItem();

    // Setup event listeners
    setupEventListeners();
});

// Setup event listeners
function setupEventListeners() {
    // Search
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', filterEmployees);
    }

    // Filters
    const filterDepartment = document.getElementById('filterDepartment');
    const filterStatus = document.getElementById('filterStatus');

    if (filterDepartment) {
        filterDepartment.addEventListener('change', filterEmployees);
    }

    if (filterStatus) {
        filterStatus.addEventListener('change', filterEmployees);
    }

    // Employee form
    const employeeForm = document.getElementById('employeeForm');
    if (employeeForm) {
        employeeForm.addEventListener('submit', handleEmployeeSubmit);
    }
}

// Load employees
async function loadEmployees() {
    const tbody = document.getElementById('employeeTableBody');
    showLoading(tbody);

    try {
        allEmployees = await getEmployeesAPI();
        renderEmployees(allEmployees);
    } catch (error) {
        console.error('Error loading employees:', error);
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center" style="color: var(--danger);">
                    <i class="fas fa-exclamation-triangle"></i>
                    Không thể tải danh sách nhân viên
                </td>
            </tr>
        `;
    }
}

// Load departments
async function loadDepartments() {
    try {
        // Note: You might need to create a departments endpoint
        // For now, we'll extract unique departments from employees
        const uniqueDepts = [...new Set(allEmployees.map(emp => emp.departmentId))];

        // Populate department filter
        const filterDepartment = document.getElementById('filterDepartment');
        const formDepartment = document.getElementById('departmentId');

        // For demo, add some default departments
        const defaultDepts = [
            { id: 1, name: 'Phòng IT' },
            { id: 2, name: 'Phòng Nhân sự' },
            { id: 3, name: 'Phòng Kế toán' },
            { id: 4, name: 'Phòng Marketing' },
            { id: 5, name: 'Phòng Kinh doanh' }
        ];

        departments = defaultDepts;

        if (filterDepartment) {
            defaultDepts.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.id;
                option.textContent = dept.name;
                filterDepartment.appendChild(option);
            });
        }

        if (formDepartment) {
            defaultDepts.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.id;
                option.textContent = dept.name;
                formDepartment.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error loading departments:', error);
    }
}

// Render employees table
function renderEmployees(employees) {
    const tbody = document.getElementById('employeeTableBody');

    if (employees.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center">
                    <i class="fas fa-inbox"></i>
                    Không có nhân viên nào
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = employees.map(emp => {
        const deptName = getDepartmentName(emp.departmentId);
        const statusBadge = emp.isActive
            ? '<span class="status-badge active">Đang làm việc</span>'
            : '<span class="status-badge inactive">Đã nghỉ</span>';

        return `
            <tr>
                <td>${emp.employeeCode || ''}</td>
                <td>${emp.fullName || ''}</td>
                <td>${emp.email || ''}</td>
                <td>${deptName}</td>
                <td>${emp.position || ''}</td>
                <td>${formatCurrency(emp.baseSalary || 0)}</td>
                <td>${statusBadge}</td>
                <td>
                    <button class="action-btn edit" onclick="editEmployee(${emp.id})" title="Sửa">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="action-btn delete" onclick="deleteEmployee(${emp.id})" title="Xóa">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    }).join('');
}

// Get department name by ID
function getDepartmentName(deptId) {
    const dept = departments.find(d => d.id === deptId);
    return dept ? dept.name : '';
}

// Filter employees
function filterEmployees() {
    const searchTerm = document.getElementById('searchInput')?.value.toLowerCase() || '';
    const deptFilter = document.getElementById('filterDepartment')?.value || '';
    const statusFilter = document.getElementById('filterStatus')?.value || '';

    let filtered = allEmployees;

    // Search filter
    if (searchTerm) {
        filtered = filtered.filter(emp =>
            emp.fullName?.toLowerCase().includes(searchTerm) ||
            emp.email?.toLowerCase().includes(searchTerm) ||
            emp.employeeCode?.toLowerCase().includes(searchTerm)
        );
    }

    // Department filter
    if (deptFilter) {
        filtered = filtered.filter(emp => emp.departmentId === parseInt(deptFilter));
    }

    // Status filter
    if (statusFilter) {
        const isActive = statusFilter === 'true';
        filtered = filtered.filter(emp => emp.isActive === isActive);
    }

    renderEmployees(filtered);
}

// Open add employee modal
function openAddEmployeeModal() {
    currentEmployee = null;
    document.getElementById('modalTitle').textContent = 'Thêm nhân viên mới';
    document.getElementById('employeeForm').reset();
    document.getElementById('employeeId').value = '';
    document.getElementById('employeeModal').classList.add('active');
}

// Close employee modal
function closeEmployeeModal() {
    document.getElementById('employeeModal').classList.remove('active');
    currentEmployee = null;
}

// Edit employee
async function editEmployee(id) {
    try {
        const employee = await getEmployeeByIdAPI(id);
        currentEmployee = employee;

        // Fill form
        document.getElementById('modalTitle').textContent = 'Chỉnh sửa nhân viên';
        document.getElementById('employeeId').value = employee.id;
        document.getElementById('employeeCode').value = employee.employeeCode || '';
        document.getElementById('fullName').value = employee.fullName || '';
        document.getElementById('email').value = employee.email || '';
        document.getElementById('phone').value = employee.phone || '';
        document.getElementById('departmentId').value = employee.departmentId || '';
        document.getElementById('position').value = employee.position || '';
        document.getElementById('baseSalary').value = employee.baseSalary || 0;
        document.getElementById('dateOfBirth').value = employee.dateOfBirth ? employee.dateOfBirth.split('T')[0] : '';
        document.getElementById('joinDate').value = employee.joinDate ? employee.joinDate.split('T')[0] : '';
        document.getElementById('gender').value = employee.gender || '';
        document.getElementById('address').value = employee.address || '';
        document.getElementById('identityCard').value = employee.identityCard || '';
        document.getElementById('taxCode').value = employee.taxCode || '';
        document.getElementById('bankAccount').value = employee.bankAccount || '';
        document.getElementById('bankName').value = employee.bankName || '';

        // Open modal
        document.getElementById('employeeModal').classList.add('active');
    } catch (error) {
        console.error('Error loading employee:', error);
        alert('Không thể tải thông tin nhân viên');
    }
}

// Delete employee
async function deleteEmployee(id) {
    if (!confirmAction('Bạn có chắc chắn muốn xóa nhân viên này?')) {
        return;
    }

    try {
        await deleteEmployeeAPI(id);
        showSuccess('Xóa nhân viên thành công');
        loadEmployees();
    } catch (error) {
        console.error('Error deleting employee:', error);
        alert('Không thể xóa nhân viên: ' + error.message);
    }
}

// Handle employee form submit
async function handleEmployeeSubmit(e) {
    e.preventDefault();

    const employeeData = {
        employeeCode: document.getElementById('employeeCode').value,
        fullName: document.getElementById('fullName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value || null,
        departmentId: parseInt(document.getElementById('departmentId').value) || null,
        position: document.getElementById('position').value || null,
        baseSalary: parseFloat(document.getElementById('baseSalary').value) || 0,
        dateOfBirth: document.getElementById('dateOfBirth').value || null,
        joinDate: document.getElementById('joinDate').value || null,
        gender: document.getElementById('gender').value || null,
        address: document.getElementById('address').value || null,
        identityCard: document.getElementById('identityCard').value || null,
        taxCode: document.getElementById('taxCode').value || null,
        bankAccount: document.getElementById('bankAccount').value || null,
        bankName: document.getElementById('bankName').value || null,
        isActive: true
    };

    try {
        const employeeId = document.getElementById('employeeId').value;

        if (employeeId) {
            // Update existing employee
            await updateEmployeeAPI(employeeId, employeeData);
            showSuccess('Cập nhật nhân viên thành công');
        } else {
            // Create new employee
            await createEmployeeAPI(employeeData);
            showSuccess('Thêm nhân viên thành công');
        }

        closeEmployeeModal();
        loadEmployees();
    } catch (error) {
        console.error('Error saving employee:', error);
        alert('Không thể lưu nhân viên: ' + error.message);
    }
}

// Close modal when clicking outside
window.addEventListener('click', function (e) {
    const modal = document.getElementById('employeeModal');
    if (e.target === modal) {
        closeEmployeeModal();
    }
});
