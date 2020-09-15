using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBModel;
using DataAccessLayer.App_Code.ViewModel;
using System.Data;
using DataAccessLayer.App_Code.DBManager;
using NLog;

namespace DataAccessLayer.App_Code.DBManager
{
    public class AirlineGSTDBManager : IDisposable, IGSTDBManager
    {
        #region Variables

        RCONDBManager rCONDBManager;

        #endregion

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TBL_ProcessInstances GetProcessInstanceByProcessId(string processId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstances tblProcessInstance = entities.USP_AllocateInstance(System.Environment.MachineName, processId).FirstOrDefault();
                return tblProcessInstance;
            }
        }

        public void CleanInstance(long processInstanceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                entities.USP_CleanInstance(processInstanceId);
            }
        }

        public void CompleteProcess(long processInstanceId, string nextStateId)
        {
            try
            {
                logger.Info("CompleteProcess Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstanceData processData = entities.TBL_ProcessInstanceData.Where(x => x.ProcessInstanceId == processInstanceId)
                        .OrderByDescending(x => x.MetaDataSequenceId).FirstOrDefault();
                    if (processData != null)
                    {
                        processData.IsFinal = true;
                        entities.SaveChanges();
                    }

                    TBL_ProcessInstanceDetails processDetails = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId)
                        .OrderByDescending(x => x.SequenceId).FirstOrDefault();
                    if (processDetails != null)
                    {
                        processDetails.IsCompleted = true;
                        entities.SaveChanges();
                        AddProcessInstanceDetails(processInstanceId, processDetails.SequenceId, nextStateId, true, DateTime.Now);
                    }

                    TBL_ProcessInstances processInstance = entities.TBL_ProcessInstances.Where(x => x.ProcessInstanceId == processInstanceId).FirstOrDefault();
                    if (processInstance != null)
                    {
                        processInstance.IsCompleted = true;
                        entities.SaveChanges();
                    }
                }
                logger.Info("CompleteProcess Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void CompleteProcess(long processInstanceId)
        {
            try
            {
                logger.Info("CompleteProcess Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    List<TBL_ProcessInstanceData> processDataList = entities.TBL_ProcessInstanceData.Where(x => x.ProcessInstanceId == processInstanceId).ToList();
                    foreach (TBL_ProcessInstanceData processData in processDataList)
                    {
                        processData.IsProcessed = true;
                        entities.SaveChanges();
                    }

                    List<TBL_ProcessInstanceDetails> processDetailList = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId).ToList();
                    foreach (TBL_ProcessInstanceDetails processDetail in processDetailList)
                    {
                        processDetail.IsCompleted = true;
                        entities.SaveChanges();
                    }

                    TBL_ProcessInstances processInstance = entities.TBL_ProcessInstances.Where(x => x.ProcessInstanceId == processInstanceId).First();
                    processInstance.IsCompleted = true;
                    entities.SaveChanges();

                }
                logger.Info("CompleteProcess Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public TBL_ProcessInstances GetProcessInstanceByInstanceId(long processInstanceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                return entities.TBL_ProcessInstances.Where(x => x.ProcessInstanceId == processInstanceId).First();
            }
        }

        public TBL_ProcessInstances GetProcessInstanceByChildId(long processInstanceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceData tblProcessData = entities.TBL_ProcessInstanceData.Where(x => x.ChildInstanceId == processInstanceId).OrderByDescending(x => x.ProcessInstanceId).First();
                return entities.TBL_ProcessInstances.Where(x => x.ProcessInstanceId == tblProcessData.ProcessInstanceId).First();
            }
        }

        public TBL_ProcessInstanceDetails GetProcessInstanceDetail(long processInstanceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceDetails tblProcessInstanceDetails = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId).OrderByDescending(x => x.SequenceId).FirstOrDefault();
                return tblProcessInstanceDetails;
            }
        }

        public TBL_ProcessInstanceDetails GetProcessInstanceDetailByState(long processInstanceId, string stateId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceDetails tblProcessInstanceDetails = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId && x.StateId == stateId).OrderByDescending(x => x.SequenceId).FirstOrDefault();
                return tblProcessInstanceDetails;
            }
        }

        public TBL_ProcessInstanceDetails GetProcessInstanceDetailBySequenceId(long processInstanceId, int sequenceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceDetails tblProcessInstanceDetails = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId && x.SequenceId == sequenceId).OrderByDescending(x => x.SequenceId).FirstOrDefault();
                return tblProcessInstanceDetails;
            }
        }

        public TBL_ProcessInstanceDetails AddProcessInstanceDetails(long processInstanceId, int sequenceId, string stateId, bool isCompleted, DateTime createTS)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceDetails processDetailUpd = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId).
                    OrderByDescending(x => x.SequenceId).FirstOrDefault();
                if (processDetailUpd != null)
                {
                    UpdateProcessInstanceDetails(processDetailUpd.ProcessInstanceId, true);
                }

                TBL_ProcessInstanceDetails processDetail = new TBL_ProcessInstanceDetails();
                processDetail.ProcessInstanceId = processInstanceId;
                processDetail.StateId = stateId;
                processDetail.SequenceId = sequenceId+1;
                processDetail.IsCompleted = isCompleted;
                processDetail.CreateTS = createTS;

                entities.TBL_ProcessInstanceDetails.Add(processDetail);
                entities.SaveChanges();
                return processDetail;
            }
        }

        public void AddProcessInstanceData(TBL_ProcessInstanceData processInstanceData)
        {
            try
            {
                logger.Info("AddProcessInstanceData Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstanceData processData = new TBL_ProcessInstanceData();
                    processData.ProcessInstanceId = processInstanceData.ProcessInstanceId;
                    processData.SequenceId = processInstanceData.SequenceId;
                    processData.MetaData = processInstanceData.MetaData;
                    processData.MetaDataSequenceId = processInstanceData.MetaDataSequenceId;
                    processData.CreatedTS = processInstanceData.CreatedTS;
                    processData.IsProcessed = processInstanceData.IsProcessed;
                    processData.IsFinal = processInstanceData.IsFinal;
                    processData.ChildInstanceId = processInstanceData.ChildInstanceId;
                    processData.ErrorMessage = processInstanceData.ErrorMessage;

                    entities.TBL_ProcessInstanceData.Add(processData);
                    entities.SaveChanges();
                }
                logger.Info("AddProcessInstanceData Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateProcessInstanceData(TBL_ProcessInstanceData processInstanceDataUpd)
        {
            try
            {
                logger.Info("UpdateProcessInstanceData Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstanceData processData = entities.TBL_ProcessInstanceData.Where(x => x.ProcessInstanceId == processInstanceDataUpd.ProcessInstanceId
                    && x.SequenceId == processInstanceDataUpd.SequenceId && x.MetaDataSequenceId == processInstanceDataUpd.MetaDataSequenceId
                    ).First();

                    processData.MetaData = processInstanceDataUpd.MetaData;
                    processData.ChildInstanceId = processInstanceDataUpd.ChildInstanceId;
                    processData.MessageId = processInstanceDataUpd.MessageId;
                    processData.ErrorMessage = processInstanceDataUpd.ErrorMessage;
                    entities.SaveChanges();
                }
                logger.Info("UpdateProcessInstanceData Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public long AddProcessInstance(string processId, long? parentProcessInstanceId)
        {
            long processInstanceID = 0;
            try
            {
                logger.Info("AddProcessInstance Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    processInstanceID = Convert.ToInt64(entities.USP_Insert_ProcessInstance(processId, parentProcessInstanceId).FirstOrDefault());
                }
                logger.Info("AddProcessInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return processInstanceID;
        }

        public TBL_ProcessInstanceData GetProcessInstanceData(long processInstanceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceData tblProcessInstanceData = entities.TBL_ProcessInstanceData.Where(x => x.ProcessInstanceId == processInstanceId).OrderByDescending(x => x.MetaDataSequenceId).FirstOrDefault();
                return tblProcessInstanceData;
            }
        }

        public TBL_ProcessInstanceData GetProcessInstanceDataBySequence(long processInstanceId, int sequenceId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceData tblProcessInstanceData = entities.TBL_ProcessInstanceData.Where(x => x.ProcessInstanceId == processInstanceId && x.SequenceId == sequenceId).OrderByDescending(x => x.MetaDataSequenceId).FirstOrDefault();
                return tblProcessInstanceData;
            }
        }

        public TBL_ProcessInstanceData GetProcessInstanceDataByChildId(long processInstanceId, string ticketNumber)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessInstanceData tblProcessInstanceData = entities.TBL_ProcessInstanceData.Where(x => x.ChildInstanceId == processInstanceId && x.MetaData.Contains(ticketNumber)).FirstOrDefault();
                return tblProcessInstanceData;
            }
        }

        public void UpdateProcessInstanceData(long processInstanceId)
        {
            try
            {
                logger.Info("UpdateProcessInstanceData Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstanceData tblProcessInstanceData = entities.TBL_ProcessInstanceData.Where(x => x.ProcessInstanceId == processInstanceId).OrderByDescending(x => x.MetaDataSequenceId).FirstOrDefault();
                    // return tblProcessInstanceData;
                }
                logger.Info("UpdateProcessInstanceData Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateProcessInstance(long processInstanceId)
        {
            try
            {
                logger.Info("UpdateProcessInstance Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstances tblProcessInstance = entities.TBL_ProcessInstances.Where(x => x.ProcessInstanceId == processInstanceId).FirstOrDefault();
                    tblProcessInstance.CreatedTS = DateTime.Now;
                    entities.SaveChanges();
                    // return tblProcessInstanceData;
                }
                logger.Info("UpdateProcessInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateProcessInstanceDetails(long processInstanceId, bool isComplete)
        {
            try
            {
                logger.Info("UpdateProcessInstanceDetails Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstanceDetails tblProcessInstanceDetails = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId).OrderByDescending(x => x.SequenceId).FirstOrDefault();
                    tblProcessInstanceDetails.IsCompleted = isComplete;
                    entities.SaveChanges();
                }
                logger.Info("UpdateProcessInstanceDetails Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateProcessInstanceDetailsCreateTS(long processInstanceId)
        {
            try
            {
                logger.Info("UpdateProcessInstanceDetails Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessInstanceDetails tblProcessInstanceDetails = entities.TBL_ProcessInstanceDetails.Where(x => x.ProcessInstanceId == processInstanceId).OrderBy(x => x.SequenceId).FirstOrDefault();
                    tblProcessInstanceDetails.CreateTS = DateTime.Now;
                    entities.SaveChanges();
                }
                logger.Info("UpdateProcessInstanceDetails Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void AddMessageToTracker(long processInstanceId, string requestJson, int sequence, string messageId)
        {
            try
            {
                logger.Info("AddMessageToTracker Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_MessageTracker tracker = new TBL_MessageTracker();

                    tracker.MessageID = messageId;
                    tracker.ProcessInstanceID = processInstanceId;
                    tracker.RequestJsonData = requestJson;
                    tracker.Sequence = sequence;
                    tracker.Status = "0";
                    tracker.StartTime = DateTime.Now;
                    entities.TBL_MessageTracker.Add(tracker);
                    entities.SaveChanges();
                }
                logger.Info("AddMessageToTracker Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateMessageTrackerForBotId(string messageId, string botId, string status)
        {
            try
            {
                logger.Info("UpdateMessageTrackerForBotId Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_MessageTracker tracker = entities.TBL_MessageTracker.Where(x => x.MessageID == messageId).First();
                    tracker.IncrementTimeout = 60;
                    tracker.BotID = botId;
                    tracker.Status = status;

                    entities.SaveChanges();
                }
                logger.Info("UpdateMessageTrackerForBotId Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateMessageTrackerForResponse(string messageId, string responseJson, string status)
        {
            try
            {
                logger.Info("UpdateMessageTrackerForResponse Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_MessageTracker tracker = entities.TBL_MessageTracker.Where(x => x.MessageID == messageId).First();
                    tracker.ResponseJsonData = responseJson;
                    tracker.Status = status;
                    tracker.EndTime = DateTime.Now;
                    entities.SaveChanges();
                }
                logger.Info("UpdateMessageTrackerForResponse Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void NextInstance(TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceData tblProcessInstanceData, TBL_ProcessInstanceDetails tblProcessInstanceDetails, string pdfData, string nextStateId)
        {
            try
            {
                logger.Info("NextInstance Started");
                int sequenceId = tblProcessInstanceDetails.SequenceId;
                AddProcessInstanceDetails(tblProcessInstance.ProcessInstanceId, sequenceId, nextStateId, false, DateTime.Now);

                TBL_ProcessInstanceData tblprocessData = new TBL_ProcessInstanceData();
                tblprocessData.ProcessInstanceId = tblProcessInstance.ProcessInstanceId;
                tblprocessData.MetaData = pdfData;
                tblprocessData.SequenceId = sequenceId + 1;
                tblprocessData.CreatedTS = DateTime.Now;
                tblprocessData.MetaDataSequenceId = tblProcessInstanceData.MetaDataSequenceId + 1;

                AddProcessInstanceData(tblprocessData);
                logger.Info("NextInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void NextInstance(TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceData tblProcessInstanceData, TBL_ProcessInstanceDetails tblProcessInstanceDetails, string pdfData, string nextStateId, long childInstanceId)
        {
            try
            {
                logger.Info("NextInstance Started");
                int sequenceId = tblProcessInstanceDetails.SequenceId;
                AddProcessInstanceDetails(tblProcessInstance.ProcessInstanceId, sequenceId, nextStateId, false, DateTime.Now);

                TBL_ProcessInstanceData tblprocessData = new TBL_ProcessInstanceData();
                tblprocessData.ProcessInstanceId = tblProcessInstance.ProcessInstanceId;
                tblprocessData.MetaData = pdfData;
                tblprocessData.SequenceId = sequenceId + 1;
                tblprocessData.CreatedTS = DateTime.Now;
                tblprocessData.MetaDataSequenceId = tblProcessInstanceData.MetaDataSequenceId + 1;
                tblprocessData.ChildInstanceId = childInstanceId;
                AddProcessInstanceData(tblprocessData);
                logger.Info("NextInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void NextInstance(TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceData tblProcessInstanceData, TBL_ProcessInstanceDetails tblProcessInstanceDetails, string pdfData, string nextStateId, long childInstanceId, string errorMessage)
        {
            try
            {
                logger.Info("NextInstance Started");
                int sequenceId = tblProcessInstanceDetails.SequenceId;
                AddProcessInstanceDetails(tblProcessInstance.ProcessInstanceId, sequenceId, nextStateId, false, DateTime.Now);

                TBL_ProcessInstanceData tblprocessData = new TBL_ProcessInstanceData();
                tblprocessData.ProcessInstanceId = tblProcessInstance.ProcessInstanceId;
                tblprocessData.MetaData = pdfData;
                tblprocessData.SequenceId = sequenceId + 1;
                tblprocessData.CreatedTS = DateTime.Now;
                tblprocessData.MetaDataSequenceId = tblProcessInstanceData.MetaDataSequenceId + 1;
                tblprocessData.ChildInstanceId = childInstanceId;
                tblprocessData.ErrorMessage = errorMessage;
                AddProcessInstanceData(tblprocessData);
                logger.Info("NextInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void ClearLockedInstances(string processID)
        {
            try
            {
                using (GPILEntities entities = new GPILEntities())
                {
                    entities.USP_CleanAllLockdedInstance(processID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public List<BotDetails> GetAvailableBotList(string processId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                var filterBotList = (from info in entities.TBL_BotInfo
                                     join tracker in entities.TBL_MessageTracker
                                     on info.BotId equals tracker.BotID into bot
                                     from tracker in bot.DefaultIfEmpty()
                                     where info.IsActive == true
                                     group new { info, tracker } by new { info.BotId } into grp
                                     where grp.Count() < grp.Select(x => x.info.MaxBotQueue.Value).FirstOrDefault()
                                     select new BotDetails { BotId = grp.Key.BotId }
                                  ).ToList();
                //select new 
                //{
                //    key = grp.Key,
                //    cnt = grp.Count()
                //}).ToList();

                var avlBotList = filterBotList.Select(x => x.BotId).ToList();

                var activeBotList = (from info in entities.TBL_BotInfo
                                     join process in entities.TBL_BotProcessAssigment
                                     on info.BotId equals process.BotId
                                     join avl in avlBotList on info.BotId equals avl
                                     where process.ProcessId == processId && info.IsActive == true
                                     select new BotDetails
                                     {
                                         BotId = info.BotId,
                                         MaxBotQueue = info.MaxBotQueue,
                                         RequestQueueName = info.RequestQueueName,
                                         ResponseQueueName = info.ResponseQueueName,
                                         MachineName = info.MachineName
                                     }).ToList();


                return activeBotList;
            }
        }

        public TBL_MessageTracker GetMessageTrackerDetails(string messageId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_MessageTracker tracker = entities.TBL_MessageTracker.Where(x => x.MessageID == messageId).FirstOrDefault();
                return tracker;
            }
        }

        public List<TBL_Process_Frequency> GetAllProcessFrequency()
        {
            using (GPILEntities entities = new GPILEntities())
            {
                return entities.TBL_Process_Frequency.ToList();
            }
        }

        public TBL_ProcessExecution_Settings GetProcessExecutionSetting(long ProcessInstanceId, string ProcessId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessExecution_Settings prcExecutin = entities.TBL_ProcessExecution_Settings.Where(x => x.ProcessId == ProcessId && x.ProcessInstanceId == ProcessInstanceId).OrderByDescending(x => x.Id).FirstOrDefault();
                return prcExecutin;
            }
        }

        public TBL_ProcessExecution_Settings GetProcessExecutionSet(string ProcessId)
        {
            using (GPILEntities entities = new GPILEntities())
            {
                TBL_ProcessExecution_Settings prcExecutin = entities.TBL_ProcessExecution_Settings.Where(x => x.ProcessId == ProcessId).OrderByDescending(x => x.Id).FirstOrDefault();
                return prcExecutin;
            }
        }

        public void AddProcessExecutionSetting(string processId)
        {
            try
            {
                logger.Info("AddProcessExecutionSetting Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    TBL_ProcessExecution_Settings process = new TBL_ProcessExecution_Settings();
                    process.StartDate = DateTime.Now;
                    process.ProcessId = processId;
                    process.IsComplete = false;

                    entities.TBL_ProcessExecution_Settings.Add(process);
                    entities.SaveChanges();
                }
                logger.Info("AddProcessExecutionSetting Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public List<TBL_BotInfo> GetBotList()
        {
            using (GPILEntities entities = new GPILEntities())
            {
                return entities.TBL_BotInfo.Where(x => x.IsActive == true).ToList();
            }
        }

        public void CreateInvoiceInstance(string fileName, TBL_ProcessInstances tblProcessInstance, TBL_ProcessInstanceDetails tblProcessInstanceDetails,
            string invoiceBillProcess, string initialState, string key, string invoicePath)
        {
            try
            {
                logger.Info("CreateInvoiceInstance Started");
                long processInstanceId = AddProcessInstance(invoiceBillProcess, tblProcessInstance.ParentProcessInstanceId);
                tblProcessInstance = GetProcessInstanceByInstanceId(processInstanceId);
                AddProcessInstanceDetails(processInstanceId, 0, initialState, false, DateTime.Now);

                TBL_ProcessInstanceData lastProcessData = GetProcessInstanceData(processInstanceId);

                string metaData = "{\"" + key + "\":{\"" + invoicePath + "\":\"" + fileName + "\"}}";
                tblProcessInstanceDetails = GetProcessInstanceDetailByState(processInstanceId, initialState);

                TBL_ProcessInstanceData processData = new TBL_ProcessInstanceData();
                processData.ProcessInstanceId = processInstanceId;
                processData.SequenceId = tblProcessInstanceDetails.SequenceId;
                processData.MetaDataSequenceId = lastProcessData == null ? 0 : lastProcessData.MetaDataSequenceId + 1; ;
                processData.MetaData = metaData;
                processData.CreatedTS = DateTime.Now;
                processData.IsProcessed = false;
                // processData.IsFinal = false;
                AddProcessInstanceData(processData);
                logger.Info("CreateInvoiceInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        public void CreateApPdfInstance(string ApPdfProcess, string initialState,string Metadata,TBL_ProcessInstances tblProcessInstance)
        {
            try
            {
                logger.Info("CreateInvoiceInstance Started");
                long processInstanceId = AddProcessInstance(ApPdfProcess, tblProcessInstance.ProcessInstanceId);
                tblProcessInstance = GetProcessInstanceByInstanceId(processInstanceId);
                var detail=AddProcessInstanceDetails(processInstanceId, 0, initialState, false, DateTime.Now);
                TBL_ProcessInstanceData processData = new TBL_ProcessInstanceData();
                processData.ProcessInstanceId = processInstanceId;
                processData.SequenceId = detail.SequenceId;
                processData.MetaDataSequenceId = 0;
                processData.MetaData = Metadata;
                processData.CreatedTS = DateTime.Now;
                processData.IsProcessed = false;              
                // processData.IsFinal = false;
                AddProcessInstanceData(processData);
                logger.Info("CreateInvoiceInstance Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void AddInvoiceDetail(TBL_InvoiceDetail invoiceDetail)
        {
            try
            {
                logger.Info("AddInvoiceDetail Started");
                using (GPILEntities entities = new GPILEntities())
                {
                    entities.TBL_InvoiceDetail.Add(invoiceDetail);
                    entities.SaveChanges();
                }
                logger.Info("AddInvoiceDetail Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public List<TBL_InvoiceDetail> GetInvoiceList()
        {
            using (GPILEntities entities = new GPILEntities())
            {
                return entities.TBL_InvoiceDetail.ToList();
            }
        }

        #region GPILDBLayer

        public List<GP_Airline_Gst_Party_Map> GET_GP_Airline_Gst_Party_Map(string AirlineGSTNumber, string GPIGSTNumber)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Airline_Gst_Party_Map> partMapList = eRPDBManager.gstPartyMapList;
                List<GP_Airline_Gst_Party_Map> partMap = partMapList.Where(x => x.AIRLINE_GST_REG_NUM == AirlineGSTNumber && x.GPI_GST_REGN_NO == GPIGSTNumber).ToList();
                return partMap;
            }
        }

        public List<GP_Gst_Doc_Type_Map> GET_GP_Gst_Doc_Type_Map(string documentTypeInPDF)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Gst_Doc_Type_Map> typeMapList = eRPDBManager.gstDocTypeMapList;
                List<GP_Gst_Doc_Type_Map> typeMap = typeMapList.Where(x => x.DOCUMENT_TYPE_IN_PDF == documentTypeInPDF.ToUpper()).ToList();
                if (typeMap.Count == 0)
                {
                    if (documentTypeInPDF.ToUpper().Contains("TAX"))
                    {
                        typeMap = typeMapList.Where(x => x.DOCUMENT_TYPE_IN_PDF == "TAX INVOICE").ToList();
                    }
                    if (documentTypeInPDF.ToUpper().Contains("CREDIT"))
                    {
                        typeMap = typeMapList.Where(x => x.DOCUMENT_TYPE_IN_PDF == "CREDIT NOTE").ToList();
                    }
                    if (documentTypeInPDF.ToUpper().Contains("DEBIT"))
                    {
                        typeMap = typeMapList.Where(x => x.DOCUMENT_TYPE_IN_PDF == "DEBIT NOTE").ToList();
                    }
                }

                return typeMap;
            }
        }

        public List<GP_Gpi_Gst_OU_Map> GET_GP_Gpi_Gst_OU_Map(string GPIGSTNumber)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Gpi_Gst_OU_Map> gpiMapList = eRPDBManager.gstOUMapList;
                List<GP_Gpi_Gst_OU_Map> gpiMap = gpiMapList.Where(x => x.GPI_GST_REGN_NUMBER == GPIGSTNumber).ToList();
                return gpiMap;
            }
        }

        public List<GP_Gst_Sac_Code_Map> GET_GP_Gst_Sac_Code_Map(string sacCode)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Gst_Sac_Code_Map> codeMapList = eRPDBManager.gstSacCodeMapList;
                List<GP_Gst_Sac_Code_Map> codeMap = codeMapList.Where(x => x.SAC_CODE == sacCode && x.GST_ENTRY == DBConstants.Yes).ToList();
                return codeMap;
            }
        }

        public List<GP_Gst_Tax_Rate_Map> GET_GP_Gst_Tax_Rate_Map(string gpilGstNumber, string taxType, string ratePercentage)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Gst_Tax_Rate_Map> rateMapList = eRPDBManager.gstTaxRateMapList;
                double ratePer = Math.Round(Convert.ToDouble(ratePercentage), 2, MidpointRounding.AwayFromZero);
                List<GP_Gst_Tax_Rate_Map> rateMap = rateMapList.Where(x => x.GPI_GST_REG_NUMBER == gpilGstNumber && x.TAX_TYPE_IN_PDF == taxType && x.TAX_RATE_PERCENTAGE == ratePer.ToString()).ToList();
                return rateMap;
            }
        }

        public List<GP_Gst_Tax_Type_Map> GET_GP_Gst_Tax_Type_Map(string pdfTaxType)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Gst_Tax_Type_Map> typeMapList = eRPDBManager.taxTypeMapList;
                List<GP_Gst_Tax_Type_Map> typeMap = typeMapList.Where(x => x.Tax_Type_in_PDF == pdfTaxType).ToList();
                return typeMap;
            }
        }

        public List<GP_Becon_Inv_Rcon> GET_GP_Becon_Inv_Rcon(string ticketNumber)
        {
            using (ERPDBManager eRPDBManager = new ERPDBManager())
            {
                List<GP_Becon_Inv_Rcon> beconRconpList = eRPDBManager.gstBeconRconList;
                List<GP_Becon_Inv_Rcon> rconList = beconRconpList.Where(x => x.TICKET_NO == ticketNumber).ToList();
                return rconList;
            }
        }

        public List<RCON_Tr_Reconciliation> GET_RCON_Reconciliation_List(string refNum, string invoiceNum, string ticketNum)
        {
            if (rCONDBManager == null)
            {
                RCONDBManager rCONDBManager = new RCONDBManager();
            }
            List<RCON_Tr_Reconciliation> rconList = rCONDBManager.rconRNList;
            List<RCON_Tr_Reconciliation> returnRconList = rconList.Where(x => x.REQUEST_NUMBER == refNum && x.AGENCY_INVNO == invoiceNum && x.TICKET_NO == ticketNum).ToList();
            return returnRconList;
        }

        public RCON_Rn_Od_Rqst GET_RCON_RN_Check_List(string refNum)
        {
            if (rCONDBManager == null)
            {
                RCONDBManager rCONDBManager = new RCONDBManager();
            }
            List<RCON_Rn_Od_Rqst> rconList = rCONDBManager.rconCheckList;
            RCON_Rn_Od_Rqst returnRconList = rconList.Where(x => x.TA_NUMBER == refNum).FirstOrDefault();
            return returnRconList;
        }

        public RCON_Party_List GET_RCON_Party_List(string partyName)
        {
            if (rCONDBManager == null)
            {
                RCONDBManager rCONDBManager = new RCONDBManager();
            }
            List<RCON_Party_List> rconList = rCONDBManager.rconPartyList;
            RCON_Party_List returnPartyList = rconList.Where(x => x.PARTY_NAME == partyName).FirstOrDefault();
            return returnPartyList;
        }

        public RCON_VoucherType_List GET_RCON_VoucherType_List(string voucherType)
        {
            if (rCONDBManager == null)
            {
                RCONDBManager rCONDBManager = new RCONDBManager();
            }
            List<RCON_VoucherType_List> rconList = rCONDBManager.rconVoucherTypeList;
            RCON_VoucherType_List returnVoucherTypeList = rconList.Where(x => x.INV_TYPE == voucherType).FirstOrDefault();
            return returnVoucherTypeList;
        }

        #endregion

        public void Dispose()
        {

        }


    }
}
