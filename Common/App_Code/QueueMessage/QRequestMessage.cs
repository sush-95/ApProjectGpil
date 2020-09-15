using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;

namespace Common.App_Code.QueueMessage
{
    public class QRequestMessage
    {
        public QRequestMessage(QMessageHeader header)
        {
            _header = header;
        }

        public QRequestMessage(QMessageHeader header, QDetails detail, TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceDetails tblProcessInstanceDetails, IGSTDBManager dbManager, string serverQueue)
        {
            _tblProcessInstanceDetails = tblProcessInstanceDetails;
            _header = header;
            _details = detail;
            _dbManager = dbManager;
            _tblProcessInstance = tblProcessInstance;
            _serverQueue = serverQueue;

        }
        
        #region Private Variables
        private QMessageHeader _header;
        private QDetails _details;
        TBL_ProcessInstances _tblProcessInstance;
        TBL_ProcessInstanceDetails _tblProcessInstanceDetails;
        IGSTDBManager _dbManager;
        string _serverQueue;
        #endregion
        
        public string JSONString
        {
            get
            {
                string json = "{" + _header.JSONString + "," + _details.JSONString + "}";
                return json;
            }
        }

        public string JSONStringForBeat
        {
            get
            {
                string json = "{" + _header.JSONString + "," + _details.JSONStringForBeat + "}";
                return json;
            }
        }

        public void PostMessage(string nextState)
        {
            UpdateProcessInstanceDetails();
            _tblProcessInstanceDetails = _dbManager.AddProcessInstanceDetails(_tblProcessInstance.ProcessInstanceId, _tblProcessInstanceDetails.SequenceId, nextState, false, DateTime.Now);
            AddMessageToTracker();
            SendMsgToBotQueue();
        }

        public void PostMessagee(string nextState, int SequenceId)
        {
            UpdateProcessInstanceDetails();
            _tblProcessInstanceDetails = _dbManager.AddProcessInstanceDetails(_tblProcessInstance.ProcessInstanceId, SequenceId, nextState, false, DateTime.Now);
            AddMessageToTracker();
            SendMsgToBotQueue();
        }

        public void PostMessagetoBot()
        {

            UpdateProcessInstanceDetails();
            //_tblProcessInstanceDetails = _dbManager.AddProcessInstanceDetails(_tblProcessInstance.ProcessInstanceId, _tblProcessInstanceDetails.SequenceId, nextState, false, DateTime.Now);
            AddMessageToTracker();
            SendMsgToBotQueue();
        }

        public void PostMessagetoBotForBeat(string nextState, int SequenceId)
        {

            UpdateProcessInstanceDetails();
            _tblProcessInstanceDetails = _dbManager.AddProcessInstanceDetails(_tblProcessInstance.ProcessInstanceId, SequenceId, nextState, false, DateTime.Now);
            AddMessageToTrackerForBeat();
            SendMsgToBotQueueForBeat();
        }

        public void UpdateProcessInstanceDetails()
        {
            _tblProcessInstanceDetails.IsCompleted = true;
            _dbManager.UpdateProcessInstanceDetails(_tblProcessInstanceDetails.ProcessInstanceId, true);
        }

        public void UpdateProcessInstanceDetailsForBeat()
        {
            _tblProcessInstanceDetails.IsCompleted = false;
            _dbManager.UpdateProcessInstanceDetails(_tblProcessInstanceDetails.ProcessInstanceId, false);
        }

        public void AddMessageToTracker()
        {
            _dbManager.AddMessageToTracker(_tblProcessInstanceDetails.ProcessInstanceId, JSONString, _tblProcessInstanceDetails.SequenceId, _header.MessageId);
        }

        public void SendMsgToBotQueue()
        {
            SendReciveMsg sendMessage = new SendReciveMsg();
            sendMessage.SendMsgToBotQueue(JSONString, _serverQueue);
        }

        public void AddMessageToTrackerForBeat()
        {
            _dbManager.AddMessageToTracker(_tblProcessInstanceDetails.ProcessInstanceId, JSONStringForBeat, _tblProcessInstanceDetails.SequenceId, _header.MessageId);
        }

        public void SendMsgToBotQueueForBeat()
        {
            SendReciveMsg sendMessage = new SendReciveMsg();
            sendMessage.SendMsgToBotQueue(JSONStringForBeat, _serverQueue);
        }
    }
}
