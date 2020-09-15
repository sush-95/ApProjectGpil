using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    namespace Constants
    {
        public static class BusinessRules
        {
            public static class GP_Airline_Gst_Party_Map
            {

            }

            public class GP_Gst_Tax_Type_Map
            {

            }

            public class GP_Gpi_Gst_OU_Map
            {

            }

            public class GP_Gst_Tax_Rate_Map
            {

            }

            public class GP_Gst_Doc_Type_Map
            {

            }

            public static class GP_Gst_Sac_Code_Map
            {

            }
            //public static class Ap_Business_Rules_failed
            //{
            //    public const string AP_XXGP_RPA_PO_VIEW_Match_Option = "AP-BL-STAT-000";
            //    public const string IGST = "IGST";
            //    public const string SGST = "SGST";
            //    public const string UTGST = "UTGST";
            //}
            //public static class Ap_Business_Rules_Success
            //{
            //    public const string AP_XXGP_RPA_PO_VIEW_Match_Option = "AP-BL-STAT-001";
            //    public const string IGST = "IGST";
            //    public const string SGST = "SGST";
            //    public const string UTGST = "UTGST";
            //}

        }
        public static class PODetailInvoiceStatus
        {
            public const string Extracted = "AP-POSTAT-000";
            public const string Error = "AP-POSTAT-001";
            public const string PendingForApproval = "AP-POSTAT-005";
            public const string Approved = "AP-POSTAT-010";
            public const string Rejected = "AP-POSTAT-999";

        }
        public static class APViewMApped
        {
            public const string PaymentReference = "XXGP_RPA_INV_PAY_REF";
            public const string PoApproved = "XXGP_RPA_PO_VIEW_00";
            public const string OtherData = "XXGP_RPA_PO_VIEW_05";
            public const string GStNumber = "XXGP_RPA_PO_VIEW_01";
            public const string TwoWayPoMap = "XXGP_RPA_2WAY_PO_VIEW_00";
            public const string TwoWayGSTMatch = "XXGP_RPA_2WAY_PO_VIEW_01";
            public const string TwoWayManualMatchAmount = "XXGP_RPA_2WAY_PO_VIEW_05";
            public const string TwoWayManualMatchTotalAmount = "XXGP_RPA_2WAY_PO_VIEW_010";
            public const string ThreeWayPoMap = "XXGP_RPA_3WAY_PO_VIEW";
            public const string ThreeWayAmountMap = "XXGP_RPA_3WAY_PO_VIEW_01";
            public const string ThreeWayGSTMap = "XXGP_RPA_3WAY_PO_VIEW_05";
        }
        public static class APMatchOption
        {
            public const string TwoWay = "2-way";
            public const string Threeway = "3-way";

        }
        public static class PDF
        {
            public const string CGST = "CGST";
            public const string IGST = "IGST";
            public const string SGST = "SGST";
            public const string UTGST = "UTGST";
        }

        public static class BotMicroServices
        {
            public const string DownloadInvoice = "DownloadInvoice";
            public const string CreateTransaction = "CreateTransaction";
            public const string DownloadExcel = "DownloadExcel";
            public const string DownloadFinalExcel = "DownloadFinalExcel";
            public const string DownloadCustomerRoute = "DownloadCustomerRoute";
            public const string UploadExcel = "UploadExcel";
            public const string BeatPlanning = "BeatPlanning";
        }

        public static class Process
        {
            public static class ProcessID
            {
                public const string AirlineBillProcessEmail = "AirlineBillProcessEmail";
                public const string AirlineBillProcessAirIndia = "AirlineBillProcessAirIndia";
                public const string AirlineBillProcessJetAirWays = "AirlineBillProcessJetAirWays";
                public const string InvoiceBillProcess = "InvoiceBillProcess";
                public const string RCPExccelProcess = "RCPExcelProcess";
                public const string RCONProcess = "RCONProcess";
                public const string APProcess = "APProcess";
                public const string APPDFExtractProcess = "APPDFProcess";
                public const string APLineItemProcess = "APLineItemProcess";
            }

            public static class States
            {
                public static class AirlineBillProcessEmail
                {

                    public const string InitialState = "ABP-EML-0000";
                    public const string Failed = "ABP-EML-0010";
                    public const string FinalState = "ABP-EML-9999";
                }

                public static class AirlineBillProcessAirIndia
                {

                    public const string InitialState = "ABP-AIN-0000";
                    public const string Failed = "ABP-AIN-0010";
                    public const string Requested = "ABP-AIN-0005";
                    public const string FinalState = "ABP-AIN-9999";
                }

                public static class AirlineBillProcessJetAirWays
                {

                    public const string InitialState = "ABP-JET-0000";
                    public const string Failed = "ABP-JET-0010";
                    public const string Requested = "ABP-JET-0005";
                    public const string FinalState = "ABP-JET-9999";
                }
                public static class APProcess
                {
                    public const string InitialState = "AP-INV-000";
                    public const string InvoiceDownloaded = "AP-INV-005";
                    public const string InvoiceDownloadFailed = "AP-INV-005-ERR";
                    public const string Complete = "AP-INV-999";
                }
                public static class APPDFProcess
                {
                    public const string InitialState = "AP-PDF-000";
                    public const string DataExtracted = "AP-PDF-010";
                    public const string DataExtractionFailed = "AP-PDF-010-ERR";
                    public const string BusinessRuleFailed = "AP-PDF-015-ERR";
                    public const string BusinessRuleException = "AP-PDF-020-ERR";
                    public const string Complete = "AP-PDF-999";
                }
                public static class APLineIetmProcess
                {
                    public const string InitialSate = "AP-LITEM-000";
                }
                public static class APInvoiceStatus
                {
                    public const string PdfExctracted = "AP-INVSTAT-000";
                    public const string ErrorState = "AP-INVSTAT-005";
                    public const string PendingForApproval = "AP-INVSTAT-010";
                    public const string Processed = "AP-INVSTAT-015";
                    public const string Rejected = "AP-INVSTAT-020";
                }
                public static class InvoiceBillProcess
                {
                    public const string InitialState = "ABP-INV-0000";
                    public const string DataExtracted = "ABP-INV-0005";
                    public const string RCONValidation = "ABP-INV-0006";
                    public const string DataExtractionFailed = "ABP-INV-0010";
                    public const string BusinessRulesExecuted = "ABP-INV-0015";
                    public const string BusinessRulesExecutionFailed = "ABP-INV-0020";
                    public const string CreateTransactionRequested = "ABP-INV-0025";
                    public const string CreateTransactionFailed = "ABP-INV-0030";
                    public const string TransactionCreated = "ABP-INV-0035";
                    public const string Complete = "ABP-INV-9999";
                }

                public static class RCPExccelProcess
                {
                    public const string InitialState = "RCP-0000";
                    public const string MailAttachmentDownload = "RCP-0005";
                    public const string DMSAttachmentDownload = "RCP-0010";
                    public const string DMSAttachmentDownloadFailed = "RCP-0015";
                    public const string CustomerRouteDownload = "RCP-0020";
                    public const string CustomerRouteDownloadCompleted = "RCP-0025";
                    public const string CustomerRouteDownloadFailed = "RCP-0030";
                    public const string ApplyBusinessRule = "RCP-0035";
                    public const string PrepareRawExcel = "RCP-0040";
                    public const string PrepareSalesmanExcel = "RCP-0045";
                    public const string PrepareBeatExcel = "RCP-0050";
                    public const string PrepareSalesmanRouteExcel = "RCP-0055";
                    public const string PrepareOutletExcel = "RCP-0060";
                    public const string PrepareCPCategoryExcel = "RCP-0065";
                    public const string PrepareCustomerRouteExcel = "RCP-0070";
                    public const string PrepareBeatPlanningExcel = "RCP-0075";
                    public const string PrepareORMExcel = "RCP-0080";
                    public const string DMSExcelUpload = "RCP-0085";
                    public const string DMSExcelUploadRequested = "RCP-0090";
                    public const string DMSExcelUploadCompleted = "RCP-0095";
                    public const string DMSExcelUploadFailed = "RCP-0100";
                    public const string PrepareDMSBeatPanning = "RCP-0105";
                    public const string PrepareDMSBeatPanningRequested = "RCP-0110";
                    public const string PrepareDMSBeatPanningCompeted = "RCP-0115";
                    public const string PrepareDMSBeatPanningFailed = "RCP-0120";
                    public const string FinalDMSExcelDownload = "RCP-0125";
                    public const string FinalDMSExcelDownloadFailed = "RCP-0130";
                    public const string ErrorInProcess = "RCP-0135";
                    public const string Complete = "RCP-9999";
                }

                public static class RCONProcess
                {

                    public const string InitialState = "RCON-0000";
                    public const string AttachmentDownloaded = "RCON-0005";
                    public const string ValidateExcel = "RCON-0010";
                    public const string ValidateExcelFailed = "RCON-0010-ERR";
                    public const string EnterRNNumber = "RCON-0015";
                    public const string InsertIntoDatabase = "RCON-0020";
                    public const string InsertIntoDatabaseFailed = "RCON-0020-ERR";
                    public const string SendNotification = "RCON-0025";
                    public const string Complete = "RCON-9999";
                }

                public static class CommandID
                {

                }
            }

            public static class Message
            {
                public static class Types
                {
                    public const string Request = "Request";
                    public const string Response = "Response";
                }

                public static class Values
                {
                    public static class Status
                    {
                        public const string Success = "0000";
                        public const string Failed = "0001";

                    }

                    public static class ProgramExecutionStatus
                    {
                        public const string InProgress = "P";
                        public const string Failed = "F";
                        public const string Success = "S";
                    }
                }
            }

            public static class MessageResponse
            {
                public static class Status
                {
                    public const string Initial = "0";
                    public const string Send = "1";
                    public const string Sucess = "2";
                    public const string Failed = "3";
                }
            }
        }

        public static class JSON
        {
            public static class Tags
            {
                public static class ERP
                {
                    public const string Url = "Url";
                    public const string OperatingUnit = "OperatingUnit";
                    public const string OrganisationName = "OrganisationName";
                    public const string TransactionType = "TransactionType";
                    public const string PartyName = "PartyName";
                    public const string PartySite = "PartySite";
                    public const string InvoiceNumber = "InvoiceNumber";
                    public const string InvoiceDate = "InvoiceDate";
                    public const string ItemClass = "ItemClass";
                    public const string ItemName = "ItemName";
                    public const string Remarks = "Remarks";
                    public const string CGSTTaxType = "CGSTTaxType";
                    public const string SGSTTaxType = "SGSTTaxType";
                    public const string IGSTTaxType = "IGSTTaxType";
                    public const string TaxableAmount = "TaxableAmount";
                    public const string AccessibleValue = "AccessibleValue";
                    public const string CGSTTaxRateCode = "CGSTTaxRateCode";
                    public const string SGSTTaxRateCode = "SGSTTaxRateCode";
                    public const string IGSTTaxRateCode = "IGSTTaxRateCode";
                    public const string Location = "Location";
                    public const string AdjustmentAmount = "AdjustmentAmount";
                }

                public static class Message
                {
                    public static class Header
                    {
                        public const string Key = "Header";
                        public const string Type = "Type";
                        public const string ProcessID = "ProcessID";
                        public const string CommandID = "CommandID";
                        public const string MessageId = "MessageID";
                        public const string MessageTS = "MessageTS";
                        public const string RetrySequence = "RetrySequence";
                        public const string BOTID = "BOTID";
                        public const string CheckPoint = "CheckPoint";
                        public const string FinalStatus = "FinalStatus";
                    }

                    public static class Types
                    {
                        public const string Request = "Request";
                        public const string Response = "Response";
                    }

                    public static class Status
                    {
                        public const string Key = "Status";
                        public const string Value = "Value";
                        public const string Description = "Description";
                    }

                    public static class Details
                    {
                        public const string Key = "Detail";
                        public const string EndDate = "LastDownloadDate";
                        public const string StartDate = "FromDate";
                        public const string Action = "Action";
                        public const string PDF = "PDF";
                        public const string Message = "Message";
                        public const string ERP = "ERP";
                        public const string DMS = "DMS";
                        public const string Excel = "Excel";
                        public const string Status = "Status";
                        public const string ToMailId = "ToMailId";
                        public const string Subject = "Subject";
                        public const string InvoicePath = "InvoicePath";
                        public const string ExcelPath = "FileName";
                        public const string IsFinal = "IsFinal";
                        public const string Data = "Data";
                        public const string SequenceId = "SequenceId";
                        public const string OldFileName = "OldFileName";
                        public const string TownName = "TownName";
                    }
                    //public static class Detail
                    //{
                    //    public const string URL = "URL";
                    //    public const string FromDate = "";
                    //    public const string EndDate = "";
                    //}
                }

                public static class Values
                {
                    public static class Action
                    {
                        public const string ForceComplete = "F";
                        public const string Resubmit = "R";
                        public const string Error = "E";
                        public const string IsFinal = "1";
                        public const string DuplicateEntry = "Duplicate Entry";
                        public const string TaxAmtNull = "Taxable Amount Null";
                        public const string DateBeforeGST = "GST Not Applied";
                        public const string SummaryOfBill = "Summary Of Bill of Supply";
                        public const string InvalidPDF = "Invalid Format";
                        public const string ExERP = "Exception In ERP";
                        public const string GSTDate = "01-Apr-2018";
                        public const string InvalidDate = "Invoice date is prior to 01-APR-2018 so this invoice not updated in ERP";
                        public const string BeaconTicket = "Pending for Beacon Invoice Payment";
                        public const string ExDMS = "Exception In DMS";
                    }
                }
            }
        }

        public static class ExcelFileName
        {
            public const string ExcelFolder = @"D:\Shared\RCPExcelFiles\";
            public const string ExcelFolderToCopy = @"D:\Shared\FormatToBeUsed\";
            public const string ProcessedExcel = @"D:\Shared\Processed\";
            public const string RCPFromSales = "RCPFromSales";
            public const string RCONFinalExcel = "RCONFinalExcel";
            public const string RCONInitialExcel = "RCONInitialExcel";
            public const string FormattedRCPFromSales = "FormattedRCPFromSales.xlsx";
            public const string Salesman = "Salesman.xls";
            public const string Beat = "Beat.xls";
            public const string SalesmanRoute = "SalesmanRoute.xls";
            public const string Outlet = "Outlet.xls";
            public const string CPCategory = "CustomerProductCategory.xls";
            public const string CustomerRoute = "CustomerRoute.xls";
            public const string BeatPlanning = "BeatPlanning.xls";
            public const string OutletRemoval = "OutletRouteRemoval.xls";
        }

        public static class ExcelFileType
        {
            public const string Salesman = "Salesman";
            public const string Beat = "Beat";
            public const string SalesmanRoute = "Salesman Beat Mapping";
            public const string Outlet = "Outlet";
            public const string CPCategory = "Customer Product Category";
            public const string CustomerRoute = "Customer Route";
            public const string OutletRemoval = "Outlet Route Removal";
        }

        public enum BranchCode
        {
            MUM = 111,
            DEL = 108,
            AHM = 113,
            CHD = 106
        }

        public enum RegionCode
        {
            MUMBAI = 02,
            DELHI = 01,
            AHEMDABAD = 03,
            CHANDIGARH = 16
        }
    }

}
