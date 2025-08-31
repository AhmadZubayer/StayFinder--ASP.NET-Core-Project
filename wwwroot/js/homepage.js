class StayFinderAPI {
    constructor() {
        // Detect if we're running from Live Server or the .NET API
        const isLiveServer = window.location.port !== '5295';
        this.baseUrl = isLiveServer ? 'http://localhost:5295/api' : '/api';
        
        console.log('API Base URL:', this.baseUrl);
        console.log('Current origin:', window.location.origin);
    }

    async getFeaturedProperties() {
        try {
            const response = await fetch(`${this.baseUrl}/properties/featured`);
            if (!response.ok) throw new Error(`Failed to fetch properties: ${response.status}`);
            return await response.json();
        } catch (error) {
            console.error('Error fetching featured properties:', error);
            return [];
        }
    }

    async searchProperties(searchParams) {
        try {
            const queryString = new URLSearchParams(searchParams).toString();
            const response = await fetch(`${this.baseUrl}/properties?${queryString}`);
            if (!response.ok) throw new Error('Failed to search properties');
            return await response.json();
        } catch (error) {
            console.error('Error searching properties:', error);
            return { Properties: [], TotalCount: 0 };
        }
    }

    async getPropertyDetails(id) {
        try {
            const response = await fetch(`${this.baseUrl}/properties/${id}`);
            if (!response.ok) throw new Error('Failed to fetch property details');
            return await response.json();
        } catch (error) {
            console.error('Error fetching property details:', error);
            return null;
        }
    }
}

class HomePage {
    constructor() {
        this.api = new StayFinderAPI();
        this.currentPage = 1;
        this.isLoading = false;
        this.init();
    }

    async init() {
        await this.loadFeaturedProperties();
        this.setupEventListeners();
    }

    async loadFeaturedProperties() {
        this.showLoading();
        const properties = await this.api.getFeaturedProperties();
        this.renderProperties(properties);
        this.hideLoading();
    }

    renderProperties(properties) {
        const container = document.querySelector('.featured-properties-grid');
        if (!container) {
            console.error('Featured properties grid container not found!');
            return;
        }

        if (!properties || properties.length === 0) {
            container.innerHTML = '<div class="col-span-full text-center py-8"><p class="text-gray-500">No properties available</p></div>';
            return;
        }

        container.innerHTML = properties.map(property => this.createPropertyCard(property)).join('');
    }

    createPropertyCard(property) {
        const badgeHtml = property.badgeText ? 
            `<div class="absolute top-3 left-3">
                <span class="badge ${property.badgeClass}">${property.badgeText}</span>
            </div>` : '';

        const amenitiesText = property.amenities.slice(0, 3).join(' • ');

        return `
            <div class="card bg-base-100 shadow-lg hover:shadow-xl transition-shadow duration-300">
                <figure class="relative">
                    <img src="${property.primaryImageUrl}" alt="${property.title}" class="h-64 w-full object-cover" />
                    <div class="absolute top-3 right-3">
                        <button class="btn btn-circle btn-sm bg-base-100/80 border-none hover:bg-base-100 heart-btn" data-property-id="${property.id}">
                            <i class="far fa-heart"></i>
                        </button>
                    </div>
                    ${badgeHtml}
                </figure>
                <div class="card-body p-4">
                    <div class="flex justify-between items-start mb-2">
                        <h3 class="card-title text-lg">${property.title}</h3>
                        <div class="flex items-center gap-1">
                            <i class="fas fa-star text-yellow-400 text-sm"></i>
                            <span class="text-sm font-medium">${property.rating}</span>
                        </div>
                    </div>
                    <p class="text-gray-600 text-sm mb-1">${property.location}</p>
                    <p class="text-gray-500 text-xs mb-3">${amenitiesText}</p>
                    <div class="flex justify-between items-center">
                        <div>
                            <span class="text-lg font-bold">$${property.pricePerNight}</span>
                            <span class="text-gray-500 text-sm">/ night</span>
                        </div>
                        <button class="btn btn-primary btn-sm view-details-btn" data-property-id="${property.id}">
                            View Details
                        </button>
                    </div>
                </div>
            </div>
        `;
    }

    setupEventListeners() {
        // Search form submission
        const searchForm = document.querySelector('.search-form');
        if (searchForm) {
            searchForm.addEventListener('submit', this.handleSearch.bind(this));
        }

        // View details buttons
        document.addEventListener('click', (e) => {
            if (e.target.closest('.view-details-btn')) {
                const propertyId = e.target.closest('.view-details-btn').dataset.propertyId;
                this.viewPropertyDetails(propertyId);
            }
        });

        // Heart/favorite buttons
        document.addEventListener('click', (e) => {
            if (e.target.closest('.heart-btn')) {
                this.toggleFavorite(e.target.closest('.heart-btn'));
            }
        });

        // Show more button
        const showMoreBtn = document.querySelector('.show-more-btn');
        if (showMoreBtn) {
            showMoreBtn.addEventListener('click', this.loadMoreProperties.bind(this));
        }
    }

    async handleSearch(e) {
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const searchParams = {
            location: formData.get('location') || '',
            checkin: formData.get('checkin') || '',
            checkout: formData.get('checkout') || '',
            guests: formData.get('guests') || ''
        };

        // Store search params for potential navigation to results page
        sessionStorage.setItem('searchParams', JSON.stringify(searchParams));
        
        // For now, just filter current properties
        this.showLoading();
        const result = await this.api.searchProperties(searchParams);
        this.renderProperties(result.Properties || []);
        this.hideLoading();
    }

    async viewPropertyDetails(propertyId) {
        // Store the property ID and navigate to details page
        sessionStorage.setItem('selectedPropertyId', propertyId);
        
        // For now, just show an alert
        const property = await this.api.getPropertyDetails(propertyId);
        if (property) {
            alert(`Viewing details for: ${property.Title}\nLocation: ${property.Location}\nPrice: $${property.PricePerNight}/night`);
            // TODO: Navigate to property details page
            // window.location.href = `/property-details.html?id=${propertyId}`;
        }
    }

    toggleFavorite(button) {
        const icon = button.querySelector('i');
        const isFavorited = icon.classList.contains('fas');
        
        if (isFavorited) {
            icon.classList.remove('fas', 'text-red-500');
            icon.classList.add('far');
        } else {
            icon.classList.remove('far');
            icon.classList.add('fas', 'text-red-500');
        }

        // TODO: Send to backend to save favorite
        const propertyId = button.dataset.propertyId;
        console.log(`Toggled favorite for property ${propertyId}`);
    }

    async loadMoreProperties() {
        if (this.isLoading) return;
        
        this.currentPage++;
        this.isLoading = true;
        
        const result = await this.api.searchProperties({ page: this.currentPage });
        const newProperties = result.Properties || [];
        
        if (newProperties.length > 0) {
            const container = document.querySelector('.featured-properties-grid');
            container.innerHTML += newProperties.map(property => this.createPropertyCard(property)).join('');
        }
        
        this.isLoading = false;
        
        // Hide button if no more properties
        if (this.currentPage >= (result.TotalPages || 1)) {
            document.querySelector('.show-more-btn').style.display = 'none';
        }
    }

    showLoading() {
        const container = document.querySelector('.featured-properties-grid');
        if (container) {
            container.innerHTML = '<div class="col-span-full text-center py-8"><span class="loading loading-spinner loading-lg"></span></div>';
        }
    }

    hideLoading() {
        // Loading will be hidden when properties are rendered
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new HomePage();
});