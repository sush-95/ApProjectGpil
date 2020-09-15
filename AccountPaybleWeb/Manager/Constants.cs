using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountPaybleWeb.Manager
{
    public class Constants
    {
        public static class PODetailInvoiceStatus
        {
            public const string Extracted = "AP-POSTAT-000";
            public const string Error = "AP-POSTAT-001";
            public const string PendingForApproval = "AP-POSTAT-005";
            public const string Approved = "AP-POSTAT-010";
            public const string Rejected = "AP-POSTAT-999";

        }
        public static string GetStat(string stat)
        {
            if (!string.IsNullOrEmpty(stat))
            {
                stat = stat.Contains(",") ? stat.Split(',')[0] : stat;
                switch (stat.Trim())
                {
                    case "AP-POSTAT-001":
                        return "AP-POSTAT-001,Error";

                    case "AP-POSTAT-005":
                        return "AP-POSTAT-005,Pending For Approval";
                    case "AP-POSTAT-999":
                        return "AP-POSTAT-999,Rejected";
                    default:
                        return "AP-POSTAT-010,Processed";

                }
            }
            else
            {
                return string.Empty;
            }

        }
    }
}