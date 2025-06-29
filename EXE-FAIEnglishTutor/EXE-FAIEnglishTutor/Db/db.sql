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
	UpgradeLevel INT,
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


INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber,Avatar, CreatedAt, [Status], UpgradeLevel, expiryDate, [Provider])
VALUES ('Admin FAI', 'faienglishtutor@gmail.com', 'AQAAAAEAACcQAAAAELfDRs7CCqQzPSu0IXkn11h2+6y8J0gFL7JhlYgD4HIo5Wd2EP8bKU4FCn1N8La50g==', '+84869620295','/Images/avatar.jpg', GETDATE(), 'ACTIVATED', 0, NULL, 'Local');


CREATE TABLE [dbo].[UserRole]
(
    userId INT,
    roleId INT,
    PRIMARY KEY (userId, roleId),
    FOREIGN KEY (userId) REFERENCES [dbo].[Users] (userId),
    FOREIGN KEY (roleId) REFERENCES [dbo].[Role] (RoleId)
    );

INSERT INTO UserRole (userId, roleId) VALUES (1, 1);


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

CREATE TABLE [Level] (
	LevelID INT IDENTITY(1,1) PRIMARY KEY,
	LevelName NVARCHAR(200),
	LevelScore NVARCHAR(50)
);


CREATE TABLE Situation (
    SituatuonID INT IDENTITY(1,1) PRIMARY KEY,
    SituationName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
	ImageUrl NVARCHAR(MAX),
	RoleAI NVARCHAR(255),
	RoleUser NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
	TypeID Int foreign key references [Type](TypeID),
	LevelID Int foreign key references [Level](LevelID)
);
ALTER TABLE [FAI_ENGLISH].[dbo].[Situation]
ADD [AudioURL] NVARCHAR(500) NULL;


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
	Content NVARCHAR(200),
    Status NVARCHAR(50) CHECK (Status IN ('Pending', 'Completed', 'Failed', 'Cancel'))
);

CREATE TABLE [Podcast] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(255) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME NOT NULL,
    [UserId] INT NULL,
    [Topic] NVARCHAR(100) NULL
);
ALTER TABLE [FAI_ENGLISH].[dbo].[Podcast]
ADD [AudioUrl] NVARCHAR(500);

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
		PartType VARCHAR(50),
        PartName NVARCHAR(100),         -- 'Listening Part 1', 'Reading Part 5', ...
        Description NVARCHAR(MAX) NULL,  -- nếu cần mô tả thêm
		DefaultDuration INT
    );

    CREATE TABLE Exam (
        ExamID INT IDENTITY(1,1) PRIMARY KEY,
        ExamTypeID INT FOREIGN KEY REFERENCES ExamType(ExamTypeID),
        Title NVARCHAR(255),
        Description NVARCHAR(MAX),
        CreatedAt DATETIME DEFAULT GETDATE(),
        ParentExamID INT NULL,
        Slug NVARCHAR(255),
		Duration INT,
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
        Score FLOAT,
		CustomDuration INT
    );

	CREATE TABLE UserExamPartSelection (
    SelectionID INT IDENTITY(1,1) PRIMARY KEY,
    ResultID INT FOREIGN KEY REFERENCES UserExamResult(ResultID),
    PartID INT FOREIGN KEY REFERENCES ExamPart(PartID),
    CONSTRAINT UK_UserExamPartSelection UNIQUE (ResultID, PartID)
);

    CREATE TABLE UserAnswer (
        UserAnswerID INT IDENTITY(1,1) PRIMARY KEY,
        ResultID INT FOREIGN KEY REFERENCES UserExamResult(ResultID),
        QuestionID INT FOREIGN KEY REFERENCES Question(QuestionID),
        SelectedAnswerID INT FOREIGN KEY REFERENCES Answer(AnswerID),
        IsCorrect BIT
    );


INSERT INTO [Type]  VALUES 
(N'Role Play'),(N'Listening'), (N'Pronounce');


-- TOEIC Levels
INSERT INTO [Level] (LevelName, LevelScore) VALUES 
(N'Beginner', '0-250'),
(N'Pre-Intermediate', '405-600'),
(N'Intermediate', '605-780'),
(N'Upper-Intermediate', '785-900'),
(N'Upper-Intermediate', '905-990'),
(N'Advanced','0-250')




