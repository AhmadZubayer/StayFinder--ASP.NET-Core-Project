// Global variables
let allProperties = [];
let currentFilter = 'all';
let currentSort = '';

// DOM Content Loaded Event
document.addEventListener('DOMContentLoaded', function() {
    loadProperties();
    setupEventListeners();
    setupSearchFunctionality();
    setupFilterFunctionality();
});

// Setup event listeners for tabs and sorting
function setupEventListeners() {
    // Tab filtering
    const tabs = document.querySelectorAll('input[name="my_tabs_1"]');
    tabs.forEach(tab => {
        tab.addEventListener('change', function() {
            if (this.checked) {
                const filterType = this.getAttribute('aria-label').toLowerCase();
                currentFilter = filterType;
                filterAndDisplayProperties();
            }
        });
    });
}

// Load properties from API
async function loadProperties() {
    try {
        showLoadingState(true);
        const response = await fetch('/api/properties');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        allProperties = await response.json();
        displayProperties(allProperties);
        showLoadingState(false);
    } catch (error) {
        console.error('Error loading properties:', error);
        showLoadingState(false);
        displayErrorMessage('Failed to load properties. Please try again later.');
    }
}

// Filter and display properties based on current filter and sort
async function filterAndDisplayProperties() {
    try {
        showLoadingState(true);
        const queryParams = new URLSearchParams();
        
        if (currentFilter !== 'all') {
            queryParams.append('type', currentFilter);
        }
        
        if (currentSort) {
            queryParams.append('sortBy', currentSort);
        }

        const url = `/api/properties/filter?${queryParams.toString()}`;
        const response = await fetch(url);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const filteredProperties = await response.json();
        displayProperties(filteredProperties);
        showLoadingState(false);
    } catch (error) {
        console.error('Error filtering properties:', error);
        showLoadingState(false);
        displayErrorMessage('Failed to filter properties. Please try again later.');
    }
}

// Advanced search functionality
async function performAdvancedSearch(searchParams) {
    try {
        showLoadingState(true);
        const queryParams = new URLSearchParams();
        
        Object.keys(searchParams).forEach(key => {
            if (searchParams[key] !== null && searchParams[key] !== undefined && searchParams[key] !== '') {
                queryParams.append(key, searchParams[key]);
            }
        });

        const url = `/api/properties/search?${queryParams.toString()}`;
        const response = await fetch(url);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const searchResults = await response.json();
        displayProperties(searchResults);
        showSearchSummary(searchParams, searchResults.length);
        showLoadingState(false);
    } catch (error) {
        console.error('Error searching properties:', error);
        showLoadingState(false);
        displayErrorMessage('Failed to search properties. Please try again later.');
    }
}

// Sort properties by price
function sortByPrice(sortType) {
    currentSort = sortType;
    filterAndDisplayProperties();
    
    // Update dropdown button text
    const dropdownButton = document.querySelector('.dropdown .btn');
    if (dropdownButton) {
        const sortText = sortType === 'low-to-high' ? 'Price: Low to High' : 'Price: High to Low';
        dropdownButton.innerHTML = `${sortText} <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m19 9-7 7-7-7"></path></svg>`;
    }
}

