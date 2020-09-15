using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constants;

namespace Common.App_Code.QueueMessage
{
    public abstract class QDetails
    {

        public QDetails() { }

        public QDetails(string json)
        {
            CreateFromJSON(json);
        }

        #region Public Properties
        public string JSONString
        {
            get
            {
                return Utils.Quote(JSON.Tags.Message.Details.Key) + ":" + DetailString();
            }
        }

        public string JSONStringForBeat
        {
            get
            {
                return Utils.Quote(JSON.Tags.Message.Details.Key) + ":" + DetailStringForBeat();
            }
        }

        public string GetJSONString
        {
            get
            {
                //return Utils.JElement(JSON.Tags.Message.Details.Key, DetailString());
                return "{\"" + JSON.Tags.Message.Details.Key + "\":" + DetailString() + "}";
            }
        }

        public string GetJSONStringForBeat
        {
            get
            {
                //return Utils.JElement(JSON.Tags.Message.Details.Key, DetailString());
                return "{\"" + JSON.Tags.Message.Details.Key + "\":" + DetailStringForBeat() + "}";
            }
        }
        #endregion

        #region Protected Functions
        protected abstract string DetailString();
        protected abstract string DetailStringForBeat();
        protected abstract void CreateFromJSON(string json);
        #endregion
    }
}
