using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace QQMhtToHtml {
    class Program {

        //生成HTML的正文,第二步进行
        public static bool IsGetHtml = false;

        //生成消息附件中的图片,第一步进行
        public static bool IsGetImg = false;

        public static string OutputPath = @"";

        public static string ImgDirName = "img";

        //多少条记录换一个新文件继续输出
        public static int FileLength = 50000;

        static void Main(string[] args)
        {
            
            if (args.Length == 0)
            {
                ShowHelpMsg();
            }
            else if (args.Length == 2)
            {
                if (args[1].ToLower() == "getHtml".ToLower()) IsGetHtml = true;
                if (args[1].ToLower() == "getImg".ToLower()) IsGetImg = true;
                if (IsGetHtml || IsGetImg)
                {
                    string strFilePath = args[0];
                    if (File.Exists(strFilePath))
                    {
                        QqMhtToHtml.FileLength = FileLength;
                        QqMhtToHtml.ImgDirName = ImgDirName;
                        QqMhtToHtml.IsGetHtml = IsGetHtml;
                        QqMhtToHtml.IsGetImg = IsGetImg;
                        QqMhtToHtml.OutputPath = OutputPath;

                        QqMhtToHtml.DoConvert(strFilePath);
                    }
                    else
                    {
                        Console.WriteLine("文件 " + strFilePath + " 不存在,按任意键退出");
                        Console.ReadKey();
                    }
                }
                else
                {
                    if (!IsGetHtml && !IsGetImg) Console.WriteLine("参数填写错误,按任意键退出...");
                    Console.ReadKey();
                }
            }
            else
            {
                if (!IsGetHtml && !IsGetImg) Console.WriteLine("参数数量错误,按任意键退出...");
                Console.ReadKey();
            }
        }

        //显示帮助信息
        public static void ShowHelpMsg()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("使用说明:");
            sb.AppendLine("该工具用于将较大的MHT格式的QQ记录转换成HTML文本内容和对应的图片附件,解决记录较大时无法打开的问题");
            sb.AppendLine("将该工具与聊天记录放在同一级目录，并进入命令行调用,调用步骤如下:");
            sb.AppendLine("第1步:");
            sb.AppendLine("\t" + "QQMhtToHtml.exe <QQ记录.mht> getimg");
            sb.AppendLine("\t" + "会在img目录下生成图片文件，以及图片字典文件");
            sb.AppendLine("第2步:");
            sb.AppendLine("\t" + "QQMhtToHtml.exe <QQ记录.mht> gethtml");
            sb.AppendLine("\t" + "会生成html格式的消息记录正文,每5万条记录一个文件");
            sb.AppendLine("注意:如果不按照顺序执行，会因没有字典文件导致生成的HTML消息图片不显示");
            sb.AppendLine("按任意键退出...");
            Console.WriteLine(sb.ToString());
            Console.ReadKey();
        }


    }
}
