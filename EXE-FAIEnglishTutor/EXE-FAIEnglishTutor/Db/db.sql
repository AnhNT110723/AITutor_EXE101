CREATE DATABASE FAI_ENGLISH
GO

USE FAI_ENGLISH
GO

CREATE TABLE [Role] (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(255),
);

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255),
	Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
	PhoneNumber Varchar(15),
    CreatedAt DATETIME DEFAULT GETDATE(),
    [Status] NVARCHAR(10)  NOT NULL CHECK ([Status] IN ('Pending', 'Trial', 'Actived', 'Lock')) ,
	expiryDate DATETIME
);

CREATE TABLE [dbo].[UserRole]
(
    userId INT,
    roleId INT,
    PRIMARY KEY (userId, roleId),
    FOREIGN KEY (userId) REFERENCES [dbo].[Users] (userId),
    FOREIGN KEY (roleId) REFERENCES [dbo].[Role] (RoleId)
    );

CREATE TABLE VerificationToken
(
    tokenId    INT IDENTITY (1,1) PRIMARY KEY,
    tokenCode  NVARCHAR(MAX),
    expiryDate DATETIME,
    userId     INT UNIQUE, -- Đảm bảo mỗi userId chỉ xuất hiện một lần
    CONSTRAINT FK_UserToken FOREIGN KEY (userId) REFERENCES users (userId)
);

CREATE TABLE [Type] (
	TypeID INT IDENTITY(1,1) PRIMARY KEY,
	TypeName NVARCHAR(200)
);

CREATE TABLE Courses (
    CourseID INT IDENTITY(1,1) PRIMARY KEY,
    CourseName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Duration INT NOT NULL, -- Số giờ học
    CreatedAt DATETIME DEFAULT GETDATE(),
	TypeID Int foreign key references [Type](TypeID)

);



CREATE TABLE Lessons (
    LessonID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT FOREIGN KEY REFERENCES Courses(CourseID) ON DELETE CASCADE,
    Title NVARCHAR(255) NOT NULL,
    Situation NVARCHAR(MAX) NOT NULL,
    VideoURL NVARCHAR(500),
	ImageURL NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE()
);


CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    CourseID INT FOREIGN KEY REFERENCES Courses(CourseID) ON DELETE CASCADE,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) CHECK (Status IN ('Pending', 'Completed', 'Failed'))
);

CREATE TABLE Progress (
    ProgressID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    LessonID INT FOREIGN KEY REFERENCES Lessons(LessonID) ON DELETE CASCADE,
    Completed BIT DEFAULT 0,
    Score FLOAT,
    LastUpdated DATETIME DEFAULT GETDATE()
);

CREATE TABLE Feeback (
    FeebackID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    CourseID INT FOREIGN KEY REFERENCES Courses(CourseID) ON DELETE CASCADE,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

