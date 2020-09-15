using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBModel;
using DataAccessLayer.App_Code.DBManager;
using NLog;

namespace  LineProcessorConsole.App_Code
{
    class ProcessorManager:IDisposable
    {
        IGSTDBManager dbManager;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ProcessorManager(IGSTDBManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public void Dispose()
        {
           
        }

        public Processor GetProcessInstance(TBL_ProcessInstances tblProcessInstance)
        {
            switch(tblProcessInstance.ProcessId)
            {              
                
                case Common.Constants.Process.ProcessID.APLineItemProcess:
                    return new LineItemProcessor(tblProcessInstance, dbManager);
                default:
                    throw new Exception("Process Id Not Implemented");
            }
        }
    }
}
