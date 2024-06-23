### TourPlanner Project Documentation: Setup and Layered Architecture

#### Development Environment and Tools
- *Development Tools*: Developed in Visual Studio, the project uses the .NET Framework and WPF for user interface development. Entity Framework Core serves as the ORM tool for data interactions, PostgreSQL as the backend database, and Docker for managing database deployment.

#### Architecture Overview
- *MVVM Pattern*:
  - *Models*: Located in the Presentation Layer, these C# classes (Tour, TourLog) define the data structure.
  - *ViewModels*: Bridge the Views and the Business Layer, managing logic and data binding.
  - *Views*: User interface components designed in XAML, displaying data and interfacing with user interactions.

- *Business Layer*: Contains core business logic, leveraging services to process data and manage business rules.

- *Data Layer*: Handles data persistence and retrieval, interfacing with PostgreSQL through Entity Framework Core for database operations.

### Database Setup and Docker Configuration

#### PostgreSQL Configuration in Docker:
- *Docker Setup*:
  1. *Retrieve PostgreSQL Image*:
     
     docker pull postgres
     
  2. *Initialize PostgreSQL Container*:
     
     docker run -d --name postgresdb -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres
     
     This command ensures persistent data storage with Docker volumes.

#### Entity Framework Core Integration:
- *DbContext Management*:
  - *AppDbContext*: Extends DbContext, configuring entity mappings and managing the database connection.
  - *Connection Management*: Utilizes appsettings.json for flexible database connection strings.
  - *EF Core Migrations*: Manages schema changes without data loss, facilitating database version control.

### Configuration and Project Structure

#### Configuration Files:
- *appsettings.json*:
  json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5432;Username=postgres;Password=password;Database=postgresdb;"
    }
  }
  
#### Detailed Structure and Project Components:
- *Presentation Layer*:
  - *Models*: Define the business data entities.
  - *Views*: Layout and visual elements are crafted in XAML.
  - *ViewModels*: Manage data presentation and respond to user inputs, utilizing commands for actions.

- *Business Layer*:
  - Implements logic necessary for data manipulation and business rule enforcement through various services.

- *Data Layer*:
  - Uses Entity Framework Core for database interactions, aligning models to the database schema.

- *TourPlanner.Commands*:
  - *RelayCommand.cs*: Located in the TourPlanner.Commands folder, this file implements the ICommand interface for WPF commands, facilitating interaction between the UI actions and ViewModel methods.
