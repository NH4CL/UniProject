-- 本文结构，所有创建规则均按数据字典顺序
-- 1. CREATE LOGIN
-- 2. CREATE TABLE AND INDEX
-- 3. ALTER TALBE ADD CONSTRAINT
-- 4. CREATE VIEW
-- 5. CREATE TRIGGER
-- 6. CREATE FUNCTION
-- 7. CREATE PROCEDURE
-- 最近更新 2011-09-28 by 伯鉴

-- 创建登陆用户名
--USE [master];
--CREATE LOGIN [live] WITH PASSWORD = 'root', DEFAULT_DATABASE = [Zhuchao], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF;
--EXECUTE sys.sp_addsrvrolemember @loginame = N'live', @rolename = N'sysadmin'
--GO
--USE [Zhuchao]
--CREATE USER [live] FOR LOGIN [live]
--ALTER AUTHORIZATION ON SCHEMA::[db_owner] TO [live]
--EXEC sp_addrolemember N'db_owner', N'live'
--GO

USE [Zhuchao];
-- General模块数据库建表脚本
CREATE TABLE GeneralConfig (
    Gid uniqueidentifier CONSTRAINT DF_GeneralConfig_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralConfig_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralConfig_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralConfig_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (50) NOT NULL,
    Parent uniqueidentifier,
    Culture int NOT NULL CONSTRAINT DF_GeneralConfig_Culture DEFAULT 0,
    Ctype tinyint NOT NULL CONSTRAINT DF_GeneralConfig_Ctype DEFAULT 0,
    IntValue int,
    DecValue decimal (18, 4),
    StrValue nvarchar (256),
    DateValue datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralConfig PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralConfig_GeneralConfig FOREIGN KEY (Parent) REFERENCES GeneralConfig (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralConfig_Code ON GeneralConfig (Code) ON [PRIMARY];

CREATE TABLE GeneralResource (
    Gid uniqueidentifier CONSTRAINT DF_GeneralResource_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralResource_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralResource_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralResource_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Rtype tinyint NOT NULL CONSTRAINT DF_GeneralResource_Rtype DEFAULT 0,
    Code nvarchar (20),
    Culture int NOT NULL CONSTRAINT DF_GeneralResource_Culture DEFAULT 0,
    Matter nvarchar (512),
    Currency uniqueidentifier,
    Cash money NOT NULL CONSTRAINT DF_GeneralResource_Cash DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralResource PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralResItem (
    Gid uniqueidentifier CONSTRAINT DF_GeneralResItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralResItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralResItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralResItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ResID uniqueidentifier NOT NULL,
    Code nvarchar (20),
    Culture int NOT NULL CONSTRAINT DF_GeneralResItem_Culture DEFAULT 0,
    Matter nvarchar (512),
    Currency uniqueidentifier,
    Cash money NOT NULL CONSTRAINT DF_GeneralResItem_Cash DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralResItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralResItem_GeneralResource FOREIGN KEY (ResID) REFERENCES GeneralResource (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralLargeObject (
    Gid uniqueidentifier CONSTRAINT DF_GeneralLargeObject_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralLargeObject_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralLargeObject_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralLargeObject_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar(20),
    Culture int NOT NULL CONSTRAINT DF_GeneralLargeObject_Culture DEFAULT 2052,
    BLOB varbinary (MAX),
    CLOB nvarchar (MAX),
    FileType nvarchar (50),
    Source tinyint NOT NULL CONSTRAINT DF_GeneralLargeObject_Source DEFAULT 0,
    ObjUrl nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralLargeObject PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralLargeItem (
    Gid uniqueidentifier CONSTRAINT DF_GeneralLargeItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralLargeItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralLargeItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralLargeItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    BlobID uniqueidentifier NOT NULL,
    Culture int NOT NULL CONSTRAINT DF_GeneralLargeItem_Culture DEFAULT 2052,
    BLOB varbinary (MAX),
    CLOB nvarchar (MAX),
    FileType nvarchar (50),
    Source tinyint NOT NULL CONSTRAINT DF_GeneralLargeItem_Source DEFAULT 0,
    ObjUrl nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralLargeItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralLargeItem_GeneralLargeObject FOREIGN KEY (BlobID) REFERENCES GeneralLargeObject (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralLargeItem_BlobID_Culture ON GeneralLargeItem (BlobID, Culture) ON [PRIMARY];

CREATE TABLE GeneralOptional (
    Gid uniqueidentifier CONSTRAINT DF_GeneralOptional_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralOptional_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralOptional_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralOptional_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Otype tinyint NOT NULL CONSTRAINT DF_GeneralOptional_Otype DEFAULT 0,
    Code nvarchar (20) NOT NULL,
    RefID uniqueidentifier,
    Sorting int NOT NULL CONSTRAINT DF_GeneralOptional_Sorting DEFAULT 0,
    Name uniqueidentifier,
    InputMode tinyint NOT NULL CONSTRAINT DF_GeneralOptional_InputMode DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralOptional PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralOptional_OrgID_Code ON GeneralOptional (OrgID, Code) ON [PRIMARY];

CREATE TABLE GeneralOptItem (
    Gid uniqueidentifier CONSTRAINT DF_GeneralOptItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralOptItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralOptItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralOptItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OptID uniqueidentifier NOT NULL,
    Code nvarchar (20) NOT NULL,
    Sorting int NOT NULL CONSTRAINT DF_GeneralOptItem_Sorting DEFAULT 0,
    Name uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralOptItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralOptItem_GeneralOptional FOREIGN KEY (OptID) REFERENCES GeneralOptional (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralOptItem_OptID_Code ON GeneralOptItem (OptID, Code) ON [PRIMARY];

CREATE TABLE GeneralStandardCategory (
    Gid uniqueidentifier CONSTRAINT DF_GeneralStandardCategory_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralStandardCategory_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralStandardCategory_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralStandardCategory_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Ctype tinyint NOT NULL CONSTRAINT DF_GeneralStandardCategory_Ctype DEFAULT 0,
    Code nvarchar (20) NOT NULL,
    Parent uniqueidentifier,
    Name uniqueidentifier,
    Sorting int NOT NULL CONSTRAINT DF_GeneralStandardCategory_Sorting DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralStandardCategory PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralStandardCategory_GeneralStandardCategory FOREIGN KEY (Parent) REFERENCES GeneralStandardCategory (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralStandardCategory_Ctype_Code ON GeneralStandardCategory (Ctype, Code) ON [PRIMARY];

CREATE TABLE GeneralPrivateCategory (
    Gid uniqueidentifier CONSTRAINT DF_GeneralPrivateCategory_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralPrivateCategory_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralPrivateCategory_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralPrivateCategory_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Ctype tinyint NOT NULL CONSTRAINT DF_GeneralPrivateCategory_Ctype DEFAULT 0,
    Code nvarchar (20) NOT NULL,
    Parent uniqueidentifier,
    Name uniqueidentifier,
    Sorting int NOT NULL CONSTRAINT DF_GeneralPrivateCategory_Sorting DEFAULT 0,
    Show bit NOT NULL CONSTRAINT DF_GeneralPrivateCategory_Show DEFAULT 0,
    StdUnit uniqueidentifier,
    ShowGuarantee bit NOT NULL CONSTRAINT DF_GeneralPrivateCategory_ShowGuarantee DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralPrivateCategory PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralPrivateCategory_GeneralPrivateCategory FOREIGN KEY (Parent) REFERENCES GeneralPrivateCategory (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralPrivateCategory_OrgID_Ctype_Code ON GeneralPrivateCategory (OrgID, Ctype, Code) ON [PRIMARY];

CREATE TABLE GeneralRegion (
    Gid uniqueidentifier CONSTRAINT DF_GeneralRegion_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralRegion_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralRegion_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralRegion_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (50),
    Parent uniqueidentifier,
    FullName nvarchar (512),
    ShortName nvarchar (128),
    PostCode nvarchar (20),
    Map01 nvarchar (128),
    Map02 nvarchar (128),
    Map03 nvarchar (128),
    Map04 nvarchar (128),
    Map05 nvarchar (128),
    Sorting int NOT NULL CONSTRAINT DF_GeneralRegion_Sorting DEFAULT 0,
    RegionLevel int NOT NULL CONSTRAINT DF_GeneralRegion_RegionLevel DEFAULT 0,
    Statistics01 int NOT NULL CONSTRAINT DF_GeneralRegion_Statistics01 DEFAULT 0,
    Statistics02 int NOT NULL CONSTRAINT DF_GeneralRegion_Statistics02 DEFAULT 0,
    Statistics03 int NOT NULL CONSTRAINT DF_GeneralRegion_Statistics03 DEFAULT 0,
    Statistics04 int NOT NULL CONSTRAINT DF_GeneralRegion_Statistics04 DEFAULT 0,
    Statistics05 int NOT NULL CONSTRAINT DF_GeneralRegion_Statistics05 DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralRegion PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralRegion_GeneralRegion FOREIGN KEY (Parent) REFERENCES GeneralRegion (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralMeasureUnit (
    Gid uniqueidentifier CONSTRAINT DF_GeneralMeasureUnit_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralMeasureUnit_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMeasureUnit_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMeasureUnit_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Utype tinyint NOT NULL CONSTRAINT DF_GeneralMeasureUnit_Utype DEFAULT 0,
    Code nvarchar (20) NOT NULL,
    Name uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralMeasureUnit PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralMeasureUnit_Utype_Code ON GeneralMeasureUnit (Utype, Code) ON [PRIMARY];

CREATE TABLE GeneralCultureUnit (
    Gid uniqueidentifier CONSTRAINT DF_GeneralCultureUnit_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralCultureUnit_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralCultureUnit_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralCultureUnit_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Culture int NOT NULL CONSTRAINT DF_GeneralCultureUnit_Culture DEFAULT 2052,
    Piece uniqueidentifier,
    Weight uniqueidentifier,
    Volume uniqueidentifier,
    Fluid uniqueidentifier,
    Area uniqueidentifier,
    Linear uniqueidentifier,
    Currency uniqueidentifier,
    Other uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralCultureUnit PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralCultureUnit_Culture ON GeneralCultureUnit (Culture) ON [PRIMARY];

CREATE TABLE GeneralMessageTemplate (
    Gid uniqueidentifier CONSTRAINT DF_GeneralMessageTemplate_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralMessageTemplate_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessageTemplate_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessageTemplate_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    Matter uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralMessageTemplate PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralMessageTemplate_OrgID_Code ON GeneralMessageTemplate (OrgID, Code) ON [PRIMARY];

CREATE TABLE GeneralMessagePending (
    Gid uniqueidentifier CONSTRAINT DF_GeneralMessagePending_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralMessagePending_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessagePending_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessagePending_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier,
    Mtype tinyint NOT NULL CONSTRAINT DF_GeneralMessagePending_Mtype DEFAULT 0,
    Mstatus tinyint NOT NULL CONSTRAINT DF_GeneralMessagePending_Mstatus DEFAULT 0,
    Name nvarchar (256),
    Recipient nvarchar (256),
    Title nvarchar (256),
    Matter nvarchar (MAX),
    RefType tinyint NOT NULL CONSTRAINT DF_GeneralMessagePending_RefType DEFAULT 0,
    RefID uniqueidentifier,
    Schedule datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessagePending_Schedule DEFAULT sysdatetimeoffset(),
    SentTime datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralMessagePending PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralMessageReceive (
    Gid uniqueidentifier CONSTRAINT DF_GeneralMessageReceive_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralMessageReceive_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessageReceive_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessageReceive_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    SendFrom nvarchar (128),
    Matter nvarchar (MAX),
    SentTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralMessageReceive_Schedule DEFAULT sysdatetimeoffset(),
    GetFrom nvarchar (128),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralMessageReceive PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralIpBase (
    Gid uniqueidentifier CONSTRAINT DF_GeneralIpBase_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralIpBase_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralIpBase_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralIpBase_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    IpFrom bigint NOT NULL CONSTRAINT DF_GeneralIpBase_IpFrom DEFAULT 0,
    IpTo bigint NOT NULL CONSTRAINT DF_GeneralIpBase_IpTo DEFAULT 0,
    Country nvarchar (512),
    Province nvarchar (128),
    City nvarchar (256),
    Isp nvarchar (256),
    Source nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralIpBase PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralProgram (
    Gid uniqueidentifier CONSTRAINT DF_GeneralProgram_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralProgram_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralProgram_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralProgram_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (256) NOT NULL,
    Parent uniqueidentifier,
    Name uniqueidentifier,
    Sorting int NOT NULL CONSTRAINT DF_GeneralProgram_Sorting DEFAULT 0,
    ProgUrl nvarchar (256),
    Terminal bit NOT NULL CONSTRAINT DF_GeneralProgram_Terminal DEFAULT 0,
    Show bit NOT NULL CONSTRAINT DF_GeneralProgram_Show DEFAULT 1,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralProgram PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralProgram_GeneralProgram FOREIGN KEY (Parent) REFERENCES GeneralProgram (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralProgram_Code ON GeneralProgram (Code) ON [PRIMARY];

CREATE TABLE GeneralProgNode (
    Gid uniqueidentifier CONSTRAINT DF_GeneralProgNode_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralProgNode_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralProgNode_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralProgNode_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ProgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    InputMode tinyint NOT NULL CONSTRAINT DF_GeneralProgNode_InputMode DEFAULT 0,
    Optional uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralProgNode PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralProgNode_GeneralProgram FOREIGN KEY (ProgID) REFERENCES GeneralProgram (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralProgNode_ProdID_Code ON GeneralProgNode (ProgID, Code) ON [PRIMARY];

CREATE TABLE GeneralPrivTemplate (
    Gid uniqueidentifier CONSTRAINT DF_GeneralPrivTemplate_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralPrivTemplate_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralPrivTemplate_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralPrivTemplate_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (128) NOT NULL,
    Ptype tinyint NOT NULL CONSTRAINT DF_GeneralPrivTemplate_Ptype DEFAULT 0,
    Pstatus tinyint NOT NULL CONSTRAINT DF_GeneralPrivTemplate_Pstatus DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralPrivTemplate PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralPrivTemplate_Code_Ptype ON GeneralPrivTemplate (Code, Ptype) ON [PRIMARY];

CREATE TABLE GeneralPrivItem (
    Gid uniqueidentifier CONSTRAINT DF_GeneralPrivItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralPrivItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralPrivItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralPrivItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PrivID uniqueidentifier NOT NULL,
    RefID uniqueidentifier,
    NodeCode nvarchar (50),
    NodeValue nvarchar (50),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralPrivItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_GeneralPrivItem_GeneralPrivTemplate FOREIGN KEY (PrivID) REFERENCES GeneralPrivTemplate (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralAction (
    Gid uniqueidentifier CONSTRAINT DF_GeneralAction_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralAction_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralAction_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralAction_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ActID int NOT NULL CONSTRAINT DF_GeneralAction_ActID DEFAULT 0,
    Grade tinyint NOT NULL CONSTRAINT DF_GeneralAction_Grade DEFAULT 0,
    Source tinyint NOT NULL CONSTRAINT DF_GeneralAction_Source DEFAULT 0,
    ClassName nvarchar (256),
    UserID uniqueidentifier,
    Atype tinyint NOT NULL CONSTRAINT DF_GeneralAction_Atype DEFAULT 0,
    RefType tinyint NOT NULL CONSTRAINT DF_GeneralAction_RefType DEFAULT 0,
    RefID uniqueidentifier,
    Matter nvarchar (512),
    Keyword nvarchar (128),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralAction PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE GeneralErrorReport (
    Gid uniqueidentifier CONSTRAINT DF_GeneralErrorReport_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralErrorReport_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralErrorReport_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralErrorReport_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (50),
    UserID uniqueidentifier NOT NULL,
    ProgID uniqueidentifier,
    Title nvarchar (256),
    Matter nvarchar (MAX),
    Keyword nvarchar (128),
    Attached01 nvarchar (256),
    Attached02 nvarchar (256),
    Programmer nvarchar (128),
    Estatus tinyint NOT NULL CONSTRAINT DF_GeneralErrorReport_Estatus DEFAULT 0,
    Comment nvarchar (256),
    Technology nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralErrorReport PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralErrorReport_Code ON GeneralErrorReport (Code) ON [PRIMARY];

CREATE TABLE GeneralTodoList (
    Gid uniqueidentifier CONSTRAINT DF_GeneralTodoList_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_GeneralTodoList_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralTodoList_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_GeneralTodoList_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (50),
    Ttype tinyint NOT NULL CONSTRAINT DF_GeneralTodoList_Ttype DEFAULT 0,
    Tstatus tinyint NOT NULL CONSTRAINT DF_GeneralTodoList_Tstatus DEFAULT 0,
    Title nvarchar (256),
    Keyword nvarchar (128),
    Technology nvarchar (512),
    JumpUrl nvarchar (128),
    Remark nvarchar (256),
    CONSTRAINT PK_GeneralTodoList PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_GeneralTodoList_Code ON GeneralTodoList (Code) ON [PRIMARY];

GO


-- Member模块数据库建表脚本
CREATE TABLE MemberOrganization (
    Gid uniqueidentifier CONSTRAINT DF_MemberOrganization_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberOrganization_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrganization_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrganization_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    Code nvarchar (50) NOT NULL,
    ExCode nvarchar (50),
    Ostatus tinyint NOT NULL CONSTRAINT DF_MemberOrganization_Ostatus DEFAULT 0,
    Otype tinyint NOT NULL CONSTRAINT DF_MemberOrganization_Otype DEFAULT 0,
    ExType uniqueidentifier,
    Parent uniqueidentifier,
    Terminal bit NOT NULL CONSTRAINT DF_MemberOrganization_Terminal DEFAULT 0,
    FullName uniqueidentifier,
    ShortName uniqueidentifier,
    Location uniqueidentifier,
    FullAddress nvarchar (512),
    PostCode nvarchar (20),
    Contact nvarchar (128),
    CellPhone nvarchar (50),
    WorkPhone nvarchar (50),
    WorkFax nvarchar (50),
    Email nvarchar (256),
    HomeUrl nvarchar (256),
    Sorting int NOT NULL CONSTRAINT DF_MemberOrganization_Sorting DEFAULT 0,
    Brief nvarchar (512),
    Introduction uniqueidentifier,
    ProdCodePolicy nvarchar (256),
    SkuCodePolicy nvarchar (256),
    BarcodePolicy nvarchar (256),
    Discriminator nvarchar (128),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberOrganization PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberOrganization_MemberOrganization FOREIGN KEY (Parent) REFERENCES MemberOrganization (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberOrganization_Code ON MemberOrganization (Code) ON [PRIMARY];

CREATE TABLE MemberOrgChannel (
    Gid uniqueidentifier CONSTRAINT DF_MemberOrgChannel_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberOrgChannel_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrgChannel_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrgChannel_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    Cstatus tinyint NOT NULL CONSTRAINT DF_MemberOrgChannel_Cstatus DEFAULT 0,
    RemoteUrl nvarchar (128),
    ConfigKey nvarchar (128),
    SecretKey nvarchar (128),
    SessionKey nvarchar (128),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberOrgChannel PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberOrgChannel_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid),
    CONSTRAINT FK_MemberOrgChannel_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberOrgChannel_OrgID_ChlID ON MemberOrgChannel (OrgID, ChlID) ON [PRIMARY];

CREATE TABLE MemberOrgCulture (
    Gid uniqueidentifier CONSTRAINT DF_MemberOrgCulture_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberOrgCulture_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrgCulture_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrgCulture_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Ctype tinyint NOT NULL CONSTRAINT DF_MemberOrgCulture_Ctype DEFAULT 0,
    Culture uniqueidentifier,
    Currency uniqueidentifier,
    Sorting int NOT NULL CONSTRAINT DF_MemberOrgCulture_Sorting DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MemberOrgCulture PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberOrgCulture_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid)
) ON [PRIMARY];

CREATE TABLE MemberOrgAttribute (
    Gid uniqueidentifier CONSTRAINT DF_MemberOrgAttribute_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberOrgAttribute_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrgAttribute_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberOrgAttribute_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    OptID uniqueidentifier NOT NULL,
    OptResult uniqueidentifier,
    Matter nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberOrgAttribute PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberOrgAttribute_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberOrgAttribute_OrgID_OptID ON MemberOrgAttribute (OrgID, OptID) ON [PRIMARY];

CREATE TABLE MemberRole (
    Gid uniqueidentifier CONSTRAINT DF_MemberRole_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberRole_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberRole_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberRole_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Rtype uniqueidentifier,
    Parent uniqueidentifier,
    Name uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_MemberRole PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberRole_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberRole_OrgID_Code ON MemberRole (OrgID, Code) ON [PRIMARY];

CREATE TABLE MemberUser (
    Gid uniqueidentifier CONSTRAINT DF_MemberUser_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberUser_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUser_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUser_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    RoleID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    LoginName nvarchar (128) NOT NULL,
    Manager uniqueidentifier,
    ExCode nvarchar (50),
    LuckyNumber int NOT NULL CONSTRAINT DF_MemberUser_LuckyNumber DEFAULT 0,
    Authenticate bit NOT NULL CONSTRAINT DF_MemberUser_Authenticate DEFAULT 0,
    Ustatus tinyint NOT NULL CONSTRAINT DF_MemberUser_Ustatus DEFAULT 0,
	UserLevel uniqueidentifier,
    NickName nvarchar (128),
    FirstName nvarchar (128),
    LastName nvarchar (128),
    DisplayName nvarchar (256),
    Passcode nvarchar (256) NOT NULL,
    SaltKey nchar (8) NOT NULL,
    Culture uniqueidentifier,
    Title nvarchar (10),
    Gender tinyint NOT NULL CONSTRAINT DF_MemberUser_Gender DEFAULT 3,
    HeadPic nvarchar (256),
    UserSign nvarchar (512),
    Brief nvarchar (512),
    Birthday datetimeoffset (0),
    Telephone nvarchar (50),
    CellPhone nvarchar (50),
    Email nvarchar (256),
    LastLoginTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUser_LastLoginTime DEFAULT sysdatetimeoffset(),
    LastLoginIP nvarchar (20),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberUser PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberUser_MemberUser FOREIGN KEY (Manager) REFERENCES MemberUser (Gid),
    CONSTRAINT FK_MemberUser_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid),
    CONSTRAINT FK_MemberUser_MemberRole FOREIGN KEY (RoleID) REFERENCES MemberRole (Gid),
    CONSTRAINT FK_MemberUser_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberUser_LoginName ON MemberUser (LoginName) ON [PRIMARY];

CREATE TABLE MemberAddress (
    Gid uniqueidentifier CONSTRAINT DF_MemberAddress_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberAddress_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberAddress_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberAddress_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier NOT NULL,
    Code nvarchar (128) NOT NULL,
    IsDefault bit NOT NULL CONSTRAINT DF_MemberAddress_IsDefault DEFAULT 0,
    FirstName nvarchar (128),
    LastName nvarchar (128),
    DisplayName nvarchar (256),
    Location uniqueidentifier,
    FullAddress nvarchar (512),
    PostCode nvarchar (20),
    CellPhone nvarchar (50),
    WorkPhone nvarchar (50),
    WorkFax nvarchar (50),
    HomePhone nvarchar (50),
    Email nvarchar (128),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberAddress PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberAddress_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];

CREATE TABLE MemberUserAttribute (
    Gid uniqueidentifier CONSTRAINT DF_MemberUserAttribute_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberUserAttribute_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUserAttribute_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUserAttribute_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier NOT NULL,
    OptID uniqueidentifier NOT NULL,
    OptResult uniqueidentifier,
    Matter nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberUserAttribute PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberUserAttribute_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];

CREATE TABLE MemberPoint (
    Gid uniqueidentifier CONSTRAINT DF_MemberPoint_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberPoint_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPoint_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPoint_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    UserID uniqueidentifier NOT NULL,
    Ptype tinyint NOT NULL CONSTRAINT DF_MemberPoint_Ptype DEFAULT 0,
    Pstatus tinyint NOT NULL CONSTRAINT DF_MemberPoint_Pstatus DEFAULT 0,
    Reason nvarchar (256),
    PromID uniqueidentifier,
    RefType tinyint NOT NULL CONSTRAINT DF_MemberPoint_RefType DEFAULT 0,
    RefID uniqueidentifier,
    Score int NOT NULL CONSTRAINT DF_MemberPoint_Score DEFAULT 0,
    Remain int NOT NULL CONSTRAINT DF_MemberPoint_Remain DEFAULT 0,
    CouponID uniqueidentifier,
    Currency uniqueidentifier,
    Amount money NOT NULL CONSTRAINT DF_MemberPoint_Amount DEFAULT 0,
    Balance money NOT NULL CONSTRAINT DF_MemberPoint_Balance DEFAULT 0,
    MinCharge money NOT NULL CONSTRAINT DF_MemberPoint_MinCharge DEFAULT 0,
    Cashier bit NOT NULL CONSTRAINT DF_MemberPoint_Cashier DEFAULT 0,
    OnceUse bit NOT NULL CONSTRAINT DF_MemberPoint_OnceUse DEFAULT 0,
    Together bit NOT NULL CONSTRAINT DF_MemberPoint_Together DEFAULT 0,
    StartTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPoint_StartTime DEFAULT sysdatetimeoffset(),
    EndTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPoint_EndTime DEFAULT sysdatetimeoffset(),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberPoint PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberPoint_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];


CREATE TABLE MemberUsePoint (
    Gid uniqueidentifier CONSTRAINT DF_MemberUsePoint_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberUsePoint_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUsePoint_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUsePoint_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PointID uniqueidentifier NOT NULL,
    Pstatus tinyint NOT NULL CONSTRAINT DF_MemberUsePoint_Pstatus DEFAULT 0,
    RefType tinyint NOT NULL CONSTRAINT DF_MemberUsePoint_RefType DEFAULT 0,
    RefID uniqueidentifier,
    Comment nvarchar (128),
    Score int NOT NULL CONSTRAINT DF_MemberUsePoint_Score DEFAULT 0,
    Amount money NOT NULL CONSTRAINT DF_MemberUsePoint_Amount DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MemberUsePoint PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberUsePoint_MemberPoint FOREIGN KEY (PointID) REFERENCES MemberPoint (Gid)
) ON [PRIMARY];

CREATE TABLE MemberLevel (
    Gid uniqueidentifier CONSTRAINT DF_MemberLevel_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberLevel_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberLevel_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberLevel_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    Mlevel tinyint NOT NULL CONSTRAINT DF_MemberLevel_Mlevel DEFAULT 0,
    Discount decimal (18,4) NOT NULL CONSTRAINT DF_MemberLevel_Discount DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MemberLevel PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberLevel_OrgID_Code ON MemberLevel (OrgID, Code) ON [PRIMARY];

CREATE TABLE MemberSubscribe (
    Gid uniqueidentifier CONSTRAINT DF_MemberSubscribe_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberSubscribe_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberSubscribe_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberSubscribe_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier NOT NULL,
    ShortMessage bit NOT NULL CONSTRAINT DF_MemberSubscribe_ShortMessage DEFAULT 1,
    Email bit NOT NULL CONSTRAINT DF_MemberSubscribe_Email DEFAULT 1,
    Bulletin bit NOT NULL CONSTRAINT DF_MemberSubscribe_Bulletin DEFAULT 0,
    OrderConfirm bit NOT NULL CONSTRAINT DF_MemberSubscribe_OrderConfirm DEFAULT 0,
    OrderDelivery bit NOT NULL CONSTRAINT DF_MemberSubscribe_OrderDelivery DEFAULT 1,
    ShortSupply bit NOT NULL CONSTRAINT DF_MemberSubscribe_ShortSupply DEFAULT 1,
    Promotion bit NOT NULL CONSTRAINT DF_MemberSubscribe_Promotion DEFAULT 0,
    Discount bit NOT NULL CONSTRAINT DF_MemberSubscribe_Discount DEFAULT 0,
    UnionNotice bit NOT NULL CONSTRAINT DF_MemberSubscribe_UnionNotice DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MemberSubscribe PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberSubscribe_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];

CREATE TABLE MemberUserEvent (
    Gid uniqueidentifier CONSTRAINT DF_MemberUserEvent_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberUserEvent_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUserEvent_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUserEvent_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier NOT NULL,
    Etype tinyint NOT NULL CONSTRAINT DF_MemberUserEvent_Etype DEFAULT 0,
    Reminder datetimeoffset (0),
    Source nvarchar (128),
    Destination nvarchar (128),
    Matter nvarchar (512),
    RefType tinyint NOT NULL CONSTRAINT DF_MemberUserEvent_RefType DEFAULT 0,
    RefID uniqueidentifier,	
    Remark nvarchar (256),
    CONSTRAINT PK_MemberUserEvent PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberUserEvent_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];

CREATE TABLE MemberUserShortcut (
    Gid uniqueidentifier CONSTRAINT DF_MemberUserShortcut_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberUserShortcut_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUserShortcut_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberUserShortcut_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier NOT NULL,
    Stype tinyint NOT NULL CONSTRAINT DF_MemberUserShortcut_Stype DEFAULT 0,
    ProgID uniqueidentifier,
    LinkUrl nvarchar (256),
    Sorting int,
    Icon nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberUserShortcut PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberUserShortcut_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];

CREATE TABLE MemberPrivilege (
    Gid uniqueidentifier CONSTRAINT DF_MemberPrivilege_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberPrivilege_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPrivilege_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPrivilege_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    UserID uniqueidentifier NOT NULL,
    Ptype tinyint NOT NULL CONSTRAINT DF_MemberPrivilege_Ptype DEFAULT 0,
    Pstatus tinyint NOT NULL CONSTRAINT DF_MemberPrivilege_Pstatus DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MemberPrivilege PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberPrivilege_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MemberPrivilege_UserID_Ptype ON MemberPrivilege (UserID, Ptype) ON [PRIMARY];

CREATE TABLE MemberPrivItem (
    Gid uniqueidentifier CONSTRAINT DF_MemberPrivItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MemberPrivItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPrivItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MemberPrivItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PrivID uniqueidentifier NOT NULL,
    RefID uniqueidentifier,
    NodeCode nvarchar (50),
    NodeValue nvarchar (50),
    Remark nvarchar (256),
    CONSTRAINT PK_MemberPrivItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MemberPrivItem_MemberPrivilege FOREIGN KEY (PrivID) REFERENCES MemberPrivilege (Gid)
) ON [PRIMARY];

-- Product模块数据库建表脚本
CREATE TABLE ProductInformation (
    Gid uniqueidentifier CONSTRAINT DF_ProductInformation_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductInformation_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductInformation_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductInformation_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier NOT NULL,
    StdCatID uniqueidentifier,
    Block tinyint NOT NULL CONSTRAINT DF_ProductInformation_Block DEFAULT 0,
    Mode tinyint NOT NULL CONSTRAINT DF_ProductInformation_Mode DEFAULT 0,
    Picture nvarchar (256),
    Brief uniqueidentifier,
    Matter uniqueidentifier,
    MinQuantity decimal (18,4),
    ProductionCycle int NOT NULL CONSTRAINT DF_ProductInformation_ProductionCycle DEFAULT 0,
    GuaranteeDays int NOT NULL CONSTRAINT DF_ProductInformation_GuaranteeDays DEFAULT 0,
    Keywords nvarchar (256),
    SaleType tinyint NOT NULL CONSTRAINT DF_ProductInformation_SaleType DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductInformation PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductInformation_OrgID_Code ON ProductInformation (OrgID, Code) ON [PRIMARY];

CREATE TABLE ProductInfoItem (
    Gid uniqueidentifier CONSTRAINT DF_ProductInfoItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductInfoItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductInfoItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductInfoItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ProdID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Barcode nvarchar (50),
    CodeEx1 nvarchar (50),
    CodeEx2 nvarchar (50),
    CodeEx3 nvarchar (50),
    FullName uniqueidentifier,
    ShortName uniqueidentifier,
    StdUnit uniqueidentifier NOT NULL,
    Specification uniqueidentifier,
    Percision tinyint NOT NULL CONSTRAINT DF_ProductInfoItem_Percision DEFAULT 0,
    MarketPrice uniqueidentifier,
    SuggestPrice uniqueidentifier,
    LowestPrice uniqueidentifier,
    GrossWeight decimal (18,4) NOT NULL CONSTRAINT DF_ProductInfoItem_GrossWeight DEFAULT 0,
    NetWeight decimal (18,4) NOT NULL CONSTRAINT DF_ProductInfoItem_NetWeight DEFAULT 0,
    GrossVolume decimal (18,4) NOT NULL CONSTRAINT DF_ProductInfoItem_GrossVolume DEFAULT 0,
    NetVolume decimal (18,4) NOT NULL CONSTRAINT DF_ProductInfoItem_NetVolume DEFAULT 0,
    NetArea decimal (18,4) NOT NULL CONSTRAINT DF_ProductInfoItem_NetArea DEFAULT 0,
    NetPiece int NOT NULL CONSTRAINT DF_ProductInfoItem_NetPiece DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductInfoItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductInfoItem_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductInfoItem_OrgID_Code ON ProductInfoItem (OrgID, Code) ON [PRIMARY];
CREATE UNIQUE INDEX IX_ProductInfoItem_OrgID_Barcode ON ProductInfoItem (OrgID, Barcode) ON [PRIMARY];

CREATE TABLE ProductExtendCategory (
    Gid uniqueidentifier CONSTRAINT DF_ProductExtendCategory_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductExtendCategory_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductExtendCategory_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductExtendCategory_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ProdID uniqueidentifier NOT NULL,
    PrvCatID uniqueidentifier NOT NULL,
    IsDefault bit NOT NULL CONSTRAINT DF_ProductExtendCategory_IsDefault DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductExtendCategory PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductExtendCategory_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductExtendCategory_ProdID_PrvCatID ON ProductExtendCategory (ProdID, PrvCatID) ON [PRIMARY];

CREATE TABLE ProductExtendAttribute (
    Gid uniqueidentifier CONSTRAINT DF_ProductExtendAttribute_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductExtendAttribute_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductExtendAttribute_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductExtendAttribute_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ProdID uniqueidentifier NOT NULL,
    OptID uniqueidentifier NOT NULL,
    OptResult uniqueidentifier,
    Matter nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_ProductExtendAttribute PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductExtendAttribute_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductExtendAttribute_ProdID_OptID ON ProductExtendAttribute (ProdID, OptID) ON [PRIMARY];

CREATE TABLE ProductGallery (
    Gid uniqueidentifier CONSTRAINT DF_ProductGallery_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductGallery_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductGallery_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductGallery_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ProdID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier,
    Gtype tinyint NOT NULL CONSTRAINT DF_ProductGallery_Gtype DEFAULT 0,
    Code nvarchar (50),
    Sorting int NOT NULL CONSTRAINT DF_ProductGallery_Sorting DEFAULT 0,
    Enlarge nvarchar (256),
    Thumburl nvarchar (256),
    Thumbnail nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_ProductGallery PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductGallery_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid)
) ON [PRIMARY];

CREATE TABLE ProductOnSale (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnSale_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnSale_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnSale_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnSale_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ProdID uniqueidentifier NOT NULL,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    Code nvarchar (20) NOT NULL,
    Ostatus tinyint NOT NULL CONSTRAINT DF_ProductOnSale_Ostatus DEFAULT 0,
    Name uniqueidentifier NOT NULL,
    Mode tinyint NOT NULL CONSTRAINT DF_ProductOnSale_Mode DEFAULT 0,
    MarketPrice uniqueidentifier,
    SalePrice uniqueidentifier,
    Validity datetimeoffset (0),
    CanSplit bit NOT NULL CONSTRAINT DF_ProductOnSale_CanSplit DEFAULT 0,
    Brief uniqueidentifier,
    Matter uniqueidentifier,
    Picture nvarchar (256),
    VideoUrl nvarchar (256),
    DeliveryDays int NOT NULL CONSTRAINT DF_ProductOnSale_DeliveryDays DEFAULT 0,
    SortingNew int NOT NULL CONSTRAINT DF_ProductOnSale_SortingNew DEFAULT 0,
    SortingClick int NOT NULL CONSTRAINT DF_ProductOnSale_SortingClick DEFAULT 0,
    SortingHot int NOT NULL CONSTRAINT DF_ProductOnSale_SortingHot DEFAULT 0,
    SortingPush int NOT NULL CONSTRAINT DF_ProductOnSale_SortingPush DEFAULT 0,
    Sorting01 int NOT NULL CONSTRAINT DF_ProductOnSale_Sorting01 DEFAULT 0,
    Sorting02 int NOT NULL CONSTRAINT DF_ProductOnSale_Sorting02 DEFAULT 0,
    Sorting03 int NOT NULL CONSTRAINT DF_ProductOnSale_Sorting03 DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnSale PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnSale_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnSale_ProdID_ChlID_Code ON ProductOnSale (ProdID, ChlID, Code) ON [PRIMARY];

CREATE TABLE ProductOnItem (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSaleID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    FullName uniqueidentifier,
    ShortName uniqueidentifier,
    Sorting int NOT NULL CONSTRAINT DF_ProductOnItem_Sorting DEFAULT 0,
    SetQuantity decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnItem_SetQuantity DEFAULT 1,
    MaxQuantity decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnItem_MaxQuantity DEFAULT -1,
    OnTheWay bit NOT NULL CONSTRAINT DF_ProductOnItem_OnTheWay DEFAULT 0,
    Overflow bit NOT NULL CONSTRAINT DF_ProductOnItem_Overflow DEFAULT 0,
    DependTag tinyint NOT NULL CONSTRAINT DF_ProductOnSale_DependTag DEFAULT 0,
    DependRate decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnItem_DependRate DEFAULT 1,
    UseScore int NOT NULL CONSTRAINT DF_ProductOnItem_UseScore DEFAULT 0,
    ScoreDeduct uniqueidentifier,
    GetScore int NOT NULL CONSTRAINT DF_ProductOnItem_GetScore DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnItem_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnItem_OnSaleID_SkuID ON ProductOnItem (OnSaleID, SkuID) ON [PRIMARY];

CREATE TABLE ProductOnUnitPrice (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnUnitPrice_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnUnitPrice_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnUnitPrice_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnUnitPrice_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSkuID uniqueidentifier NOT NULL,
    ShowUnit uniqueidentifier NOT NULL,
    IsDefault bit NOT NULL CONSTRAINT DF_ProductOnUnitPrice_IsDefault DEFAULT 0,
    UnitRatio decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnUnitPrice_UnitRatio DEFAULT 1,
    Percision tinyint NOT NULL CONSTRAINT DF_ProductOnUnitPrice_Percision DEFAULT 0,
    MarketPrice uniqueidentifier,
    SalePrice uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnUnitPrice PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnUnitPrice_ProductOnItem FOREIGN KEY (OnSkuID) REFERENCES ProductOnItem (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnUnitPrice_OnSkuID_ShowUnit ON ProductOnUnitPrice (OnSkuID, ShowUnit) ON [PRIMARY];

CREATE TABLE ProductOnLevelDiscount (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnLevelDiscount_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnLevelDiscount_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnLevelDiscount_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnLevelDiscount_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSaleID uniqueidentifier NOT NULL,
    UserLevel uniqueidentifier NOT NULL,
    Discount decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnLevelDiscount_Discount DEFAULT 1,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnLevelDiscount PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnLevelDiscount_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnLevelDiscount_OnSaleID_UserLevel ON ProductOnLevelDiscount (OnSaleID, UserLevel) ON [PRIMARY];

CREATE TABLE ProductOnTemplate (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnTemplate_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnTemplate_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnTemplate_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnTemplate_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    ShipPolicy nvarchar (max),
    PayPolicy nvarchar (max),
    Relation nvarchar (max),
    LevelDiscount nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnTemplate PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnTemplate_OrgID_Code ON ProductOnTemplate (OrgID, Code) ON [PRIMARY];

CREATE TABLE ProductOnShipping (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnShipping_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnShipping_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnShipping_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnShipping_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSaleID uniqueidentifier NOT NULL,
    ShipID uniqueidentifier NOT NULL,
    Solution tinyint NOT NULL CONSTRAINT DF_ProductOnShipping_Solution DEFAULT 0,
    ShipWeight int NOT NULL CONSTRAINT DF_ProductOnShipping_Weight DEFAULT 0,
    Condition decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnShipping_Condition DEFAULT 0,
    Discount decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnShipping_Discount DEFAULT 1,
    SupportCod bit NOT NULL CONSTRAINT DF_ProductOnShipping_SupportCod DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnShipping PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnShipping_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnShipping_OnSaleID_ShipID ON ProductOnShipping (OnSaleID, ShipID) ON [PRIMARY];

CREATE TABLE ProductOnShipArea (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnShipArea_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnShipArea_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnShipArea_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnShipArea_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnShip uniqueidentifier NOT NULL,
    RegionID uniqueidentifier NOT NULL,
    Terminal bit NOT NULL CONSTRAINT DF_ProductOnShipArea_Terminal DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnShipArea PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnShipArea_ProductOnShipping FOREIGN KEY (OnShip) REFERENCES ProductOnShipping (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnShipArea_OnShip_RegionID ON ProductOnShipArea (OnShip, RegionID) ON [PRIMARY];

CREATE TABLE ProductOnPayment (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnPayment_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnPayment_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnPayment_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnPayment_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSaleID uniqueidentifier NOT NULL,
    PayID uniqueidentifier NOT NULL,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnPayment PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnPayment_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnPayment_OnSaleID_PayID ON ProductOnPayment (OnSaleID, PayID) ON [PRIMARY];

CREATE TABLE ProductOnRelation (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnRelation_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnRelation_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnRelation_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnRelation_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSaleID uniqueidentifier NOT NULL,
    OnRelation uniqueidentifier NOT NULL,
    Rtype tinyint NOT NULL CONSTRAINT DF_ProductOnRelation_Rtype DEFAULT 0,
    Sorting int NOT NULL CONSTRAINT DF_ProductOnRelation_Sorting DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnRelation PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnRelation_ProductOnSale_OnSaleID FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid),
    CONSTRAINT FK_ProductOnRelation_ProductOnSale_OnRelation FOREIGN KEY (OnRelation) REFERENCES ProductOnSale (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ProductOnRelation_OnSaleID_OnRelation ON ProductOnRelation (OnSaleID, OnRelation) ON [PRIMARY];

CREATE TABLE ProductOnAdjust (
    Gid uniqueidentifier CONSTRAINT DF_ProductOnAdjust_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ProductOnAdjust_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnAdjust_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnAdjust_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OnSaleID uniqueidentifier NOT NULL,
    OnSkuID uniqueidentifier NOT NULL,
    Astatus tinyint NOT NULL CONSTRAINT DF_ProductOnAdjust_Astatus DEFAULT 0,
    PreparedBy uniqueidentifier,
    ApprovedBy uniqueidentifier,
    ApprovedTime datetimeoffset (0),
    StartTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ProductOnAdjust_StartTime DEFAULT sysdatetimeoffset(),
    RestoreTime datetimeoffset (0),
    BeRestore bit NOT NULL CONSTRAINT DF_ProductOnAdjust_BeRestore DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_ProductOnAdjust_Quantity DEFAULT 0,
    SalePriceOld uniqueidentifier,
    UseScoreOld int NOT NULL CONSTRAINT DF_ProductOnAdjust_UseScoreOld DEFAULT 0,
    ScoreDeductOld uniqueidentifier,
    GetScoreOld int NOT NULL CONSTRAINT DF_ProductOnAdjust_GetScoreOld DEFAULT 0,
    SalePriceNew uniqueidentifier,
    UseScoreNew int NOT NULL CONSTRAINT DF_ProductOnAdjust_UseScoreNew DEFAULT 0,
    ScoreDeductNew uniqueidentifier,
    GetScoreNew int NOT NULL CONSTRAINT DF_ProductOnAdjust_GetScoreNew DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_ProductOnAdjust PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ProductOnAdjust_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid)
) ON [PRIMARY];

-- Purchase模块数据库建表脚本
CREATE TABLE PurchaseInformation (
    Gid uniqueidentifier CONSTRAINT DF_PurchaseInformation_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PurchaseInformation_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInformation_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInformation_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    WhID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Pstatus tinyint NOT NULL CONSTRAINT DF_PurchaseInformation_Pstatus DEFAULT 0,
    Locking tinyint NOT NULL CONSTRAINT DF_PurchaseInformation_Locking DEFAULT 0,
    Hanged tinyint NOT NULL CONSTRAINT DF_PurchaseInformation_Hanged DEFAULT 0,
    Supplier uniqueidentifier NOT NULL,
    Ptype uniqueidentifier,
    TrackLot int NOT NULL CONSTRAINT DF_PurchaseInformation_TrackLot DEFAULT 0,
    DocVersion int NOT NULL CONSTRAINT DF_PurchaseInformation_DocVersion DEFAULT 0,
    Discount decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInformation_Discount DEFAULT 1,
    Currency uniqueidentifier,
    CalcMode tinyint NOT NULL CONSTRAINT DF_PurchaseInformation_CalcMode DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInformation_Quantity DEFAULT 0,
    Amount money NOT NULL CONSTRAINT DF_PurchaseInformation_Amount DEFAULT 0,
    TaxFee money NOT NULL CONSTRAINT DF_PurchaseInformation_TaxFee DEFAULT 0,
    ShipFee money NOT NULL CONSTRAINT DF_PurchaseInformation_ShipFee DEFAULT 0,
    Paid money NOT NULL CONSTRAINT DF_PurchaseInformation_Paid DEFAULT 0,
    Differ money NOT NULL CONSTRAINT DF_PurchaseInformation_Differ DEFAULT 0,
    etd datetimeoffset (0),
    atd datetimeoffset (0),
    eta datetimeoffset (0),
    ata datetimeoffset (0),
    PortDate datetimeoffset (0),
    Brief nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_PurchaseInformation PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PurchaseInformation_OrgID_Code ON PurchaseInformation (OrgID, Code) ON [PRIMARY];

CREATE TABLE PurchaseItem (
    Gid uniqueidentifier CONSTRAINT DF_PurchaseItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PurchaseItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PurID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseItem_Quantity DEFAULT 0,
    InQty decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseItem_InQty DEFAULT 0,
    Price money NOT NULL CONSTRAINT DF_PurchaseItem_Price DEFAULT 0,
    TaxFee money NOT NULL CONSTRAINT DF_PurchaseItem_TaxFee DEFAULT 0,
    ShipFee money NOT NULL CONSTRAINT DF_PurchaseItem_ShipFee DEFAULT 0,
    AvgCost money NOT NULL CONSTRAINT DF_PurchaseItem_AvgCost DEFAULT 0,
    Amount money NOT NULL CONSTRAINT DF_PurchaseItem_Amount DEFAULT 0,
    Guarantee datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_PurchaseItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PurchaseItem_PurchaseInformation FOREIGN KEY (PurID) REFERENCES PurchaseInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PurchaseItem_PurID_SkuID ON PurchaseItem (PurID, SkuID) ON [PRIMARY];

CREATE TABLE PurchaseHistory (
    Gid uniqueidentifier CONSTRAINT DF_PurchaseHistory_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PurchaseHistory_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseHistory_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseHistory_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PurID uniqueidentifier NOT NULL,
    DocVersion int NOT NULL CONSTRAINT DF_PurchaseHistory_DocVersion DEFAULT 0,
    Htype tinyint NOT NULL CONSTRAINT DF_PurchaseHistory_Htype DEFAULT 0,
    Reason nvarchar (256),
    WhID uniqueidentifier NOT NULL,
    Pstatus tinyint NOT NULL CONSTRAINT DF_PurchaseHistory_Pstatus DEFAULT 0,
    Locking tinyint NOT NULL CONSTRAINT DF_PurchaseHistory_Locking DEFAULT 0,
    Hanged tinyint NOT NULL CONSTRAINT DF_PurchaseHistory_Hanged DEFAULT 0,
    Supplier uniqueidentifier,
    Ptype uniqueidentifier,
    TrackLot int NOT NULL CONSTRAINT DF_PurchaseHistory_TrackLot DEFAULT 0,
    Discount decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseHistory_Discount DEFAULT 1,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseHistory_Quantity DEFAULT 0,
    Amount money NOT NULL CONSTRAINT DF_PurchaseHistory_Amount DEFAULT 0,
    TaxFee money NOT NULL CONSTRAINT DF_PurchaseHistory_TaxFee DEFAULT 0,
    ShipFee money NOT NULL CONSTRAINT DF_PurchaseHistory_ShipFee DEFAULT 0,
    Paid money NOT NULL CONSTRAINT DF_PurchaseHistory_Paid DEFAULT 0,
    Differ money NOT NULL CONSTRAINT DF_PurchaseHistory_Differ DEFAULT 0,
    etd datetimeoffset (0),
    atd datetimeoffset (0),
    eta datetimeoffset (0),
    ata datetimeoffset (0),
    PortDate datetimeoffset (0),
    Brief nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_PurchaseHistory PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PurchaseHistory_PurchaseInformation FOREIGN KEY (PurID) REFERENCES PurchaseInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PurchaseHistory_PurID_DocVersion ON PurchaseHistory (PurID, DocVersion) ON [PRIMARY];

CREATE TABLE PurchaseHisItem (
    Gid uniqueidentifier CONSTRAINT DF_PurchaseHisItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PurchaseHisItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseHisItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseHisItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PurHisID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseHisItem_Quantity DEFAULT 0,
    InQty decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseHisItem_InQty DEFAULT 0,
    Price money NOT NULL CONSTRAINT DF_PurchaseHisItem_Price DEFAULT 0,
    TaxFee money NOT NULL CONSTRAINT DF_PurchaseHisItem_TaxFee DEFAULT 0,
    ShipFee money NOT NULL CONSTRAINT DF_PurchaseHisItem_ShipFee DEFAULT 0,
    AvgCost money NOT NULL CONSTRAINT DF_PurchaseHisItem_AvgCost DEFAULT 0,
    Amount money NOT NULL CONSTRAINT DF_PurchaseHisItem_Amount DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_PurchaseHisItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PurchaseHisItem_PurchaseHistory FOREIGN KEY (PurHisID) REFERENCES PurchaseHistory (Gid)
) ON [PRIMARY];

CREATE TABLE PurchaseInspection (
    Gid uniqueidentifier CONSTRAINT DF_PurchaseInspection_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PurchaseInspection_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInspection_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInspection_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    PurID uniqueidentifier,
    Code nvarchar (50),
    Inspector nvarchar (50),
    InspDate datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInspection_InspDate DEFAULT sysdatetimeoffset(),
    Total decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInspection_Total DEFAULT 0,
    Passed bit NOT NULL CONSTRAINT DF_PurchaseInspection_Passed DEFAULT 0,
    Brief nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_PurchaseInspection PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PurchaseInspection_OrgID_Code ON PurchaseInspection (OrgID, Code) ON [PRIMARY];

CREATE TABLE PurchaseInspItem (
    Gid uniqueidentifier CONSTRAINT DF_PurchaseInspItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PurchaseInspItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInspItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PurchaseInspItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    InspID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    TotalQty decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInspItem_TotalQty DEFAULT 0,
    InspQty decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInspItem_InspQty DEFAULT 0,
    PassQty decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInspItem_PassQty DEFAULT 0,
    PassRate decimal (18,4) NOT NULL CONSTRAINT DF_PurchaseInspItem_PassRate DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_PurchaseInspItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PurchaseInspItem_PurchaseInspection FOREIGN KEY (InspID) REFERENCES PurchaseInspection (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PurchaseInspItem_InspID_SkuID ON PurchaseInspItem (InspID, SkuID) ON [PRIMARY];

-- Warehouse模块数据库建表脚本
CREATE TABLE WarehouseShelf (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseShelf_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseShelf_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseShelf_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseShelf_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Barcode nvarchar (50) NOT NULL,
    Reserved bit NOT NULL CONSTRAINT DF_WarehouseShelf_Reserved DEFAULT 0,
    Name nvarchar (256),
    Brief nvarchar (512),
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseShelf PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseShelf_WhID_Code ON WarehouseShelf (WhID, Code) ON [PRIMARY];
CREATE UNIQUE INDEX IX_WarehouseShelf_WhID_Barcode ON WarehouseShelf (WhID, Barcode) ON [PRIMARY];

CREATE TABLE WarehouseChannel (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseChannel_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseChannel_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseChannel_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseChannel_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    Cstatus tinyint NOT NULL CONSTRAINT DF_WarehouseChannel_Cstatus DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseChannel PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseChannel_WhID_ChlID ON WarehouseChannel (WhID, ChlID) ON [PRIMARY];

CREATE TABLE WarehouseRegion (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseRegion_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseRegion_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseRegion_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseRegion_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    RegionID uniqueidentifier NOT NULL,
    Terminal bit NOT NULL CONSTRAINT DF_WarehouseRegion_Terminal DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseRegion PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseRegion_WhID_RegionID ON WarehouseRegion (WhID, RegionID) ON [PRIMARY];

CREATE TABLE WarehouseShipping (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseShipping_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseShipping_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseShipping_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseShipping_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    ShipID uniqueidentifier NOT NULL,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseShipping PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseShipping_WhID_ShipID ON WarehouseShipping (WhID, ShipID) ON [PRIMARY];

CREATE TABLE WarehouseLedger (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseLedger_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseLedger_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseLedger_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseLedger_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    InQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_InQty DEFAULT 0,
    OutQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_OutQty DEFAULT 0,
    RealQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_RealQty DEFAULT 0,
    LockQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_LockQty DEFAULT 0,
    CanSaleQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_CanSaleQty DEFAULT 0,
    CanDelivery decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_CanDelivery DEFAULT 0,
    TobeDelivery decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_TobeDelivery DEFAULT 0,
    Arranged decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_Arranged DEFAULT 0,
    Presale decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_Presale DEFAULT 0,
    Ontheway decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_Ontheway DEFAULT 0,
    SafeQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_SafeQty DEFAULT 0,
    MaxQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseLedger_MaxQty DEFAULT 0,
    AvgCost uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseLedger PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseLedger_WhID_SkuID ON WarehouseLedger (WhID, SkuID) ON [PRIMARY];

CREATE TABLE WarehouseSkuShelf (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseSkuShelf_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseSkuShelf_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseSkuShelf_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseSkuShelf_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    ShelfID uniqueidentifier NOT NULL,
    TrackLot int NOT NULL CONSTRAINT DF_WarehouseSkuShelf_TrackLot DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseSkuShelf_Quantity DEFAULT 0,
    LockQty decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseSkuShelf_LockQty DEFAULT 0,
    Guarantee datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseSkuShelf PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseSkuShelf_SkuID_ShelfID_TrackLot ON WarehouseSkuShelf (SkuID, ShelfID, TrackLot) ON [PRIMARY];

CREATE TABLE WarehouseStockIn (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseStockIn_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseStockIn_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseStockIn_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseStockIn_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Istatus tinyint NOT NULL CONSTRAINT DF_WarehouseStockIn_Istatus DEFAULT 0,
    InType uniqueidentifier,
    RefType tinyint NOT NULL CONSTRAINT DF_WarehouseStockIn_RefType DEFAULT 0,
    RefID uniqueidentifier,
    Total decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseStockIn_Total DEFAULT 0,
    PrintInSheet int NOT NULL CONSTRAINT DF_WarehouseStockIn_PrintInSheet DEFAULT 0,
    Prepared uniqueidentifier,
    Approved uniqueidentifier,
    ApproveTime	datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseStockIn PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseStockIn_WhID_Code ON WarehouseStockIn (WhID, Code) ON [PRIMARY];

CREATE TABLE WarehouseInItem (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseInItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseInItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseInItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseInItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    InID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    ShelfID uniqueidentifier NOT NULL,
    TrackLot int NOT NULL CONSTRAINT DF_WarehouseInItem_TrackLot DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseInItem_Quantity DEFAULT 0,
    Guarantee datetimeoffset (0),
    GenBarcode int NOT NULL CONSTRAINT DF_WarehouseInItem_GenBarcode DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseInItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_WarehouseInItem_WarehouseStockIn FOREIGN KEY (InID) REFERENCES WarehouseStockIn (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseInItem_InID_SkuID_ShelfID_TrackLot ON WarehouseInItem (InID, SkuID, ShelfID, TrackLot) ON [PRIMARY];

CREATE TABLE WarehouseStockOut (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseStockOut_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseStockOut_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseStockOut_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseStockOut_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    WhID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Ostatus	tinyint NOT NULL CONSTRAINT DF_WarehouseStockOut_Ostatus DEFAULT 0,
    OutType uniqueidentifier,
    RefType tinyint NOT NULL CONSTRAINT DF_WarehouseStockOut_RefType DEFAULT 0,
    RefID uniqueidentifier,
    ShipID uniqueidentifier,
    TakeMan uniqueidentifier,
    SortMan uniqueidentifier,
    SendMan uniqueidentifier,
    PrintOutSheet int NOT NULL CONSTRAINT DF_WarehouseStockOut_PrintOutSheet DEFAULT 0,
    PrintEnvelope int NOT NULL CONSTRAINT DF_WarehouseStockOut_PrintEnvelope DEFAULT 0,
    Total decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseStockOut_Total DEFAULT 0,
    Package int NOT NULL CONSTRAINT DF_WarehouseStockOut_Package DEFAULT 0,
    Prepared uniqueidentifier,
    Approved uniqueidentifier,
    ApproveTime datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseStockOut PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseStockOut_WhID_Code ON WarehouseStockOut (WhID, Code) ON [PRIMARY];

CREATE TABLE WarehouseOutItem (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseOutItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseOutItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OutID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    ShelfID uniqueidentifier NOT NULL,
    TrackLot int NOT NULL CONSTRAINT DF_WarehouseOutItem_TrackLot DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseOutItem_Quantity DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseOutItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_WarehouseOutItem_WarehouseStockOut FOREIGN KEY (OutID) REFERENCES WarehouseStockOut (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseOutItem_OutID_SkuID_ShelfID_TrackLot ON WarehouseOutItem (OutID, SkuID, ShelfID, TrackLot) ON [PRIMARY];

CREATE TABLE WarehouseOutScan (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseOutScan_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseOutScan_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutScan_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutScan_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OutID uniqueidentifier NOT NULL,
    TakeMan uniqueidentifier,
    Barcode nvarchar (50),
    SkuID uniqueidentifier NOT NULL,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseOutScan_Quantity DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseOutScan PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_WarehouseOutScan_WarehouseStockOut FOREIGN KEY (OutID) REFERENCES WarehouseStockOut (Gid)
) ON [PRIMARY];

CREATE TABLE WarehouseOutDelivery (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseOutDelivery_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseOutDelivery_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutDelivery_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutDelivery_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OutID uniqueidentifier NOT NULL,
    ShipID uniqueidentifier NOT NULL,
    Envelope nvarchar (50) NOT NULL,
    PackWeight decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseOutDelivery_PackWeight DEFAULT 0,
    SendTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseOutDelivery_SendTime DEFAULT sysdatetimeoffset(),
    TraceRoute nvarchar (MAX),
    ETA datetimeoffset (0),
    ShipPrice money NOT NULL CONSTRAINT DF_WarehouseOutDelivery_ShipPrice DEFAULT 0,
    ClosePrice money NOT NULL CONSTRAINT DF_WarehouseOutDelivery_ClosePrice DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseOutDelivery PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_WarehouseOutDelivery_WarehouseStockOut FOREIGN KEY (OutID) REFERENCES WarehouseStockOut (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseOutDelivery_OutID_ShipID_Envelope ON WarehouseOutDelivery (OutID, ShipID, Envelope) ON [PRIMARY];

CREATE TABLE WarehouseMoving (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseMoving_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseMoving_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseMoving_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseMoving_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Mstatus tinyint NOT NULL CONSTRAINT DF_WarehouseMoving_Mstatus DEFAULT 0,
    Mtype tinyint NOT NULL CONSTRAINT DF_WarehouseMoving_Mtype DEFAULT 0,
    OldWhID uniqueidentifier NOT NULL,
    NewWhID uniqueidentifier NOT NULL,
    Reason nvarchar (256),
    Total decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseMoving_Total DEFAULT 0,
    ShipID uniqueidentifier,
    Prepared uniqueidentifier,
    Approved uniqueidentifier,
    ApproveTime datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseMoving PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseMoving_OrgID_Code ON WarehouseMoving (OrgID, Code) ON [PRIMARY];

CREATE TABLE WarehouseMoveItem (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseMoveItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseMoveItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseMoveItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseMoveItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    MoveID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    OldShelf uniqueidentifier,
    NewShelf uniqueidentifier,
    TrackLot int NOT NULL CONSTRAINT DF_WarehouseMoveItem_TrackLot DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseMoveItem_Quantity DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseMoveItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_WarehouseMoveItem_WarehouseMoving FOREIGN KEY (MoveID) REFERENCES WarehouseMoving (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseMoveItem_MoveID_SkuID_Shelf_TrackLot ON WarehouseMoveItem (MoveID, SkuID, OldShelf, NewShelf, TrackLot) ON [PRIMARY];

CREATE TABLE WarehouseInventory (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseInventory_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseInventory_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseInventory_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseInventory_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    WhID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Istatus tinyint NOT NULL CONSTRAINT DF_WarehouseInventory_Istatus DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseInventory_Quantity DEFAULT 0,
    Prepared uniqueidentifier,
    Approved uniqueidentifier,
    ApproveTime	datetimeoffset (0),
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseInventory PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseInventory_OrgID_Code ON WarehouseInventory (OrgID, Code) ON [PRIMARY];

CREATE TABLE WarehouseInvItem (
    Gid uniqueidentifier CONSTRAINT DF_WarehouseInvItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_WarehouseInvItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseInvItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_WarehouseInvItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    InvID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    ShelfID uniqueidentifier NOT NULL,
    TrackLot int NOT NULL CONSTRAINT DF_WarehouseInvItem_TrackLot DEFAULT 0,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_WarehouseInvItem_Quantity DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_WarehouseInvItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_WarehouseInvItem_WarehouseInventory FOREIGN KEY (InvID) REFERENCES WarehouseInventory (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_WarehouseInvItem_InvID_SkuID_ShelfID_TrackLot ON WarehouseInvItem (InvID, SkuID, ShelfID, TrackLot) ON [PRIMARY];

-- Order模块数据库建表脚本
CREATE TABLE OrderInformation (
    Gid uniqueidentifier CONSTRAINT DF_OrderInformation_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderInformation_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderInformation_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderInformation_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    WhID uniqueidentifier,
    UserID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    LinkCode nvarchar (max),
    Ostatus tinyint NOT NULL CONSTRAINT DF_OrderInformation_Ostatus DEFAULT 0,
    Locking tinyint NOT NULL CONSTRAINT DF_OrderInformation_Locking DEFAULT 0,
    PayStatus tinyint NOT NULL CONSTRAINT DF_OrderInformation_PayStatus DEFAULT 0,
    Hanged tinyint NOT NULL CONSTRAINT DF_OrderInformation_Hanged DEFAULT 0,
    HangReason nvarchar (128),
    ReleaseTime datetimeoffset (0),
    TransType tinyint NOT NULL CONSTRAINT DF_OrderInformation_TransType DEFAULT 0,
    DocVersion int NOT NULL CONSTRAINT DF_OrderInformation_DocVersion DEFAULT 0,
    PayID uniqueidentifier,
    PayNote nvarchar (256),
    Pieces decimal (18,4) NOT NULL CONSTRAINT DF_OrderInformation_Pieces DEFAULT 0,
    Currency uniqueidentifier,
    SaleAmount money NOT NULL CONSTRAINT DF_OrderInformation_SaleAmount DEFAULT 0,
    ExecuteAmount money NOT NULL CONSTRAINT DF_OrderInformation_ExecuteAmount DEFAULT 0,
    ShippingFee money NOT NULL CONSTRAINT DF_OrderInformation_ShippingFee DEFAULT 0,
    TaxFee money NOT NULL CONSTRAINT DF_OrderInformation_TaxFee DEFAULT 0,
    Insurance money NOT NULL CONSTRAINT DF_OrderInformation_Insurance DEFAULT 0,
    PaymentFee money NOT NULL CONSTRAINT DF_OrderInformation_PaymentFee DEFAULT 0,
    PackingFee money NOT NULL CONSTRAINT DF_OrderInformation_PackingFee DEFAULT 0,
    ResidenceFee money NOT NULL CONSTRAINT DF_OrderInformation_ResidenceFee DEFAULT 0,
    LiftGateFee money NOT NULL CONSTRAINT DF_OrderInformation_LiftGateFee DEFAULT 0,
    InstallFee money NOT NULL CONSTRAINT DF_OrderInformation_InstallFee DEFAULT 0,
    OtherFee money NOT NULL CONSTRAINT DF_OrderInformation_OtherFee DEFAULT 0,
    TotalFee money NOT NULL CONSTRAINT DF_OrderInformation_TotalFee DEFAULT 0,
    UsePoint int NOT NULL CONSTRAINT DF_OrderInformation_UsePoint DEFAULT 0,
    PointPay money NOT NULL CONSTRAINT DF_OrderInformation_PointPay DEFAULT 0,
    CouponPay money NOT NULL CONSTRAINT DF_OrderInformation_CouponPay DEFAULT 0,
    BounsPay money NOT NULL CONSTRAINT DF_OrderInformation_BounsPay DEFAULT 0,
    Discount money NOT NULL CONSTRAINT DF_OrderInformation_Discount DEFAULT 0,
    MoneyPaid money NOT NULL CONSTRAINT DF_OrderInformation_MoneyPaid DEFAULT 0,
    TotalPaid money NOT NULL CONSTRAINT DF_OrderInformation_TotalPaid DEFAULT 0,
    OrderAmount money NOT NULL CONSTRAINT DF_OrderInformation_OrderAmount DEFAULT 0,
    Differ money NOT NULL CONSTRAINT DF_OrderInformation_Differ DEFAULT 0,
    MergeFrom nvarchar (256),
    SplitFrom uniqueidentifier,
    GetPoint int NOT NULL CONSTRAINT DF_OrderInformation_GetPoint DEFAULT 0,
    ConfirmTime datetimeoffset (0),
    PaidTime datetimeoffset (0),
    ArrangeTime datetimeoffset (0),
    NoticeTime datetimeoffset (0),
    ClosedTime datetimeoffset (0),
    Consignee nvarchar (256),
    Location uniqueidentifier,
    FullAddress nvarchar (256),
    PostCode nvarchar (50),
    Telephone nvarchar (50),
    Mobile nvarchar (50),
    Email nvarchar (128),
    ErrorAddress bit NOT NULL CONSTRAINT DF_OrderInformation_ErrorAddr DEFAULT 0,
    BestDelivery nvarchar (128),
    BuildingSign nvarchar (128),
    PostComment nvarchar (256),
    LeaveWord nvarchar (256),
    IpAddress nvarchar (20),
    AdvID uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_OrderInformation PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_OrderInformation_Code ON OrderInformation (Code) ON [PRIMARY];

CREATE TABLE OrderItem (
    Gid uniqueidentifier CONSTRAINT DF_OrderItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    OnSkuID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    Name nvarchar (512),
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_Quantity DEFAULT 0,
    TobeShip decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_TobeShip DEFAULT 0,
    Shipped decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_Shipped DEFAULT 0,
    BeReturn decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_BeReturn DEFAULT 0,
    Returned decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_Returned DEFAULT 0,
    MarketPrice money NOT NULL CONSTRAINT DF_OrderItem_MarketPrice DEFAULT 0,
    SalePrice money NOT NULL CONSTRAINT DF_OrderItem_SalePrice DEFAULT 0,
    ExecutePrice money NOT NULL CONSTRAINT DF_OrderItem_ExecutePrice DEFAULT 0,
    SkuPoint int NOT NULL CONSTRAINT DF_OrderItem_SkuPoint DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_OrderItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_OrderItem_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_OrderItem_OrderID_OnSkuID ON OrderItem (OrderID, OnSkuID) ON [PRIMARY];

CREATE TABLE OrderShipping (
    Gid uniqueidentifier CONSTRAINT DF_OrderShipping_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderShipping_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderShipping_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderShipping_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    ShipID uniqueidentifier NOT NULL,
    ShipWeight int NOT NULL CONSTRAINT DF_OrderShipping_ShipWeight DEFAULT 0,
    Ostatus tinyint NOT NULL CONSTRAINT DF_OrderShipping_Ostatus DEFAULT 0,
    Candidate bit NOT NULL CONSTRAINT DF_OrderShipping_Candidate DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_OrderShipping PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_OrderShipping_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_OrderShipping_OrderID_ShipID ON OrderShipping (OrderID, ShipID) ON [PRIMARY];

CREATE TABLE OrderAttribute (
    Gid uniqueidentifier CONSTRAINT DF_OrderAttribute_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderAttribute_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderAttribute_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderAttribute_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    OptID uniqueidentifier NOT NULL,
    OptResult uniqueidentifier,
    Matter nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_OrderAttribute PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_OrderAttribute_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_OrderAttribute_OrderID_OptID ON OrderAttribute (OrderID, OptID) ON [PRIMARY];

CREATE TABLE OrderProcess (
    Gid uniqueidentifier CONSTRAINT DF_OrderProcess_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderProcess_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderProcess_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderProcess_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Show bit NOT NULL CONSTRAINT DF_OrderProcess_Show DEFAULT 0,
    Matter nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_OrderProcess PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_OrderProcess_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid)
) ON [PRIMARY];

CREATE TABLE OrderSplitPolicy (
    Gid uniqueidentifier CONSTRAINT DF_OrderSplitPolicy_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderSplitPolicy_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderSplitPolicy_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderSplitPolicy_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    SetHanged bit NOT NULL CONSTRAINT DF_OrderSplitPolicy_SetHanged DEFAULT 1,
    ProdGroup bit NOT NULL CONSTRAINT DF_OrderSplitPolicy_ProdGroup DEFAULT 0,
    ProdShort bit NOT NULL CONSTRAINT DF_OrderSplitPolicy_ProdShort DEFAULT 0,
    Warehouse bit NOT NULL CONSTRAINT DF_OrderSplitPolicy_Warehouse DEFAULT 0,
    ShipLimit bit NOT NULL CONSTRAINT DF_OrderSplitPolicy_ShipLimit DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_OrderSplitPolicy PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_OrderSplitPolicy_OrgID_ChlID ON OrderSplitPolicy (OrgID, ChlID) ON [PRIMARY];

CREATE TABLE OrderHistory (
    Gid uniqueidentifier CONSTRAINT DF_OrderHistory_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderHistory_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderHistory_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderHistory_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    DocVersion int NOT NULL CONSTRAINT DF_OrderHistory_DocVersion DEFAULT 0,
    Htype tinyint NOT NULL CONSTRAINT DF_OrderHistory_Htype DEFAULT 0,
    Reason nvarchar (256),
    RefRefund uniqueidentifier,
    RefStockIn uniqueidentifier,
    WhID uniqueidentifier,
    LinkCode nvarchar (50),
    Ostatus tinyint NOT NULL CONSTRAINT DF_OrderHistory_Ostatus DEFAULT 0,
    Locking tinyint NOT NULL CONSTRAINT DF_OrderHistory_Locking DEFAULT 0,
    PayStatus tinyint NOT NULL CONSTRAINT DF_OrderHistory_PayStatus DEFAULT 0,
    Hanged tinyint NOT NULL CONSTRAINT DF_OrderHistory_Hanged DEFAULT 0,
    HangReason nvarchar (128),
    ReleaseTime datetimeoffset (0),
    TransType tinyint NOT NULL CONSTRAINT DF_OrderHistory_TransType DEFAULT 0,
    PayID uniqueidentifier,
    PayNote nvarchar (256),
    Pieces decimal (18,4) NOT NULL CONSTRAINT DF_OrderHistory_Pieces DEFAULT 0,
    Currency uniqueidentifier,
    SaleAmount money NOT NULL CONSTRAINT DF_OrderHistory_SaleAmount DEFAULT 0,
    ExecuteAmount money NOT NULL CONSTRAINT DF_OrderHistory_ExecuteAmount DEFAULT 0,
    ShippingFee money NOT NULL CONSTRAINT DF_OrderHistory_ShippingFee DEFAULT 0,
    TaxFee money NOT NULL CONSTRAINT DF_OrderHistory_TaxFee DEFAULT 0,
    Insurance money NOT NULL CONSTRAINT DF_OrderHistory_Insurance DEFAULT 0,
    PaymentFee money NOT NULL CONSTRAINT DF_OrderHistory_PaymentFee DEFAULT 0,
    PackingFee money NOT NULL CONSTRAINT DF_OrderHistory_PackingFee DEFAULT 0,
    ResidenceFee money NOT NULL CONSTRAINT DF_OrderHistory_ResidenceFee DEFAULT 0,
    LiftGateFee money NOT NULL CONSTRAINT DF_OrderHistory_LiftGateFee DEFAULT 0,
    InstallFee money NOT NULL CONSTRAINT DF_OrderHistory_InstallFee DEFAULT 0,
    OtherFee money NOT NULL CONSTRAINT DF_OrderHistory_OtherFee DEFAULT 0,
    TotalFee money NOT NULL CONSTRAINT DF_OrderHistory_TotalFee DEFAULT 0,
    UsePoint int NOT NULL CONSTRAINT DF_OrderHistory_UsePoint DEFAULT 0,
    PointPay money NOT NULL CONSTRAINT DF_OrderHistory_PointPay DEFAULT 0,
    CouponPay money NOT NULL CONSTRAINT DF_OrderHistory_CouponPay DEFAULT 0,
    BounsPay money NOT NULL CONSTRAINT DF_OrderHistory_BounsPay DEFAULT 0,
    MoneyPaid money NOT NULL CONSTRAINT DF_OrderHistory_MoneyPaid DEFAULT 0,
    Discount money NOT NULL CONSTRAINT DF_OrderHistory_Discount DEFAULT 0,
    TotalPaid money NOT NULL CONSTRAINT DF_OrderHistory_TotalPaid DEFAULT 0,
    OrderAmount money NOT NULL CONSTRAINT DF_OrderHistory_OrderAmount DEFAULT 0,
    Differ money NOT NULL CONSTRAINT DF_OrderHistory_Differ DEFAULT 0,
    MergeFrom nvarchar (256),
    SplitFrom uniqueidentifier,
    GetPoint int NOT NULL CONSTRAINT DF_OrderHistory_GetPoint DEFAULT 0,
    ConfirmTime datetimeoffset (0),
    PaidTime datetimeoffset (0),
    ArrangeTime datetimeoffset (0),
    NoticeTime datetimeoffset (0),
    ClosedTime datetimeoffset (0),
    Consignee nvarchar (256),
    Location uniqueidentifier,
    FullAddress nvarchar (256),
    PostCode nvarchar (50),
    Telephone nvarchar (50),
    Mobile nvarchar (50),
    Email nvarchar (128),
    ErrorAddress bit NOT NULL CONSTRAINT DF_OrderHistory_ErrorAddr DEFAULT 0,
    BestDelivery nvarchar (128),
    BuildingSign nvarchar (128),
    PostComment nvarchar (256),
    LeaveWord nvarchar (256),
    IpAddress nvarchar (20),
    AdvID uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_OrderHistory PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_OrderHistory_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid)
) ON [PRIMARY];

CREATE TABLE OrderHisItem (
    Gid uniqueidentifier CONSTRAINT DF_OrderHisItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_OrderHisItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderHisItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_OrderHisItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderHisID uniqueidentifier NOT NULL,
    OnSkuID uniqueidentifier NOT NULL,
    SkuID uniqueidentifier NOT NULL,
    Name nvarchar (512),
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_Quantity DEFAULT 0,
    TobeShip decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_TobeShip DEFAULT 0,
    Shipped decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_Shipped DEFAULT 0,
    BeReturn decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_BeReturn DEFAULT 0,
    Returned decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_Returned DEFAULT 0,
    MarketPrice money NOT NULL CONSTRAINT DF_OrderHisItem_MarketPrice DEFAULT 0,
    SalePrice money NOT NULL CONSTRAINT DF_OrderHisItem_SalePrice DEFAULT 0,
    ExecutePrice money NOT NULL CONSTRAINT DF_OrderHisItem_ExecutePrice DEFAULT 0,
    SkuPoint int NOT NULL CONSTRAINT DF_OrderHisItem_SkuPoint DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_OrderHisItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_OrderHisItem_OrderHistory FOREIGN KEY (OrderHisID) REFERENCES OrderHistory (Gid)
) ON [PRIMARY];

CREATE TABLE PromotionInformation (
    Gid uniqueidentifier CONSTRAINT DF_PromotionInformation_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PromotionInformation_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionInformation_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionInformation_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    Matter nvarchar (256),
    Pstatus tinyint NOT NULL CONSTRAINT DF_PromotionInformation_Pstatus DEFAULT 0,
    IssueType tinyint NOT NULL CONSTRAINT DF_PromotionInformation_IssueType DEFAULT 0,
    Sorting int NOT NULL CONSTRAINT DF_PromotionInformation_Sorting DEFAULT 0,
    Ptype tinyint NOT NULL CONSTRAINT DF_PromotionInformation_Ptype DEFAULT 0,
    ConditionA nvarchar (50),
    ConditionB nvarchar (50),
    ConditionC nvarchar (50),
    ConditionD nvarchar (50),
    IssueStart datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionInformation_IssueStart DEFAULT sysdatetimeoffset(),
    IssueEnd datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionInformation_IssueEnd DEFAULT sysdatetimeoffset(),
    StartTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionInformation_StartTime DEFAULT sysdatetimeoffset(),
    EndTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionInformation_EndTime DEFAULT sysdatetimeoffset(),
    EffectDays int NOT NULL CONSTRAINT DF_PromotionInformation_EffectDays DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_PromotionInformation PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PromotionInformation_OrgID_Code ON PromotionInformation (OrgID, Code) ON [PRIMARY];

CREATE TABLE PromotionMutex (
    Gid uniqueidentifier CONSTRAINT DF_PromotionMutex_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PromotionMutex_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionMutex_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionMutex_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PromID uniqueidentifier NOT NULL,
    MutexID uniqueidentifier NOT NULL,
    Relation tinyint NOT NULL CONSTRAINT DF_PromotionMutex_Relation DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_PromotionMutex PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PromotionMutex_PromotionInformation_PromID FOREIGN KEY (PromID) REFERENCES PromotionInformation (Gid),
    CONSTRAINT FK_PromotionMutex_PromotionInformation_MutexID FOREIGN KEY (MutexID) REFERENCES PromotionInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PromotionMutex_PromID_MutexID ON PromotionMutex (PromID, MutexID) ON [PRIMARY];

CREATE TABLE PromotionProduct (
    Gid uniqueidentifier CONSTRAINT DF_PromotionProduct_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PromotionProduct_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionProduct_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionProduct_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PromID uniqueidentifier NOT NULL,
    OnSkuID uniqueidentifier NOT NULL,
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_PromotionProduct_Quantity DEFAULT -1,
    Price uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_PromotionProduct PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PromotionProduct_PromotionInformation FOREIGN KEY (PromID) REFERENCES PromotionInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PromotionProduct_PromID_OnSkuID ON PromotionProduct (PromID, OnSkuID) ON [PRIMARY];

CREATE TABLE PromotionCoupon (
    Gid uniqueidentifier CONSTRAINT DF_PromotionCoupon_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_PromotionCoupon_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionCoupon_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionCoupon_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    PromID uniqueidentifier NOT NULL,
    Code nvarchar (64) NOT NULL,
    Passcode nvarchar (64) NOT NULL,
    Cstatus tinyint NOT NULL CONSTRAINT DF_PromotionCoupon_Cstatus DEFAULT 0,
    Currency uniqueidentifier,
    Amount money NOT NULL CONSTRAINT DF_PromotionCoupon_Amount DEFAULT 0,
    MinCharge money NOT NULL CONSTRAINT DF_PromotionCoupon_MinCharge DEFAULT 0,
    Cashier bit NOT NULL CONSTRAINT DF_PromotionCoupon_Cashier DEFAULT 0,
    OnceUse bit NOT NULL CONSTRAINT DF_PromotionCoupon_OnceUse DEFAULT 0,
    Together bit NOT NULL CONSTRAINT DF_PromotionCoupon_Together DEFAULT 0,
    StartTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionCoupon_StartTime DEFAULT sysdatetimeoffset(),
    EndTime datetimeoffset (0) NOT NULL CONSTRAINT DF_PromotionCoupon_EndTime DEFAULT sysdatetimeoffset(),
    Remark nvarchar (256),
    CONSTRAINT PK_PromotionCoupon PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_PromotionCoupon_PromotionInformation FOREIGN KEY (PromID) REFERENCES PromotionInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_PromotionCoupon_PromID_Code ON PromotionCoupon (PromID, Code) ON [PRIMARY];

-- Shipping模块数据库建表脚本
CREATE TABLE ShippingArea (
    Gid uniqueidentifier CONSTRAINT DF_ShippingArea_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ShippingArea_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ShippingArea_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ShippingArea_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ShipID uniqueidentifier NOT NULL,
    RegionID uniqueidentifier NOT NULL,
    Terminal bit NOT NULL CONSTRAINT DF_ShippingArea_Terminal DEFAULT 0,
    SupportCod bit NOT NULL CONSTRAINT DF_ShippingArea_SupportCod DEFAULT 0,
    Residential uniqueidentifier,
    LiftGate uniqueidentifier,
    Installation uniqueidentifier,
    PriceWeight uniqueidentifier,
    PriceVolume uniqueidentifier,
    PricePiece uniqueidentifier,
    PriceHigh uniqueidentifier,
    PriceLow uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_ShippingArea PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ShippingArea_ShipID_RegionID ON ShippingArea (ShipID, RegionID) ON [PRIMARY];

CREATE TABLE ShippingEnvelope (
    Gid uniqueidentifier CONSTRAINT DF_ShippingEnvelope_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ShippingEnvelope_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ShippingEnvelope_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_ShippingEnvelope_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ShipID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Culture int NOT NULL CONSTRAINT DF_ShippingEnvelope_Culture DEFAULT 2052,
    Estatus tinyint NOT NULL CONSTRAINT DF_ShippingEnvelope_Cstatus DEFAULT 0,
    Matter nvarchar (512),
    Template uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_ShippingEnvelope PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ShippingEnvelope_ShipID_Code ON ShippingEnvelope (ShipID, Code) ON [PRIMARY];

-- Mall模块数据库建表脚本
CREATE TABLE MallArtPosition (
    Gid uniqueidentifier CONSTRAINT DF_MallArtPosition_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallArtPosition_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArtPosition_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArtPosition_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    Matter nvarchar (256),
    Show bit NOT NULL CONSTRAINT DF_MallArtPosition_Show DEFAULT 0,
    Width int NOT NULL CONSTRAINT DF_MallArtPosition_Width DEFAULT 0,
    Height int NOT NULL CONSTRAINT DF_MallArtPosition_Height DEFAULT 0,
    Template uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_MallArtPosition PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallArtPosition_OrgID_Code ON MallArtPosition (OrgID, Code) ON [PRIMARY];

CREATE TABLE MallArticle (
    Gid uniqueidentifier CONSTRAINT DF_MallArticle_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallArticle_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArticle_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArticle_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Astatus tinyint NOT NULL CONSTRAINT DF_MallArticle_Astatus DEFAULT 0,
    Atype uniqueidentifier,
    Acategory uniqueidentifier,
    Parent uniqueidentifier,
    UserID uniqueidentifier,
    UserName nvarchar (256),
    ProdID uniqueidentifier,
    Title uniqueidentifier,
    Matter uniqueidentifier,
    Copyright nvarchar (256),
    Keywords nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_MallArticle PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MallArticle_MallArticle FOREIGN KEY (Parent) REFERENCES MallArticle (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallArticle_OrgID_Code ON MallArticle (OrgID, Code) ON [PRIMARY];

CREATE TABLE MallArtPublish (
    Gid uniqueidentifier CONSTRAINT DF_MallArtPublish_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallArtPublish_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArtPublish_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArtPublish_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    ArtID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    PosID uniqueidentifier NOT NULL,
    Sorting int NOT NULL CONSTRAINT DF_MallArtPublish_Sorting DEFAULT 0,
    Show bit NOT NULL CONSTRAINT DF_MallArtPublish_Show DEFAULT 0,
    TotalRank int NOT NULL CONSTRAINT DF_MallArtPublish_TotalRank DEFAULT 0,
    MatterRank int NOT NULL CONSTRAINT DF_MallArtPublish_MatterRank DEFAULT 0,
    LayoutRank int NOT NULL CONSTRAINT DF_MallArtPublish_LayoutRank DEFAULT 0,
    ComfortRank int NOT NULL CONSTRAINT DF_MallArtPublish_ComfortRank DEFAULT 0,
    StartTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArtPublish_StartTime DEFAULT sysdatetimeoffset(),
    EndTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallArtPublish_EditTime DEFAULT sysdatetimeoffset(),
    Remark nvarchar (256),
    CONSTRAINT PK_MallArtPublish PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_MallArtPublish_MallArticle FOREIGN KEY (ArtID) REFERENCES MallArticle (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallArtPublish_ArtID_ChlID_PosID ON MallArtPublish (ArtID, ChlID, PosID) ON [PRIMARY];

CREATE TABLE MallHotWord (
    Gid uniqueidentifier CONSTRAINT DF_MallHotWord_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallHotWord_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallHotWord_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallHotWord_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    Keyword nvarchar (128) NOT NULL,
    Approx nvarchar (128),
    Show bit NOT NULL CONSTRAINT DF_MallHotWord_Show DEFAULT 0,
    KeyWeight int NOT NULL CONSTRAINT DF_MallHotWord_KeyWeight DEFAULT 0,
    SearchCount int NOT NULL CONSTRAINT DF_MallHotWord_SearchCount DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MallHotWord PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallHotWord_ChlID_Keyword ON MallHotWord (ChlID, Keyword) ON [PRIMARY];

CREATE TABLE MallHotItem (
    Gid uniqueidentifier CONSTRAINT DF_MallHotItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallHotItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallHotItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallHotItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    Keyword nvarchar (128) NOT NULL,
    Source nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_MallHotItem PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE MallFavorite (
    Gid uniqueidentifier CONSTRAINT DF_MallFavorite_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallFavorite_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallFavorite_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallFavorite_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    UserID uniqueidentifier NOT NULL,
    OnSaleID uniqueidentifier NOT NULL,
    Remark nvarchar (256),
    CONSTRAINT PK_MallFavorite PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallFavorite_ChlID_UserID_OnSaleID ON MallFavorite (ChlID, UserID, OnSaleID) ON [PRIMARY];

CREATE TABLE MallFocusProduct (
    Gid uniqueidentifier CONSTRAINT DF_MallFocusProduct_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallFocusProduct_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallFocusProduct_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallFocusProduct_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    UserID uniqueidentifier NOT NULL,
    OnSaleID uniqueidentifier NOT NULL,
    Ftype tinyint NOT NULL CONSTRAINT DF_MallFocusProduct_Ftype DEFAULT 0,
    Currency uniqueidentifier,
    Price money NOT NULL CONSTRAINT DF_MallFocusProduct_Price DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MallFocusProduct PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallFocusProduct_ChlID_UserID_OnSaleID ON MallFocusProduct (ChlID, UserID, OnSaleID) ON [PRIMARY];

CREATE TABLE MallFriendLink (
    Gid uniqueidentifier CONSTRAINT DF_MallFriendLink_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallFriendLink_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallFriendLink_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallFriendLink_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    LinkURL nvarchar (256),
    LinkLogo nvarchar (256),
    Show bit NOT NULL CONSTRAINT DF_MallFriendLink_Show DEFAULT 0,
    Sorting int NOT NULL CONSTRAINT DF_MallFriendLink_Sorting DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MallFriendLink PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallFriendLink_ChlID_Code ON MallFriendLink (ChlID, Code) ON [PRIMARY];

CREATE TABLE MallSensitiveWord (
    Gid uniqueidentifier CONSTRAINT DF_MallSensitiveWord_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallSensitiveWord_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallSensitiveWord_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallSensitiveWord_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    Keyword nvarchar (128) NOT NULL,
    Wstatus tinyint NOT NULL CONSTRAINT DF_MallSensitiveWord_Wstatus DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MallSensitiveWord PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE MallDisabledIp (
    Gid uniqueidentifier CONSTRAINT DF_MallDisabledIp_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallDisabledIp_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallDisabledIp_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallDisabledIp_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    IpAddress nvarchar (50) NOT NULL,
    Istatus tinyint NOT NULL CONSTRAINT DF_MallDisabledIp_Istatus DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MallDisabledIp PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallDisabledIp_ChlID_IpAddress ON MallDisabledIp (ChlID, IpAddress) ON [PRIMARY];

CREATE TABLE MallVisitClick (
    Gid uniqueidentifier CONSTRAINT DF_MallVisitClick_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallVisitClick_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallVisitClick_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallVisitClick_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier NOT NULL,
    UserID uniqueidentifier,
    OnSaleID uniqueidentifier,
    AdvID uniqueidentifier,
    ClickUrl nvarchar (256),
    PreUrl nvarchar (256),
    SessionID nvarchar (50) NOT NULL,
    IpAddress nvarchar (20),
    Culture int NOT NULL CONSTRAINT DF_MallVisitClick_Culture DEFAULT 2052,
    Currency uniqueidentifier,
    Browser nvarchar (30),
    WebSystem nvarchar (30),
    Response nvarchar (20),
    Remark nvarchar (256),
    CONSTRAINT PK_MallVisitClick PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE INDEX IX_MallVisitClick_SessionID ON MallVisitClick (SessionID) ON [PRIMARY];

CREATE TABLE MallCart (
    Gid uniqueidentifier CONSTRAINT DF_MallCart_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_MallCart_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallCart_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_MallCart_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    UserID uniqueidentifier NOT NULL,
    OnSaleID uniqueidentifier NOT NULL,
    OnSkuID uniqueidentifier NOT NULL,
    Ctype uniqueidentifier,
    WebSession nvarchar (50),
    Quantity decimal (18,4) NOT NULL CONSTRAINT DF_MallCart_Quantity DEFAULT 0,
    SetQty decimal (18,4) NOT NULL CONSTRAINT DF_MallCart_SetQty DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_MallCart PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_MallCart_OrgID_ChlID_UserID_OnSkuID ON MallCart (OrgID, ChlID, UserID, OnSkuID) ON [PRIMARY];

-- Union模块数据库建表脚本
CREATE TABLE UnionLevelPercent (
    Gid uniqueidentifier CONSTRAINT DF_UnionLevelPercent_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_UnionLevelPercent_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_UnionLevelPercent_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_UnionLevelPercent_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    RoleID uniqueidentifier NOT NULL,
    BackLevel int NOT NULL CONSTRAINT DF_UnionLevelPercent_BackLevel DEFAULT 0,
    Ustatus tinyint NOT NULL CONSTRAINT DF_UnionLevelPercent_Ustatus DEFAULT 0,
    Percentage decimal (18,4) NOT NULL CONSTRAINT DF_UnionLevelPercent_Percentage DEFAULT 0,
    PercentTop decimal (18,4) NOT NULL CONSTRAINT DF_UnionLevelPercent_PercentTop DEFAULT 0,
    Cashier bit NOT NULL CONSTRAINT DF_UnionLevelPercent_Cashier DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_UnionLevelPercent PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_UnionLevelPercent_OrgID_RoleID_BackLevel ON UnionLevelPercent (OrgID, RoleID, BackLevel) ON [PRIMARY];

CREATE TABLE UnionFixedPercent (
    Gid uniqueidentifier CONSTRAINT DF_UnionFixedPercent_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_UnionFixedPercent_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_UnionFixedPercent_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_UnionFixedPercent_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    SaleType tinyint NOT NULL CONSTRAINT DF_UnionFixedPercent_SaleType DEFAULT 0,
    Ftype tinyint NOT NULL CONSTRAINT DF_UnionFixedPercent_Ftype DEFAULT 0,
    Fstatus tinyint NOT NULL CONSTRAINT DF_UnionFixedPercent_Fstatus DEFAULT 0,
    Percentage decimal (18,4) NOT NULL CONSTRAINT DF_UnionFixedPercent_Percentage DEFAULT 0,
    Cashier bit NOT NULL CONSTRAINT DF_UnionFixedPercent_Cashier DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_UnionFixedPercent PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_UnionFixedPercent_ChlID_SaleType_Ftype ON UnionFixedPercent (ChlID, SaleType, Ftype) ON [PRIMARY];

CREATE TABLE UnionAdvertising (
    Gid uniqueidentifier CONSTRAINT DF_UnionAdvertising_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_UnionAdvertising_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_UnionAdvertising_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_UnionAdvertising_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    Fstatus tinyint NOT NULL CONSTRAINT DF_UnionAdvertising_Fstatus DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_UnionAdvertising PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

-- Knowledge模块数据库建表脚本

-- Finance模块数据库建表脚本
CREATE TABLE FinancePayType (
    Gid uniqueidentifier CONSTRAINT DF_FinancePayType_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_FinancePayType_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_FinancePayType_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_FinancePayType_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Name uniqueidentifier,
    Matter nvarchar (512),
    Pstatus tinyint NOT NULL CONSTRAINT DF_FinancePayType_Pstatus DEFAULT 0,
    Sorting int NOT NULL CONSTRAINT DF_FinancePayType_Sorting DEFAULT 0,
    IsCod bit NOT NULL CONSTRAINT DF_FinancePayType_IsCod DEFAULT 0,
    IsOnline bit NOT NULL CONSTRAINT DF_FinancePayType_IsOnline DEFAULT 0,
    IsSecured bit NOT NULL CONSTRAINT DF_FinancePayType_IsSecured DEFAULT 0,
    Fee money NOT NULL CONSTRAINT DF_FinancePayType_Fee DEFAULT 0,
    Config nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_FinancePayType PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_FinancePayType_OrgID_Code ON FinancePayType (OrgID, Code) ON [PRIMARY];

CREATE TABLE FinanceInvoice (
    Gid uniqueidentifier CONSTRAINT DF_FinanceInvoice_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_FinanceInvoice_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_FinanceInvoice_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_FinanceInvoice_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    Code nvarchar (50) NOT NULL,
    Title nvarchar (256),
    Matter nvarchar (512),
    Istatus tinyint NOT NULL CONSTRAINT DF_FinanceInvoice_Istatus DEFAULT 0,
    Amount money NOT NULL CONSTRAINT DF_FinanceInvoice_Amount DEFAULT 0,
    SendNote nvarchar (256),
    Remark nvarchar (256),
    CONSTRAINT PK_FinanceInvoice PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_FinanceInvoice_OrderID_Code ON FinanceInvoice (OrderID, Code) ON [PRIMARY];

CREATE TABLE FinancePayment (
    Gid uniqueidentifier CONSTRAINT DF_FinancePayment_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_FinancePayment_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_FinancePayment_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_FinancePayment_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    Code nvarchar (50),
    Ptype uniqueidentifier,
    PayTo tinyint NOT NULL CONSTRAINT DF_FinancePayment_PayTo DEFAULT 0,
    Pstatus tinyint NOT NULL CONSTRAINT DF_FinancePayment_Pstatus DEFAULT 0,
    RefType tinyint NOT NULL CONSTRAINT DF_FinancePayment_RefType DEFAULT 0,
    RefID uniqueidentifier,
    Reason nvarchar (256),
    PayDate datetimeoffset (0) NOT NULL CONSTRAINT DF_FinancePayment_PayDate DEFAULT sysdatetimeoffset(),
    Currency uniqueidentifier,
    Amount money NOT NULL CONSTRAINT DF_FinancePayment_Amount DEFAULT 0,
    Remark nvarchar (256),
    CONSTRAINT PK_FinancePayment PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_FinancePayment_OrgID_Code ON FinancePayment (OrgID, Code) ON [PRIMARY];

-- Performance模块数据库建表脚本

-- Synchro模块数据库建表脚本
CREATE TABLE SynchroTable (
    Gid uniqueidentifier CONSTRAINT DF_SynchroTable_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_SynchroTable_Deleted DEFAULT 0,
    CreateTime datetimeoffset (0) NOT NULL CONSTRAINT DF_SynchroTable_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset (0) NOT NULL CONSTRAINT DF_SynchroTable_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    TableName nvarchar (128) NOT NULL,
    Direction tinyint NOT NULL CONSTRAINT DF_SynchroTable_Direction DEFAULT 0,
    Condition nvarchar (256),
    SyncLastRow binary (8),
    Remark nvarchar (256),
    CONSTRAINT PK_SynchroTable PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_SynchroTable_TableName_Direction ON SynchroTable (TableName, Direction) ON [PRIMARY];


-- Exchange模块数据库建表脚本
CREATE TABLE ExTaobaoOrder (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoOrder_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoOrder_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoOrder_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoOrder_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier NOT NULL,
    ChlID uniqueidentifier NOT NULL,
    OrderID uniqueidentifier,
    Transfered bit,
    Changed bit,
    end_time datetime2 (0),
    buyer_message nvarchar (max),
    shipping_type nvarchar (max),
    buyer_cod_fee nvarchar (max),
    seller_cod_fee nvarchar (max),
    express_agency_fee nvarchar (max),
    alipay_warn_msg nvarchar (max),
    [status] nvarchar (max),
    buyer_memo nvarchar (max),
    seller_memo nvarchar (max),
    modified datetime2 (0),
    buyer_flag bigint,
    seller_flag bigint,
    trade_from nvarchar (max),
    seller_nick nvarchar (max),
    buyer_nick nvarchar (max),
    title nvarchar (max),
    [type] nvarchar (max),
    created datetime2 (0),
    iid nvarchar (max),
    price nvarchar (max),
    pic_path nvarchar (max),
    num bigint,
    tid bigint,
    alipay_no nvarchar (max),
    payment nvarchar (max),
    discount_fee nvarchar (max),
    adjust_fee nvarchar (max),
    snapshot_url nvarchar (max),
    [snapshot] nvarchar (max),
    seller_rate bit,
    buyer_rate bit,
    trade_memo nvarchar (max),
    pay_time datetime2 (0),
    buyer_obtain_point_fee bigint,
    point_fee bigint,
    real_point_fee bigint,
    total_fee nvarchar (max),
    post_fee nvarchar (max),
    buyer_alipay_no nvarchar (max),
    receiver_name nvarchar (max),
    receiver_state nvarchar (max),
    receiver_city nvarchar (max),
    receiver_district nvarchar (max),
    receiver_address nvarchar (max),
    receiver_zip nvarchar (max),
    receiver_mobile nvarchar (max),
    receiver_phone nvarchar (max),
    consign_time datetime2 (0),
    buyer_email nvarchar (max),
    commission_fee nvarchar (max),
    seller_alipay_no nvarchar (max),
    seller_mobile nvarchar (max),
    seller_phone nvarchar (max),
    seller_name nvarchar (max),
    seller_email nvarchar (max),
    available_confirm_fee nvarchar (max),
    has_post_fee bit,
    received_payment nvarchar (max),
    cod_fee nvarchar (max),
    cod_status nvarchar (max),
    timeout_action_time datetime2 (0),
    is_3D bit,
    num_iid bigint,
    promotion nvarchar (max),
    invoice_name nvarchar (max),
    alipay_url nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoOrder PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoOrder_tid ON ExTaobaoOrder (tid) ON [PRIMARY];

CREATE TABLE ExTaobaoOrdItem (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoOrdItem_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoOrdItem_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoOrdItem_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoOrdItem_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    TopOrderID uniqueidentifier,
    total_fee nvarchar (max),
    discount_fee nvarchar (max),
    adjust_fee nvarchar (max),
    payment nvarchar (max),
    modified datetime2 (0),
    item_meal_id bigint,
    [status] nvarchar (max),
    refund_id bigint,
    iid nvarchar (max),
    sku_id nvarchar (max),
    sku_properties_name nvarchar (max),
    item_meal_name nvarchar (max),
    num bigint,
    title nvarchar (max),
    price nvarchar (max),
    pic_path nvarchar (max),
    seller_nick nvarchar (max),
    buyer_nick nvarchar (max),
    refund_status nvarchar (max),
    oid bigint,
    outer_iid nvarchar (max),
    outer_sku_id nvarchar (max),
    snapshot_url nvarchar (max),
    [snapshot] nvarchar (max),
    timeout_action_time datetime2 (0),
    buyer_rate bit,
    seller_rate bit,
    seller_type nvarchar (max),
    num_iid bigint,
    cid bigint,
    is_oversold bit,
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoOrdItem PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoOrdItem_ExTaobaoOrder FOREIGN KEY (TopOrderID) REFERENCES ExTaobaoOrder (Gid)
) ON [PRIMARY];

CREATE TABLE ExTaobaoPromotion (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoPromotion_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoPromotion_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoPromotion_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoPromotion_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    TopOrderID uniqueidentifier,
    id bigint,
    promotion_name nvarchar (max),
    discount_fee nvarchar (max),
    gift_item_name nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoPromotion PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoPromotion_ExTaobaoOrder FOREIGN KEY (TopOrderID) REFERENCES ExTaobaoOrder (Gid)
) ON [PRIMARY];

CREATE TABLE ExTaobaoTradeRate (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoTradeRate_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoTradeRate_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoTradeRate_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoTradeRate_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    TopOrderID uniqueidentifier,
    valid_score bit,
    tid bigint,
    oid bigint,
    [role] nvarchar (max),
    nick nvarchar (max),
    result nvarchar (max),
    created datetime2 (0),
    rated_nick nvarchar (max),
    item_title nvarchar (max),
    item_price nvarchar (max),
    content nvarchar (max),
    reply nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoTradeRate PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoTradeRate_ExTaobaoOrder FOREIGN KEY (TopOrderID) REFERENCES ExTaobaoOrder (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoTradeRate_tid ON ExTaobaoTradeRate (tid) ON [PRIMARY];

CREATE TABLE ExTaobaoRefund (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoRefund_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoRefund_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoRefund_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoRefund_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    TopOrderID uniqueidentifier,
    shipping_type nvarchar (max),
    refund_id bigint,
    tid bigint,
    oid bigint,
    alipay_no nvarchar (max),
    total_fee nvarchar (max),
    buyer_nick nvarchar (max),
    seller_nick nvarchar (max),
    created datetime2 (0),
    modified datetime2 (0),
    order_status nvarchar (max),
    [status] nvarchar (max),
    good_status nvarchar (max),
    has_good_return bit,
    refund_fee nvarchar (max),
    payment nvarchar (max),
    reason nvarchar (max),
    [desc] nvarchar (max),
    iid nvarchar (max),
    title nvarchar (max),
    price nvarchar (max),
    num bigint,
    good_return_time datetime2 (0),
    company_name nvarchar (max),
    [sid] nvarchar (max),
    [address] nvarchar (max),
    num_iid bigint,
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoRefund PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoRefund_ExTaobaoOrder FOREIGN KEY (TopOrderID) REFERENCES ExTaobaoOrder (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoRefund_tid ON ExTaobaoRefund (tid) ON [PRIMARY];

CREATE TABLE ExTaobaoRefundRemind (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoRefundRemind_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoRefundRemind_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoRefundRemind_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoRefundRemind_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    RefundID uniqueidentifier,
    remind_type bigint,
    exist_timeout bit,
    [timeout] datetime2 (0),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoRefundRemind PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoRefundRemind_ExTaobaoRefund FOREIGN KEY (RefundID) REFERENCES ExTaobaoRefund (Gid)
) ON [PRIMARY];

CREATE TABLE ExTaobaoUser (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoUser_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoUser_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoUser_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoUser_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    [user_id] bigint,
    [uid] nvarchar (max),
    nick nvarchar (max),
    sex nvarchar (max),
    buyer_credit uniqueidentifier,
    seller_credit uniqueidentifier,
    created datetime2 (0),
    last_visit datetime2 (0),
    birthday datetime2 (0),
    [type] nvarchar (max),
    has_more_pic bit,
    item_img_num bigint,
    item_img_size bigint,
    prop_img_num bigint,
    prop_img_size bigint,
    auto_repost nvarchar (max),
    promoted_type nvarchar (max),
    [status] nvarchar (max),
    alipay_bind nvarchar (max),
    consumer_protection bit,
    alipay_account nvarchar (max),
    alipay_no nvarchar (max),
    avatar nvarchar (max),
    liangpin bit,
    sign_food_seller_promise bit,
    has_shop bit,
    is_lightning_consignment bit,
    vip_info nvarchar (max),
    email nvarchar (max),
    magazine_subscribe bit,
    vertical_market nvarchar (max),
    online_gaming bit,
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoUser PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoUser_user_id ON ExTaobaoUser (user_id) ON [PRIMARY];

CREATE TABLE ExTaobaoUserCredit (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoUserCredit_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoUserCredit_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoUserCredit_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoUserCredit_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    [level] bigint,
    score bigint,
    total_num bigint,
    good_num bigint,
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoUserCredit PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE TABLE ExTaobaoLocation (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoLocation_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoLocation_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoLocation_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoLocation_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    TopUser uniqueidentifier NOT NULL,
    zip nvarchar (max),
    [address] nvarchar (max),
    city nvarchar (max),
    [state] nvarchar (max),
    country nvarchar (max),
    district nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoLocation PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoLocation_ExTaobaoUser FOREIGN KEY (TopUser) REFERENCES ExTaobaoUser (Gid)
) ON [PRIMARY];

CREATE TABLE ExTaobaoUserOrgan (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoUserOrgan_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoUserOrgan_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoUserOrgan_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoUserOrgan_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrgID uniqueidentifier,
    ChlID uniqueidentifier,
    TopUser uniqueidentifier,
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoUserOrgan PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoUserOrgan_OrgID_ChlID_TopUser ON ExTaobaoUserOrgan (OrgID, ChlID, TopUser) ON [PRIMARY];

CREATE TABLE ExTaobaoArea (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoArea_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoArea_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoArea_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoArea_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    RegionID uniqueidentifier,
    id bigint,
    [type] bigint,
    name nvarchar (max),
    parent_id bigint,
    zip nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoArea PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoArea_id ON ExTaobaoArea (id) ON [PRIMARY];

CREATE TABLE ExTaobaoLogisticsCompany (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoLogisticsCompany_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoLogisticsCompany_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoLogisticsCompany_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoLogisticsCompany_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    id bigint,
    code nvarchar (max),
    name nvarchar (max),
    reg_mail_no nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoLogisticsCompany PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoLogisticsCompany_id ON ExTaobaoLogisticsCompany (id) ON [PRIMARY];

CREATE TABLE ExTaobaoDeliveryPending (
    Gid uniqueidentifier CONSTRAINT DF_ExTaobaoDeliveryPending_Gid DEFAULT newsequentialid() ROWGUIDCOL,
    Deleted bit NOT NULL CONSTRAINT DF_ExTaobaoDeliveryPending_Deleted DEFAULT 0,
    CreateTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoDeliveryPending_CreateTime DEFAULT sysdatetimeoffset(),
    LastModifiedBy uniqueidentifier,
    LastModifyTime datetimeoffset NOT NULL CONSTRAINT DF_ExTaobaoDeliveryPending_LastModifyTime DEFAULT sysdatetimeoffset(),
    RowStamp rowversion,
    OrderID uniqueidentifier NOT NULL,
    Dstatus tinyint NOT NULL CONSTRAINT DF_ExTaobaoDeliveryPending_Dstatus DEFAULT 0,
    ShipID uniqueidentifier,
    tid nvarchar (max),
    logistics nvarchar (max),
    out_sid nvarchar (max),
    err_msg nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoDeliveryPending PRIMARY KEY CLUSTERED (Gid),
    CONSTRAINT FK_ExTaobaoDeliveryPending_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoDeliveryPending_OrderID ON ExTaobaoDeliveryPending (OrderID) ON [PRIMARY];

GO

-- General模块外键
ALTER TABLE GeneralOptional ADD CONSTRAINT FK_GeneralOptional_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE GeneralOptional ADD CONSTRAINT FK_GeneralOptional_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE GeneralOptItem ADD CONSTRAINT FK_GeneralOptItem_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE GeneralStandardCategory ADD CONSTRAINT FK_GeneralStandardCategory_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE GeneralPrivateCategory ADD CONSTRAINT FK_GeneralPrivateCategory_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);
ALTER TABLE GeneralPrivateCategory ADD CONSTRAINT FK_GeneralPrivateCategory_GeneralMeasureUnit FOREIGN KEY (StdUnit) REFERENCES GeneralMeasureUnit (Gid);

ALTER TABLE GeneralMeasureUnit ADD CONSTRAINT FK_GeneralMeasureUnit_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Piece FOREIGN KEY (Piece) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Weight FOREIGN KEY (Weight) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Volume FOREIGN KEY (Volume) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Fluid FOREIGN KEY (Fluid) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Area FOREIGN KEY (Area) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Linear FOREIGN KEY (Linear) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE GeneralCultureUnit ADD CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Currency FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

ALTER TABLE GeneralMessageTemplate ADD CONSTRAINT FK_GeneralMessageTemplate_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE GeneralMessageTemplate ADD CONSTRAINT FK_GeneralMessageTemplate_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);
ALTER TABLE GeneralMessageTemplate ADD CONSTRAINT FK_GeneralMessageTemplate_GeneralLargeObject FOREIGN KEY (Matter) REFERENCES GeneralLargeObject (Gid);

ALTER TABLE GeneralMessagePending ADD CONSTRAINT FK_GeneralMessagePending_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);

ALTER TABLE GeneralProgram ADD CONSTRAINT FK_GeneralProgram_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE GeneralProgNode ADD CONSTRAINT FK_GeneralProgNode_GeneralResource_Name FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);
ALTER TABLE GeneralProgNode ADD CONSTRAINT FK_GeneralProgNode_GeneralResource_Optional FOREIGN KEY (Optional) REFERENCES GeneralResource (Gid);

-- Member模块外键
ALTER TABLE MemberOrganization ADD CONSTRAINT FK_MemberOrganization_GeneralStandardCategory FOREIGN KEY (ExType) REFERENCES GeneralStandardCategory (Gid);
ALTER TABLE MemberOrganization ADD CONSTRAINT FK_MemberOrganization_GeneralResource_FullName FOREIGN KEY (FullName) REFERENCES GeneralResource (Gid);
ALTER TABLE MemberOrganization ADD CONSTRAINT FK_MemberOrganization_GeneralResource_ShortName FOREIGN KEY (ShortName) REFERENCES GeneralResource (Gid);
ALTER TABLE MemberOrganization ADD CONSTRAINT FK_MemberOrganization_GeneralRegion FOREIGN KEY (Location) REFERENCES GeneralRegion (Gid);
ALTER TABLE MemberOrganization ADD CONSTRAINT FK_MemberOrganization_GeneralLargeObject FOREIGN KEY (Introduction) REFERENCES GeneralLargeObject (Gid);

ALTER TABLE MemberOrgCulture ADD CONSTRAINT FK_MemberOrgCulture_GeneralCultureUnit FOREIGN KEY (Culture) REFERENCES GeneralCultureUnit (Gid);
ALTER TABLE MemberOrgCulture ADD CONSTRAINT FK_MemberOrgCulture_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

ALTER TABLE MemberOrgAttribute ADD CONSTRAINT FK_MemberOrgAttribute_GeneralOptional FOREIGN KEY (OptID) REFERENCES GeneralOptional (Gid);
ALTER TABLE MemberOrgAttribute ADD CONSTRAINT FK_MemberOrgAttribute_GeneralOptItem FOREIGN KEY (OptResult) REFERENCES GeneralOptItem (Gid);

ALTER TABLE MemberRole ADD CONSTRAINT FK_MemberRole_GeneralStandardCategory FOREIGN KEY (Rtype) REFERENCES GeneralStandardCategory (Gid);

ALTER TABLE MemberUser ADD CONSTRAINT FK_MemberUser_GeneralCultureUnit FOREIGN KEY (Culture) REFERENCES GeneralCultureUnit (Gid);
ALTER TABLE MemberUser ADD CONSTRAINT FK_MemberUser_MemberLevel FOREIGN KEY (UserLevel) REFERENCES MemberLevel (Gid);

ALTER TABLE MemberAddress ADD CONSTRAINT FK_MemberAddress_GeneralRegion FOREIGN KEY (Location) REFERENCES GeneralRegion (Gid);

ALTER TABLE MemberUserAttribute ADD CONSTRAINT FK_MemberUserAttribute_GeneralOptional FOREIGN KEY (OptID) REFERENCES GeneralOptional (Gid);
ALTER TABLE MemberUserAttribute ADD CONSTRAINT FK_MemberUserAttribute_GeneralOptItem FOREIGN KEY (OptResult) REFERENCES GeneralOptItem (Gid);

ALTER TABLE MemberPoint ADD CONSTRAINT FK_MemberPoint_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MemberPoint ADD CONSTRAINT FK_MemberPoint_PromotionInformation FOREIGN KEY (PromID) REFERENCES PromotionInformation (Gid);
ALTER TABLE MemberPoint ADD CONSTRAINT FK_MemberPoint_PromotionCoupon FOREIGN KEY (CouponID) REFERENCES PromotionCoupon (Gid);
ALTER TABLE MemberPoint ADD CONSTRAINT FK_MemberPoint_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

ALTER TABLE MemberLevel ADD CONSTRAINT FK_MemberLevel_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MemberLevel ADD CONSTRAINT FK_MemberLevel_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE MemberUserShortcut ADD CONSTRAINT FK_MemberUserShortcut_GeneralProgram FOREIGN KEY (ProgID) REFERENCES GeneralProgram (Gid);

-- Product模块外键
ALTER TABLE ProductInformation ADD CONSTRAINT FK_ProductInformation_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ProductInformation ADD CONSTRAINT FK_ProductInformation_GeneralResource_Name FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInformation ADD CONSTRAINT FK_ProductInformation_GeneralStandardCategory FOREIGN KEY (StdCatID) REFERENCES GeneralStandardCategory (Gid);
ALTER TABLE ProductInformation ADD CONSTRAINT FK_ProductInformation_GeneralResource_Brief FOREIGN KEY (Brief) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInformation ADD CONSTRAINT FK_ProductInformation_GeneralLargeObject FOREIGN KEY (Matter) REFERENCES GeneralLargeObject (Gid);

ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralResource_FullName FOREIGN KEY (FullName) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralResource_ShortName FOREIGN KEY (ShortName) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralMeasureUnit FOREIGN KEY (StdUnit) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralResource_Specification FOREIGN KEY (Specification) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralResource_MarketPrice FOREIGN KEY (MarketPrice) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralResource_SuggestPrice FOREIGN KEY (SuggestPrice) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductInfoItem ADD CONSTRAINT FK_ProductInfoItem_GeneralResource_LowestPrice FOREIGN KEY (LowestPrice) REFERENCES GeneralResource (Gid);

ALTER TABLE ProductExtendCategory ADD CONSTRAINT FK_ProductExtendCategory_GeneralPrivateCategory FOREIGN KEY (PrvCatID) REFERENCES GeneralPrivateCategory (Gid);

ALTER TABLE ProductExtendAttribute ADD CONSTRAINT FK_ProductExtendAttribute_GeneralOptional FOREIGN KEY (OptID) REFERENCES GeneralOptional (Gid);
ALTER TABLE ProductExtendAttribute ADD CONSTRAINT FK_ProductExtendAttribute_GeneralOptItem FOREIGN KEY (OptResult) REFERENCES GeneralOptItem (Gid);

ALTER TABLE ProductGallery ADD CONSTRAINT FK_ProductGallery_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);

ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_GeneralResource_Name FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_GeneralResource_MarketPrice FOREIGN KEY (MarketPrice) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_GeneralResource_SalePrice FOREIGN KEY (SalePrice) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_GeneralResource_Brief FOREIGN KEY (Brief) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_GeneralLargeObject FOREIGN KEY (Matter) REFERENCES GeneralLargeObject (Gid);

ALTER TABLE ProductOnItem ADD CONSTRAINT FK_ProductOnItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);
ALTER TABLE ProductOnItem ADD CONSTRAINT FK_ProductOnItem_GeneralResource_FullName FOREIGN KEY (FullName) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnItem ADD CONSTRAINT FK_ProductOnItem_GeneralResource_ShortName FOREIGN KEY (ShortName) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnItem ADD CONSTRAINT FK_ProductOnItem_GeneralResource_ScoreDeduct FOREIGN KEY (ScoreDeduct) REFERENCES GeneralResource (Gid);

ALTER TABLE ProductOnUnitPrice ADD CONSTRAINT FK_ProductOnUnitPrice_GeneralMeasureUnit FOREIGN KEY (ShowUnit) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE ProductOnUnitPrice ADD CONSTRAINT FK_ProductOnUnitPrice_GeneralResource_MarketPrice FOREIGN KEY (MarketPrice) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnUnitPrice ADD CONSTRAINT FK_ProductOnUnitPrice_GeneralResource_SalePrice FOREIGN KEY (SalePrice) REFERENCES GeneralResource (Gid);

ALTER TABLE ProductOnLevelDiscount ADD CONSTRAINT FK_ProductOnLevelDiscount_MemberLevel FOREIGN KEY (UserLevel) REFERENCES MemberLevel (Gid);

ALTER TABLE ProductOnTemplate ADD CONSTRAINT FK_ProductOnTemplate_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ProductOnTemplate ADD CONSTRAINT FK_ProductOnTemplate_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE ProductOnShipping ADD CONSTRAINT FK_ProductOnShipping_MemberOrganization FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);

ALTER TABLE ProductOnShipArea ADD CONSTRAINT FK_ProductOnShipArea_GeneralRegion FOREIGN KEY (RegionID) REFERENCES GeneralRegion (Gid);

ALTER TABLE ProductOnPayment ADD CONSTRAINT FK_ProductOnPayment_FinancePayType FOREIGN KEY (PayID) REFERENCES FinancePayType (Gid);

ALTER TABLE ProductOnAdjust ADD CONSTRAINT FK_ProductOnAdjust_ProductOnItem FOREIGN KEY (OnSkuID) REFERENCES ProductOnItem (Gid);
ALTER TABLE ProductOnAdjust ADD CONSTRAINT FK_ProductOnAdjust_GeneralResource_SalePriceOld FOREIGN KEY (SalePriceOld) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnAdjust ADD CONSTRAINT FK_ProductOnAdjust_GeneralResource_ScoreDeductOld FOREIGN KEY (ScoreDeductOld) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnAdjust ADD CONSTRAINT FK_ProductOnAdjust_GeneralResource_SalePriceNew FOREIGN KEY (SalePriceNew) REFERENCES GeneralResource (Gid);
ALTER TABLE ProductOnAdjust ADD CONSTRAINT FK_ProductOnAdjust_GeneralResource_ScoreDeductNew FOREIGN KEY (ScoreDeductNew) REFERENCES GeneralResource (Gid);

-- Purchase模块外键
ALTER TABLE PurchaseInformation ADD CONSTRAINT FK_PurchaseInformation_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE PurchaseInformation ADD CONSTRAINT FK_PurchaseInformation_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE PurchaseInformation ADD CONSTRAINT FK_PurchaseInformation_MemberOrganization_Supplier FOREIGN KEY (Supplier) REFERENCES MemberOrganization (Gid);
ALTER TABLE PurchaseInformation ADD CONSTRAINT FK_PurchaseInformation_GeneralPrivateCategory FOREIGN KEY (Ptype) REFERENCES GeneralPrivateCategory (Gid);
ALTER TABLE PurchaseInformation ADD CONSTRAINT FK_PurchaseInformation_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

ALTER TABLE PurchaseItem ADD CONSTRAINT FK_PurchaseItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);

ALTER TABLE PurchaseInspection ADD CONSTRAINT FK_PurchaseInspection_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE PurchaseInspection ADD CONSTRAINT FK_PurchaseInspection_PurchaseInformation FOREIGN KEY (PurID) REFERENCES PurchaseInformation (Gid);

ALTER TABLE PurchaseInspItem ADD CONSTRAINT FK_PurchaseInspItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);

-- Warehouse模块外键
ALTER TABLE WarehouseShelf ADD CONSTRAINT FK_WarehouseShelf_MemberOrganization FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseChannel ADD CONSTRAINT FK_WarehouseChannel_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseChannel ADD CONSTRAINT FK_WarehouseChannel_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseRegion ADD CONSTRAINT FK_WarehouseRegion_MemberOrganization FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseRegion ADD CONSTRAINT FK_WarehouseRegion_GeneralRegion FOREIGN KEY (RegionID) REFERENCES GeneralRegion (Gid);

ALTER TABLE WarehouseShipping ADD CONSTRAINT FK_WarehouseShipping_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseShipping ADD CONSTRAINT FK_WarehouseShipping_MemberOrganization_ShipID FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseLedger ADD CONSTRAINT FK_WarehouseLedger_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseLedger ADD CONSTRAINT FK_WarehouseLedger_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);
ALTER TABLE WarehouseLedger ADD CONSTRAINT FK_WarehouseLedger_GeneralResource_AvgCost FOREIGN KEY (AvgCost) REFERENCES GeneralResource (Gid);

ALTER TABLE WarehouseSkuShelf ADD CONSTRAINT FK_WarehouseSkuShelf_MemberOrganization FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseSkuShelf ADD CONSTRAINT FK_WarehouseSkuShelf_WarehouseShelf FOREIGN KEY (ShelfID) REFERENCES WarehouseShelf (Gid);
ALTER TABLE WarehouseSkuShelf ADD CONSTRAINT FK_WarehouseSkuShelf_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);

ALTER TABLE WarehouseStockIn ADD CONSTRAINT FK_WarehouseStockIn_MemberOrganization FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseStockIn ADD CONSTRAINT FK_WarehouseStockIn_GeneralStandardCategory FOREIGN KEY (InType) REFERENCES GeneralStandardCategory (Gid);

ALTER TABLE WarehouseInItem ADD CONSTRAINT FK_WarehouseInItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);
ALTER TABLE WarehouseInItem ADD CONSTRAINT FK_WarehouseInItem_WarehouseShelf FOREIGN KEY (ShelfID) REFERENCES WarehouseShelf (Gid);

ALTER TABLE WarehouseStockOut ADD CONSTRAINT FK_WarehouseStockOut_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseStockOut ADD CONSTRAINT FK_WarehouseStockOut_GeneralStandardCategory FOREIGN KEY (OutType) REFERENCES GeneralStandardCategory (Gid);
ALTER TABLE WarehouseStockOut ADD CONSTRAINT FK_WarehouseStockOut_MemberOrganization_ShipID FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseOutItem ADD CONSTRAINT FK_WarehouseOutItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);
ALTER TABLE WarehouseOutItem ADD CONSTRAINT FK_WarehouseOutItem_WarehouseShelf FOREIGN KEY (ShelfID) REFERENCES WarehouseShelf (Gid);

ALTER TABLE WarehouseOutScan ADD CONSTRAINT FK_WarehouseOutScan_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);

ALTER TABLE WarehouseOutDelivery ADD CONSTRAINT FK_WarehouseOutDelivery_MemberOrganization FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseMoving ADD CONSTRAINT FK_WarehouseMoving_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseMoving ADD CONSTRAINT FK_WarehouseMoving_MemberOrganization_OldWhID FOREIGN KEY (OldWhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseMoving ADD CONSTRAINT FK_WarehouseMoving_MemberOrganization_NewWhID FOREIGN KEY (NewWhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseMoving ADD CONSTRAINT FK_WarehouseMoving_MemberOrganization_ShipID FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseMoveItem ADD CONSTRAINT FK_WarehouseMoveItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);
ALTER TABLE WarehouseMoveItem ADD CONSTRAINT FK_WarehouseMoveItem_WarehouseShelf_OldShelf FOREIGN KEY (OldShelf) REFERENCES WarehouseShelf (Gid);
ALTER TABLE WarehouseMoveItem ADD CONSTRAINT FK_WarehouseMoveItem_WarehouseShelf_NewShelf FOREIGN KEY (NewShelf) REFERENCES WarehouseShelf (Gid);

ALTER TABLE WarehouseInventory ADD CONSTRAINT FK_WarehouseInventory_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE WarehouseInventory ADD CONSTRAINT FK_WarehouseInventory_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);

ALTER TABLE WarehouseInvItem ADD CONSTRAINT FK_WarehouseInvItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);
ALTER TABLE WarehouseInvItem ADD CONSTRAINT FK_WarehouseInvItem_WarehouseShelf FOREIGN KEY (ShelfID) REFERENCES WarehouseShelf (Gid);

-- Order模块外键
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_MemberOrganization_WhID FOREIGN KEY (WhID) REFERENCES MemberOrganization (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_FinancePayType FOREIGN KEY (PayID) REFERENCES FinancePayType (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_GeneralRegion FOREIGN KEY (Location) REFERENCES GeneralRegion (Gid);
ALTER TABLE OrderInformation ADD CONSTRAINT FK_OrderInformation_UnionAdvertising FOREIGN KEY (AdvID) REFERENCES UnionAdvertising (Gid);

ALTER TABLE OrderItem ADD CONSTRAINT FK_OrderItem_ProductOnItem FOREIGN KEY (OnSkuID) REFERENCES ProductOnItem (Gid);
ALTER TABLE OrderItem ADD CONSTRAINT FK_OrderItem_ProductInfoItem FOREIGN KEY (SkuID) REFERENCES ProductInfoItem (Gid);

ALTER TABLE OrderShipping ADD CONSTRAINT FK_OrderShipping_MemberOrganization FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);

ALTER TABLE OrderAttribute ADD CONSTRAINT FK_OrderAttribute_GeneralOptional FOREIGN KEY (OptID) REFERENCES GeneralOptional (Gid);
ALTER TABLE OrderAttribute ADD CONSTRAINT FK_OrderAttribute_GeneralOptItem FOREIGN KEY (OptResult) REFERENCES GeneralOptItem (Gid);

ALTER TABLE OrderSplitPolicy ADD CONSTRAINT FK_OrderSplitPolicy_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE OrderSplitPolicy ADD CONSTRAINT FK_OrderSplitPolicy_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

ALTER TABLE PromotionInformation ADD CONSTRAINT FK_PromotionInformation_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE PromotionInformation ADD CONSTRAINT FK_PromotionInformation_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE PromotionInformation ADD CONSTRAINT FK_PromotionInformation_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE PromotionProduct ADD CONSTRAINT FK_PromotionProduct_ProductOnItem FOREIGN KEY (OnSkuID) REFERENCES ProductOnItem (Gid);
ALTER TABLE PromotionProduct ADD CONSTRAINT FK_PromotionProduct_GeneralResource FOREIGN KEY (Price) REFERENCES GeneralResource (Gid);

ALTER TABLE PromotionCoupon ADD CONSTRAINT FK_PromotionCoupon_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

-- Shipping模块外键
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_MemberOrganization FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralRegion FOREIGN KEY (RegionID) REFERENCES GeneralRegion (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_Residential FOREIGN KEY (Residential) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_LiftGate FOREIGN KEY (LiftGate) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_Installation FOREIGN KEY (Installation) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_PriceWeight FOREIGN KEY (PriceWeight) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_PriceVolume FOREIGN KEY (PriceVolume) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_PricePiece FOREIGN KEY (PricePiece) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_PriceHigh FOREIGN KEY (PriceHigh) REFERENCES GeneralResource (Gid);
ALTER TABLE ShippingArea ADD CONSTRAINT FK_ShippingArea_GeneralResource_PriceLow FOREIGN KEY (PriceLow) REFERENCES GeneralResource (Gid);

ALTER TABLE ShippingEnvelope ADD CONSTRAINT FK_ShippingEnvelope_MemberOrganization FOREIGN KEY (ShipID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ShippingEnvelope ADD CONSTRAINT FK_ShippingEnvelope_GeneralLargeObject FOREIGN KEY (Template) REFERENCES GeneralLargeObject (Gid);

-- Mall模块外键
ALTER TABLE MallArtPosition ADD CONSTRAINT FK_MallArtPosition_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallArtPosition ADD CONSTRAINT FK_MallArtPosition_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);
ALTER TABLE MallArtPosition ADD CONSTRAINT FK_MallArtPosition_GeneralLargeObject FOREIGN KEY (Template) REFERENCES GeneralLargeObject (Gid);

ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_GeneralPrivateCategory_Atype FOREIGN KEY (Atype) REFERENCES GeneralPrivateCategory (Gid);
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_GeneralPrivateCategory_Acategory FOREIGN KEY (Acategory) REFERENCES GeneralPrivateCategory (Gid);
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid);
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_GeneralResource FOREIGN KEY (Title) REFERENCES GeneralResource (Gid);
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_GeneralLargeObject FOREIGN KEY (Matter) REFERENCES GeneralLargeObject (Gid);

ALTER TABLE MallArtPublish ADD CONSTRAINT FK_MallArtPublish_MemberOrganization FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallArtPublish ADD CONSTRAINT FK_MallArtPublish_MallArtPosition FOREIGN KEY (PosID) REFERENCES MallArtPosition (Gid);

ALTER TABLE MallHotWord ADD CONSTRAINT FK_MallHotWord_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallHotWord ADD CONSTRAINT FK_MallHotWord_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

ALTER TABLE MallHotItem ADD CONSTRAINT FK_MallHotItem_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallHotItem ADD CONSTRAINT FK_MallHotItem_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

ALTER TABLE MallFavorite ADD CONSTRAINT FK_MallFavorite_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallFavorite ADD CONSTRAINT FK_MallFavorite_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallFavorite ADD CONSTRAINT FK_MallFavorite_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);
ALTER TABLE MallFavorite ADD CONSTRAINT FK_MallFavorite_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid);

ALTER TABLE MallFocusProduct ADD CONSTRAINT FK_MallFocusProduct_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallFocusProduct ADD CONSTRAINT FK_MallFocusProduct_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallFocusProduct ADD CONSTRAINT FK_MallFocusProduct_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);
ALTER TABLE MallFocusProduct ADD CONSTRAINT FK_MallFocusProduct_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid);
ALTER TABLE MallFocusProduct ADD CONSTRAINT FK_MallFocusProduct_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

ALTER TABLE MallFriendLink ADD CONSTRAINT FK_MallFriendLink_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallFriendLink ADD CONSTRAINT FK_MallFriendLink_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallFriendLink ADD CONSTRAINT FK_MallFriendLink_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE MallSensitiveWord ADD CONSTRAINT FK_MallSensitiveWord_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallSensitiveWord ADD CONSTRAINT FK_MallSensitiveWord_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

ALTER TABLE MallDisabledIp ADD CONSTRAINT FK_MallDisabledIp_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallDisabledIp ADD CONSTRAINT FK_MallDisabledIp_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

ALTER TABLE MallVisitClick ADD CONSTRAINT FK_MallVisitClick_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallVisitClick ADD CONSTRAINT FK_MallVisitClick_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallVisitClick ADD CONSTRAINT FK_MallVisitClick_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);
ALTER TABLE MallVisitClick ADD CONSTRAINT FK_MallVisitClick_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid);
ALTER TABLE MallVisitClick ADD CONSTRAINT FK_MallVisitClick_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);
ALTER TABLE MallVisitClick ADD CONSTRAINT FK_MallVisitClick_UnionAdvertising FOREIGN KEY (AdvID) REFERENCES UnionAdvertising (Gid);

ALTER TABLE MallCart ADD CONSTRAINT FK_MallCart_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallCart ADD CONSTRAINT FK_MallCart_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MallCart ADD CONSTRAINT FK_MallCart_MemberUser FOREIGN KEY (UserID) REFERENCES MemberUser (Gid);
ALTER TABLE MallCart ADD CONSTRAINT FK_MallCart_ProductOnSale FOREIGN KEY (OnSaleID) REFERENCES ProductOnSale (Gid);
ALTER TABLE MallCart ADD CONSTRAINT FK_MallCart_ProductOnItem FOREIGN KEY (OnSkuID) REFERENCES ProductOnItem (Gid);
ALTER TABLE MallCart ADD CONSTRAINT FK_MallCart_GeneralStandardCategory FOREIGN KEY (Ctype) REFERENCES GeneralStandardCategory (Gid);

-- Union模块外键
ALTER TABLE UnionLevelPercent ADD CONSTRAINT FK_UnionLevelPercent_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE UnionLevelPercent ADD CONSTRAINT FK_UnionLevelPercent_MemberRole FOREIGN KEY (RoleID) REFERENCES MemberRole (Gid);

ALTER TABLE UnionFixedPercent ADD CONSTRAINT FK_UnionFixedPercent_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE UnionFixedPercent ADD CONSTRAINT FK_UnionFixedPercent_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

-- Knowledge模块外键

-- Finance模块外键
ALTER TABLE FinancePayType ADD CONSTRAINT FK_FinancePayType_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE FinancePayType ADD CONSTRAINT FK_FinancePayType_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE FinanceInvoice ADD CONSTRAINT FK_FinanceInvoice_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid);

ALTER TABLE FinancePayment ADD CONSTRAINT FK_FinancePayment_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE FinancePayment ADD CONSTRAINT FK_FinancePayment_GeneralPrivateCategory FOREIGN KEY (Ptype) REFERENCES GeneralPrivateCategory (Gid);
ALTER TABLE FinancePayment ADD CONSTRAINT FK_FinancePayment_GeneralMeasureUnit FOREIGN KEY (Currency) REFERENCES GeneralMeasureUnit (Gid);

-- Exchange模块外键
ALTER TABLE ExTaobaoOrder ADD CONSTRAINT FK_ExTaobaoOrder_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ExTaobaoOrder ADD CONSTRAINT FK_ExTaobaoOrder_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ExTaobaoOrder ADD CONSTRAINT FK_ExTaobaoOrder_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid);

ALTER TABLE ExTaobaoUser ADD CONSTRAINT FK_ExTaobaoUser_ExTaobaoUserCredit_buyer_credit FOREIGN KEY (buyer_credit) REFERENCES ExTaobaoUserCredit (Gid);
ALTER TABLE ExTaobaoUser ADD CONSTRAINT FK_ExTaobaoUser_ExTaobaoUserCredit_seller_credit FOREIGN KEY (seller_credit) REFERENCES ExTaobaoUserCredit (Gid);

ALTER TABLE ExTaobaoUserOrgan ADD CONSTRAINT FK_ExTaobaoUserOrgan_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ExTaobaoUserOrgan ADD CONSTRAINT FK_ExTaobaoUserOrgan_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ExTaobaoUserOrgan ADD CONSTRAINT FK_ExTaobaoUserOrgan_ExTaobaoUser FOREIGN KEY (TopUser) REFERENCES ExTaobaoUser (Gid);

GO


-- 创建视图
CREATE VIEW viewResourceMatter AS
  -- 合并获取字符型资源
  SELECT gr.Gid, gr.Culture, gr.Matter
    FROM GeneralResource gr
  UNION
  SELECT gri.ResID AS Gid, gri.Culture, gri.Matter
    FROM GeneralResItem gri
GO

CREATE VIEW viewResourceMoney AS
  -- 合并获取金额型资源
  SELECT gr.Gid, gr.Currency, gr.Cash
    FROM GeneralResource gr
  UNION
  SELECT gri.ResID AS Gid, gri.Currency, gri.Cash
    FROM GeneralResItem gri;
GO

CREATE VIEW viewLargeObject AS
  -- 合并获取大对象资源
  SELECT gl.Gid, gl.Culture, gl.CLOB
    FROM GeneralLargeObject gl
  UNION
  SELECT gli.BlobID AS Gid, gli.Culture, gli.CLOB
    FROM GeneralLargeItem gli
GO

CREATE VIEW viewRandom AS
  -- 产生随机数，用于函数中；RAND不能直接在函数中使用
  SELECT RAND() AS random;
GO


-- 创建触发器
CREATE TRIGGER TR_GeneralErrorReport ON GeneralErrorReport AFTER INSERT AS
BEGIN
  -- 生成错误报告代码
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'ErrorReportCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'ER' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'ER' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE GeneralErrorReport SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'ErrorReportCode';
  -- WAITFOR DELAY '00:00:15';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_GeneralTodoList ON GeneralTodoList AFTER INSERT AS
BEGIN
  -- 生成错误报告代码
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'TodoListCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'TD' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'TD' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE GeneralTodoList SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'TodoListCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_MemberOrganization ON MemberOrganization AFTER INSERT AS
BEGIN
  -- 新建运营商组织时，同时新建两个角色，同时设置PU/SKU/条码的默认正则表达式
  -- 新建仓库时，同时新建三个货架
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @Otype tinyint;   -- 组织类型
  DECLARE @Code nvarchar (50);
  
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResourceGid uniqueidentifier;
  DECLARE @InternalGid uniqueidentifier;
  
  DECLARE @RolePublic uniqueidentifier;
  DECLARE @RoleInternal uniqueidentifier;
  
  -- 插入新组织
  SELECT @InsertedGid = Gid, @Otype = Otype, @Code = Code FROM inserted;
  IF (@Otype = 0)           -- 运营商
  BEGIN
    -- 角色类型
    SELECT @RolePublic = sc.Gid FROM GeneralStandardCategory sc WHERE sc.Code = 'Public' AND sc.Ctype = 2;
    SELECT @RoleInternal = sc.Gid FROM GeneralStandardCategory sc WHERE sc.Code = 'Internal' AND sc.Ctype = 2;
    
    -- 添加公众角色
    INSERT GeneralResource (Rtype, Culture, Matter) OUTPUT inserted.Gid INTO @GeneratedKeys
      VALUES (0, 1033, 'Public User');
    SELECT @ResourceGid = g.Gid FROM @GeneratedKeys g;
    INSERT GeneralResItem (ResID, Culture, Matter) VALUES (@ResourceGid, 2052, '公众用户');
    INSERT MemberRole (OrgID, Code, Rtype, Name, Remark)
      VALUES (@InsertedGid, 'Public', @RolePublic, @ResourceGid, '公众用户');
    
    -- 添加内部用户角色
    INSERT GeneralResource (Rtype, Culture, Matter) OUTPUT inserted.Gid INTO @GeneratedKeys
      VALUES (0, 1033, 'Internal User');
    SELECT @ResourceGid = g.Gid FROM @GeneratedKeys g;
    INSERT GeneralResItem (ResID, Culture, Matter) VALUES (@ResourceGid, 2052, '内部用户');
    INSERT MemberRole (OrgID, Code, Rtype, Name, Remark) OUTPUT inserted.Gid INTO @GeneratedKeys
      VALUES (@InsertedGid, 'Internal', @RoleInternal, @ResourceGid, '内部用户');
    SELECT @InternalGid = g.Gid FROM @GeneratedKeys g;
    
    -- 添加超级管理员角色，仅Zhuchao组织
    IF (@Code = 'Zhuchao')
    BEGIN
      INSERT GeneralResource (Rtype, Culture, Matter) OUTPUT inserted.Gid INTO @GeneratedKeys VALUES (0, 1033, 'Supervisor');
      SELECT @ResourceGid = g.Gid FROM @GeneratedKeys g;
      INSERT GeneralResItem (ResID, Culture, Matter) VALUES (@ResourceGid, 2052, '超级管理员');
      INSERT MemberRole (OrgID, Code, Parent, Name, Remark) VALUES (@InsertedGid, 'Supervisor', @InternalGid, @ResourceGid, '超级管理员');
    END;
    
    -- 设置PU/SKU/条码的默认正则表达式
    UPDATE MemberOrganization
      SET ProdCodePolicy = '^[1-9]\d{9}',
          SkuCodePolicy = '^[1-9]\d{12}',
          BarcodePolicy = '^[1-9]\d{12}'
      WHERE Gid = @InsertedGid;
  END
  ELSE IF (@Otype = 2)    -- 仓库
  BEGIN
    INSERT WarehouseShelf (WhID, Code, Barcode, Reserved, Name, Brief)
      VALUES (@InsertedGid, 'BUFFER', 'BUFFER', 1, 'Buffer', '入库货位未定时的临时堆放区');
    INSERT WarehouseShelf (WhID, Code, Barcode, Reserved, Name, Brief)
      VALUES (@InsertedGid, 'DEFECT_GOOD', 'DEFECTGOOD', 1, 'Defect (Good)', '次品区域，但还能使用');
    INSERT WarehouseShelf (WhID, Code, Barcode, Reserved, Name, Brief)
      VALUES (@InsertedGid, 'DEFECT_BAD', 'DEFECTBAD', 1, 'Defect (Bad)', '次品区域，完全不能使用了');
  END;
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_PurchaseInformation ON PurchaseInformation AFTER INSERT AS
BEGIN
  -- 生成采购单号，批次号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @OrgID uniqueidentifier;
  DECLARE @Supplier uniqueidentifier;
  DECLARE @TrackLot int = 1;
  
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  -- 生成代码
  SELECT @InsertedGid = Gid, @OrgID = OrgID, @Supplier = Supplier FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'PurchaseCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'PR' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'PR' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  -- 默认生成批次号，如果有Disabled，则不生成批次号
  IF NOT EXISTS (
    SELECT 1
      FROM MemberOrgAttribute moa
      JOIN GeneralOptional opt ON moa.OptID = opt.Gid
      JOIN GeneralOptItem opi ON opt.Gid = opi.OptID AND moa.OptResult = opi.Gid
      WHERE moa.OrgID = @OrgID
            AND opi.Code = 'Disabled')
  BEGIN
    SELECT @TrackLot = MAX(TrackLot) FROM PurchaseInformation WHERE OrgID = @OrgID;
    IF (@TrackLot IS NULL)
      SET @TrackLot = 1;
    ELSE
      SET @TrackLot = @TrackLot + 1;
  END;
  
  UPDATE PurchaseInformation SET Code = @NewCode, TrackLot = @TrackLot WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'PurchaseCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_PurchaseItem ON PurchaseItem AFTER INSERT, UPDATE AS
BEGIN
  -- 计算采购单件平均成本，不含税单件+单件运费
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @Quantity decimal (18,4);
  DECLARE @AvgCost money;
  DECLARE @Amount money;
  
  SELECT @InsertedGid = Gid, @Quantity = Quantity, @Amount = Amount FROM inserted;
  IF (@Quantity = 0)
    SET @AvgCost = 0;
  ELSE
    SET @AvgCost = @Amount / @Quantity;
  
  UPDATE PurchaseItem SET AvgCost = @AvgCost WHERE Gid = @InsertedGid;
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_PurchaseInspection ON PurchaseInspection AFTER INSERT AS
BEGIN
  -- 生成质检单号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'InspectionCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'QC' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'QC' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE PurchaseInspection SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'InspectionCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_WarehouseLedger ON WarehouseLedger AFTER INSERT, UPDATE AS
BEGIN
  -- 更新库存总账的计算字段
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @RealQty decimal (18,4);      -- 实际库存
  DECLARE @CanSaleQty decimal (18,4);   -- 可用销售库存
  DECLARE @CanDelivery decimal (18,4);  -- 可用现货库存
  
  IF (UPDATE(InQty) OR UPDATE(OutQty) OR UPDATE(LockQty) OR UPDATE(TobeDelivery) OR UPDATE(Arranged))
  BEGIN
    SELECT @InsertedGid = Gid,
           @RealQty = InQty - OutQty,
           @CanSaleQty = InQty - OutQty - LockQty - TobeDelivery,
           @CanDelivery = InQty - OutQty - LockQty - Arranged
      FROM inserted;
    UPDATE WarehouseLedger
      SET RealQty = @RealQty,
          CanSaleQty = @CanSaleQty,
          CanDelivery = @CanDelivery
      WHERE Gid = @InsertedGid;
  END;
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_WarehouseStockIn ON WarehouseStockIn AFTER INSERT AS
BEGIN
  -- 生成入库单号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'StockInCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'IS' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'IS' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE WarehouseStockIn SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'StockInCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_WarehouseStockOut ON WarehouseStockOut AFTER INSERT AS
BEGIN
  -- 生成出库单号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'StockOutCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'OS' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'OS' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE WarehouseStockOut SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'StockOutCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_WarehouseMoving ON WarehouseMoving AFTER INSERT AS
BEGIN
  -- 生成移库/移货位单号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'MovingCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'MV' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'MV' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE WarehouseMoving SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'MovingCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_WarehouseInventory ON WarehouseInventory AFTER INSERT AS
BEGIN
  -- 生成盘点单号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'InventoryCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'IV' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'IV' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE WarehouseInventory SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'InventoryCode';
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_OrderInformation ON OrderInformation AFTER INSERT, UPDATE AS
BEGIN
  -- 生成订单号，更新计算字段，匹配仓库
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  DECLARE @TotalFee money;
  DECLARE @TotalPaid money;
  
  DECLARE @TableKeys TABLE (Gid uniqueidentifier);
  DECLARE @OrgID uniqueidentifier;
  DECLARE @ChlID uniqueidentifier;
  DECLARE @WhID uniqueidentifier;
  DECLARE @Location uniqueidentifier;
  
  -- 更新计算字段
  SELECT @InsertedGid = Gid,
         @OrgID = OrgID,
         @ChlID = ChlID,
         @WhID = WhID,
         @Location = Location,
         @TotalFee = ExecuteAmount + ShippingFee + TaxFee + Insurance + PaymentFee + PackingFee + ResidenceFee + LiftGateFee + InstallFee + OtherFee,
         @TotalPaid = PointPay + CouponPay + BounsPay + MoneyPaid + Discount
    FROM inserted;
  
  IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
  BEGIN
    -- 插入新订单，生成订单号
    SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_N';
    SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'OrderCode';
    
    SET @NewValue = @NewValue + 1;
    SET @NewCode = CAST(@NewValue AS nvarchar(50));
    IF (LEN(@NewCode) <= 6)
      SET @NewCode = @CodePrefix + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
    ELSE
      SET @NewCode = @CodePrefix + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
    
    -- 匹配仓库
    IF (@WhID IS NULL)
      SET @WhID = dbo.fn_FindBestWarehouse(@OrgID, @ChlID, @Location);
    
    -- 保存
    UPDATE OrderInformation
      SET Code = @NewCode,
          WhID = @WhID,
          TotalFee = @TotalFee,
          TotalPaid = @TotalPaid,
          OrderAmount = @TotalFee - @TotalPaid
      WHERE Gid = @InsertedGid;
    UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'OrderCode';
  END;
  ELSE IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
  BEGIN
    -- 更新计算字段
    UPDATE OrderInformation
      SET TotalFee = @TotalFee,
          TotalPaid = @TotalPaid,
          OrderAmount = @TotalFee - @TotalPaid
      WHERE Gid = @InsertedGid;
  END;
  COMMIT TRANSACTION;
END;
GO

CREATE TRIGGER TR_FinancePayment ON FinancePayment AFTER INSERT AS
BEGIN
  -- 生成应付单号
  BEGIN TRANSACTION;
  DECLARE @InsertedGid uniqueidentifier;
  DECLARE @CodePrefix nvarchar (256);
  DECLARE @NewValue int;
  DECLARE @NewCode nvarchar (50);
  
  SELECT @InsertedGid = Gid FROM inserted;
  SELECT @CodePrefix = StrValue FROM GeneralConfig WHERE Code = 'CodePrefix_C';
  SELECT @NewValue = IntValue FROM GeneralConfig WHERE Code = 'PaymentCode';
  
  SET @NewValue = @NewValue + 1;
  SET @NewCode = CAST(@NewValue AS nvarchar(50));
  IF (LEN(@NewCode) <= 6)
    SET @NewCode = @CodePrefix + 'PY' + REPLICATE('0', 6-LEN(@NewCode)) + @NewCode;
  ELSE
    SET @NewCode = @CodePrefix + 'PY' + REPLICATE('0', 10-LEN(@NewCode)) + @NewCode;
  
  UPDATE FinancePayment SET Code = @NewCode WHERE Gid = @InsertedGid;
  UPDATE GeneralConfig SET IntValue = @NewValue WHERE Code = 'PaymentCode';
  COMMIT TRANSACTION;
END;
GO


-- 创建标量值函数
CREATE FUNCTION fn_FindDefaultCurrency (@Gid uniqueidentifier) RETURNS uniqueidentifier AS
BEGIN
  -- 查询组织的默认货币，仓库/供应商等向上找到所属组织
  DECLARE @OrganGid uniqueidentifier;
  DECLARE @OrganType tinyint;
  DECLARE @FindGid uniqueidentifier;
  DECLARE @Currency uniqueidentifier;
  
  SET @FindGid = @Gid;
  SELECT @OrganType = Otype, @OrganGid = Parent FROM MemberOrganization WHERE Gid = @Gid;
  WHILE (@OrganType IS NOT NULL) AND (@OrganType <> 0) AND (@OrganGid IS NOT NULL)
  BEGIN
    SET @FindGid = @OrganGid;
    SELECT @OrganType = Otype, @OrganGid = Parent FROM MemberOrganization WHERE Gid = @OrganGid;
  END;
  
  -- 查找默认货币
  SELECT TOP (1) @Currency = Currency
    FROM MemberOrgCulture
    WHERE OrgID = @FindGid
          AND Ctype = 1
    ORDER BY Sorting DESC;
  
  RETURN (@Currency);
END;
GO

CREATE FUNCTION fn_FindDefaultCulture (@Gid uniqueidentifier) RETURNS uniqueidentifier AS
BEGIN
  -- 查询组织的默认语言和计量单位，仓库/供应商等向上找到所属组织
  DECLARE @OrganGid uniqueidentifier;
  DECLARE @OrganType tinyint;
  DECLARE @FindGid uniqueidentifier;
  DECLARE @Culture uniqueidentifier;
  
  SET @FindGid = @Gid;
  SELECT @OrganType = Otype, @OrganGid = Parent FROM MemberOrganization WHERE Gid = @Gid;
  WHILE (@OrganType IS NOT NULL) AND (@OrganType <> 0) AND (@OrganGid IS NOT NULL)
  BEGIN
    SET @FindGid = @OrganGid;
    SELECT @OrganType = Otype, @OrganGid = Parent FROM MemberOrganization WHERE Gid = @OrganGid;
  END;
  
  -- 查找默认语言
  SELECT TOP (1) @Culture = Culture
    FROM MemberOrgCulture
    WHERE OrgID = @FindGid
          AND Ctype = 0
    ORDER BY Sorting DESC;
  
  RETURN (@Culture);
END;
GO

CREATE FUNCTION fn_FindResourceMatter (
  @ResID uniqueidentifier,
  @Culture int )
  RETURNS nvarchar (512) AS
BEGIN
  -- 根据资源ID和语言文化，查询字符资源的值
  DECLARE @ResMatter nvarchar (512);
  SELECT @ResMatter = rm.Matter
    FROM viewResourceMatter rm
    WHERE rm.Gid = @ResID
          AND rm.Culture = @Culture;
  RETURN (@ResMatter);
END;
GO

CREATE FUNCTION fn_FindResourceMoney (
  @ResID uniqueidentifier,
  @Currency uniqueidentifier )
  RETURNS money AS
BEGIN
  -- 根据资源ID和货币ID，查询金额资源的值
  DECLARE @ResMoney money = 0;
  SELECT @ResMoney = rm.Cash
    FROM viewResourceMoney rm
    WHERE rm.Gid = @ResID
          AND rm.Currency = @Currency;
  RETURN (@ResMoney);
END;
GO

CREATE FUNCTION fn_FindFullRegions (@Location uniqueidentifier)
  RETURNS @FindKeys TABLE (Gid uniqueidentifier) AS
BEGIN
  -- 递归查询地区所有上级,完整地区级别
  DECLARE @City uniqueidentifier;
  SELECT @City = gr.Gid FROM GeneralRegion gr WHERE gr.Gid = @Location; -- 验证参数
  WHILE (@City IS NOT NULL)
  BEGIN
    INSERT @FindKeys (Gid) VALUES (@City);
    SELECT @City = gr.Parent FROM GeneralRegion gr WHERE gr.Gid = @City;
  END;
  RETURN;
END;
GO

CREATE FUNCTION fn_FindBestWarehouse (
  @OrgID uniqueidentifier,
  @ChlID uniqueidentifier,
  @Location uniqueidentifier )
  RETURNS uniqueidentifier AS
BEGIN
  -- 根据组织、渠道和地区，匹配最佳仓库
  DECLARE @WhID uniqueidentifier;
  -- 运营商下的支持当前渠道的所有仓库
  SELECT TOP (1) @WhID = wh.Gid
    FROM MemberOrganization wh
    JOIN MemberOrgChannel oc ON wh.Gid = oc.OrgID
    JOIN WarehouseRegion wr ON wr.WhID = wh.Gid
    WHERE wh.Parent = @OrgID                  -- 仅支持两层结构
          AND oc.ChlID = @ChlID
          AND wh.Otype = 2                    -- 仓库类型
          AND wr.RegionID IN (SELECT Gid FROM fn_FindFullRegions(@Location));
  IF (@WhID IS NULL)
  BEGIN
    -- 没有任何一个仓库能匹配，则随机找当前组织下的一个仓库
    SELECT TOP (1) @WhID = Gid FROM MemberOrganization wh WHERE wh.Parent = @OrgID AND wh.Otype = 2;
  END;
  RETURN (@WhID);
END;
GO

CREATE FUNCTION fn_FindAllShippings (
  @OrgID uniqueidentifier,
  @ChlID uniqueidentifier,
  @UserID uniqueidentifier,
  @Location uniqueidentifier )
  RETURNS @FindKeys TABLE (ShipID uniqueidentifier, ShipWeight int) AS
BEGIN
  -- 根据MallCart的组织、渠道、用户和地区，查询所有支持的承运商
  -- 请参阅根据OrderItem的同类算法sp_PrepareOrderShippings
  INSERT @FindKeys (ShipID, ShipWeight)
    SELECT DISTINCT ps.ShipID, ps.ShipWeight
      FROM MallCart mc
      JOIN ProductOnItem poi ON mc.OnSkuID = poi.Gid
      JOIN ProductOnSale pos ON poi.OnSaleID = pos.Gid
      JOIN ProductOnShipping ps ON ps.OnSaleID = pos.Gid
      JOIN ProductOnShipArea psa ON psa.OnShip = ps.Gid
      JOIN MemberOrgChannel woc ON woc.ChlID = mc.ChlID
      JOIN MemberOrganization wh ON wh.Gid = woc.OrgID AND wh.Otype = 2 AND wh.Parent = @OrgID
      JOIN WarehouseShipping ws ON ws.WhID = wh.Gid AND ws.ShipID = ps.ShipID
      JOIN WarehouseRegion wr ON wr.WhID = wh.Gid
      JOIN fn_FindFullRegions(@Location) fr ON fr.Gid = psa.RegionID AND fr.Gid = wr.RegionID
      WHERE mc.OrgID = @OrgID
            AND mc.ChlID = @ChlID
            AND mc.UserID = @UserID
      ORDER BY ps.ShipWeight DESC;
  RETURN;
END;
GO

CREATE FUNCTION fn_FindBestShipping (
  @OrgID uniqueidentifier,
  @ChlID uniqueidentifier,
  @UserID uniqueidentifier,
  @Location uniqueidentifier )
  RETURNS uniqueidentifier AS
BEGIN
  -- 根据组织、渠道、用户和地区，查询最佳承运商
  DECLARE @ShipID uniqueidentifier;
  SELECT TOP (1) @ShipID = s.ShipID
    FROM fn_FindAllShippings(@OrgID, @ChlID, @UserID, @Location) s
    ORDER BY s.ShipWeight DESC;
  RETURN (@ShipID);
END;
GO

CREATE FUNCTION fn_FindShippingPrice (
  @Currency uniqueidentifier,
  @OnSkuID uniqueidentifier,
  @Location uniqueidentifier )
  RETURNS money AS
BEGIN
  -- 根据上架SKU和地区，预估运费，用于商品详情页显示
  DECLARE @ShippingFee money = -1;    -- -1表示无法送达
  
  DECLARE @ShipID uniqueidentifier;
  DECLARE @Solution tinyint;
  DECLARE @Discount decimal (18,4);
  
  DECLARE @GrossWeight decimal (18,4);
  DECLARE @GrossVolume decimal (18,4);
  DECLARE @NetPiece int;
  
  DECLARE @Residential uniqueidentifier;
  DECLARE @LiftGate uniqueidentifier;
  DECLARE @Installation uniqueidentifier;
  DECLARE @PriceWeight uniqueidentifier;
  DECLARE @PriceVolume uniqueidentifier;
  DECLARE @PricePiece uniqueidentifier;
  DECLARE @PriceHigh uniqueidentifier;
  DECLARE @PriceLow uniqueidentifier;
  
  -- 查询最佳承运商，不考虑条件Condition，不考虑上楼费等
  SELECT TOP (1) @ShipID = ps.ShipID, @Solution = ps.Solution, @Discount = ps.Discount,
         @GrossWeight = pii.GrossWeight,
         @GrossVolume = pii.GrossVolume,
         @NetPiece = pii.NetPiece,
         @Residential = sa.Residential,
         @LiftGate = sa.LiftGate,
         @Installation = sa.Installation,
         @PriceWeight = sa.PriceWeight,
         @PriceVolume = sa.PriceVolume,
         @PricePiece = sa.PricePiece,
         @PriceHigh = sa.PriceHigh,
         @PriceLow = sa.PriceLow
    FROM ProductOnItem poi
    JOIN ProductOnSale pos ON pos.Gid = poi.OnSaleID
    JOIN ProductOnShipping ps ON ps.OnSaleID = pos.Gid
    JOIN ShippingArea sa ON ps.ShipID = sa.ShipID
    JOIN ProductInfoItem pii ON poi.SkuID = pii.Gid
    JOIN ProductOnShipArea psa ON psa.OnShip = ps.Gid
    JOIN fn_FindFullRegions(@Location) fr ON fr.Gid = sa.RegionID AND fr.Gid = psa.RegionID
    WHERE poi.Gid = @OnSkuID
    ORDER BY ps.ShipWeight DESC;
  SET @ShippingFee =
    CASE @Solution
      WHEN 0 THEN @GrossWeight * dbo.fnFindResourceMoney(@PriceWeight, @Currency)
      WHEN 1 THEN @GrossVolume * dbo.fnFindResourceMoney(@PriceVolume, @Currency)
      WHEN 2 THEN @NetPiece * dbo.fnFindResourceMoney(@PricePiece, @Currency)
    END;
  IF (@ShippingFee > dbo.fnFindResourceMoney(@PriceHigh, @Currency))
    SET @ShippingFee = dbo.fnFindResourceMoney(@PriceHigh, @Currency);
  IF (@ShippingFee < dbo.fnFindResourceMoney(@PriceLow, @Currency))
    SET @ShippingFee = dbo.fnFindResourceMoney(@PriceLow, @Currency);
  RETURN (@ShippingFee);
END;
GO

CREATE FUNCTION fn_CartShippingFee (
  @Currency uniqueidentifier,
  @OrgID uniqueidentifier,
  @ChlID uniqueidentifier,
  @UserID uniqueidentifier,
  @Location uniqueidentifier )
  RETURNS money AS
BEGIN
  -- 根据组织、渠道、用户和地区，查询和计算最佳承运商的运费
  -- 类似算法从OrderItem，参阅fn_OrderShippingFee
  DECLARE @ShipID uniqueidentifier;
  DECLARE @ShippingFee money = -1;    -- -1表示无法送达
  DECLARE @PriceWeight money = -1;
  DECLARE @PriceVolume money = -1;
  DECLARE @PricePiece money = -1;
  DECLARE @PriceHigh money = -1;
  DECLARE @PriceLow money = -1;
  
  SET @ShipID = dbo.fn_FindBestShipping(@OrgID, @ChlID, @UserID, @Location);
  SELECT TOP (1)
         @PriceWeight = dbo.fn_FindResourceMoney(sa.PriceWeight, @Currency),
         @PriceVolume = dbo.fn_FindResourceMoney(sa.PriceVolume, @Currency),
         @PricePiece = dbo.fn_FindResourceMoney(sa.PricePiece, @Currency),
         @PriceHigh = dbo.fn_FindResourceMoney(sa.PriceHigh, @Currency),
         @PriceLow = dbo.fn_FindResourceMoney(sa.PriceLow, @Currency)
    FROM ShippingArea sa
    JOIN fn_FindFullRegions(@Location) fr ON sa.RegionID = fr.Gid
    WHERE sa.ShipID = @ShipID;
  -- 如果找不到记录，表示无法送达
  IF (@PriceWeight + @PriceVolume + @PricePiece) <> -3
  BEGIN
    SELECT @ShippingFee = SUM(s.ShippingFee)
      FROM (
        SELECT mc.OnSkuID,
               ShippingFee = CASE ps.Solution
                 WHEN 0 THEN @PriceWeight * pii.GrossWeight * mc.Quantity * ps.Discount
                 WHEN 1 THEN @PriceVolume * pii.GrossVolume * mc.Quantity * ps.Discount
                 WHEN 2 THEN @PricePiece * pii.NetPiece * mc.Quantity * ps.Discount
               END
          FROM MallCart mc
          LEFT JOIN ProductOnItem poi ON mc.OnSkuID = poi.Gid
          LEFT JOIN ProductOnSale pos ON poi.OnSaleID = pos.Gid
          LEFT JOIN ProductOnShipping ps ON ps.OnSaleID = pos.Gid AND ps.ShipID = @ShipID
          LEFT JOIN ProductInfoItem pii ON poi.SkuID = pii.Gid
          WHERE mc.OrgID = @OrgID
                AND mc.ChlID = @ChlID
                AND mc.UserID = @UserID ) s;
  END;
  RETURN (@ShippingFee);
END;
GO

CREATE FUNCTION fn_FindOnSkuShippings (
  -- @OrgID uniqueidentifier,
  -- @ChlID uniqueidentifier,
  @WhID uniqueidentifier,
  @OnSkuID uniqueidentifier,
  @Location uniqueidentifier )
  RETURNS @FindKeys TABLE (ShipID uniqueidentifier, ShipWeight int) AS
BEGIN
  -- 根据上架SKU、仓库、地区等查询所有支持的承运商
  -- 请参阅根据OrderItem的同类算法sp_PrepareOrderShippings, fn_FindAllShippings
  INSERT @FindKeys (ShipID, ShipWeight)
    SELECT DISTINCT ps.ShipID, ps.ShipWeight
      FROM ProductOnItem poi
      JOIN ProductOnSale pos ON poi.OnSaleID = pos.Gid
      JOIN ProductOnShipping ps ON ps.OnSaleID = pos.Gid
      JOIN ProductOnShipArea psa ON psa.OnShip = ps.Gid
      -- JOIN MemberOrgChannel woc ON woc.ChlID = @ChlID
      -- JOIN MemberOrganization wh ON wh.Gid = woc.OrgID AND wh.Otype = 2 AND wh.Parent = @OrgID
      JOIN WarehouseShipping ws ON ws.ShipID = ps.ShipID AND ws.WhID = @WhID
      JOIN WarehouseRegion wr ON wr.WhID = ws.WhID
      JOIN fn_FindFullRegions(@Location) fr ON fr.Gid = psa.RegionID AND fr.Gid = wr.RegionID
      WHERE poi.Gid = @OnSkuID
      ORDER BY ps.ShipWeight DESC;
  RETURN;
END;
GO

CREATE FUNCTION fn_OrderShippingFee (@OrderID uniqueidentifier)
  RETURNS money AS
BEGIN
  -- 根据订单ID，查询和计算最佳承运商的运费
  -- 类似算法从MallCart，参阅fn_FindShippingFee
  DECLARE @ResultFlag int = 0;
  DECLARE @OrderGid uniqueidentifier;
  DECLARE @Currency uniqueidentifier;
  DECLARE @Location uniqueidentifier;
  DECLARE @ShipID uniqueidentifier;
  DECLARE @ShippingFee money = -1;    -- -1表示无法送达
  DECLARE @PriceWeight money = -1;
  DECLARE @PriceVolume money = -1;
  DECLARE @PricePiece money = -1;
  DECLARE @PriceHigh money = -1;
  DECLARE @PriceLow money = -1;
  
  SELECT @OrderGid = oi.Gid, @Currency = oi.Currency, @Location = oi.Location
    FROM OrderInformation oi
    WHERE oi.Gid = @OrderID
          AND oi.Deleted = 0;
  IF (@OrderGid IS NULL)
    SET @ResultFlag = 1;                          -- 订单号不存在
  
  IF (@ResultFlag = 0)
  BEGIN
    SELECT TOP (1) @ShipID = ss.ShipID
      FROM (
        SELECT os.ShipID, os.ShipWeight,
               OutStatus = CASE os.Ostatus WHEN 3 THEN 0 ELSE os.Ostatus END
          FROM OrderShipping os
          WHERE os.Deleted = 0
                AND os.Ostatus <> 2               -- 物流审单通过或不需要审单
                AND os.OrderID = @OrderGid
        ) ss
      ORDER BY ss.OutStatus DESC, ss.ShipWeight DESC;
    SELECT TOP (1)
           @PriceWeight = dbo.fn_FindResourceMoney(sa.PriceWeight, @Currency),
           @PriceVolume = dbo.fn_FindResourceMoney(sa.PriceVolume, @Currency),
           @PricePiece = dbo.fn_FindResourceMoney(sa.PricePiece, @Currency),
           @PriceHigh = dbo.fn_FindResourceMoney(sa.PriceHigh, @Currency),
           @PriceLow = dbo.fn_FindResourceMoney(sa.PriceLow, @Currency)
      FROM ShippingArea sa
      JOIN fn_FindFullRegions(@Location) fr ON sa.RegionID = fr.Gid
      WHERE sa.ShipID = @ShipID;
    -- 如果找不到记录，表示无法送达
    IF (@PriceWeight + @PriceVolume + @PricePiece) <> -3
    BEGIN
      SELECT @ShippingFee = SUM(s.ShippingFee)
        FROM (
          SELECT oit.OnSkuID,
                 ShippingFee = CASE ps.Solution
                   WHEN 0 THEN @PriceWeight * pii.GrossWeight * oit.Quantity * ps.Discount
                   WHEN 1 THEN @PriceVolume * pii.GrossVolume * oit.Quantity * ps.Discount
                   WHEN 2 THEN @PricePiece * pii.NetPiece * oit.Quantity * ps.Discount
                 END
            FROM OrderItem oit
            LEFT JOIN ProductOnItem poi ON oit.OnSkuID = poi.Gid
            LEFT JOIN ProductOnSale pos ON poi.OnSaleID = pos.Gid
            LEFT JOIN ProductOnShipping ps ON ps.OnSaleID = pos.Gid AND ps.ShipID = @ShipID
            LEFT JOIN ProductInfoItem pii ON poi.SkuID = pii.Gid
            WHERE oit.OrderID = @OrderGid
                  AND oit.Deleted = 0 ) s;
    END;
  END;
  RETURN (@ShippingFee);
END;
GO

CREATE FUNCTION fn_FindFullPrograms (@UserID uniqueidentifier)
  RETURNS @FindKeys TABLE (Gid uniqueidentifier) AS
BEGIN
  -- 递归用户权限的所有程序,完整树结构
  DECLARE @ProgKeys TABLE (Gid uniqueidentifier);
  DECLARE @Program uniqueidentifier;
  
  DECLARE ProgramCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT DISTINCT mpi.RefID
      FROM MemberPrivItem mpi
      JOIN MemberPrivilege mp ON mpi.PrivID = mp.Gid
      WHERE mpi.Deleted = 0
            AND mp.Deleted = 0
            AND mp.UserID = @UserID
            AND mp.Ptype = 0           -- 程序
  OPEN ProgramCursor;
  FETCH NEXT FROM ProgramCursor INTO @Program;
  WHILE (@@FETCH_STATUS = 0)
  BEGIN
    WHILE (@Program IS NOT NULL)
    BEGIN
      INSERT @ProgKeys (Gid) VALUES (@Program);
      SELECT @Program = gp.Parent FROM GeneralProgram gp WHERE gp.Gid = @Program;
    END;
    FETCH NEXT FROM ProgramCursor INTO @Program;
  END;
  CLOSE ProgramCursor;
  DEALLOCATE ProgramCursor;
  INSERT @FindKeys SELECT DISTINCT pk.Gid FROM @ProgKeys pk;
  RETURN;
END;
GO

CREATE FUNCTION fn_FindFullCategories (@UserID uniqueidentifier)
  RETURNS @FindKeys TABLE (Gid uniqueidentifier) AS
BEGIN
  -- 递归用户权限的所有私有分类,完整树结构
  DECLARE @CateKeys TABLE (Gid uniqueidentifier);
  DECLARE @Category uniqueidentifier;
  
  DECLARE CategoryCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT DISTINCT mpi.RefID
      FROM MemberPrivItem mpi
      JOIN MemberPrivilege mp ON mpi.PrivID = mp.Gid
      WHERE mpi.Deleted = 0
            AND mp.Deleted = 0
            AND mp.UserID = @UserID
            AND mp.Ptype = 5           -- 商品私有分类
  OPEN CategoryCursor;
  FETCH NEXT FROM CategoryCursor INTO @Category;
  WHILE (@@FETCH_STATUS = 0)
  BEGIN
    WHILE (@Category IS NOT NULL)
    BEGIN
      INSERT @CateKeys (Gid) VALUES (@Category);
      SELECT @Category = pc.Parent FROM GeneralPrivateCategory pc WHERE pc.Gid = @Category;
    END;
    FETCH NEXT FROM CategoryCursor INTO @Category;
  END;
  CLOSE CategoryCursor;
  DEALLOCATE CategoryCursor;
  INSERT @FindKeys SELECT DISTINCT ck.Gid FROM @CateKeys ck;
  RETURN;
END;
GO

CREATE FUNCTION fn_FindOnSkuID (@SkuID uniqueidentifier, @Channel uniqueidentifier)
  RETURNS uniqueidentifier AS
BEGIN
  -- 查询某个SKU上架后的OnSku值，最新且有效的值
  DECLARE @OnSkuID uniqueidentifier;
  SELECT TOP (1) @OnSkuID = poi.Gid
    FROM ProductOnItem poi
    JOIN ProductOnSale pos ON poi.OnSaleID = pos.Gid
    WHERE pos.ChlID = @Channel
          AND pos.Ostatus = 1                     -- 已上架状态
          AND poi.SkuID = @SkuID
    ORDER BY pos.CreateTime DESC;
  RETURN (@OnSkuID);
END;
GO

CREATE FUNCTION fn_FixLengthRand (@Length int)
  RETURNS int AS
BEGIN
  -- 产生固定长度的伪随机数，可能会重复
  DECLARE @RandNumber int;
  DECLARE @LengthKey int;
  SET @LengthKey = '1' + REPLICATE('0', @Length)
  
  SELECT @RandNumber = @LengthKey * r.random FROM viewRandom r;
  IF (@RandNumber < @LengthKey/10)
    SET @RandNumber = @RandNumber + @LengthKey/10;
  RETURN (@RandNumber);
END;
GO


-- 创建存储过程
CREATE PROCEDURE sp_InventoryByWarehouseSku (
  @WhID uniqueidentifier,
  @SkuID uniqueidentifier
) AS
BEGIN
  -- 按仓库和SKU统计实际入库数和出库数，更新总账和货架库存等
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @LedgerGid uniqueidentifier;
  DECLARE @InQty decimal (18,4);
  DECLARE @OutQty decimal (18,4);
  DECLARE @TobeDelivery decimal (18,4);
  DECLARE @Arranged decimal (18,4);
  DECLARE @Ontheway decimal (18,4);
  
  DECLARE @CostResource uniqueidentifier;
  DECLARE @DefaultCurrency uniqueidentifier;
  DECLARE @DefaultAverage money;
  DECLARE @Currency uniqueidentifier;
  DECLARE @AverageCost money;
  
  DECLARE @ShelfSGid uniqueidentifier;
  DECLARE @ShelfID uniqueidentifier;
  DECLARE @ShelfQty decimal (18,4);
  DECLARE @LockQty decimal (18,4);
  DECLARE @TrackLot int;
  DECLARE @ZeroQty decimal (18,4);
  
  -- 总账库存数
  -- 入库数
  SELECT @InQty = SUM(sii.Quantity)
    FROM WarehouseStockIn wsi
    JOIN WarehouseInItem sii ON wsi.Gid = sii.InID AND sii.SkuID = @SkuID
    WHERE wsi.WhID = @WhID
          AND wsi.Deleted = 0
          AND sii.Deleted = 0
          AND wsi.Istatus = 1;                    -- 已确认状态
  IF (@InQty IS NULL)
    SET @InQty = 0;
  
  -- 出库数
  SELECT @OutQty = SUM(soi.Quantity)
    FROM WarehouseStockOut wso
    JOIN WarehouseOutItem soi ON wso.Gid = soi.OutID AND soi.SkuID = @SkuID
    WHERE wso.WhID = @WhID
          AND wso.Deleted = 0
          AND soi.Deleted = 0
          AND wso.Ostatus IN (2,3);               -- 已发货或已签收
  IF (@OutQty IS NULL)
    SET @OutQty = 0;
  
  -- D已留货，未发货；所有有效订单，包括已付款和已确认，发货后清零
  SELECT @TobeDelivery = SUM(oit.Quantity)
    FROM OrderInformation oi
    JOIN OrderItem oit ON oi.Gid = oit.OrderID AND oit.SkuID = @SkuID
    WHERE oi.WhID = @WhID
          AND oi.Deleted = 0
          AND oit.Deleted = 0
          AND (oi.Ostatus = 1 OR oi.PayStatus = 3)    -- 已确认或已付款
  IF (@TobeDelivery IS NULL)
    SET @TobeDelivery = 0;
  
  -- E已排单，未发货；所有已排单订单，发货后清零
  -- 修改方案为，从出库单中统计为确认的数据
  -- SELECT @Arranged = SUM(oit.Quantity)
  --   FROM OrderInformation oi
  --   JOIN OrderItem oit ON oi.Gid = oit.OrderID AND oit.SkuID = @SkuID
  --   WHERE oi.WhID = @WhID
  --         AND oi.Deleted = 0
  --         AND oit.Deleted = 0
  --         AND oi.Ostatus = 2;                       -- 已排单（已付款或未付款）
  SELECT @Arranged = SUM(soi.Quantity)
    FROM WarehouseStockOut wso
    JOIN WarehouseOutItem soi ON wso.Gid = soi.OutID AND soi.SkuID = @SkuID
    WHERE wso.WhID = @WhID
          AND wso.Deleted = 0
          AND soi.Deleted = 0
          AND wso.Ostatus IN (0,1);               -- 未确认，拣货中/扫描中
  IF (@Arranged IS NULL)
    SET @Arranged = 0;
  
  -- 在途库存数，采购单，且未入库
  SELECT @Ontheway = SUM(pit.Quantity - pit.InQty)
    FROM PurchaseInformation pr
    JOIN PurchaseItem pit ON pr.Gid = pit.PurID AND pit.SkuID = @SkuID
    WHERE pr.WhID = @WhID
          AND pr.Deleted = 0
          AND pit.Deleted = 0
          AND pr.Pstatus = 1;                     -- 已确认（未确认和已结算不计入在途）
  IF (@Ontheway IS NULL)
    SET @Ontheway = 0;
  
  -- 平均成本，按采购货币分列
  SET @DefaultCurrency = dbo.fn_FindDefaultCurrency(@WhID);  -- 组织的默认货币
  SELECT @DefaultAverage = (SUM(pit.Amount) / SUM(pit.Quantity))
    FROM PurchaseInformation pu
    JOIN PurchaseItem pit ON pu.Gid = pit.PurID AND pit.SkuID = @SkuID
    WHERE pu.WhID = @WhID
          AND pu.Deleted = 0
          AND pit.Deleted = 0
          AND pu.Pstatus IN (1,2)                 -- 已确认或已结算
          AND pu.Currency = @DefaultCurrency;     -- 默认货币
  DECLARE AvgCostCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT pu.Currency, SUM(pit.Amount) / SUM(pit.Quantity) AS AverageCost
      FROM PurchaseInformation pu
      INNER JOIN PurchaseItem pit ON pu.Gid = pit.PurID AND pit.SkuID = @SkuID
      WHERE pu.WhID = @WhID
            AND pu.Deleted = 0
            AND pit.Deleted = 0
            AND pu.Pstatus IN (1,2)               -- 已确认或已结算
      GROUP BY pu.Currency;                       -- 其他货币
  
  -- 保存库存总账
  DECLARE @IsDefault bit;
  SELECT @LedgerGid = Gid, @CostResource = AvgCost FROM WarehouseLedger WHERE WhID = @WhID AND SkuID = @SkuID;
  IF (@LedgerGid IS NULL)
  BEGIN
    SET @IsDefault = 1;
    IF (@DefaultAverage IS NULL)                  -- 没有默认货币的成本值
    BEGIN
      OPEN AvgCostCursor;
      FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      WHILE (@@FETCH_STATUS = 0)
      BEGIN
        IF (@IsDefault = 1)                       -- 使用第一个成本值作为默认货币的成本
        BEGIN
          INSERT GeneralResource (Rtype, Currency, Cash) OUTPUT inserted.Gid INTO @GeneratedKeys VALUES (1, @Currency, @AverageCost);
          SELECT @CostResource = g.Gid FROM @GeneratedKeys g;
          SET @IsDefault = 0;
        END;
        ELSE
        BEGIN
          INSERT GeneralResItem (ResID, Currency, Cash) VALUES (@CostResource, @Currency, @AverageCost);
        END;
        FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      END;
    END;
    ELSE
    BEGIN
      INSERT GeneralResource (Rtype, Currency, Cash) OUTPUT inserted.Gid INTO @GeneratedKeys VALUES (1, @DefaultCurrency, @DefaultAverage);
      SELECT @CostResource = g.Gid FROM @GeneratedKeys g;
      OPEN AvgCostCursor;
      FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      WHILE (@@FETCH_STATUS = 0)
      BEGIN
        IF (@Currency <> @DefaultCurrency)
          INSERT GeneralResItem (ResID, Currency, Cash) VALUES (@CostResource, @Currency, @AverageCost);
        FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      END;
    END;
    INSERT WarehouseLedger (WhID, SkuID, InQty, OutQty, TobeDelivery, Arranged, Ontheway, AvgCost)
      VALUES (@WhID, @SkuID, @InQty, @OutQty, @TobeDelivery, @Arranged, @Ontheway, @CostResource);
  END;
  ELSE
  BEGIN
    SET @IsDefault = 1;
    IF (@DefaultAverage IS NULL)                  -- 没有默认货币的成本值
    BEGIN
      OPEN AvgCostCursor;
      FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      WHILE (@@FETCH_STATUS = 0)
      BEGIN
        IF (@IsDefault = 1)                       -- 使用第一个成本值作为默认货币的成本
        BEGIN
          IF EXISTS (SELECT 1 FROM GeneralResource WHERE Gid = @CostResource)
            UPDATE GeneralResource SET Rtype = 1, Currency = @Currency, Cash = @AverageCost WHERE Gid = @CostResource;
          ELSE
          BEGIN
            INSERT GeneralResource (Rtype, Currency, Cash) OUTPUT inserted.Gid INTO @GeneratedKeys VALUES (1, @Currency, @AverageCost);
            SELECT @CostResource = g.Gid FROM @GeneratedKeys g;
          END;
          SET @IsDefault = 0;
        END;
        ELSE
        BEGIN
          IF EXISTS (SELECT 1 FROM GeneralResItem WHERE ResID = @CostResource AND Currency = @Currency)
            UPDATE GeneralResItem SET Cash = @AverageCost WHERE ResID = @CostResource AND Currency = @Currency;
          ELSE
            INSERT GeneralResItem (ResID, Currency, Cash) VALUES (@CostResource, @Currency, @AverageCost);
        END;
        FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      END;
    END;
    ELSE
    BEGIN
      IF EXISTS (SELECT 1 FROM GeneralResource WHERE Gid = @CostResource)
        UPDATE GeneralResource SET Rtype = 1, Currency = @DefaultCurrency, Cash = @DefaultAverage WHERE Gid = @CostResource;
      ELSE
      BEGIN
        INSERT GeneralResource (Rtype, Currency, Cash) OUTPUT inserted.Gid INTO @GeneratedKeys VALUES (1, @DefaultCurrency, @DefaultAverage);
        SELECT @CostResource = g.Gid FROM @GeneratedKeys g;
      END;
      OPEN AvgCostCursor;
      FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      WHILE (@@FETCH_STATUS = 0)
      BEGIN
        IF (@Currency <> @DefaultCurrency)
        BEGIN
          IF EXISTS (SELECT 1 FROM GeneralResItem WHERE ResID = @CostResource AND Currency = @Currency)
            UPDATE GeneralResItem SET Cash = @AverageCost WHERE ResID = @CostResource AND Currency = @Currency;
          ELSE
            INSERT GeneralResItem (ResID, Currency, Cash) VALUES (@CostResource, @Currency, @AverageCost);
        END;
        FETCH NEXT FROM AvgCostCursor INTO @Currency, @AverageCost;
      END;
    END;
    UPDATE WarehouseLedger
      SET InQty = @InQty,
          OutQty = @OutQty,
          TobeDelivery = @TobeDelivery,
          Arranged = @Arranged,
          Ontheway = @Ontheway,
          AvgCost = @CostResource
      WHERE Gid = @LedgerGid;
  END;
  CLOSE AvgCostCursor;
  DEALLOCATE AvgCostCursor;
  
  -- 计算货架库存数
  SET @ZeroQty = 0;
  DECLARE ShelfSkuCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT s.ShelfID, s.TrackLot, SUM(s.ShelfInQty) - SUM(s.ShelfOutQty) + SUM(s.MoveIn) - SUM(s.MoveOut) AS ShelfQty, SUM(s.ShelfLockQty) AS LockQty
      FROM (
          SELECT sii.ShelfID, sii.TrackLot, SUM(sii.Quantity) AS ShelfInQty, @ZeroQty AS ShelfOutQty, @ZeroQty AS ShelfLockQty, @ZeroQty AS MoveIn, @ZeroQty AS MoveOut
            FROM WarehouseStockIn si
            JOIN WarehouseInItem sii ON si.Gid = sii.InID AND sii.SkuID = @SkuID
            WHERE si.WhID = @WhID
                  AND si.Deleted = 0
                  AND sii.Deleted = 0
                  AND si.Istatus = 1              -- 入库单：已确认状态
            GROUP BY sii.ShelfID, sii.TrackLot
          UNION ALL
          SELECT soi.ShelfID, soi.TrackLot, @ZeroQty AS ShelfInQty, SUM(soi.Quantity) AS ShelfOutQty, @ZeroQty AS ShelfLockQty, @ZeroQty AS MoveIn, @ZeroQty AS MoveOut
            FROM WarehouseStockOut so
            JOIN WarehouseOutItem soi ON so.Gid = soi.OutID AND soi.SkuID = @SkuID
            WHERE so.WhID = @WhID
                  AND so.Deleted = 0
                  AND soi.Deleted = 0
                  AND so.Ostatus IN (2,3)         -- 出库单：已发货或已签收
            GROUP BY soi.ShelfID, soi.TrackLot
          UNION ALL
          SELECT soi.ShelfID, soi.TrackLot, @ZeroQty AS ShelfInQty, @ZeroQty AS ShelfOutQty, SUM(soi.Quantity) AS ShelfLockQty, @ZeroQty AS MoveIn, @ZeroQty AS MoveOut
            FROM WarehouseStockOut so
            INNER JOIN WarehouseOutItem soi ON so.Gid = soi.OutID AND soi.SkuID = @SkuID
            WHERE so.WhID = @WhID
                  AND so.Deleted = 0
                  AND soi.Deleted = 0
                  AND so.Ostatus IN (0,1)         -- 出库单：未确认或拣货/扫描中
            GROUP BY soi.ShelfID, soi.TrackLot
          UNION ALL
          SELECT wmi.NewShelf AS ShelfID, wmi.TrackLot, @ZeroQty AS ShelfInQty, @ZeroQty AS ShelfOutQty, @ZeroQty AS ShelfLockQty, SUM(wmi.Quantity) AS MoveIn, @ZeroQty AS MoveOut
            FROM WarehouseMoving wm
            JOIN WarehouseMoveItem wmi ON wm.Gid = wmi.MoveID AND wmi.SkuID = @SkuID
            WHERE wm.NewWhID = @WhID              -- 移入货位
                  AND wm.Deleted = 0
                  AND wmi.Deleted = 0
                  AND wm.Mstatus = 1              -- 移货位单已确认
                  AND wm.Mtype = 1                -- 移货位
            GROUP BY wmi.NewShelf, wmi.TrackLot
          UNION ALL
          SELECT wmi.OldShelf AS ShelfID, wmi.TrackLot, @ZeroQty AS ShelfInQty, @ZeroQty AS ShelfOutQty, @ZeroQty AS ShelfLockQty, @ZeroQty AS MoveIn, SUM(wmi.Quantity) AS MoveOut
            FROM WarehouseMoving wm
            JOIN WarehouseMoveItem wmi ON wm.Gid = wmi.MoveID AND wmi.SkuID = @SkuID
            WHERE wm.OldWhID = @WhID              -- 移出货位
                  AND wm.Deleted = 0
                  AND wmi.Deleted = 0
                  AND wm.Mstatus = 1              -- 移货位单已确认
                  AND wm.Mtype = 1                -- 移货位
            GROUP BY wmi.OldShelf, wmi.TrackLot ) s
      GROUP BY s.ShelfID, s.TrackLot;
  
  -- 清除原有记录，以便更新
  UPDATE WarehouseSkuShelf
    SET Deleted = 1,
        Quantity = 0,
        LockQty = 0
    WHERE WhID = @WhID
          AND SkuID = @SkuID;
  
  -- 保存
  OPEN ShelfSkuCursor;
  FETCH NEXT FROM ShelfSkuCursor INTO @ShelfID, @TrackLot, @ShelfQty, @LockQty;
  WHILE (@@FETCH_STATUS = 0)
  BEGIN
    SELECT @ShelfSGid = Gid
      FROM WarehouseSkuShelf
      WHERE WhID = @WhID
            AND SkuID = @SkuID
            AND ShelfID = @ShelfID
            AND TrackLot = @TrackLot;
    IF (@ShelfSGid IS NULL)
      INSERT WarehouseSkuShelf (WhID, SkuID, ShelfID, TrackLot, Quantity, LockQty)
        VALUES (@WhID, @SkuID, @ShelfID, @TrackLot, @ShelfQty, @LockQty);
    ELSE
      UPDATE WarehouseSkuShelf
        SET Deleted = 0,
            Quantity = @ShelfQty,
            LockQty = @LockQty
        WHERE Gid = @ShelfSGid;
    FETCH NEXT FROM ShelfSkuCursor INTO @ShelfID, @TrackLot, @ShelfQty, @LockQty;
  END;
  CLOSE ShelfSkuCursor;
  DEALLOCATE ShelfSkuCursor;
  
  COMMIT TRANSACTION;
  SELECT 1;    -- 返回值
END;
GO

CREATE PROCEDURE sp_UpdatePurchaseInQty (@PurID uniqueidentifier) AS
BEGIN
  -- 入库单确认后，更新采购单的已入库数量
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @SkuID uniqueidentifier;
  DECLARE @InQty decimal (18,4);
  
  -- 采购单的实际已入库数
  DECLARE PurchaseInCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT sii.SkuID, SUM(sii.Quantity) AS InQty
      FROM WarehouseStockIn si
      JOIN WarehouseInItem sii ON si.Gid = sii.InID
      WHERE si.RefType = 1                          -- 采购单入库
            AND si.RefID = @PurID
            AND si.Deleted = 0
            AND sii.Deleted = 0
            AND si.Istatus = 1                      -- 已确认状态
      GROUP BY sii.SkuID;
  OPEN PurchaseInCursor;
  FETCH NEXT FROM PurchaseInCursor INTO @SkuID, @InQty;
  WHILE (@@FETCH_STATUS = 0)
  BEGIN
    UPDATE PurchaseItem
      SET InQty = @InQty
      WHERE PurID = @PurID
            AND SkuID = @SkuID;
    FETCH NEXT FROM PurchaseInCursor INTO @SkuID, @InQty;
  END;
  CLOSE PurchaseInCursor;
  DEALLOCATE PurchaseInCursor;
  
  COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE sp_GenerateStockInFromPurchase (
  @PurID uniqueidentifier,
  @PreparedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 由采购单生成入库单，成功后返回入库单
  -- 返回值 0成功 1采购单状态不正确
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  DECLARE @PurchaseGid uniqueidentifier;
  DECLARE @StockInGid uniqueidentifier;
  DECLARE @StockInType uniqueidentifier;
  DECLARE @BufferShelf uniqueidentifier;
  DECLARE @WhID uniqueidentifier;
  DECLARE @TrackLot int;
  DECLARE @SkuID uniqueidentifier;
  DECLARE @InQty decimal (18,4);
  DECLARE @Guarantee datetimeoffset (0);
  
  -- 更新已入库数量
  EXECUTE sp_UpdatePurchaseInQty @PurID;
  
  -- 查询待入库数量
  SELECT @PurchaseGid = pu.Gid, @WhID = pu.WhID, @TrackLot = pu.TrackLot
    FROM PurchaseInformation pu
    WHERE pu.Deleted = 0
          AND pu.Gid = @PurID
          AND pu.Deleted = 0
          AND pu.Pstatus = 1;                     -- 已确认状态（未确认和已结算不能生成入库单）
  IF (@PurchaseGid IS NULL)
    SET @ResultFlag = 1;                          -- 采购单状态不正确，返回错误代码1
  
  -- 状态合理，开始生成入库单
  IF (@ResultFlag = 0)
  BEGIN
    SELECT @StockInType = sc.Gid
      FROM GeneralStandardCategory sc
      WHERE sc.Ctype = 4
            AND sc.Code = 'PurchaseIn';           -- 采购入库（大货）类型
    SELECT @BufferShelf = ws.Gid
      FROM WarehouseShelf ws
      WHERE ws.WhID = @WhID
            AND ws.Code = 'BUFFER'                -- 虚拟的入库缓冲区，临时存放区
    -- 入库单主表
    INSERT WarehouseStockIn (WhID, Istatus, InType, RefType, RefID, Prepared, LastModifyTime)
      OUTPUT inserted.Gid INTO @GeneratedKeys
      VALUES (@WhID, 0, @StockInType, 1, @PurID, @PreparedBy, sysdatetimeoffset());
    SELECT @StockInGid = g.Gid FROM @GeneratedKeys g;
    -- 入库单明细
    DECLARE InItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT pui.SkuID, (pui.Quantity - pui.InQty) AS InQty, pui.Guarantee
        FROM PurchaseItem pui
        WHERE pui.Deleted = 0
              AND pui.PurID = @PurchaseGid;
    OPEN InItemCursor;
    FETCH NEXT FROM InItemCursor INTO @SkuID, @InQty, @Guarantee;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      INSERT WarehouseInItem (InID, SkuID, ShelfID, TrackLot, Quantity, Guarantee)
        VALUES (@StockInGid, @SkuID, @BufferShelf, @TrackLot, @InQty, @Guarantee);
      FETCH NEXT FROM InItemCursor INTO @SkuID, @InQty, @Guarantee;
    END;
    CLOSE InItemCursor;
    DEALLOCATE InItemCursor;
  END;
  
  IF @ResultFlag = 0
  BEGIN
    COMMIT TRANSACTION;
    SELECT CAST(@StockInGid AS NVARCHAR(50));     -- 成功返回值，入库单Gid
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    SELECT CAST(@ResultFlag AS NVARCHAR(50));
  END;
END;
GO

CREATE PROCEDURE sp_StockInConfirm (
  @InID uniqueidentifier,
  @ApprovedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 入库单确认，增加库存数量。返回代码0成功 1入库单不存在或状态不正确 2入库单货位尚未明确 3没有明细数据
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  DECLARE @HasDetails bit = 0;
  DECLARE @StockInGid uniqueidentifier;
  DECLARE @WhID uniqueidentifier;
  DECLARE @RefType tinyint;
  DECLARE @RefID uniqueidentifier;
  
  DECLARE @SkuID uniqueidentifier;
  DECLARE @ShelfID uniqueidentifier;
  DECLARE @TrackLot int;
  DECLARE @Quantity decimal (18, 4);
  DECLARE @Guarantee datetimeoffset (0);
  
  -- 查询入库单，一个入库单只能确认一次，不可返回
  SELECT @StockInGid = wsi.Gid, @WhID = wsi.WhID, @RefType = wsi.RefType, @RefID = wsi.RefID
    FROM WarehouseStockIn wsi
    WHERE wsi.Deleted = 0
          AND wsi.Istatus = 0                     -- 未确认状态
          AND wsi.Gid = @InID;
  IF (@StockInGid IS NULL)
    SET @ResultFlag = 1;                          -- 入库单不存在或状态不正确，返回错误代码1
  ELSE IF EXISTS (                                -- BUFFER中的不能确认，但DEFECT中仍可确认入库
    SELECT wii.Gid
      FROM WarehouseInItem wii
      JOIN WarehouseShelf ws ON wii.ShelfID = ws.Gid AND ws.WhID = @WhID AND ws.Code = 'BUFFER'
      WHERE wii.InID = @StockInGid
            AND wii.Deleted = 0 )
    SET @ResultFlag = 2;                          -- 入库单货位尚未明确，即存在货位为Buffer，返回错误代码2
  
  -- 状态合理，开始确认入库
  IF (@ResultFlag = 0)
  BEGIN
    DECLARE InItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT wii.SkuID, wii.ShelfID, wii.TrackLot, wii.Quantity, wii.Guarantee
        FROM WarehouseInItem wii
        WHERE wii.InID = @StockInGid
              AND wii.Deleted = 0;
    OPEN InItemCursor;
    FETCH NEXT FROM InItemCursor INTO @SkuID, @ShelfID, @TrackLot, @Quantity, @Guarantee;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      SET @HasDetails = 1;
      -- 更新WarehouseSkuShelf货架帐，未考虑到删除和恢复的情况，是否需要？
      IF EXISTS (SELECT 1 FROM WarehouseSkuShelf wss WHERE wss.SkuID = @SkuID AND wss.ShelfID = @ShelfID AND wss.TrackLot = @TrackLot)
        UPDATE WarehouseSkuShelf
          SET Deleted = 0,
              Quantity = Quantity + @Quantity,
              Guarantee = @Guarantee
          WHERE SkuID = @SkuID
                AND ShelfID = @ShelfID
                AND TrackLot = @TrackLot;
      ELSE
        INSERT WarehouseSkuShelf (WhID, SkuID, ShelfID, TrackLot, Quantity, Guarantee)
          VALUES (@WhID, @SkuID, @ShelfID, @TrackLot, @Quantity, @Guarantee);
      -- 更新WarhouseLedger库存总账，未考虑到删除和恢复的情况，是否需要？
      IF EXISTS (SELECT 1 FROM WarehouseLedger wl WHERE wl.WhID = @WhID AND wl.SkuID = @SkuID )
        UPDATE WarehouseLedger
          SET Deleted = 0,
              InQty = InQty + @Quantity
          WHERE WhID = @WhID
                AND SkuID = @SkuID;
      ELSE
        INSERT WarehouseLedger (WhID, SkuID, InQty)
          VALUES (@WhID, @SkuID, @Quantity);
      -- 更新采购单已入库数量，未考虑到删除和恢复的情况，是否需要？
      IF @RefType = 1                             -- 采购单类型
        UPDATE PurchaseItem
          SET InQty = InQty + @Quantity
          WHERE PurID = @RefID
                AND SkuID = @SkuID;
      FETCH NEXT FROM InItemCursor INTO @SkuID, @ShelfID, @TrackLot, @Quantity, @Guarantee;
    END;
    CLOSE InItemCursor;
    DEALLOCATE InItemCursor;
    -- 更新入库单状态为已确认
    IF (@HasDetails = 1)
    BEGIN
      UPDATE WarehouseStockIn
        SET Istatus = 1,                            -- 已确认
            Approved = @ApprovedBy,
            ApproveTime = sysdatetimeoffset()
        WHERE Gid = @StockInGid;
    END
    ELSE
    BEGIN
      SET @ResultFlag = 1;                          -- 入库单没有明细数据，返回错误代码3
    END;
  END;
  
  IF @ResultFlag = 0
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_StockInDiscard (
  @InID uniqueidentifier,
  @ApprovedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 入库单作废，不变库存；只有未确认的入库单才能作废。返回代码0成功，1入库单状态不正确
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @ResultFlag int = 0;
  DECLARE @InGid uniqueidentifier;
  
  -- 查询入库单，作废后不可返回
  SELECT @InGid = wsi.Gid
    FROM WarehouseStockIn wsi
    WHERE wsi.Deleted = 0
          AND wsi.Istatus = 0                     -- 未确认状态
          AND wsi.Gid = @InID;
  IF (@InGid IS NULL)
    SET @ResultFlag = 1;                          -- 入库单状态不正确，不能作废，返回错误代码1
  
  -- 状态合理，开始处理作废
  IF (@ResultFlag = 0)
  BEGIN
    UPDATE WarehouseStockIn
      SET Deleted = 1,                            -- 删除，作废
          LastModifiedBy = @ApprovedBy,
          LastModifyTime = SYSDATETIMEOFFSET(),
          Approved = @ApprovedBy,
          ApproveTime = SYSDATETIMEOFFSET()
      WHERE Gid = @InGid;
    UPDATE WarehouseInItem
      SET Deleted = 1,
          LastModifiedBy = @ApprovedBy,
          LastModifyTime = SYSDATETIMEOFFSET()
      WHERE InID = @InGid;
  END;
  
  IF @ResultFlag = 0
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_GenerateStockOutFromOrder (
  @OrderID uniqueidentifier,
  @PreparedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 排单 由订单生成出库单，成功后返回出库单Gid；
  -- 返回值0成功，1出库单已经存在，2订单状态不对，3库存不足
  -- 生成出库单后，仓库发生变化，出库前在程序页面可以重新生成出库单，即作废后再生成新出库单号？
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  DECLARE @OrderGid uniqueidentifier;
  DECLARE @WhID uniqueidentifier;
  DECLARE @Total decimal (18, 4);
  DECLARE @ShipID uniqueidentifier;
  DECLARE @StockOutGid uniqueidentifier;
  DECLARE @StockOutType uniqueidentifier;
  
  DECLARE @SkuID uniqueidentifier;
  DECLARE @Quantity decimal (18, 4);
  DECLARE @SkuShelfGid uniqueidentifier;
  DECLARE @ShelfID uniqueidentifier;
  DECLARE @TrackLot int;
  DECLARE @ShelfQty decimal (18, 4);
  DECLARE @OutQty decimal (18, 4);
  
  -- 查询是否已经生成出库单，一个订单只能生成一个出库单，但可作废后重新生成
  SELECT @StockOutGid = wso.Gid
    FROM WarehouseStockOut wso
    WHERE wso.Deleted = 0
          AND wso.RefType = 0                     -- 订单类型
          AND wso.RefID = @OrderID;
  -- 检测订单状态
  SELECT @OrderGid = oi.Gid, @WhID = oi.WhID, @Total = oi.Pieces
    FROM OrderInformation oi
    WHERE oi.Deleted = 0
          AND oi.Gid = @OrderID
          AND oi.Ostatus = 1                      -- 订单已确认
          AND oi.Locking = 1                      -- 已锁定
          AND oi.Hanged = 0                       -- 未挂起
          AND (oi.TransType <> 0 OR oi.PayStatus = 3); -- 款到发货，则必须已付款
  IF (@StockOutGid IS NOT NULL)
    SET @ResultFlag = 1;                          -- 出库单已经存在，返回错误代码1
  ELSE IF (@OrderGid IS NULL)
    SET @ResultFlag = 2;                          -- 订单状态不正确，返回错误代码2
  
  -- 状态合理，开始生成出库单
  IF (@ResultFlag = 0)
  BEGIN
    SELECT @StockOutType = sc.Gid
      FROM GeneralStandardCategory sc
      WHERE sc.Ctype = 5
            AND sc.Code = 'Sale';                 -- 销售出库类型
    SELECT TOP (1) @ShipID = ss.ShipID
      FROM (
        SELECT os.ShipID, os.ShipWeight,
               OutStatus = CASE os.Ostatus WHEN 3 THEN 0 ELSE os.Ostatus END
          FROM OrderShipping os
          WHERE os.Deleted = 0
                AND os.Ostatus <> 2               -- 物流审单通过或不需要审单
                AND os.OrderID = @OrderGid
        ) ss
      ORDER BY ss.OutStatus DESC, ss.ShipWeight DESC;
    -- 生成出库单主表
    INSERT WarehouseStockOut (WhID, Ostatus, OutType, RefType, RefID, ShipID, Total, Prepared, LastModifyTime)
      OUTPUT inserted.Gid INTO @GeneratedKeys
      VALUES (@WhID, 0, @StockOutType, 0, @OrderGid, @ShipID, @Total, @PreparedBy, sysdatetimeoffset());
    SELECT @StockOutGid = g.Gid FROM @GeneratedKeys g;
    
    -- 订单详情，生成出库单明细
    DECLARE OrderItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT oit.SkuID, oit.Quantity
        FROM OrderItem oit
        WHERE oit.Deleted = 0
              AND oit.OrderID = @OrderGid;
    OPEN OrderItemCursor;
    FETCH NEXT FROM OrderItemCursor INTO @SkuID, @Quantity;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      -- 查询货架库存数
      DECLARE ShelfFifoCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
        SELECT wss.Gid, wss.ShelfID, wss.TrackLot, wss.Quantity - wss.LockQty AS ShelfQty
          FROM WarehouseSkuShelf wss
          JOIN WarehouseShelf ws ON wss.ShelfID = ws.Gid AND ws.WhID = @WhID AND ws.Reserved = 0
          WHERE wss.WhID = @WhID
                AND wss.SkuID = @SkuID
                AND wss.Quantity > wss.LockQty
          ORDER BY wss.TrackLot ASC, wss.ShelfID ASC;   -- 按批次号先进先出，一个货架可能不够数量，则多个货架
      OPEN ShelfFifoCursor;
      FETCH NEXT FROM ShelfFifoCursor INTO @SkuShelfGid, @ShelfID, @TrackLot, @ShelfQty;
      WHILE (@@FETCH_STATUS = 0)
      BEGIN
        IF @ShelfQty >= @Quantity                 -- 货架上的数量足够
          SET @OutQty = @Quantity;
        ELSE
          SET @OutQty = @ShelfQty;
        -- 插入出库单明细，同时设置锁定数量，即生成了出库单，但还没发货状态之间
        INSERT WarehouseOutItem (OutID, SkuID, ShelfID, TrackLot, Quantity)
          VALUES (@StockOutGid, @SkuID, @ShelfID, @TrackLot, @OutQty);
        -- 更新货架占用库存
        UPDATE WarehouseSkuShelf
          SET LockQty = LockQty + @OutQty
          WHERE Gid = @SkuShelfGid;
        -- 更新总账已排单数量，货架有数量，则总账肯定有总账数量
        UPDATE WarehouseLedger
          SET Arranged = Arranged + @Quantity
          WHERE WhID = @WhID
                AND SkuID = @SkuID;
        SET @Quantity = @Quantity - @OutQty;
        IF @Quantity <= 0
          BREAK;
        FETCH NEXT FROM ShelfFifoCursor INTO @SkuShelfGid, @ShelfID, @TrackLot, @ShelfQty;
      END;
      CLOSE ShelfFifoCursor;
      DEALLOCATE ShelfFifoCursor;
      IF @Quantity > 0
      BEGIN
        SET @ResultFlag = 3;                      -- 库存不足，排单失败，返回错误代码3
        BREAK;
      END;
      FETCH NEXT FROM OrderItemCursor INTO @SkuID, @Quantity;
    END;
    CLOSE OrderItemCursor;
    DEALLOCATE OrderItemCursor;
    IF (@ResultFlag = 0)
    BEGIN
      -- 排单成功，更新订单状态为已排单
      UPDATE OrderInformation
        SET Ostatus = 2,                          -- 已排单
            Locking = 0                           -- 解锁
        WHERE Gid = @OrderGid;
    END;
  END;
  
  IF (@ResultFlag = 0)
  BEGIN
    COMMIT TRANSACTION;
    SELECT CAST(@StockOutGid AS NVARCHAR(50));    -- 成功返回值，出库单Gid
  END
  ELSE
  BEGIN
    ROLLBACK TRANSACTION;
    SELECT CAST(@ResultFlag AS NVARCHAR(50));
  END;
END;
GO

CREATE PROCEDURE sp_StockOutConfirm (
  @OutID uniqueidentifier,
  @ApprovedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 出库单确认（即已发货），仅做确认和减库存动作，不检查出库扫描情况，不检查运单情况
  -- 减少库存数量
  -- 返回代码0成功，1出库单不存在或状态不正确  2订单状态不正确
  --         3出库单货位尚未明确，即存在货位为Buffer
  --         4货架不存在，无法确认发货
  --         5没有明细数据
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  DECLARE @HasDetails bit = 0;
  DECLARE @OutGid uniqueidentifier;
  DECLARE @WhID uniqueidentifier;
  DECLARE @ShipID uniqueidentifier;
  DECLARE @RefType tinyint;
  DECLARE @RefID uniqueidentifier;
  
  DECLARE @SkuID uniqueidentifier;
  DECLARE @ShelfID uniqueidentifier;
  DECLARE @TrackLot int;
  DECLARE @Quantity decimal (18, 4);
  
  -- 查询出库单，一个出库单只能确认一次，不可回退
  SELECT @OutGid = wso.Gid, @WhID = wso.WhID, @RefType = wso.RefType, @RefID = wso.RefID, @ShipID = wso.ShipID
    FROM WarehouseStockOut wso
    WHERE wso.Deleted = 0
          AND wso.Ostatus = 0                     -- 未确认状态
          AND wso.Gid = @OutID;
  IF (@OutGid IS NULL)
    SET @ResultFlag = 1;                          -- 出库单不存在或状态不正确，返回错误代码1
  ELSE IF (@RefType = 0) AND NOT EXISTS (         -- 订单类型
    SELECT oi.Gid
      FROM OrderInformation oi
      WHERE oi.Deleted = 0
            AND oi.Ostatus = 2                    -- 已排单
            AND oi.Gid = @RefID )
    SET @ResultFlag = 2;                          -- 订单状态不正确，不能出库，返回代码2
  ELSE IF EXISTS (                                -- BUFFER中的不能确认发货，但DEFECT中可以确认发货
    SELECT woi.Gid
      FROM WarehouseOutItem woi
      JOIN WarehouseShelf ws ON woi.ShelfID = ws.Gid AND ws.WhID = @WhID AND ws.Code = 'BUFFER'
      WHERE woi.Deleted = 0
            AND woi.OutID = @OutGid )
    SET @ResultFlag = 3;                          -- 出库单货位尚未明确，即存在货位为Buffer，返回错误代码3
  
  -- 状态合理，开始确认（发货）
  IF (@ResultFlag = 0)
  BEGIN
    DECLARE OutItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT woi.SkuID, woi.ShelfID, woi.TrackLot, woi.Quantity
        FROM WarehouseOutItem woi
        WHERE woi.Deleted = 0
              AND woi.OutID = @OutGid;
    OPEN OutItemCursor;
    FETCH NEXT FROM OutItemCursor INTO @SkuID, @ShelfID, @TrackLot, @Quantity;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      SET @HasDetails = 1;
      -- 更新WarehouseSkuShelf货架帐，未考虑到删除和恢复的情况，是否需要？
      IF EXISTS (
        SELECT wss.Gid
          FROM WarehouseSkuShelf wss
          WHERE wss.SkuID = @SkuID
                AND wss.ShelfID = @ShelfID
                AND wss.TrackLot = @TrackLot )
      BEGIN
        UPDATE WarehouseSkuShelf
          SET Quantity = Quantity - @Quantity,
              LockQty = LockQty - @Quantity       -- 释放排单占用数，错误，有些出库单没有占用数量？？？？
          WHERE SkuID = @SkuID
                AND ShelfID = @ShelfID
                AND TrackLot = @TrackLot;
      END
      ELSE
      BEGIN
        SET @ResultFlag = 4;                      -- 货架不存在，无法确认发货，返回错误代码4
        BREAK;
      END;
      -- 更新WarhouseLedger库存总账，未考虑到删除和恢复的情况，是否需要？
      -- 有货架数量，则肯定有总账数量
      UPDATE WarehouseLedger
        SET OutQty = OutQty + @Quantity,
            Arranged = Arranged - @Quantity       -- 释放已排单占用数量，错误，有些出库单没有占用数量？？？？
        WHERE WhID = @WhID
              AND SkuID = @SkuID;
      FETCH NEXT FROM OutItemCursor INTO @SkuID, @ShelfID, @TrackLot, @Quantity;
    END;
    CLOSE OutItemCursor;
    DEALLOCATE OutItemCursor;
    -- 更新订单状态为已发货，更新最终承运商
    IF (@HasDetails = 1)
    BEGIN
      IF (@RefType = 0)                             -- 订单类型
      BEGIN
        UPDATE OrderInformation
          SET Ostatus = 3                           -- 已发货
          WHERE Gid = @RefID;
        UPDATE OrderShipping
          SET Candidate = CASE ShipID WHEN @ShipID THEN 1 ELSE 0 END -- 设置最终承运商
          WHERE OrderID = @RefID;
      END;
      -- 更新出库单状态为已确认(已发货)
      UPDATE WarehouseStockOut
        SET Ostatus = 2,                            -- 已发货
            Approved = @ApprovedBy,
            ApproveTime = sysdatetimeoffset()
        WHERE Gid = @OutGid;
    END
    ELSE
    BEGIN
      SET @ResultFlag = 5;                          -- 没有明细数据，返回代码5
    END;
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_StockOutDiscard (
  @OutID uniqueidentifier,
  @ApprovedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 出库单作废，只有未确认的出库单才能作废，释放占用库存
  -- 返回代码0成功，1出库单不存在或状态不正确
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @ResultFlag int = 0;
  DECLARE @OutGid uniqueidentifier;
  
  DECLARE @WhID uniqueidentifier;
  DECLARE @RefType tinyint;
  DECLARE @RefID uniqueidentifier;
  
  DECLARE @SkuID uniqueidentifier;
  DECLARE @ShelfID uniqueidentifier;
  DECLARE @TrackLot int;
  DECLARE @Quantity decimal (18, 4);
  
  -- 查询出库单
  SELECT @OutGid = wso.Gid, @WhID = wso.WhID, @RefType = wso.RefType, @RefID = wso.RefID
    FROM WarehouseStockOut wso
    WHERE wso.Deleted = 0
          AND wso.Ostatus = 0                     -- 未确认状态
          AND wso.Gid = @OutID;
  IF (@OutGid IS NULL)
    SET @ResultFlag = 1;                          -- 出库单不存在或状态不正确，返回错误代码1
  
  -- 状态合理，开始处理作废
  IF (@ResultFlag = 0)
  BEGIN
    DECLARE OutItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT woi.SkuID, woi.ShelfID, woi.TrackLot, woi.Quantity
        FROM WarehouseOutItem woi
        WHERE woi.Deleted = 0
              AND woi.OutID = @OutGid;
    OPEN OutItemCursor;
    FETCH NEXT FROM OutItemCursor INTO @SkuID, @ShelfID, @TrackLot, @Quantity;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      -- 更新WarehouseSkuShelf货架帐，未考虑到删除和恢复的情况，是否需要？
      UPDATE WarehouseSkuShelf
        SET LockQty = LockQty - @Quantity         -- 释放排单占用数
        WHERE SkuID = @SkuID
              AND ShelfID = @ShelfID
              AND TrackLot = @TrackLot;
      -- 更新WarhouseLedger库存总账，未考虑到删除和恢复的情况，是否需要？
      UPDATE WarehouseLedger
        SET Arranged = Arranged - @Quantity       -- 释放已排单占用数量
        WHERE WhID = @WhID
              AND SkuID = @SkuID;
      FETCH NEXT FROM OutItemCursor INTO @SkuID, @ShelfID, @TrackLot, @Quantity;
    END;
    CLOSE OutItemCursor;
    DEALLOCATE OutItemCursor;
    -- 更新订单状态为已确认（回退）
    IF (@RefType = 0)                             -- 订单类型
    BEGIN
      UPDATE OrderInformation
        SET Ostatus = 1                           -- 已确认
        WHERE Gid = @RefID
              AND Ostatus = 2;                    -- 从已排单改为已确认
    END;
    -- 作废出库单
    UPDATE WarehouseStockOut
      SET Deleted = 1,                            -- 作废
          LastModifiedBy = @ApprovedBy,
          LastModifyTime = SYSDATETIMEOFFSET(),
          Approved = @ApprovedBy,
          ApproveTime = SYSDATETIMEOFFSET()
      WHERE Gid = @OutGid;
    UPDATE WarehouseOutItem
      SET Deleted = 1,
          LastModifiedBy = @ApprovedBy,
          LastModifyTime = SYSDATETIMEOFFSET()
      WHERE OutID = @OutGid;
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_MovingConfirm (
  @MoveID uniqueidentifier,
  @ApprovedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 移库单确认
  -- 返回代码0成功，1移库单不存在或状态不正确，2库存不足
  --         3移库单货位尚未明确，即存在货位为Buffer
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  DECLARE @MoveGid uniqueidentifier;
  DECLARE @MoveType tinyint;
  DECLARE @OldWhID uniqueidentifier;
  DECLARE @NewWhID uniqueidentifier;
  
  DECLARE @SkuID uniqueidentifier;
  DECLARE @OldShelf uniqueidentifier;
  DECLARE @NewShelf uniqueidentifier;
  DECLARE @TrackLot int;
  DECLARE @Quantity decimal (18, 4);
  DECLARE @Guarantee datetimeoffset (0);          -- 一并移入新库位
  
  DECLARE @ShipID uniqueidentifier;
  DECLARE @Total decimal (18, 4);
  DECLARE @BufferShelf uniqueidentifier;
  DECLARE @StockOutType uniqueidentifier;
  DECLARE @StockInType uniqueidentifier;
  DECLARE @StockOutGid uniqueidentifier;
  DECLARE @StockInGid uniqueidentifier;
  
  -- 查询移库单，一个移库单只能确认一次，不可回退
  SELECT @MoveGid = wm.Gid, @MoveType = wm.Mtype, @OldWhID = wm.OldWhID, @NewWhID = wm.NewWhID, @Total = Total, @ShipID = ShipID
    FROM WarehouseMoving wm
    WHERE wm.Deleted = 0
          AND wm.Mstatus = 0                      -- 未确认状态
          AND wm.Gid = @MoveID;
  IF (@MoveGid IS NULL)
    SET @ResultFlag = 1;                          -- 移库单不存在或状态不正确，返回错误代码1
  ELSE IF EXISTS (
    SELECT m.Gid
      FROM (
        SELECT wmi.Gid,
               wmi.Quantity AS MoveQty,
               CASE WHEN wss.Quantity IS NULL THEN 0 ELSE wss.Quantity END AS ShelfQty,
               CASE WHEN wss.LockQty IS NULL THEN 0 ELSE wss.LockQty END AS LockQty
          FROM WarehouseMoveItem wmi
          LEFT JOIN WarehouseSkuShelf wss ON wmi.SkuID = wss.SkuID AND wmi.OldShelf = wss.ShelfID AND wmi.TrackLot = wss.TrackLot
          WHERE wmi.Deleted = 0
                AND wmi.MoveID = @MoveGid ) m
        WHERE m.MoveQty > m.ShelfQty - m.LockQty ) -- 原库位库存不足
    SET @ResultFlag = 2;                          -- 原库位库存不足，返回错误代码2
  ELSE IF EXISTS (                                -- BUFFER中的不能确认移库，但DEFECT中可以确认移库
    SELECT wmi.Gid
      FROM WarehouseMoveItem wmi
      JOIN WarehouseShelf ws ON wmi.OldShelf = ws.Gid AND ws.WhID = @OldWhID AND ws.Code = 'BUFFER'
      WHERE wmi.Deleted = 0
            AND wmi.MoveID = @MoveGid )
    SET @ResultFlag = 3;                          -- 移库单货位尚未明确，即存在货位为Buffer，返回错误代码3
  
  -- 状态合理，开始确认（移库）
  IF (@ResultFlag = 0)
  BEGIN
    IF (@MoveType = 0)                            -- 移库
    BEGIN
      SELECT @StockOutType = sc.Gid
        FROM GeneralStandardCategory sc
        WHERE sc.Ctype = 5
              AND sc.Code = 'MoveOut';            -- 移出库类型
      SELECT @StockInType = sc.Gid
        FROM GeneralStandardCategory sc
        WHERE sc.Ctype = 4
              AND sc.Code = 'MoveIn';             -- 移入库类型
      SELECT @BufferShelf = ws.Gid
        FROM WarehouseShelf ws
        WHERE ws.WhID = @NewWhID
              AND ws.Code = 'BUFFER'              -- 虚拟的入库缓冲区，临时存放区
      -- 生成出库单，并立即确认，且减原库存
      INSERT WarehouseStockOut (WhID, Ostatus, OutType, RefType, RefID, ShipID, Total, Prepared, Approved, LastModifyTime)
        OUTPUT inserted.Gid INTO @GeneratedKeys
        VALUES (@OldWhID, 2, @StockOutType, 6, @MoveGid, @ShipID, @Total, @ApprovedBy, @ApprovedBy, sysdatetimeoffset());
      SELECT @StockOutGid = g.Gid FROM @GeneratedKeys g;
      -- 生成入库单，状态为未确认，货位未Buffer
      INSERT WarehouseStockIn (WhID, Istatus, InType, RefType, RefID, Prepared, LastModifyTime)
        OUTPUT inserted.Gid INTO @GeneratedKeys
        VALUES (@NewWhID, 0, @StockInType, 6, @MoveGid, @ApprovedBy, sysdatetimeoffset());
      SELECT @StockInGid = g.Gid FROM @GeneratedKeys g;
    END;
    DECLARE MoveItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT wmi.SkuID, wmi.OldShelf, wmi.NewShelf, wmi.TrackLot, wmi.Quantity, wss.Guarantee
        FROM WarehouseMoveItem wmi
        JOIN WarehouseSkuShelf wss ON wmi.SkuID = wss.SkuID AND wmi.OldShelf = wss.ShelfID AND wmi.TrackLot = wss.TrackLot
        WHERE wmi.Deleted = 0
              AND wmi.MoveID = @MoveGid;
    OPEN MoveItemCursor;
    FETCH NEXT FROM MoveItemCursor INTO @SkuID, @OldShelf, @NewShelf, @TrackLot, @Quantity, @Guarantee;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      IF (@MoveType = 0)                          -- 移库
      BEGIN
        -- 生成出库单明细，且减原库存
        INSERT WarehouseOutItem (OutID, SkuID, ShelfID, TrackLot, Quantity)
          VALUES (@StockOutGid, @SkuID, @OldShelf, @TrackLot, @Quantity);
        UPDATE WarehouseSkuShelf
          SET Quantity = Quantity - @Quantity     -- 原货位减库存
          WHERE SkuID = @SkuID
                AND ShelfID = @OldShelf
                AND TrackLot = @TrackLot;
        UPDATE WarehouseLedger
          SET OutQty = OutQty + @Quantity         -- 更新WarhouseLedger库存总账
          WHERE WhID = @OldWhID
                AND SkuID = @SkuID;
        -- 生成入库单明细，货架为Buffer
        INSERT WarehouseInItem (InID, SkuID, ShelfID, TrackLot, Quantity, Guarantee)
          VALUES (@StockInGid, @SkuID, @BufferShelf, @TrackLot, @Quantity, @Guarantee);
      END
      ELSE IF (@MoveType = 1)                     -- 移货位
      BEGIN
        UPDATE WarehouseSkuShelf
          SET Quantity = Quantity - @Quantity     -- 原货位减库存
          WHERE SkuID = @SkuID
                AND ShelfID = @OldShelf
                AND TrackLot = @TrackLot;
        IF EXISTS (
          SELECT wss.Gid
            FROM WarehouseSkuShelf wss
            WHERE wss.SkuID = @SkuID
                  AND wss.ShelfID = @NewShelf
                  AND wss.TrackLot = @TrackLot )
        BEGIN
          UPDATE WarehouseSkuShelf
            SET Quantity = Quantity + @Quantity   -- 新货位加库存
            WHERE SkuID = @SkuID
                  AND ShelfID = @NewShelf
                  AND TrackLot = @TrackLot;
        END
        ELSE
        BEGIN
          INSERT WarehouseSkuShelf (WhID, SkuID, ShelfID, TrackLot, Quantity, Guarantee)
            VALUES (@OldWhID, @SkuID, @NewShelf, @TrackLot, @Quantity, @Guarantee);
        END;
      END;
      FETCH NEXT FROM MoveItemCursor INTO @SkuID, @OldShelf, @NewShelf, @TrackLot, @Quantity, @Guarantee;
    END;
    CLOSE MoveItemCursor;
    DEALLOCATE MoveItemCursor;
    -- 更新移库单状态为已确认
    UPDATE WarehouseMoving
      SET Mstatus = 1,                            -- 已确认
          Approved = @ApprovedBy,
          ApproveTime = sysdatetimeoffset()
      WHERE Gid = @MoveGid;
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_MovingDiscard (
  @MoveID uniqueidentifier,
  @ApprovedBy uniqueidentifier = NULL
) AS
BEGIN
  -- 移库单作废
  -- TODO: 待数据测试
  BEGIN TRANSACTION;
  DECLARE @ResultFlag int = 0;
  DECLARE @MoveGid uniqueidentifier;
  
  -- 查询移库单，一个移库单只能确认一次，不可回退
  SELECT @MoveGid = wm.Gid
    FROM WarehouseMoving wm
    WHERE wm.Deleted = 0
          AND wm.Mstatus = 0                      -- 未确认状态
          AND wm.Gid = @MoveID;
  IF (@MoveGid IS NULL)
    SET @ResultFlag = 1;                          -- 移库单不存在或状态不正确，返回错误代码1
  
  -- 状态合理，开始处理作废
  IF (@ResultFlag = 0)
  BEGIN
    -- 作废移库单
    UPDATE WarehouseMoving
      SET Deleted = 1,                            -- 作废
          LastModifiedBy = @ApprovedBy,
          LastModifyTime = SYSDATETIMEOFFSET(),
          Approved = @ApprovedBy,
          ApproveTime = SYSDATETIMEOFFSET()
      WHERE Gid = @MoveGid;
    UPDATE WarehouseMoveItem
      SET Deleted = 1,
          LastModifiedBy = @ApprovedBy,
          LastModifyTime = SYSDATETIMEOFFSET()
      WHERE MoveID = @MoveGid;
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_ClearRegions(@ParentID uniqueidentifier) AS
BEGIN
  -- 递归删除某个节点下的所有地区，用于更新
  DECLARE @RegionID uniqueidentifier;
  DECLARE RegionsCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT gr.Gid
      FROM GeneralRegion gr
      WHERE gr.Deleted = 0
            AND gr.Parent = @ParentID;
  OPEN RegionsCursor;
  FETCH NEXT FROM RegionsCursor INTO @RegionID;
  WHILE (@@FETCH_STATUS = 0)
  BEGIN
    EXECUTE dbo.sp_ClearRegions @RegionID;
    UPDATE GeneralRegion SET Deleted = 1 WHERE Gid = @RegionID;
    FETCH NEXT FROM RegionsCursor INTO @RegionID;
  END;
  CLOSE RegionsCursor;
  DEALLOCATE RegionsCursor;
END;
GO

CREATE PROCEDURE sp_PrepareOrderShippings (@OrderID uniqueidentifier) AS
BEGIN
  -- 根据OrderItem的组织、渠道、用户和地区，查询所有支持的承运商
  -- 请参阅根据MallCart的同类算法fn_FindAllShippings
  BEGIN TRANSACTION;
  DECLARE @ResultFlag int = 0;
  DECLARE @OrderGid uniqueidentifier;
  DECLARE @Location uniqueidentifier;
  DECLARE @WhID uniqueidentifier;
  DECLARE @ShipID uniqueidentifier;
  DECLARE @ShipWeight int;
  
  SELECT @OrderGid = oi.Gid, @Location = oi.Location, @WhID = oi.WhID
    FROM OrderInformation oi
    WHERE oi.Gid = @OrderID
          AND oi.Deleted = 0;
  IF (@OrderGid IS NULL)
  BEGIN
    SET @ResultFlag = 1;
  END;
  
  IF (@ResultFlag = 0)
  BEGIN
    DECLARE OrderItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT DISTINCT ps.ShipID, ps.ShipWeight
        FROM OrderItem oit
        JOIN ProductOnItem poi ON oit.OnSkuID = poi.Gid
        JOIN ProductOnSale pos ON poi.OnSaleID = pos.Gid
        JOIN ProductOnShipping ps ON ps.OnSaleID = pos.Gid
        JOIN ProductOnShipArea psa ON psa.OnShip = ps.Gid
        JOIN WarehouseShipping ws ON ws.WhID = @WhID AND ws.ShipID = ps.ShipID
        JOIN WarehouseRegion wr ON wr.WhID = @WhID
        JOIN fn_FindFullRegions(@Location) fr ON fr.Gid = psa.RegionID AND fr.Gid = wr.RegionID
        WHERE oit.OrderID = @OrderID
              AND oit.Deleted = 0
        ORDER BY ps.ShipWeight DESC;
    OPEN OrderItemCursor;
    FETCH NEXT FROM OrderItemCursor INTO @ShipID, @ShipWeight;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      IF EXISTS (
        SELECT os.Gid
          FROM OrderShipping os
          WHERE os.OrderID = @OrderID
                AND os.ShipID = @ShipID )
      BEGIN
        UPDATE OrderShipping
          SET ShipWeight = @ShipWeight
          WHERE OrderID = @OrderID
                AND ShipID = @ShipID;
      END
      ELSE
      BEGIN
        INSERT OrderShipping (OrderID, ShipID, ShipWeight, Ostatus)
          VALUES (@OrderID, @ShipID, @ShipWeight, 0);
      END;
      FETCH NEXT FROM OrderItemCursor INTO @ShipID, @ShipWeight;
    END;
    CLOSE OrderItemCursor;
    DEALLOCATE OrderItemCursor;
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_UpdateOrderItem (@OrderID uniqueidentifier) AS
BEGIN
  -- 根据出入库记录更新OrderItem的待发货、已发货、待退货、已退货
  BEGIN TRANSACTION;
  DECLARE @ResultFlag int = 0;
  DECLARE @OrderGid uniqueidentifier;
  DECLARE @Channel uniqueidentifier;
  DECLARE @SkuID uniqueidentifier;
  DECLARE @OnSkuID uniqueidentifier;
  
  SELECT @OrderGid = oi.Gid, @Channel = oi.ChlID
    FROM OrderInformation oi
    WHERE oi.Deleted = 0
          AND oi.Gid = @OrderID;
  IF (@OrderGid IS NOT NULL)
    SET @ResultFlag = 1;                          -- 订单ID有误
  
  IF (@ResultFlag = 0)
  BEGIN
    -- 清除原数据
    UPDATE OrderItem
      SET TobeShip = 0,
          Shipped = 0,
          BeReturn = 0,
          Returned = 0
      WHERE OrderID = @OrderID;
    
    -- 插入新增加的记录（原订单没有的SKU也可以添加进去）
    DECLARE ItemCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
      SELECT DISTINCT s.SkuID
        FROM (
          SELECT wii.SkuID AS SkuID
            FROM WarehouseStockIn wsi
            JOIN WarehouseInItem wii ON wsi.Gid = wii.InID
            WHERE wsi.Deleted = 0 AND wii.Deleted = 0
                  AND wsi.RefType = 0             -- 订单类型
                  AND wsi.RefID = @OrderGid
          UNION
          SELECT woi.SkuID AS SkuID
            FROM WarehouseStockOut wso
            JOIN WarehouseOutItem woi ON wso.Gid = woi.OutID
            WHERE wso.Deleted = 0 AND woi.Deleted = 0
                  AND wso.RefType = 0             -- 订单类型
                  AND wso.RefID = @OrderGid ) s
        WHERE s.SkuID NOT IN (SELECT SkuID FROM OrderItem WHERE Deleted = 0 AND OrderID = @OrderGid);
    OPEN ItemCursor;
    FETCH NEXT FROM ItemCursor INTO @SkuID;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN
      SET @OnSkuID = dbo.fn_FindOnSkuID(@SkuID, @Channel);
      IF (@OnSkuID IS NOT NULL)
        INSERT OrderItem (OrderID, OnSkuID, SkuID, Remark)
          VALUES (@OrderGid, @OnSkuID, @SkuID, '入库单自动添加项');
      FETCH NEXT FROM ItemCursor INTO @SkuID;
    END;
    CLOSE ItemCursor;
    DEALLOCATE ItemCursor;
    
    -- 更新待发货数据
    UPDATE OrderItem
      SET TobeShip = u.Quantity
      FROM (
        SELECT woi.SkuID, SUM(woi.Quantity) AS Quantity
          FROM WarehouseStockOut wso
          JOIN WarehouseOutItem woi ON wso.Gid = woi.OutID
          WHERE wso.Deleted = 0 AND woi.Deleted = 0
                AND wso.Ostatus IN (0, 1)         -- 待发货，扫描中
                AND wso.RefType = 0               -- 订单类型
                AND wso.RefID = @OrderID
          GROUP BY woi.SkuID ) u
      WHERE OrderItem.SkuID = u.SkuID;
    -- 更新已发货数据
    UPDATE OrderItem
      SET Shipped = u.Quantity
      FROM (
        SELECT woi.SkuID, SUM(woi.Quantity) AS Quantity
          FROM WarehouseStockOut wso
          JOIN WarehouseOutItem woi ON wso.Gid = woi.OutID
          WHERE wso.Deleted = 0 AND woi.Deleted = 0
                AND wso.Ostatus IN (2, 3)         -- 已发货，已签收
                AND wso.RefType = 0               -- 订单类型
                AND wso.RefID = @OrderID
          GROUP BY woi.SkuID ) u
      WHERE OrderItem.SkuID = u.SkuID;
    -- 更新待退货数据
    UPDATE OrderItem
      SET Returned = u.Quantity
      FROM (
        SELECT wii.SkuID, SUM(wii.Quantity) AS Quantity
          FROM WarehouseStockIn wsi
          JOIN WarehouseInItem wii ON wsi.Gid = wii.InID
          WHERE wsi.Deleted = 0 AND wii.Deleted = 0
                AND wsi.Istatus = 0               -- 待退货
                AND wsi.RefType = 0               -- 订单类型
                AND wsi.RefID = @OrderID
        GROUP BY wii.SkuID ) u
      WHERE OrderItem.SkuID = u.SkuID;
    -- 更新已退货数据
    UPDATE OrderItem
      SET Returned = u.Quantity
      FROM (
        SELECT wii.SkuID, SUM(wii.Quantity) AS Quantity
          FROM WarehouseStockIn wsi
          JOIN WarehouseInItem wii ON wsi.Gid = wii.InID
          WHERE wsi.Deleted = 0 AND wii.Deleted = 0
                AND wsi.Istatus = 1               -- 已退货
                AND wsi.RefType = 0               -- 订单类型
                AND wsi.RefID = @OrderID
        GROUP BY wii.SkuID ) u
      WHERE OrderItem.SkuID = u.SkuID;
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_UpdateMemberPoint (@PointID uniqueidentifier) AS
BEGIN
  -- 按PointID刷新和重新计算MemberPoint中的余额
  BEGIN TRANSACTION;
  DECLARE @PointGid uniqueidentifier;
  DECLARE @UseScore int;
  DECLARE @UseAmount money;
  
  SELECT @PointGid = mp.Gid
    FROM MemberPoint mp
    WHERE mp.Deleted = 0
          AND mp.Gid = @PointID;
  IF (@PointGid IS NOT NULL)
  BEGIN
    SELECT @UseScore = SUM(mup.Score),
           @UseAmount = SUM(mup.Amount)
      FROM MemberUsePoint mup
      WHERE mup.Deleted = 0
            AND mup.Pstatus > 0
            AND mup.PointID = @PointGid;
    IF (@UseScore IS NOT NULL)
      SET @UseScore = 0;
    IF (@UseAmount IS NOT NULL)
      SET @UseAmount = 0;
    UPDATE MemberPoint
      SET Remain = Score - @UseScore,
          Balance = Amount - @UseAmount
      WHERE Gid = @PointGid;
  END;
  COMMIT TRANSACTION;
END;
GO

CREATE PROCEDURE sp_UpdatePointByOrder (@OrderID uniqueidentifier) AS
BEGIN
  -- 按OrderID刷新和重新计算MemberPoint中的余额
  DECLARE @PointID uniqueidentifier;
  DECLARE PointCursor CURSOR LOCAL FAST_FORWARD READ_ONLY FOR
    SELECT DISTINCT mup.PointID
      FROM MemberUsePoint mup
      WHERE mup.Deleted = 0
            AND mup.RefType = 0                   -- 订单类型
            AND mup.RefID = @OrderID;
  OPEN PointCursor;
  FETCH NEXT FROM PointCursor INTO @PointID;
  WHILE (@@FETCH_STATUS = 0)
  BEGIN
    EXECUTE dbo.sp_UpdateMemberPoint @PointID;
    FETCH NEXT FROM PointCursor INTO @PointID;
  END;
  CLOSE PointCursor;
  DEALLOCATE PointCursor;
  SELECT 0;                                       -- 返回值
END;
GO

CREATE PROCEDURE sp_GenerateLuckyNumber (
  @UserID uniqueidentifier
) AS
BEGIN
  -- 根据既定条件生成幸运号，45天前累计金额过￥2千，并发送手机短信
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  
  DECLARE @OrganID uniqueidentifier;
  DECLARE @UserGid uniqueidentifier;
  DECLARE @Culture int;
  DECLARE @LoginName nvarchar(256);
  DECLARE @CellPhone nvarchar (50);
  DECLARE @Email nvarchar (256);
  
  DECLARE @ReturnPeriod int = 45;                 -- 最长退货期限
  DECLARE @Currency uniqueidentifier;
  DECLARE @PayAmount money = 0;
  
  DECLARE @RandLength int = 8;                    -- 幸运号长度
  DECLARE @RandNumber int;
  
  DECLARE @Template nvarchar (max);
  
  -- 查询用户
  SELECT @OrganID = mu.OrgID, @UserGid = mu.Gid, @Culture = cu.Culture,
         @LoginName = mu.LoginName, @CellPhone = mu.CellPhone, @Email = mu.Email
    FROM MemberUser mu
    LEFT JOIN GeneralCultureUnit cu ON cu.Gid = mu.Culture
    WHERE mu.Deleted = 0
          AND mu.LuckyNumber = 0                  -- 还没有幸运号
          AND mu.Gid = @UserID;
  IF (@UserGid IS NOT NULL)
  BEGIN
    -- 查询退货期限
    SELECT @ReturnPeriod = moa.Matter
      FROM MemberOrgAttribute moa
      JOIN GeneralOptional opt ON opt.Gid = moa.OptID
      WHERE moa.Deleted = 0
            AND moa.OrgID = @OrganID
            AND opt.OrgID = @OrganID
            AND opt.Code = 'ReturnPeriod';        -- 最长退货期限
    SET @ReturnPeriod = (-1) * @ReturnPeriod;
    -- 查询消费金额，不论货币，超过2000元即可，联盟用户条件
    SELECT @PayAmount = SUM(fp.Amount)
        FROM MemberUser mu
        JOIN OrderInformation oi ON oi.UserID = mu.Gid
        JOIN FinancePayment fp ON fp.RefID = oi.Gid
        WHERE mu.Gid = @UserGid
              AND oi.Deleted = 0
              AND oi.Ostatus = 4                      -- 已结算
              AND oi.CreateTime < DATEADD("DAY", @ReturnPeriod, GETDATE())  -- 45天以前的订单
              AND fp.Deleted = 0
              AND fp.RefType = 0                      -- 仅查询订单的付款记录
              AND fp.Pstatus = 2;                     -- 已结算
    IF (@PayAmount >= 2000)
    BEGIN
      -- 符合条件，成为联盟会员，产生幸运号
      SET @RandNumber = dbo.fn_FixLengthRand(@RandLength);
      WHILE EXISTS(SELECT 1 FROM MemberUser WHERE LuckyNumber = @RandNumber)
      BEGIN
        SET @RandNumber = dbo.fn_FixLengthRand(@RandLength);
      END;
      UPDATE MemberUser
        SET LuckyNumber = @RandNumber
        WHERE Gid = @UserGid;
      
      -- 发送手机短信告知，读取模板，替换，保存
      IF (@Culture IS NULL)
        SET @Culture = 2052;
      SELECT @Template = lo.CLOB
        FROM GeneralMessageTemplate gmt
        JOIN viewLargeObject lo ON lo.Gid = gmt.Matter
        WHERE gmt.Deleted = 0
              AND gmt.OrgID = @OrganID
              AND gmt.Code = 'UnionSms'
              AND lo.Culture = @Culture;
      IF (@Template IS NOT NULL)
      BEGIN
        SET @Template = REPLACE(@Template, '{$LuckyNumber}', @RandNumber);
        SET @Template = REPLACE(@Template, '{$UserName}', @LoginName);
        INSERT GeneralMessagePending (UserID, Mtype, Name, Recipient, Matter)
          VALUES (@UserGid, 1, @LoginName, @CellPhone, @Template);
      END;
      -- 发送邮件告知，读取模板，替换，保存
      SET @Template = NULL;
      SELECT @Template = lo.CLOB
        FROM GeneralMessageTemplate gmt
        JOIN viewLargeObject lo ON lo.Gid = gmt.Matter
        WHERE gmt.Deleted = 0
              AND gmt.OrgID = @OrganID
              AND gmt.Code = 'UnionEmail'
              AND lo.Culture = @Culture;
      IF (@Template IS NOT NULL)
      BEGIN
        SET @Template = REPLACE(@Template, '{$LuckyNumber}', @RandNumber);
        SET @Template = REPLACE(@Template, '{$UserName}', @LoginName);
        INSERT GeneralMessagePending (UserID, Mtype, Name, Recipient, Title, Matter)
          VALUES (@UserGid, 2, @LoginName, @Email, 'Notice for VIP Member', @Template);
      END;
    END;
  END
  ELSE
  BEGIN
    SET @ResultFlag = 1;                          -- 用户已有幸运号，或用户不存在
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO

CREATE PROCEDURE sp_UpdateUnionPointByOrder (
  @OrderID uniqueidentifier
) AS
BEGIN
  -- 订单发货后，按联盟规则返点
  BEGIN TRANSACTION;
  DECLARE @GeneratedKeys TABLE ([Gid] uniqueidentifier);
  DECLARE @ResultFlag int = 0;
  
  DECLARE @ReturnPeriod int = 45;                 -- 最长退货期限
  DECLARE @OrderGid uniqueidentifier;
  DECLARE @OrganID uniqueidentifier;
  DECLARE @Currency uniqueidentifier;
  DECLARE @MoneyPaid money = 0;                   -- 订单实际支付款(COD按待付款)
  DECLARE @UserID uniqueidentifier;               -- 订单用户
  
  DECLARE @StartDate datetimeoffset;              -- 返点生效期
  DECLARE @EndDate datetimeoffset;                -- 返点失效期
  DECLARE @Manager uniqueidentifier;              -- 上线会员
  DECLARE @BackLevel int;
  DECLARE @Percentage decimal (18,4);
  DECLARE @PercentTop decimal (18,4);
  DECLARE @Cashier bit;
  DECLARE @PointGid uniqueidentifier;
  DECLARE @GetPoint decimal (18,4);               -- 实际返点金额
  
  -- 查询退换货期限
  SELECT @ReturnPeriod = moa.Matter
    FROM MemberOrgAttribute moa
    JOIN GeneralOptional opt ON opt.Gid = moa.OptID
    WHERE moa.Deleted = 0
          AND moa.OrgID = @OrganID
          AND opt.OrgID = @OrganID
          AND opt.Code = 'ReturnPeriod';          -- 最长退货期限
  
  -- 订单发货后（COD同方案），按联盟规则返点，45天后生效；金额为 TotalFee - K - L - M - O
  SELECT @OrderGid = oi.Gid,
         @OrganID = oi.OrgID,
         @UserID = oi.UserID,
         @Currency = oi.Currency,
         @MoneyPaid = oi.TotalFee - oi.PointPay - oi.CouponPay - oi.BounsPay - oi.Discount,
         @StartDate = DATEADD("DAY", @ReturnPeriod, oi.CreateTime),
         @EndDate = DATEADD("YEAR", 10, oi.CreateTime)       -- 10年有效期
    FROM OrderInformation oi
    WHERE oi.Deleted = 0
          AND oi.Gid = @OrderID
          AND oi.Ostatus IN (3,4)                 -- 已发货，已结算
          AND (oi.TransType <> 0 OR oi.PayStatus = 3); -- 款到发货，则必须已付款
  IF (@OrderGid IS NOT NULL)
  BEGIN
    SELECT @Manager = mu.Manager FROM MemberUser mu WHERE mu.Gid = @UserID;
    SET @BackLevel = 1;
    WHILE (@Manager IS NOT NULL)
    BEGIN
      SET @UserID = @Manager;
      -- TODO RoleID非全关联，存在隐患
      SELECT @Percentage = ulp.Percentage,
             @PercentTop = ulp.PercentTop,
             @Cashier = ulp.Cashier
        FROM UnionLevelPercent ulp
        JOIN MemberRole mr ON mr.Gid = ulp.RoleID
        JOIN (SELECT mr1.Code FROM MemberUser mu1, MemberRole mr1 WHERE mu1.RoleID = mr1.Gid AND mu1.Gid = @UserID) mr2
          ON mr2.Code = mr.Code
        WHERE ulp.Deleted = 0 AND mr.Deleted = 0
              AND ulp.OrgID = @OrganID
              AND mr.OrgID = @OrganID
              AND ulp.BackLevel = @BackLevel      -- 向上层级
              AND ulp.Ustatus = 1;                -- 有效
      IF (@Percentage IS NULL) BREAK;             -- 不支持跳级返点
      -- 判断UserID是否还有上线
      SELECT @Manager = mu.Manager FROM MemberUser mu WHERE mu.Gid = @UserID;
      IF (@Manager IS NULL)
        SET @GetPoint = ROUND(@MoneyPaid * @PercentTop, 2);
      ELSE
        SET @GetPoint = ROUND(@MoneyPaid * @Percentage, 2);
      IF (@GetPoint > 0)
      BEGIN
        -- 发放联盟返点
        SELECT @PointGid = mp.Gid FROM MemberPoint mp
          WHERE mp.OrgID = @OrganID
                AND mp.UserID = @UserID
                AND mp.Ptype = 3                  -- 联盟返点
                AND mp.RefType = 0                -- 订单类型
                AND mp.RefID = @OrderGid;
        IF (@PointGid IS NULL)
          INSERT MemberPoint (OrgID, UserID, Ptype, Pstatus, RefType, RefID, Currency, Amount, Cashier, StartTime, EndTime)
            VALUES (@OrganID, @UserID, 3, 1, 0, @OrderGid, @Currency, @GetPoint, @Cashier, @StartDate, @EndDate);
        ELSE
          UPDATE MemberPoint
            SET Currency = @Currency,
                Amount = @GetPoint,
                Cashier = @Cashier,
                StartTime = @StartDate,
                EndTime = @EndDate
            WHERE Gid = @PointGid;
      END;
      SET @BackLevel = @BackLevel + 1;            -- 再向上级查询
    END;
  END
  ELSE
  BEGIN
    SET @ResultFlag = 1;                          -- 订单状态不正确
  END;
  
  IF (@ResultFlag = 0)
    COMMIT TRANSACTION;
  ELSE
    ROLLBACK TRANSACTION;
  SELECT @ResultFlag;                             -- 返回值
END;
GO


