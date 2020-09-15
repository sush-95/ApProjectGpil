using AccountPaybleProcessor;
using DataAccessLayer.App_Code.DBManager;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountPaybleConsole
{
    class ProgramAp
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            int x = 3;
            while (x>0)
            {
                try
                {
                    AccountPableManager initiator = new AccountPableManager(new AirlineGSTDBManager());
                    initiator.ExecuteProcess();
                   // System.Threading.Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                x--;
            }
        }
    }
}