// Create a single property card element
function createPropertyCard(property) {
    const cardDiv = document.createElement('div');
    cardDiv.className = 'property-cards bg-white rounded-xl shadow-lg overflow-hidden max-w-xs mx-auto mb-6';
    cardDiv.setAttribute('data-property-id', property.id);
    
    // Handle image fallback
    const imageUrl = property.imageUrl || property.imagePath || './files/images/placeholder.jpg';
    
    cardDiv.innerHTML = `
        <img src="${imageUrl}" alt="${escapeHtml(property.title)}" class="w-full h-40 object-cover bg-gray-200" onerror="this.src='./files/images/placeholder.jpg'; this.onerror=null;">
        <div class="p-4">
            <h3 class="property-title text-lg font-bold text-gray-800 mb-1">${escapeHtml(property.title)}</h3>
            <div class="flex items-center gap-1 mb-2">
                <svg class="w-4 h-4 text-gray-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd"/>
                </svg>
                <p class="property-city text-sm text-gray-600">${escapeHtml(property.city)}</p>
            </div>
            <p class="property-features text-xs text-gray-500 mb-2 line-clamp-2">${escapeHtml(property.description || property.features || 'No description available')}</p>
            
            <!-- Rating -->
            ${property.rating ? `
            <div class="flex items-center gap-1 mb-3">
                <span class="text-yellow-400">⭐</span>
                <span class="text-sm font-medium">${property.rating}</span>
                <span class="text-xs text-gray-500">(${Math.floor(Math.random() * 100) + 10} reviews)</span>
            </div>
            ` : ''}
            
            <!-- Price and Button Row -->
            <div class="flex items-center justify-between">
                <div>
                    <p class="property-price text-xl font-bold text-[#04aa6d]">$${property.price.toFixed(2)}</p>
                    <span class="text-xs text-gray-500">per night</span>
                </div>
                <button class="btn btn-outline btn-sm rounded-lg hover:bg-[#04aa6d] hover:text-white hover:border-[#04aa6d] transition-colors" onclick="viewPropertyDetails(${property.id})">View Details</button>
            </div>
            
            <!-- Availability info -->
            <div class="mt-3 pt-3 border-t border-gray-100">
                <p class="text-xs text-gray-500">
                    Available: ${formatDate(property.availableFrom)} - ${formatDate(property.availableTo)}
                </p>
            </div>
        </div>
    `;
    
    return cardDiv;
}

// Display properties in the container
function displayProperties(properties) {
    const container = document.querySelector('.property-card-container .flex-1');
    if (!container) {
        console.error('Property container not found');
        return;
    }

    // Find or create the grid container
    let gridContainer = container.querySelector('.property-grid');
    if (!gridContainer) {
        // Remove existing property cards
        const existingCards = container.querySelectorAll('.property-cards');
        existingCards.forEach(card => card.remove());
        
        // Create grid container
        gridContainer = document.createElement('div');
        gridContainer.className = 'property-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6';
        
        // Insert after the title
        const title = container.querySelector('p');
        if (title && title.nextSibling) {
            container.insertBefore(gridContainer, title.nextSibling);
        } else {
            container.appendChild(gridContainer);
        }
    } else {
        // Clear existing cards
        gridContainer.innerHTML = '';
    }

    // Add property cards
    if (properties.length === 0) {
        displayNoResultsMessage(gridContainer);
    } else {
        properties.forEach(property => {
            const card = createPropertyCard(property);
            gridContainer.appendChild(card);
        });
    }

    // Update results count
    updateResultsCount(properties.length);
}

// Display no results message
function displayNoResultsMessage(container) {
    const noResultsDiv = document.createElement('div');
    noResultsDiv.className = 'col-span-full text-center py-12';
    noResultsDiv.innerHTML = `
        <div class="text-gray-500">
            <svg class="w-16 h-16 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1" d="M9.172 16.172a4 4 0 015.656 0M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
            </svg>
            <h3 class="text-lg font-medium mb-2">No properties found</h3>
            <p class="text-sm">Try adjusting your filters or search criteria.</p>
            <button onclick="clearAllFilters()" class="btn bg-[#04aa6d] text-white mt-4 hover:bg-[#038a5a] border-none">
                Clear Filters
            </button>
        </div>
    `;
    container.appendChild(noResultsDiv);
}

