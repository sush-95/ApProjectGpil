using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.App_Code.QueueMessage
{
    public class QDetailsExcelUpload : QDetails
    {
        #region Public Variables
        public object Json { get; set; }
        #endregion

        public QDetailsExcelUpload(object json)
        {
            Json = json;
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
            var json = JsonConvert.SerializeObject(Json);
            return json;
        }

        protected override string DetailStringForBeat()
        {
            return "";
        }
    }
}
