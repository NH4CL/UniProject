Documents\                      项目设计文件
Library\
    LiveAzure.Model             数据模式层
    LiveAzure.BLL               DAL，BLL 业务逻辑层
    LiveAzure.Resource          全局资源文件
	LiveAzure.Utility           常量、配置和小工具
    LiveAzure.Control           自定义控件，包括LiveTree, LiveEditor, LiveChat, ...
Portal
    LiveAzure.Portal            美国前台网站程序，通用的Controller，不同的View页面
              View\CHN\Index.cshtml
              View\EUR\Index.cshtml
              View\EUR\Index.fr-FR.html
Stage
    LiveAzure.Stage             电商后台ERP系统
    LiveAzure.Stage.Tests       电商后台测试
	LiveAzure.Stage.Alpha       内部OA应用系统
Service\
    LiveAzure.Service.Daemon    系统服务守护进程
    LiveAzure.WCF.Front         前台WCF接口服务程序
	LiveAzure.WCF.Back          后台WCF接口服务程序
Tools\
    LiveAzure.Tools.Encrypt     字符串加密工具
	LiveAzure.Tools.Message     短信群发工具，使用Excel存储数据
	LiveAzure.Tools.MySqlUtil   MySQL工具，发送OpenShop的邮件和短信。驱动程序不一致，必须卸载。TODO:MySQL向SQL Server导数据
	LiveAzure.Tools.Prompt      命令行工具，FTPS和邮件群发等
    LiveAzure.Tools.Tester      测试，练习使用


开发模块及其前缀（表名示例）
General                         GeneralConfig, GeneralUnitExchange, GeneralMenuResource, 
Exchange                        ExchangeTaobaoMaster, ExchangePaipaiDetail, ExchangeGroupBuy
Finance                         FinanceIncome
Mall                            MallPageTop
Member                          MemberCorporation

Order                           OrderMaster/OrderDetail
Product                         ProductList
Purchase                        PurchaseOrderMaster, PurchaseOrderDetail
Shipment                        ShipmentDelivery, ShipmentRegion
Warehouse                       WarehouseLocation
Synchro                         SynchroTimestamp


约定
URL参数类似 http://..../Home?id=b68a6051c6b9e01186f060eb69d65ae8
Product 统称 商品

Controllers
预备期
css             昆仑

一期预计完成日期 2011-08-16
Home            天佑
Resource        刘鑫
Optional        天罡
Category        天罡
Region          陆旻，凤凰
Config          刘鑫，龚灵芳
  Config
  MeasureUnit
  CultureUnit
  Message
  Action
  ErrorReport
Program         陆旻，凤凰

Organization    于洋洋
User            吴桂平，李韦男
Shipping
Purchase


二期
Product
OnSale
Order

Warehouse

Finance

Exchange


三期
Promotion
Mall
  Home
  Product
  Order (Cart)
  Account
  Promotion
  Advertise


四期
Deploy
Test

WarehouseLedger
  改为组合表结构
PromotionController

ProductController
  /Product/Gallery   图片压缩和处理，在主服务器上处理，处理完成后调用.ps1将图片扩散到图片服务器上
  /images/org_code/pu/pu_1.jpg
  /images/org_code/pu/pu_1_100x100.jpg

HomeController       首页
ProductController    包括列表和详情页
  /Product/List?cat_id=...&...   列表，包括搜索，可带很多参数，并可扩充参数
  /Product/Detail/1002           详情，参数为Prod.Code
  /Product/Detail/DC8C59B6-9BF4-E011-B4ED-60EB69D65AE8  重载详情，参数为Prod.Gid
AccountController
  /Account/Index     我的账号主页
  /Account/Logon     登陆和注册页
OrderController
  Order/Index        我的购物车 - 内容列表
  Order/Cart         生成订单过程，参考淘宝的购物车，使用Promotion中定义的规则
AdvertiseController
  Advertise/Index/gid   各种广告页，促销，活动等

   
