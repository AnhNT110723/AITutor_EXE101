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
	LevelID Int foreign key references [Level](LevelID),
	LearningObjectives NVARCHAR(MAX),
    [RowVersion] ROWVERSION NOT NULL
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
(N'Gọi đặt lịch khám', N'Sáng nay ngủ dậy bạn nhức đầu và đau họng kinh khủng. Bạn cần đi bác sĩ khám bệnh và lấy thuốc. Hãy gọi cho phòng khám để đặt lịch nhé!', N'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673763/roleplay/goi-dat-lich-kham.jpg', N'Y tá', N'Người bệnh',1, 1)



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
INSERT [dbo].[Situation] ([SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID], [LearningObjectives]) VALUES 
(N'Booking a Doctor Appointment', N'You woke up this morning with a terrible headache and sore throat. You need to visit the doctor for an examination and medication. Call the clinic to make an appointment!', N'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673763/roleplay/goi-dat-lich-kham.jpg', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 1, N''),

(N'Child with Persistent Cough', N'The child has been coughing for several days, possibly due to bronchitis or a sore throat.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/9/1ef81600-b2a1-659c-b6f8-0242ac110005-1727943497.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 2, N''),

(N'Child with Diarrhea', N'The child has frequent loose stools and needs to avoid dehydration.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-21c3-622e-aa19-0242ac110002-1723465683.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 2, N''),

(N'Child Showing Allergy Symptoms', N'Rash or itching appeared after eating or exposure to an allergen.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-4b1e-62cc-8ab5-0242ac110002-1723465688.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 3, N''),

(N'Child with Minor Injury', N'The child fell and got a minor scrape. Needs disinfection and monitoring.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/7/1ef58a65-7f9b-68f6-a031-0242ac110002-1723465693.png', N'Nurse', N'Patient', CAST(N'2025-06-05T21:34:16.687' AS DateTime), 1, 5, N''),

(N'Greetings and Introductions', N'Basic greetings and self-introduction for beginners.', N'https://i.ytimg.com/vi/KKh_CallEp8/maxresdefault.jpg', N'Hello! What''s your name?', N'My name is Anna.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 1, N''),

(N'Daily Routines', N'Listening to simple daily routines.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT5AvLdE1tXqkMwbt6bbKNytBDxRdl0mdXswA&s', N'What do you do every morning?', N'I brush my teeth and have breakfast.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 1, N''),

(N'Ordering Food', N'Listening to restaurant conversations.', N'https://www.thoughtco.com/thmb/5I9sp2N2xSRZ7ugZu1crYb-19QE=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/beginner-dialogues-at-a-restaurant-1210039_FINAL-587dc04424dc4a12897f120a57bed771.png', N'May I take your order?', N'I would like a hamburger, please.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 2, N''),

(N'Asking for Directions', N'Simple street direction dialogues.', N'https://www.shutterstock.com/image-vector/local-man-help-travelers-couple-600nw-2161932825.jpg', N'Where is the post office?', N'Turn left at the corner.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 2, N''),

(N'Travel Planning', N'Planning a trip and making reservations.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSPGQ7V1Vm3i_cJBTZ6wezDcttEXkmXzIaOLQ&s', N'When would you like to fly?', N'Next Monday, please.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 3, N''),

(N'Shopping Conversation', N'Dialogues in shops or markets.', N'https://i.ytimg.com/vi/4kr-TKEEUH0/maxresdefault.jpg', N'How much is this shirt?', N'It''s twenty dollars.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 3, N''),

(N'Job Interview', N'Listening to job interview scenarios.', N'https://eaog2nkqckp.exactdn.com/wp-content/uploads/2022/12/6287e12157f45e15bd788725_nervous-job-interview.png?strip=all&lossy=1&ssl=1', N'What are your strengths?', N'I am very organized and responsible.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 4, N''),

(N'Medical Appointment', N'Dialogue in a clinic setting.', N'https://img.freepik.com/premium-vector/medical-appointment-with-happy-doctor-character-flat-design-illustration_692379-39.jpg', N'What symptoms do you have?', N'I have a sore throat and fever.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 4, N''),

(N'News Report', N'Listening to a short news broadcast.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRPOogpwPKwSTLJ06xpopxxGU_2xTACOk2jug&s', N'Today''s headline: heavy rain causes flooding.', N'This has affected over 500 homes.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 5, N''),

(N'Environmental Issues Discussion', N'Discussion on climate change.', N'https://media.geeksforgeeks.org/wp-content/uploads/20230620175013/Environmental-Issues-.webp', N'What can we do to reduce pollution?', N'We should use less plastic and drive less.', CAST(N'2025-06-09T18:58:18.543' AS DateTime), 2, 5, N'');

GO


USE [FAI_ENGLISH]
GO
SET IDENTITY_INSERT [dbo].[Podcast] ON 

INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (1, N'Podcast 1', N'Hello and welcome to the Vietnam Weekly News Roundup your goto source for the latest developments across Vietnam I’m your host and here’s what’s been happening this past week

Top Headlines
1 Economic Recovery Continues
Vietnams economy is showing strong signs of recovery as the government reported a GDP growth of 56 percent in Q2 of 2025 The manufacturing and export sectors remain the primary drivers with key industries such as textiles electronics and seafood seeing increasing demand from foreign markets

2 Education Reforms Announced
The Ministry of Education and Training has unveiled a plan to revamp the national high school curriculum starting in 2026 This reform aims to focus more on digital literacy critical thinking and soft skills aligning Vietnams education system with global trends Pilot programs will be launched in 12 provinces later this year

3 Tech Startups Thriving in Hanoi and Ho Chi Minh City
Vietnams startup ecosystem continues to gain momentum In the last month three Vietnamese tech startups raised over 50 million USD in seed and Series A funding Notably a Hanoi based AI company has partnered with Japanese investors to develop smart city infrastructure

4 Flood Warnings in Central Vietnam
Torrential rain in central provinces like Quang Nam and Quang Ngai has led to flash flooding in some rural areas Authorities are warning residents to evacuate low lying areas while rescue teams have been dispatched to provide assistance Over 3000 people have been temporarily relocated

5 National Football Team Prepares for World Cup Qualifiers
Vietnams national football team is training intensively ahead of the upcoming FIFA World Cup qualifiers Head coach Philippe Troussier has called up several young talents from the U23 squad raising hopes for a competitive performance in the Asian zone

Closing Thoughts
That wraps up this weeks highlights from across Vietnam From economic progress and education reforms to environmental challenges and football fever Vietnam continues to evolve on many fronts Dont forget to subscribe to stay informed every week

Thanks for listening and until next time take care and stay updated', N'https://www.science.edu/acellus/wp-content/uploads/2016/12/Discover-English-HS-Part-1-712x388-712x388.jpg', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 1, N'English')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (2, N'Podcast 2', N'TechBit Podcast: Episode - Technology Trends in 2025
Intro Music: Upbeat electronic track fades in and out
Host: Hey everyone, welcome to TechBit, your bite-sized dive into the world of technology! I’m your host, Alex, and today we’re exploring some of the hottest tech trends shaping 2025. So, grab your coffee, put on those headphones, and let’s get started!
Segment 1: AI EverywhereFirst up, artificial intelligence is no longer just a buzzword—it’s practically running the show. From smarter virtual assistants to AI-driven healthcare diagnostics, 2025 is the year AI gets even more personal. Imagine your fridge suggesting recipes based on what’s inside or your car predicting traffic jams before you even leave the house. Companies are integrating AI into everything, making our lives more connected but also raising questions about privacy. Are we ready to share that much data with our devices? Something to think about.
Segment 2: The Rise of Quantum ComputingNext, let’s talk quantum computing. It sounds like sci-fi, but it’s getting real. In 2025, we’re seeing more breakthroughs in quantum tech, with companies like IBM and Google pushing the boundaries. These machines could solve problems in seconds that would take traditional computers years—like cracking complex encryption or designing new drugs. But don’t expect a quantum laptop on your desk just yet; we’re still in the early days. Exciting, right?
Segment 3: Sustainable TechNow, let’s get to something super important: sustainability. Green tech is booming in 2025. From solar panels that are twice as efficient to biodegradable electronics, innovators are tackling climate change head-on. I read about a startup making phone batteries from recycled materials—how cool is that? It’s not just about saving the planet; it’s about building tech that lasts without trashing the environment.
Segment 4: The Metaverse and BeyondAnd of course, we can’t skip the metaverse. Virtual reality and augmented reality are blending work, play, and socializing in ways we couldn’t imagine a few years ago. In 2025, expect more immersive virtual worlds for everything from meetings to concerts. But let’s be real—sometimes I just want to log off and touch some grass, you know?
OutroThat’s all for this episode of TechBit! What do you think about these trends? Are you excited for quantum leaps or worried about AI knowing too much? Drop us a message on social media, and let’s keep the convo going. Until next time, stay curious and stay techy!
Outro Music: Fades out
Produced by Alex at TechBit Studios. Recorded on June 21, 2025.
', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVLRyHwRo12aVkMCkwxrb81rX5P0HAZXyp_Q&s', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 2, N'Technology')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (3, N'Podcast 3', N'EduTalk Podcast: Episode - Education Trends in 2025
Intro Music: Bright, uplifting acoustic track fades in and out
Host: Hello, everyone, and welcome to EduTalk, your go-to podcast for all things education! I’m your host, Sarah, and today we’re diving into the exciting world of education trends shaping 2025. Whether you’re a student, teacher, or just curious about learning, stick around—let’s explore what’s next! 
Segment 1: Personalized Learning Powered by AIFirst up, personalized learning is taking center stage. Thanks to AI, education is becoming more tailored than ever. In 2025, platforms are using artificial intelligence to create custom learning paths for students, adapting to their strengths and weaknesses in real time. Imagine a math app that knows exactly where you’re struggling and serves up the perfect practice problem. It’s like having a personal tutor 24/7. But here’s the catch—how do we balance tech with human connection in the classroom? Something to ponder.
Segment 2: The Rise of Micro-CredentialsNext, let’s talk about micro-credentials. Forget the traditional four-year degree for a moment—2025 is all about bite-sized, skill-specific certifications. From coding bootcamps to digital marketing badges, these micro-credentials are helping people upskill fast for today’s job market. Employers love them, and learners can stack them like building blocks. I saw a stat that 60% of workers plan to pursue one this year. Are we moving toward a world where skills trump degrees? Maybe!
Segment 3: Immersive Learning with VR and ARNow, here’s something straight out of a sci-fi movie: virtual and augmented reality in education. In 2025, VR and AR are transforming how we learn. Picture students exploring ancient Rome in a virtual field trip or medical students practicing surgeries in a simulated environment. It’s engaging, hands-on, and honestly, pretty fun. The challenge? Making this tech accessible to schools with tight budgets. Equity in education is still a big hurdle.
Segment 4: Focus on Lifelong LearningFinally, lifelong learning is no longer optional—it’s a must. With technology and jobs evolving so fast, 2025 is seeing a boom in adult education programs. From online courses to community workshops, people of all ages are hitting the books—or screens—to stay relevant. It’s inspiring to see grandparents learning to code alongside college students. Learning never stops, and that’s the beauty of it!
OutroThat’s a wrap for this episode of EduTalk! What’s your take on these trends? Are you excited about VR classrooms or all-in on micro-credentials? Let us know on social media—we’d love to hear from you. Until next time, keep learning and stay curious!
Outro Music: Fades out
Produced by Sarah at EduTalk Studios. Recorded on June 21, 2025.
', N'https://cdn.unicaf.org/websites/unicaf/wp-content/uploads/2021/07/education-large.jpg', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 1, N'Education')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (4, N'Podcast 4', N'LifeVibes Podcast: Episode - Living Well in 2025
Intro Music: Warm, soothing acoustic melody fades in and out
Host: Hey there, welcome to LifeVibes, the podcast where we dive into the messy, beautiful, and real moments of life! I’m your host, Emma, and today we’re talking about what it means to live well in 2025. So, grab a cozy drink, settle in, and let’s get into it!
Segment 1: Finding Balance in a Hyper-Connected WorldFirst off, let’s talk about balance. In 2025, we’re more connected than ever—phones buzzing, smartwatches tracking our every step, and social media never sleeping. But here’s the thing: so many of us are craving a digital detox. People are setting boundaries, like no-screen Sundays or mindfulness apps that remind you to breathe. I tried a 24-hour phone-free day last week, and wow, it was like rediscovering the world. Are you finding ways to unplug and just be?
Segment 2: The Power of Small JoysNext up, let’s celebrate the small stuff. Life in 2025 can feel overwhelming with work, goals, and global news, but there’s magic in the little moments. Think morning coffee rituals, a walk with your dog, or laughing over a silly meme with a friend. I read about a trend where people keep “joy journals” to jot down one happy moment each day. I started one, and it’s honestly a game-changer. What’s a small joy that’s sparked your day lately?
Segment 3: Building CommunityNow, let’s talk community. In 2025, people are rethinking connection. With remote work still huge, folks are seeking out local meetups, hobby groups, or even virtual book clubs to feel grounded. There’s something special about sharing stories with people who get you. I joined a local gardening club this year, and even though I’m terrible with plants, the friendships I’ve made are blooming. Where do you find your sense of belonging?
Segment 4: Embracing ChangeFinally, let’s get real about change. Life in 2025 is full of it—new tech, new opportunities, new challenges. Whether it’s a career pivot, moving to a new city, or just figuring out who you are, embracing change is key. I’ve learned that it’s okay to feel scared but still take the leap. One quote I love is, “You don’t have to see the whole staircase, just take the first step.” What’s one change you’re navigating right now?
OutroThat’s it for this episode of LifeVibes! I hope you’re feeling inspired to find balance, chase small joys, connect with others, and embrace life’s twists and turns. Let us know what living well means to you—hit us up on social media. Until next time, keep vibing and living your best life!
Outro Music: Fades out
Produced by Emma at LifeVibes Studios. Recorded on June 21, 2025.
', N'https://static.toiimg.com/thumb/msid-112543392,width-1280,height-720,resizemode-4/112543392.jpg', CAST(N'2025-06-20T09:29:08.780' AS DateTime), NULL, N'Life')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (5, N'Podcast 5', N'HealthPulse Podcast: Episode - Healthy Living in 2025
Intro Music: Energetic, uplifting pop track fades in and out
Host: Hello, everyone, and welcome to HealthPulse, your podcast for all things wellness! I’m your host, Mia, and today we’re diving into how to live your healthiest life in 2025. Whether you’re a fitness buff or just starting your wellness journey, this episode’s for you. Let’s get moving!
Segment 1: Mental Health Takes Center StageFirst up, mental health is finally getting the attention it deserves. In 2025, we’re seeing more apps and tools designed to reduce stress and boost mindfulness. From AI-guided meditation to virtual therapy sessions, it’s easier than ever to prioritize your mind. I’ve been using a gratitude app that prompts me to reflect daily, and it’s honestly so grounding. But let’s be real—sometimes it’s just about taking five minutes to breathe. What’s your go-to for mental wellness?
Segment 2: Food as MedicineNext, let’s talk nutrition. The “food as medicine” movement is huge in 2025. People are swapping processed snacks for whole, nutrient-packed foods. Think gut-friendly fermented dishes like kimchi or plant-based meals that don’t skimp on flavor. There’s even a trend of personalized diets based on your DNA—wild, right? I tried a week of eating mostly plants, and I felt so energized. Small swaps can make a big difference. What’s one healthy food you’re loving lately?
Segment 3: Fitness for EveryoneNow, onto fitness. In 2025, it’s all about inclusive movement. Forget intimidating gyms—people are embracing activities like dance workouts, hiking clubs, or even VR fitness games. The focus is on fun, not perfection. I joined a local walking group, and it’s less about burning calories and more about laughing with new friends. The best part? There’s something for every body and every schedule. What’s a fun way you’re staying active?
Segment 4: Sleep is the New SuperpowerFinally, let’s talk sleep. In 2025, sleep is being called the ultimate health hack. Wearable tech is helping us track sleep patterns, and experts are pushing for better “sleep hygiene”—like dimming lights and ditching screens before bed. I started a no-phone-in-the-bedroom rule, and wow, I wake up feeling like a new person. Quality sleep boosts your mood, energy, even your immune system. Are you team early bird or night owl? How’s your sleep game?
OutroThat’s a wrap for this episode of HealthPulse! I hope you’re inspired to nurture your mind, fuel your body, move with joy, and catch those Z’s. Drop us a message on social media—what’s your health goal for 2025? Until next time, stay healthy and keep pulsing!
Outro Music: Fades out
Produced by Mia at HealthPulse Studios. Recorded on June 21, 2025.
', N'https://shanhealth.vn/wp-content/uploads/2023/11/mental-health-la-gi-1.webp', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 3, N'Health')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (6, N'Podcast 6', N'SoundWave Podcast: Episode - Music Trends in 2025
Intro Music: Catchy, upbeat indie pop track fades in and out
Host: Hey, music lovers, welcome to SoundWave, the podcast that hits all the right notes! I’m your host, Liam, and today we’re diving into the vibrant world of music in 2025. From new sounds to tech-driven tunes, let’s crank up the volume and explore what’s trending. Ready? Let’s go!
Segment 1: AI and Music CreationFirst up, artificial intelligence is shaking up the music scene. In 2025, AI tools are helping artists—and even fans—create original tracks in minutes. From generating beats to writing lyrics, these tools are like a co-producer in your pocket. I messed around with an AI music app last week, and I made a decent lo-fi track—me, who can barely play a chord! But here’s the question: can AI capture the soul of music, or is it just a cool gimmick? What do you think?
Segment 2: The Revival of Live MusicNext, live music is back with a vengeance. After years of virtual concerts, 2025 is all about in-person experiences. Festivals are popping up everywhere, and small venues are packed with fans craving that raw energy. There’s even a trend of “silent discos” where everyone’s dancing with wireless headphones. I went to one, and it’s wild seeing people groove to different tracks in the same room. Nothing beats the vibe of live music, right? When’s the last time you caught a show?
Segment 3: Genre MashupsNow, let’s talk genres. In 2025, boundaries are blurring like never before. Think hip-hop fused with classical, or country mixed with EDM. Artists are experimenting, and fans are loving it. I’ve been obsessed with this new afrobeat-jazz fusion—it’s like a party in my headphones. Streaming platforms are making it easier to discover these cross-genre gems. What’s a wild music combo you’ve come across lately?
Segment 4: Music for WellnessFinally, music is becoming a go-to for mental health. In 2025, curated playlists for relaxation, focus, or even grief are everywhere. There’s science behind it—certain beats can lower stress or boost your mood. I’ve been using a “deep focus” playlist for work, and it’s like my brain’s on turbo. Plus, sound baths and music therapy sessions are trending. It’s amazing how a song can feel like a hug sometimes. What’s a track that lifts you up?
OutroThat’s it for this episode of SoundWave! I hope you’re as pumped about music in 2025 as I am. Whether you’re creating with AI, dancing at a festival, or chilling with a playlist, keep the music flowing. Hit us up on social media—what’s on your playlist right now? Until next time, stay tuned and keep vibing!
Outro Music: Fades out
Produced by Liam at SoundWave Studios. Recorded on June 21, 2025.
', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRY1iHEtGpPZPkn7NyTN-MvFW936EUuBZ-vVA&s', CAST(N'2025-06-20T09:29:08.780' AS DateTime), NULL, N'Music')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (7, N'Podcast 7', N'Wanderlust Podcast: Episode - Travel Trends in 2025
Intro Music: Upbeat, tropical-inspired track fades in and out
Host: Hello, explorers, and welcome to Wanderlust, the podcast that fuels your travel dreams! I’m your host, Zoe, and today we’re packing our bags to dive into the exciting world of travel in 2025. From new destinations to game-changing tech, let’s hit the road—or the skies—and see what’s trending. Ready? Let’s go!
Segment 1: Sustainable Travel Takes OffFirst up, sustainability is the name of the game in 2025. Travelers are prioritizing eco-friendly adventures, like staying in solar-powered hotels or joining tours that support local communities. There’s a big push for low-carbon travel—think electric planes and trains over short flights. I stayed at an eco-lodge last month, and it was amazing to see how luxury and green living can coexist. How are you making your travels more sustainable?
Segment 2: Off-the-Beaten-Path DestinationsNext, let’s talk about where we’re going. In 2025, travelers are skipping the usual hotspots for hidden gems. Places like Bhutan, Montenegro, or even lesser-known islands in the Pacific are stealing the spotlight. These spots offer authentic experiences without the crowds. I’m dreaming of hiking in Georgia—the country, not the state—with its stunning mountains and rich culture. What’s a destination on your radar that’s a bit under-the-radar?
Segment 3: Tech-Enhanced TravelNow, technology is transforming how we explore. In 2025, augmented reality apps are turning city tours into immersive history lessons—imagine seeing ancient ruins rebuilt through your phone. AI travel planners are also huge, curating perfect itineraries based on your vibe. I used one to plan a weekend trip, and it nailed every restaurant pick. But sometimes, I miss the chaos of figuring it out on the fly. Are you team tech or old-school maps?
Segment 4: Slow Travel MovementFinally, slow travel is stealing hearts. Instead of rushing through checklists, people are spending weeks or months in one place—really soaking in the culture. Think renting a villa in Tuscany or volunteering in a coastal village. It’s about connection over conquest. I tried slow travel in a small town last year, and learning to cook local dishes with my host family was unforgettable. Where would you love to slow down and stay awhile?
OutroThat’s a wrap for this episode of Wanderlust! I hope you’re itching to plan your next adventure, whether it’s green, offbeat, techy, or slow. Drop us a line on social media—what’s your dream trip for 2025? Until next time, keep wandering and stay curious!
Outro Music: Fades out
Produced by Zoe at Wanderlust Studios. Recorded on June 21, 2025.
', N'https://i.scdn.co/image/ab6765630000ba8a8f61e01a7fadcbdd1dbfdad6', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 2, N'Travel')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (8, N'Podcast 8', N'BizBuzz Podcast: Episode - Business Trends in 2025
Intro Music: Dynamic, modern corporate track fades in and out
Host: Hey everyone, welcome to BizBuzz, the podcast that keeps you ahead in the world of business! I’m your host, Ryan, and today we’re diving into the top business trends shaping 2025. Whether you’re an entrepreneur, a corporate pro, or just curious about the market, this episode’s got you covered. Let’s jump in!
Segment 1: AI-Driven Decision MakingFirst up, artificial intelligence is revolutionizing how businesses operate. In 2025, AI isn’t just for tech giants—it’s helping small businesses predict customer trends, optimize supply chains, and even personalize marketing in real time. I know a local café that uses AI to tweak its menu based on what’s selling. It’s smart, but it makes me wonder: are we relying too much on algorithms over gut instinct? What’s your take?
Segment 2: The Gig Economy EvolvesNext, the gig economy is getting a major upgrade. In 2025, freelancers and contract workers are powering everything from graphic design to AI consulting. Platforms are popping up to connect businesses with specialized talent for short-term projects. It’s flexible for workers and cost-effective for companies. I hired a freelance strategist for a project last month, and the results were phenomenal. Have you tapped into the gig economy yet?
Segment 3: Sustainability as a Business MustNow, let’s talk sustainability. It’s no longer optional—customers and investors are demanding eco-conscious practices. In 2025, businesses are going green with circular supply chains, like recycling materials to make new products, or carbon-neutral offices. I saw a startup making packaging from seaweed—innovative and planet-friendly. Going green isn’t just good ethics; it’s good business. How’s your workplace stepping up?
Segment 4: Hybrid Work Here to StayFinally, hybrid work is shaping the future of business. In 2025, companies are blending remote and in-office work to keep employees happy and productive. Think flexible schedules and virtual collaboration tools that feel like you’re in the same room. My friend’s company just redesigned their office for “collaboration hubs” instead of cubicles. It’s a vibe shift! How’s hybrid work changing your business game?
OutroThat’s all for this episode of BizBuzz! I hope you’re fired up about the opportunities in 2025, from AI smarts to sustainable wins. Drop us a message on social media—what’s the biggest business trend you’re watching? Until next time, stay sharp and keep buzzing!
Outro Music: Fades out
Produced by Ryan at BizBuzz Studios. Recorded on June 21, 2025.
', N'https://hub.edu.vn/DATA/IMAGES/2021/07/18/20210718200309-1472e-business1-1.jpg', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 4, N'Business')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (9, N'Podcast 9', N'ScienceSpark Podcast: Episode - Science Trends in 2025
Intro Music: Futuristic, upbeat synth track fades in and out
Host: Hey there, science enthusiasts, and welcome to ScienceSpark, the podcast that ignites your curiosity about the universe! I’m your host, Nora, and today we’re diving into the mind-blowing world of science in 2025. From breakthroughs to big ideas, let’s explore what’s sparking excitement. Ready? Let’s dive in!
Segment 1: CRISPR and Gene Editing AdvancesFirst up, gene editing is making waves. In 2025, CRISPR technology is pushing boundaries beyond curing diseases. Scientists are now tweaking crops to survive climate change and even exploring ways to bring back extinct species—think de-extinction projects for animals like the dodo. I read about a lab growing drought-resistant rice, which could be a game-changer for food security. But it raises questions: where do we draw the line with editing life? What’s your take?
Segment 2: Space Exploration Gets PersonalNext, space is closer than ever. In 2025, private companies are making space travel more accessible—not just for astronauts but for civilians too. We’re talking lunar tourism and plans for Mars habitats. Meanwhile, new telescopes are revealing exoplanets that might host life. I can’t stop thinking about what it’d be like to stargaze from the moon. Are you ready to book a ticket to space, or are you keeping your feet on Earth?
Segment 3: Green Energy BreakthroughsNow, let’s talk energy. Science in 2025 is all about sustainable power. Fusion energy—mimicking the sun’s power—is getting closer to reality, with prototypes showing promise. Plus, advances in solar and wind tech are making renewable energy cheaper than ever. I saw a report about a new battery that charges in minutes and lasts for days. It’s exciting, but scaling it up is the challenge. What green tech are you rooting for?
Segment 4: The Brain-Machine ConnectionFinally, the frontier of brain-machine interfaces is blowing my mind. In 2025, scientists are developing tech that lets your brain control devices—like typing with your thoughts or helping people with paralysis move again. It’s straight out of sci-fi, but it’s real. I tried a basic brainwave headset for a meditation app, and it was wild. The potential is huge, but so are the ethical questions. How far should we go with merging minds and machines?
OutroThat’s it for this episode of ScienceSpark! I hope you’re as thrilled about these scientific leaps as I am. Whether it’s editing genes, exploring space, saving the planet, or wiring our brains, 2025 is full of possibilities. Let us know on social media—what scientific topic sparks your curiosity? Until next time, stay curious and keep exploring!
Outro Music: Fades out
Produced by Nora at ScienceSpark Studios. Recorded on June 21, 2025.
', N'https://nativespeaker.vn/uploaded/page_1294_1688782864_1692888675.jpg', CAST(N'2025-06-20T09:29:08.780' AS DateTime), 5, N'Science')
INSERT [dbo].[Podcast] ([Id], [Title], [Content], [ImageUrl], [CreatedAt], [UserId], [Topic]) VALUES (10, N'Podcast 10', N'PopVibe Podcast: Episode - Entertainment Trends in 2025
Intro Music: Upbeat, cinematic pop track fades in and out
Host: Hey, everyone, welcome to PopVibe, the podcast that’s all about the pulse of entertainment! I’m your host, Ava, and today we’re diving into the dazzling world of entertainment in 2025. From movies to gaming to live events, we’ve got the scoop on what’s hot. So, grab your popcorn and let’s get this show started!
Segment 1: Immersive Storytelling in Film and TVFirst up, storytelling is getting a major upgrade. In 2025, films and TV shows are using augmented reality and interactive formats to pull you right into the story. Imagine choosing how a movie ends or walking through a scene with VR goggles. I tried an interactive sci-fi show last week, and it felt like I was part of the crew on a spaceship. It’s thrilling, but I wonder if it’ll make us miss good old-fashioned couch binging. What’s your favorite way to watch?
Segment 2: Gaming Goes Hyper-SocialNext, gaming is more than just play—it’s a social universe. In 2025, games are blending with social platforms, letting players hang out, create, and compete in virtual worlds. Think massive online festivals with live DJs or games where you design your own levels with friends. I got hooked on a game where my crew built a virtual city together—it’s like a party with a controller. What game’s got you glued to the screen these days?
Segment 3: Live Events Get a Tech TwistNow, let’s talk live entertainment. Concerts and theater in 2025 are blending tech and IRL magic. Holographic performances are huge—imagine seeing a legendary artist “live” years after they’ve retired. Plus, festivals are using AI to personalize your experience, like suggesting the perfect band based on your vibe. I went to a concert with real-time light shows synced to my phone, and it was next-level. What’s the best live event you’ve been to?
Segment 4: Short-Form Content RulesFinally, short-form content is still king. In 2025, platforms like X are buzzing with bite-sized videos—think 60-second comedies, tutorials, or mini-dramas. Creators are getting super creative, and fans are eating it up. I laughed so hard at a viral skit about AI roommates the other day—it’s wild how much story fits in a minute. But sometimes, I crave a long movie to unwind. Are you team short clips or long-form vibes?
OutroThat’s a wrap for this episode of PopVibe! I hope you’re pumped about the entertainment scene in 2025, from immersive stories to virtual worlds. Drop us a message on social media—what’s keeping you entertained right now? Until next time, keep vibing and stay in the spotlight!
Outro Music: Fades out
Produced by Ava at PopVibe Studios. Recorded on June 21, 2025.
', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQlW3aExD49xyzn--wIo48UbsVOBy9LX8YOsQ&s', CAST(N'2025-06-20T09:29:08.780' AS DateTime), NULL, N'Entertainment')
SET IDENTITY_INSERT [dbo].[Podcast] OFF
GO
-- =============================================
-- UPDATE ImageUrl cho tất cả Situation -> Cloudinary
-- =============================================
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673763/roleplay/goi-dat-lich-kham.jpg' WHERE SituationID = 1;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673765/roleplay/asking-for-directions.jpg' WHERE SituationID = 2;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673766/roleplay/discussing-work-project.jpg' WHERE SituationID = 3;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673770/roleplay/presenting-business-idea.jpg' WHERE SituationID = 4;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673774/roleplay/job-interview.jpg' WHERE SituationID = 5;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673776/roleplay/booking-hotel-room.jpg' WHERE SituationID = 6;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673966/roleplay/complaining-product.jpg' WHERE SituationID = 7;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673782/roleplay/resolving-workplace-dispute.jpg' WHERE SituationID = 8;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673803/roleplay/ordering-food-restaurant.jpg' WHERE SituationID = 9;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673809/roleplay/pitching-to-investors.jpg' WHERE SituationID = 10;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673816/roleplay/negotiating-contract.jpg' WHERE SituationID = 11;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673826/roleplay/booking-movie-tickets.jpg' WHERE SituationID = 12;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673828/roleplay/consulting-business-strategy.jpg' WHERE SituationID = 13;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673834/roleplay/booking-doctor-appointment.jpg' WHERE SituationID = 14;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673836/roleplay/child-persistent-cough.jpg' WHERE SituationID = 15;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673839/roleplay/child-with-diarrhea.jpg' WHERE SituationID = 16;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673856/roleplay/child-allergy-symptoms.jpg' WHERE SituationID = 17;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673874/roleplay/child-minor-injury.jpg' WHERE SituationID = 18;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673877/roleplay/greetings-introductions.jpg' WHERE SituationID = 19;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673882/roleplay/daily-routines.jpg' WHERE SituationID = 20;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673885/roleplay/ordering-food-cafe.jpg' WHERE SituationID = 21;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673890/roleplay/asking-directions-street.jpg' WHERE SituationID = 22;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673892/roleplay/travel-planning.jpg' WHERE SituationID = 23;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673894/roleplay/shopping-conversation.jpg' WHERE SituationID = 24;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673897/roleplay/job-interview-office.jpg' WHERE SituationID = 25;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673899/roleplay/medical-appointment.jpg' WHERE SituationID = 26;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673901/roleplay/news-report.jpg' WHERE SituationID = 27;
UPDATE [Situation] SET ImageUrl = 'https://res.cloudinary.com/dvzbqymu8/image/upload/v1787673992/roleplay/environmental-discussion.jpg' WHERE SituationID = 28;
GO

-- Cập nhật LearningObjectives (Mục tiêu bài học) cho các Tình huống
USE FAI_ENGLISH;
GO

UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Biết cách đặt lịch hẹn"", ""Nắm từ vựng chủ đề sức khỏe"", ""Luyện tập cấu trúc \"I want to...\" ""]' WHERE [SituationName] = N'Booking a Doctor Appointment';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Mô tả triệu chứng ho của trẻ"", ""Học từ vựng về bệnh hô hấp"", ""Luyện nghe lời khuyên y tế""]' WHERE [SituationName] = N'Child with Persistent Cough';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Mô tả triệu chứng tiêu hóa"", ""Hỏi về cách phòng ngừa mất nước"", ""Nắm từ vựng y tế nhi khoa""]' WHERE [SituationName] = N'Child with Diarrhea';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Báo cáo triệu chứng dị ứng"", ""Hỏi về cách điều trị khẩn cấp"", ""Từ vựng về thực phẩm gây dị ứng""]' WHERE [SituationName] = N'Child Showing Allergy Symptoms';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Giải thích nguyên nhân chấn thương"", ""Hỏi cách sát trùng và băng bó"", ""Luyện cấu trúc diễn đạt sự cố""]' WHERE [SituationName] = N'Child with Minor Injury';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Biết cách chào hỏi cơ bản"", ""Giới thiệu bản thân"", ""Thực hành cấu trúc \"My name is...\" ""]' WHERE [SituationName] = N'Greetings and Introductions';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Mô tả thói quen hàng ngày"", ""Nắm từ vựng chỉ thời gian"", ""Thực hành cấu trúc \"I usually...\" ""]' WHERE [SituationName] = N'Daily Routines';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Cách gọi món tại nhà hàng"", ""Từ vựng về thực đơn"", ""Cấu trúc \"I would like...\" ""]' WHERE [SituationName] = N'Ordering Food';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Cách hỏi đường cơ bản"", ""Từ vựng chỉ phương hướng"", ""Cấu trúc \"Where is the...\" ""]' WHERE [SituationName] = N'Asking for Directions';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Lên kế hoạch chuyến đi"", ""Từ vựng về phương tiện giao thông"", ""Cách đặt vé máy bay""]' WHERE [SituationName] = N'Travel Planning';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Cách hỏi giá cả"", ""Từ vựng về quần áo và mặc cả"", ""Cấu trúc \"How much is this...\" ""]' WHERE [SituationName] = N'Shopping Conversation';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Cách giới thiệu điểm mạnh"", ""Từ vựng môi trường công sở"", ""Luyện nghe câu hỏi phỏng vấn""]' WHERE [SituationName] = N'Job Interview';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Mô tả triệu chứng cá nhân"", ""Từ vựng về các bệnh thông thường"", ""Giao tiếp cơ bản tại phòng khám""]' WHERE [SituationName] = N'Medical Appointment';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Luyện nghe hiểu tin tức"", ""Từ vựng về sự kiện và xã hội"", ""Nắm bắt ý chính của bản tin""]' WHERE [SituationName] = N'News Report';
UPDATE [dbo].[Situation] SET [LearningObjectives] = N'[""Thảo luận về biến đổi khí hậu"", ""Từ vựng về bảo vệ môi trường"", ""Cấu trúc đề xuất giải pháp \"We should...\" ""]' WHERE [SituationName] = N'Environmental Issues Discussion';
GO
