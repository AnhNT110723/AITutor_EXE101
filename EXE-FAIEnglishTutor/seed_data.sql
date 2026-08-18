-- =============================================
-- SEED DATA SCRIPT - FAI English Tutor
-- Chạy script này trong SQL Server Management Studio
-- Kết nối vào database: FAI_ENGLISH
--
-- ImageUrl dùng ảnh từ Unsplash (public, load được ngay)
-- Sau khi có Cloudinary, có thể UPDATE lại ImageUrl bằng URL từ Cloudinary
-- =============================================

USE FAI_ENGLISH;
GO

-- =============================================
-- 0. XÓA DỮ LIỆU CŨ (chạy lại an toàn, không cần drop DB)
-- Thứ tự xóa: Situation trước (có FK), rồi mới xóa Level và Type
-- =============================================
DELETE FROM [Situation];  -- xóa trước vì có FK trỏ vào Level và Type
DELETE FROM [Level];
DELETE FROM [Type];

-- Reset identity counter về 1 (bắt buộc sau DELETE để LevelID/TypeID bắt đầu lại từ 1)
DBCC CHECKIDENT ('[Situation]', RESEED, 0);
DBCC CHECKIDENT ('[Level]', RESEED, 0);
DBCC CHECKIDENT ('[Type]', RESEED, 0);

PRINT 'Old data cleared.';
GO

-- =============================================
-- 1. THÊM DỮ LIỆU BẢNG Level
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [Level])
BEGIN
    INSERT INTO [Level] (LevelName, LevelScore) VALUES
    (N'Beginner (A1)',           N'0 - 300'),
    (N'Elementary (A2)',         N'301 - 450'),
    (N'Intermediate (B1)',       N'451 - 600'),
    (N'Upper-Intermediate (B2)', N'601 - 750'),
    (N'Advanced (C1)',           N'751 - 900');
    PRINT 'Level: inserted 5 records';
END
ELSE
    PRINT 'Level: already has data, skipped';
GO

-- =============================================
-- 2. THÊM DỮ LIỆU BẢNG Type
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [Type])
BEGIN
    INSERT INTO [Type] (TypeName) VALUES
    (N'Role Play'),
    (N'Listening'),
    (N'Speak Freely'),
    (N'Pronunciation');
    PRINT 'Type: inserted 4 records';
END
ELSE
    PRINT 'Type: already has data, skipped';
GO

-- =============================================
-- 3. THÊM DỮ LIỆU BẢNG Situation (30 bản ghi)
-- ImageUrl = URL ảnh Unsplash công khai (dùng tạm, có thể UPDATE sau)
-- =============================================
DECLARE @RolePlayTypeId INT = (SELECT TOP 1 TypeID FROM [Type] WHERE TypeName = N'Role Play');

IF @RolePlayTypeId IS NULL
BEGIN
    PRINT 'ERROR: Could not find Role Play type. Make sure Type table is seeded first.';
    RETURN;
END

