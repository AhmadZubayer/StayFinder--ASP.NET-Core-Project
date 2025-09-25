// StayFinder Registration System
console.log('register.js file connected');

document.addEventListener('DOMContentLoaded', function() {
    setupRegistrationForm();
});

function setupRegistrationForm() {
    const registerForm = document.querySelector('form:not(#modal-signin-form)');
    
    if (registerForm) {
        registerForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const formData = new FormData(registerForm);
            const registrationData = {
                firstName: formData.get('firstName'),
                lastName: formData.get('lastName'),
                email: formData.get('email'),
                phone: formData.get('phone') || '',
                password: formData.get('password'),
                role: formData.get('role') || 'customer'
            };
            
            // Validate form data
            if (!validateRegistrationData(registrationData)) {
                return;
            }
            
            // Show loading state
            showRegistrationLoading(true);
            
            // Perform registration
            await performRegistration(registrationData);
        });
    }
}

function validateRegistrationData(data) {
    // Check required fields
    if (!data.firstName || !data.lastName || !data.email || !data.password) {
        showRegistrationError('Please fill in all required fields');
        return false;
    }
    
    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(data.email)) {
        showRegistrationError('Please enter a valid email address');
        return false;
    }
    
    // Validate password strength
    if (data.password.length < 6) {
        showRegistrationError('Password must be at least 6 characters long');
        return false;
    }
    
    // Check password confirmation
    const confirmPassword = document.querySelector('input[name="confirmPassword"]').value;
    if (data.password !== confirmPassword) {
        showRegistrationError('Passwords do not match');
        return false;
    }
    
    return true;
}

async function performRegistration(data) {
    try {
        const response = await fetch('/api/Auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (response.ok && result.success) {
            showRegistrationLoading(false);
            showRegistrationSuccess();
            
            // Redirect to login page or auto-login
            setTimeout(() => {
                // Auto login the user
                performAutoLogin(data.email, data.password);
            }, 2000);
            
        } else {
            showRegistrationLoading(false);
            showRegistrationError(result.message || 'Registration failed');
        }
    } catch (error) {
        console.error('Registration error:', error);
        showRegistrationLoading(false);
        showRegistrationError('Network error. Please try again.');
    }
}

async function performAutoLogin(email, password) {
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
            
            // Redirect based on user role
            redirectBasedOnRole(data.data.role);
        } else {
            // If auto-login fails, redirect to home
            window.location.href = '/home.html';
        }
    } catch (error) {
        console.error('Auto-login error:', error);
        window.location.href = '/home.html';
    }
}

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

function showRegistrationLoading(isLoading) {
    const submitBtn = document.querySelector('button[type="submit"]');
    if (submitBtn) {
        if (isLoading) {
            submitBtn.disabled = true;
            submitBtn.innerHTML = `
                <span class="loading loading-spinner loading-xs"></span>
                Creating Account...
            `;
        } else {
            submitBtn.disabled = false;
            submitBtn.innerHTML = 'Create Account';
        }
    }
}

function showRegistrationSuccess() {
    const toast = document.createElement('div');
    toast.className = 'fixed top-4 right-4 bg-green-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
    toast.innerHTML = `
        <div class="flex items-center gap-2">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
            </svg>
            <span>Account created successfully! Logging you in...</span>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 5000);
}

function showRegistrationError(message) {
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
    
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 5000);
}

// Logo click handler
document.getElementById('logo')?.addEventListener('click', function(e) {
    window.location.href = '/home.html';
});

// Setup modal sign-in form if not already handled by auth.js
window.setupModalSignIn = function() {
    const modalForm = document.getElementById('modal-signin-form');
    if (modalForm) {
        modalForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const formData = new FormData(modalForm);
            const loginData = {
                email: formData.get('email'),
                password: formData.get('password')
            };
            
            // Use the performLogin function from auth.js
            if (typeof performLogin === 'function') {
                const success = await performLogin(loginData.email, loginData.password);
                if (success) {
                    document.getElementById('signin-modal').close();
                }
            } else {
                console.error('performLogin function not available');
            }
        });
    }
};