# Hospital Management System (.NET 8)

This project is a migration of the original ASP.NET Web Forms Hospital Management System to a modern .NET 8 application using Razor Pages.

## Project Structure

The solution follows a clean architecture approach with the following projects:

1. **HospitalManagement.Domain**
   - Contains all domain entities and interfaces
   - Business logic is encapsulated in domain services
   - Repository interfaces for data access abstraction

2. **HospitalManagement.Infrastructure**
   - Entity Framework Core implementation
   - Database context and migrations
   - Repository implementations
   - Unit of Work pattern implementation

3. **HospitalManagement.Web**
   - ASP.NET Core Razor Pages frontend
   - Authentication and authorization
   - User interfaces for patients, doctors, and staff

## Key Features

- **Patient Management**
  - Patient registration and profile management
  - Appointment booking and management
  - Treatment history and records
  - Bill payment

- **Doctor Management**
  - Doctor profile and schedule management
  - Appointment approval and management
  - Treatment recording and prescription management
  - Patient treatment history

- **Administrative Functions**
  - Department management
  - Staff management
  - User management

## Technology Stack

- **Framework**: .NET 8
- **UI**: ASP.NET Core Razor Pages
- **Database Access**: Entity Framework Core 8.0
- **Authentication**: Cookie-based authentication
- **CSS Framework**: Bootstrap 5

## Database Schema

The application uses the following main entities:

- User (LoginTable)
- Patient
- Doctor
- Department
- Appointment
- OtherStaff

## Migration Notes

### What's Changed

1. **Architecture**
   - Moved from ASP.NET Web Forms to ASP.NET Core Razor Pages
   - Implemented clean architecture pattern with separate projects for Domain, Infrastructure, and Web

2. **Data Access**
   - Replaced direct ADO.NET with Entity Framework Core
   - Implemented Repository and Unit of Work patterns
   - Code-first approach with fluent configurations

3. **Authentication**
   - Replaced Forms Authentication with ASP.NET Core Identity
   - Implemented role-based authorization policies

4. **User Experience**
   - Modern, responsive design with Bootstrap 5
   - Improved navigation and user flows

### Future Improvements

1. **Testing**
   - Add unit tests for repositories and services
   - Add integration tests for key workflows

2. **Features**
   - Add advanced reporting capabilities
   - Implement email notifications
   - Add file upload for medical records

## Setup Instructions

1. Clone the repository
2. Ensure .NET 8 SDK is installed
3. Run database migrations:

```bash
dotnet ef database update --project HospitalManagement.Infrastructure --startup-project HospitalManagement.Web
```

4. Run the application:

```bash
dotnet run --project HospitalManagement.Web
```

## Default Users

The application will be seeded with the following default users:

- **Admin**: plm@gmail.com / abcdef

## License

This project is licensed under the MIT License.
