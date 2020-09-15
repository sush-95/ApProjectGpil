using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.App_Code.ViewModel
{
    class BotViewModel
    {
    }

    public class BotDetails
    {
        public string BotId { get; set; }
        public int? MaxBotQueue { get; set; }
        public string RequestQueueName { get; set; }
        public string ResponseQueueName { get; set; }
        public string MachineName { get; set; }
    }
}
