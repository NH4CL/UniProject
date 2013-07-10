-- ���Ľṹ��������������ֵ䵹��
-- 1. DROP LOGIN
-- 2. DROP PROCEDURE
-- 3. DROP FUNCTION
-- 4. DROP VIEW
-- 5. DROP CONSTRAINT
-- 6. DROP TABLE
-- ������� 2011-09-28 by ����

-- ɾ����½�û���
--USE [master];
--DROP LOGIN [live];

USE [Zhuchao];
-- ����洢����
DROP PROCEDURE sp_InventoryByWarehouseSku;
DROP PROCEDURE sp_UpdatePurchaseInQty;
DROP PROCEDURE sp_GenerateStockInFromPurchase;
DROP PROCEDURE sp_StockInConfirm;
DROP PROCEDURE sp_StockInDiscard;
DROP PROCEDURE sp_GenerateStockOutFromOrder;
DROP PROCEDURE sp_StockOutConfirm;
DROP PROCEDURE sp_StockOutDiscard;
DROP PROCEDURE sp_MovingConfirm;
DROP PROCEDURE sp_MovingDiscard;
DROP PROCEDURE sp_ClearRegions;
DROP PROCEDURE sp_PrepareOrderShippings;
DROP PROCEDURE sp_UpdateOrderItem;
DROP PROCEDURE sp_UpdateMemberPoint;
DROP PROCEDURE sp_UpdatePointByOrder;
DROP PROCEDURE sp_GenerateLuckyNumber;
DROP PROCEDURE sp_UpdateUnionPointByOrder;
DROP PROCEDURE sp_UpdateUnionPointByOrgan;

-- ������
DROP FUNCTION fn_FindDefaultCurrency;
DROP FUNCTION fn_FindDefaultCulture;
DROP FUNCTION fn_FindResourceMoney;
DROP FUNCTION fn_FindResourceMatter;
DROP FUNCTION fn_FindFullRegions;
DROP FUNCTION fn_FindBestWarehouse;
DROP FUNCTION fn_FindAllShippings;
DROP FUNCTION fn_FindBestShipping;
DROP FUNCTION fn_FindShippingPrice;
DROP FUNCTION fn_CartShippingFee;
DROP FUNCTION fn_FindOnSkuShippings;
DROP FUNCTION fn_OrderShippingFee;
DROP FUNCTION fn_FindFullPrograms;
DROP FUNCTION fn_FindFullCategories;
DROP FUNCTION fn_FindOnSkuID;
DROP FUNCTION fn_FixLengthRand;

-- ɾ����ͼ
DROP VIEW viewResourceMatter;
DROP VIEW viewResourceMoney;
DROP VIEW viewLargeObject;
DROP VIEW viewRandom;
GO

-- ����Generalģ�����
ALTER TABLE GeneralOptional DROP CONSTRAINT FK_GeneralOptional_MemberOrganization;
ALTER TABLE GeneralOptional DROP CONSTRAINT FK_GeneralOptional_GeneralResource;
ALTER TABLE GeneralOptItem DROP CONSTRAINT FK_GeneralOptItem_GeneralResource;
ALTER TABLE GeneralStandardCategory DROP CONSTRAINT FK_GeneralStandardCategory_GeneralResource;
ALTER TABLE GeneralPrivateCategory DROP CONSTRAINT FK_GeneralPrivateCategory_GeneralResource;
ALTER TABLE GeneralPrivateCategory DROP CONSTRAINT FK_GeneralPrivateCategory_GeneralMeasureUnit;
ALTER TABLE GeneralMeasureUnit DROP CONSTRAINT FK_GeneralMeasureUnit_GeneralResource;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Piece;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Weight;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Volume;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Fluid;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Area;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Linear;
ALTER TABLE GeneralCultureUnit DROP CONSTRAINT FK_GeneralCultureUnit_GeneralMeasureUnit_Currency;
ALTER TABLE GeneralMessageTemplate DROP CONSTRAINT FK_GeneralMessageTemplate_MemberOrganization;
ALTER TABLE GeneralMessageTemplate DROP CONSTRAINT FK_GeneralMessageTemplate_GeneralResource;
ALTER TABLE GeneralMessageTemplate DROP CONSTRAINT FK_GeneralMessageTemplate_GeneralLargeObject;
ALTER TABLE GeneralMessagePending DROP CONSTRAINT FK_GeneralMessagePending_MemberUser;
ALTER TABLE GeneralProgram DROP CONSTRAINT FK_GeneralProgram_GeneralResource;
ALTER TABLE GeneralProgNode DROP CONSTRAINT FK_GeneralProgNode_GeneralResource_Name;
ALTER TABLE GeneralProgNode DROP CONSTRAINT FK_GeneralProgNode_GeneralResource_Optional;

