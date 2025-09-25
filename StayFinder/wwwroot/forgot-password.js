// Forgot Password functionality
document.addEventListener('DOMContentLoaded', function() {
    // Get the forgot password link from the sign-in modal
    const forgotPasswordLink = document.querySelector('#signin-modal a[href="#"]');
    
    if (forgotPasswordLink) {
        forgotPasswordLink.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Close sign-in modal first
            document.getElementById('signin-modal').close();
            
            // Show forgot password modal
            showForgotPasswordModal();
        });
    }
    
    // Function to create and show forgot password modal
    function showForgotPasswordModal() {
        // Create forgot password modal using DOM manipulation
        const forgotModal = document.createElement('div');
        forgotModal.id = 'forgot-password-modal';
        forgotModal.className = 'fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50';
        forgotModal.innerHTML = `
            <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4 shadow-xl">
                <div class="flex justify-between items-center mb-4">
                    <h3 class="text-xl font-bold text-gray-800">Forgot Password</h3>
                    <button id="close-forgot-modal" class="text-gray-500 hover:text-gray-700">
                        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                        </svg>
                    </button>
                </div>
                
                <div class="mb-6">
                    <p class="text-gray-600 text-sm mb-4">Enter your email address and we'll send you a link to reset your password.</p>
                    
                    <form id="forgot-password-form" class="space-y-4">
                        <div>
                            <label class="block text-sm font-medium text-gray-700 mb-1">Email Address</label>
                            <input type="email" id="forgot-email" class="input input-bordered w-full" placeholder="Enter your email" required>
                        </div>
                        
                        <button type="submit" class="btn bg-[#04aa6d] text-white w-full hover:bg-[#038a5a] border-none">
                            Send Reset Link
                        </button>
                    </form>
                </div>
                
                <div class="text-center">
                    <p class="text-sm text-gray-600">
                        Remember your password? 
                        <button id="back-to-signin" class="text-[#04aa6d] hover:underline font-medium">Sign in</button>
                    </p>
                </div>
            </div>
        `;
        
        document.body.appendChild(forgotModal);
        
        // Add event listeners
        const closeBtn = forgotModal.querySelector('#close-forgot-modal');
        const backToSigninBtn = forgotModal.querySelector('#back-to-signin');
        const forgotForm = forgotModal.querySelector('#forgot-password-form');
        
        // Close modal when clicking X
        closeBtn.addEventListener('click', function() {
            closeForgotPasswordModal();
        });
        
        // Close modal when clicking outside
        forgotModal.addEventListener('click', function(e) {
            if (e.target === forgotModal) {
                closeForgotPasswordModal();
            }
        });
        
        // Back to sign in
        backToSigninBtn.addEventListener('click', function() {
            closeForgotPasswordModal();
            // Reopen sign-in modal
            document.getElementById('signin-modal').showModal();
        });
        
        // Handle form submission
        forgotForm.addEventListener('submit', function(e) {
            e.preventDefault();
            handleForgotPasswordSubmission();
        });
        
        // Focus on email input
        document.getElementById('forgot-email').focus();
    }
    
    // Function to close forgot password modal
    function closeForgotPasswordModal() {
        const modal = document.getElementById('forgot-password-modal');
        if (modal && modal.parentNode) {
            modal.parentNode.removeChild(modal);
        }
    }
    
    // Function to handle forgot password form submission
    function handleForgotPasswordSubmission() {
        const emailInput = document.getElementById('forgot-email');
        const email = emailInput.value.trim();
        
        // Simple email validation
        if (!isValidEmail(email)) {
            showErrorMessage('Please enter a valid email address.');
            return;
        }
        
        // Check if email exists in our system (using the same email as login)
        if (email === 'ahmadzubayer007@gmail.com') {
            // Show success message
            showResetLinkSent(email);
        } else {
            // Show error for unregistered email
            showErrorMessage('This email address is not registered with us.');
        }
    }
    
    // Function to show success message
    function showResetLinkSent(email) {
        // Replace modal content with success message
        const modal = document.getElementById('forgot-password-modal');
        const modalContent = modal.querySelector('.bg-white');
        
        modalContent.innerHTML = `
            <div class="text-center p-6">
                <svg class="w-16 h-16 text-green-500 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                
                <h3 class="text-xl font-bold text-gray-800 mb-2">Reset Link Sent!</h3>
                <p class="text-gray-600 mb-4">We've sent a password reset link to:</p>
                <p class="text-sm font-medium text-[#04aa6d] mb-6">${email}</p>
                <p class="text-sm text-gray-500 mb-6">Please check your email and follow the instructions to reset your password.</p>
                
                <div class="space-y-2">
                    <button id="close-success-modal" class="btn bg-[#04aa6d] text-white w-full hover:bg-[#038a5a] border-none">
                        Close
                    </button>
                    <button id="back-to-signin-success" class="btn btn-outline w-full">
                        Back to Sign In
                    </button>
                </div>
            </div>
        `;
        
        // Add event listeners for success modal buttons
        const closeSuccessBtn = modal.querySelector('#close-success-modal');
        const backToSigninSuccessBtn = modal.querySelector('#back-to-signin-success');
        
        closeSuccessBtn.addEventListener('click', function() {
            closeForgotPasswordModal();
        });
        
        backToSigninSuccessBtn.addEventListener('click', function() {
            closeForgotPasswordModal();
            document.getElementById('signin-modal').showModal();
        });
    }
    
    // Function to show error message
    function showErrorMessage(message) {
        // Create error toast
        const errorToast = document.createElement('div');
        errorToast.className = 'fixed top-4 right-4 bg-red-500 text-white px-6 py-3 rounded-lg shadow-lg z-50';
        errorToast.innerHTML = `
            <div class="flex items-center gap-2">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
                </svg>
                <span>${message}</span>
            </div>
        `;
        
        document.body.appendChild(errorToast);
        
        // Remove toast after 3 seconds
        setTimeout(() => {
            if (errorToast && errorToast.parentNode) {
                errorToast.parentNode.removeChild(errorToast);
            }
        }, 3000);
    }
    
    // Function to validate email format
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
});
