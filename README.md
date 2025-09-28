# AITutor_EXE101

# MyEnglishTutor - AI English Tutor
## 📋 **Introduction**
**MyEnglishTutor** is a web application built with ASP.NET Core MVC and Razor Pages, designed to provide a comprehensive learning platform for **Speaking** and **Listening** skills using artificial intelligence (AI).

This project helps users improve their English communication skills effectively and in a personalized way, with key features including:
- 🗣️ **AI-powered speaking practice**.
- 👂 **English podcast listening**.
- 🚀 **Instant analysis and feedback**.

---

## 🏗️ **Project Architecture**
The project is organized using an **ASP.NET Core MVC** architecture, with a folder structure similar to the one provided in the image, ensuring clear separation of concerns and maintainability.

### Layer Details:
- **Controllers**: Contains the controllers that handle user requests and return views.
- **Views**: Holds **Razor Pages (.cshtml)** files, which define the user interface (UI).
- **Models**: Holds data models (entities) and view models.
- **Services**: Contains complex business logic, separated from the controllers.
- **Repositories**: Manages interactions with the database (Database access).
- **Utils/Helpers**: Includes utility classes and helper functions.
- **wwwroot**: Stores static files like CSS, JavaScript, and images.

---

## 🛠️ **Technologies Used**
- **Framework**: ASP.NET Core 8.0 MVC with Razor Pages
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: Cookie Authentication with Refresh Token
- **AI/ML**: External AI APIs for Speech-to-Text (STT) and pronunciation evaluation.
- **Podcast**: Integrates RSS feeds or APIs from popular podcast sources.
- **Validation**: Data Annotations or FluentValidation
- **Logging**: ASP.NET Core built-in logging

### Key Packages:
- `Microsoft.AspNetCore.Authentication.Cookies` - Cookie Authentication
- `Microsoft.EntityFrameworkCore` - Entity Framework Core
- `Newtonsoft.Json` - JSON Serialization

---

## ⚡ **Key Features**
### 🔐 **User Management**
- User registration and login.
- **Cookie Authentication** for session management.
- **Refresh Token** integration for secure session renewal.
- Personal information management.

### 🗣️ **AI Speaking Practice**
- Users can record their speech and get instant feedback on pronunciation and intonation.
- Detailed analysis for individual sentences or words.
- Personalized exercises based on the user's proficiency level.

### 👂 **Podcast Listening**
- Listen to English podcasts from various topics.
- Integrated subtitle features and vocabulary lookup.
- Bookmark favorite podcast sections for later review.

### 📊 **Progress Tracking**
- Detailed charts and statistics on learning progress.
- History of all practice sessions.

---

## 🚀 **Installation and Running the Project**
### System Requirements:
- .NET 8.0 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022+ or VS Code

### Installation Steps:
1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MyEnglishTutor


2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Cấu hình database**
   
   Cập nhật connection string trong `API/appsettings.json`:
   ```json
   {
     "Database": {
       "UseInMemoryDatabase": false,
       "ConnectionString": "server=localhost; database=ToeicApp;TrustServerCertificate=True;Trusted_Connection=True;"
     }
   }
   ```

4. **Cấu hình Email (tùy chọn)**
   
   Cập nhật SMTP settings trong `API/appsettings.json`:
   ```json
   {
     "SmtpSettings": {
       "Server": "smtp.gmail.com",
       "Port": 587,
       "SenderName": "ToeicApp",
       "SenderEmail": "your-email@gmail.com",
       "Username": "your-email@gmail.com",
       "Password": "your-app-password"
     }
   }
   ```

