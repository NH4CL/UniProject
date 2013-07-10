using System.Windows;
using System.Windows.Input;
using LiveAzure.Utility;

namespace LiveAzure.Tools.Encrypt
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑，加密字符串工具
    /// </summary>
    public partial class MainWindow : Window
    {

        private int nSystem = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEncode_Click(object sender, RoutedEventArgs e)
        {
            if (nSystem == 0)
            {
                if (rdoKey0.IsChecked == true)
                    txtTarget.Text = CommonHelper.EncryptDES(txtSource.Text, ConfigHelper.EncodeKey.EncryptKeys[0]);
                else if (rdoKey1.IsChecked == true)
                    txtTarget.Text = CommonHelper.EncryptDES(txtSource.Text, ConfigHelper.EncodeKey.EncryptKeys[1]);
                else if (rdoKey2.IsChecked == true)
                    txtTarget.Text = CommonHelper.EncryptDES(txtSource.Text, ConfigHelper.EncodeKey.EncryptKeys[2]);
                else if (rdoKey3.IsChecked == true)
                    txtTarget.Text = CommonHelper.EncryptDES(txtSource.Text, txtSaltKey.Text.Trim());
            }
            else if (nSystem == 1)
            {
                if (rdoKey0.IsChecked == true)
                    txtTarget.Text = CommonHelper.DecryptDES(txtSource.Text, ConfigHelper.EncodeKey.EncryptKeys[0]);
                else if (rdoKey1.IsChecked == true)
                    txtTarget.Text = CommonHelper.DecryptDES(txtSource.Text, ConfigHelper.EncodeKey.EncryptKeys[1]);
                else if (rdoKey2.IsChecked == true)
                    txtTarget.Text = CommonHelper.DecryptDES(txtSource.Text, ConfigHelper.EncodeKey.EncryptKeys[2]);
                else if (rdoKey3.IsChecked == true)
                    txtTarget.Text = CommonHelper.DecryptDES(txtSource.Text, txtSaltKey.Text.Trim());
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(txtTarget.Text, true);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void rdoKey3_Click(object sender, RoutedEventArgs e)
        {
            txtSaltKey.IsEnabled = (bool)rdoKey3.IsChecked;
        }

        private void imgLogo_MouseEnter(object sender, MouseEventArgs e)
        {
            nSystem = 1;
        }

        private void imgLogo_MouseLeave(object sender, MouseEventArgs e)
        {
            nSystem = 0;
        }

    }
}
