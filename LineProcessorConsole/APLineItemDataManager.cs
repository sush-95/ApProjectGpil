using AccountPaybleProcessor.App_Code;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using LineProcessorConsole.App_Code;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineProcessorConsole
{
    class APLineItemDataManager
    {

        IGSTDBManager dbManager;
        TBL_ProcessInstances tblProcessInstance;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public APLineItemDataManager(IGSTDBManager dbManager)
        {
            this.dbManager = dbManager;
        }
        public void ExecuteProcess()
        {
            try
            {
                logger.Info("ExecuteProcess Started");
                List<long> instanceProcessed = new List<long>();
                dbManager = new AirlineGSTDBManager();
                dbManager.ClearLockedInstances(Common.Constants.Process.ProcessID.APLineItemProcess);
                tblProcessInstance = dbManager.GetProcessInstanceByProcessId(Common.Constants.Process.ProcessID.APLineItemProcess);
                while (tblProcessInstance != null)
                {
                    instanceProcessed.Add(tblProcessInstance.ProcessInstanceId);
                    ManageProcess();
                    tblProcessInstance = dbManager.GetProcessInstanceByProcessId(Common.Constants.Process.ProcessID.APLineItemProcess);
                }
                instanceProcessed.ForEach(x => dbManager.CleanInstance(x));
                instanceProcessed.Clear();
            }
            catch (Exception ex)
            {

            }
        }

        private void ManageProcess()
        {
            try
            {
                logger.Info("ManageProcess Started");
                using (ProcessorManager processManager = new ProcessorManager(dbManager))
                {
                    Processor process = processManager.GetProcessInstance(tblProcessInstance);
                    process.ExecuteState();
                }
                logger.Info("ManageProcess Completed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
