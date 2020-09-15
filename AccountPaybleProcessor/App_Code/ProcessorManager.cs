using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBModel;
using DataAccessLayer.App_Code.DBManager;
using NLog;

namespace  AccountPaybleProcessor.App_Code
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
                case Common.Constants.Process.ProcessID.APProcess:
                    return new InvoiceProcessor(tblProcessInstance, dbManager);
                case Common.Constants.Process.ProcessID.APPDFExtractProcess:
                    return new PDFExctractProcessor(tblProcessInstance, dbManager);
              
                default:
                    throw new Exception("Process Id Not Implemented");
            }
        }
    }
}
