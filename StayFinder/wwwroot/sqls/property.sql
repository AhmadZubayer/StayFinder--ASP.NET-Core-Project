CREATE DATABASE StayFinderDB;
GO

USE StayFinderDB;
GO

CREATE TABLE properties (
    property_id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(200) NOT NULL,
    location NVARCHAR(150) NOT NULL,
    city NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX),
    type NVARCHAR(50) NOT NULL CHECK (type IN ('apartment', 'villa', 'room')),
    rating DECIMAL(2,1),
    price DECIMAL(10,2) NOT NULL,
    available_from DATE NOT NULL,
    available_to DATE NOT NULL,
    image_path NVARCHAR(500)  -- stores local path or URL
);

SELECT * FROM properties;

INSERT INTO properties 
(title, location, city, description, type, rating, price, available_from, available_to, image_path)
VALUES
('Cozy Apartment in City Center', '123 Main St', 'Dhaka', 'Modern apartment close to shopping and transport.', 'apartment', 4.5, 75.00, '2025-09-10', '2025-12-31', 'files/images/1.jpg'),

('Luxury Villa with Pool', 'Beach Road 45', 'Coxs Bazar', 'Spacious villa with private pool and ocean view.', 'villa', 4.8, 250.00, '2025-09-15', '2026-01-15', 'files/images/2.jpg'),

('Budget Room near University', '45 Student Ln', 'Chittagong', 'Affordable single room, perfect for students.', 'room', 3.9, 25.00, '2025-09-01', '2025-12-31', 'files/images/3.jpg'),

('Modern Apartment with Balcony', '78 Green Ave', 'Dhaka', 'Stylish apartment with balcony and great city view.', 'apartment', 4.2, 90.00, '2025-09-05', '2025-11-30', 'files/images/4.jpg'),

('Traditional Villa Retreat', 'Hilltop Road', 'Sylhet', 'Quiet villa surrounded by tea gardens and hills.', 'villa', 4.7, 180.00, '2025-09-20', '2026-02-28', 'files/images/5.jpg'),

('Compact Room for Travelers', 'Airport Rd 12', 'Khulna', 'Simple and clean room, close to transport.', 'room', 3.8, 30.00, '2025-09-01', '2025-12-15', 'files/images/6.jpg');

