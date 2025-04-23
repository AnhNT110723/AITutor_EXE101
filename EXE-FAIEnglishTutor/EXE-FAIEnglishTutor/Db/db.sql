USE MASTER
GO

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
    PasswordHash NVARCHAR(255) NULL, -- Cho phép null cho Google, Facebook login
    PhoneNumber VARCHAR(15),
    Avatar NVARCHAR(MAX),
	Dob DATETIME,
	Province INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    [Status] NVARCHAR(10) NOT NULL CHECK ([Status] IN ('PENDING', 'TRIAL', 'ACTIVATED', 'LOCKED', 'DELETE')),
    expiryDate DATETIME,
    [Provider] NVARCHAR(50) NOT NULL, -- "Google", "Facebook", "Local", v.v.
    ProviderId NVARCHAR(255) NULL, -- ID từ provider (Google, Facebook), Local có thể NULL
    LastLogin DATETIME, -- Thời gian đăng nhập cuối

    -- Đảm bảo ProviderId chỉ unique khi nó KHÔNG NULL (Google/Facebook)
    CONSTRAINT CK_Provider_ProviderId CHECK (
        (Provider = 'Local' AND ProviderId IS NULL) 
        OR (Provider <> 'Local' AND ProviderId IS NOT NULL)
    )
);

-- Unique chỉ áp dụng khi ProviderId không NULL (chặn trùng ID từ Google, Facebook)
CREATE UNIQUE INDEX UQ_Provider_ProviderId 
ON Users([Provider], ProviderId) 
WHERE ProviderId IS NOT NULL;


INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber,Avatar, CreatedAt, [Status], expiryDate, [Provider])
VALUES ('Nguyen Tuan Anh', 'anhnthe172115@fpt.edu.vn', 'AQAAAAEAACcQAAAAELfDRs7CCqQzPSu0IXkn11h2+6y8J0gFL7JhlYgD4HIo5Wd2EP8bKU4FCn1N8La50g==', '0869620295','/Images/avatar.jpg', GETDATE(), 'ACTIVATED', NULL, 'Local');


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

    CREATE TABLE ExamType (
        ExamTypeID INT IDENTITY(1,1) PRIMARY KEY,
        ExamName NVARCHAR(100) -- 'IELTS', 'TOEIC'
    );

    CREATE TABLE ExamPart (
        PartID INT IDENTITY(1,1) PRIMARY KEY,
        ExamTypeID INT FOREIGN KEY REFERENCES ExamType(ExamTypeID),
        PartName NVARCHAR(100),         -- 'Listening Part 1', 'Reading Part 5', ...
        Description NVARCHAR(MAX) NULL  -- nếu cần mô tả thêm
    );

    CREATE TABLE Exam (
        ExamID INT IDENTITY(1,1) PRIMARY KEY,
        ExamTypeID INT FOREIGN KEY REFERENCES ExamType(ExamTypeID),
        Title NVARCHAR(255),
        Description NVARCHAR(MAX),
        CreatedAt DATETIME DEFAULT GETDATE(),
        ParentExamID INT NULL, 
        CONSTRAINT FK_Exam_Parent FOREIGN KEY (ParentExamID) REFERENCES Exam(ExamID)
    );


    CREATE TABLE ExamSection (
        SectionID INT IDENTITY(1,1) PRIMARY KEY,
        ExamID INT FOREIGN KEY REFERENCES Exam(ExamID),
        PartID INT FOREIGN KEY REFERENCES ExamPart(PartID)
    );


    CREATE TABLE Question (
        QuestionID INT IDENTITY(1,1) PRIMARY KEY,
        SectionID INT FOREIGN KEY REFERENCES ExamSection(SectionID),
        QuestionText NVARCHAR(MAX),
        AudioURL NVARCHAR(500),   -- cho câu hỏi nghe TOEIC/IELTS
        ImageURL NVARCHAR(500),   -- nếu có hình ảnh
        QuestionType NVARCHAR(50) CHECK (QuestionType IN ('MultipleChoice', 'FillBlank', 'Essay')),
        Explanation NVARCHAR(MAX) NULL
    );


    CREATE TABLE Answer (
        AnswerID INT IDENTITY(1,1) PRIMARY KEY,
        QuestionID INT FOREIGN KEY REFERENCES Question(QuestionID),
        AnswerText NVARCHAR(MAX),
        IsCorrect BIT
    );


    CREATE TABLE UserExamResult (
        ResultID INT IDENTITY(1,1) PRIMARY KEY,
        UserID INT FOREIGN KEY REFERENCES Users(UserID),
        ExamID INT FOREIGN KEY REFERENCES Exam(ExamID),
        StartTime DATETIME,
        EndTime DATETIME,
        Score FLOAT
    );


    CREATE TABLE UserAnswer (
        UserAnswerID INT IDENTITY(1,1) PRIMARY KEY,
        ResultID INT FOREIGN KEY REFERENCES UserExamResult(ResultID),
        QuestionID INT FOREIGN KEY REFERENCES Question(QuestionID),
        SelectedAnswerID INT FOREIGN KEY REFERENCES Answer(AnswerID),
        IsCorrect BIT
    );

    INSERT INTO ExamType 
    VALUES (N'TOEIC'),(N'IELTS'),
    (N'TOEIC Listening & Reading'), 
    (N'TOEIC Speaking & Writing');


    INSERT INTO ExamPart (ExamTypeID, PartName, Description) VALUES