// Display error message
function displayErrorMessage(message) {
    const container = document.querySelector('.property-card-container .flex-1');
    if (!container) return;

    const errorDiv = document.createElement('div');
    errorDiv.className = 'text-center py-12';
    errorDiv.innerHTML = `
        <div class="bg-red-50 border border-red-200 rounded-lg p-6 max-w-md mx-auto">
            <div class="text-red-600">
                <svg class="w-12 h-12 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 14.5c-.77.833.192 2.5 1.732 2.5z"></path>
                </svg>
                <h3 class="text-lg font-medium mb-2">Error Loading Properties</h3>
                <p class="text-sm">${escapeHtml(message)}</p>
                <button onclick="loadProperties()" class="mt-4 btn btn-sm btn-outline btn-error">Try Again</button>
            </div>
        </div>
    `;
    
    // Clear existing content and show error
    const gridContainer = container.querySelector('.property-grid');
    if (gridContainer) {
        gridContainer.innerHTML = '';
        gridContainer.appendChild(errorDiv);
    }
}

// Show loading state
function showLoadingState(show) {
    const container = document.querySelector('.property-card-container .flex-1');
    if (!container) return;

    let loadingDiv = container.querySelector('.loading-state');
    
    if (show && !loadingDiv) {
        loadingDiv = document.createElement('div');
        loadingDiv.className = 'loading-state text-center py-12';
        loadingDiv.innerHTML = `
            <div class="flex flex-col items-center justify-center">
                <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[#04aa6d] mb-4"></div>
                <p class="text-gray-600">Loading properties...</p>
            </div>
        `;
        
        const gridContainer = container.querySelector('.property-grid');
        if (gridContainer) {
            gridContainer.innerHTML = '';
            gridContainer.appendChild(loadingDiv);
        }
    } else if (!show && loadingDiv) {
        loadingDiv.remove();
    }
}

// Update results count
function updateResultsCount(count) {
    const titleElement = document.querySelector('.property-card-container p[class*="text-"]');
    if (titleElement) {
        const baseText = 'Popular Properties Nearby';
        titleElement.textContent = count > 0 ? `${baseText} (${count} found)` : baseText;
    }
}

// Handle property details view
async function viewPropertyDetails(propertyId) {
    try {
        // Show property details modal
        const property = allProperties.find(p => p.id === propertyId);
        if (!property) {
            // Fetch from API if not found in local array
            const response = await fetch(`/api/properties/${propertyId}`);
            if (response.ok) {
                property = await response.json();
            } else {
                throw new Error('Property not found');
            }
        }
        
        if (property) {
            showPropertyModal(property);
        }
    } catch (error) {
        console.error('Error loading property details:', error);
        alert('Error loading property details. Please try again.');
    }
}

// Show property modal
function showPropertyModal(property) {
    const modal = document.getElementById('property-modal');
    const modalContent = document.getElementById('property-modal-content');
    
    if (modal && modalContent) {
        const imageUrl = property.imageUrl || property.imagePath || './files/images/placeholder.jpg';
        
        modalContent.innerHTML = `
            <img src="${imageUrl}" alt="${escapeHtml(property.title)}" class="w-full h-64 object-cover rounded-lg mb-4" onerror="this.src='./files/images/placeholder.jpg'; this.onerror=null;">
            <h3 class="text-2xl font-bold text-gray-800 mb-2">${escapeHtml(property.title)}</h3>
            <div class="flex items-center gap-1 mb-4">
                <svg class="w-5 h-5 text-gray-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd"/>
                </svg>
                <p class="text-gray-600">${escapeHtml(property.city)}, ${escapeHtml(property.location)}</p>
            </div>
            
            <div class="grid grid-cols-2 gap-6 mb-6">
                <div class="space-y-2">
                    <h4 class="font-semibold text-gray-800">Property Details</h4>
                    <ul class="text-sm text-gray-600 space-y-1">
                        <li>• Type: ${escapeHtml(property.propertyType)}</li>
                        <li>• Rating: ${property.rating ? `⭐ ${property.rating}/5` : 'Not rated'}</li>
                        <li>• Price: $${property.price.toFixed(2)} per night</li>
                    </ul>
                </div>
                
                <div class="space-y-2">
                    <h4 class="font-semibold text-gray-800">Availability</h4>
                    <div class="text-sm text-gray-600 space-y-1">
                        <p>From: ${formatDate(property.availableFrom)}</p>
                        <p>To: ${formatDate(property.availableTo)}</p>
                    </div>
                </div>
            </div>
            
            <div class="mb-6">
                <h4 class="font-semibold text-gray-800 mb-2">Description</h4>
                <p class="text-gray-600 text-sm leading-relaxed">${escapeHtml(property.description || 'No description available')}</p>
            </div>
            
            <div class="flex items-center justify-between">
                <p class="text-3xl font-bold text-[#04aa6d]">$${property.price.toFixed(2)}<span class="text-sm font-normal text-gray-500"> per night</span></p>
                <button class="btn bg-[#04aa6d] text-white hover:bg-[#038a5a] border-none" onclick="bookProperty(${property.id})">Book Now</button>
            </div>
        `;
        
        modal.showModal();
    }
}

