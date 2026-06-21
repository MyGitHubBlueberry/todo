# Todo App
A full-stack Todo application.

## Requirenments:
1. Creating, Viewing, Editing, Deleting tasks
2. Adding Categories for tasks
3. Log in/log out
4. Pagination for the list of tasks
5. Implement searching and filtering by categories
## Technologies:
1. Back - REST API on .NET Core with any relational DB (ex., MS SQL)
2. Use 4 levels architecture: controllers, services, interfaces and data access
3. Use EF Core, Dependency Injection
4. Front - Angular (use bootstrap or tailwind)
---
## Prerequisites

Before you begin, ensure you have the following installed on your machine:
* **Node.js** (v18+) and **npm**
* **.NET 9.0 SDK** 
* **MySQL DB**
---

## Backend Setup (.NET API)

The backend is a C# .NET Web API utilizing Entity Framework Core for database management.

### 1. Database Configuration
The application expects a database named `todoDB` with a user `user`. 

To safely store your local database credentials and JWT secret without committing them to source control, use the .NET `user-secrets` tool. 

Navigate to the API directory:
```bash
cd Server/Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=todoDB;Uid=user;Pwd=YOUR_LOCAL_DB_PASSWORD;"
dotnet user-secrets set "JwtSettings:SecretKey" "YOUR_SUPER_SECRET_LONG_RANDOM_STRING_HERE"
```
### 2. Apply Migrations
Create the database tables by applying the existing Entity Framework migrations:

```bash
dotnet ef database update --project ../Data/Data.csproj
```

### 3. Start
Start the backend with:
```bash
dotnet run
```
## Frontend Setup (Angular)

### 1. Install Dependencies
  Open a new terminal window, ensure you are in the root directory of the repository (where package.json is located), and install the required Node packages:
```bash
npm install
```
### 2. Environment Configuration
Create a `.env` file in the root of your project (right next to `package.json`) to define your local backend URL. This keeps your API endpoints dynamic and prevents them from being hardcoded.
```
NG_APP_API_URL=http://localhost:5000/api
```
### 3. Run the Development Server
Start the Angular CLI server:
```bash
npx ng start
```
Navigate to `http://localhost:4200/` in your browser. The application will automatically reload if you change any of the source files.

### 4. Build for Production
To compile the application into a production-ready build:
```bash
ngx ng build
```
The compiled files will be generated in the `dist/` directory.
## Db design
![db schema](./public/db.png)
