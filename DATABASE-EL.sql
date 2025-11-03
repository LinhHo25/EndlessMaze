-- TỰ ĐỘNG XÓA DATABASE NẾU ĐÃ TỒN TẠI
USE master;
GO
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EndlessMazeGameDB')
BEGIN
    ALTER DATABASE EndlessMazeGameDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EndlessMazeGameDB;
    PRINT N'Đã xóa Database EndlessMazeGameDB cũ.';
END
GO

-- TẠO DATABASE
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EndlessMazeGameDB')
BEGIN
    CREATE DATABASE EndlessMazeGameDB COLLATE Vietnamese_CI_AS;
    PRINT N'Đã tạo Database EndlessMazeGameDB mới.';
END
GO

USE EndlessMazeGameDB;
GO

-- ========= PHẦN 1: TÀI KHOẢN VÀ NHÂN VẬT =========

-- Bảng lưu tài khoản, mật khẩu người dùng (Theo yêu cầu)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL, -- Nên lưu mật khẩu đã hash
    DateCreated DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng lưu thông tin nhân vật CỐ ĐỊNH của người dùng (Theo yêu cầu)
-- Lưu trạng thái cơ bản khi mới tạo hoặc sau khi reset.
CREATE TABLE PlayerCharacters (
    CharacterID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT UNIQUE NOT NULL,
    CharacterName NVARCHAR(50) NOT NULL DEFAULT N'Nhà Thám Hiểm',
    BaseHealth INT NOT NULL DEFAULT 100,
    BaseAttack INT NOT NULL DEFAULT 20,
    BaseDefense INT NOT NULL DEFAULT 0,
    BaseStamina INT NOT NULL DEFAULT 100,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
    ON DELETE CASCADE -- Nếu xóa User, nhân vật cũng bị xóa
);
GO

-- ========= PHẦN 2: ĐỊNH NGHĨA VẬT PHẨM (ITEMS) =========

-- Bảng Giáp (Theo yêu cầu)
CREATE TABLE Armors (
    ArmorID INT PRIMARY KEY IDENTITY(1,1),
    ArmorName NVARCHAR(50) NOT NULL,
    ArmorRank CHAR(1) NOT NULL CHECK (ArmorRank IN ('D', 'C', 'B', 'A')),
    DefensePoints INT NOT NULL
);
GO

-- Bảng Vũ Khí (Theo yêu cầu)
CREATE TABLE Weapons (
    WeaponID INT PRIMARY KEY IDENTITY(1,1),
    WeaponName NVARCHAR(50) NOT NULL,
    WeaponRank CHAR(1) NOT NULL CHECK (WeaponRank IN ('D', 'C', 'B', 'A')),
    AttackBonus INT NOT NULL
);
GO

-- Bảng Thuốc (Theo yêu cầu)
CREATE TABLE Potions (
    PotionID INT PRIMARY KEY IDENTITY(1,1),
    PotionName NVARCHAR(50) NOT NULL,
    HealAmount INT NOT NULL,
    Description NVARCHAR(100)
);
GO

-- ========= PHẦN 3: ĐỊNH NGHĨA QUÁI VẬT (MONSTERS) =========

-- Bảng Quái vật (Theo yêu cầu)
CREATE TABLE Monsters (
    MonsterID INT PRIMARY KEY IDENTITY(1,1),
    MonsterName NVARCHAR(50) NOT NULL,
    Health INT NOT NULL,
    Attack INT NOT NULL
);
GO

-- ========= PHẦN 4: TÚI ĐỒ TẠM THỜI (TRONG GAME) =========

-- Bảng túi đồ tạm thời khi trong ván game (Theo yêu cầu)
-- Bảng này sẽ bị xóa (hoặc reset) khi kết thúc ván game.
-- Nó phản ánh đúng hướng dẫn "không dùng database trong game loop"
-- mà chỉ dùng để "LOAD" khi bắt đầu ván và "SAVE" (nếu cần) khi kết thúc.
CREATE TABLE PlayerSessionInventory (
    SessionInventoryID INT PRIMARY KEY IDENTITY(1,1),
    CharacterID INT NOT NULL,
    -- Giả sử ID 1 là Giáp D và ID 1 là Kiếm D
    EquippedArmorID INT NOT NULL DEFAULT 1, 
    EquippedWeaponID INT NOT NULL DEFAULT 1,
    HealthPotionCount INT NOT NULL DEFAULT 10,
    
    -- Chỉ cho phép 1 bản ghi túi đồ tạm thời cho mỗi nhân vật
    CONSTRAINT UQ_PlayerSessionInventory_CharacterID UNIQUE (CharacterID),
    FOREIGN KEY (CharacterID) REFERENCES PlayerCharacters(CharacterID) ON DELETE CASCADE,
    FOREIGN KEY (EquippedArmorID) REFERENCES Armors(ArmorID),
    FOREIGN KEY (EquippedWeaponID) REFERENCES Weapons(WeaponID)
);
GO

