// User Dashboard Functionality

// Check user authentication
function checkUserAuthentication() {
    const currentUser = localStorage.getItem('currentUser');
    const userSession = localStorage.getItem('userSession');
    
    if (!currentUser || userSession !== 'active') {
        // Redirect to home page if not logged in
        window.location.href = 'home.html';
        return;
    }
}

// Load user information
function loadUserInfo() {
    const userEmail = localStorage.getItem('userEmail') || localStorage.getItem('currentUser');
    const loginTime = localStorage.getItem('loginTime');
    
    // Update welcome message if there's a place for it
    const welcomeElement = document.querySelector('.welcome-message, h1');
    if (welcomeElement && userEmail) {
        const firstName = userEmail.split('@')[0].split('.')[0];
        const capitalizedName = firstName.charAt(0).toUpperCase() + firstName.slice(1);
        
        if (welcomeElement.tagName === 'H1') {
            welcomeElement.textContent = `Welcome back, ${capitalizedName}!`;
        }
    }
}

// Load and display user bookings
function loadUserBookings() {
    const bookings = JSON.parse(localStorage.getItem('userBookings') || '[]');
    
    // Get booking containers
    const bookingsContainer = document.querySelector('#bookings-content .grid');
    const historyContainer = document.querySelector('#history-content .grid');
    
    if (bookings.length === 0) {
        // Show no bookings message (already in HTML)
        return;
    }
    
    // Clear existing content
    if (bookingsContainer) {
        bookingsContainer.innerHTML = '';
    }
    if (historyContainer) {
        historyContainer.innerHTML = '';
    }
    
    // Separate current bookings from history
    const currentBookings = bookings.filter(booking => booking.status === 'Confirmed');
    const historyBookings = bookings.filter(booking => booking.status !== 'Confirmed');
    
    // Display current bookings
    if (currentBookings.length > 0) {
        // Sort bookings to show new booking first
        const newBookingId = localStorage.getItem('newBookingId');
        const sortedBookings = currentBookings.sort((a, b) => {
            if (a.bookingId === newBookingId) return -1; // New booking first
            if (b.bookingId === newBookingId) return 1;
            return 0; // Keep original order for others
        });
        
        sortedBookings.forEach((booking, index) => {
            const bookingCard = createBookingCard(booking, true);
            bookingCard.classList.add('booking-card-enter');
            bookingCard.style.animationDelay = `${index * 0.1}s`;
            bookingsContainer.appendChild(bookingCard);
        });
    } else {
        // Show no current bookings message
        const noBookingsDiv = document.createElement('div');
        noBookingsDiv.className = 'bg-white rounded-lg shadow-sm border p-6';
        noBookingsDiv.innerHTML = `
            <p class="text-gray-600">No current bookings found.</p>
            <p class="text-sm text-gray-500 mt-2">Your upcoming reservations will appear here.</p>
            <a href="home.html" class="btn bg-[#04aa6d] text-white mt-4 hover:bg-[#038a5a] border-none">
                Browse Properties
            </a>
        `;
        bookingsContainer.appendChild(noBookingsDiv);
    }
    
    // Display booking history
    if (historyBookings.length > 0) {
        historyBookings.forEach(booking => {
            const bookingCard = createBookingCard(booking, false);
            historyContainer.appendChild(bookingCard);
        });
    } else {
        // Show no history message
        const noHistoryDiv = document.createElement('div');
        noHistoryDiv.className = 'bg-white rounded-lg shadow-sm border p-6';
        noHistoryDiv.innerHTML = `
            <p class="text-gray-600">No booking history found.</p>
            <p class="text-sm text-gray-500 mt-2">Your past stays will appear here.</p>
        `;
        historyContainer.appendChild(noHistoryDiv);
    }
}

