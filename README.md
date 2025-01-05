# DexefTask 

## Overview
DexefTask consists of several components, including the API, Data Access, Business Logic, and their respective tests. This README provides instructions on how to set up and run the project.

## Prerequisites
Before you begin, ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (version compatible with the project)
- [Visual Studio](https://visualstudio.microsoft.com/) or any preferred IDE
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (for database access)

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/AbdallahElshenawy/DexefTask.git
   
   cd DexefTask

2. Restore dependencies for each project:
    ```bash
    cd DexefTask.API
    
    dotnet restore
    
    cd ../DexefTask.DataAccess
    
    dotnet restore
    
    cd ../DexefTask.BusinessLogic
    
    dotnet restore
    
    cd ../DexefTask.API.IntegrationTests
    
    dotnet restore
    
    cd ../DexefTask.BusinessLogic.Tests
    
    dotnet restore

3. Update the database: After restoring dependencies, update the database to match the application's requirements:
   ```bash
    cd ../DexefTask.API

    dotnet ef database update

4. Run the project:
   ```bash
    To run the application:
    
    cd ../DexefTask.API
    
    dotnet run

    To run tests:
    
    cd ../DexefTask.BusinessLogic.Tests
    
    dotnet test

## API Endpoints

### AuthController
- **POST** `/api/auth/register`
  - Registers a new user in the system.
  
- **POST** `/api/auth/login`
  - Logs in an existing user and returns a token.
  - **Example Credentials To Test The Endpoints**:
    - **User**: 
     "email": "string@yahoo.com",
     "password":"string123@aA"

    - **Admin**: 
      "email": "admin@gmail.com",
      "password": "123456@aA"

### BooksController
- **GET** `/api/books`
  - Retrieves all books from the system (requires Admin or User role).
  
- **POST** `/api/books`
  - Adds a new book to the system (requires Admin role).
  
- **GET** `/api/books/{id}`
  - Retrieves a book by its unique identifier (requires Admin or User role).
  
- **DELETE** `/api/books/{id}`
  - Deletes a book by its unique identifier (requires Admin role).
  
- **PUT** `/api/books/{id}`
  - Updates the information of an existing book (requires Admin role).

### BorrowController (requires User role)
- **POST** `/api/borrow/{bookId}`
  - Allows a user to borrow a book.
  
- **GET** `/api/borrow`
  - Retrieves all books that the logged-in user has borrowed.