-- ========= PHẦN 5: TỈ LỆ RỚT ĐỒ (LOOT TABLES) =========

-- Bảng tỉ lệ rớt đồ từ Quái vật (Theo yêu cầu)
CREATE TABLE MonsterLootTables (
    LootTableID INT PRIMARY KEY IDENTITY(1,1),
    MonsterID INT NOT NULL,
    -- ItemType: 1 = Armor, 2 = Weapon
    ItemType INT NOT NULL CHECK (ItemType IN (1, 2)),
    -- ItemRank: 'D', 'C', 'B', 'A'
    ItemRank CHAR(1) NOT NULL,
    DropChance FLOAT NOT NULL CHECK (DropChance > 0 AND DropChance <= 1.0), -- Tỉ lệ từ 0.0 đến 1.0
    
    FOREIGN KEY (MonsterID) REFERENCES Monsters(MonsterID)
);
GO

-- Bảng tỉ lệ rớt đồ từ Rương (Theo yêu cầu)
CREATE TABLE ChestLootTables (
    LootTableID INT PRIMARY KEY IDENTITY(1,1),
    -- ItemType: 1 = Armor, 2 = Weapon, 3 = Potion
    ItemType INT NOT NULL CHECK (ItemType IN (1, 2, 3)),
    
    -- Nếu là Giáp/Vũ khí, dùng ItemRank
    ItemRank CHAR(1) NULL CHECK (ItemRank IN ('D', 'C', 'B', 'A')),
    
    -- Nếu là Thuốc, dùng PotionID (Giả sử PotionID 1 là thuốc Heal)
    PotionID INT NULL,
    
    -- Số lượng rớt (chủ yếu cho thuốc)
    MinQuantity INT NOT NULL DEFAULT 1,
    MaxQuantity INT NOT NULL DEFAULT 1,
    
    -- Tỉ lệ rớt ra vật phẩm/rank này
    DropChance FLOAT NOT NULL CHECK (DropChance > 0 AND DropChance <= 1.0)
);
GO


-- ========= PHẦN 6: BẢNG XẾP HẠNG (LEADERBOARDS) =========

