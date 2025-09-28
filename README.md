# AITutor_EXE101

MyEnglishTutor - AI English Tutor
📋 Introduction
MyEnglishTutor is a web application built with ASP.NET Core MVC and Razor Pages, designed to provide a comprehensive learning platform for Speaking and Listening skills using artificial intelligence (AI).

This project helps users improve their English communication skills effectively and in a personalized way, with key features including:

🗣️ AI-powered speaking practice.

👂 English podcast listening.

🚀 Instant analysis and feedback.

🏗️ Project Architecture
The project is organized using an ASP.NET Core MVC architecture, with a folder structure similar to the one provided in the image, ensuring clear separation of concerns and maintainability.

Layer Details:
Controllers: Contains the controllers that handle user requests and return views.

Views: Contains Razor Pages (.cshtml) files, which define the user interface (UI).

Models: Holds data models (entities) and view models.

Services: Contains complex business logic, separated from the controllers.

Repositories: Manages interactions with the database (Database access).

Utils/Helpers: Includes utility classes and helper functions.

wwwroot: Stores static files like CSS, JavaScript, and images.

🛠️ Technologies Used
Framework: ASP.NET Core 8.0 MVC with Razor Pages

Database: SQL Server with Entity Framework Core 8.0

Authentication: Cookie Authentication with Refresh Token

AI/ML: External AI APIs for Speech-to-Text (STT) and pronunciation evaluation.

Podcast: Integrates RSS feeds or APIs from popular podcast sources.

Validation: Data Annotations or FluentValidation

Logging: ASP.NET Core built-in logging

Key Packages:
Microsoft.AspNetCore.Authentication.Cookies - Cookie Authentication

Microsoft.EntityFrameworkCore - Entity Framework Core

Newtonsoft.Json - JSON Serialization

⚡ Key Features
🔐 User Management
User registration and login.

Cookie Authentication for session management.

Refresh Token integration for secure session renewal.

Personal information management.

🗣️ AI Speaking Practice
Users can record their speech and get instant feedback on pronunciation and intonation.

Detailed analysis for individual sentences or words.

Personalized exercises based on the user's proficiency level.

👂 Podcast Listening
Listen to English podcasts from various topics.

Integrated subtitle features and vocabulary lookup.

Bookmark favorite podcast sections for later review.

📊 Progress Tracking
Detailed charts and statistics on learning progress.

History of all practice sessions.

🚀 Installation and Running the Project
System Requirements:
.NET 8.0 SDK

SQL Server (or SQL Server Express)

Visual Studio 2022+ or VS Code

Installation Steps:
Clone the repository

Bash

git clone <repository-url>
cd MyEnglishTutor
Restore packages

Bash

dotnet restore
Configure the database
Update the connection string in appsettings.json. The project uses a DB-first approach, so make sure the database schema is already in place.

JSON

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MyEnglishTutor;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
Configure AI APIs (optional)
Update the API keys in appsettings.json:

JSON

"AIApiSettings": {
  "SpeechToTextKey": "your-stt-api-key",
  "PronunciationEvaluationKey": "your-pronunciation-api-key"
}
Run the application

Bash

dotnet run
Alternatively, use Visual Studio: Set the MyEnglishTutor project as the startup project and press F5.

📚 API Documentation
The project is built on Razor Pages and MVC, so there are no public API endpoints documented with Swagger. All functionalities are accessible through the web UI.

Login/Register: Access /Identity/Account/Login or /Identity/Account/Register.

Homepage: After logging in, you will be redirected to the dashboard.

Speaking Practice: Access /Speech.

Podcast Listening: Access /Podcast.

🤝 Contributing
We welcome contributions! Please follow these steps to contribute:

Fork the project.

Create your feature branch (git checkout -b feature/NewFeature).

Commit your changes (git commit -m 'Add some NewFeature').

Push to the branch (git push origin feature/NewFeature).

Open a Pull Request.

📄 License
The project is distributed under the MIT License. See LICENSE for more information.
