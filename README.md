# Book Management API

## Overview
**Book Management API** is a RESTful API built with ASP.NET Core Web API that allows you to manage books. The API supports CRUD operations, including adding (single and bulk), updating, and soft deleting books. 
It also provides endpoints for retrieving a list of books (only titles) sorted by popularity with pagination/information about a specific book.

## Features
- **User Authentication:** 
  - Registration and login endpoints using ASP.NET Core Identity.
  - JWT tokens for securing all API endpoints.
- **Book Management:**
  - **Create:** Add a single book or bulk add multiple books.
  - **Read:** 
    - Retrieve detailed information about a book.
    - Retrieve a paginated list of book titles sorted by popularity.
  - **Update:** Update book details.
  - **Delete:** Soft delete single or multiple books.
- **Popularity Calculation:**
  - Each book has a view counter (incremented on each detail retrieval).
  - The popularity score is calculated on the fly (using a formula based on views count and book age with the idea of increasing the popularity of new books by introducing a decreasing function) and is not stored in the database.
- **Validation:**
  - Ensures that a book cannot be added if a book with the same title already exists.
  - Other validations (e.g., non-empty title, valid publication year, valid id) are applied.

  ## Technologies Used
- **Language:** C#
- **Framework:** .NET 8(ASP.NET Core Web API)
- **Database:** SQL Server (using Entity Framework Core)
- **Authentication:** JWT-based authentication using ASP.NET Core Identity
- **Documentation:** Swagger for API documentation

## Architecture
The solution follows a 3-layered architecture:
1. **Core**  
2. **Data**  
3. **API** 

