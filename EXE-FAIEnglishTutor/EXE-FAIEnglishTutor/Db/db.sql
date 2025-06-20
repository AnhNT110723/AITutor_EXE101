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


INSERT INTO [Podcast] ([Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic])
VALUES 
(N'Podcast 1', N'[Intro Music – Fades In]

Host (Warm tone):
Welcome to “Mastering English”, your trusted podcast for learning, loving, and living the English language. I’m your host, and today, we begin our journey together — not with grammar, not with vocabulary, but with why English is so important in today’s world.

 Segment 1: The Global Language
English is more than just a language. It’s a global bridge.

English is spoken by over 1.5 billion people worldwide — as a native, second, or foreign language. It’s the most widely studied language, the official language of aviation, international business, and technology.

Ask yourself: Why is English everywhere?

Because it connects people. A German engineer, a Japanese investor, and a Vietnamese entrepreneur might not share the same native language — but English gives them a common ground.

 Segment 2: Career Opportunities
Let’s talk careers. If you want to work for a multinational company, apply for a job abroad, or work in IT, tourism, science, or education — English opens doors.

Many of the world’s top companies, like Google, Apple, or Amazon, operate in English. Even if their offices are in Japan or Brazil, their communication often happens in English.

In fact, many job descriptions now include:
“Fluent in English required.”

So, if you improve your English, you''re not just learning a language — you''re upgrading your CV.

 Segment 3: Education and Study Abroad
Thousands of students dream of studying in countries like the US, UK, Canada, or Australia.

But here''s the thing: even universities in non-English-speaking countries offer programs taught in English. Why? Because it attracts international students.

To qualify for these programs, students often need to take standardized tests like IELTS, TOEFL, or Duolingo English Test.

So whether you’re aiming for Oxford or online courses on Coursera, English is your ticket.

 Segment 4: Entertainment and Culture
Let’s admit it — a lot of us start learning English because of music, movies, or games.

Do you like watching Netflix? Shows like Stranger Things, Friends, or Breaking Bad are much better in the original version. Subtitles help, but real understanding brings real joy.

The same goes for books. Reading Harry Potter in your language is fine — but reading it in English? That’s a magical experience of its own.

And don’t forget the internet. Over 60% of the web is in English. So if you want access to blogs, forums, tutorials, or memes — English is your best tool.

Segment 5: Travel and Connections
Imagine this: you land in a new country — let’s say Italy. You don’t speak Italian. But when you ask someone, “Excuse me, where is the train station?” — chances are, they’ll understand if you speak English.

English is the universal fallback language in most tourist destinations.

Whether you''re booking hotels, using Google Maps, or asking for help, English gives you the freedom to travel more confidently and connect more easily.', N'https://nnvh.tnus.edu.vn/uploads/files/Tailieu/learning-english-doodle-set-language-school-in-sketch-style-online-language-education-course-hand-drawn-illustration-isolated-on-white-background-vector.jpg', GETDATE(), 1, N'English'),
(N'Podcast 2', N'Nội dung podcast 2', N'https://example.com/img2.jpg', GETDATE(), 2, N'Technology'),
(N'Podcast 3', N'Nội dung podcast 3', NULL, GETDATE(), 1, N'Education'),
(N'Podcast 4', N'Nội dung podcast 4', N'https://example.com/img4.jpg', GETDATE(), NULL, N'Life'),
(N'Podcast 5', N'Nội dung podcast 5', NULL, GETDATE(), 3, N'Health'),
(N'Podcast 6', N'Nội dung podcast 6', N'https://example.com/img6.jpg', GETDATE(), NULL, N'Music'),
(N'Podcast 7', N'Nội dung podcast 7', N'https://example.com/img7.jpg', GETDATE(), 2, N'Travel'),
(N'Podcast 8', N'Nội dung podcast 8', NULL, GETDATE(), 4, N'Business'),
(N'Podcast 9', N'Nội dung podcast 9', N'https://example.com/img9.jpg', GETDATE(), 5, N'Science'),
(N'Podcast 10', N'Nội dung podcast 10', NULL, GETDATE(), NULL, N'Entertainment');