(3, N'Listening Part 1', N'Mô tả part nghe tranh'),
(3, N'Listening Part 2', N'Mô tả part hỏi đáp'),
(3, N'Reading Part 5', N'Mô tả part điền từ'),
(2, N'Listening Section 1', N'Listening cho IELTS'),
(2, N'Writing Task 1', N'Viết báo cáo');


INSERT INTO Exam (ExamTypeID, Title, Description, CreatedAt) VALUES
(3, N'TEST ĐẦU VÀO', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00'),
(3, N'HACKER TOEIC 3', N'Đề thi luyện tập Hacker 3', '2025-04-20 10:00:00'),
(3, N'HACKER TOEIC 2', N'Đề thi luyện tập Hacker 2', '2025-04-19 09:30:00'),
(1, N'ETS 2024', N'Đề mô phỏng từ ETS 2024', '2025-04-18 15:20:00'),
(2, N'IELTS MOCK TEST 1', N'Thi thử IELTS 4 kỹ năng', '2025-04-17 11:45:00');

INSERT INTO Exam (ExamTypeID, Title, Description, CreatedAt, ParentExamID) VALUES
(3, N'TEST ĐẦU VÀO (1)', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00', 1),
(3, N'TEST ĐẦU VÀO (2)', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00',1),
(3, N'TEST ĐẦU VÀO (3)', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00',1);

INSERT INTO ExamSection (ExamID, PartID) VALUES
(1, 1),
(1, 2),
(2, 1),
(2, 2),
(2, 3),
(3, 1),
(3, 2),
(4, 1),
(4, 2),
(4, 3),
(5, 4),
(5, 5);


INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(1, N'What is the man doing?', '/audio/q1.mp3', '/img/q1.jpg', 'MultipleChoice', N'Nhìn hành động'),
(2, N'Where is the nearest bank?', '/audio/q2.mp3', NULL, 'MultipleChoice', N'Đoán theo context'),
(5, N'Choose the correct word', NULL, NULL, 'FillBlank', N'Từ loại đúng');


INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(1, N'He is cooking', 0),
(1, N'He is running', 1),
(1, N'He is sleeping', 0),
(2, N'Next to the library', 1),
(2, N'In the supermarket', 0),
(3, N'went', 1),
(3, N'go', 0);


INSERT INTO UserExamResult (UserID, ExamID, StartTime, EndTime, Score) VALUES
(2, 1, '2025-04-21 09:00:00', '2025-04-21 10:30:00', 650);


INSERT INTO UserAnswer (ResultID, QuestionID, SelectedAnswerID, IsCorrect) VALUES
(2, 1, 2, 1),
(2, 2, 4, 1),
(2, 3, 6, 1);



