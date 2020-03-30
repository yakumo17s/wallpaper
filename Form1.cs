using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Console;

namespace TouhouDesktop
{
    public partial class Form1 : Form
    {
        private const string paulzzhUrl = "https://img.paulzzh.tech/touhou/random?site=all";

        private string directory = @"D:\img\";
        private System.Timers.Timer timer;
        const int SPI_GETDESKWALLPAPER = 0x0073;
        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WriteLine(this.directory);
            textBox1.Text = this.directory;

        }

        private void GetCurrentWallPaper()
        {
            StringBuilder wallPaperPath = new StringBuilder("a", 50);
            //string a = "                                                                                    ";
            string[] b = Registry.CurrentUser.GetSubKeyNames();
            foreach (string c in b)
            {
                WriteLine(c);

            }
            //Registry.CurrentUser.OpenSubKey("Control Panel", new RegistryKeyPermissionCheck()).CreateSubKey("C D");
            //.OpenSubKey("Desktop").GetValue("WallPaper"));
            //myRegKey = myRegKey.OpenSubKey("Desktop");
            //Registry.CurrentUser.CreateSubKey(@"A B");
            //Registry.CurrentUser.OpenSubKey("A B", true).CreateSubKey("C D");

            //object a = Registry.CurrentUser.GetValue("Control Panel/Desktop/WallPaper");

            //if (Convert.ToBoolean(SystemParametersInfo(SPI_GETDESKWALLPAPER, 50, a, 0)))
            //{
            //    a.Replace(" ", "");a.Trim();
            //    WriteLine($"|{a.Substring(0)}|");

            //    WriteLine($"|{a}|");
            //    tabPage1.BackgroundImage = Image.FromFile(a);
            //}

        }

        private async void button1_Click(object sender, EventArgs e)
        {

            await GetPicture();

        }



        private async Task GetPicture()
        {

            using (var client = new HttpClient())
            {

                try
                {
                    // 下载图片
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    toolStripStatusLabel1.Text = "正在下载";

                    HttpResponseMessage rp = await client.GetAsync(paulzzhUrl);

                    rp.EnsureSuccessStatusCode();

                    string rp_body = await rp.Content.ReadAsStringAsync();

                    toolStripStatusLabel1.Text = $"下载完成{rp_body.Length / 1024 } KB";

                    // 读取图片资源
                    System.Drawing.Image downImage = System.Drawing.Image.FromStream(await rp.Content.ReadAsStreamAsync());

                    // 保存图片 
                    string deerory = string.Format(this.directory + @"{0}\", DateTime.Now.ToString("yyyy-MM-dd"));

                    string fileName = string.Format("{0}.png", DateTime.Now.ToString("HHmmssffff"));


                    if (!System.IO.Directory.Exists(deerory))
                    {
                        System.IO.Directory.CreateDirectory(deerory);
                    }

                    downImage.Save(deerory + fileName);

                    // 设置为tab背景
                    try
                    {
                        tabPage1.BackgroundImage = Image.FromFile(deerory + fileName);

                    }
                    catch (Exception err)
                    {
                        WriteLine(err);
                    }

                    downImage.Dispose();


                    SystemParametersInfo(20, 1, deerory + fileName, 1);

                    WriteLine("获取随机图片结束");

                    return;

                }
                catch (Exception err)
                {
                    toolStripStatusLabel1.Text = $"下载失败：{err}";
                    WriteLine(err);
                    return;
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private void menuItemCenter_Click(object sender, System.EventArgs e)
        {
            //设置墙纸显示方式
            RegistryKey myRegKey = Registry.CurrentUser.OpenSubKey("Control Panel/desktop", true);
            //赋值
            //注意：在把数值型的数据赋到注册表里面的时候，
            //如果不加引号，则该键值会成为“REG_DWORD”型；
            //如果加上引号，则该键值会成为“REG_SZ”型。
            myRegKey.SetValue("TileWallpaper", "0");
            myRegKey.SetValue("WallpaperStyle", "0");

            //关闭该项,并将改动保存到磁盘
            myRegKey.Close();

            //设置墙纸
            /* Bitmap bmpWallpaper = (Bitmap)pictureBox1.Image;
             bmpWallpaper.Save("resource.bmp", ImageFormat.Bmp); *///图片保存路径为相对路径，保存在程序的目录下
            string strSavePath = Application.StartupPath + "/resource.bmp";
            SystemParametersInfo(20, 1, strSavePath, 1);
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            // 检查文件夹是否存在
            if (!System.IO.Directory.Exists(this.directory))
            {
                System.IO.Directory.CreateDirectory(this.directory);
            }

            System.Diagnostics.Process.Start(this.directory);
        }



        private void Button3_Click(object sender, EventArgs e)
        {
            this.directory = textBox1.Text;
            toolStripStatusLabel1.Text = $"已修改图片存储目录";
        }



        private void Button4_Click(object sender, EventArgs e)
        {

            // 停止定时器
            if (!(timer is null) && timer.Enabled)
            {
                timer.Stop();
                timer.Dispose();
                toolStripStatusLabel1.Text = "已停止定时修改";
                button4.Text = "启动定时修改";
                return;
            }

            // 启动定时器
            bool rp = double.TryParse(textBox2.Text, out double time);

            WriteLine($"{rp}:{time}");

            if (rp)
            {


                timer = new System.Timers.Timer
                {
                    // 秒->毫秒
                    Interval = time * 1000,
                    //timer.Enabled = true;
                    AutoReset = false//设置是执行一次（false）还是一直执行(true)；  
                };
                timer.Start();

                timer.Elapsed += (o, a) =>
                {

                    this.Invoke(new TimerGetPictureDelegate(TimerGetPicture));

                };

                // 文本修改
                toolStripStatusLabel1.Text = "启动定时修改";
                button4.Text = "停止定时修改";
            }
            else
            {
                toolStripStatusLabel1.Text = "时间设置错误";
            }
        }

        public delegate void TimerGetPictureDelegate();

        private async void TimerGetPicture()
        {

            await GetPicture();
            // 完成任务后，启动下一次定时
            timer.Start();

            // toolStripStatusLabel1.Text = $"下次修改时间：{DateTime.Now + TimeSpan.FromSeconds(timer_interval)}";


        }
    }
}