-- ����Memberģ�����
ALTER TABLE MemberOrganization DROP CONSTRAINT FK_MemberOrganization_GeneralStandardCategory;
ALTER TABLE MemberOrganization DROP CONSTRAINT FK_MemberOrganization_GeneralResource_FullName;
ALTER TABLE MemberOrganization DROP CONSTRAINT FK_MemberOrganization_GeneralResource_ShortName;
ALTER TABLE MemberOrganization DROP CONSTRAINT FK_MemberOrganization_GeneralRegion;
ALTER TABLE MemberOrganization DROP CONSTRAINT FK_MemberOrganization_GeneralLargeObject;
ALTER TABLE MemberOrgCulture DROP CONSTRAINT FK_MemberOrgCulture_GeneralCultureUnit;
ALTER TABLE MemberOrgAttribute DROP CONSTRAINT FK_MemberOrgAttribute_GeneralOptional;
ALTER TABLE MemberOrgAttribute DROP CONSTRAINT FK_MemberOrgAttribute_GeneralOptItem;
ALTER TABLE MemberRole DROP CONSTRAINT FK_MemberRole_GeneralStandardCategory;
ALTER TABLE MemberUser DROP CONSTRAINT FK_MemberUser_GeneralCultureUnit;
ALTER TABLE MemberUser DROP CONSTRAINT FK_MemberUser_MemberLevel;
ALTER TABLE MemberAddress DROP CONSTRAINT FK_MemberAddress_GeneralRegion;
ALTER TABLE MemberUserAttribute DROP CONSTRAINT FK_MemberUserAttribute_GeneralOptional;
ALTER TABLE MemberUserAttribute DROP CONSTRAINT FK_MemberUserAttribute_GeneralOptItem;
ALTER TABLE MemberPoint DROP CONSTRAINT FK_MemberPoint_MemberOrganization;
ALTER TABLE MemberPoint DROP CONSTRAINT FK_MemberPoint_PromotionInformation;
ALTER TABLE MemberPoint DROP CONSTRAINT FK_MemberPoint_PromotionCoupon;
ALTER TABLE MemberPoint DROP CONSTRAINT FK_MemberPoint_GeneralMeasureUnit;
ALTER TABLE MemberLevel DROP CONSTRAINT FK_MemberLevel_MemberOrganization;
ALTER TABLE MemberLevel DROP CONSTRAINT FK_MemberLevel_GeneralResource;
ALTER TABLE MemberUserShortcut DROP CONSTRAINT FK_MemberUserShortcut_GeneralProgram;