// Create booking card HTML
function createBookingCard(booking, isCurrent = true) {
    const card = document.createElement('div');
    const isNewBooking = localStorage.getItem('newBookingId') === booking.bookingId;
    
    // Special styling for new bookings
    const cardClasses = isNewBooking ? 
        'bg-gradient-to-r from-green-50 to-emerald-50 rounded-lg shadow-lg border-2 border-green-200 p-6 hover:shadow-xl transition-all duration-300 animate-pulse-slow' : 
        'bg-white rounded-lg shadow-sm border p-6 hover:shadow-md transition-shadow';
    
    card.className = cardClasses;
    
    const statusColor = booking.status === 'Confirmed' ? 'bg-green-100 text-green-800' : 
                       booking.status === 'Cancelled' ? 'bg-red-100 text-red-800' : 
                       'bg-blue-100 text-blue-800';

    card.innerHTML = `
        ${isNewBooking ? `
            <div class="mb-4 p-3 bg-green-100 border-l-4 border-green-500 rounded-r-lg">
                <div class="flex items-center">
                    <svg class="w-5 h-5 text-green-600 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                    </svg>
                    <div class="text-green-800">
                        <p class="font-semibold text-sm">🎉 Booking Confirmed Successfully!</p>
                        <p class="text-xs">Your reservation is confirmed and ready for your stay.</p>
                    </div>
                </div>
            </div>
        ` : ''}
        
        <div class="flex flex-col md:flex-row md:items-center md:justify-between">
            <div class="flex items-start space-x-4">
                <img src="${booking.image}" alt="${booking.title}" class="w-16 h-16 rounded-lg object-cover ${isNewBooking ? 'ring-2 ring-green-300' : ''}">
                <div class="flex-1">
                    <h3 class="font-semibold text-lg ${isNewBooking ? 'text-green-900' : 'text-gray-800'}">${booking.title}</h3>
                    <p class="text-gray-600 text-sm flex items-center mt-1">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd"/>
                        </svg>
                        ${booking.city}
                    </p>
                    <p class="text-sm text-gray-500 mt-1">Booking ID: ${booking.bookingId}</p>
                </div>
            </div>
            
            <div class="mt-4 md:mt-0 md:text-right">
                <div class="flex flex-col items-start md:items-end space-y-2">
                    <span class="px-3 py-1 text-xs font-medium rounded-full ${statusColor}">
                        ${booking.status}
                    </span>
                    <div class="text-sm text-gray-600">
                        <p><strong>Check-in:</strong> ${booking.checkIn}</p>
                        <p><strong>Check-out:</strong> ${booking.checkOut}</p>
                        <p><strong>Guests:</strong> ${booking.guests}</p>
                    </div>
                    <p class="text-xl font-bold ${isNewBooking ? 'text-green-600' : 'text-[#04aa6d]'}">${booking.price}/day</p>
                </div>
            </div>
        </div>
        
        <div class="mt-4 pt-4 border-t ${isNewBooking ? 'border-green-200' : 'border-gray-100'}">
            <div class="flex flex-wrap gap-2 justify-between items-center">
                <div class="text-sm text-gray-500">
                    <span>Booked on: ${booking.bookingDate}</span>
                    ${booking.rating ? `<span class="ml-4">★ ${booking.rating}</span>` : ''}
                    ${isNewBooking ? `<span class="ml-4 text-green-600 font-semibold">✨ New Booking</span>` : ''}
                </div>
                <div class="space-x-2">
                    ${isCurrent ? `
                        <button onclick="viewBookingDetails('${booking.bookingId}')" class="btn btn-sm btn-outline">
                            View Details
                        </button>
                        <button onclick="cancelBooking('${booking.bookingId}')" class="btn btn-sm btn-error btn-outline">
                            Cancel
                        </button>
                    ` : `
                        <button onclick="viewBookingDetails('${booking.bookingId}')" class="btn btn-sm btn-outline">
                            View Details
                        </button>
                        ${booking.status === 'Completed' ? `
                            <button onclick="leaveReview('${booking.bookingId}')" class="btn btn-sm bg-[#04aa6d] text-white hover:bg-[#038a5a] border-none">
                                Leave Review
                            </button>
                        ` : ''}
                    `}
                </div>
            </div>
        </div>
    `;
    
    return card;
}

