using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Drawing;

namespace LiveAzure.Stage.Helpers
{
    /// <summary>
    /// 打印窗体
    /// </summary>
    public class PrintForm : System.Windows.Forms.Form
    {
        private System.Drawing.Printing.PrintDocument pd;
        private PrintPreviewControl ppc;
        private PrintPreviewDialog ppd;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private List<string[]> PrintContent;
        private float shiftX;
        private float shiftY;
        private System.ComponentModel.Container components = null;

        public PrintForm(List<string[]> Template)
        {
            InitializeComponent(Template);
        }

        /// <summary>  
        /// 清理所有正在使用的资源。  
        /// </summary>  
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code">Code=0 打印快递单，Code=1打印库存总账</param>
        /// <param name="TemplateArrey">打印内容</param>
        private void InitializeComponent(List<string[]> Template)
        {   
            string[] temp=Template.First();
            this.shiftX = Convert.ToSingle(temp[0]);
            this.shiftY = Convert.ToSingle(temp[1]);
            List<string[]> TempList = Template;
            TempList.RemoveAt(0);
            this.PrintContent = TempList;

            this.pd = new System.Drawing.Printing.PrintDocument();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.SuspendLayout();

            // button1  //   
            this.button1.Location = new System.Drawing.Point(32, 154);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "开始打印";
            this.button1.Click += new System.EventHandler(this.button1_Click);

            //   // button2  //   
            this.button2.Location = new System.Drawing.Point(120, 154);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "打印预览";
            this.button2.Click += new System.EventHandler(this.button2_Click);

            //   // button3  //   
            this.button3.Location = new System.Drawing.Point(208, 154);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "打印机设置";
            this.button3.Click += new System.EventHandler(this.button3_Click);

            //   // Form1  //   
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(314, 199);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "打印窗体";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            this.pd = new System.Drawing.Printing.PrintDocument();
            this.ppc = new PrintPreviewControl();
            this.ppd = new PrintPreviewDialog();
            this.printDialog1 = new PrintDialog();

            this.pd.BeginPrint += new System.Drawing.Printing.PrintEventHandler(pd_BeginPrint);
            this.pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
        }
        private void pd_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //设置横向打印  
            this.pd.DefaultPageSettings.Landscape = false;
            //设置彩色打印  
            this.pd.DefaultPageSettings.Color = false;
            //设置打印纸张类型和大小  
            this.pd.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("订单打印纸", 807, 492);
        }
        private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            foreach (string[] item in this.PrintContent)
            {
                if (Convert.ToSingle(item[3]) != 0)
                {
                    SizeF reawRectangleSize = new SizeF(Convert.ToSingle(item[3]), Convert.ToSingle(item[4]));
                    SizeF sizef = e.Graphics.MeasureString(item[0], new Font("宋体", Convert.ToSingle(item[6]), FontStyle.Regular), reawRectangleSize);
                    Rectangle layoutRectangle = new Rectangle(new Point(Convert.ToInt32(item[1]) + Convert.ToInt32(this.shiftY), Convert.ToInt32(item[2]) + Convert.ToInt32(this.shiftY)), sizef.ToSize());
                    RectangleF layoutString = new RectangleF(new PointF(Convert.ToSingle(item[1])+this.shiftX, Convert.ToSingle(item[2])+this.shiftY), sizef);
                    e.Graphics.DrawString(item[0], new Font(item[5], Convert.ToSingle(item[6]), FontStyle.Regular), Brushes.Black, layoutString);
                }
                else
                {
                    e.Graphics.DrawString(item[0], new Font(item[5], Convert.ToSingle(item[6]), FontStyle.Regular), Brushes.Black, Convert.ToSingle(item[1]) + this.shiftX, Convert.ToSingle(item[2]) + this.shiftY);
                }
            }
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            //开始打印  
            this.pd.Print();
        }
        private void button2_Click(object sender, System.EventArgs e)
        {
            //设置打印预览信息  
            ppc.Document = pd;
            ppc.Columns = 2;
            ppc.Rows = 2;
            ppc.Zoom = 1;
            ppc.StartPage = 1;
            //显示预览  
            ppd.Document = pd;
            try
            {
                ppd.ShowDialog();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, System.EventArgs e)
        {
            //打印机设置  
            this.printDialog1.Document = pd;
            this.printDialog1.AllowSomePages = false;
            this.printDialog1.PrintToFile = false;
            //确定打印机信息后开始打印  
            if (this.printDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.pd.Print();
                }
                catch (Exception excep)
                {
                    MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}