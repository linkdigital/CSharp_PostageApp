using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Security.Cryptography;

namespace PostageApp
{
    /**
    * PostageApp Class
    *
    * Permits email to be sent via PostageApp service
    *
    * @package PostageApp
    * @author David Jackson, Link Digital (http://www.linkdigital.ca)
    * @link http://postageapp.com
    */
    public class PostageApp
    {
        public string api_key;
        public bool secure;
        public string host = "api.postageapp.com";
        public string version = "1.0.0";
        public string recipient_override;
        private Dictionary<string, object> arguments = new Dictionary<string, object>();

        /**
        * Constructor - Sets PostageApp Preferences
        *
        * The constructor can be passed an array of config values
        */
        public PostageApp(string _api_key, bool _secure = true) {
            api_key = _api_key;
            secure = _secure;
        }

        /**
        * Setting arbitrary message headers. You may set from, subject, etc here
        *
        * @access  public
        * @return  void
        */
        public void headers(Dictionary<string, string> headers)
        {
            arguments.Add("headers", headers);
        }

        /**
        * Setting Subject Header
        *
        * @access  public
        * @return  void
        */
        public void subject(string subject)
        {
            if (arguments.ContainsKey("headers"))
            {
                Dictionary<string, string> headers = (Dictionary<string, string>) arguments["headers"];
                if (headers.ContainsKey("subject"))
                {
                    headers["subject"] = subject;
                }
                else
                {
                    headers.Add("subject", subject);
                }
                arguments["headers"] = headers;
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("subject", subject);
                arguments.Add("headers", headers);
            }
        }

        /**
        * Setting From header
        *
        * @access  public
        * @return  void
        */
        public void from(string from)
        {
            if (arguments.ContainsKey("headers"))
            {
                Dictionary<string, string> headers = (Dictionary<string, string>)arguments["headers"];
                if (headers.ContainsKey("from"))
                {
                    headers["from"] = from;
                }
                else
                {
                    headers.Add("from", from);
                }
                arguments["headers"] = headers;
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("from", from);
                arguments.Add("headers", headers);
            }
        }

        /**
        * Setting Recipients. Accepted formats for $to are (see API docs):
        *   a single recipient -> 'recipient@example.com'
        *   a single recipient -> 'John Doe <recipient@example.com>'
        *   multiple recipients who can see each others email addresses -> 'recipient1@example.com, recipient2@example.com'
        * @access  public
        * @return  void
        */
        public void to(string to)
        {
            arguments.Add("recipients", to);
        }

        /**
        * Setting Recipients. Accepted formats for $to are (see API docs):
        *  multiple recipients who can't see each other email addresses
        *     List<string> recipients = new List<string>();
        *     recipients.Add("recipient1@example.com");
        *     recipients.Add("recipient2@example.com");
        * @access  public
        * @return  void
        */
        public void to(List<string> to)
        {
            arguments.Add("recipients", to);
        }

        /**
        * Setting Recipients. Accepted formats for $to are (see API docs):
        *  multiple recipients who can't see each other email addresses with variables
        *     Dictionary<string, Dictionary<string, string>> recipients = new Dictionary<string, Dictionary<string, string>>();
        *     recipients.Add("recipient1@example.com", new Dictionary<string, string>(){{"variable_1", "value"},{"variable_2", "value"} });
        *     recipients.Add("recipient2@example.com", new Dictionary<string, string>() { { "variable_1", "value" }, { "variable_2", "value" } });
        * @access  public
        * @return  void
        */
        public void to(Dictionary<string, Dictionary<string,string>> to)
        {
            arguments.Add("recipients", to);
        }

        /**
        * Setting message body. One or both text and html content can be set using:
        *   Dictionary<string, string>() {
        *    'text/html'   => 'HTML Content,
        *    'text/plain'  => 'Plain Text Content
        *   };
        *
        * @access  public
        * @return  void
        */
        public void message(Dictionary<string, string> message)
        {
            if (arguments.ContainsKey("content"))
            {
                arguments["content"] = message;
            }
            else
            {
                arguments.Add("content", message);
            }
        }

        public void attach(string filename)
        {
        }

        /**
        * Setting PostageApp project template
        *
        * @access  public
        * @return  void
        */
        public void template(string template)
        {
            if (arguments.ContainsKey("template"))
            {
                arguments["template"] = template;
            }
            else
            {
                arguments.Add("template", template);
            }
        }

        /**
        * Setting  message variables
        *
        * @access  public
        * @return  void
        */
        public void variables(Dictionary<string, string> variables)
        {
            if (arguments.ContainsKey("variables"))
            {
                arguments["variables"] = variables;
            }
            else
            {
                arguments.Add("variables", variables);
            }
        }

