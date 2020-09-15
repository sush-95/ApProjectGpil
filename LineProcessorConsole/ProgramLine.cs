using AccountPaybleProcessor;
using DataAccessLayer.App_Code.DBManager;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineProcessorConsole
{
    class ProgramLine
    {
       public static Logger logger = LogManager.GetCurrentClassLogger();
        public static DateTime start, end;
        static void Main(string[] args)
        {
            start = DateTime.Now;
            int x = 3;
            while (x != 0)
            {
                try
                {
                    APLineItemDataManager initiator = new APLineItemDataManager(new AirlineGSTDBManager());
                    initiator.ExecuteProcess();
                    System.Threading.Thread.Sleep(1000);
                    x--;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            end = DateTime.Now;
            //ProcessLogger.WriteLogger(start, end);
        }
    }
}