// Book property function
async function bookProperty(propertyId) {
    try {
        // For demo purposes, create a booking with default values
        const bookingData = {
            propertyId: propertyId,
            checkIn: new Date(Date.now() + 86400000).toISOString().split('T')[0], // Tomorrow
            checkOut: new Date(Date.now() + 3 * 86400000).toISOString().split('T')[0], // 3 days later
            guests: 2,
            bookingNotes: "Booking created from web interface"
        };

        const response = await fetch('/api/bookings', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(bookingData)
        });

        if (response.ok) {
            const booking = await response.json();
            alert(`Booking successful! Your booking reference is: ${booking.bookingReference}`);
            
            // Close modal
            const modal = document.getElementById('property-modal');
            if (modal) {
                modal.close();
            }
        } else {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Booking failed');
        }
    } catch (error) {
        console.error('Error booking property:', error);
        alert(`Booking failed: ${error.message}`);
    }
}

// Search functionality
function setupSearchFunctionality() {
    const searchInput = document.querySelector('#search-bar input[type="search"]');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const searchTerm = this.value.toLowerCase().trim();
                if (searchTerm) {
                    performAdvancedSearch({ destination: searchTerm });
                } else {
                    loadProperties();
                }
            }, 300); // Debounce search
        });
    }

    // Main search form functionality
    const findButton = document.querySelector('.bg-\\[\\#04aa6d\\].text-white.px-6.py-3.rounded-full');
    if (findButton) {
        findButton.addEventListener('click', function(e) {
            e.preventDefault();
            handleMainSearch();
        });
    }
}

// Handle main search form
function handleMainSearch() {
    const destination = document.querySelector('input[placeholder="Search destinations"]')?.value || '';
    const checkIn = document.querySelector('input[type="date"]:first-of-type')?.value || '';
    const checkOut = document.querySelector('input[type="date"]:last-of-type')?.value || '';
    
    const searchParams = {};
    
    if (destination) searchParams.destination = destination;
    if (checkIn) searchParams.checkIn = checkIn;
    if (checkOut) searchParams.checkOut = checkOut;
    
    if (Object.keys(searchParams).length > 0) {
        performAdvancedSearch(searchParams);
    } else {
        loadProperties();
    }
}

// Setup filter functionality
function setupFilterFunctionality() {
    // Apply filters button
    const applyButton = document.querySelector('button[onclick="applyFilters()"]');
    if (applyButton) {
        applyButton.addEventListener('click', applyFilters);
    }

    // Reset filters button
    const resetButton = document.querySelector('button[onclick="resetFilters()"]');
    if (resetButton) {
        resetButton.addEventListener('click', resetFilters);
    }
}

// Apply filters function
function applyFilters() {
    const minPrice = parseFloat(document.getElementById('minPrice')?.value) || 0;
    const maxPrice = parseFloat(document.getElementById('maxPrice')?.value) || 10000;
    const minRating = parseFloat(document.getElementById('minRating')?.value) || 0;
    const maxRating = parseFloat(document.getElementById('maxRating')?.value) || 5;
    
    const searchParams = {
        minPrice: minPrice,
        maxPrice: maxPrice,
        minRating: minRating,
        maxRating: maxRating
    };
    
    // Add current filter type
    if (currentFilter && currentFilter !== 'all') {
        searchParams.type = currentFilter;
    }
    
    performAdvancedSearch(searchParams);
}

