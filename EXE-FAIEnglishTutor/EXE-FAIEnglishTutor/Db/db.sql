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
(N'Nhập Vai'),(N'Học qua hình'), (N'Phát âm');


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
(N'Gọi đặt lịch khám', N'Sáng nay ngủ dậy bạn nhức đầu và đau họng kinh khủng. Bạn cần đi bác sĩ khám bệnh và lấy thuốc. Hãy gọi cho phòng khám để đặt lịch nhé!', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a64-78f8-663a-be94-0242ac110002-1723465666.png', N'Y tá', N'Người bệnh',1, 1),

(N'Trẻ bị ho kéo dài', N'Trẻ bị ho nhiều ngày không khỏi, có thể do viêm phế quản hoặc viêm họng.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/9/1ef81600-b2a1-659c-b6f8-0242ac110005-1727943497.png',N'Y tá', N'Người bệnh',1, 2),

(N'Trẻ bị tiêu chảy', N'Trẻ đi ngoài nhiều lần, phân lỏng, cần tránh mất nước.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-21c3-622e-aa19-0242ac110002-1723465683.png',N'Y tá', N'Người bệnh',1, 2),

(N'Trẻ có dấu hiệu dị ứng', N'Xuất hiện phát ban hoặc ngứa sau khi ăn hoặc tiếp xúc với dị nguyên.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-4b1e-62cc-8ab5-0242ac110002-1723465688.png',N'Y tá', N'Người bệnh',1, 3),

(N'Trẻ bị chấn thương nhẹ', N'Trẻ bị té ngã, trầy xước nhẹ, cần sát trùng và theo dõi.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/7/1ef58a65-7f9b-68f6-a031-0242ac110002-1723465693.png',N'Y tá', N'Người bệnh',1, 5);



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
(2, 1, '2025-04-21 09:00:00', '2025-04-21 10:30:00', 650);


INSERT INTO UserAnswer (ResultID, QuestionID, SelectedAnswerID, IsCorrect) VALUES
(2, 1, 2, 1),
(2, 2, 4, 1),
(2, 3, 6, 1);