-- ����Productģ�����
ALTER TABLE ProductInformation DROP CONSTRAINT FK_ProductInformation_MemberOrganization;
ALTER TABLE ProductInformation DROP CONSTRAINT FK_ProductInformation_GeneralResource_Name;
ALTER TABLE ProductInformation DROP CONSTRAINT FK_ProductInformation_GeneralStandardCategory;
ALTER TABLE ProductInformation DROP CONSTRAINT FK_ProductInformation_GeneralResource_Brief;
ALTER TABLE ProductInformation DROP CONSTRAINT FK_ProductInformation_GeneralLargeObject;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_MemberOrganization;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralResource_FullName;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralResource_ShortName;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralMeasureUnit;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralResource_Specification;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralResource_MarketPrice;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralResource_SuggestPrice;
ALTER TABLE ProductInfoItem DROP CONSTRAINT FK_ProductInfoItem_GeneralResource_LowestPrice;
ALTER TABLE ProductExtendCategory DROP CONSTRAINT FK_ProductExtendCategory_GeneralPrivateCategory;
ALTER TABLE ProductExtendAttribute DROP CONSTRAINT FK_ProductExtendAttribute_GeneralOptional;
ALTER TABLE ProductExtendAttribute DROP CONSTRAINT FK_ProductExtendAttribute_GeneralOptItem;
ALTER TABLE ProductGallery DROP CONSTRAINT FK_ProductGallery_ProductInfoItem;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_MemberOrganization_OrgID;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_MemberOrganization_ChlID;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_GeneralResource_Name;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_GeneralResource_MarketPrice;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_GeneralResource_SalePrice;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_GeneralResource_Brief;
ALTER TABLE ProductOnSale DROP CONSTRAINT FK_ProductOnSale_GeneralLargeObject;
ALTER TABLE ProductOnItem DROP CONSTRAINT FK_ProductOnItem_ProductInfoItem;
ALTER TABLE ProductOnItem DROP CONSTRAINT FK_ProductOnItem_GeneralResource_ScoreDeduct;
ALTER TABLE ProductOnUnitPrice DROP CONSTRAINT FK_ProductOnUnitPrice_GeneralMeasureUnit;
ALTER TABLE ProductOnUnitPrice DROP CONSTRAINT FK_ProductOnUnitPrice_GeneralResource_MarketPrice;
ALTER TABLE ProductOnUnitPrice DROP CONSTRAINT FK_ProductOnUnitPrice_GeneralResource_SalePrice;
ALTER TABLE ProductOnLevelDiscount DROP CONSTRAINT FK_ProductOnLevelDiscount_MemberLevel;
ALTER TABLE ProductOnTemplate DROP CONSTRAINT FK_ProductOnTemplate_MemberOrganization;
ALTER TABLE ProductOnTemplate DROP CONSTRAINT FK_ProductOnTemplate_GeneralResource;
ALTER TABLE ProductOnShipping DROP CONSTRAINT FK_ProductOnShipping_MemberOrganization;
ALTER TABLE ProductOnShipArea DROP CONSTRAINT FK_ProductOnShipArea_GeneralRegion;
ALTER TABLE ProductOnPayment DROP CONSTRAINT FK_ProductOnPayment_FinancePayType;
ALTER TABLE ProductOnAdjust DROP CONSTRAINT FK_ProductOnAdjust_ProductOnItem;
ALTER TABLE ProductOnAdjust DROP CONSTRAINT FK_ProductOnAdjust_GeneralResource_SalePriceOld;
ALTER TABLE ProductOnAdjust DROP CONSTRAINT FK_ProductOnAdjust_GeneralResource_ScoreDeductOld;
ALTER TABLE ProductOnAdjust DROP CONSTRAINT FK_ProductOnAdjust_GeneralResource_SalePriceNew;
ALTER TABLE ProductOnAdjust DROP CONSTRAINT FK_ProductOnAdjust_GeneralResource_ScoreDeductNew;

-- ����Purchaseģ�����
ALTER TABLE PurchaseInformation DROP CONSTRAINT FK_PurchaseInformation_MemberOrganization_OrgID;
ALTER TABLE PurchaseInformation DROP CONSTRAINT FK_PurchaseInformation_MemberOrganization_WhID;
ALTER TABLE PurchaseInformation DROP CONSTRAINT FK_PurchaseInformation_MemberOrganization_Supplier;
ALTER TABLE PurchaseInformation DROP CONSTRAINT FK_PurchaseInformation_GeneralPrivateCategory;
ALTER TABLE PurchaseInformation DROP CONSTRAINT FK_PurchaseInformation_GeneralMeasureUnit;
ALTER TABLE PurchaseItem DROP CONSTRAINT FK_PurchaseItem_ProductInfoItem;
ALTER TABLE PurchaseInspection DROP CONSTRAINT FK_PurchaseInspection_MemberOrganization;
ALTER TABLE PurchaseInspection DROP CONSTRAINT FK_PurchaseInspection_PurchaseInformation;
ALTER TABLE PurchaseInspItem DROP CONSTRAINT FK_PurchaseInspItem_ProductInfoItem;