INSERT INTO Situation (SituationName, Description, ImageUrl, RoleAI, RoleUser, TypeId , LevelID)
VALUES 
(N'Gọi đặt lịch khám', N'Sáng nay ngủ dậy bạn nhức đầu và đau họng kinh khủng. Bạn cần đi bác sĩ khám bệnh và lấy thuốc. Hãy gọi cho phòng khám để đặt lịch nhé!', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a64-78f8-663a-be94-0242ac110002-1723465666.png', N'Y tá', N'Người bệnh',1, 1)



    INSERT INTO ExamType 
    VALUES (N'TOEIC'),(N'IELTS'),
    (N'TOEIC Listening & Reading'), 
    (N'TOEIC Speaking & Writing');


-- [Thay đổi] Cập nhật dữ liệu cho ExamPart, bao gồm PartType và DefaultDuration
INSERT INTO ExamPart (ExamTypeID, PartName, Description, PartType, DefaultDuration) VALUES
(3, N'Listening Part 1', N'Mô tả part nghe tranh', 'Listening', 10), -- 6 câu -> 10 phút
(3, N'Listening Part 2', N'Mô tả part hỏi đáp', 'Listening', 20), -- 25 câu -> 20 phút
(3, N'Listening Part 3', N'Mô tả part hội thoại', 'Listening', 30), -- 39 câu -> 30 phút
(3, N'Listening Part 4', N'Mô tả part bài nói', 'Listening', 25), -- 30 câu -> 25 phút
(3, N'Reading Part 5', N'Mô tả part điền từ', 'Reading', 25), -- 30 câu -> 25 phút
(3, N'Reading Part 6', N'Mô tả part điền đoạn văn', 'Reading', 15), -- 16 câu -> 15 phút
(3, N'Reading Part 7', N'Mô tả part đọc hiểu', 'Reading', 40), -- 54 câu -> 40 phút
(2, N'Listening Section 1', N'Listening cho IELTS', 'Listening', 20),
(2, N'Writing Task 1', N'Viết báo cáo', 'Writing', 20);


INSERT INTO Exam (ExamTypeID, Title, Description, CreatedAt) VALUES
(3, N'TEST ĐẦU VÀO', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00'),
(3, N'HACKER TOEIC 3', N'Đề thi luyện tập Hacker 3', '2025-04-20 10:00:00'),
(3, N'HACKER TOEIC 2', N'Đề thi luyện tập Hacker 2', '2025-04-19 09:30:00'),
(1, N'ETS 2024', N'Đề mô phỏng từ ETS 2024', '2025-04-18 15:20:00'),
(2, N'IELTS MOCK TEST 1', N'Thi thử IELTS 4 kỹ năng', '2025-04-17 11:45:00');

INSERT INTO Exam (ExamTypeID, Title, Description, CreatedAt, ParentExamID, Slug, Duration) VALUES
(3, N'TEST ĐẦU VÀO (1)', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00', 1,'test-dau-vao-1', 120),
(3, N'TEST ĐẦU VÀO (2)', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00',1,'test-dau-vao-2', 120),
(3, N'TEST ĐẦU VÀO (3)', N'Kiểm tra năng lực đầu vào', '2025-04-21 08:00:00',1,'test-dau-vao-3', 120);

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
(5, 5),
(6, 1),
(6, 2),
(6, 3),
(6, 4),
(6, 5),
(6, 6),
(6, 7);



INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(1, N'What is the man doing?', '/audio/q1.mp3', '/img/q1.jpg', 'MultipleChoice', N'Nhìn hành động'),
(2, N'Where is the nearest bank?', '/audio/q2.mp3', NULL, 'MultipleChoice', N'Đoán theo context'),
(5, N'Choose the correct word', NULL, NULL, 'FillBlank', N'Từ loại đúng');
-- Listening Part 1: Mô tả tranh (6 câu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(13, N'What is the woman doing?', '/audio/listening/part1/q1.mp3', '/img/listening/part1/q1.jpg', 'MultipleChoice', N'The woman is walking in the park.'),
(13, N'What is the man holding?', '/audio/listening/part1/q2.mp3', '/img/listening/part1/q2.jpg', 'MultipleChoice', N'The man is holding a book.'),
(13, N'Where are the people sitting?', '/audio/listening/part1/q3.mp3', '/img/listening/part1/q3.jpg', 'MultipleChoice', N'The people are sitting on a bench.');
-- Listening Part 2: Hỏi đáp (25 câu, thêm 3 câu mẫu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(14, N'Where is the meeting room?', '/audio/listening/part2/q1.mp3', NULL, 'MultipleChoice', N'The meeting room is on the second floor.'),
(14, N'What time does the train leave?', '/audio/listening/part2/q2.mp3', NULL, 'MultipleChoice', N'The train leaves at 3 PM.'),
(14, N'Who is calling?', '/audio/listening/part2/q3.mp3', NULL, 'MultipleChoice', N'The manager is calling.');
-- Listening Part 3: Hội thoại (39 câu, thêm 3 câu mẫu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(15, N'What are they discussing?', '/audio/listening/part3/q1.mp3', NULL, 'MultipleChoice', N'They are discussing a new project.'),
(15, N'When will the meeting take place?', '/audio/listening/part3/q2.mp3', NULL, 'MultipleChoice', N'The meeting will take place tomorrow.'),
(15, N'Who is leading the project?', '/audio/listening/part3/q3.mp3', NULL, 'MultipleChoice', N'Mr. Smith is leading the project.');
-- Listening Part 4: Bài nói (30 câu, thêm 3 câu mẫu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(16, N'What is the speaker talking about?', '/audio/listening/part4/q1.mp3', NULL, 'MultipleChoice', N'The speaker is talking about company policies.'),
(16, N'When is the event scheduled?', '/audio/listening/part4/q2.mp3', NULL, 'MultipleChoice', N'The event is scheduled for next week.'),
(16, N'Who should employees contact?', '/audio/listening/part4/q3.mp3', NULL, 'MultipleChoice', N'Employees should contact the HR department.');
-- Reading Part 5: Điền từ (30 câu, thêm 3 câu mẫu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(17, N'The manager ______ the report yesterday.', NULL, NULL, 'FillBlank', N'Correct word: "finished".'),
(17, N'She ______ to the meeting on time.', NULL, NULL, 'FillBlank', N'Correct word: "came".'),
(17, N'They ______ working on the project.', NULL, NULL, 'FillBlank', N'Correct word: "are".');
-- Reading Part 6: Điền đoạn văn (16 câu, thêm 3 câu mẫu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(18, N'Choose the best word to fill in the blank: "The company has ______ its profits this year."', NULL, NULL, 'MultipleChoice', N'Correct word: "increased".'),
(18, N'Choose the best phrase: "We need to ______ the deadline."', NULL, NULL, 'MultipleChoice', N'Correct phrase: "meet".'),
(18, N'Choose the best word: "The team works ______ together."', NULL, NULL, 'MultipleChoice', N'Correct word: "well".');
-- Reading Part 7: Đọc hiểu (54 câu, thêm 3 câu mẫu)
INSERT INTO Question (SectionID, QuestionText, AudioURL, ImageURL, QuestionType, Explanation) VALUES
(19, N'What is the main purpose of the email?', NULL, '/img/reading/part7/email.jpg', 'MultipleChoice', N'The email is to announce a meeting.'),
(19, N'Who is the email addressed to?', NULL, '/img/reading/part7/email.jpg', 'MultipleChoice', N'The email is addressed to all employees.'),
(19, N'When is the meeting scheduled?', NULL, '/img/reading/part7/email.jpg', 'MultipleChoice', N'The meeting is scheduled for Monday.');
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(1, N'He is cooking', 0),
(1, N'He is running', 1),
(1, N'He is sleeping', 0),
(2, N'Next to the library', 1),
(2, N'In the supermarket', 0),
(3, N'went', 1),
(3, N'go', 0);

-- QuestionID 4: What is the woman doing?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(4, N'She is reading.', 0),
(4, N'She is walking.', 1),
(4, N'She is cooking.', 0),
(4, N'She is sleeping.', 0);

-- QuestionID 5: What is the man holding?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(5, N'A phone.', 0),
(5, N'A book.', 1),
(5, N'A bag.', 0),
(5, N'A cup.', 0);

-- QuestionID 6: Where are the people sitting?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(6, N'In a car.', 0),
(6, N'On a bench.', 1),
(6, N'At a table.', 0),
(6, N'On the floor.', 0);


-- QuestionID 7: Where is the meeting room?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(7, N'On the first floor.', 0),
(7, N'On the second floor.', 1),
(7, N'On the third floor.', 0),
(7, N'In the basement.', 0);

-- QuestionID 8: What time does the train leave?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(8, N'At 2 PM.', 0),
(8, N'At 3 PM.', 1),
(8, N'At 4 PM.', 0),
(8, N'At 5 PM.', 0);

-- QuestionID 9: Who is calling?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(9, N'The manager.', 1),
(9, N'The client.', 0),
(9, N'The receptionist.', 0),
(9, N'The colleague.', 0);

-- QuestionID 10: What are they discussing?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(10, N'A new project.', 1),
(10, N'A vacation plan.', 0),
(10, N'A budget report.', 0),
(10, N'A marketing strategy.', 0);

-- QuestionID 11: When will the meeting take place?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(11, N'Today.', 0),
(11, N'Tomorrow.', 1),
(11, N'Next week.', 0),
(11, N'Next month.', 0);

-- QuestionID 12: Who is leading the project?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(12, N'Mr. Smith.', 1),
(12, N'Ms. Johnson.', 0),
(12, N'Mr. Brown.', 0),
(12, N'Ms. Lee.', 0);

-- QuestionID 13: What is the speaker talking about?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(13, N'Company policies.', 1),
(13, N'A new product.', 0),
(13, N'A sales report.', 0),
(13, N'A training session.', 0);

-- QuestionID 14: When is the event scheduled?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(14, N'Tomorrow.', 0),
(14, N'Next week.', 1),
(14, N'Next month.', 0),
(14, N'Next year.', 0);

-- QuestionID 15: Who should employees contact?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(15, N'The HR department.', 1),
(15, N'The IT department.', 0),
(15, N'The marketing team.', 0),
(15, N'The sales team.', 0);


-- QuestionID 16: The manager ______ the report yesterday.
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(16, N'finish.', 0),
(16, N'finished.', 1),
(16, N'finishing.', 0),
(16, N'finishes.', 0);

-- QuestionID 17: She ______ to the meeting on time.
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(17, N'come.', 0),
(17, N'came.', 1),
(17, N'comes.', 0),
(17, N'coming.', 0);

-- QuestionID 18: They ______ working on the project.
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(18, N'is.', 0),
(18, N'are.', 1),
(18, N'was.', 0),
(18, N'were.', 0);

-- QuestionID 19: The company has ______ its profits this year.
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(19, N'increased.', 1),
(19, N'decreased.', 0),
(19, N'maintained.', 0),
(19, N'reduced.', 0);

-- QuestionID 20: We need to ______ the deadline.
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(20, N'miss.', 0),
(20, N'meet.', 1),
(20, N'extend.', 0),
(20, N'ignore.', 0);

-- QuestionID 21: The team works ______ together.
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(21, N'good.', 0),
(21, N'well.', 1),
(21, N'bad.', 0),
(21, N'poorly.', 0);

-- QuestionID 22: What is the main purpose of the email?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(22, N'To announce a meeting.', 1),
(22, N'To request a report.', 0),
(22, N'To share a schedule.', 0),
(22, N'To discuss a project.', 0);

-- QuestionID 23: Who is the email addressed to?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(23, N'The manager.', 0),
(23, N'All employees.', 1),
(23, N'The HR team.', 0),
(23, N'The sales team.', 0);

-- QuestionID 24: When is the meeting scheduled?
INSERT INTO Answer (QuestionID, AnswerText, IsCorrect) VALUES
(24, N'Monday.', 1),
(24, N'Tuesday.', 0),
(24, N'Wednesday.', 0),
(24, N'Thursday.', 0);



--ANH EM CẦN TẠO TÀI KHOẢN TRƯỚC R CHẠY TIẾP CÂU LỆNH NÀY TRỞ ĐI
INSERT INTO UserExamResult (UserID, ExamID, StartTime, EndTime, Score) VALUES
(1, 1, '2025-04-21 09:00:00', '2025-04-21 10:30:00', 650);


INSERT INTO UserAnswer (ResultID, QuestionID, SelectedAnswerID, IsCorrect) VALUES
(1, 1, 2, 1),
(1, 2, 4, 1),
(1, 3, 6, 1);



INSERT INTO Situation (SituationName, Description, ImageUrl, RoleAI, RoleUser, TypeId, LevelID)
VALUES 
('Asking for directions to a supermarket', 'You’re in a new neighborhood and need to find the nearest supermarket. Ask a passerby for directions!', '/images/role-play/asking-for-directions-to-a-supermarket.jpg', 'Person asking for directions', 'Passerby', 1, 1),

('Discussing a work project', 'You’re working in a team and need to discuss project progress with a colleague. Talk about your tasks and ask for feedback!', '/images/role-play/discussing-work.jpg', 'Colleague', 'Employee', 1, 3),

('Presenting a business idea', 'You need to pitch a new business idea to your boss. Explain the idea and answer their questions!', '/images/role-play/pressenting-idea.webp', 'Boss', 'Employee', 1, 4),

('Job interview', 'You’re attending an interview for a managerial position. Answer questions about your experience and skills!', '/images/role-play/job-intervierw.jpg', 'Interviewer', 'Candidate', 1, 5),

('Booking a hotel room', 'You need to book a hotel room for a business trip at JW Marriott Hotel Hanoi. Call to ask about prices, amenities, and confirm the reservation!', '/images/role-play/booking-hotel.webp', 'Hotel receptionist', 'Customer', 1, 3),

('Complaining about a product', 'You bought a shirt but noticed it’s torn. Call the Dior store to complain and request a replacement!', '/images/role-play/complain-product.jpg', 'Store staff', 'Customer', 1, 2),

('Resolving a workplace dispute', 'You and a colleague disagree on how to handle a task. Discuss to find the best solution!', '/images/role-play/resolvin-a-workplace-dispute.jpg', 'Colleague', 'Employee', 1, 4),

('Ordering food at a restaurant', 'You walk into a fast-food restaurant and want to order a hamburger and a soft drink. Talk to the staff to place your order!', '/images/role-play/order-restaurant.jpg', 'Restaurant staff', 'Customer', 1, 1),

('Pitching to investors', 'You’re presenting a startup project to a group of investors. Convince them to invest by explaining benefits and answering detailed questions!', '/images/role-play/pitching-to-investors-a-min.jpg', 'Investor', 'Entrepreneur', 1, 6),

('Negotiating a contract', 'You’re negotiating a contract with a business partner. Discuss terms and try to reach the best agreement!', '/images/role-play/negotiating-contract-terms.jpg', 'Business partner', 'Employee', 1, 5),

('Booking movie tickets', 'You want to watch a new movie at the cinema. Call the theater to book tickets and ask about showtimes!', '/images/role-play/booking-movie.webp', 'Cinema staff', 'Customer', 1, 2),

('Consulting on business strategy', 'You’re a consultant and need to suggest improvements to a struggling company’s business strategy. Discuss with the director!', '/images/role-play/consulting-on-business-strategy.webp', 'Company director', 'Consultant', 1, 6);


--LISTENING DATA
USE [FAI_ENGLISH]
GO

SET IDENTITY_INSERT [dbo].[Situation] OFF -- Ensure identity insert is OFF

-- Inserting records without specifying the identity column
INSERT [dbo].[Situation] ([SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES 
(N'Booking a Doctor Appointment', N'You woke up this morning with a terrible headache and sore throat. You need to visit the doctor for an examination and medication. Call the clinic to make an appointment!', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a64-78f8-663a-be94-0242ac110002-1723465666.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 1),

(N'Child with Persistent Cough', N'The child has been coughing for several days, possibly due to bronchitis or a sore throat.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/9/1ef81600-b2a1-659c-b6f8-0242ac110005-1727943497.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 2),

(N'Child with Diarrhea', N'The child has frequent loose stools and needs to avoid dehydration.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-21c3-622e-aa19-0242ac110002-1723465683.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 2),

(N'Child Showing Allergy Symptoms', N'Rash or itching appeared after eating or exposure to an allergen.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-4b1e-62cc-8ab5-0242ac110002-1723465688.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 3),

(N'Child with Minor Injury', N'The child fell and got a minor scrape. Needs disinfection and monitoring.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/7/1ef58a65-7f9b-68f6-a031-0242ac110002-1723465693.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 5),

(N'Greetings and Introductions', N'Basic greetings and self-introduction for beginners.', N'https://i.ytimg.com/vi/KKh_CallEp8/maxresdefault.jpg', N'Hello! What''s your name?', N'My name is Anna.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 1),

(N'Daily Routines', N'Listening to simple daily routines.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT5AvLdE1tXqkMwbt6bbKNytBDxRdl0mdXswA&s', N'What do you do every morning?', N'I brush my teeth and have breakfast.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 1),

(N'Ordering Food', N'Listening to restaurant conversations.', N'https://www.thoughtco.com/thmb/5I9sp2N2xSRZ7ugZu1crYb-19QE=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/beginner-dialogues-at-a-restaurant-1210039_FINAL-587dc04424dc4a12897f120a57bed771.png', N'May I take your order?', N'I would like a hamburger, please.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 2),

(N'Asking for Directions', N'Simple street direction dialogues.', N'https://www.shutterstock.com/image-vector/local-man-help-travelers-couple-600nw-2161932825.jpg', N'Where is the post office?', N'Turn left at the corner.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 2),

(N'Travel Planning', N'Planning a trip and making reservations.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSPGQ7V1Vm3i_cJBTZ6wezDcttEXkmXzIaOLQ&s', N'When would you like to fly?', N'Next Monday, please.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 3),

(N'Shopping Conversation', N'Dialogues in shops or markets.', N'https://i.ytimg.com/vi/4kr-TKEEUH0/maxresdefault.jpg', N'How much is this shirt?', N'It''s twenty dollars.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 3),

(N'Job Interview', N'Listening to job interview scenarios.', N'https://eaog2nkqckp.exactdn.com/wp-content/uploads/2022/12/6287e12157f45e15bd788725_nervous-job-interview.png?strip=all&lossy=1&ssl=1', N'What are your strengths?', N'I am very organized and responsible.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 4),

(N'Medical Appointment', N'Dialogue in a clinic setting.', N'https://img.freepik.com/premium-vector/medical-appointment-with-happy-doctor-character-flat-design-illustration_692379-39.jpg', N'What symptoms do you have?', N'I have a sore throat and fever.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 4),

(N'News Report', N'Listening to a short news broadcast.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRPOogpwPKwSTLJ06xpopxxGU_2xTACOk2jug&s', N'Today''s headline: heavy rain causes flooding.', N'This has affected over 500 homes.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 5),

(N'Environmental Issues Discussion', N'Discussion on climate change.', N'https://media.geeksforgeeks.org/wp-content/uploads/20230620175013/Environmental-Issues-.webp', N'What can we do to reduce pollution?', N'We should use less plastic and drive less.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 5);

GO


INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (13, N'Tech Podcast 2025', N'Hello and welcome to our podcast! Today, we will be discussing the technology trends that are expected to shape the year 2025. 

One of the most anticipated developments in the tech world is the widespread adoption of 5G technology. With its faster speeds and lower latency, 5G is set to revolutionize the way we connect and communicate. This will pave the way for advancements in areas such as virtual reality, augmented reality, and the Internet of Things.

Speaking of the Internet of Things, experts predict that by 2025, there will be over 75 billion connected devices worldwide. This interconnected network of devices will enable seamless communication between our gadgets, appliances, and even vehicles. Smart homes and smart cities will become more prevalent, leading to increased efficiency and convenience in our daily lives.

Artificial intelligence is another technology trend that is expected to continue its rapid growth in 2025. AI-powered systems will become more sophisticated and capable of performing complex tasks, such as autonomous driving, medical diagnosis, and natural language processing. However, ethical concerns surrounding AI will also come to the forefront, as we grapple with issues of privacy, bias, and accountability.

In the realm of healthcare, advancements in genomics and personalized medicine will revolutionize the way we approach treatment and prevention. With the help of AI and big data analytics, doctors will be able to tailor medical interventions to individual patients based on their genetic makeup and lifestyle factors.

Overall, the year 2025 promises to be an exciting time for technology, with innovations in 5G, IoT, AI, and healthcare transforming the way we live, work, and interact with the world around us. Stay tuned as we continue to explore these trends and their implications in future episodes. Thank you for listening!', N'https://cdn.ceps.eu/wp-content/uploads/2024/06/Podcast-news-01.png', CAST(N'2025-06-25T15:22:49.140' AS DateTime), NULL, N'Technology trends in 2025', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750864843/podcast_638864617655713693.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (14, N'AI for All: The 2025 Revolution', N'Artificial intelligence, or AI, has become an integral part of our everyday lives. From voice assistants like Siri and Alexa to recommendation algorithms on streaming platforms like Netflix and Spotify, AI is all around us, making our lives easier and more efficient.

One of the most common uses of AI in everyday life is in the realm of virtual assistants. These AI-powered systems can schedule appointments, set reminders, and even provide weather updates with just a simple voice command. This technology has revolutionized the way we interact with our devices, allowing for a more seamless and hands-free experience.

In addition to virtual assistants, AI is also being used in the healthcare industry to improve patient care and diagnosis. Machine learning algorithms can analyze medical images and data to detect diseases like cancer at an early stage, leading to better outcomes for patients. AI is also being used to personalize treatment plans and predict patient outcomes, allowing for more targeted and effective healthcare interventions.

AI is also present in the world of online shopping and e-commerce. Recommendation algorithms use AI to analyze customer behavior and preferences, providing personalized product recommendations that increase sales and customer satisfaction. These algorithms can also help retailers optimize their inventory and pricing strategies, leading to more efficient and profitable businesses.

Despite its many benefits, AI also raises ethical and social concerns. Privacy issues, algorithmic bias, and job displacement are just a few of the challenges that come with the widespread adoption of AI in everyday life. It is important for policymakers, businesses, and individuals to consider these implications and work towards creating a more ethical and responsible AI ecosystem.

In conclusion, artificial intelligence has the potential to transform our everyday lives in profound ways. By harnessing the power of AI responsibly and ethically, we can unlock its full potential to improve healthcare, streamline our daily tasks, and enhance our overall quality of life. As we continue to integrate AI into our lives, it is crucial to approach this technology with caution and mindfulness to ensure a positive and equitable future for all.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSNS3sWvUi3W9AImikCLn6a2SSrvxFbghVI0w&s', CAST(N'2025-06-25T17:25:52.817' AS DateTime), NULL, N'Artificial Intelligence in Everyday Life', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872226/podcast_638864691493012762.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (15, N'Remote Work 2.0: What''s Next?', N'In recent years, remote work has become increasingly popular, with more and more companies embracing the concept of allowing their employees to work from home or from any location of their choosing. This shift has been accelerated by advancements in technology, which have made it easier for people to stay connected and collaborate with their colleagues, even when they are not physically present in the office.

The COVID-19 pandemic further highlighted the benefits of remote work, as companies were forced to quickly adapt to a remote work model to ensure the safety of their employees. This unprecedented situation has shown that remote work is not only possible, but also highly effective in many cases. As a result, many companies are now considering making remote work a permanent option for their employees even after the pandemic is over.

So, what does the future hold for remote work? It is clear that remote work is here to stay, and it is likely to become even more prevalent in the years to come. With more and more companies adopting remote work policies, we can expect to see a shift in the way we think about work and the traditional office environment. Remote work offers employees greater flexibility and work-life balance, allowing them to better manage their personal and professional responsibilities.

However, remote work also presents its own set of challenges. Communication and collaboration can be more difficult in a remote work setting, and employees may struggle with feelings of isolation and disconnection from their colleagues. Companies will need to find ways to address these challenges and ensure that their remote employees feel supported and connected to the rest of the team.

Overall, the future of remote work looks bright. As technology continues to advance and companies become more comfortable with the idea of remote work, we can expect to see even greater adoption of this flexible work model. Remote work offers a host of benefits for both employees and employers, and it is likely to become a key part of the modern work environment. As we move forward, it will be important for companies to embrace remote work and find ways to make it work for everyone involved.', N'https://i.ytimg.com/vi/ycyE0EOE_MQ/sddefault.jpg', CAST(N'2025-06-25T17:27:29.463' AS DateTime), NULL, N'The Future of Remote Work', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872323/podcast_638864692457994277.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (16, N'Smart Health: Wearables in 2025', N'Hello and welcome to our podcast on wearable health tech innovations. With the advancement of technology, wearable devices have become increasingly popular in the healthcare industry. These devices are designed to monitor and track various aspects of our health, allowing us to take control of our well-being like never before.

One of the most common types of wearable health tech is fitness trackers. These devices can monitor our physical activity, heart rate, and even our sleep patterns. By providing us with real-time data, fitness trackers motivate us to stay active and make healthier choices throughout the day. Some advanced trackers even offer personalized workout plans and tips for improving our overall fitness levels.

Another exciting innovation in wearable health tech is smart clothing. These garments are embedded with sensors that can track our vital signs, such as heart rate and body temperature. Smart clothing is particularly useful for athletes and individuals with chronic health conditions, as it can provide valuable insights into their physical well-being during exercise or daily activities.

Additionally, wearable devices are revolutionizing the way we manage chronic conditions such as diabetes and hypertension. For example, continuous glucose monitors can track blood sugar levels in real-time, allowing individuals with diabetes to make timely adjustments to their insulin dosage or diet. Similarly, smart blood pressure monitors can help individuals with hypertension monitor their blood pressure and receive alerts if it spikes to dangerous levels.

Overall, wearable health tech innovations are empowering individuals to take charge of their health and well-being in a convenient and efficient manner. As technology continues to advance, we can expect even more groundbreaking developments in the field of wearable health tech. So, whether you''re looking to improve your fitness levels, manage a chronic condition, or simply live a healthier lifestyle, wearable devices are certainly worth considering. Thank you for listening to our podcast, and we hope you found this information on wearable health tech innovations helpful. Stay tuned for more exciting updates in the world of health technology.', N'https://cdn.mos.cms.futurecdn.net/FkGweMeB7hdPgaSFQdgsfj.jpg', CAST(N'2025-06-25T17:28:07.863' AS DateTime), NULL, N'Wearable Health Tech Innovations', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872361/podcast_638864692846607448.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (17, N'Hyperconnected: The 5G Era', N'Hello and welcome to our podcast on 5G and beyond: Next-Gen Connectivity. In today''s episode, we will be discussing the future of communication technology and how 5G is paving the way for even faster and more reliable connectivity.

5G, the fifth generation of wireless technology, has been hailed as a game-changer in the world of telecommunications. With speeds up to 100 times faster than 4G, 5G is enabling a whole new era of innovation and connectivity. From autonomous vehicles to smart cities, the possibilities are endless with 5G technology.

But what comes next? Beyond 5G, researchers and engineers are already working on developing the next generation of connectivity. This new technology, often referred to as 6G, is expected to be even faster and more efficient than 5G. Imagine downloading a movie in just a few seconds or streaming high-definition video without any lag - that''s the kind of experience that 6G promises to deliver.

One of the key features of 6G will be its ability to support massive connectivity, allowing for billions of devices to be connected simultaneously. This will be crucial for the development of the Internet of Things (IoT) and the proliferation of smart devices in our homes, offices, and cities.

In addition to speed and connectivity, security will also be a top priority for 6G technology. With cyber threats on the rise, it is essential that the next generation of connectivity is built with robust security measures to protect data and privacy.

Overall, the future of communication technology looks incredibly bright with the advancements in 5G and beyond. As we continue to push the boundaries of what is possible, we can expect to see a world that is more connected, more efficient, and more innovative than ever before.

Thank you for tuning in to our podcast on 5G and beyond: Next-Gen Connectivity. Stay tuned for more episodes on the latest trends and developments in technology. Until next time!', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJi7ptUNFOGKA5Sg6qhJg5bSQ2jYZT-1flbw&s', CAST(N'2025-06-25T17:29:07.070' AS DateTime), NULL, N'5G and Beyond: Next-Gen Connectivity', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872421/podcast_638864693438504130.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (18, N'GreenTech: Powering a Sustainable Future', N'Hello everyone and welcome back to our podcast. Today, we will be discussing the importance of sustainable technology and green energy in our modern society.

In recent years, there has been a growing awareness of the impact that traditional forms of energy production have on our environment. The burning of fossil fuels for electricity and transportation has led to increased levels of greenhouse gas emissions, which in turn contribute to climate change. This is why the development and adoption of sustainable technology and green energy sources have become more crucial than ever.

One of the key benefits of sustainable technology and green energy is their ability to reduce our reliance on finite resources like coal, oil, and natural gas. Instead, renewable energy sources such as solar, wind, and hydropower offer a cleaner and more sustainable alternative. By harnessing these sources of energy, we can significantly decrease our carbon footprint and work towards a more sustainable future.

Furthermore, sustainable technology not only benefits the environment but also has economic advantages. The renewable energy sector has been growing rapidly in recent years, creating jobs and stimulating economic growth. Additionally, investing in sustainable technology can lead to long-term cost savings for both businesses and consumers.

It is important for individuals, businesses, and governments to prioritize the development and adoption of sustainable technology and green energy. By making conscious choices to support renewable energy sources and invest in energy-efficient technologies, we can all contribute to a more sustainable and environmentally-friendly future.

In conclusion, sustainable technology and green energy play a vital role in addressing the challenges of climate change and environmental degradation. By embracing these technologies and transitioning towards a more sustainable energy system, we can create a cleaner, healthier, and more prosperous world for future generations. Thank you for listening and stay tuned for more discussions on sustainability and green living.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQpPrqFhwhwhmCq_w7lIe9uVLw490jmx5t9HA&s', CAST(N'2025-06-25T17:29:52.487' AS DateTime), NULL, N'Sustainable Tech and Green Energy', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872466/podcast_638864693888380237.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (19, N'Blockchain Everywhere: Real-World Uses', N'Hello and welcome back to our podcast. Today, we''re going to explore the world of blockchain applications beyond cryptocurrency. Blockchain technology has gained immense popularity in recent years due to its decentralized and secure nature. While it is most commonly associated with cryptocurrencies like Bitcoin, blockchain has the potential to revolutionize various industries beyond finance.

One of the most promising applications of blockchain technology is in supply chain management. By using blockchain, companies can track the journey of a product from its origin to the hands of the consumer. This not only helps in ensuring the authenticity and quality of products but also improves transparency and trust between all parties involved in the supply chain.

Another area where blockchain is making a significant impact is in healthcare. With the ability to securely store and share sensitive data, blockchain technology can help in improving the efficiency and security of medical records. This can lead to better coordination among healthcare providers and ultimately improve patient outcomes.

Blockchain technology is also being explored in the field of voting and elections. By using blockchain for voting, governments can ensure the integrity and transparency of the electoral process, reducing the risk of fraud and manipulation. This can lead to higher voter turnout and increased trust in the democratic process.

Furthermore, blockchain technology is being used in the real estate industry to streamline property transactions and reduce the need for intermediaries. By using smart contracts on the blockchain, buyers and sellers can securely and efficiently transfer ownership of properties, reducing the time and cost associated with traditional real estate transactions.

Overall, the potential applications of blockchain technology are vast and varied. From supply chain management to healthcare, voting, and real estate, blockchain has the power to transform industries and improve efficiency, transparency, and security. As we continue to explore the possibilities of blockchain technology, we can expect to see even more innovative applications emerge in the years to come.

That''s all for today''s podcast. Thank you for tuning in and stay tuned for more insights on the world of blockchain technology.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRXq-aQIGkPItHgXc0Os5iBa_miDaQWJ3r4mg&s', CAST(N'2025-06-25T17:30:34.757' AS DateTime), NULL, N'Blockchain Applications Beyond Crypto', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872508/podcast_638864694313341400.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (20, N'Securing Tomorrow: Cyber Threats & Solutions', N'Hello everyone, and welcome back to our podcast series on cybersecurity. Today, we are going to discuss the challenges that we can expect to face in the year 2025.

As technology continues to advance at a rapid pace, so do the threats in the cybersecurity landscape. One of the biggest challenges that we anticipate in 2025 is the rise of sophisticated cyber attacks. With the increasing use of artificial intelligence and machine learning by cyber criminals, we can expect to see more targeted and personalized attacks that are harder to detect and defend against.

Another challenge that we will likely face in 2025 is the proliferation of connected devices. The Internet of Things (IoT) has revolutionized the way we live and work, but it has also created new vulnerabilities in our networks. As more and more devices become interconnected, the attack surface for cyber criminals expands, making it easier for them to infiltrate our systems and steal sensitive information.

Additionally, the shortage of skilled cybersecurity professionals is expected to worsen in 2025. As the demand for cybersecurity experts continues to grow, companies will struggle to find and retain qualified individuals to protect their networks. This skills gap will leave many organizations vulnerable to attacks and data breaches.

Finally, the regulatory landscape is also expected to change in 2025. With the implementation of new data protection laws and regulations, companies will need to adapt their cybersecurity practices to ensure compliance. Failure to do so could result in hefty fines and reputational damage.

In conclusion, the cybersecurity challenges that we will face in 2025 are complex and ever-evolving. It is crucial for organizations to stay ahead of the curve by investing in the latest technologies and training their employees to recognize and respond to cyber threats. By being proactive and vigilant, we can work together to secure our digital future. Thank you for listening.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTEJhXHQj-ytt0tpZq7hBZkFd83eUDsuXfMYg&s', CAST(N'2025-06-25T17:31:09.313' AS DateTime), NULL, N'Cybersecurity Challenges in 2025', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872542/podcast_638864694657351052.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (21, N'Quantum Leap: Computing Reimagined', N'Hello and welcome back to our podcast on the latest breakthroughs in quantum computing. Quantum computing has been a hot topic in the world of technology and science, with researchers making significant strides in recent years. Today, we''ll be discussing some of the most exciting breakthroughs in the field.

One of the most significant breakthroughs in quantum computing is the development of quantum supremacy. Quantum supremacy refers to the point at which a quantum computer can outperform even the most powerful supercomputers in certain tasks. In 2019, Google claimed to have achieved quantum supremacy with its 53-qubit quantum processor, Sycamore. This milestone represents a major step forward in the field of quantum computing and has opened up new possibilities for solving complex problems that were previously thought to be unsolvable.

Another exciting breakthrough in quantum computing is the development of error correction techniques. Quantum computers are extremely sensitive to errors, which can arise from factors such as noise and decoherence. Researchers have been working on developing error correction techniques that can help mitigate these errors and improve the reliability of quantum computers. By implementing error correction, quantum computers can perform more complex calculations and simulations with greater accuracy.

Furthermore, researchers have been exploring new quantum algorithms that can harness the power of quantum mechanics to solve problems more efficiently than classical algorithms. For example, Shor''s algorithm, developed by Peter Shor in 1994, demonstrated that a quantum computer could factor large numbers exponentially faster than a classical computer. This has significant implications for cybersecurity and cryptography, as it could potentially render current encryption methods obsolete.

In conclusion, quantum computing has seen numerous breakthroughs in recent years that have pushed the boundaries of what is possible with this revolutionary technology. From achieving quantum supremacy to developing error correction techniques and new quantum algorithms, researchers are making remarkable progress in advancing the field of quantum computing. As we continue to explore the potential of quantum computing, we can expect to see even more groundbreaking discoveries in the years to come. Thank you for tuning in to our podcast, and stay tuned for more updates on the latest breakthroughs in quantum computing.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR05-zUBxb6Mu0V4K47JICkbv9mUxrOo2nkEQ&s', CAST(N'2025-06-25T17:31:55.783' AS DateTime), NULL, N'Quantum Computing Breakthroughs', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872589/podcast_638864695117061690.mp3')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic], [AudioUrl]) VALUES (22, N'Urban Evolution: Building Smart Cities', N'In recent years, there has been a significant rise in the development of smart cities around the world. These cities are utilizing technology and data to improve the quality of life for their residents and create more efficient and sustainable urban environments.

One of the key features of smart cities is the use of Internet of Things (IoT) devices to collect and analyze data in real-time. This data can be used to optimize services such as public transportation, waste management, and energy consumption. For example, sensors can monitor traffic flow and adjust traffic lights accordingly to reduce congestion and improve air quality.

Smart cities also focus on creating a more connected and accessible urban environment. This includes the implementation of smart infrastructure such as smart streetlights, which can adjust their brightness based on the time of day or the presence of pedestrians. Additionally, smart cities often provide residents with digital services such as mobile apps for paying bills, reporting issues, and accessing information about local events.

Another important aspect of smart cities is sustainability. By using technology to monitor and manage resources more effectively, cities can reduce their environmental impact and create a more livable environment for their residents. For example, smart irrigation systems can help conserve water in parks and green spaces, while smart building technologies can optimize energy usage and reduce carbon emissions.

Overall, the rise of smart cities represents a major shift in urban planning and development. By harnessing the power of technology and data, cities can improve efficiency, sustainability, and quality of life for their residents. As more cities around the world embrace this trend, we can expect to see even greater advancements in the way we live, work, and interact with our urban environments.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQrYdjKa0hdp7U2R5SlQlJ4Vz2-m6FWB7YoQQ&s', CAST(N'2025-06-25T17:32:37.953' AS DateTime), NULL, N'The Rise of Smart Cities', N'https://res.cloudinary.com/dv6dsqvtp/raw/upload/v1750872632/podcast_638864695547208277.mp3')
