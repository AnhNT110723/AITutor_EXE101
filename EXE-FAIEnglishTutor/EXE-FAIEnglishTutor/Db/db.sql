CREATE DATABASE FAI_ENGLISH
GO

USE FAI_ENGLISH
GO

CREATE TABLE [Role] (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(255),
);
INSERT INTO [Role] (RoleName) VALUES ('admin');
INSERT INTO [Role] (RoleName) VALUES ('mentor');
INSERT INTO [Role] (RoleName) VALUES ('mentee');
INSERT INTO [Role] (RoleName) VALUES ('staff');
INSERT INTO [Role] (RoleName) VALUES ('guest');

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255),
	Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
	PhoneNumber Varchar(15),
	Avatar Varchar(max),
    CreatedAt DATETIME DEFAULT GETDATE(),
    [Status] NVARCHAR(10)  NOT NULL CHECK ([Status] IN ('PENDING', 'TRIAL', 'ACTIVATED', 'LOCKED')) ,
	expiryDate DATETIME
);

INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber,Avatar, CreatedAt, [Status], expiryDate)
VALUES ('Nguyen Tuan Anh', 'anhnthe172115@fpt.edu.vn', 'AQAAAAEAACcQAAAAELfDRs7CCqQzPSu0IXkn11h2+6y8J0gFL7JhlYgD4HIo5Wd2EP8bKU4FCn1N8La50g==', '0869620295','/Images/avatar.jpg', GETDATE(), 'ACTIVATED', NULL);


CREATE TABLE [dbo].[UserRole]
(
    userId INT,
    roleId INT,
    PRIMARY KEY (userId, roleId),
    FOREIGN KEY (userId) REFERENCES [dbo].[Users] (userId),
    FOREIGN KEY (roleId) REFERENCES [dbo].[Role] (RoleId)
    );

INSERT INTO UserRole (userId, roleId) VALUES (1, 2);


CREATE TABLE RefreshToken
(
    RefreshTokenId INT IDENTITY(1,1) PRIMARY KEY,
    TokenCode NVARCHAR(MAX) NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    UserId INT NOT NULL,
    DeviceInfo NVARCHAR(256) NULL, -- Lưu thông tin thiết bị, nếu cần
	IsRevoked BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_UserRefreshToken FOREIGN KEY (UserId) REFERENCES Users(UserId)
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

