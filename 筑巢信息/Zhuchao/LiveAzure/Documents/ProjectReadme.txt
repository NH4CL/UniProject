Documents\                      ��Ŀ����ļ�
Library\
    LiveAzure.Model             ����ģʽ��
    LiveAzure.BLL               DAL��BLL ҵ���߼���
    LiveAzure.Resource          ȫ����Դ�ļ�
	LiveAzure.Utility           ���������ú�С����
    LiveAzure.Control           �Զ���ؼ�������LiveTree, LiveEditor, LiveChat, ...
Portal
    LiveAzure.Portal            ����ǰ̨��վ����ͨ�õ�Controller����ͬ��Viewҳ��
              View\CHN\Index.cshtml
              View\EUR\Index.cshtml
              View\EUR\Index.fr-FR.html
Stage
    LiveAzure.Stage             ���̺�̨ERPϵͳ
    LiveAzure.Stage.Tests       ���̺�̨����
	LiveAzure.Stage.Alpha       �ڲ�OAӦ��ϵͳ
Service\
    LiveAzure.Service.Daemon    ϵͳ�����ػ�����
    LiveAzure.WCF.Front         ǰ̨WCF�ӿڷ������
	LiveAzure.WCF.Back          ��̨WCF�ӿڷ������
Tools\
    LiveAzure.Tools.Encrypt     �ַ������ܹ���
	LiveAzure.Tools.Message     ����Ⱥ�����ߣ�ʹ��Excel�洢����
	LiveAzure.Tools.MySqlUtil   MySQL���ߣ�����OpenShop���ʼ��Ͷ��š���������һ�£�����ж�ء�TODO:MySQL��SQL Server������
	LiveAzure.Tools.Prompt      �����й��ߣ�FTPS���ʼ�Ⱥ����
    LiveAzure.Tools.Tester      ���ԣ���ϰʹ��


����ģ�鼰��ǰ׺������ʾ����
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


Լ��
URL�������� http://..../Home?id=b68a6051c6b9e01186f060eb69d65ae8
Product ͳ�� ��Ʒ

Controllers
Ԥ����
css             ����

һ��Ԥ��������� 2011-08-16
Home            ����
Resource        ����
Optional        ���
Category        ���
Region          ½�F�����
Config          ���Σ����鷼
  Config
  MeasureUnit
  CultureUnit
  Message
  Action
  ErrorReport
Program         ½�F�����

Organization    ������
User            ���ƽ����Τ��
Shipping
Purchase


����
Product
OnSale
Order

Warehouse

Finance

Exchange


����
Promotion
Mall
  Home
  Product
  Order (Cart)
  Account
  Promotion
  Advertise


����
Deploy
Test

WarehouseLedger
  ��Ϊ��ϱ�ṹ
PromotionController

ProductController
  /Product/Gallery   ͼƬѹ���ʹ��������������ϴ���������ɺ����.ps1��ͼƬ��ɢ��ͼƬ��������
  /images/org_code/pu/pu_1.jpg
  /images/org_code/pu/pu_1_100x100.jpg

HomeController       ��ҳ
ProductController    �����б������ҳ
  /Product/List?cat_id=...&...   �б������������ɴ��ܶ�����������������
  /Product/Detail/1002           ���飬����ΪProd.Code
  /Product/Detail/DC8C59B6-9BF4-E011-B4ED-60EB69D65AE8  �������飬����ΪProd.Gid
AccountController
  /Account/Index     �ҵ��˺���ҳ
  /Account/Logon     ��½��ע��ҳ
OrderController
  Order/Index        �ҵĹ��ﳵ - �����б�
  Order/Cart         ���ɶ������̣��ο��Ա��Ĺ��ﳵ��ʹ��Promotion�ж���Ĺ���
AdvertiseController
  Advertise/Index/gid   ���ֹ��ҳ�����������

   
