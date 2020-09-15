using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InitiatorService.App_Code;
using DataAccessLayer.App_Code.DBManager;
using NLog;

namespace InitiatorService
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            //int monthsApart = 12 * (DateTime.Now.Year-DateTime.Now.AddMonths(-1).Year ) +  DateTime.Now.Month-DateTime.Now.AddMonths(-2).Month;
            //Console.WriteLine(monthsApart.ToString());
            try
            {
                //while (1 == 1)
                {
                    logger.Info("InitiatorService Started");
                    IGSTDBManager dbManager = new AirlineGSTDBManager();
                    ProcessInitiatorManager manager = new ProcessInitiatorManager(dbManager);
                    manager.ExecuteInitiator();
                    System.Threading.Thread.Sleep(5000);
                    logger.Info("InitiatorService Completed");
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
