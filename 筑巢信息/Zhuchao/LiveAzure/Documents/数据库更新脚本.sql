-- 2011-09-06
DROP TABLE MemberLevel;

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

CREATE UNIQUE INDEX IX_MemberLevel_OrgID_Mlevel ON MemberLevel (OrgID, Mlevel) ON [PRIMARY];

ALTER TABLE MemberLevel ADD CONSTRAINT FK_MemberLevel_MemberOrganization FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE MemberLevel ADD CONSTRAINT FK_MemberLevel_GeneralResource FOREIGN KEY (Name) REFERENCES GeneralResource (Gid);

ALTER TABLE MemberUser ADD UserLevel uniqueidentifier;

ALTER TABLE MemberUser ADD CONSTRAINT FK_MemberUser_MemberLevel FOREIGN KEY (UserLevel) REFERENCES MemberLevel (Gid);

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

ALTER TABLE ProductOnLevelDiscount ADD CONSTRAINT FK_ProductOnLevelDiscount_MemberLevel FOREIGN KEY (UserLevel) REFERENCES MemberLevel (Gid);

-- 2011-09-07
ALTER TABLE PurchaseInformation ADD Locking tinyint NOT NULL CONSTRAINT DF_PurchaseInformation_Locking DEFAULT 0;
ALTER TABLE PurchaseHistory ADD Locking tinyint NOT NULL CONSTRAINT DF_PurchaseHistory_Locking DEFAULT 0;
DROP INDEX IX_MemberLevel_OrgID_Mlevel ON MemberLevel;
CREATE UNIQUE INDEX IX_MemberLevel_OrgID_Code ON MemberLevel (OrgID, Code) ON [PRIMARY];

--2011-09-08
ALTER TABLE MemberUser ADD ExCode nvarchar (50) NULL;

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
    location uniqueidentifier,
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
    zip nvarchar (max),
    [address] nvarchar (max),
    city nvarchar (max),
    [state] nvarchar (max),
    country nvarchar (max),
    district nvarchar (max),
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoLocation PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

ALTER TABLE ExTaobaoUser ADD CONSTRAINT FK_ExTaobaoUser_ExTaobaoUserCredit_buyer_credit FOREIGN KEY (buyer_credit) REFERENCES ExTaobaoUserCredit (Gid);
ALTER TABLE ExTaobaoUser ADD CONSTRAINT FK_ExTaobaoUser_ExTaobaoUserCredit_seller_credit FOREIGN KEY (seller_credit) REFERENCES ExTaobaoUserCredit (Gid);
ALTER TABLE ExTaobaoUser ADD CONSTRAINT FK_ExTaobaoUser_ExTaobaoLocation FOREIGN KEY (location) REFERENCES ExTaobaoLocation (Gid);


-- 2011-09-13
ALTER TABLE ExTaobaoOrder ADD OrderID uniqueidentifier, Transfered bit, Changed bit;
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

-- 2011-09-22
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
    Remark nvarchar (256),
    CONSTRAINT PK_ExTaobaoDeliveryPending PRIMARY KEY CLUSTERED (Gid)
) ON [PRIMARY];

CREATE UNIQUE INDEX IX_ExTaobaoDeliveryPending_OrderID ON ExTaobaoDeliveryPending (OrderID) ON [PRIMARY];

ALTER TABLE ExTaobaoDeliveryPending ADD CONSTRAINT FK_ExTaobaoDeliveryPending_OrderInformation FOREIGN KEY (OrderID) REFERENCES OrderInformation (Gid);

ALTER TABLE OrderInformation ALTER COLUMN LinkCode nvarchar (max);

-- 2011-09-23
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_ProductOnSale;
ALTER TABLE MallArticle DROP COLUMN OnSaleID;
ALTER TABLE MallArticle ADD Astatus tinyint NOT NULL CONSTRAINT DF_MallArticle_Astatus DEFAULT 0;
ALTER TABLE MallArticle ADD ProdID uniqueidentifier;
ALTER TABLE MallArticle ADD CONSTRAINT FK_MallArticle_ProductInformation FOREIGN KEY (ProdID) REFERENCES ProductInformation (Gid);

DROP INDEX IX_ExTaobaoOrdItem_tid_num_iid ON ExTaobaoOrdItem;

-- 2011-09-25
ALTER TABLE MemberAddress ADD IsDefault bit NOT NULL CONSTRAINT DF_MemberAddress_IsDefault DEFAULT 0;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_MemberOrganization;
ALTER TABLE ProductOnSale ADD OrgID uniqueidentifier NOT NULL;
ALTER TABLE ProductOnUnitPrice ADD IsDefault bit NOT NULL CONSTRAINT DF_ProductOnUnitPrice_IsDefault DEFAULT 0;
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_MemberOrganization_OrgID FOREIGN KEY (OrgID) REFERENCES MemberOrganization (Gid);
ALTER TABLE ProductOnSale ADD CONSTRAINT FK_ProductOnSale_MemberOrganization_ChlID FOREIGN KEY (ChlID) REFERENCES MemberOrganization (Gid);

-- 2011-09-26
ALTER TABLE OrderItem ADD TobeShip decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_TobeShip DEFAULT 0;
ALTER TABLE OrderItem ADD Shipped decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_Shipped DEFAULT 0;
ALTER TABLE OrderItem ADD BeReturn decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_BeReturn DEFAULT 0;
ALTER TABLE OrderItem ADD Returned decimal (18,4) NOT NULL CONSTRAINT DF_OrderItem_Returned DEFAULT 0;

ALTER TABLE OrderHisItem ADD TobeShip decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_TobeShip DEFAULT 0;
ALTER TABLE OrderHisItem ADD Shipped decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_Shipped DEFAULT 0;
ALTER TABLE OrderHisItem ADD BeReturn decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_BeReturn DEFAULT 0;
ALTER TABLE OrderHisItem ADD Returned decimal (18,4) NOT NULL CONSTRAINT DF_OrderHisItem_Returned DEFAULT 0;

ALTER TABLE MallArtPosition ALTER COLUMN Code nvarchar (50) NOT NULL;

ALTER TABLE FinanceInvoice ADD Title nvarchar (256);
ALTER TABLE FinanceInvoice ADD Matter nvarchar (512);

--2011-10-08
ALTER TABLE WarehouseShelf ADD Reserved bit NOT NULL CONSTRAINT DF_WarehouseShelf_Reserved DEFAULT 0;