// Reset filters function
function resetFilters() {
    // Reset all filter inputs
    if (document.getElementById('minPrice')) document.getElementById('minPrice').value = '0';
    if (document.getElementById('maxPrice')) document.getElementById('maxPrice').value = '1000';
    if (document.getElementById('minRating')) document.getElementById('minRating').value = '1';
    if (document.getElementById('maxRating')) document.getElementById('maxRating').value = '5';
    if (document.getElementById('priceRange')) document.getElementById('priceRange').value = '1000';
    if (document.getElementById('ratingRange')) document.getElementById('ratingRange').value = '5';
    
    // Reset checkboxes
    document.querySelectorAll('.checkbox').forEach(checkbox => {
        checkbox.checked = false;
    });
    
    // Reset to all properties
    currentFilter = 'all';
    currentSort = '';
    
    // Reset tab selection
    const firstTab = document.querySelector('input[name="my_tabs_1"][aria-label="All"]');
    if (firstTab) {
        firstTab.checked = true;
    }
    
    loadProperties();
}

// Clear all filters
function clearAllFilters() {
    resetFilters();
}

// Show search summary
function showSearchSummary(searchParams, resultCount) {
    let summaryDiv = document.getElementById('search-summary');
    
    if (!summaryDiv) {
        summaryDiv = document.createElement('div');
        summaryDiv.id = 'search-summary';
        summaryDiv.className = 'bg-blue-50 border-l-4 border-blue-400 p-4 mb-6 mx-6';
        
        const container = document.querySelector('.property-card-container .flex-1');
        const title = container.querySelector('p');
        if (title && title.parentNode) {
            title.parentNode.insertBefore(summaryDiv, title.nextSibling);
        }
    }
    
    let summaryText = `Found ${resultCount} properties`;
    if (searchParams.destination) summaryText += ` in "${searchParams.destination}"`;
    if (searchParams.checkIn && searchParams.checkOut) {
        summaryText += ` from ${searchParams.checkIn} to ${searchParams.checkOut}`;
    }
    
    summaryDiv.innerHTML = `
        <div class="flex items-center justify-between">
            <div>
                <p class="text-blue-800 font-medium">${summaryText}</p>
            </div>
            <button onclick="clearSearchSummary()" class="text-blue-600 hover:text-blue-800 text-sm">
                Clear filters
            </button>
        </div>
    `;
}

// Clear search summary
function clearSearchSummary() {
    const summaryDiv = document.getElementById('search-summary');
    if (summaryDiv) {
        summaryDiv.remove();
    }
    loadProperties();
}

// Helper functions
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric' 
    });
}

// Utility function to escape HTML to prevent XSS
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Refresh properties (useful for manual refresh)
function refreshProperties() {
    currentFilter = 'all';
    currentSort = '';
    
    // Reset tab selection
    const firstTab = document.querySelector('input[name="my_tabs_1"][aria-label="All"]');
    if (firstTab) {
        firstTab.checked = true;
    }
    
    // Reset sort dropdown
    const dropdownButton = document.querySelector('.dropdown .btn');
    if (dropdownButton) {
        dropdownButton.innerHTML = `Sort by Price <svg class="w-4 h-4 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m19 9-7 7-7-7"></path></svg>`;
    }
    
    loadProperties();
}

// Price range update functions
function updatePriceInput(val) { 
    const maxPriceInput = document.getElementById('maxPrice');
    if (maxPriceInput) {
        maxPriceInput.value = val;
    }
}

function updateRatingInput(val) { 
    const maxRatingInput = document.getElementById('maxRating');
    if (maxRatingInput) {
        maxRatingInput.value = val;
    }
}