-- ---- Level 1: Beginner (A1) ----
INSERT INTO [Situation] (SituationName, Description, ImageUrl, RoleAI, RoleUser, CreatedAt, TypeID, LevelID) VALUES
(
    N'Greeting a New Friend',
    N'Practice a simple first-time greeting at school. Learn how to introduce yourself and ask basic questions.',
    N'https://images.unsplash.com/photo-1529156069898-49953e39b3ac?w=600',
    N'You are a friendly student named Alex who just moved to a new school. Be welcoming, ask simple questions like name and hometown.',
    N'You are a new student on your first day of school. Introduce yourself and make a new friend.',
    GETDATE(), @RolePlayTypeId, 1
),
(
    N'Ordering Food at a Café',
    N'Practice ordering food and drinks at a café. Learn common food vocabulary and polite expressions.',
    N'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=600',
    N'You are a friendly café staff. Take the customer''s order, offer simple recommendations, and process payment.',
    N'You are a customer who wants to order a coffee and a snack at a small café.',
    GETDATE(), @RolePlayTypeId, 1
),
(
    N'Asking for Directions',
    N'Practice asking for and giving directions. Learn basic location vocabulary.',
    N'https://images.unsplash.com/photo-1503614472-8c93d56e92ce?w=600',
    N'You are a local resident who knows the area well. Help the tourist find their destination using simple directions.',
    N'You are a tourist who is lost and needs to find the nearest bus station.',
    GETDATE(), @RolePlayTypeId, 1
),
(
    N'Shopping at a Supermarket',
    N'Practice shopping conversation: asking for items, checking prices, and paying.',
    N'https://images.unsplash.com/photo-1542838132-92c53300491e?w=600',
    N'You are a helpful supermarket staff. Assist the customer in finding items and provide prices.',
    N'You are a customer looking for specific grocery items. Ask for help and ask about prices.',
    GETDATE(), @RolePlayTypeId, 1
),
(
    N'Making a Phone Call',
    N'Practice a basic phone conversation. Learn phone vocabulary and polite phrases.',
    N'https://images.unsplash.com/photo-1534536281715-e28d76689b4d?w=600',
    N'You are a hotel receptionist receiving a booking inquiry. Answer questions politely and confirm availability.',
    N'You want to book a room at a hotel. Call the hotel and ask about available rooms and prices.',
    GETDATE(), @RolePlayTypeId, 1
),
(
    N'Talking About Hobbies',
    N'Practice sharing information about hobbies and free time activities.',
    N'https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=600',
    N'You are a friendly person at a social gathering. Share your hobbies (reading, cooking) and ask simple follow-up questions.',
    N'You are at a social event and meet someone new. Share your hobbies and show interest in theirs.',
    GETDATE(), @RolePlayTypeId, 1
);

-- ---- Level 2: Elementary (A2) ----
INSERT INTO [Situation] (SituationName, Description, ImageUrl, RoleAI, RoleUser, CreatedAt, TypeID, LevelID) VALUES
(
    N'At the Doctor''s Office',
    N'Practice describing symptoms and understanding medical advice at a clinic.',
    N'https://images.unsplash.com/photo-1576091160550-2173dba999ef?w=600',
    N'You are a general practitioner. Listen to symptoms, ask relevant questions, and give simple health advice.',
    N'You have had a headache and sore throat for two days. Visit the doctor and describe your symptoms.',
    GETDATE(), @RolePlayTypeId, 2
),
(
    N'Booking a Hotel Room',
    N'Practice booking accommodation. Learn reservation vocabulary.',
    N'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=600',
    N'You are a hotel receptionist. Help the guest book a room, explain room types, facilities, and pricing.',
    N'You want to book a double room for 3 nights. Ask about room types, breakfast, and cancellation policy.',
    GETDATE(), @RolePlayTypeId, 2
),
(
    N'Job Interview – Part-time Position',
    N'Practice a simple job interview for a part-time position.',
    N'https://images.unsplash.com/photo-1507679799987-c73779587ccf?w=600',
    N'You are an HR manager interviewing for a part-time cashier position. Ask basic questions about experience and availability.',
    N'You are applying for a part-time cashier job at a convenience store. Answer questions and show enthusiasm.',
    GETDATE(), @RolePlayTypeId, 2
),
(
    N'Returning a Defective Product',
    N'Practice complaining about a faulty product and requesting a refund or exchange.',
    N'https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=600',
    N'You are a customer service representative. Handle the return professionally and offer a solution.',
    N'You bought a blender last week but it stopped working. Go to the store to return it and ask for a replacement.',
    GETDATE(), @RolePlayTypeId, 2
),
(
    N'Planning a Weekend Trip',
    N'Practice discussing travel plans with a friend.',
    N'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=600',
    N'You are a travel enthusiast. Suggest destinations, activities, and help organize the trip.',
    N'You want to plan a weekend getaway with your friend. Discuss options for destination, budget, and activities.',
    GETDATE(), @RolePlayTypeId, 2
),
(
    N'Renting an Apartment',
    N'Practice inquiring about and negotiating apartment rental terms.',
    N'https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=600',
    N'You are a landlord showing an apartment. Describe its features, explain rental terms, and answer questions.',
    N'You are looking for a 1-bedroom apartment to rent. Ask about rent, utilities, lease terms, and amenities.',
    GETDATE(), @RolePlayTypeId, 2
);

