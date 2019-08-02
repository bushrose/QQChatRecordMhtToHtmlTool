using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QQMhtToHtml
{
    class Program
    {
        public static string strHtmlHead = @"<html xmlns=""http://www.w3.org/1999/xhtml""><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" /><title>QQ Message</title><style type=""text/css"">body{font-size:12px; line-height:22px; margin:2px;}td{font-size:12px; line-height:22px;}</style></head><body><table width=100% cellspacing=0>";
        public static string strHtmlEnd = @"</table></body></html>";
        public static bool blGetHtml = false;        //生成HTML的正文,第二步进行
        public static bool blGetImg = false;         //生成消息附件中的图片,第一步进行
        public static Dictionary<string, string> dicImg = new Dictionary<string, string>(); //保存图片ID和后缀的对应关系

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                ShowHelpMsg();
            }
            else if (args.Length == 2)
            {
                if (args[1].ToLower() == "gethtml") blGetHtml = true;
                if (args[1].ToLower() == "getimg") blGetImg = true;
                if (blGetHtml || blGetImg)
                {
                    string strFilePath = args[0];
                    if (File.Exists(strFilePath))
                    {
                        DoConvert(strFilePath);
                    }
                    else
                    {
                        Console.WriteLine("文件 " + strFilePath + " 不存在,按任意键退出");
                        Console.ReadKey();
                    }
                }
                else
                {
                    if (!blGetHtml && !blGetImg) Console.WriteLine("参数填写错误,按任意键退出...");
                    Console.ReadKey();
                }
            }
            else
            {
                if (!blGetHtml && !blGetImg) Console.WriteLine("参数数量错误,按任意键退出...");
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

        //进行MHT转HTML的消息记录转化
        public static void DoConvert(string strSrcFilePath)
        {
            string strLine;
            string strSuffix = "";
            string strContent;
            string strImgFileName = "";
            bool blBegin = false;           //表示到一个附件开头的标志位
            bool blEnd = false;             //表示到一个附件结尾的标志位
            bool blDicExist = false;        //表示字典文件是否存在的标志位

            FileStream fsSrc = new FileStream(strSrcFilePath, FileMode.Open);
            StreamReader rsSrc = new StreamReader(fsSrc);
            StringBuilder sbSrc = new StringBuilder();
            FileStream fsDic = null;
            StreamWriter swDic = null;
            FileStream fsHtml = null;
            StreamWriter swHtml = null;
            StringBuilder sbHtml = null;
            if (blGetHtml)
            {
                fsHtml = new FileStream(Path.GetFileNameWithoutExtension(strSrcFilePath) + ".html", FileMode.Create);
                swHtml = new StreamWriter(fsHtml);
                sbHtml = new StringBuilder();
            }
            if (blGetImg)
            {
                fsDic = new FileStream("ImgDictionary.txt", FileMode.Create);
                swDic = new StreamWriter(fsDic);
                if (!Directory.Exists("img")) Directory.CreateDirectory("img");
            }
            int count = 0;              //记录每个生成的html文件中的记录条数
            int filecount = 1;          //生成多个html文件时的序号
            string strImgId = "";
            if (blGetHtml)
            {
                //如果是获取QQ消息文本，则事先加载图片文件字典
                if (File.Exists("ImgDictionary.txt"))
                {
                    blDicExist = true;
                    FileStream fsTmp = new FileStream("ImgDictionary.txt", FileMode.Open);
                    StreamReader srTmp = new StreamReader(fsTmp);
                    string strTmpId = "";
                    string strTmpSuffix = "";
                    string strTmpLine = "";
                    while (!srTmp.EndOfStream)
                    {
                        strTmpLine = srTmp.ReadLine();
                        strTmpId = strTmpLine.Substring(0, 36);
                        strTmpSuffix = strTmpLine.Substring(37);
                        if (!dicImg.ContainsKey(strTmpId)) dicImg.Add(strTmpId, strTmpSuffix);
                    }
                    srTmp.Close();
                    fsTmp.Close();
                }
            }
            while (!rsSrc.EndOfStream)
            {
                strLine = rsSrc.ReadLine().TrimEnd();
                if (blGetHtml)
                {
                    //第2步操作,正文部分读取成HTML文件,5万行记录生成一个文件,并根据字典文件中的后缀信息生成对应URL
                    if (strLine.Contains("<tr><td><div"))
                    {
                        if (strLine.Contains("}.dat"))
                        {
                            strImgId = strLine.Substring(strLine.IndexOf('{') + 1, 36);
                            if (blDicExist && dicImg.ContainsKey(strImgId))
                            {
                                strLine = strLine.Replace("}.dat", "." + dicImg[strImgId]);
                            }
                            else
                            {
                                strLine = strLine.Replace("}.dat", ".jpg");
                            }
                            strLine = strLine.Replace("src=\"{", "src=\"img/");
                        }
                        swHtml.WriteLine(strLine);
                        count++;
                        if (count > 50000)
                        {
                            count = 0;
                            swHtml.WriteLine(strHtmlEnd);
                            swHtml.Close();
                            fsHtml.Close();
                            filecount++;
                            fsHtml = new FileStream(Path.GetFileNameWithoutExtension(strSrcFilePath) + "-" + filecount.ToString() + ".html", FileMode.Create);
                            swHtml = new StreamWriter(fsHtml);
                            swHtml.WriteLine(strHtmlHead);
                        }
                    }
                    else if (strLine.Contains(strHtmlEnd))
                    {
                        swHtml.WriteLine(strLine);
                        swHtml.Close();
                        fsHtml.Close();
                        break;
                    }
                }
                else if (blGetImg)
                {
                    //第1步操作,附件部分读取成相应的图片,并将图片名称和后缀信息保存成字典文件
                    if (strLine == "")
                    {
                        if (blBegin == true && blEnd == true)
                        {
                            blEnd = false;
                        }
                        else if (blBegin == true && blEnd == false)
                        {
                            blBegin = false;
                            blEnd = true;
                            strContent = sbSrc.ToString();
                            sbSrc.Remove(0, sbSrc.Length);
                            WriteToImage(strImgFileName, strContent, strSuffix);    //保存成图片文件
                            swDic.WriteLine(strImgFileName + "," + strSuffix);  //写入到字典文件,用户读取正文时生成链接
                        }
                    }
                    else if (strLine.Contains("Content-Location:"))
                    {
                        blBegin = true;
                        strImgFileName = strLine.Substring(18, 36);
                    }
                    else if (strLine.Contains("Content-Type:image/"))
                    {
                        strSuffix = strLine.Replace("Content-Type:image/", "");
                    }
                    else if (blBegin == true)
                    {
                        sbSrc.Append(strLine);
                    }
                }
            }
            rsSrc.Close();
            fsSrc.Close();
            if (blGetImg)
            {
                swDic.Close();
                fsDic.Close();
            }
            if (blGetHtml && !blDicExist)
            {
                Console.WriteLine("已完成,但未发现图片字典文件,生成的html消息记录链接会不正确,按任意键退出.......");
            }
            else
            {
                Console.WriteLine("完成,按任意键退出.......");
            }
            Console.ReadKey();
        }

        //保存每个图片到对应的文件
        private static void WriteToImage(string strFileName, string strContent, string strSuffix)
        {
            byte[] byteContent = Convert.FromBase64String(strContent);
            FileStream fs = new FileStream(@"img\" + strFileName + "." + strSuffix, FileMode.Create);
            fs.Write(byteContent, 0, byteContent.Length);
            fs.Close();
        }


    }
}
