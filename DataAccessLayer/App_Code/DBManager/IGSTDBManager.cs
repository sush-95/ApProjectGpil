using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBModel;
using DataAccessLayer.App_Code.ViewModel;
using System.Data;


namespace DataAccessLayer.App_Code.DBManager
{
   public  interface IGSTDBManager
    {

       
       
        #region TBL_ProcessInstances
        TBL_ProcessInstances GetProcessInstanceByProcessId(string processId);
        TBL_ProcessInstances GetProcessInstanceByInstanceId(long processInstanceId);
        TBL_ProcessInstances GetProcessInstanceByChildId(long processInstanceId);
        void CompleteProcess(long processInstanceId, string nextStateId);
        void CleanInstance(long processInstanceId);
        void CreateInvoiceInstance(string fileName, TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceDetails tblProcessInstanceDetails,
            string invoiceBillProcess, string initialState, string key, string invoicePath);
        void NextInstance(TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceData tblProcessInstanceData, TBL_ProcessInstanceDetails tblProcessInstanceDetails, string pdfData, string nextStateId);
        void NextInstance(TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceData tblProcessInstanceData, TBL_ProcessInstanceDetails tblProcessInstanceDetails, string pdfData, string nextStateId,long childInstanceId);
        void NextInstance(TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceData tblProcessInstanceData, TBL_ProcessInstanceDetails tblProcessInstanceDetails, string pdfData, string nextStateId, long childInstanceId, string errorMessage);
        void CompleteProcess(long processInstanceId);
        void ClearLockedInstances(string processID);
        void UpdateProcessInstance(long processInstanceId);
        void CreateApPdfInstance(string ApPdfProcess, string initialState, string Metadata, TBL_ProcessInstances tblProcessInstance);
        #endregion

        #region TBL_ProcessInstanceDetails
        TBL_ProcessInstanceDetails GetProcessInstanceDetail(long processInstanceId);
        TBL_ProcessInstanceDetails GetProcessInstanceDetailByState(long processInstanceId,string stateId);
        TBL_ProcessInstanceDetails GetProcessInstanceDetailBySequenceId(long processInstanceId, int sequenceId);
        TBL_ProcessInstanceDetails AddProcessInstanceDetails(long processInstanceId, int sequenceId, string stateId, bool isCompleted, DateTime createTS);
        long AddProcessInstance(string processId, long? parentProcessInstanceId);
        void UpdateProcessInstanceDetails(long processInstanceId,bool isComplete);
        void UpdateProcessInstanceDetailsCreateTS(long processInstanceId);
        #endregion

        #region TBL_ProcessInstanceData
        void AddProcessInstanceData(TBL_ProcessInstanceData processInstanceData);
        void UpdateProcessInstanceData(TBL_ProcessInstanceData processInstanceDataUpd);
        TBL_ProcessInstanceData GetProcessInstanceData(long processInstanceId);
        TBL_ProcessInstanceData GetProcessInstanceDataBySequence(long processInstanceId,int sequenceId);
        TBL_ProcessInstanceData GetProcessInstanceDataByChildId(long processInstanceId, string metaData);
        #endregion

        #region TBL_MessageTracker
        void AddMessageToTracker(long processInstanceId, string requestJson, int sequence, string messageId);
        TBL_MessageTracker GetMessageTrackerDetails(string messageId);
        void UpdateMessageTrackerForBotId(string messageId, string botId,string status);
        void UpdateMessageTrackerForResponse(string messageId, string responseJson,string status);
        #endregion

        #region TBL_BotInfo
        List<BotDetails> GetAvailableBotList(string processId);
        List<TBL_BotInfo> GetBotList();
        #endregion

        #region OtherTableSetting
        List<TBL_Process_Frequency> GetAllProcessFrequency();
        TBL_ProcessExecution_Settings GetProcessExecutionSetting(long processInstanceID, string ProcessId);
        TBL_ProcessExecution_Settings GetProcessExecutionSet(string ProcessId);
        void AddProcessExecutionSetting(string ProcessId);
        #endregion

        #region GPIL ERP DB

        List<GP_Airline_Gst_Party_Map> GET_GP_Airline_Gst_Party_Map(string airlineGSTNumber, string gPIGSTNumber);
        List<GP_Gst_Doc_Type_Map> GET_GP_Gst_Doc_Type_Map(string documentTypeInPDF);
        List<GP_Gpi_Gst_OU_Map> GET_GP_Gpi_Gst_OU_Map(string gPIGSTNumber);
        List<GP_Gst_Sac_Code_Map> GET_GP_Gst_Sac_Code_Map(string sacCode);
        List<GP_Gst_Tax_Rate_Map> GET_GP_Gst_Tax_Rate_Map(string gpilGstNumber, string taxType, string ratePercentage);
        List<GP_Gst_Tax_Type_Map> GET_GP_Gst_Tax_Type_Map(string pdfTaxType);
        List<GP_Becon_Inv_Rcon> GET_GP_Becon_Inv_Rcon(string ticketNumber);

        #endregion

        #region RCON DB

        List<RCON_Tr_Reconciliation> GET_RCON_Reconciliation_List(string rnNum, string invoiceNum, string ticketNum);
        RCON_Rn_Od_Rqst GET_RCON_RN_Check_List(string rnNum);
        RCON_Party_List GET_RCON_Party_List(string partyName);
        RCON_VoucherType_List GET_RCON_VoucherType_List(string voucherType);

        #endregion

        #region TBL_InvoiceDetail

        void AddInvoiceDetail(TBL_InvoiceDetail invoiceDetail);
        List<TBL_InvoiceDetail> GetInvoiceList();

        #endregion

       

    }
}
