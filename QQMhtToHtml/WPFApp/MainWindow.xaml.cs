using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace QQMhtToHtml {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_selectPath1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "mht文件(*.mht)|*.mht";
            openFileDialog.ShowDialog();

            var path = openFileDialog.FileName;
            mhtPathTextBox.Text = path;
        }

        private void Button_selectPath2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

            folder.ShowDialog();//打开文件夹会话

            var path = folder.SelectedPath;//获取文件夹路径
            outputPathTextBox.Text = path;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                if (path == "")
                {
                    return;
                }

                if (System.IO.Path.GetExtension(path).ToLower() != ".mht")
                {
                    return;
                }
                mhtPathTextBox.Text = path;
                try
                {
                    outputPathTextBox.Text = System.IO.Path.GetDirectoryName(path);
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            string path = mhtPathTextBox.Text.Trim();

            //生成HTML的正文,第二步进行
            bool IsGetHtml;

            //生成消息附件中的图片,第一步进行
            bool IsGetImg;

            string OutputPath = @"";
            if (checkBox_tosourcedir.IsChecked.Value)
            {
                try
                {
                    outputPathTextBox.Text = System.IO.Path.GetDirectoryName(path);
                }
                catch (Exception)
                {
                    ;
                }
            }
            OutputPath = outputPathTextBox.Text.Trim()+"\\";

            string ImgDirName = "img";
            ImgDirName = textBox_imgdirname.Text.Trim();

            //多少条记录换一个新文件继续输出
            int FileLength = 50000;

            if (path == "" || !File.Exists(path))
            {
                MessageBox.Show("请输入正确的路径，您也可以将文件拖放到窗口。");
                return;
            }
            boader.IsEnabled = false;

            try
            {
                FileLength = Int32.Parse(textBox_filelength.Text);
            }
            catch
            {
                FileLength = 0;
            }


            QqMhtToHtml.FileLength = FileLength;
            QqMhtToHtml.ImgDirName = ImgDirName;

            QqMhtToHtml.OutputPath = OutputPath;

            if (checkBox_outputPic.IsChecked.Value)
            {
                QqMhtToHtml.IsGetImg = true;
                QqMhtToHtml.IsGetHtml = false;
                QqMhtToHtml.DoConvert(path);
            }

            QqMhtToHtml.IsGetImg = false;
            QqMhtToHtml.IsGetHtml = true;
            QqMhtToHtml.DoConvert(path);

            if (checkBox_opendir.IsChecked.Value)
            {
                ExplorePath(OutputPath);
            }

            if (checkBox_openwebbroswer.IsChecked.Value)
            {
                System.Diagnostics.Process.Start("file://" +
                    OutputPath + System.IO.Path.GetFileNameWithoutExtension(path) + ".html");
            }


            if (checkBox_del.IsChecked.Value)
            {
                File.Delete(path);
            }

            MessageBox.Show("任务完成了哈。");
            boader.IsEnabled = true;
        }
        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            image.Source = 
            Imaging.CreateBitmapSourceFromHBitmap
                (Properties.Resources.p3.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
