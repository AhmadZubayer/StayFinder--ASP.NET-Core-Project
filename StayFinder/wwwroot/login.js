// Login functionality with API integration
document.addEventListener('DOMContentLoaded', function() {
    // Get the sign-in form
    const signinForm = document.querySelector('#signin-modal form.space-y-4');
    
    if (signinForm) {
        signinForm.addEventListener('submit', function(e) {
            e.preventDefault(); // Prevent default form submission
            
            // Get input values using DOM manipulation
            const emailInput = signinForm.querySelector('input[type="email"]');
            const passwordInput = signinForm.querySelector('input[type="password"]');
            
            const enteredEmail = emailInput.value.trim();
            const enteredPassword = passwordInput.value.trim();
            
            // Validate inputs
            if (!enteredEmail || !enteredPassword) {
                showLoginError('Please fill in all fields');
                return;
            }
            
            // Show loading state
            showLoginLoading(true);
            
            // API login call
            performLogin(enteredEmail, enteredPassword);
                    setTimeout(function() {
                        window.location.href = 'user-dashboard.html';
                    }, 1500);
                }
                
            } else {
                // Failed login
                showLoginError();
            }
        });
    }
    
    // Function to show success message
    function showLoginSuccess() {
        // Create success toast notification
        const toast = document.createElement('div');
        toast.className = 'fixed top-4 right-4 bg-green-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
        toast.innerHTML = `
            <div class="flex items-center gap-2">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"/>
                </svg>
                <span>Login Successful! Redirecting...</span>
            </div>
        `;
        
        document.body.appendChild(toast);
        
        // Remove toast after 3 seconds
        setTimeout(() => {
            if (toast && toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 3000);
    }
    
    // Function to show error message
    function showLoginError() {
        // Create error toast notification
        const toast = document.createElement('div');
        toast.className = 'fixed top-4 right-4 bg-red-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
        toast.innerHTML = `
            <div class="flex items-center gap-2">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
                </svg>
                <span>Invalid email or password!</span>
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
        
        // Remove toast after 3 seconds
        setTimeout(() => {
            if (toast && toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 3000);
    }
    
    // Function to process booking after successful login
    function processBookingAfterLogin(propertyData) {
        // Get existing bookings or create new array
        const existingBookings = JSON.parse(localStorage.getItem('userBookings') || '[]');
        
        // Add new booking with unique ID
        propertyData.bookingId = 'BK' + Date.now();
        existingBookings.push(propertyData);
        
        // Save to localStorage
        localStorage.setItem('userBookings', JSON.stringify(existingBookings));
        localStorage.setItem('showNewBooking', 'true');
        localStorage.setItem('newBookingId', propertyData.bookingId);
        
        // Show regular login success message
        showLoginSuccess();
        
        // Close the modal
        document.getElementById('signin-modal').close();
        
        // Redirect to user dashboard after showing success
        setTimeout(function() {
            window.location.href = 'user-dashboard.html';
        }, 1500);
    }
    
    // Add CSS animation for shake effect
    const style = document.createElement('style');
    style.textContent = `
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
            20%, 40%, 60%, 80% { transform: translateX(5px); }
        }
    `;
    document.head.appendChild(style);
});

// Additional function to handle Google sign-in button (placeholder)
document.addEventListener('DOMContentLoaded', function() {
    const googleSigninBtn = document.querySelector('#signin-modal button[type="button"]');
    
    if (googleSigninBtn) {
        googleSigninBtn.addEventListener('click', function() {
            alert('Google Sign-in integration would be implemented here.');
        });
    }
});
