// StayFinder Login System with API Integration
document.addEventListener('DOMContentLoaded', function() {
    console.log('Login system initialized');
    setupLoginForm();
    checkAuthenticationStatus();
});

// Setup login form event listener
function setupLoginForm() {
    const signinForm = document.querySelector('#signin-modal form.space-y-4');
    
    if (signinForm) {
        signinForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const emailInput = signinForm.querySelector('input[type="email"]');
            const passwordInput = signinForm.querySelector('input[type="password"]');
            
            const email = emailInput.value.trim();
            const password = passwordInput.value.trim();
            
            if (!email || !password) {
                showLoginError('Please fill in all fields');
                return;
            }
            
            showLoginLoading(true);
            await performLogin(email, password);
        });
    }
}

// Perform API login
async function performLogin(email, password) {
    try {
        const response = await fetch('/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        });

        const data = await response.json();

        if (response.ok && data.success) {
            // Store authentication data
            localStorage.setItem('authToken', data.data.token);
            localStorage.setItem('userEmail', data.data.email);
            localStorage.setItem('userRole', data.data.role);
            localStorage.setItem('userFirstName', data.data.firstName);
            localStorage.setItem('userLastName', data.data.lastName);
            localStorage.setItem('loginTime', new Date().toISOString());
            
            showLoginLoading(false);
            showLoginSuccess();
            
            // Close modal
            document.getElementById('signin-modal').close();
            
            // Redirect based on user role
            setTimeout(() => {
                redirectBasedOnRole(data.data.role);
            }, 1500);
            
        } else {
            showLoginLoading(false);
            showLoginError(data.message || 'Invalid credentials');
        }
    } catch (error) {
        console.error('Login error:', error);
        showLoginLoading(false);
        showLoginError('Network error. Please try again.');
    }
}

// Redirect user based on their role
function redirectBasedOnRole(role) {
    switch (role.toLowerCase()) {
        case 'customer':
            window.location.href = '/user-dashboard.html';
            break;
        case 'host':
            window.location.href = '/host-dashboard.html';
            break;
        case 'admin':
        case 'superadmin':
            window.location.href = '/super-admin.html';
            break;
        default:
            window.location.href = '/user-dashboard.html';
    }
}

// Check if user is already authenticated
function checkAuthenticationStatus() {
    const token = localStorage.getItem('authToken');
    const userRole = localStorage.getItem('userRole');
    
    if (token && userRole) {
        // Check if we're on the home page and user is logged in
        if (window.location.pathname === '/' || window.location.pathname === '/home.html' || window.location.pathname.endsWith('home.html')) {
            // Update UI to show user is logged in
            updateUIForLoggedInUser();
        }
    }
}

// Update UI when user is logged in
function updateUIForLoggedInUser() {
    const firstName = localStorage.getItem('userFirstName');
    const lastName = localStorage.getItem('userLastName');
    const role = localStorage.getItem('userRole');
    
    if (firstName) {
        // Find sign in button and replace with user info
        const signInBtn = document.querySelector('button:contains("Sign In")') || 
                         document.querySelector('label[for="signin-modal"]');
        
        if (signInBtn) {
            signInBtn.innerHTML = `
                <div class="dropdown dropdown-end">
                    <label tabindex="0" class="btn btn-ghost">
                        Hello, ${firstName}!
                        <svg class="w-4 h-4 ml-1" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clip-rule="evenodd"/>
                        </svg>
                    </label>
                    <ul tabindex="0" class="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-52">
                        <li><a href="/${role.toLowerCase()}-dashboard.html">Dashboard</a></li>
                        <li><a href="/profile.html">Profile</a></li>
                        <li><a onclick="logout()">Logout</a></li>
                    </ul>
                </div>
            `;
        }
    }
}

// Logout function
function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('userRole');
    localStorage.removeItem('userFirstName');
    localStorage.removeItem('userLastName');
    localStorage.removeItem('loginTime');
    
    // Show logout message
    showLogoutSuccess();
    
    // Redirect to home page
    setTimeout(() => {
        window.location.href = '/home.html';
    }, 1500);
}

// Show loading state
function showLoginLoading(isLoading) {
    const submitBtn = document.querySelector('#signin-modal form button[type="submit"]');
    if (submitBtn) {
        if (isLoading) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = `
                <span class="loading loading-spinner loading-xs"></span>
                Signing in...
            `;
        } else {
            submitBtn.disabled = false;
            submitBtn.innerHTML = 'Sign In';
        }
    }
}

// Show success message
function showLoginSuccess() {
    const toast = document.createElement('div');
    toast.className = 'fixed top-4 right-4 bg-green-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
    toast.innerHTML = `
        <div class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
            </svg>
            <span>Login Successful! Redirecting to your dashboard...</span>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 3000);
}

// Show error message
function showLoginError(message) {
    const toast = document.createElement('div');
    toast.className = 'fixed top-4 right-4 bg-red-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
    toast.innerHTML = `
        <div class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
            </svg>
            <span>${message}</span>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    // Add shake animation to modal
    const modal = document.querySelector('#signin-modal .modal-box');
    if (modal) {
        modal.style.animation = 'shake 0.5s ease-in-out';
        setTimeout(() => {
            modal.style.animation = '';
        }, 500);
    }
    
    // Clear password field for security
    const passwordInput = document.querySelector('#signin-modal input[type="password"]');
    if (passwordInput) {
        passwordInput.value = '';
        passwordInput.focus();
    }
    
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 5000);
}

// Show logout success
function showLogoutSuccess() {
    const toast = document.createElement('div');
    toast.className = 'fixed top-4 right-4 bg-blue-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
    toast.innerHTML = `
        <div class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M3 3a1 1 0 00-1 1v12a1 1 0 102 0V4a1 1 0 00-1-1zm10.293 9.293a1 1 0 001.414 1.414l3-3a1 1 0 000-1.414l-3-3a1 1 0 10-1.414 1.414L14.586 9H7a1 1 0 100 2h7.586l-1.293 1.293z" clip-rule="evenodd"/>
            </svg>
            <span>Logged out successfully</span>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 3000);
}

// Utility function to check if user is authenticated
function isAuthenticated() {
    return localStorage.getItem('authToken') !== null;
}

// Get user info
function getCurrentUser() {
    if (!isAuthenticated()) return null;
    
    return {
        email: localStorage.getItem('userEmail'),
        role: localStorage.getItem('userRole'),
        firstName: localStorage.getItem('userFirstName'),
        lastName: localStorage.getItem('userLastName'),
        token: localStorage.getItem('authToken')
    };
}

// Add CSS for shake animation
const style = document.createElement('style');
style.textContent = `
    @keyframes shake {
        0%, 100% { transform: translateX(0); }
        10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
        20%, 40%, 60%, 80% { transform: translateX(5px); }
    }
`;
document.head.appendChild(style);