        /**
        * Content that gets sent in the API call
        *
        * @access  public
        * @return  array
        */
        public Dictionary<string, object> payload(string _uid = null)
        {
            if (_uid == null)
            {
                _uid = GenerateUID(DateTime.Now);
            }
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"api_key", api_key},
                {"uid", _uid},
                {"arguments", arguments}
            };
            if(!string.IsNullOrEmpty(recipient_override))
            {
                payload.Add("recipient_override", recipient_override);
            }
            return payload;
        }

        /**
        * Send Email message via PostageApp
        *
        * @url: http://help.postageapp.com/kb/api/send_message
        * @access:  public
        * @return  Response object
        */
        public Response send()
        {

            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());
            return request("send_message.json", json);
        }

        /**
        * Get Account Info
        * @desc: Provides information about the account.
        * @url: http://help.postageapp.com/kb/api/get_account_info
        * @access:  public
        * @return:  Response object
        */
        public Response get_account_info()
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());

	        return request("get_account_info.json", json);
        }

        /**
        * Get Message Receipt
        * @desc: Confirm that message with a particular UID exists
        * @params:  _uid - unique email identifier of the message
        * @url: http://help.postageapp.com/kb/api/get_message_receipt
        * @access:  public
        * @return:  Response object
        */
        public Response get_message_receipt(string _uid)
        {
	        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload(_uid));

	        return request("get_message_receipt.json", json);
        }

        /**
        * Get Message Transmissions
        * @desc: To get data on individual recipients' delivery and open status, 
        * 		you can pass a particular message UID and receive a JSON encoded
        * 		set of data for each recipient within that message.
        * @params:  _uid - unique email identifier of the message
        * @url: http://help.postageapp.com/kb/api/get_message_transmissions
        * @access:  public
        * @return:  Response object
        */
        public Response get_message_transmissions(string _uid)
        {
	        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload(_uid));

	        return request("get_message_transmissions.json", json);
        }

        /**
        * Get Messages
        * @desc: Gets a list of all message UIDs within your project, for subsequent
        * use in collection statistics or open rates.
        * @url: http://help.postageapp.com/kb/api/get_messages
        * @access:  public
        * @return:  Response object
        */
        public Response get_messages()
        {
	        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());

	        return request("get_messages.json", json);
        }

        /**
        * Get Method List
        * @desc: Get a list of all available api methods.
        * @url: http://help.postageapp.com/kb/api/get_method_list
        * @access:  public
        * @return:  Response object
        */
        public Response get_method_list()
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());

	        return request("get_method_list.json", json);
        }

        /**
        * Get Metrics
        * @desc: Gets data on aggregate delivery and open status for a project,
        * 		broken down by current hour, current day, current week, current 
        * 		month with the previous of each as a comparable.
        * @url: http://help.postageapp.com/kb/api/get_metrics
        * @access:  public
        * @return:  Response object
        */
        public Response get_metrics()
        {
	        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());

	        return request("get_metrics.json", json);
        }

        /**
        * Get Project Info
        * @desc: Provides information about the project. e.g. urls, transmissions, users.
        * @url: http://help.postageapp.com/kb/api/get_project_info
        * @access:  public
        * @return:  Response object
        */
        public Response get_project_info()
        {
	        var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());

	        return request("get_project_info.json", json);
        }

        /**
        * Returns the raw JSON request
        *
        * @access  public
        * @return  json string
        */
        public string showRequest()
        {
            string protocol = secure ? "https" : "http";
            string uri = protocol + "://" + host + "/v.1.0/";

            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(payload());
            return uri + " " + json;
        }

        private Response request(string api_method, string content)
        {
            string protocol = secure ? "https" : "http";
            string uri = protocol + "://" + host + "/v.1.0/" + api_method;
            string result = string.Empty;

            ASCIIEncoding encoding = new ASCIIEncoding();
            StreamWriter paStream = null;

            HttpWebRequest paRequest = (HttpWebRequest)WebRequest.Create(uri);
            paRequest.ContentLength = content.Length;
            paRequest.ContentType = "application/json";
            paRequest.UserAgent = "POSTAGEAPP-C# " + version; 
            paRequest.Method = "POST";

            try
            {
                paStream = new StreamWriter(paRequest.GetRequestStream());
                paStream.Write(content);
            }
            catch (Exception paEx)
            {
                return new Response(paEx);
            }
            finally
            {
                paStream.Close();
            }

            try
            {
                HttpWebResponse paResponse = (HttpWebResponse)paRequest.GetResponse();
                using (StreamReader sr = new StreamReader(paResponse.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    sr.Close();

                    //get response detail
                    return new Response(result);
                }

            }
            catch (Exception paEx)
            {
                return new Response(paEx);
            }
        }

        private static string GenerateUID(DateTime dateTime)
        {
            var date = (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(date.ToString());
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(data)).Replace("-", "");
        }

    }

    public class Response
    {
        public string uid { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string json { get; set; }
        public Dictionary<string, object> data { get; set; }

        public Response(string _json)
        {
            json = _json;
            Dictionary<string, object> fullResponse = deserialize(json);

            Dictionary<string, object> response = (Dictionary<string, object>)fullResponse["response"];
            if (response.ContainsKey("uid"))
            {
                uid = (string)response["uid"];
            }
            if (response.ContainsKey("status"))
            {
                status = (string)response["status"];
            }

            data = (Dictionary<string, object>)fullResponse["data"];
        }

        public Response(Exception _json)
        {
            message = _json.ToString();
        }

        public Dictionary<string, object> deserialize(string _json)
        {
            var jsonDeserializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jsonDeserializer.Deserialize<Dictionary<string, object>>(_json);
        }
    }

}