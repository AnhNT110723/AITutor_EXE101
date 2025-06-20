USE [master]
GO
/****** Object:  Database [FAI_ENGLISH]    Script Date: 6/8/2025 10:11:34 PM ******/
CREATE DATABASE [FAI_ENGLISH]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FAI_ENGLISH', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.QUYENDZ\MSSQL\DATA\FAI_ENGLISH.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FAI_ENGLISH_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.QUYENDZ\MSSQL\DATA\FAI_ENGLISH_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [FAI_ENGLISH] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FAI_ENGLISH].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FAI_ENGLISH] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET ARITHABORT OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [FAI_ENGLISH] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FAI_ENGLISH] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FAI_ENGLISH] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET  ENABLE_BROKER 
GO
ALTER DATABASE [FAI_ENGLISH] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FAI_ENGLISH] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [FAI_ENGLISH] SET  MULTI_USER 
GO
ALTER DATABASE [FAI_ENGLISH] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FAI_ENGLISH] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FAI_ENGLISH] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FAI_ENGLISH] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FAI_ENGLISH] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FAI_ENGLISH] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [FAI_ENGLISH] SET QUERY_STORE = ON
GO
ALTER DATABASE [FAI_ENGLISH] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [FAI_ENGLISH]
GO
/****** Object:  Table [dbo].[Answer]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Answer](
	[AnswerID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionID] [int] NULL,
	[AnswerText] [nvarchar](max) NULL,
	[IsCorrect] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[AnswerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[CourseID] [int] IDENTITY(1,1) NOT NULL,
	[CourseName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Duration] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[TypeID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CourseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exam]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exam](
	[ExamID] [int] IDENTITY(1,1) NOT NULL,
	[ExamTypeID] [int] NULL,
	[Title] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[ParentExamID] [int] NULL,
	[Slug] [nvarchar](255) NULL,
	[Duration] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ExamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExamPart]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamPart](
	[PartID] [int] IDENTITY(1,1) NOT NULL,
	[ExamTypeID] [int] NULL,
	[PartType] [varchar](50) NULL,
	[PartName] [nvarchar](100) NULL,
	[Description] [nvarchar](max) NULL,
	[DefaultDuration] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExamSection]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamSection](
	[SectionID] [int] IDENTITY(1,1) NOT NULL,
	[ExamID] [int] NULL,
	[PartID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExamType]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamType](
	[ExamTypeID] [int] IDENTITY(1,1) NOT NULL,
	[ExamName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ExamTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feeback]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feeback](
	[FeebackID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[CourseID] [int] NULL,
	[Rating] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FeebackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Lessons]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lessons](
	[LessonID] [int] IDENTITY(1,1) NOT NULL,
	[CourseID] [int] NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Situation] [nvarchar](max) NOT NULL,
	[VideoURL] [nvarchar](500) NULL,
	[ImageURL] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LessonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Level]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Level](
	[LevelID] [int] IDENTITY(1,1) NOT NULL,
	[LevelName] [nvarchar](200) NULL,
	[LevelScore] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[CourseID] [int] NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[PaymentDate] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Progress]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Progress](
	[ProgressID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[LessonID] [int] NULL,
	[Completed] [bit] NULL,
	[Score] [float] NULL,
	[LastUpdated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProgressID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Question]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[QuestionID] [int] IDENTITY(1,1) NOT NULL,
	[SectionID] [int] NULL,
	[QuestionText] [nvarchar](max) NULL,
	[AudioURL] [nvarchar](500) NULL,
	[ImageURL] [nvarchar](500) NULL,
	[QuestionType] [nvarchar](50) NULL,
	[Explanation] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefreshToken]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshToken](
	[RefreshTokenId] [int] IDENTITY(1,1) NOT NULL,
	[TokenCode] [nvarchar](max) NOT NULL,
	[ExpiryDate] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[DeviceInfo] [nvarchar](256) NULL,
	[IsRevoked] [bit] NOT NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RefreshTokenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Situation]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Situation](
	[SituatuonID] [int] IDENTITY(1,1) NOT NULL,
	[SituationName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[RoleAI] [nvarchar](255) NULL,
	[RoleUser] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[TypeID] [int] NULL,
	[LevelID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SituatuonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Type]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Type](
	[TypeID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[TypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAnswer]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAnswer](
	[UserAnswerID] [int] IDENTITY(1,1) NOT NULL,
	[ResultID] [int] NULL,
	[QuestionID] [int] NULL,
	[SelectedAnswerID] [int] NULL,
	[IsCorrect] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserAnswerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserExamPartSelection]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserExamPartSelection](
	[SelectionID] [int] IDENTITY(1,1) NOT NULL,
	[ResultID] [int] NULL,
	[PartID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SelectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserExamResult]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserExamResult](
	[ResultID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[ExamID] [int] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Score] [float] NULL,
	[CustomDuration] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[userId] [int] NOT NULL,
	[roleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[userId] ASC,
	[roleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NULL,
	[PhoneNumber] [varchar](15) NULL,
	[Avatar] [nvarchar](max) NULL,
	[Dob] [datetime] NULL,
	[Province] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[Status] [nvarchar](10) NOT NULL,
	[expiryDate] [datetime] NULL,
	[Provider] [nvarchar](50) NOT NULL,
	[ProviderId] [nvarchar](255) NULL,
	[LastLogin] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VerificationToken]    Script Date: 6/8/2025 10:11:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VerificationToken](
	[tokenId] [int] IDENTITY(1,1) NOT NULL,
	[tokenCode] [nvarchar](max) NULL,
	[expiryDate] [datetime] NULL,
	[userId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[tokenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Answer] ON 

INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (1, 1, N'He is cooking', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (2, 1, N'He is running', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (3, 1, N'He is sleeping', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (4, 2, N'Next to the library', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (5, 2, N'In the supermarket', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (6, 3, N'went', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (7, 3, N'go', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (8, 4, N'She is reading.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (9, 4, N'She is walking.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (10, 4, N'She is cooking.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (11, 4, N'She is sleeping.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (12, 5, N'A phone.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (13, 5, N'A book.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (14, 5, N'A bag.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (15, 5, N'A cup.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (16, 6, N'In a car.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (17, 6, N'On a bench.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (18, 6, N'At a table.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (19, 6, N'On the floor.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (20, 7, N'On the first floor.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (21, 7, N'On the second floor.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (22, 7, N'On the third floor.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (23, 7, N'In the basement.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (24, 8, N'At 2 PM.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (25, 8, N'At 3 PM.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (26, 8, N'At 4 PM.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (27, 8, N'At 5 PM.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (28, 9, N'The manager.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (29, 9, N'The client.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (30, 9, N'The receptionist.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (31, 9, N'The colleague.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (32, 10, N'A new project.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (33, 10, N'A vacation plan.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (34, 10, N'A budget report.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (35, 10, N'A marketing strategy.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (36, 11, N'Today.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (37, 11, N'Tomorrow.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (38, 11, N'Next week.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (39, 11, N'Next month.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (40, 12, N'Mr. Smith.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (41, 12, N'Ms. Johnson.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (42, 12, N'Mr. Brown.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (43, 12, N'Ms. Lee.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (44, 13, N'Company policies.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (45, 13, N'A new product.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (46, 13, N'A sales report.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (47, 13, N'A training session.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (48, 14, N'Tomorrow.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (49, 14, N'Next week.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (50, 14, N'Next month.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (51, 14, N'Next year.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (52, 15, N'The HR department.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (53, 15, N'The IT department.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (54, 15, N'The marketing team.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (55, 15, N'The sales team.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (56, 16, N'finish.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (57, 16, N'finished.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (58, 16, N'finishing.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (59, 16, N'finishes.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (60, 17, N'come.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (61, 17, N'came.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (62, 17, N'comes.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (63, 17, N'coming.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (64, 18, N'is.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (65, 18, N'are.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (66, 18, N'was.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (67, 18, N'were.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (68, 19, N'increased.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (69, 19, N'decreased.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (70, 19, N'maintained.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (71, 19, N'reduced.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (72, 20, N'miss.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (73, 20, N'meet.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (74, 20, N'extend.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (75, 20, N'ignore.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (76, 21, N'good.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (77, 21, N'well.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (78, 21, N'bad.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (79, 21, N'poorly.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (80, 22, N'To announce a meeting.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (81, 22, N'To request a report.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (82, 22, N'To share a schedule.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (83, 22, N'To discuss a project.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (84, 23, N'The manager.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (85, 23, N'All employees.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (86, 23, N'The HR team.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (87, 23, N'The sales team.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (88, 24, N'Monday.', 1)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (89, 24, N'Tuesday.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (90, 24, N'Wednesday.', 0)
INSERT [dbo].[Answer] ([AnswerID], [QuestionID], [AnswerText], [IsCorrect]) VALUES (91, 24, N'Thursday.', 0)
SET IDENTITY_INSERT [dbo].[Answer] OFF
GO
SET IDENTITY_INSERT [dbo].[Exam] ON 

INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (1, 3, N'TEST ĐẦU VÀO', N'Kiểm tra năng lực đầu vào', CAST(N'2025-04-21T08:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (2, 3, N'HACKER TOEIC 3', N'Đề thi luyện tập Hacker 3', CAST(N'2025-04-20T10:00:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (3, 3, N'HACKER TOEIC 2', N'Đề thi luyện tập Hacker 2', CAST(N'2025-04-19T09:30:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (4, 1, N'ETS 2024', N'Đề mô phỏng từ ETS 2024', CAST(N'2025-04-18T15:20:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (5, 2, N'IELTS MOCK TEST 1', N'Thi thử IELTS 4 kỹ năng', CAST(N'2025-04-17T11:45:00.000' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (6, 3, N'TEST ĐẦU VÀO (1)', N'Kiểm tra năng lực đầu vào', CAST(N'2025-04-21T08:00:00.000' AS DateTime), 1, N'test-dau-vao-1', 120)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (7, 3, N'TEST ĐẦU VÀO (2)', N'Kiểm tra năng lực đầu vào', CAST(N'2025-04-21T08:00:00.000' AS DateTime), 1, N'test-dau-vao-2', 120)
INSERT [dbo].[Exam] ([ExamID], [ExamTypeID], [Title], [Description], [CreatedAt], [ParentExamID], [Slug], [Duration]) VALUES (8, 3, N'TEST ĐẦU VÀO (3)', N'Kiểm tra năng lực đầu vào', CAST(N'2025-04-21T08:00:00.000' AS DateTime), 1, N'test-dau-vao-3', 120)
SET IDENTITY_INSERT [dbo].[Exam] OFF
GO
SET IDENTITY_INSERT [dbo].[ExamPart] ON 

INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (1, 3, N'Listening', N'Listening Part 1', N'Mô tả part nghe tranh', 10)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (2, 3, N'Listening', N'Listening Part 2', N'Mô tả part hỏi đáp', 20)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (3, 3, N'Listening', N'Listening Part 3', N'Mô tả part hội thoại', 30)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (4, 3, N'Listening', N'Listening Part 4', N'Mô tả part bài nói', 25)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (5, 3, N'Reading', N'Reading Part 5', N'Mô tả part điền từ', 25)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (6, 3, N'Reading', N'Reading Part 6', N'Mô tả part điền đoạn văn', 15)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (7, 3, N'Reading', N'Reading Part 7', N'Mô tả part đọc hiểu', 40)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (8, 2, N'Listening', N'Listening Section 1', N'Listening cho IELTS', 20)
INSERT [dbo].[ExamPart] ([PartID], [ExamTypeID], [PartType], [PartName], [Description], [DefaultDuration]) VALUES (9, 2, N'Writing', N'Writing Task 1', N'Viết báo cáo', 20)
SET IDENTITY_INSERT [dbo].[ExamPart] OFF
GO
SET IDENTITY_INSERT [dbo].[ExamSection] ON 

INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (1, 1, 1)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (2, 1, 2)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (3, 2, 1)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (4, 2, 2)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (5, 2, 3)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (6, 3, 1)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (7, 3, 2)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (8, 4, 1)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (9, 4, 2)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (10, 4, 3)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (11, 5, 4)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (12, 5, 5)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (13, 6, 1)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (14, 6, 2)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (15, 6, 3)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (16, 6, 4)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (17, 6, 5)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (18, 6, 6)
INSERT [dbo].[ExamSection] ([SectionID], [ExamID], [PartID]) VALUES (19, 6, 7)
SET IDENTITY_INSERT [dbo].[ExamSection] OFF
GO
SET IDENTITY_INSERT [dbo].[ExamType] ON 

INSERT [dbo].[ExamType] ([ExamTypeID], [ExamName]) VALUES (1, N'TOEIC')
INSERT [dbo].[ExamType] ([ExamTypeID], [ExamName]) VALUES (2, N'IELTS')
INSERT [dbo].[ExamType] ([ExamTypeID], [ExamName]) VALUES (3, N'TOEIC Listening & Reading')
INSERT [dbo].[ExamType] ([ExamTypeID], [ExamName]) VALUES (4, N'TOEIC Speaking & Writing')
SET IDENTITY_INSERT [dbo].[ExamType] OFF
GO
SET IDENTITY_INSERT [dbo].[Level] ON 

INSERT [dbo].[Level] ([LevelID], [LevelName], [LevelScore]) VALUES (1, N'Beginner', N'0-250')
INSERT [dbo].[Level] ([LevelID], [LevelName], [LevelScore]) VALUES (2, N'Pre-Intermediate', N'405-600')
INSERT [dbo].[Level] ([LevelID], [LevelName], [LevelScore]) VALUES (3, N'Intermediate', N'605-780')
INSERT [dbo].[Level] ([LevelID], [LevelName], [LevelScore]) VALUES (4, N'Upper-Intermediate', N'785-900')
INSERT [dbo].[Level] ([LevelID], [LevelName], [LevelScore]) VALUES (5, N'Upper-Intermediate', N'905-990')
INSERT [dbo].[Level] ([LevelID], [LevelName], [LevelScore]) VALUES (6, N'Advanced', N'0-250')
SET IDENTITY_INSERT [dbo].[Level] OFF
GO
SET IDENTITY_INSERT [dbo].[Question] ON 

INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (1, 1, N'What is the man doing?', N'/audio/q1.mp3', N'/img/q1.jpg', N'MultipleChoice', N'Nhìn hành động')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (2, 2, N'Where is the nearest bank?', N'/audio/q2.mp3', NULL, N'MultipleChoice', N'Đoán theo context')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (3, 5, N'Choose the correct word', NULL, NULL, N'FillBlank', N'Từ loại đúng')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (4, 13, N'What is the woman doing?', N'/audio/listening/part1/q1.mp3', N'/img/listening/part1/q1.jpg', N'MultipleChoice', N'The woman is walking in the park.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (5, 13, N'What is the man holding?', N'/audio/listening/part1/q2.mp3', N'/img/listening/part1/q2.jpg', N'MultipleChoice', N'The man is holding a book.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (6, 13, N'Where are the people sitting?', N'/audio/listening/part1/q3.mp3', N'/img/listening/part1/q3.jpg', N'MultipleChoice', N'The people are sitting on a bench.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (7, 14, N'Where is the meeting room?', N'/audio/listening/part2/q1.mp3', NULL, N'MultipleChoice', N'The meeting room is on the second floor.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (8, 14, N'What time does the train leave?', N'/audio/listening/part2/q2.mp3', NULL, N'MultipleChoice', N'The train leaves at 3 PM.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (9, 14, N'Who is calling?', N'/audio/listening/part2/q3.mp3', NULL, N'MultipleChoice', N'The manager is calling.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (10, 15, N'What are they discussing?', N'/audio/listening/part3/q1.mp3', NULL, N'MultipleChoice', N'They are discussing a new project.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (11, 15, N'When will the meeting take place?', N'/audio/listening/part3/q2.mp3', NULL, N'MultipleChoice', N'The meeting will take place tomorrow.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (12, 15, N'Who is leading the project?', N'/audio/listening/part3/q3.mp3', NULL, N'MultipleChoice', N'Mr. Smith is leading the project.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (13, 16, N'What is the speaker talking about?', N'/audio/listening/part4/q1.mp3', NULL, N'MultipleChoice', N'The speaker is talking about company policies.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (14, 16, N'When is the event scheduled?', N'/audio/listening/part4/q2.mp3', NULL, N'MultipleChoice', N'The event is scheduled for next week.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (15, 16, N'Who should employees contact?', N'/audio/listening/part4/q3.mp3', NULL, N'MultipleChoice', N'Employees should contact the HR department.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (16, 17, N'The manager ______ the report yesterday.', NULL, NULL, N'FillBlank', N'Correct word: "finished".')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (17, 17, N'She ______ to the meeting on time.', NULL, NULL, N'FillBlank', N'Correct word: "came".')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (18, 17, N'They ______ working on the project.', NULL, NULL, N'FillBlank', N'Correct word: "are".')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (19, 18, N'Choose the best word to fill in the blank: "The company has ______ its profits this year."', NULL, NULL, N'MultipleChoice', N'Correct word: "increased".')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (20, 18, N'Choose the best phrase: "We need to ______ the deadline."', NULL, NULL, N'MultipleChoice', N'Correct phrase: "meet".')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (21, 18, N'Choose the best word: "The team works ______ together."', NULL, NULL, N'MultipleChoice', N'Correct word: "well".')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (22, 19, N'What is the main purpose of the email?', NULL, N'/img/reading/part7/email.jpg', N'MultipleChoice', N'The email is to announce a meeting.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (23, 19, N'Who is the email addressed to?', NULL, N'/img/reading/part7/email.jpg', N'MultipleChoice', N'The email is addressed to all employees.')
INSERT [dbo].[Question] ([QuestionID], [SectionID], [QuestionText], [AudioURL], [ImageURL], [QuestionType], [Explanation]) VALUES (24, 19, N'When is the meeting scheduled?', NULL, N'/img/reading/part7/email.jpg', N'MultipleChoice', N'The meeting is scheduled for Monday.')
SET IDENTITY_INSERT [dbo].[Question] OFF
GO
SET IDENTITY_INSERT [dbo].[RefreshToken] ON 

INSERT [dbo].[RefreshToken] ([RefreshTokenId], [TokenCode], [ExpiryDate], [UserId], [DeviceInfo], [IsRevoked], [CreatedDate]) VALUES (1, N'9bbb3c73-ecfe-425f-80c1-d09b4b827d02', CAST(N'2025-07-08T03:13:15.313' AS DateTime), 11, N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36', 0, CAST(N'2025-06-08T10:13:15.340' AS DateTime))
INSERT [dbo].[RefreshToken] ([RefreshTokenId], [TokenCode], [ExpiryDate], [UserId], [DeviceInfo], [IsRevoked], [CreatedDate]) VALUES (2, N'f4b67739-b817-4cb6-88a1-c642ed50085c', CAST(N'2025-07-08T03:45:54.707' AS DateTime), 11, N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36', 0, CAST(N'2025-06-08T10:45:54.817' AS DateTime))
INSERT [dbo].[RefreshToken] ([RefreshTokenId], [TokenCode], [ExpiryDate], [UserId], [DeviceInfo], [IsRevoked], [CreatedDate]) VALUES (3, N'd8d2fad4-1792-4482-9f87-f82d1c2a9b40', CAST(N'2025-07-08T07:18:24.410' AS DateTime), 11, N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36', 0, CAST(N'2025-06-08T14:18:24.470' AS DateTime))
INSERT [dbo].[RefreshToken] ([RefreshTokenId], [TokenCode], [ExpiryDate], [UserId], [DeviceInfo], [IsRevoked], [CreatedDate]) VALUES (4, N'18a64e13-950c-4a49-9ddc-cfe3af17d547', CAST(N'2025-07-08T10:13:50.583' AS DateTime), 14, N'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36', 0, CAST(N'2025-06-08T17:13:50.670' AS DateTime))
SET IDENTITY_INSERT [dbo].[RefreshToken] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (1, N'admin')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (2, N'mentor')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (3, N'mentee')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (4, N'staff')
INSERT [dbo].[Role] ([RoleID], [RoleName]) VALUES (5, N'guest')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Situation] ON 

INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (1, N'Gọi đặt lịch khám', N'Sáng nay ngủ dậy bạn nhức đầu và đau họng kinh khủng. Bạn cần đi bác sĩ khám bệnh và lấy thuốc. Hãy gọi cho phòng khám để đặt lịch nhé!', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a64-78f8-663a-be94-0242ac110002-1723465666.png', N'Y tá', N'Người bệnh', CAST(N'2025-05-30T10:22:23.073' AS DateTime), 1, 1)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (2, N'Trẻ bị ho kéo dài', N'Trẻ bị ho nhiều ngày không khỏi, có thể do viêm phế quản hoặc viêm họng.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/9/1ef81600-b2a1-659c-b6f8-0242ac110005-1727943497.png', N'Y tá', N'Người bệnh', CAST(N'2025-05-30T10:22:23.073' AS DateTime), 1, 2)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (3, N'Trẻ bị tiêu chảy', N'Trẻ đi ngoài nhiều lần, phân lỏng, cần tránh mất nước.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-21c3-622e-aa19-0242ac110002-1723465683.png', N'Y tá', N'Người bệnh', CAST(N'2025-05-30T10:22:23.073' AS DateTime), 1, 2)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (4, N'Trẻ có dấu hiệu dị ứng', N'Xuất hiện phát ban hoặc ngứa sau khi ăn hoặc tiếp xúc với dị nguyên.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/6/1ef58a65-4b1e-62cc-8ab5-0242ac110002-1723465688.png', N'Y tá', N'Người bệnh', CAST(N'2025-05-30T10:22:23.073' AS DateTime), 1, 3)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (5, N'Trẻ bị chấn thương nhẹ', N'Trẻ bị té ngã, trầy xước nhẹ, cần sát trùng và theo dõi.', N'https://files.kynaenglish.com/resize/400/tmp/ai_tutor/7/1ef58a65-7f9b-68f6-a031-0242ac110002-1723465693.png', N'Y tá', N'Người bệnh', CAST(N'2025-05-30T10:22:23.073' AS DateTime), 1, 5)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (6, N'Gọi món ăn', N'Thực hành gọi món ăn tại nhà hàng.', N'https://ktvntd.edu.vn/wp-content/uploads/tmp/ti%E1%BA%BFng-anh.jpg', N'Nhân viên phục vụ', N'Khách hàng', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 1, 1)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (7, N'Đặt phòng khách sạn', N'Đặt phòng qua điện thoại.', N'https://www.hoteljob.vn/uploads/images/19-06-05-16/kich-ban-mau-quy-trinh-nhan-dat-phong-khach-san-qua-dien-thoai-1-52.jpg', N'Lễ tân', N'Khách hàng', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 1, 2)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (8, N'Hỏi đường', N'Hỏi đường đến một địa điểm.', N'https://hoavanshz.com//uploads/images/blog-tieng-hoa/tu-vung/tieng-trung-chu-de-hoi-duong-chi-duong.jpg', N'Người dân địa phương', N'Khách du lịch', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 2, 1)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (9, N'Mua sắm quần áo', N'Mua quần áo tại cửa hàng.', N'https://cdn-images.vtv.vn/zoom/640_400/66349b6076cb4dee98746cf1/2024/08/15/anh-bia-3-73797472461795645577768-44578070876535739927869.jpg', N'Nhân viên bán hàng', N'Khách hàng', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 2, 2)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (10, N'Tại sân bay', N'Làm thủ tục tại sân bay.', N'https://www.aviationplus.vn/wp-content/uploads/Dich-vu-dua-don-khach-uu-tien-Fast-Track-VVIP.jpg', N'Nhân viên sân bay', N'Hành khách', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 1, 3)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (11, N'Đặt lịch khám bệnh', N'Đặt lịch hẹn khám bệnh.', N'https://bvthieuhoa.ytethanhhoa.gov.vn/file/thumb/500/636989949.jpg', N'Bác sĩ', N'Bệnh nhân', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 1, 1)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (12, N'Phỏng vấn xin việc', N'Thực hành trả lời câu hỏi phỏng vấn.', N'https://images.careerviet.vn/content/images/cac-loai-hinh-phong-van-careerbuilder.jpg', N'Người phỏng vấn', N'Ứng viên', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 3, 2)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (13, N'Giao dịch ngân hàng', N'Mở tài khoản ngân hàng.', N'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRqxGZo2xEZdTUgQBo4m0_8_8uwBco1210kNw&s', N'Nhân viên ngân hàng', N'Khách hàng', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 2, 2)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (14, N'Gọi điện thoại', N'Thực hành gọi điện thoại cho bạn.', N'https://etu.edu.vn/wp-content/uploads/2024/09/tieng-anh-giao-tiep-goi-dien-thoai-3.jpeg', N'Người nhận', N'Người gọi', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 3, 1)
INSERT [dbo].[Situation] ([SituatuonID], [SituationName], [Description], [ImageUrl], [RoleAI], [RoleUser], [CreatedAt], [TypeID], [LevelID]) VALUES (15, N'Mua vé xem phim', N'Mua vé tại rạp chiếu phim.', N'https://metiz.vn/media/uploads/2023/11/06/rap-chieu-phim-metiz-cinema-da-nang-9.jpg', N'Nhân viên bán vé', N'Khách hàng', CAST(N'2025-06-08T17:08:22.437' AS DateTime), 2, 1)
SET IDENTITY_INSERT [dbo].[Situation] OFF
GO
SET IDENTITY_INSERT [dbo].[Type] ON 

INSERT [dbo].[Type] ([TypeID], [TypeName]) VALUES (1, N'Nhập Vai')
INSERT [dbo].[Type] ([TypeID], [TypeName]) VALUES (2, N'Học qua hình')
INSERT [dbo].[Type] ([TypeID], [TypeName]) VALUES (3, N'Phát âm')
SET IDENTITY_INSERT [dbo].[Type] OFF
GO
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (1, 4)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (2, 3)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (3, 1)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (8, 3)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (9, 5)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (10, 3)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (11, 1)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (12, 4)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (13, 5)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (14, 2)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (15, 3)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (16, 3)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (17, 2)
INSERT [dbo].[UserRole] ([userId], [roleId]) VALUES (18, 3)
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (1, N'Nguyen Tuan Anh1', N'anhnthe172115@fpt.edu.vn', N'AQAAAAIAAYagAAAAEIDbIDAbn1VrGT9s6ruIzPgKSV9MMSy4b+SQJPNS1HV5MDmMS4ZgIHmCNC235f6PfQ==', N'+840969633111', N'/Images/avatar.jpg', CAST(N'2003-02-02T00:00:00.000' AS DateTime), NULL, CAST(N'2025-05-30T10:22:22.983' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (2, N'Nguyễn Đình Quyền', N'limnguyen243@gmail.com', NULL, NULL, N'/Images/user_icon.webp', CAST(N'2005-01-01T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-03T18:44:02.837' AS DateTime), N'ACTIVATED', NULL, N'Google', N'102674697536013945744', CAST(N'2025-06-08T10:13:00.413' AS DateTime))
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (3, N'N Đ Q', N'quyen.nddl.vn@gmail.com', NULL, N'+840934226411', N'', CAST(N'2005-04-01T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-04T16:07:27.577' AS DateTime), N'ACTIVATED', NULL, N'Google', N'117588835372005040006', CAST(N'2025-06-08T14:25:45.853' AS DateTime))
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (8, N'Nguyễn Đình Quyền K17HL', N'quyenndhe172235@fpt.edu.vn', NULL, N'0934552410', N'/uploads/avatars/e6baa8d5-6501-4ee0-8eb0-51633f8c5943.jpg', CAST(N'2025-06-03T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-07T09:16:41.837' AS DateTime), N'ACTIVATED', NULL, N'Google', N'100754004463801792526', CAST(N'2025-06-07T09:16:41.837' AS DateTime))
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (9, N'Đình Quyền⁀cute', N'nhungayhomqua244@gmail.com', NULL, N'0934556222', N'/Images/user_icon.webp', CAST(N'2001-01-11T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-07T09:17:12.550' AS DateTime), N'ACTIVATED', NULL, N'Google', N'107329372679780862104', CAST(N'2025-06-07T09:17:12.550' AS DateTime))
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (10, N'Nguyen Dinh Quyen', N'quyennd244vn@gmail.com', NULL, NULL, N'/Images/user_icon.webp', NULL, NULL, CAST(N'2025-06-07T09:17:24.830' AS DateTime), N'ACTIVATED', NULL, N'Google', N'106022381284306111318', CAST(N'2025-06-07T09:17:24.830' AS DateTime))
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (11, N'Trà My', N'jxv49832@jioso.com', N'AQAAAAIAAYagAAAAEIDbIDAbn1VrGT9s6ruIzPgKSV9MMSy4b+SQJPNS1HV5MDmMS4ZgIHmCNC235f6PfQ==', N'0934556113', N'/uploads/avatars/bba090fe-647f-427e-91fb-9cd4f1077863.jpg', CAST(N'2003-04-24T00:00:00.000' AS DateTime), 1, CAST(N'2025-06-08T03:11:17.087' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (12, N'hẹhej', N'abc@gmail.com', NULL, N'0934511410', N'/uploads/avatars/8759bba5-2bec-4b98-86c0-b3a0c8746873_bea.jpg', CAST(N'2025-05-26T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-08T08:13:22.247' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (13, N'hohoo24', N'anhnthe172215@fpt.edu.vn', N'AQAAAAIAAYagAAAAEJpzqmzlGSQb7eJTHvs7dnPKG/nsO+aeJCv5dwuZrCP5RMJiDE6SJh6vNE/ZlXZXbA==', N'0935556410', N'/uploads/avatars/49f741f0-4551-427d-8e5a-6647e148179e_bea.jpg', CAST(N'2025-05-29T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-08T08:23:42.350' AS DateTime), N'LOCKED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (14, N'Tra My Xinh Gái', N'tramy@gmail.com', N'AQAAAAIAAYagAAAAEO+ESA+4Fffv8yiXq6+WgOaYx72XXyWXr5yOKKVJNXqBWU4TPwFb4zcyAb4tmfBvmQ==', N'0964263730', N'/uploads/avatars/0ae0b2d7-8048-481f-a9ce-be58936a4cc6_bea.jpg', CAST(N'2007-07-30T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-08T10:02:23.910' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (15, N'anhnt', N'anhnt21072003@gmail.com', N'AQAAAAIAAYagAAAAEN8Oi2a//uCmo+C0PqUPGOmHdS1TGFgVZB4/GOxP6kHGvMt0i7SvYPaJuiDFOmaalg==', N'+84869620297', N'/Images/user_icon.webp', NULL, NULL, CAST(N'2025-06-08T10:14:57.633' AS DateTime), N'PENDING', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (16, N'2003', N'vuf76772@toaik.com', N'AQAAAAIAAYagAAAAELdM5WShDfcbjYG9+6fb7xx0QPBoJhuTUEZ1Ox8DQQ2XVhO8SO5XzEmRE4xj0VUIzw==', N'+84933356410', N'/Images/user_icon.webp', NULL, NULL, CAST(N'2025-06-08T10:23:09.703' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (17, N'12321', N'1111832@jioso.com', N'AQAAAAIAAYagAAAAEJBjV1/jktm2GxgUaJkm7VWYKlTDthiAKiQbnxYy09Qrmk17xiIrMZ8L9ml6fphAWQ==', N'0955641022', N'/uploads/avatars/44db8c04-f56d-43a3-8112-2ab5cf41e487_coc8mau.jpg', CAST(N'2025-05-28T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-08T13:41:30.320' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
INSERT [dbo].[Users] ([UserID], [FullName], [Email], [PasswordHash], [PhoneNumber], [Avatar], [Dob], [Province], [CreatedAt], [Status], [expiryDate], [Provider], [ProviderId], [LastLogin]) VALUES (18, N'Khang Huy', N'1quyen.nddl.vn@gmail.com', N'AQAAAAIAAYagAAAAEN9ctByEZwWLPleBHZCKePLk13tMuo4wuw5U1XckdLsNv0/yR8lj4XfFUP+7p3XWyQ==', N'+840934552231', N'/uploads/avatars/6e146bfc-b6ad-446e-b330-26d151580d9c_exe201.jpg', CAST(N'2024-09-12T00:00:00.000' AS DateTime), NULL, CAST(N'2025-06-08T13:49:21.537' AS DateTime), N'ACTIVATED', NULL, N'Local', NULL, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET IDENTITY_INSERT [dbo].[VerificationToken] ON 

INSERT [dbo].[VerificationToken] ([tokenId], [tokenCode], [expiryDate], [userId]) VALUES (1, N'1b7dfe7f-b523-4819-96df-46fe7d5b64d2', CAST(N'2025-06-09T10:11:17.290' AS DateTime), 11)
INSERT [dbo].[VerificationToken] ([tokenId], [tokenCode], [expiryDate], [userId]) VALUES (2, N'51643816-da86-424d-97ec-566cd4632066', CAST(N'2025-06-09T17:16:32.107' AS DateTime), 15)
INSERT [dbo].[VerificationToken] ([tokenId], [tokenCode], [expiryDate], [userId]) VALUES (3, N'2d880130-a977-4377-a362-5e4c98cfd386', CAST(N'2025-06-09T17:23:10.037' AS DateTime), 16)
SET IDENTITY_INSERT [dbo].[VerificationToken] OFF
GO
/****** Object:  Index [UK_UserExamPartSelection]    Script Date: 6/8/2025 10:11:35 PM ******/
ALTER TABLE [dbo].[UserExamPartSelection] ADD  CONSTRAINT [UK_UserExamPartSelection] UNIQUE NONCLUSTERED 
(
	[ResultID] ASC,
	[PartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D10534699FBB9A]    Script Date: 6/8/2025 10:11:35 PM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Provider_ProviderId]    Script Date: 6/8/2025 10:11:35 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Provider_ProviderId] ON [dbo].[Users]
