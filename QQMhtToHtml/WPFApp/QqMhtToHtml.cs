using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QQMhtToHtml {
    class QqMhtToHtml {
        public const string HtmlHeadString = @"<html xmlns=""http://www.w3.org/1999/xhtml"">\n"
                                           + @"<head>\n"
                                           + @"<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />\n"
                                           + @"<title>QQ Message</title>"
                                           + @"<style type=""text/css"">\n"
                                           + @"body{font-size:12px; line-height:22px; margin:2px;}td{font-size:12px; line-height:22px;}\n"
                                           + @"</style>\n"
                                           + @"</head>\n"
                                           + @"<body><table width=100% cellspacing=0>\n";
        public const string HtmlEndString = @"</table>\n</body>\n</html>";

        //生成HTML的正文,第二步进行
        public static bool IsGetHtml = false;

        //生成消息附件中的图片,第一步进行
        public static bool IsGetImg = false;

        //保存图片ID和后缀的对应关系
        public static Dictionary<string, string> ImgDictionary = new Dictionary<string, string>();

        public static string OutputPath = "";

        public static string ImgDirName = "img";

        //多少条记录换一个新文件继续输出
        public static int FileLength = 50000;

        //进行MHT转HTML的消息记录转化
        public static void DoConvert(string strSrcFilePath)
        {
            string strSuffix = "";
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

            if (OutputPath != "" && !Directory.Exists(OutputPath)) Directory.CreateDirectory(OutputPath);

            if (IsGetHtml)
            {
                fsHtml = new FileStream(OutputPath + Path.GetFileNameWithoutExtension(strSrcFilePath) + ".html", FileMode.Create);
                swHtml = new StreamWriter(fsHtml);
            }
            if (IsGetImg)
            {
                fsDic = new FileStream(OutputPath + "ImgDictionary.txt", FileMode.Create);
                swDic = new StreamWriter(fsDic);
                if (!Directory.Exists(OutputPath + ImgDirName)) Directory.CreateDirectory(OutputPath + ImgDirName);
            }

            //记录每个生成的html文件中的记录条数
            var count = 0;
            //生成多个html文件时的序号
            var fileCount = 1;
            if (IsGetHtml)
            {
                //如果是获取QQ消息文本，则事先加载图片文件字典
                if (File.Exists(OutputPath + "ImgDictionary.txt"))
                {
                    blDicExist = true;
                    FileStream fsTmp = new FileStream(OutputPath + "ImgDictionary.txt", FileMode.Open);
                    StreamReader srTmp = new StreamReader(fsTmp);
                    while (!srTmp.EndOfStream)
                    {
                        var strTmpLine = srTmp.ReadLine();
                        if (strTmpLine == null)
                        {
                            continue;
                        }
                        var strTmpId = strTmpLine.Substring(0, 36);
                        var strTmpSuffix = strTmpLine.Substring(37);
                        if (!ImgDictionary.ContainsKey(strTmpId)) ImgDictionary.Add(strTmpId, strTmpSuffix);
                    }
                    srTmp.Close();
                    fsTmp.Close();
                }
            }
            while (!rsSrc.EndOfStream)
            {
                var strLine = rsSrc.ReadLine().TrimEnd();
                if (IsGetHtml)
                {
                    //第2步操作,正文部分读取成HTML文件,5万行记录生成一个文件,并根据字典文件中的后缀信息生成对应URL
                    if (strLine.Contains("<tr><td><div"))
                    {
                        if (strLine.Contains("}.dat"))
                        {
                            var strImgId = strLine.Substring(strLine.IndexOf('{') + 1, 36);
                            if (blDicExist && ImgDictionary.ContainsKey(strImgId))
                            {
                                strLine = strLine.Replace("}.dat", "." + ImgDictionary[strImgId]);
                            }
                            else
                            {
                                strLine = strLine.Replace("}.dat", ".jpg");
                            }
                            strLine = strLine.Replace("src=\"{", "src=\"" + ImgDirName + "/");
                        }

                        try
                        {
                            swHtml.WriteLine(strLine);
                        }
                        catch (NullReferenceException exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        count++;
                        if (count > FileLength && FileLength > 0)
                        {
                            count = 0;
                            swHtml.WriteLine(HtmlEndString);
                            swHtml.Close();

                            try
                            {
                                fsHtml.Close();
                            }
                            catch (NullReferenceException exception)
                            {
                                Console.WriteLine(exception.Message);
                            }

                            fileCount++;
                            fsHtml = new FileStream(OutputPath + Path.GetFileNameWithoutExtension(strSrcFilePath) + " - " + fileCount.ToString() + ".html", FileMode.Create);
                            swHtml = new StreamWriter(fsHtml);
                            swHtml.WriteLine(HtmlHeadString);
                        }
                    }
                    else if (strLine.Contains(HtmlEndString))
                    {
                        try
                        {
                            swHtml.WriteLine(strLine);
                            swHtml.Close();
                            fsHtml.Close();
                        }
                        catch (NullReferenceException exception)
                        {
                            Console.WriteLine(exception.Message);
                        }

                        break;
                    }
                }
                else if (IsGetImg)
                {
                    //第1步操作,附件部分读取成相应的图片,并将图片名称和后缀信息保存成字典文件
                    if (strLine == "")
                    {
                        if (blBegin == true && blEnd == true)
                        {
                            blEnd = false;
                        }
                        else if (blBegin == true)
                        {
                            blBegin = false;
                            blEnd = true;
                            var strContent = sbSrc.ToString();
                            sbSrc.Remove(0, sbSrc.Length);
                            //保存成图片文件
                            WriteToImage(strImgFileName, strContent, strSuffix);
                            //写入到字典文件,用户读取正文时生成链接
                            swDic.WriteLine(strImgFileName + "," + strSuffix);
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
            if (IsGetImg)
            {
                try
                {
                    swDic.Close();
                    fsDic.Close();
                }
                catch (NullReferenceException exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
            if (IsGetHtml && !blDicExist)
            {
                Console.WriteLine("已完成,但未发现图片字典文件,生成的html消息记录链接会不正确,按任意键退出.......");
            }
            else
            {
                Console.WriteLine("完成,按任意键退出.......");
            }

            try
            {
                fsHtml.Close();
            }
            catch
            {
                ;
            }

        }

        //保存每个图片到对应的文件
        private static void WriteToImage(string strFileName, string strContent, string strSuffix)
        {
            byte[] byteContent = Convert.FromBase64String(strContent);
            FileStream fs = new FileStream(OutputPath + ImgDirName + "/" + strFileName + "." + strSuffix, FileMode.Create);
            fs.Write(byteContent, 0, byteContent.Length);
            fs.Close();
        }


    }
}
