# QnABotOnWechat
A sample to integrate with QnA Maker and Wechat public account 10分钟搭建微信公众账号FAQ机器人


## QnA Maker是什么？

QnA Maker一个定制化的免费问答机器人服务，用户可以提供一系列的“问题-答案”对（QA pair），然后 QnA Maker 服务会生成一个 API，用户输入询问的问题，API 会返回匹配的答案。值得注意的是，这个 API 采用了一些自然语言处理技术，能够应对语言表达上的模糊：变换着句式提问，甚至将一些词语用同义词替代，都能得到很好的匹配正确率。更多内容请访问：https://qnamaker.ai/

## 前期准备

1. 注册一个微信公众号，个人订阅即可
2. 下载VS 2017, 安装ASP.NET 模块
3. 注册一个Azure的账号
4. 注册QnA Maker

## 创建一个QnA Maker服务

1. 新建服务
2. 填写一个常见问题地址（比如：https://www.azure.cn/support/faq/），或者是自己上传文件，或者自己编
3. 然后点击Publish，记下knowledge base id和key

## 创建API

创建.net core web api, 默认的Value控制器就可以了。
可以下载本项目工程，也可以自己尝试创建。本代码基于.net core 2.0，所以需要VS 2017

主要完成两个内容：
1. GET api完成对微信服务器的认证   GET /api/values
2. POST api完成对用户提问问题的回复 POST /api/values

贴上本工程的代码，点击发布到Azure Web Service。
记下你的发布地址。类似：YOUR_WEB_APP_NAME.azurewebsites.net

## 在微信公众平台注册账号，创建一个个人的订阅号就可以了。

1. 服务器地址，填写你发布到Azure Web service的地址（类似：YOUR_WEB_APP_NAME.azurewebsites.net/api/values）
2. 填写Token, 可以随便写，但是需要跟代码中保持一致。

验证通过之后就大功告成啦！ 关注你自己的公众号试一下吧。
