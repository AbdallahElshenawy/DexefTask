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
3. Run the project:
    ```bash
    To run the application:
    
    cd ../DexefTask.API
    
    dotnet run

    To run tests:
    
    cd ../DexefTask.BusinessLogic.Tests
    
    dotnet test