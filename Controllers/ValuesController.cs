using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace QnAMakerOnWechat.Controllers
{

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Get(string signature, string timestamp, string nonce, string echostr)
        {
            string[] ArrTmp = { "微信上你配置的token", timestamp, nonce };
            //字典排序
            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);
            //字符加密
            var sha1 = HmacSha1Sign(tmpStr);
            if (sha1.Equals(signature))
            {
                return echostr;
            }
            else
            {
                return null;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            
            string input = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }


            string strCreateTime = "0";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);
            XmlNodeList listFromUserName = doc.GetElementsByTagName("FromUserName");
            string requestFromUserID = listFromUserName[0].InnerText;
            XmlNodeList listToUserName = doc.GetElementsByTagName("ToUserName");
            string requestToUserID = listToUserName[0].InnerText;


            XmlNodeList listCreateTime = doc.GetElementsByTagName("CreateTime");
            strCreateTime = listCreateTime[0].InnerText;

            XmlNodeList listContent = doc.GetElementsByTagName("Content");

            string answer = QueryBotApi(listContent[0].InnerText); // API query



            string strRespose = "<xml>";
            strRespose += "<ToUserName><![CDATA[{0}]]></ToUserName>";
            strRespose += "<FromUserName><![CDATA[{1}]]></FromUserName>";
            strRespose += "<CreateTime>{2}</CreateTime>";
            strRespose += "<MsgType><![CDATA[text]]></MsgType>";
            strRespose += "<Content><![CDATA[{3}]]></Content>";
            strRespose += "</xml>";

            string result = string.Format(strRespose, requestFromUserID, requestToUserID, DateTime.Now.ToBinary(), answer);

            return Content(result);
 
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        public string HmacSha1Sign(string str)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.Default.GetBytes(str));
            string byte2String = null;
            for (int i = 0; i < hash.Length; i++)
            {
                byte2String += hash[i].ToString("x2");
            }
            return byte2String;
        }


        public class AnswersItem
        {

    /// </summary>
            public string answer { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> questions { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double score { get; set; }
        }

        public class QnAMakerResult
        {
            /// <summary>
            /// 
            /// </summary>
            public List<AnswersItem> answers { get; set; }
        }


        private string QueryBotApi(string query)
        {
            string responseString = string.Empty;

            var knowledgebaseId = "这个ID在qnamaker的网站上找"; // Use knowledge base id created.
            var qnamakerSubscriptionKey = "这个key也在qnamaker的网站上找"; //Use subscription key assigned to you.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }

            QnAMakerResult response;
            string responseText = null;
            try
            {
                response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
                if (response.answers==null || response.answers.Count <=0 || response.answers[0].answer == "No good match found in the KB" || response.answers[0].score < 0.2)
                {
                    responseText = "这个问题我还不会回答。";
                }
                else
                {
                    responseText = response.answers[0].answer;
                }
            }
            catch
            {
                responseText = null;
            }

            return responseText;
        }

    }
}
