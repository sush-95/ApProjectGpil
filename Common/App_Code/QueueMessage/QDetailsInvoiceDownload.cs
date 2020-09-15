using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.App_Code.QueueMessage
{
    public class QDetailsInvoiceDownload : QDetails
    {

        #region Public Variables
        public string FromDate { get; set; }
        public string EndDate { get; set; }
        public string Url { get; set; }


        #endregion

        public QDetailsInvoiceDownload(string fromDate, string endDate, string url)
        {
            FromDate = fromDate;
            EndDate = endDate;
            Url = url;
        }

        protected override void CreateFromJSON(string json)
        {
            //JObject pgmJObj = JObject.Parse(json);
            //JObject jObjectDetails = (JObject)pgmJObj[JSON.Tags.Message.Details.Key];
            //FromDate = jObjectDetails[JSON.Tags.Invoice.ProgramName].ToString();
            //FromDate = jObjectDetails[JSON.Tags.Invoice.ProgramName].ToString();
            //FromDate = jObjectDetails[JSON.Tags.Invoice.ProgramName].ToString();
        }

        protected override string DetailString()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>() { { "FromDate", FromDate }, { "EndDate", EndDate }, { "Url", Url } };
            var json = JsonConvert.SerializeObject(dict);
            return json;
        }

        protected override string DetailStringForBeat()
        {
            return "";
        }
    }
}