-- ---- Level 3: Intermediate (B1) ----
INSERT INTO [Situation] (SituationName, Description, ImageUrl, RoleAI, RoleUser, CreatedAt, TypeID, LevelID) VALUES
(
    N'Negotiating a Salary',
    N'Practice negotiating your salary during a formal job offer discussion.',
    N'https://images.unsplash.com/photo-1521791136064-7986c2920216?w=600',
    N'You are a hiring manager who extended a job offer. Discuss the compensation package and respond to counter-offers.',
    N'You received a job offer but the salary is lower than expected. Negotiate professionally for a higher salary.',
    GETDATE(), @RolePlayTypeId, 3
),
(
    N'Giving Feedback to a Colleague',
    N'Practice delivering constructive feedback in a workplace setting.',
    N'https://images.unsplash.com/photo-1552664730-d307ca884978?w=600',
    N'You are a team member who submitted a report with errors. Listen to feedback and respond professionally.',
    N'You are a team leader giving constructive feedback about a colleague''s recent report. Be respectful but clear.',
    GETDATE(), @RolePlayTypeId, 3
),
(
    N'Discussing a Business Proposal',
    N'Practice presenting and discussing a business idea in a meeting.',
    N'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=600',
    N'You are an investor listening to a pitch. Ask probing questions about market, revenue, and risks.',
    N'You are presenting a business proposal for a new mobile app. Present your idea and answer questions.',
    GETDATE(), @RolePlayTypeId, 3
),
(
    N'Resolving a Customer Complaint',
    N'Practice handling a dissatisfied customer professionally.',
    N'https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=600',
    N'You are an unhappy customer who received the wrong order. Express dissatisfaction clearly.',
    N'You are a customer service manager handling an escalated complaint. Apologize and find a resolution.',
    GETDATE(), @RolePlayTypeId, 3
),
(
    N'Participating in a Team Meeting',
    N'Practice contributing ideas and responding to colleagues in a team meeting.',
    N'https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?w=600',
    N'You are a team leader facilitating the meeting. Keep discussion on track and encourage participation.',
    N'You are a team member attending a project planning meeting. Share ideas and respond to suggestions.',
    GETDATE(), @RolePlayTypeId, 3
),
(
    N'Consulting a Financial Advisor',
    N'Practice discussing personal finance and investment options.',
    N'https://images.unsplash.com/photo-1554224155-8d04cb21cd6c?w=600',
    N'You are a financial advisor. Listen to goals, explain investment options, and answer questions clearly.',
    N'You have savings and want advice on investing. Explain your goals and ask about suitable options.',
    GETDATE(), @RolePlayTypeId, 3
);

-- ---- Level 4: Upper-Intermediate (B2) ----
INSERT INTO [Situation] (SituationName, Description, ImageUrl, RoleAI, RoleUser, CreatedAt, TypeID, LevelID) VALUES
(
    N'Debate: Remote Work vs. Office Work',
    N'Practice debating the pros and cons of remote vs. traditional office work.',
    N'https://images.unsplash.com/photo-1600880292203-757bb62b4baf?w=600',
    N'You strongly support office work, arguing it improves collaboration and company culture. Defend your position.',
    N'You advocate for remote work. Argue its benefits for work-life balance and productivity.',
    GETDATE(), @RolePlayTypeId, 4
),
(
    N'Crisis Management Meeting',
    N'Practice managing a PR crisis in a high-pressure corporate setting.',
    N'https://images.unsplash.com/photo-1504711434969-e33886168f5c?w=600',
    N'You are a PR consultant advising the company during a social media backlash. Propose strategies.',
    N'You are the CEO facing a PR crisis after a product recall. Lead the crisis management discussion.',
    GETDATE(), @RolePlayTypeId, 4
),
(
    N'Medical Ethics Discussion',
    N'Practice discussing complex medical ethics scenarios with nuanced arguments.',
    N'https://images.unsplash.com/photo-1559757148-5c350d0d3c56?w=600',
    N'You are a medical ethicist with strong views on patient autonomy. Present arguments and challenge decisions.',
    N'You are a senior doctor discussing an ethically complex case about resource allocation in emergency care.',
    GETDATE(), @RolePlayTypeId, 4
),
(
    N'Cross-Cultural Business Negotiation',
    N'Practice navigating cultural differences in international business.',
    N'https://images.unsplash.com/photo-1556761175-b413da4baf72?w=600',
    N'You represent a Japanese corporation with formal, relationship-focused business culture. Build trust slowly.',
    N'You are an American entrepreneur negotiating a distribution partnership with a Japanese company.',
    GETDATE(), @RolePlayTypeId, 4
),
(
    N'Academic Research Presentation',
    N'Practice presenting research findings and defending your methodology.',
    N'https://images.unsplash.com/photo-1523580494863-6f3031224c94?w=600',
    N'You are a committee member reviewing the research. Ask critical questions about methodology and conclusions.',
    N'You are a PhD student presenting your research thesis. Explain methodology, findings, and implications.',
    GETDATE(), @RolePlayTypeId, 4
),
(
    N'Policy Debate: Climate Change Action',
    N'Practice a formal policy debate on climate change legislation.',
    N'https://images.unsplash.com/photo-1470071459604-3b5ec3a7fe05?w=600',
    N'You represent an industry lobby group concerned about economic impacts of strict regulations.',
    N'You are an environmental policy advisor arguing for immediate climate change legislation.',
    GETDATE(), @RolePlayTypeId, 4
);

