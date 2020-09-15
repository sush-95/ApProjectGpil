using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Constants;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common;

namespace Common.App_Code.QueueMessage
{
    public class QMessageHeader
    {
        public QMessageHeader(string type, string processID, string commandID)
        {
            Initialize(type, processID, commandID);
            _messageID = Guid.NewGuid().ToString();
        }

        public QMessageHeader(string type, string processID, string commandID, string messageID)
        {
            Initialize(type, processID, commandID);
            _messageID = messageID;
        }

        public QMessageHeader(string headerJSON)
        {
            JObject pgmJObj = JObject.Parse(headerJSON);
            _type = pgmJObj[JSON.Tags.Message.Header.Type].ToString();
            _processID = pgmJObj[JSON.Tags.Message.Header.ProcessID].ToString();
            _CommandID = pgmJObj[JSON.Tags.Message.Header.CommandID].ToString();
            _messageID = pgmJObj[JSON.Tags.Message.Header.MessageId].ToString();
            MessageTS = pgmJObj[JSON.Tags.Message.Header.MessageTS].ToString();
            string retrySeq = pgmJObj[JSON.Tags.Message.Header.RetrySequence].ToString();
            RetrySequence = Convert.ToInt32(retrySeq);
            BOTID = pgmJObj[JSON.Tags.Message.Header.BOTID].ToString();
          //  CheckPoint = Convert.ToInt32(pgmJObj[JSON.Tags.Message.Header.CheckPoint].ToString());
           // FinalStatus = pgmJObj[JSON.Tags.Message.Header.FinalStatus].ToString();
        }

        #region Private Functions
        private void Initialize(string type, string processID, string commandID)
        {
            _type = type;
            _processID = processID;
            _CommandID = commandID;

        }
        #endregion

        #region Private Variables
        private string _type;
        private string _processID;
        private string _CommandID;
        private string _messageID;
        #endregion

        #region Public Properties
        public string Type { get { return _type; } }
        public string ProcessID { get { return _processID; } }
        public string CommandID { get { return _CommandID; } }
        public string MessageId { get { return _messageID; } }
        public string MessageTS { get; set; }
        public int RetrySequence { get; set; }
        public string BOTID { get; set; }
        public int CheckPoint { get; set; }
        public string FinalStatus { get; set; }

        public string JSONString
        {
            get
            {
                string json = "{";
                json += Utils.JElement(JSON.Tags.Message.Header.Type, _type) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.ProcessID, _processID) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.CommandID, _CommandID) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.MessageId, _messageID) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.MessageTS, MessageTS) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.RetrySequence, RetrySequence.ToString()) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.BOTID, BOTID) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.CheckPoint, CheckPoint.ToString()) + ",";
                json += Utils.JElement(JSON.Tags.Message.Header.FinalStatus, FinalStatus) + "}";

                return Utils.Quote(JSON.Tags.Message.Header.Key) + ":" + json;
            }
        }
        #endregion


    }
}

