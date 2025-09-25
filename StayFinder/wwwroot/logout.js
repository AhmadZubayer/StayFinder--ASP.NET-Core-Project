// Logout functionality - Simple and direct
document.addEventListener('DOMContentLoaded', function() {
    // Get the logout button by its specific ID
    const logoutBtn = document.getElementById('logout-btn');
    
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function(e) {
            e.preventDefault(); // Prevent default link behavior
            
            // Perform immediate logout
            performLogout();
        });
    }
    
    // Function to perform the logout - simple and direct
    function performLogout() {
        // Clear any stored user data (localStorage, sessionStorage)
        localStorage.removeItem('userSession');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('loginTime');
        sessionStorage.clear();
        
        // Redirect immediately to home page
        window.location.href = 'home.html';
    }
});

// Optional: Function to check if user is logged in (can be used across pages)
function isUserLoggedIn() {
    return localStorage.getItem('userSession') !== null;
}

// Optional: Function to get current user info
function getCurrentUser() {
    const userEmail = localStorage.getItem('userEmail');
    return userEmail || null;
}
