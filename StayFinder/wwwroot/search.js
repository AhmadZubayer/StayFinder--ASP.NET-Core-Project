// Search functionality for properties
document.addEventListener('DOMContentLoaded', function() {
    // Get search elements
    const headerSearchInput = document.querySelector('#search-bar input[type="search"]');
    const destinationInput = document.querySelector('input[placeholder="Search destinations"]');
    const checkinInput = document.querySelector('input[type="date"]:first-of-type');
    const checkoutInput = document.querySelector('input[type="date"]:last-of-type');
    const searchButton = document.querySelector('button:contains("Find Your Place")');
    const findButton = document.querySelector('.bg-\\[\\#04aa6d\\].text-white.px-6.py-3.rounded-full');
    
    // Get all property cards
    const propertyCards = document.querySelectorAll('.property-card');
    
    // Header search functionality
    if (headerSearchInput) {
        headerSearchInput.addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase().trim();
            filterProperties(searchTerm);
        });
    }
    
    // Main search form functionality
    if (destinationInput) {
        destinationInput.addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase().trim();
            filterProperties(searchTerm);
        });
    }
    
    // Find Your Place button functionality
    if (findButton) {
        findButton.addEventListener('click', function(e) {
            e.preventDefault();
            performAdvancedSearch();
        });
    }
    
    // Function to filter properties based on search term
    function filterProperties(searchTerm) {
        let visibleCount = 0;
        
        propertyCards.forEach(card => {
            const city = card.dataset.city?.toLowerCase() || '';
            const title = card.dataset.title?.toLowerCase() || '';
            const features = card.dataset.features?.toLowerCase() || '';
            
            const isMatch = searchTerm === '' || 
                           city.includes(searchTerm) || 
                           title.includes(searchTerm) || 
                           features.includes(searchTerm);
            
            if (isMatch) {
                card.style.display = 'block';
                card.style.animation = 'fadeIn 0.3s ease-in';
                visibleCount++;
            } else {
                card.style.display = 'none';
            }
        });
        
        // Show/hide no results message
        showNoResultsMessage(visibleCount === 0 && searchTerm !== '');
    }
    
    // Advanced search with all filters
    function performAdvancedSearch() {
        const destination = destinationInput?.value.toLowerCase().trim() || '';
        const checkinDate = checkinInput?.value || '';
        const checkoutDate = checkoutInput?.value || '';
        const guests = parseInt(document.getElementById('guestCount')?.textContent) || 2;
        
        let visibleCount = 0;
        
        propertyCards.forEach(card => {
            const city = card.dataset.city?.toLowerCase() || '';
            const title = card.dataset.title?.toLowerCase() || '';
            const features = card.dataset.features?.toLowerCase() || '';
            
            // Check destination match
            const destinationMatch = destination === '' || 
                                   city.includes(destination) || 
                                   title.includes(destination) || 
                                   features.includes(destination);
            
            // For demo purposes, assume all properties are available for selected dates
            const dateMatch = true;
            
            // For demo purposes, assume all properties can accommodate the guest count
            const guestMatch = true;
            
            if (destinationMatch && dateMatch && guestMatch) {
                card.style.display = 'block';
                card.style.animation = 'fadeIn 0.3s ease-in';
                visibleCount++;
            } else {
                card.style.display = 'none';
            }
        });
        
        // Show search results summary
        showSearchSummary(destination, checkinDate, checkoutDate, guests, visibleCount);
        
        // Show/hide no results message
        showNoResultsMessage(visibleCount === 0);
    }
    
    // Function to show no results message
    function showNoResultsMessage(show) {
        let noResultsDiv = document.getElementById('no-results-message');
        
        if (show && !noResultsDiv) {
            noResultsDiv = document.createElement('div');
            noResultsDiv.id = 'no-results-message';
            noResultsDiv.className = 'text-center py-12 col-span-full';
            noResultsDiv.innerHTML = `
                <svg class="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                </svg>
                <h3 class="text-xl font-medium text-gray-800 mb-2">No Properties Found</h3>
                <p class="text-gray-600">Try adjusting your search criteria or browse all properties.</p>
                <button onclick="clearSearch()" class="btn bg-[#04aa6d] text-white mt-4 hover:bg-[#038a5a] border-none">
                    Clear Search
                </button>
            `;
            
            const propertiesGrid = document.getElementById('properties-grid');
            propertiesGrid.appendChild(noResultsDiv);
        } else if (!show && noResultsDiv) {
            noResultsDiv.remove();
        }
    }
    
    // Function to show search summary
    function showSearchSummary(destination, checkin, checkout, guests, count) {
        let summaryDiv = document.getElementById('search-summary');
        
        if (!summaryDiv) {
            summaryDiv = document.createElement('div');
            summaryDiv.id = 'search-summary';
            summaryDiv.className = 'bg-blue-50 border-l-4 border-blue-400 p-4 mb-6 mx-6';
            
            const propertyContainer = document.querySelector('.property-card-container');
            const title = propertyContainer.querySelector('p');
            title.parentNode.insertBefore(summaryDiv, title.nextSibling);
        }
        
        let summaryText = `Found ${count} properties`;
        if (destination) summaryText += ` in "${destination}"`;
        if (checkin && checkout) summaryText += ` from ${checkin} to ${checkout}`;
        if (guests > 1) summaryText += ` for ${guests} guests`;
        
        summaryDiv.innerHTML = `
            <div class="flex items-center justify-between">
                <div>
                    <p class="text-blue-800 font-medium">${summaryText}</p>
                </div>
                <button onclick="clearSearch()" class="text-blue-600 hover:text-blue-800 text-sm">
                    Clear filters
                </button>
            </div>
        `;
        
        // Auto-hide after 5 seconds if there are results
        if (count > 0) {
            setTimeout(() => {
                if (summaryDiv && summaryDiv.parentNode) {
                    summaryDiv.style.animation = 'fadeOut 0.3s ease-out';
                    setTimeout(() => summaryDiv.remove(), 300);
                }
            }, 5000);
        }
    }
    
    // Add fadeIn/fadeOut animations
    const style = document.createElement('style');
    style.textContent = `
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }
        @keyframes fadeOut {
            from { opacity: 1; transform: translateY(0); }
            to { opacity: 0; transform: translateY(-10px); }
        }
    `;
    document.head.appendChild(style);
});

// Global function to clear search
function clearSearch() {
    // Clear all search inputs
    const headerSearchInput = document.querySelector('#search-bar input[type="search"]');
    const destinationInput = document.querySelector('input[placeholder="Search destinations"]');
    const checkinInput = document.querySelector('input[type="date"]:first-of-type');
    const checkoutInput = document.querySelector('input[type="date"]:last-of-type');
    
    if (headerSearchInput) headerSearchInput.value = '';
    if (destinationInput) destinationInput.value = '';
    if (checkinInput) checkinInput.value = '';
    if (checkoutInput) checkoutInput.value = '';
    
    // Reset guest count to 2
    if (typeof updateGuestDisplay === 'function') {
        guestCount = 2;
        updateGuestDisplay();
    }
    
    // Show all properties
    const propertyCards = document.querySelectorAll('.property-card');
    propertyCards.forEach(card => {
        card.style.display = 'block';
        card.style.animation = 'fadeIn 0.3s ease-in';
    });
    
    // Remove no results and search summary messages
    const noResultsDiv = document.getElementById('no-results-message');
    const searchSummary = document.getElementById('search-summary');
    
    if (noResultsDiv) noResultsDiv.remove();
    if (searchSummary) searchSummary.remove();
}