-- ---- Level 5: Advanced (C1) ----
INSERT INTO [Situation] (SituationName, Description, ImageUrl, RoleAI, RoleUser, CreatedAt, TypeID, LevelID) VALUES
(
    N'Geopolitical Strategy Discussion',
    N'Practice high-level diplomatic negotiations on complex geopolitical issues.',
    N'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=600',
    N'You represent a rising economic power seeking greater influence in international trade agreements.',
    N'You are a senior diplomat navigating trade and security tensions with an emerging power.',
    GETDATE(), @RolePlayTypeId, 5
),
(
    N'Philosophical Discourse: AI and Consciousness',
    N'Practice an in-depth philosophical discussion on artificial intelligence and ethics.',
    N'https://images.unsplash.com/photo-1677442135703-1787eea5ce01?w=600',
    N'You are a philosopher arguing that AI consciousness is possible and AI could have moral status.',
    N'You are a cognitive scientist skeptical of machine consciousness. Challenge claims with evidence.',
    GETDATE(), @RolePlayTypeId, 5
),
(
    N'Supreme Court Mock Trial',
    N'Practice formal legal argumentation in a simulated appellate court.',
    N'https://images.unsplash.com/photo-1589829545856-d10d557cf95f?w=600',
    N'You are a Chief Justice asking probing questions about constitutional law and precedent.',
    N'You are lead counsel arguing a landmark case on digital privacy rights before the Supreme Court.',
    GETDATE(), @RolePlayTypeId, 5
),
(
    N'Corporate Merger Negotiation',
    N'Practice complex high-stakes M&A negotiation.',
    N'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=600',
    N'You represent the target company''s board. Protect shareholder value and negotiate terms carefully.',
    N'You are the CEO of an acquiring company pursuing a merger. Navigate legal and financial complexities.',
    GETDATE(), @RolePlayTypeId, 5
),
(
    N'UN Security Council Simulation',
    N'Practice formal UN-style multilateral diplomacy on a global crisis.',
    N'https://images.unsplash.com/photo-1598986646512-9330bcc4c0dc?w=600',
    N'You represent a nation with veto power that has conflicting strategic interests. Use formal UN language.',
    N'You are the rotating chair of the UN Security Council managing debate on a peacekeeping mission.',
    GETDATE(), @RolePlayTypeId, 5
),
(
    N'Tech IPO Press Conference',
    N'Practice responding to tough journalist questions at a high-profile tech IPO.',
    N'https://images.unsplash.com/photo-1504711434969-e33886168f5c?w=600',
    N'You are an investigative journalist asking hard-hitting questions about privacy, labor, and valuation.',
    N'You are the CEO at your IPO press conference. Answer difficult questions with confidence and transparency.',
    GETDATE(), @RolePlayTypeId, 5
);

GO

PRINT N'';
PRINT N'========================================';
PRINT N'Seed data inserted successfully!';
PRINT N'- Level: 5 records';
PRINT N'- Type: 4 records';
PRINT N'- Situation: 30 records (6 per level)';
PRINT N'========================================';
PRINT N'';
PRINT N'NOTE: ImageUrl hiện dùng ảnh Unsplash tạm thời.';
PRINT N'Sau khi upload ảnh thật lên Cloudinary, dùng lệnh sau để cập nhật:';
PRINT N'  UPDATE [Situation] SET ImageUrl = ''https://res.cloudinary.com/...'' WHERE SituatuonID = X';
