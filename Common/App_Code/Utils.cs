using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Utils
    {

        public static string GetProcessInitialState(string processId)
        {
            switch (processId)
            {
                case Constants.Process.ProcessID.AirlineBillProcessAirIndia:
                    return Constants.Process.States.AirlineBillProcessAirIndia.InitialState;
                case Constants.Process.ProcessID.AirlineBillProcessJetAirWays:
                    return Constants.Process.States.AirlineBillProcessJetAirWays.InitialState;
                case Constants.Process.ProcessID.AirlineBillProcessEmail:
                    return Constants.Process.States.AirlineBillProcessEmail.InitialState;
                case Constants.Process.ProcessID.InvoiceBillProcess:
                    return Constants.Process.States.AirlineBillProcessEmail.InitialState;
                default:
                    throw new Exception("Process Not Implemented");
                    
            }
        }

        public static string GetProcessFinalState(string processId)
        {
            switch (processId)
            {
                case Constants.Process.ProcessID.AirlineBillProcessAirIndia:
                    return Constants.Process.States.AirlineBillProcessAirIndia.FinalState;
                case Constants.Process.ProcessID.AirlineBillProcessJetAirWays:
                    return Constants.Process.States.AirlineBillProcessJetAirWays.FinalState;
                case Constants.Process.ProcessID.AirlineBillProcessEmail:
                    return Constants.Process.States.AirlineBillProcessEmail.FinalState;
                case Constants.Process.ProcessID.InvoiceBillProcess:
                    return Constants.Process.States.AirlineBillProcessEmail.FinalState;
                default:
                    throw new Exception("Process Not Implemented");

            }
        }

        public static string Quote(string item)
        {
            return "\"" + item + "\"";
        }

        public static string JElement(string key, string value)
        {
            return Quote(key) + ":" + Quote(value);
        }

        public static string GetJsonValueByTag(string json, string tag)
        {
            JObject jObject = JObject.Parse(json);
            return jObject[tag].ToString();
        }

        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string ReadJsonTagValue(string jsonString, string tagParent, string tagName)
        {
            string tagValue = "";
            JObject jObject = JObject.Parse(jsonString);
            JObject jObjectHeader = (JObject)jObject[tagParent];
            tagValue = jObjectHeader[tagName].ToString();
            return tagValue;
        }

        public static string ReadJsonTagValue(string jsonString, string tagName)
        {
            string tagValue = "";
            JObject jObject = JObject.Parse(jsonString);
            
            tagValue = jObject[tagName].ToString();
            return tagValue;
        }

        public static string GetTextFromPDF(string path)
        {
            StringBuilder text = new StringBuilder();



            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }

            return text.ToString();
        }

    }
}