(
	[Provider] ASC,
	[ProviderId] ASC
)
WHERE ([ProviderId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Verifica__CB9A1CFE2E03DC36]    Script Date: 6/8/2025 10:11:35 PM ******/
ALTER TABLE [dbo].[VerificationToken] ADD UNIQUE NONCLUSTERED 
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Courses] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Exam] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Feeback] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Lessons] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Payments] ADD  DEFAULT (getdate()) FOR [PaymentDate]
GO
ALTER TABLE [dbo].[Progress] ADD  DEFAULT ((0)) FOR [Completed]
GO
ALTER TABLE [dbo].[Progress] ADD  DEFAULT (getdate()) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[RefreshToken] ADD  DEFAULT ((0)) FOR [IsRevoked]
GO
ALTER TABLE [dbo].[RefreshToken] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Situation] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Answer]  WITH CHECK ADD FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Question] ([QuestionID])
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD FOREIGN KEY([TypeID])
REFERENCES [dbo].[Type] ([TypeID])
GO
ALTER TABLE [dbo].[Exam]  WITH CHECK ADD FOREIGN KEY([ExamTypeID])
REFERENCES [dbo].[ExamType] ([ExamTypeID])
GO
ALTER TABLE [dbo].[Exam]  WITH CHECK ADD  CONSTRAINT [FK_Exam_Parent] FOREIGN KEY([ParentExamID])
REFERENCES [dbo].[Exam] ([ExamID])
GO
ALTER TABLE [dbo].[Exam] CHECK CONSTRAINT [FK_Exam_Parent]
GO
ALTER TABLE [dbo].[ExamPart]  WITH CHECK ADD FOREIGN KEY([ExamTypeID])
REFERENCES [dbo].[ExamType] ([ExamTypeID])
GO
ALTER TABLE [dbo].[ExamSection]  WITH CHECK ADD FOREIGN KEY([ExamID])
REFERENCES [dbo].[Exam] ([ExamID])
GO
ALTER TABLE [dbo].[ExamSection]  WITH CHECK ADD FOREIGN KEY([PartID])
REFERENCES [dbo].[ExamPart] ([PartID])
GO
ALTER TABLE [dbo].[Feeback]  WITH CHECK ADD FOREIGN KEY([CourseID])
REFERENCES [dbo].[Courses] ([CourseID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feeback]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Lessons]  WITH CHECK ADD FOREIGN KEY([CourseID])
REFERENCES [dbo].[Courses] ([CourseID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD FOREIGN KEY([CourseID])
REFERENCES [dbo].[Courses] ([CourseID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Progress]  WITH CHECK ADD FOREIGN KEY([LessonID])
REFERENCES [dbo].[Lessons] ([LessonID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Progress]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD FOREIGN KEY([SectionID])
REFERENCES [dbo].[ExamSection] ([SectionID])
GO
ALTER TABLE [dbo].[RefreshToken]  WITH CHECK ADD  CONSTRAINT [FK_UserRefreshToken] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[RefreshToken] CHECK CONSTRAINT [FK_UserRefreshToken]
GO
ALTER TABLE [dbo].[Situation]  WITH CHECK ADD FOREIGN KEY([LevelID])
REFERENCES [dbo].[Level] ([LevelID])
GO
ALTER TABLE [dbo].[Situation]  WITH CHECK ADD FOREIGN KEY([TypeID])
REFERENCES [dbo].[Type] ([TypeID])
GO
ALTER TABLE [dbo].[UserAnswer]  WITH CHECK ADD FOREIGN KEY([QuestionID])
REFERENCES [dbo].[Question] ([QuestionID])
GO
ALTER TABLE [dbo].[UserAnswer]  WITH CHECK ADD FOREIGN KEY([ResultID])
REFERENCES [dbo].[UserExamResult] ([ResultID])
GO
ALTER TABLE [dbo].[UserAnswer]  WITH CHECK ADD FOREIGN KEY([SelectedAnswerID])
REFERENCES [dbo].[Answer] ([AnswerID])
GO
ALTER TABLE [dbo].[UserExamPartSelection]  WITH CHECK ADD FOREIGN KEY([PartID])
REFERENCES [dbo].[ExamPart] ([PartID])
GO
ALTER TABLE [dbo].[UserExamPartSelection]  WITH CHECK ADD FOREIGN KEY([ResultID])
REFERENCES [dbo].[UserExamResult] ([ResultID])
GO
ALTER TABLE [dbo].[UserExamResult]  WITH CHECK ADD FOREIGN KEY([ExamID])
REFERENCES [dbo].[Exam] ([ExamID])
GO
ALTER TABLE [dbo].[UserExamResult]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD FOREIGN KEY([roleId])
REFERENCES [dbo].[Role] ([RoleID])
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[VerificationToken]  WITH CHECK ADD  CONSTRAINT [FK_UserToken] FOREIGN KEY([userId])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[VerificationToken] CHECK CONSTRAINT [FK_UserToken]
GO
ALTER TABLE [dbo].[Feeback]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD CHECK  (([Status]='Failed' OR [Status]='Completed' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD CHECK  (([QuestionType]='Essay' OR [QuestionType]='FillBlank' OR [QuestionType]='MultipleChoice'))
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([Status]='DELETE' OR [Status]='LOCKED' OR [Status]='ACTIVATED' OR [Status]='TRIAL' OR [Status]='PENDING'))
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [CK_Provider_ProviderId] CHECK  (([Provider]='Local' AND [ProviderId] IS NULL OR [Provider]<>'Local' AND [ProviderId] IS NOT NULL))
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [CK_Provider_ProviderId]
GO
USE [master]
GO
ALTER DATABASE [FAI_ENGLISH] SET  READ_WRITE 
GO