-- Bảng xếp hạng Khám Phá (Theo yêu cầu)
CREATE TABLE LeaderboardExploration (
    ScoreID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Username NVARCHAR(50) NOT NULL, -- Lưu lại username để truy vấn nhanh hơn
    CompletionTimeSeconds INT NOT NULL,
    ItemsCollected INT NOT NULL,
    MapsCompleted INT NOT NULL,
    AchievedDate DATETIME NOT NULL DEFAULT GETDATE(),
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- Bảng xếp hạng Phiêu Lưu (Theo yêu cầu)
CREATE TABLE LeaderboardAdventure (
    ScoreID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Username NVARCHAR(50) NOT NULL, -- Lưu lại username để truy vấn nhanh hơn
    CompletionTimeSeconds INT NOT NULL,
    MonstersKilled INT NOT NULL,
    MapsCompleted INT NOT NULL,
    AchievedDate DATETIME NOT NULL DEFAULT GETDATE(),
    
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- ========= PHẦN 7: CHÈN DỮ LIỆU MẪU (SAMPLE DATA) =========
PRINT 'Bắt đầu chèn dữ liệu mẫu...';
GO

-- Chèn Giáp
INSERT INTO Armors (ArmorName, ArmorRank, DefensePoints) VALUES
(N'Giáp D', 'D', 20),
(N'Giáp C', 'C', 40),
(N'Giáp B', 'B', 75),
(N'Giáp A', 'A', 130);
GO

-- Chèn Vũ Khí
INSERT INTO Weapons (WeaponName, WeaponRank, AttackBonus) VALUES
(N'Kiếm D', 'D', 5),
(N'Kiếm C', 'C', 15),
(N'Kiếm B', 'B', 20),
(N'Kiếm A', 'A', 40);
GO

-- Chèn Thuốc
INSERT INTO Potions (PotionName, HealAmount, Description) VALUES
(N'Thuốc hồi máu', 40, N'Hồi 40 Máu ngay lập tức.');
GO

-- Chèn Quái vật
INSERT INTO Monsters (MonsterName, Health, Attack) VALUES
(N'Slime', 30, 5),
(N'Orc', 50, 10),
(N'Boss-Maze', 500, 20);
GO

-- Chèn Tỉ lệ rớt đồ (Giả định MonsterID: 1=Slime, 2=Orc)
-- ItemType: 1=Armor, 2=Weapon

-- Đồ rớt từ Slime (ID 1)
INSERT INTO MonsterLootTables (MonsterID, ItemType, ItemRank, DropChance) VALUES
(1, 1, 'D', 0.60), -- 60% Giáp D
(1, 1, 'C', 0.30), -- 30% Giáp C
(1, 1, 'B', 0.10), -- 10% Giáp B
(1, 2, 'D', 0.70), -- 70% Kiếm D
(1, 2, 'C', 0.20), -- 20% Kiếm C
(1, 2, 'B', 0.10); -- 10% Kiếm B
GO

-- Đồ rớt từ Orc (ID 2)
INSERT INTO MonsterLootTables (MonsterID, ItemType, ItemRank, DropChance) VALUES
(2, 1, 'C', 0.60), -- 60% Giáp C
(2, 1, 'B', 0.30), -- 30% Giáp B
(2, 1, 'A', 0.10), -- 10% Giáp A
(2, 2, 'D', 0.50), -- 50% Kiếm D
(2, 2, 'C', 0.30), -- 30% Kiếm C
(2, 2, 'B', 0.15), -- 15% Kiếm B
(2, 2, 'A', 0.05); -- 5% Kiếm A
GO

-- Chèn Tỉ lệ rớt đồ từ Rương (Tự thiết lập theo yêu cầu)
-- ItemType: 1=Armor, 2=Weapon, 3=Potion

-- Rớt Giáp (Yêu cầu: 5% ra A, còn lại tự set)
INSERT INTO ChestLootTables (ItemType, ItemRank, DropChance) VALUES
(1, 'A', 0.05), -- 5% Giáp A
(1, 'B', 0.25), -- 25% Giáp B
(1, 'C', 0.35), -- 35% Giáp C
(1, 'D', 0.35); -- 35% Giáp D
GO

-- Rớt Vũ Khí (Yêu cầu: 5% ra A, còn lại tự set)
INSERT INTO ChestLootTables (ItemType, ItemRank, DropChance) VALUES
(2, 'A', 0.05), -- 5% Kiếm A
(2, 'B', 0.20), -- 20% Kiếm B
(2, 'C', 0.40), -- 40% Kiếm C
(2, 'D', 0.35); -- 35% Kiếm D
GO

-- Rớt Thuốc (Yêu cầu: 100% ra ít nhất 1, 10% ra 4)
-- Giả định PotionID 1 là thuốc hồi máu
INSERT INTO ChestLootTables (ItemType, PotionID, MinQuantity, MaxQuantity, DropChance) VALUES
(3, 1, 4, 4, 0.10), -- 10% rớt 4 bình
(3, 1, 3, 3, 0.20), -- 20% rớt 3 bình
(3, 1, 2, 2, 0.30), -- 30% rớt 2 bình
(3, 1, 1, 1, 0.40); -- 40% rớt 1 bình
GO


-- Chèn dữ liệu mẫu cho người dùng và nhân vật
INSERT INTO Users (Username, PasswordHash) VALUES
('player_one', 'hashed_password_1'),
('gamer_vn', 'hashed_password_2');
GO

INSERT INTO PlayerCharacters (UserID, CharacterName) VALUES
(1, N'Chiến Binh')
-- Dấu phẩy đã bị xóa ở đây
GO

-- Chèn dữ liệu túi đồ tạm thời MẶC ĐỊNH cho nhân vật
-- (Giả định ArmorID 1 là Giáp D, WeaponID 1 là Kiếm D)
INSERT INTO PlayerSessionInventory (CharacterID, EquippedArmorID, EquippedWeaponID, HealthPotionCount) VALUES
(1, 1, 1, 10)
-- Dấu phẩy đã bị xóa ở đây
GO

-- Chèn điểm mẫu cho Bảng Xếp Hạng
INSERT INTO LeaderboardExploration (UserID, Username, CompletionTimeSeconds, ItemsCollected, MapsCompleted) VALUES
(1, 'player_one', 1200, 50, 3);
GO

INSERT INTO LeaderboardAdventure (UserID, Username, CompletionTimeSeconds, MonstersKilled, MapsCompleted) VALUES
(1, 'gamer_vn', 1850, 120, 5);
GO

PRINT N'Hoàn tất tạo Database và chèn dữ liệu mẫu!';
GO

