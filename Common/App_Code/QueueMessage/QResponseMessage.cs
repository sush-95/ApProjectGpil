using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constants;

namespace Common.App_Code.QueueMessage
{
    public class QResponseMessage
    {

        public QResponseMessage(string responseJSON)
        {
            MessageJSON = responseJSON;

            JObject jObject = JObject.Parse(responseJSON);

            string headerJSON = jObject[JSON.Tags.Message.Header.Key].ToString();
            Header = new QMessageHeader(headerJSON);

            DetailJSON = jObject[JSON.Tags.Message.Details.Key].ToString();

            string statusJSON = jObject[JSON.Tags.Message.Status.Key].ToString();
            Status = new QMessageStatus(statusJSON);
        }

        #region Public Properties
        /// <summary>
        /// Header Part of the Response JSON
        /// </summary>
        public QMessageHeader Header { get; set; }

        /// <summary>
        /// Detail Part of the Response JSON
        /// </summary>
        public string DetailJSON { get; set; }

        /// <summary>
        /// Status Part of the Response JSON
        /// </summary>
        public QMessageStatus Status { get; set; }
        public string MessageJSON { get; set; }
        #endregion
    }
}
