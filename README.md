TechMove-GLMS
TechMove Global Logistics Management System (GLMS)

Overview

A full-stack enterprise web application for logistics management, built with ASP.NET Core MVC and Entity Framework Core. The system manages clients, contracts/agreements, and service requests with real-time currency conversion and document management.

Technologies Used

ASP.NET Core MVC (.NET 10.0)
SQL Server LocalDB
Entity Framework Core 10.0
xUnit Testing
Bootstrap 5
ExchangeRate-API v4
Features Implemented

Database and Models
SQL Server database with Entity Framework Core
Three main entities: Client, Agreement, ServiceRequest
One-to-many relationships with proper foreign keys
Database migrations included
File Handling
PDF file upload for signed agreements
File validation (only .pdf allowed)
Rejects .exe, .jpg, .png, .docx
PDF download functionality
Currency API Integration
Real-time USD to ZAR exchange rate
Auto-calculation when entering USD amount
Fallback rate (19.50 ZAR) if API unavailable
Displays exchange rate used per request
Workflow Logic
Service requests require ACTIVE agreement
Prevents requests with Draft/Expired/OnHold agreements
Validation error messages displayed
LINQ Search and Filter
Filter service requests by date range
Filter by status (Pending, Approved, InProgress, Completed)
Dynamic query building
Unit Testing (xUnit)
32 passing unit tests
Currency calculation tests
File validation tests
Workflow validation tests
Model state tests
Modern UI Theme
Navy blue and gold professional theme
Responsive design with Bootstrap
Dashboard with live statistics
Clean, modern card-based layout
Smooth hover animations
Setup Instructions

Prerequisites

Visual Studio 2022/2026
.NET 10.0 SDK
SQL Server LocalDB
Clone Repository

git clone https://github.com/YOUR_USERNAME/TechMove-GLMS.git cd TechMove-GLMS

Database Migration

dotnet ef migrations add InitialCreate --project TechMove.GLMS.Web dotnet ef database update --project TechMove.GLMS.Web

Run Application

dotnet run --project TechMove.GLMS.Web

Run Tests

dotnet test TechMove.GLMS.Tests

Test Results

Test run finished: 32 Tests (32 Passed, 0 Failed, 0 Skipped)

CurrencyCalculationTests: 5 passed
FileValidationTests: 7 passed
WorkflowValidationTests: 5 passed
ServiceRequestModelTests: 15 passed
Key Business Rules

Service Request Creation requires Active Agreement
PDF Upload only accepts .pdf files
Currency Conversion auto-calculates USD to ZAR
Agreement Status flow: Draft -> Active -> Expired/OnHold
Author

Lebohang Chabangu

License

Academic Project - For assignment purposes only

Acknowledgments

ExchangeRate-API for free currency data
Bootstrap for UI components
xUnit for testing framework
