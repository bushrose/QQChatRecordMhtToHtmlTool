##QQ消息记录导出的Mht格式转Html
***

图形GUI版下载地址
https://github.com/a645162/QQChatRecordMhtToHtmlTool/blob/master/QQMhtToHtml_GUI.exe?raw=true

命令行版下载地址:
https://github.com/a645162/QQChatRecordMhtToHtmlTool/blob/master/QQMhtToHtml.exe?raw=true

macOS下使用.net Core
必须同时存在这两个文件且要求安装.net Core 版本至少为2.2
    QQMhtToHtml_dNetCore_macOS.dll
	QQMhtToHtml_dNetCore_macOS.runtimeconfig.json

##孔昊旻(DearKoala)

    2019年08月02日

    致大学一年级下半学期陪伴我的月

    你我的29541条消息我都保存下来了。存到了我的iCloud永远保存。
    美好的回忆。希望你将来是个善良的人。

##介绍

    我即将大二，我是一直在备份我的qq消息记录的整个文件夹，从2011到现在，每次导出记录都是导出成bak文件，导出mht文件的话，你可以直接使用浏览器查看，非常的方便，不需要打开qq，我这种macOS做主力操作系统的人太适合了，偶尔回顾一下某个重要的人或者曾经是重要的人的聊天记录，在合适不过了。

    但是，如果你和“她”的聊天记录太多(主要是图片)，会导致mht文件非常的大，我这个i7-8850H(ps.去年还是非常高的配置)，都打不开，浏览器直接崩溃，我们只好将其转换为Html这种格式，他有个特点，不需要在打开文件的时候解码图片，提前处理完图片，而且，现在的浏览器的都是看到哪里加载那里的图片，不会出现大文件直接卡死浏览器。

    网上推荐的2009年出的软件mht2html对于这种大文件还是会报错，直到我在CSDN上看到number321大神的这段代码，如是说，要让我写，我还真没耐心。

##截图
![截图](https://github.com/a645162/QQChatRecordMhtToHtmlTool/raw/master/pic/Main.png)

##使用方法

    Windows

    将聊天记录文件放到本工具同级目录，并进入cmd：
    目录中按住shift并点击空白区域早期版本是打开cmd，新版win10是打开powershell
    1.
    QQMhtToHtml.exe QQ记录.mht getimg
    powershell命令为:
    .\QQMhtToHtml.exe ".\QQ记录.mht" getimg

    2.
    QQMhtToHtml.exe QQ记录.mht gethtml
    powershell命令为:
    .\QQMhtToHtml.exe ".\QQ记录.mht" gethtml

    这两条命令必须按顺序执行，不然会没有图片。

    ※推荐使用GUI版，即QQMhtToHtml_GUI.exe

    macOS
    将聊天记录文件放到本工具同级目录，并进入cmd：
    1.
    dotnet QQMhtToHtml_dNetCore_macOS.dll QQ记录.mht getimg

    2.
    dotnet QQMhtToHtml_dNetCore_macOS.dll QQ记录.mht gethtml

##从聊天记录导出mht的方法

    打开qq的消息管理器
    要导出的好友那里右键
![P1](https://github.com/a645162/QQChatRecordMhtToHtmlTool/raw/master/pic/p1.png)
![P2](https://github.com/a645162/QQChatRecordMhtToHtmlTool/raw/master/pic/p2.png)


##声明

    本程序核心代码来自CSDN的number321大神

    本人仅仅，因为需要，用了一下感觉很棒。
    于是，仔细制作，适配macOS的.net Core
    以及为Windows平台制作GUI界面

number321大神的帖子地址:[https://bbs.csdn.net/topics/391027030]

源代码地址:[https://download.csdn.net/download/number321/8735489]

2019年08月02日12:02:12
