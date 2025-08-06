# Smartkeeda Test - Project Management API

This is a sample ASP.NET Core Web API developed as part of the skill test for the ASP.Net Core Developer position at **Smartkeeda**.

## 🔧 Tech Stack

- ASP.NET Core Web API (.NET 8)
- ADO.NET for database operations
- MySQL as the database
- Swagger for API testing and documentation

## 📁 Project Structure

├── Controllers/
│ ├── UsersController.cs
│ ├── ProjectsController.cs
│ └── TasksController.cs
├── Models/
├── Repository/
├── StoredProcedures.sql
├── SmartkeedaDb.sql
├── Program.cs


## ⚙️ Features Implemented

- **User Management**: Admin-only user listing
- **Project Management**:
  - Create, Update, Delete (Admin & PM roles)
  - View all and specific project
- **Task Management**:
  - CRUD based on role: Admin, PM, Developer
- **Role-based Authorization** using claims
- **Error Handling** and Validation
- **Stored Procedures** used for all DB operations

## 🔐 Role-based Access

| API Endpoint | Admin | Project Manager | Developer |
|--------------|:-----:|:---------------:|:---------:|
| GET /api/users | ✅ | ❌ | ❌ |
| CRUD /api/projects | ✅ | ✅ (own only) | ❌ |
| CRUD /api/tasks | ✅ | ✅ (assigned) | ✅ (assigned) |

## 🚀 How to Run the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/PranayKalamkar/Smartkeeda-test-Project-management-API
