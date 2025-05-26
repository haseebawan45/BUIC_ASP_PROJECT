# Smart Campus Portal - ASP.NET Core MVC Project

A comprehensive web-based system for university automation that simulates various administrative and student services.

## Project Overview

Smart Campus Portal is a centralized platform that handles core campus services and enhances communication and transparency between students and university staff. The system allows different types of users (students, faculty, and administrators) to interact with the portal based on their roles.

## Features

### User Authentication & Role Management
- Students, faculty, and admin users can log in using unique credentials
- Role-based access control using ASP.NET Identity

### Student Portal
- View personal profile and academic record
- Register for courses
- Submit assignments and view grades
- View fee details and payment status

### Faculty Portal
- Upload course content and assignments
- Grade students' submissions
- Send announcements to students
- View attendance reports

### Admin Panel
- Add/remove students and faculty
- Manage course offerings
- Generate reports (fee reports, course registration stats)

## Technology Stack

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Bootstrap 5
- jQuery
- Font Awesome

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or higher)
- Visual Studio 2022 or Visual Studio Code

### Installation

1. Clone the repository
2. Open the solution in Visual Studio
3. Update the connection string in `appsettings.json` if needed
4. Run the application

The database will be automatically created and seeded with initial data on first run.

### Default Admin Login

- Email: admin@campus.edu
- Password: Admin@123

## Project Structure

- **Controllers/**: Contains all the controller classes
- **Models/**: Contains data models and view models
- **Views/**: Contains the Razor views organized by controller
- **wwwroot/**: Contains static files (CSS, JavaScript, images)

## License

This project is licensed under the MIT License. 