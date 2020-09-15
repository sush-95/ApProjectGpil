using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Common.App_Code.QueueMessage
{
    public class QMessageStatus
    {
        public QMessageStatus() { }
        public QMessageStatus(string statusJSON)
        {
            JObject pgmJObj = JObject.Parse(statusJSON);
            Value = pgmJObj[JSON.Tags.Message.Status.Value].ToString();
            Description = pgmJObj[JSON.Tags.Message.Status.Description].ToString();
        }

        public string Value { get; set; }
        public string Description { get; set; }
        public string JSONString
        {
            get
            {
                string json = "{";
                json += Utils.JElement(JSON.Tags.Message.Status.Value, Value) + ",";
                json += Utils.JElement(JSON.Tags.Message.Status.Description, Description) + "}";
                return json;
            }
        }
    }
}
