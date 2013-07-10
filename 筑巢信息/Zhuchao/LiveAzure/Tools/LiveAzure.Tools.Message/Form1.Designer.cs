namespace LiveAzure.Tools.Message
{
    partial class frmShortMessage
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShortMessage));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblSingleMessage = new System.Windows.Forms.Label();
            this.btnSendSingle = new System.Windows.Forms.Button();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.lblReceiver = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkExcelDuplicate = new System.Windows.Forms.CheckBox();
            this.lblExcelMessage = new System.Windows.Forms.Label();
            this.imgExcelFormat = new System.Windows.Forms.PictureBox();
            this.lblExcelFormat = new System.Windows.Forms.Label();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.txtExcelFile = new System.Windows.Forms.TextBox();
            this.lblExcelFile = new System.Windows.Forms.Label();
            this.btnSendExcel = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkGeneralDuplicate = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGeneralContent = new System.Windows.Forms.TextBox();
            this.lblGeneralMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnBrowseGeneral = new System.Windows.Forms.Button();
            this.txtGeneralExcel = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnGeneralSend = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSessionKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSerialNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseData = new System.Windows.Forms.Button();
            this.grpAccountInfo = new System.Windows.Forms.GroupBox();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnGetMessage = new System.Windows.Forms.Button();
            this.lblViewMessage = new System.Windows.Forms.Label();
            this.btnViewData = new System.Windows.Forms.Button();
            this.txtStorage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.txtHelp = new System.Windows.Forms.RichTextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgExcelFormat)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpAccountInfo.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(12, 63);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(512, 261);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblSingleMessage);
            this.tabPage1.Controls.Add(this.btnSendSingle);
            this.tabPage1.Controls.Add(this.txtContent);
            this.tabPage1.Controls.Add(this.lblContent);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtReceiver);
            this.tabPage1.Controls.Add(this.lblReceiver);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(504, 235);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "单发短信";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblSingleMessage
            // 
            this.lblSingleMessage.AutoSize = true;
            this.lblSingleMessage.Location = new System.Drawing.Point(173, 211);
            this.lblSingleMessage.Name = "lblSingleMessage";
            this.lblSingleMessage.Size = new System.Drawing.Size(89, 12);
            this.lblSingleMessage.TabIndex = 6;
            this.lblSingleMessage.Text = "Return Message";
            // 
            // btnSendSingle
            // 
            this.btnSendSingle.Location = new System.Drawing.Point(83, 206);
            this.btnSendSingle.Name = "btnSendSingle";
            this.btnSendSingle.Size = new System.Drawing.Size(75, 23);
            this.btnSendSingle.TabIndex = 5;
            this.btnSendSingle.Text = "立即发送";
            this.btnSendSingle.UseVisualStyleBackColor = true;
            this.btnSendSingle.Click += new System.EventHandler(this.btnSendSingle_Click);
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(83, 44);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(388, 130);
            this.txtContent.TabIndex = 4;
            this.txtContent.Tag = "SingleContent";
            this.txtContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Location = new System.Drawing.Point(6, 53);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(41, 12);
            this.lblContent.TabIndex = 3;
            this.lblContent.Text = "内容：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(310, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "最多10个手机号，用逗号分隔";
            // 
            // txtReceiver
            // 
            this.txtReceiver.Location = new System.Drawing.Point(83, 14);
            this.txtReceiver.Name = "txtReceiver";
            this.txtReceiver.Size = new System.Drawing.Size(221, 21);
            this.txtReceiver.TabIndex = 1;
            // 
            // lblReceiver
            // 
            this.lblReceiver.AutoSize = true;
            this.lblReceiver.Location = new System.Drawing.Point(6, 17);
            this.lblReceiver.Name = "lblReceiver";
            this.lblReceiver.Size = new System.Drawing.Size(53, 12);
            this.lblReceiver.TabIndex = 0;
            this.lblReceiver.Text = "收件人：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkExcelDuplicate);
            this.tabPage2.Controls.Add(this.lblExcelMessage);
            this.tabPage2.Controls.Add(this.imgExcelFormat);
            this.tabPage2.Controls.Add(this.lblExcelFormat);
            this.tabPage2.Controls.Add(this.btnBrowseFile);
            this.tabPage2.Controls.Add(this.txtExcelFile);
            this.tabPage2.Controls.Add(this.lblExcelFile);
            this.tabPage2.Controls.Add(this.btnSendExcel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(504, 235);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "群发短信";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkExcelDuplicate
            // 
            this.chkExcelDuplicate.AutoSize = true;
            this.chkExcelDuplicate.Checked = true;
            this.chkExcelDuplicate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExcelDuplicate.Location = new System.Drawing.Point(83, 41);
            this.chkExcelDuplicate.Name = "chkExcelDuplicate";
            this.chkExcelDuplicate.Size = new System.Drawing.Size(108, 16);
            this.chkExcelDuplicate.TabIndex = 11;
            this.chkExcelDuplicate.Text = "过滤重复手机号";
            this.chkExcelDuplicate.UseVisualStyleBackColor = true;
            // 
            // lblExcelMessage
            // 
            this.lblExcelMessage.AutoSize = true;
            this.lblExcelMessage.Location = new System.Drawing.Point(173, 211);
            this.lblExcelMessage.Name = "lblExcelMessage";
            this.lblExcelMessage.Size = new System.Drawing.Size(89, 12);
            this.lblExcelMessage.TabIndex = 13;
            this.lblExcelMessage.Text = "Return Message";
            // 
            // imgExcelFormat
            // 
            this.imgExcelFormat.Image = global::LiveAzure.Tools.Message.Properties.Resources.Pending;
            this.imgExcelFormat.Location = new System.Drawing.Point(83, 63);
            this.imgExcelFormat.Name = "imgExcelFormat";
            this.imgExcelFormat.Size = new System.Drawing.Size(362, 119);
            this.imgExcelFormat.TabIndex = 12;
            this.imgExcelFormat.TabStop = false;
            // 
            // lblExcelFormat
            // 
            this.lblExcelFormat.AutoSize = true;
            this.lblExcelFormat.Location = new System.Drawing.Point(6, 63);
            this.lblExcelFormat.Name = "lblExcelFormat";
            this.lblExcelFormat.Size = new System.Drawing.Size(65, 12);
            this.lblExcelFormat.TabIndex = 11;
            this.lblExcelFormat.Text = "格式示例：";
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Location = new System.Drawing.Point(415, 12);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFile.TabIndex = 10;
            this.btnBrowseFile.Tag = "SendFile";
            this.btnBrowseFile.Text = "浏览";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // txtExcelFile
            // 
            this.txtExcelFile.Location = new System.Drawing.Point(83, 14);
            this.txtExcelFile.Name = "txtExcelFile";
            this.txtExcelFile.Size = new System.Drawing.Size(310, 21);
            this.txtExcelFile.TabIndex = 9;
            // 
            // lblExcelFile
            // 
            this.lblExcelFile.AutoSize = true;
            this.lblExcelFile.Location = new System.Drawing.Point(6, 17);
            this.lblExcelFile.Name = "lblExcelFile";
            this.lblExcelFile.Size = new System.Drawing.Size(71, 12);
            this.lblExcelFile.TabIndex = 8;
            this.lblExcelFile.Text = "Excel文件：";
            // 
            // btnSendExcel
            // 
            this.btnSendExcel.Location = new System.Drawing.Point(83, 206);
            this.btnSendExcel.Name = "btnSendExcel";
            this.btnSendExcel.Size = new System.Drawing.Size(75, 23);
            this.btnSendExcel.TabIndex = 12;
            this.btnSendExcel.Text = "立即发送";
            this.btnSendExcel.UseVisualStyleBackColor = true;
            this.btnSendExcel.Click += new System.EventHandler(this.btnSendExcel_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkGeneralDuplicate);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.txtGeneralContent);
            this.tabPage3.Controls.Add(this.lblGeneralMessage);
            this.tabPage3.Controls.Add(this.pictureBox1);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.btnBrowseGeneral);
            this.tabPage3.Controls.Add(this.txtGeneralExcel);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.btnGeneralSend);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(504, 235);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "统一群发";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkGeneralDuplicate
            // 
            this.chkGeneralDuplicate.AutoSize = true;
            this.chkGeneralDuplicate.Checked = true;
            this.chkGeneralDuplicate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGeneralDuplicate.Location = new System.Drawing.Point(315, 47);
            this.chkGeneralDuplicate.Name = "chkGeneralDuplicate";
            this.chkGeneralDuplicate.Size = new System.Drawing.Size(108, 16);
            this.chkGeneralDuplicate.TabIndex = 18;
            this.chkGeneralDuplicate.Text = "过滤重复手机号";
            this.chkGeneralDuplicate.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(312, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 12);
            this.label8.TabIndex = 22;
            this.label8.Text = "Excel 格式示例：";
            // 
            // txtGeneralContent
            // 
            this.txtGeneralContent.Location = new System.Drawing.Point(83, 44);
            this.txtGeneralContent.Multiline = true;
            this.txtGeneralContent.Name = "txtGeneralContent";
            this.txtGeneralContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtGeneralContent.Size = new System.Drawing.Size(223, 156);
            this.txtGeneralContent.TabIndex = 17;
            this.txtGeneralContent.Tag = "GeneralContent";
            this.txtGeneralContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            // 
            // lblGeneralMessage
            // 
            this.lblGeneralMessage.AutoSize = true;
            this.lblGeneralMessage.Location = new System.Drawing.Point(173, 211);
            this.lblGeneralMessage.Name = "lblGeneralMessage";
            this.lblGeneralMessage.Size = new System.Drawing.Size(89, 12);
            this.lblGeneralMessage.TabIndex = 20;
            this.lblGeneralMessage.Text = "Return Message";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LiveAzure.Tools.Message.Properties.Resources.Pending;
            this.pictureBox1.Location = new System.Drawing.Point(312, 96);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(178, 104);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "短信内容：";
            // 
            // btnBrowseGeneral
            // 
            this.btnBrowseGeneral.Location = new System.Drawing.Point(415, 12);
            this.btnBrowseGeneral.Name = "btnBrowseGeneral";
            this.btnBrowseGeneral.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseGeneral.TabIndex = 16;
            this.btnBrowseGeneral.Tag = "GeneralSend";
            this.btnBrowseGeneral.Text = "浏览";
            this.btnBrowseGeneral.UseVisualStyleBackColor = true;
            this.btnBrowseGeneral.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // txtGeneralExcel
            // 
            this.txtGeneralExcel.Location = new System.Drawing.Point(83, 14);
            this.txtGeneralExcel.Name = "txtGeneralExcel";
            this.txtGeneralExcel.Size = new System.Drawing.Size(310, 21);
            this.txtGeneralExcel.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "Excel文件：";
            // 
            // btnGeneralSend
            // 
            this.btnGeneralSend.Location = new System.Drawing.Point(83, 206);
            this.btnGeneralSend.Name = "btnGeneralSend";
            this.btnGeneralSend.Size = new System.Drawing.Size(75, 23);
            this.btnGeneralSend.TabIndex = 19;
            this.btnGeneralSend.Text = "立即发送";
            this.btnGeneralSend.UseVisualStyleBackColor = true;
            this.btnGeneralSend.Click += new System.EventHandler(this.btnGeneralSend_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.btnBrowseData);
            this.tabPage4.Controls.Add(this.grpAccountInfo);
            this.tabPage4.Controls.Add(this.btnGetMessage);
            this.tabPage4.Controls.Add(this.lblViewMessage);
            this.tabPage4.Controls.Add(this.btnViewData);
            this.tabPage4.Controls.Add(this.txtStorage);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(504, 235);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "系统配置";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSessionKey);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSerialNumber);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 117);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置信息";
            // 
            // txtSessionKey
            // 
            this.txtSessionKey.Location = new System.Drawing.Point(109, 81);
            this.txtSessionKey.Name = "txtSessionKey";
            this.txtSessionKey.PasswordChar = '*';
            this.txtSessionKey.Size = new System.Drawing.Size(185, 21);
            this.txtSessionKey.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "Session Key:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(109, 54);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(185, 21);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtSerialNumber_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 24;
            this.label3.Text = "Password:";
            // 
            // txtSerialNumber
            // 
            this.txtSerialNumber.Location = new System.Drawing.Point(109, 27);
            this.txtSerialNumber.Name = "txtSerialNumber";
            this.txtSerialNumber.Size = new System.Drawing.Size(185, 21);
            this.txtSerialNumber.TabIndex = 2;
            this.txtSerialNumber.TextChanged += new System.EventHandler(this.txtSerialNumber_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "Serial Number:";
            // 
            // btnBrowseData
            // 
            this.btnBrowseData.Location = new System.Drawing.Point(395, 140);
            this.btnBrowseData.Name = "btnBrowseData";
            this.btnBrowseData.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseData.TabIndex = 9;
            this.btnBrowseData.Tag = "StoreFile";
            this.btnBrowseData.Text = "浏览";
            this.btnBrowseData.UseVisualStyleBackColor = true;
            this.btnBrowseData.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // grpAccountInfo
            // 
            this.grpAccountInfo.Controls.Add(this.txtPrice);
            this.grpAccountInfo.Controls.Add(this.txtBalance);
            this.grpAccountInfo.Controls.Add(this.label7);
            this.grpAccountInfo.Controls.Add(this.label6);
            this.grpAccountInfo.Location = new System.Drawing.Point(327, 12);
            this.grpAccountInfo.Name = "grpAccountInfo";
            this.grpAccountInfo.Size = new System.Drawing.Size(159, 75);
            this.grpAccountInfo.TabIndex = 5;
            this.grpAccountInfo.TabStop = false;
            this.grpAccountInfo.Text = "账户信息";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(68, 45);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.ReadOnly = true;
            this.txtPrice.Size = new System.Drawing.Size(63, 21);
            this.txtPrice.TabIndex = 7;
            this.txtPrice.TabStop = false;
            // 
            // txtBalance
            // 
            this.txtBalance.Location = new System.Drawing.Point(68, 18);
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.ReadOnly = true;
            this.txtBalance.Size = new System.Drawing.Size(63, 21);
            this.txtBalance.TabIndex = 6;
            this.txtBalance.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "单价：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "余额：";
            // 
            // btnGetMessage
            // 
            this.btnGetMessage.Location = new System.Drawing.Point(165, 206);
            this.btnGetMessage.Name = "btnGetMessage";
            this.btnGetMessage.Size = new System.Drawing.Size(75, 23);
            this.btnGetMessage.TabIndex = 11;
            this.btnGetMessage.Text = "刷新余额";
            this.btnGetMessage.UseVisualStyleBackColor = true;
            this.btnGetMessage.Click += new System.EventHandler(this.btnGetMessage_Click);
            // 
            // lblViewMessage
            // 
            this.lblViewMessage.AutoSize = true;
            this.lblViewMessage.Location = new System.Drawing.Point(258, 211);
            this.lblViewMessage.Name = "lblViewMessage";
            this.lblViewMessage.Size = new System.Drawing.Size(89, 12);
            this.lblViewMessage.TabIndex = 27;
            this.lblViewMessage.Text = "Return Message";
            // 
            // btnViewData
            // 
            this.btnViewData.Location = new System.Drawing.Point(83, 206);
            this.btnViewData.Name = "btnViewData";
            this.btnViewData.Size = new System.Drawing.Size(75, 23);
            this.btnViewData.TabIndex = 10;
            this.btnViewData.Text = "查看数据";
            this.btnViewData.UseVisualStyleBackColor = true;
            this.btnViewData.Click += new System.EventHandler(this.btnViewData_Click);
            // 
            // txtStorage
            // 
            this.txtStorage.Location = new System.Drawing.Point(85, 142);
            this.txtStorage.Name = "txtStorage";
            this.txtStorage.ReadOnly = true;
            this.txtStorage.Size = new System.Drawing.Size(304, 21);
            this.txtStorage.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 22;
            this.label5.Text = "数据记录：";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.txtHelp);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(504, 235);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "帮助说明";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // txtHelp
            // 
            this.txtHelp.Location = new System.Drawing.Point(3, 6);
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.ReadOnly = true;
            this.txtHelp.Size = new System.Drawing.Size(498, 226);
            this.txtHelp.TabIndex = 1;
            this.txtHelp.Text = resources.GetString("txtHelp.Text");
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("楷体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(14, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(186, 21);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "手机短信群发工具";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(228, 33);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(113, 12);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "ver: 1.0.2011.0524";
            // 
            // frmShortMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 336);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmShortMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "手机短信群发";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmShortMessage_FormClosed);
            this.Load += new System.EventHandler(this.frmShortMessage_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgExcelFormat)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpAccountInfo.ResumeLayout(false);
            this.grpAccountInfo.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.Label lblReceiver;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSendSingle;
        private System.Windows.Forms.Label lblSingleMessage;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblExcelMessage;
        private System.Windows.Forms.PictureBox imgExcelFormat;
        private System.Windows.Forms.Label lblExcelFormat;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.TextBox txtExcelFile;
        private System.Windows.Forms.Label lblExcelFile;
        private System.Windows.Forms.Button btnSendExcel;
        private System.Windows.Forms.Label lblGeneralMessage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnBrowseGeneral;
        private System.Windows.Forms.TextBox txtGeneralExcel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnGeneralSend;
        private System.Windows.Forms.Button btnBrowseData;
        private System.Windows.Forms.GroupBox grpAccountInfo;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnGetMessage;
        private System.Windows.Forms.Label lblViewMessage;
        private System.Windows.Forms.Button btnViewData;
        private System.Windows.Forms.TextBox txtStorage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.RichTextBox txtHelp;
        private System.Windows.Forms.TextBox txtGeneralContent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSessionKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSerialNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkGeneralDuplicate;
        private System.Windows.Forms.CheckBox chkExcelDuplicate;
    }
}

