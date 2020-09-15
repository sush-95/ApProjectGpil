using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineProcessorConsole.App_Code
{
    abstract class Processor
    {

        public string stateId;
        public string StateId { get { return stateId; } }
        public abstract void ExecuteState();
    }
}
