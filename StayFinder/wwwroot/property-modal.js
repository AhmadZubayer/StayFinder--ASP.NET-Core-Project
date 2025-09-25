// Property Modal Functionality
document.addEventListener('DOMContentLoaded', function() {
    // Get modal elements
    const propertyModal = document.getElementById('property-modal');
    const modalImage = propertyModal.querySelector('img');
    const modalTitle = propertyModal.querySelector('h3');
    const modalLocation = propertyModal.querySelector('p:has(svg)');
    const modalFeatures = propertyModal.querySelector('.grid .space-y-1:first-child');
    const modalDetails = propertyModal.querySelector('.grid .space-y-1:last-child');
    const modalPrice = propertyModal.querySelector('.text-3xl');
    const modalDescription = propertyModal.querySelector('.py-4');
    
    // Get all view details buttons
    const viewDetailsButtons = document.querySelectorAll('.view-details-btn');
    
    // Add click event listeners to all view details buttons
    viewDetailsButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Get the property card that contains this button
            const propertyCard = this.closest('.property-card');
            
            if (propertyCard) {
                populateModal(propertyCard);
                propertyModal.showModal();
            }
        });
    });
    
    // Function to populate modal with property data
    function populateModal(propertyCard) {
        // Extract data from the property card
        const data = {
            image: propertyCard.dataset.image || propertyCard.querySelector('img').src,
            title: propertyCard.dataset.fullTitle || propertyCard.querySelector('.property-title').textContent,
            city: propertyCard.dataset.fullCity || propertyCard.querySelector('.property-city').textContent + ', Bangladesh',
            price: propertyCard.dataset.price || propertyCard.querySelector('.property-price').textContent.split('/')[0],
            rating: propertyCard.dataset.rating || '4.5',
            reviews: propertyCard.dataset.reviews || '50',
            type: propertyCard.dataset.type || 'Property',
            amenities: propertyCard.dataset.amenities ? propertyCard.dataset.amenities.split(',') : ['WiFi', 'Clean Environment'],
            description: propertyCard.dataset.description || 'A wonderful property with excellent amenities and great location.'
        };
        
        // Update modal content
        if (modalImage) {
            modalImage.src = data.image;
            modalImage.alt = data.title;
        }
        
        if (modalTitle) {
            modalTitle.textContent = data.title;
        }
        
        if (modalLocation) {
            modalLocation.innerHTML = `
                <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd"/>
                </svg>
                ${data.city}
            `;
        }
        
        if (modalPrice) {
            modalPrice.textContent = data.price;
        }
        
        // Update features list
        if (modalFeatures) {
            const featuresList = modalFeatures.querySelector('ul') || modalFeatures;
            featuresList.innerHTML = data.amenities.map(amenity => `<li>• ${amenity.trim()}</li>`).join('');
        }
        
        // Update details section
        if (modalDetails) {
            const detailsContainer = modalDetails.querySelector('div') || modalDetails;
            detailsContainer.innerHTML = `
                <p>Rating: ⭐ ${data.rating} (${data.reviews} reviews)</p>
                <p>Available: Sept 10 - Dec 31</p>
                <p>Property Type: ${data.type}</p>
                <p>Instant Book: Available</p>
            `;
        }
        
        // Add description if there's a place for it
        const descriptionContainer = modalDescription.querySelector('.py-4');
        if (descriptionContainer && !descriptionContainer.querySelector('.description-text')) {
            const descDiv = document.createElement('div');
            descDiv.className = 'description-text mb-4 p-3 bg-gray-50 rounded-lg';
            descDiv.innerHTML = `
                <h4 class="font-semibold text-lg mb-2">About this place</h4>
                <p class="text-gray-700 text-sm leading-relaxed">${data.description}</p>
            `;
            
            // Insert before the grid
            const gridElement = descriptionContainer.querySelector('.grid');
            if (gridElement) {
                descriptionContainer.insertBefore(descDiv, gridElement);
            }
        } else if (descriptionContainer && descriptionContainer.querySelector('.description-text')) {
            // Update existing description
            const existingDesc = descriptionContainer.querySelector('.description-text p');
            if (existingDesc) {
                existingDesc.textContent = data.description;
            }
        }
    }
    
    // Add booking functionality
    const bookNowBtn = propertyModal.querySelector('.btn.bg-\\[\\#04aa6d\\]');
    if (bookNowBtn) {
        bookNowBtn.addEventListener('click', function() {
            // Get current property data
            const propertyData = {
                title: modalTitle.textContent,
                city: modalLocation.textContent.replace('📍 ', '').trim(),
                price: modalPrice.textContent,
                image: modalImage.src,
                rating: modalDetails.querySelector('p').textContent.match(/Rating: ⭐ ([\d.]+)/)?.[1] || '4.5',
                type: modalDetails.querySelector('p:nth-child(3)').textContent.replace('Property Type: ', ''),
                bookingDate: new Date().toLocaleDateString(),
                checkIn: getDefaultCheckIn(),
                checkOut: getDefaultCheckOut(),
                guests: 2,
                status: 'Confirmed'
            };
            
            // Check if user is logged in
            const currentUser = localStorage.getItem('currentUser');
            if (!currentUser) {
                // Store the property data for after login
                localStorage.setItem('pendingBooking', JSON.stringify(propertyData));
                localStorage.setItem('bookingIntent', 'true');
                
                // Show sign in modal
                propertyModal.close();
                document.getElementById('signin-modal').showModal();
                return;
            }
            
            // User is logged in, process booking immediately
            processBooking(propertyData);
        });
    }
    
    // Helper function to get default check-in date (today + 1 day)
    function getDefaultCheckIn() {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        return tomorrow.toLocaleDateString();
    }
    
    // Helper function to get default check-out date (today + 3 days)
    function getDefaultCheckOut() {
        const checkout = new Date();
        checkout.setDate(checkout.getDate() + 3);
        return checkout.toLocaleDateString();
    }
    
    // Function to process booking
    function processBooking(propertyData) {
        // Get existing bookings or create new array
        const existingBookings = JSON.parse(localStorage.getItem('userBookings') || '[]');
        
        // Add new booking with unique ID
        propertyData.bookingId = 'BK' + Date.now();
        existingBookings.push(propertyData);
        
        // Save to localStorage
        localStorage.setItem('userBookings', JSON.stringify(existingBookings));
        localStorage.setItem('showNewBooking', 'true');
        localStorage.setItem('newBookingId', propertyData.bookingId);
        
        // Close modal and redirect to dashboard without confirmation dialog
        propertyModal.close();
        window.location.href = 'user-dashboard.html';
    }
    
    // Add keyboard navigation
    propertyModal.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            this.close();
        }
    });
    
    // Add click outside to close
    propertyModal.addEventListener('click', function(e) {
        if (e.target === this) {
            this.close();
        }
    });
});

// Function to handle modal opening from external sources
function openPropertyModal(propertyData) {
    const propertyModal = document.getElementById('property-modal');
    if (propertyModal && propertyData) {
        // Create a temporary element with the data
        const tempCard = document.createElement('div');
        tempCard.dataset.image = propertyData.image;
        tempCard.dataset.fullTitle = propertyData.title;
        tempCard.dataset.fullCity = propertyData.city;
        tempCard.dataset.price = propertyData.price;
        tempCard.dataset.rating = propertyData.rating;
        tempCard.dataset.reviews = propertyData.reviews;
        tempCard.dataset.type = propertyData.type;
        tempCard.dataset.amenities = propertyData.amenities;
        tempCard.dataset.description = propertyData.description;
        
        populateModal(tempCard);
        propertyModal.showModal();
    }
}