-- ����Warehouseģ�����
ALTER TABLE WarehouseShelf DROP CONSTRAINT FK_WarehouseShelf_MemberOrganization;
ALTER TABLE WarehouseChannel DROP CONSTRAINT FK_WarehouseChannel_MemberOrganization_WhID;
ALTER TABLE WarehouseChannel DROP CONSTRAINT FK_WarehouseChannel_MemberOrganization_ChlID;
ALTER TABLE WarehouseRegion DROP CONSTRAINT FK_WarehouseRegion_MemberOrganization;
ALTER TABLE WarehouseRegion DROP CONSTRAINT FK_WarehouseRegion_GeneralRegion;
ALTER TABLE WarehouseShipping DROP CONSTRAINT FK_WarehouseShipping_MemberOrganization_WhID;
ALTER TABLE WarehouseShipping DROP CONSTRAINT FK_WarehouseShipping_MemberOrganization_ShipID;
ALTER TABLE WarehouseLedger DROP CONSTRAINT FK_WarehouseLedger_MemberOrganization_WhID;
ALTER TABLE WarehouseLedger DROP CONSTRAINT FK_WarehouseLedger_ProductInfoItem;
ALTER TABLE WarehouseLedger DROP CONSTRAINT FK_WarehouseLedger_GeneralResource_AvgCost;
ALTER TABLE WarehouseSkuShelf DROP CONSTRAINT FK_WarehouseSkuShelf_MemberOrganization;
ALTER TABLE WarehouseSkuShelf DROP CONSTRAINT FK_WarehouseSkuShelf_WarehouseShelf;
ALTER TABLE WarehouseSkuShelf DROP CONSTRAINT FK_WarehouseSkuShelf_ProductInfoItem;
ALTER TABLE WarehouseStockIn DROP CONSTRAINT FK_WarehouseStockIn_MemberOrganization;
ALTER TABLE WarehouseStockIn DROP CONSTRAINT FK_WarehouseStockIn_GeneralStandardCategory;
ALTER TABLE WarehouseInItem DROP CONSTRAINT FK_WarehouseInItem_ProductInfoItem;
ALTER TABLE WarehouseInItem DROP CONSTRAINT FK_WarehouseInItem_WarehouseShelf;
ALTER TABLE WarehouseStockOut DROP CONSTRAINT FK_WarehouseStockOut_MemberOrganization_WhID;
ALTER TABLE WarehouseStockOut DROP CONSTRAINT FK_WarehouseStockOut_GeneralStandardCategory;
ALTER TABLE WarehouseStockOut DROP CONSTRAINT FK_WarehouseStockOut_MemberOrganization_ShipID;
ALTER TABLE WarehouseOutItem DROP CONSTRAINT FK_WarehouseOutItem_ProductInfoItem;
ALTER TABLE WarehouseOutItem DROP CONSTRAINT FK_WarehouseOutItem_WarehouseShelf;
ALTER TABLE WarehouseOutScan DROP CONSTRAINT FK_WarehouseOutScan_ProductInfoItem;
ALTER TABLE WarehouseOutDelivery DROP CONSTRAINT FK_WarehouseOutDelivery_MemberOrganization;
ALTER TABLE WarehouseMoving DROP CONSTRAINT FK_WarehouseMoving_MemberOrganization_OrgID;
ALTER TABLE WarehouseMoving DROP CONSTRAINT FK_WarehouseMoving_MemberOrganization_OldWhID;
ALTER TABLE WarehouseMoving DROP CONSTRAINT FK_WarehouseMoving_MemberOrganization_NewWhID;
ALTER TABLE WarehouseMoving DROP CONSTRAINT FK_WarehouseMoving_MemberOrganization_ShipID;
ALTER TABLE WarehouseMoveItem DROP CONSTRAINT FK_WarehouseMoveItem_ProductInfoItem;
ALTER TABLE WarehouseMoveItem DROP CONSTRAINT FK_WarehouseMoveItem_WarehouseShelf_OldShelf;
ALTER TABLE WarehouseMoveItem DROP CONSTRAINT FK_WarehouseMoveItem_WarehouseShelf_NewShelf;
ALTER TABLE WarehouseInventory DROP CONSTRAINT FK_WarehouseInventory_MemberOrganization_OrgID;
ALTER TABLE WarehouseInventory DROP CONSTRAINT FK_WarehouseInventory_MemberOrganization_WhID;
ALTER TABLE WarehouseInvItem DROP CONSTRAINT FK_WarehouseInvItem_ProductInfoItem;
ALTER TABLE WarehouseInvItem DROP CONSTRAINT FK_WarehouseInvItem_WarehouseShelf;

