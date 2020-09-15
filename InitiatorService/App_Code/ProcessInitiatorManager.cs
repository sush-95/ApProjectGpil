using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.App_Code.DBManager;
using DataAccessLayer.DBModel;
using Common;

namespace InitiatorService.App_Code
{
    class ProcessInitiatorManager
    {
        IGSTDBManager dbManager;
        public ProcessInitiatorManager(IGSTDBManager dbManager)
        {
            this.dbManager = dbManager;
        }
        public void ExecuteInitiator()
        {
            List<TBL_Process_Frequency> frequencyList = dbManager.GetAllProcessFrequency();
            foreach(var frequency in frequencyList)
            {
                AddProcessExecution(frequency);
            }
        }

        private bool DailyProcess(TBL_Process_Frequency frequency, TBL_ProcessExecution_Settings processSetting)
        {
            if ((DateTime.Now - processSetting.StartDate).Days >= 1) return true;
            else return false;

        }
        private bool HourlyProcess(TBL_Process_Frequency frequency, TBL_ProcessExecution_Settings processSetting)
        {
            if ((DateTime.Now - processSetting.StartDate).Hours >= 1) return true;
            else return false;
        }
        private bool MonthlyProcess(TBL_Process_Frequency frequency, TBL_ProcessExecution_Settings processSetting)
        {
            int monthsApart = 12 * (DateTime.Now.Year - processSetting.StartDate.Year) + DateTime.Now.Month - processSetting.StartDate.Month;
            if (monthsApart >= 1) return true;
            else return false;
        }
        private bool WeeklyProcess(TBL_Process_Frequency frequency, TBL_ProcessExecution_Settings processSetting)
        {
            if ((DateTime.Now - processSetting.StartDate).Days >= 1) return true;
            else return false;
        }

        private void AddProcessExecution(TBL_Process_Frequency frequency)
        {
            TBL_ProcessExecution_Settings processSetting = dbManager.GetProcessExecutionSet(frequency.ProcessId);
            if(processSetting ==null)
            {
                AddProcess(frequency, processSetting);
            }
            else if(processSetting.IsComplete!=null && processSetting.IsComplete.Value)
            {
                bool isProcess = false;
                switch (frequency.FrequenceId)
                {
                    case "Daily":
                        isProcess = DailyProcess(frequency, processSetting);
                        break;
                    case "Hourly":
                        isProcess = HourlyProcess(frequency, processSetting);
                        break;
                    case "Monthly":
                        isProcess = MonthlyProcess(frequency, processSetting);
                        break;
                    case "Weekly":
                        isProcess = WeeklyProcess(frequency, processSetting);
                        break;
                }
                if(isProcess)
                    AddProcess(frequency, processSetting);
            }

        }

        private void AddProcess(TBL_Process_Frequency frequency,TBL_ProcessExecution_Settings processSetting)
        {
            dbManager.AddProcessExecutionSetting(frequency.ProcessId);
            long processInstanceId = dbManager.AddProcessInstance(frequency.ProcessId, null);
            dbManager.AddProcessInstanceDetails(processInstanceId, 0, Utils.GetProcessInitialState(frequency.ProcessId), false, DateTime.Now);
        }

    }
}
