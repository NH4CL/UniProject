using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.BLL.UnionPay;

namespace JqForMvcTest_tianyou.Controllers
{
    public class UnionPayController : Controller
    {
        //
        // GET: /UnionPay/

        public ActionResult Index()
        {
            return View();
        }

        public void InfoSend()
        {
            UnionPayAPI cpy = new UnionPayAPI();
            //获取传递给银联chinapay的各个参数-----------------------------------------------
            //string cpyUrl = "http://payment-test.chinapay.com/pay/TransGet"; //测试地址，测试的时候用这个地址，应用到网站时用下面那个地址
            string cpyUrl = "https://payment.chinapay.com/pay/TransGet";   //支付地址                         
            string cpyMerId = "808080580004253"; //ChinaPay统一分配给商户的商户号，15位长度，必填
            string cpyOrdId = "1234022530123456";           //商户提交给ChinaPay的交易订单号，订单号的第五至第九位必须是商户号的最后五位，即“12345”；16位长度，必填
            string cpyTransAmt = "000000001234";             //订单交易金额，12位长度，左补0，必填,单位为分，000000001234 表示 12.34 元
            string cpyCuryId = "156";            //订单交易币种，3位长度，固定为人民币156，必填
            string cpyTransDate = "20110819";            //订单交易日期，8位长度，必填，格式yyyyMMdd
            string cpyTransType = "0001";        //交易类型，4位长度，必填，0001表示消费交易，0002表示退货交易
            string cpyVersion = "20070129";      //支付接入版本号，808080开头的商户用此版本，必填,另一版本为"20070129"
            string cpyBgRetUrl = "/chinapay/Chinapay_Bgreturn.aspx";   //后台交易接收URL，为后台接受应答地址，用于商户记录交易信息和处理，对于使用者是不可见的，长度不要超过80个字节，必填
            string cpyPageRetUrl = "/chinapay/Chinapay_Pgreturn.aspx"; //页面交易接收URL，为页面接受应答地址，用于引导使用者返回支付后的商户网站页面，长度不要超过80个字节，必填
            string cpyGateId = ""; //支付网关号，可选，参看银联网关类型，如填写GateId（支付网关号），则消费者将直接进入支付页面，否则进入网关选择页面
            string cpyPriv1 = "";  //商户私有域，长度不要超过60个字节,商户通过此字段向Chinapay发送的信息，Chinapay依原样填充返回给商户   

            string strChkValue = ""; //256字节长的ASCII码,此次交易所提交的关键数据的数字签名，必填
            strChkValue = cpy.getSign(cpyMerId, cpyOrdId, cpyTransAmt, cpyCuryId, cpyTransDate, cpyTransType);

            ViewBag.message = null;
            if (strChkValue != "")
            {
                Response.Write("<form name='chinapayForm' method='post' action='" + cpyUrl + "'>");         //支付地址
                Response.Write("<input type='hidden' name='MerId' value='" + cpyMerId + "' />");            //商户号
                Response.Write("<input type='hidden' name='OrdId' value='" + cpyOrdId + "' />");            //订单号
                Response.Write("<input type='hidden' name='TransAmt' value='" + cpyTransAmt + "' />");      //支付金额
                Response.Write("<input type='hidden' name='CuryId' value='" + cpyCuryId + "' />");          //交易币种
                Response.Write("<input type='hidden' name='TransDate' value='" + cpyTransDate + "' />");    //交易日期
                Response.Write("<input type='hidden' name='TransType' value='" + cpyTransType + "' />");    //交易类型
                Response.Write("<input type='hidden' name='Version' value='" + cpyVersion + "' />");        //支付接入版本号
                Response.Write("<input type='hidden' name='BgRetUrl' value='" + cpyBgRetUrl + "' />");      //后台接受应答地址
                Response.Write("<input type='hidden' name='PageRetUrl' value='" + cpyPageRetUrl + "' />");  //为页面接受应答地址
                Response.Write("<input type='hidden' name='GateId' value='" + cpyGateId + "' />");          //支付网关号
                Response.Write("<input type='hidden' name='Priv1' value='" + cpyPriv1 + "' />");            //商户私有域，这里将订单自增编号放进去了
                Response.Write("<input type='hidden' name='ChkValue' value='" + strChkValue + "' />");      //此次交易所提交的关键数据的数字签名
                Response.Write("<script>");
                Response.Write("document.chinapayForm.submit();");
                Response.Write("</script></form>");
            }
            else
                Response.Write("参数错误，请稍后再试！");
        }

    }
}