-- ����Order���
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_MemberOrganization_OrgID;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_MemberOrganization_ChlID;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_MemberOrganization_WhID;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_MemberUser;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_FinancePayType;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_GeneralMeasureUnit;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_GeneralRegion;
ALTER TABLE OrderInformation DROP CONSTRAINT FK_OrderInformation_UnionAdvertising;
ALTER TABLE OrderItem DROP CONSTRAINT FK_OrderItem_ProductOnItem;
ALTER TABLE OrderItem DROP CONSTRAINT FK_OrderItem_ProductInfoItem;
ALTER TABLE OrderShipping DROP CONSTRAINT FK_OrderShipping_MemberOrganization;
ALTER TABLE OrderAttribute DROP CONSTRAINT FK_OrderAttribute_GeneralOptional;
ALTER TABLE OrderAttribute DROP CONSTRAINT FK_OrderAttribute_GeneralOptItem;
ALTER TABLE OrderSplitPolicy DROP CONSTRAINT FK_OrderSplitPolicy_MemberOrganization_OrgID;
ALTER TABLE OrderSplitPolicy DROP CONSTRAINT FK_OrderSplitPolicy_MemberOrganization_ChlID;
ALTER TABLE PromotionInformation DROP CONSTRAINT FK_PromotionInformation_MemberOrganization_OrgID;
ALTER TABLE PromotionInformation DROP CONSTRAINT FK_PromotionInformation_MemberOrganization_ChlID;
ALTER TABLE PromotionInformation DROP CONSTRAINT FK_PromotionInformation_GeneralResource;
ALTER TABLE PromotionProduct DROP CONSTRAINT FK_PromotionProduct_ProductOnItem;
ALTER TABLE PromotionProduct DROP CONSTRAINT FK_PromotionProduct_GeneralResource;
ALTER TABLE PromotionCoupon DROP CONSTRAINT FK_PromotionCoupon_GeneralMeasureUnit;

-- ����Shipping���
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_MemberOrganization;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralRegion;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_Residential;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_LiftGate;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_Installation;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_PriceWeight;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_PriceVolume;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_PricePiece;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_PriceHigh;
ALTER TABLE ShippingArea DROP CONSTRAINT FK_ShippingArea_GeneralResource_PriceLow;
ALTER TABLE ShippingEnvelope DROP CONSTRAINT FK_ShippingEnvelope_MemberOrganization;
ALTER TABLE ShippingEnvelope DROP CONSTRAINT FK_ShippingEnvelope_GeneralLargeObject;

