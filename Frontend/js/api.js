// ===== API CONFIGURATION =====

const API_BASE_URL = 'http://localhost:5286/api';

// API endpoints
const API_ENDPOINTS = {
    // Auth
    login: '/auth/login',
    register: '/auth/register',
    me: '/auth/me',
    changePassword: '/auth/change-password',

    // Employee
    employees: '/employee',
    employeeById: (id) => `/employee/${id}`,
    employeeByCode: (code) => `/employee/code/${code}`,
    activeEmployees: '/employee/active',

    // Attendance
    attendance: '/attendance',
    checkIn: '/attendance/check-in',
    checkOut: '/attendance/check-out',
    myTodayAttendance: '/attendance/my-today',

    // Leave Request
    leaveRequests: '/leaverequest',
    pendingLeaves: '/leaverequest/pending',
    myLeaveRequests: '/leaverequest/my-requests',
    approveLeave: '/leaverequest/approve',

    // Payroll
    payrolls: '/payroll',
    generatePayroll: '/payroll/generate',

    // Chat
    chatRooms: '/chat/rooms',
    chatMessages: (roomId) => `/chat/rooms/${roomId}/messages`,
    sendMessage: '/chat/messages'
};

// ===== API HELPER FUNCTIONS =====

// Make API request
async function apiRequest(endpoint, options = {}) {
    const token = getToken();

    const defaultHeaders = {
        'Content-Type': 'application/json'
    };

    if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        ...options,
        headers: {
            ...defaultHeaders,
            ...options.headers
        }
    };

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);

        // Handle 401 Unauthorized
        if (response.status === 401) {
            removeToken();
            window.location.href = '../index.html';
            throw new Error('Phiên đăng nhập đã hết hạn');
        }

        // Parse response
        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || `HTTP error! status: ${response.status}`);
        }

        return data;
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// GET request
async function apiGet(endpoint) {
    return apiRequest(endpoint, {
        method: 'GET'
    });
}

// POST request
async function apiPost(endpoint, data) {
    return apiRequest(endpoint, {
        method: 'POST',
        body: JSON.stringify(data)
    });
}

// PUT request
async function apiPut(endpoint, data) {
    return apiRequest(endpoint, {
        method: 'PUT',
        body: JSON.stringify(data)
    });
}

// DELETE request
async function apiDelete(endpoint) {
    return apiRequest(endpoint, {
        method: 'DELETE'
    });
}

// ===== AUTH API =====

async function loginAPI(username, password) {
    return apiPost(API_ENDPOINTS.login, { username, password });
}

async function getMeAPI() {
    return apiGet(API_ENDPOINTS.me);
}

// ===== EMPLOYEE API =====

async function getEmployeesAPI() {
    return apiGet(API_ENDPOINTS.employees);
}

async function getActiveEmployeesAPI() {
    return apiGet(API_ENDPOINTS.activeEmployees);
}

async function getEmployeeByIdAPI(id) {
    return apiGet(API_ENDPOINTS.employeeById(id));
}

async function createEmployeeAPI(employeeData) {
    return apiPost(API_ENDPOINTS.employees, employeeData);
}

async function updateEmployeeAPI(id, employeeData) {
    return apiPut(API_ENDPOINTS.employeeById(id), employeeData);
}

async function deleteEmployeeAPI(id) {
    return apiDelete(API_ENDPOINTS.employeeById(id));
}

// ===== ATTENDANCE API =====

async function checkInAPI(data) {
    return apiPost(API_ENDPOINTS.checkIn, data);
}

async function checkOutAPI(data) {
    return apiPost(API_ENDPOINTS.checkOut, data);
}

async function getMyTodayAttendanceAPI() {
    return apiGet(API_ENDPOINTS.myTodayAttendance);
}

// ===== LEAVE REQUEST API =====

async function getLeaveRequestsAPI() {
    return apiGet(API_ENDPOINTS.leaveRequests);
}

async function getPendingLeavesAPI() {
    return apiGet(API_ENDPOINTS.pendingLeaves);
}

async function getMyLeaveRequestsAPI() {
    return apiGet(API_ENDPOINTS.myLeaveRequests);
}

async function createLeaveRequestAPI(data) {
    return apiPost(API_ENDPOINTS.leaveRequests, data);
}

async function approveLeaveRequestAPI(data) {
    return apiPost(API_ENDPOINTS.approveLeave, data);
}

// ===== PAYROLL API =====

async function getPayrollsAPI() {
    return apiGet(API_ENDPOINTS.payrolls);
}

async function generatePayrollAPI(data) {
    return apiPost(API_ENDPOINTS.generatePayroll, data);
}

// ===== CHAT API =====

async function getChatRoomsAPI(employeeId) {
    return apiGet(`${API_ENDPOINTS.chatRooms}/employee/${employeeId}`);
}

async function getChatMessagesAPI(roomId) {
    return apiGet(API_ENDPOINTS.chatMessages(roomId));
}

async function sendMessageAPI(data) {
    return apiPost(API_ENDPOINTS.sendMessage, data);
}