// View booking details
function viewBookingDetails(bookingId) {
    const bookings = JSON.parse(localStorage.getItem('userBookings') || '[]');
    const booking = bookings.find(b => b.bookingId === bookingId);
    
    if (booking) {
        alert(`Booking Details:\n\nProperty: ${booking.title}\nLocation: ${booking.city}\nBooking ID: ${booking.bookingId}\nCheck-in: ${booking.checkIn}\nCheck-out: ${booking.checkOut}\nGuests: ${booking.guests}\nPrice: ${booking.price}/day\nStatus: ${booking.status}\nBooked on: ${booking.bookingDate}`);
    }
}

// Cancel booking
function cancelBooking(bookingId) {
    if (confirm('Are you sure you want to cancel this booking?\n\nNote: Cancellation policies may apply.')) {
        const bookings = JSON.parse(localStorage.getItem('userBookings') || '[]');
        const bookingIndex = bookings.findIndex(b => b.bookingId === bookingId);
        
        if (bookingIndex !== -1) {
            bookings[bookingIndex].status = 'Cancelled';
            localStorage.setItem('userBookings', JSON.stringify(bookings));
            
            // Show success message
            showMessage('Booking cancelled successfully', 'success');
            
            // Reload bookings
            setTimeout(() => {
                loadUserBookings();
            }, 1000);
        }
    }
}

// Leave review
function leaveReview(bookingId) {
    const rating = prompt('Please rate your stay (1-5 stars):');
    const review = prompt('Leave a review (optional):');
    
    if (rating && rating >= 1 && rating <= 5) {
        showMessage('Thank you for your review!', 'success');
        // Here you would typically send the review to a server
        console.log(`Review for booking ${bookingId}: ${rating} stars, "${review}"`);
    }
}

// Show message utility
function showMessage(message, type = 'success') {
    const toast = document.createElement('div');
    const bgColor = type === 'success' ? 'bg-green-500' : 'bg-red-500';
    
    toast.className = `fixed top-4 right-4 ${bgColor} text-white px-6 py-4 rounded-lg shadow-lg z-[9999] max-w-sm`;
    toast.textContent = message;
    
    document.body.appendChild(toast);
    
    // Remove after delay
    setTimeout(() => {
        if (toast && toast.parentNode) {
            toast.parentNode.removeChild(toast);
        }
    }, 3000);
}

// Function to highlight new bookings (called from login redirect)
function highlightNewBooking() {
    const newBookingId = localStorage.getItem('newBookingId');
    if (newBookingId) {
        // Remove the highlight after some time
        setTimeout(() => {
            localStorage.removeItem('newBookingId');
            // Reload the bookings to remove the special styling
            setTimeout(() => {
                loadUserBookings();
            }, 5000); // Keep highlight for 5 seconds
        }, 1000);
    }
}

// Add custom CSS for animations
function addCustomStyles() {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes pulse-slow {
            0%, 100% { 
                opacity: 1; 
                transform: scale(1);
            }
            50% { 
                opacity: 0.95; 
                transform: scale(1.01);
            }
        }
        
        .animate-pulse-slow {
            animation: pulse-slow 2s ease-in-out infinite;
        }
        
        @keyframes slideInUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .booking-card-enter {
            animation: slideInUp 0.5s ease-out;
        }
    `;
    document.head.appendChild(style);
}

// Call add styles when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    addCustomStyles();
    
    // Check if user is logged in
    checkUserAuthentication();
    
    // Load user bookings
    loadUserBookings();
    
    // Load user information
    loadUserInfo();
});

// Call highlight function if redirected from booking flow
if (localStorage.getItem('showNewBooking') === 'true') {
    localStorage.removeItem('showNewBooking');
    setTimeout(highlightNewBooking, 1000);
}