-- ����Mall���
ALTER TABLE MallArtPosition DROP CONSTRAINT FK_MallArtPosition_MemberOrganization;
ALTER TABLE MallArtPosition DROP CONSTRAINT FK_MallArtPosition_GeneralResource;
ALTER TABLE MallArtPosition DROP CONSTRAINT FK_MallArtPosition_GeneralLargeObject;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_MemberOrganization;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_GeneralPrivateCategory_Atype;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_GeneralPrivateCategory_Acategory;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_MemberUser;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_ProductInformation;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_GeneralResource;
ALTER TABLE MallArticle DROP CONSTRAINT FK_MallArticle_GeneralLargeObject;
ALTER TABLE MallArtPublish DROP CONSTRAINT FK_MallArtPublish_MemberOrganization;
ALTER TABLE MallArtPublish DROP CONSTRAINT FK_MallArtPublish_MallArtPosition;
ALTER TABLE MallHotWord DROP CONSTRAINT FK_MallHotWord_MemberOrganization_OrgID;
ALTER TABLE MallHotWord DROP CONSTRAINT FK_MallHotWord_MemberOrganization_ChlID;
ALTER TABLE MallHotItem DROP CONSTRAINT FK_MallHotItem_MemberOrganization_OrgID;
ALTER TABLE MallHotItem DROP CONSTRAINT FK_MallHotItem_MemberOrganization_ChlID;
ALTER TABLE MallFavorite DROP CONSTRAINT FK_MallFavorite_MemberOrganization_OrgID;
ALTER TABLE MallFavorite DROP CONSTRAINT FK_MallFavorite_MemberOrganization_ChlID;
ALTER TABLE MallFavorite DROP CONSTRAINT FK_MallFavorite_MemberUser;
ALTER TABLE MallFavorite DROP CONSTRAINT FK_MallFavorite_ProductOnSale;
ALTER TABLE MallFocusProduct DROP CONSTRAINT FK_MallFocusProduct_MemberOrganization_OrgID;
ALTER TABLE MallFocusProduct DROP CONSTRAINT FK_MallFocusProduct_MemberOrganization_ChlID;
ALTER TABLE MallFocusProduct DROP CONSTRAINT FK_MallFocusProduct_MemberUser;
ALTER TABLE MallFocusProduct DROP CONSTRAINT FK_MallFocusProduct_ProductOnSale;
ALTER TABLE MallFocusProduct DROP CONSTRAINT FK_MallFocusProduct_GeneralMeasureUnit;
ALTER TABLE MallFriendLink DROP CONSTRAINT FK_MallFriendLink_MemberOrganization_OrgID;
ALTER TABLE MallFriendLink DROP CONSTRAINT FK_MallFriendLink_MemberOrganization_ChlID;
ALTER TABLE MallFriendLink DROP CONSTRAINT FK_MallFriendLink_GeneralResource;
ALTER TABLE MallSensitiveWord DROP CONSTRAINT FK_MallSensitiveWord_MemberOrganization_OrgID;
ALTER TABLE MallSensitiveWord DROP CONSTRAINT FK_MallSensitiveWord_MemberOrganization_ChlID;
ALTER TABLE MallDisabledIp DROP CONSTRAINT FK_MallDisabledIp_MemberOrganization_OrgID;
ALTER TABLE MallDisabledIp DROP CONSTRAINT FK_MallDisabledIp_MemberOrganization_ChlID;
ALTER TABLE MallVisitClick DROP CONSTRAINT FK_MallVisitClick_MemberOrganization_OrgID;
ALTER TABLE MallVisitClick DROP CONSTRAINT FK_MallVisitClick_MemberOrganization_ChlID;
ALTER TABLE MallVisitClick DROP CONSTRAINT FK_MallVisitClick_MemberUser;
ALTER TABLE MallVisitClick DROP CONSTRAINT FK_MallVisitClick_ProductOnSale;
ALTER TABLE MallVisitClick DROP CONSTRAINT FK_MallVisitClick_GeneralMeasureUnit;
ALTER TABLE MallVisitClick DROP CONSTRAINT FK_MallVisitClick_UnionAdvertising;
ALTER TABLE MallCart DROP CONSTRAINT FK_MallCart_MemberOrganization_OrgID;
ALTER TABLE MallCart DROP CONSTRAINT FK_MallCart_MemberOrganization_ChlID;
ALTER TABLE MallCart DROP CONSTRAINT FK_MallCart_MemberUser;
ALTER TABLE MallCart DROP CONSTRAINT FK_MallCart_ProductOnSale;
ALTER TABLE MallCart DROP CONSTRAINT FK_MallCart_ProductOnItem;
ALTER TABLE MallCart DROP CONSTRAINT FK_MallCart_GeneralStandardCategory;

