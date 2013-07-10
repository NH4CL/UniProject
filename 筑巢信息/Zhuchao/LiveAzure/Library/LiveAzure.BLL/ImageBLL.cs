using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Utility;
using LiveAzure.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace LiveAzure.BLL
{
    /// <summary>
    /// 图片尺寸类型枚举
    /// </summary>
    public enum ImageSizeEnum
    {
        Small = 0,
        Middle = 1,
        Large = 2
    }

    /// <summary>
    /// 关于图像尺寸的操作类
    /// </summary>
    public class ImageSize
    {
        /// <summary>
        /// 表示宽度的数组下标
        /// </summary>
        public const int WIDTH = 0;
        /// <summary>
        /// 表示高度的数组下标
        /// </summary>
        public const int HEIGHT = 1;
        /// <summary>
        /// 默认尺寸数组，仅在读取配置文件失败的时候调用
        /// </summary>
        public static readonly int[,] DefaultSize = new int[3, 2] 
        {
            {60, 60},
            {100, 100},
            {600, 600}
        };
        /// <summary>
        /// 尺寸类型
        /// </summary>
        private ImageSizeEnum _Size;

        /// <summary>
        /// 获取尺寸类型
        /// </summary>
        public ImageSizeEnum Size
        {
            get { return _Size; }
        }

        /// <summary>
        /// 获取高度配置项名称
        /// </summary>
        public string WidthSettingName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("img_");
                sb.Append(_Size);
                sb.Append("Width");
                return sb.ToString();
            }
        }

        /// <summary>
        /// 获取宽度配置项名称
        /// </summary>
        public string HeightSettingName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("img_");
                sb.Append(_Size);
                sb.Append("Height");
                return sb.ToString();
            }
        }

        /// <summary>
        /// 获取宽度值
        /// </summary>
        public int Width
        {
            get
            {
                int value;
                if (Int32.TryParse(ConfigHelper.GlobalConst.GetSetting(WidthSettingName), out value))
                    return value;
                else
                    return DefaultSize[(int)Size, WIDTH];
            }
        }

        /// <summary>
        /// 获取高度值
        /// </summary>
        public int Height
        {
            get
            {
                int value;
                if (Int32.TryParse(ConfigHelper.GlobalConst.GetSetting(HeightSettingName), out value))
                    return value;
                else
                    return DefaultSize[(int)Size, HEIGHT];
            }
        }

        /// <summary>
        /// 获取尺寸的完整名称字符串，表示为 (Width)X(Height)
        /// </summary>
        public string SizeName
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Width);
                sb.Append('X');
                sb.Append(Height);
                return sb.ToString();
            }
        }

        /// <summary>
        /// 构造新的图像尺寸
        /// </summary>
        /// <param name="size">尺寸枚举</param>
        public ImageSize(ImageSizeEnum size)
        {
            _Size = size;
        }

        /// <summary>
        /// 构造新的图像尺寸
        /// </summary>
        /// <param name="strSize">尺寸枚举字符串</param>
        public ImageSize(string strSize)
        {
            ImageSizeEnum size = (ImageSizeEnum)Enum.Parse(typeof(ImageSizeEnum), strSize);
            _Size = size;
        }

        /// <summary>
        /// 将尺寸配置转换为<seealso cref="ListItem"/>对象
        /// </summary>
        /// <param name="selected">是否选中</param>
        /// <returns></returns>
        public ListItem ToListItem(bool selected = false)
        {
            ListItem item = new ListItem
            {
                Text = SizeName,
                Value = Enum.GetName(typeof(ImageSizeEnum), _Size),
                Selected = selected
            };
            return item;
        }

        /// <summary>
        /// 获取所有尺寸枚举列表
        /// </summary>
        public static List<ImageSizeEnum> SizeEnumList
        {
            get
            {
                int count = Enum.GetNames(typeof(ImageSizeEnum)).Count();
                List<ImageSizeEnum> list = new List<ImageSizeEnum>();
                for (int i = 0; i < count; i++)
                    list.Add((ImageSizeEnum)i);
                return list;
            }
        }

        /// <summary>
        /// 获取所有尺寸对象列表
        /// </summary>
        public static IEnumerable<ImageSize> SizeList
        {
            get
            {
                return SizeEnumList.Select(e => new ImageSize(e));
            }
        }

        /// <summary>
        /// 获取所有尺寸<seealso cref="ListItem"/>对象集合
        /// </summary>
        /// <param name="selectedIndex">选中索引</param>
        /// <returns></returns>
        public static IEnumerable<ListItem> GetSizeInfoList(int selectedIndex = 0)
        {
            IEnumerable<ListItem> list = SizeList.Select(size =>
                size.ToListItem((int)size.Size == selectedIndex));
            return list;
        }
    }

    /// <summary>
    /// 关于组织图像虚路径的处理类
    /// </summary>
    public class OrgImage
    {
        /// <summary>
        /// 获取组织代码
        /// </summary>
        public string OrganizationCode { get { return _OrganizationCode; } }

        /// <summary>
        /// 组织代码
        /// </summary>
        private string _OrganizationCode;

        /// <summary>
        /// 获取图像扩展名
        /// </summary>
        public static string ImageExtension { get { return _ImageExtension; } }

        /// <summary>
        /// 图像扩展名
        /// </summary>
        private static string _ImageExtension = ".jpg";

        /// <summary>
        /// 获取和设置图像虚拟根目录
        /// </summary>
        public static string RootDirURL
        { 
            get { return _RootDirURL; }
            set { _RootDirURL = value; }
        }

        /// <summary>
        /// 图像虚拟根目录
        /// </summary>
        private static string _RootDirURL = "~/Img";

        /// <summary>
        /// 获取和设置图像磁盘根目录
        /// </summary>
        public static string RootDirPath
        {
            get { return _RootDirPath; }
            set { _RootDirPath = value; }
        }

        /// <summary>
        /// 图像磁盘根目录
        /// </summary>
        private static string _RootDirPath = @"C:\Img";

        /// <summary>
        /// 获取组织图片虚拟根目录
        /// </summary>
        public string OrgRootDirURL
        {
            get
            {
                return Path.Combine(_RootDirURL, _OrganizationCode);
            }
        }

        /// <summary>
        /// 获取组织图片磁盘根目录
        /// </summary>
        public string OrgRootDirPath
        {
            get
            {
                return Path.Combine(_RootDirPath, _OrganizationCode);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="orgCode">组织代码</param>
        public OrgImage(string orgCode)
        {
            _OrganizationCode = orgCode;
        }

        public IEnumerable<string> GetSkuImageNames(string skuCode)
        {
            List<string> names = new List<string>();
            int i = -1;
            string orgRootDirPath = OrgRootDirPath;
            while (true)
            {
                i++;
                string fileName = string.Format("{0}_{1:00}{2}", skuCode, i, _ImageExtension);
                string path = Path.Combine(orgRootDirPath, fileName);
                if (File.Exists(path))
                    names.Add(fileName);
                else
                    break;
            }
            return names;
        }

        public IEnumerable<string> GetSkuImageURLs(string skuCode)
        {
            string orgRootDirURL = OrgRootDirURL;
            IEnumerable<string> names = GetSkuImageNames(skuCode);
            return names.Select(name => Path.Combine(orgRootDirURL, name));
        }

        public IEnumerable<string> GetSkuImagePaths(string skuCode)
        {
            string orgRootDirPath = OrgRootDirPath;
            IEnumerable<string> names = GetSkuImageNames(skuCode);
            return names.Select(name => Path.Combine(orgRootDirPath, name));
        }

        /// <summary>
        /// 获取Sku对应尺寸的图片文件名
        /// </summary>
        /// <param name="skuCode">SkuCode</param>
        /// <param name="size">图片尺寸</param>
        /// <returns></returns>
        public IEnumerable<string> GetSkuImageNames(string skuCode, ImageSizeEnum size)
        {
            string sizeName = Enum.GetName(typeof(ImageSizeEnum), size);
            List<string> names = new List<string>();
            string orgRootDirPath = OrgRootDirPath;
            int length = GetSkuImageNames(skuCode).Count();
            for (int i = 0; i < length; i++)
            {
                string fileName = string.Format("{0}_{1:00}_{2}{3}", skuCode, i, sizeName, _ImageExtension);
                string path = Path.Combine(orgRootDirPath, fileName);
                names.Add(fileName);
            }
            return names;
        }

        /// <summary>
        /// 获取Sku对应尺寸的图片虚路径
        /// </summary>
        /// <param name="skuCode">SkuCode</param>
        /// <param name="size">图片尺寸</param>
        /// <returns></returns>
        public IEnumerable<string> GetSkuImageURLs(string skuCode, ImageSizeEnum size)
        {
            string orgRootDirURL = OrgRootDirURL;
            IEnumerable<string> names = GetSkuImageNames(skuCode, size);
            return names.Select(name => Path.Combine(orgRootDirURL, name));
        }

        public IEnumerable<string> GetSkuImagePaths(string skuCode, ImageSizeEnum size)
        {
            string orgRootDirPath = OrgRootDirPath;
            IEnumerable<string> names = GetSkuImageNames(skuCode, size);
            return names.Select(name => Path.Combine(orgRootDirPath, name));
        }
    }

    public class ImageResize
    {
        /// <summary>
        /// 修改图像尺寸并输出图像
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">处理后的缩略图路径（物理路径）</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="nWidth">缩略图宽度</param>
        /// <returns></returns>
        public static void Resize(string originalImagePath, string thumbnailPath, int nHeight, int nWidth)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            //new bmp image
            Image bitmap = new Bitmap(nWidth, nHeight);
            //new graphics
            Graphics g = Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, nWidth, nHeight),
                new Rectangle(0, 0, originalImage.Width, originalImage.Height), GraphicsUnit.Pixel);
            //以jpg格式保存缩略图
            bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
        }
    }
}