-- ����Union���
ALTER TABLE UnionLevelPercent DROP CONSTRAINT FK_UnionLevelPercent_MemberOrganization_OrgID;
ALTER TABLE UnionLevelPercent DROP CONSTRAINT FK_UnionLevelPercent_MemberRole;
ALTER TABLE UnionFixedPercent DROP CONSTRAINT FK_UnionFixedPercent_MemberOrganization_OrgID;
ALTER TABLE UnionFixedPercent DROP CONSTRAINT FK_UnionFixedPercent_MemberOrganization_ChlID;

-- ����Knowledge���

-- ����Finance���
ALTER TABLE FinancePayType DROP CONSTRAINT FK_FinancePayType_MemberOrganization;
ALTER TABLE FinancePayType DROP CONSTRAINT FK_FinancePayType_GeneralResource;
ALTER TABLE FinanceInvoice DROP CONSTRAINT FK_FinanceInvoice_OrderInformation;
ALTER TABLE FinancePayment DROP CONSTRAINT FK_FinancePayment_MemberOrganization;
ALTER TABLE FinancePayment DROP CONSTRAINT FK_FinancePayment_GeneralPrivateCategory;
ALTER TABLE FinancePayment DROP CONSTRAINT FK_FinancePayment_GeneralMeasureUnit;

-- ����Performance���

-- ����Synchro���

-- ����Exchange���
ALTER TABLE ExTaobaoOrder DROP CONSTRAINT FK_ExTaobaoOrder_MemberOrganization_OrgID;
ALTER TABLE ExTaobaoOrder DROP CONSTRAINT FK_ExTaobaoOrder_MemberOrganization_ChlID;
ALTER TABLE ExTaobaoOrder DROP CONSTRAINT FK_ExTaobaoOrder_OrderInformation;

ALTER TABLE ExTaobaoUser DROP CONSTRAINT FK_ExTaobaoUser_ExTaobaoUserCredit_buyer_credit;
ALTER TABLE ExTaobaoUser DROP CONSTRAINT FK_ExTaobaoUser_ExTaobaoUserCredit_seller_credit;

ALTER TABLE ExTaobaoUserOrgan DROP CONSTRAINT FK_ExTaobaoUserOrgan_MemberOrganization_OrgID;
ALTER TABLE ExTaobaoUserOrgan DROP CONSTRAINT FK_ExTaobaoUserOrgan_MemberOrganization_ChlID;
ALTER TABLE ExTaobaoUserOrgan DROP CONSTRAINT FK_ExTaobaoUserOrgan_ExTaobaoUser;

GO

-- ����Exchange����
DROP TABLE ExTaobaoDeliveryPending;
DROP TABLE ExTaobaoLogisticsCompany
DROP TABLE ExTaobaoArea;
DROP TABLE ExTaobaoUserOrgan;
DROP TABLE ExTaobaoLocation;
DROP TABLE ExTaobaoUserCredit;
DROP TABLE ExTaobaoUser;
DROP TABLE ExTaobaoRefundRemind;
DROP TABLE ExTaobaoRefund;
DROP TABLE ExTaobaoPromotion;
DROP TABLE ExTaobaoTradeRate;
DROP TABLE ExTaobaoOrdItem;
DROP TABLE ExTaobaoOrder;

-- ����Synchro����
DROP TABLE SynchroTable;

-- ����Performance����

-- ����Finance����
DROP TABLE FinancePayment;
DROP TABLE FinanceInvoice;
DROP TABLE FinancePayType;

-- ����Knowledge����

-- ����Union����
DROP TABLE UnionAdvertising;
DROP TABLE UnionFixedPercent;
DROP TABLE UnionLevelPercent;

-- ����Mall����
DROP TABLE MallCart;
DROP TABLE MallVisitClick;
DROP TABLE MallDisabledIp;
DROP TABLE MallSensitiveWord;
DROP TABLE MallFriendLink;
DROP TABLE MallFocusProduct;
DROP TABLE MallFavorite;
DROP TABLE MallHotItem;
DROP TABLE MallHotWord;
DROP TABLE MallArtPublish;
DROP TABLE MallArticle;
DROP TABLE MallArtPosition;

-- ����Shipping����
DROP TABLE ShippingEnvelope;
DROP TABLE ShippingArea;

-- ����Order����
DROP TABLE PromotionCoupon;
DROP TABLE PromotionProduct;
DROP TABLE PromotionMutex;
DROP TABLE PromotionInformation;
DROP TABLE OrderHisItem;
DROP TABLE OrderHistory;
DROP TABLE OrderSplitPolicy;
DROP TABLE OrderProcess;
DROP TABLE OrderAttribute;
DROP TABLE OrderShipping;
DROP TABLE OrderItem;
DROP TABLE OrderInformation;

-- ����Warehouse����
DROP TABLE WarehouseInvItem;
DROP TABLE WarehouseInventory;
DROP TABLE WarehouseMoveItem;
DROP TABLE WarehouseMoving;
DROP TABLE WarehouseOutDelivery;
DROP TABLE WarehouseOutScan;
DROP TABLE WarehouseOutItem;
DROP TABLE WarehouseStockOut;
DROP TABLE WarehouseInItem;
DROP TABLE WarehouseStockIn;
DROP TABLE WarehouseSkuShelf;
DROP TABLE WarehouseLedger;
DROP TABLE WarehouseShipping;
DROP TABLE WarehouseRegion;
DROP TABLE WarehouseChannel;
DROP TABLE WarehouseShelf;

-- ����Purchase����
DROP TABLE PurchaseInspItem;
DROP TABLE PurchaseInspection;
DROP TABLE PurchaseHisItem;
DROP TABLE PurchaseHistory;
DROP TABLE PurchaseItem;
DROP TABLE PurchaseInformation;

-- ����Product����
DROP TABLE ProductOnAdjust;
DROP TABLE ProductOnRelation;
DROP TABLE ProductOnPayment;
DROP TABLE ProductOnShipArea;
DROP TABLE ProductOnShipping;
DROP TABLE ProductOnTemplate;
DROP TABLE ProductOnLevelDiscount;
DROP TABLE ProductOnUnitPrice;
DROP TABLE ProductOnItem;
DROP TABLE ProductOnSale;
DROP TABLE ProductGallery;
DROP TABLE ProductExtendAttribute;
DROP TABLE ProductExtendCategory;
DROP TABLE ProductInfoItem;
DROP TABLE ProductInformation;

-- ����Member����
DROP TABLE MemberPrivItem;
DROP TABLE MemberPrivilege;
DROP TABLE MemberUserShortcut;
DROP TABLE MemberUserEvent;
DROP TABLE MemberSubscribe;
DROP TABLE MemberLevel;
DROP TABLE MemberUsePoint;
DROP TABLE MemberPoint;
DROP TABLE MemberUserAttribute;
DROP TABLE MemberAddress;
DROP TABLE MemberUser;
DROP TABLE MemberRole;
DROP TABLE MemberOrgAttribute;
DROP TABLE MemberOrgCulture;
DROP TABLE MemberOrgChannel;
DROP TABLE MemberOrganization;

-- ����General����
DROP TABLE GeneralTodoList;
DROP TABLE GeneralErrorReport;
DROP TABLE GeneralAction;
DROP TABLE GeneralPrivItem;
DROP TABLE GeneralPrivTemplate;
DROP TABLE GeneralProgNode;
DROP TABLE GeneralProgram;
DROP TABLE GeneralIpBase;
DROP TABLE GeneralMessageReceive;
DROP TABLE GeneralMessagePending;
DROP TABLE GeneralMessageTemplate;
DROP TABLE GeneralCultureUnit;
DROP TABLE GeneralMeasureUnit;
DROP TABLE GeneralRegion;
DROP TABLE GeneralStandardCategory;
DROP TABLE GeneralPrivateCategory;
DROP TABLE GeneralOptItem;
DROP TABLE GeneralOptional;
DROP TABLE GeneralLargeItem;
DROP TABLE GeneralLargeObject;
DROP TABLE GeneralConfig;
DROP TABLE GeneralResItem;
DROP TABLE GeneralResource;
GO
